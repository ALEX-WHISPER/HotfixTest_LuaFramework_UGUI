#if ASYNC_MODE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using LuaInterface;
using UObject = UnityEngine.Object;

public class AssetBundleInfo {
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount;

    public AssetBundleInfo(AssetBundle assetBundle) {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 0;
    }
}

namespace LuaFramework {

    public class ResourceManager : Manager {
        string m_BaseDownloadingURL = "";
        string[] m_AllManifest = null;
        AssetBundleManifest m_AssetBundleManifest = null;   //  所有包的依赖关系
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();   //  各资源包所依赖的其它资源包
        Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();   //  已加载的资源包
        Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();   //  对某资源包内的某资源文件的加载请求

        class LoadAssetRequest {
            public Type assetType;
            public string[] assetNames;
            public LuaFunction luaFunc;
            public Action<UObject[]> sharpFunc;
        }

        // Load AssetBundleManifest, which is 'StreamingAssets.manifest'
        public void Initialize(string manifestName, Action initOK) {
            m_BaseDownloadingURL = Util.GetRelativePath();

            //  加载文件：StreamingAssets.manifest
            LoadAsset<AssetBundleManifest>(manifestName, new string[] { "AssetBundleManifest" }, delegate(UObject[] objs) {
                if (objs.Length > 0) {
                    m_AssetBundleManifest = objs[0] as AssetBundleManifest; //  赋值，使 m_AssetBundleManifest 指向 StreamingAssets.manifest
                    m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
                }
                if (initOK != null) initOK();
            });
        }

        //  加载 ab 中的某 asset
        public void LoadPrefab(string abName, string assetName, Action<UObject[]> func) {
            LoadAsset<GameObject>(abName, new string[] { assetName }, func);
        }

        public void LoadPrefab(string abName, string[] assetNames, Action<UObject[]> func) {
            LoadAsset<GameObject>(abName, assetNames, func);
        }

        public void LoadPrefab(string abName, string[] assetNames, LuaFunction func) {
            LoadAsset<GameObject>(abName, assetNames, null, func);
        }

        string GetRealAssetPath(string abName) {
            if (abName.Equals(AppConst.AssetDir)) {
                return abName;
            }
            abName = abName.ToLower();
            if (!abName.EndsWith(AppConst.ExtName)) {
                abName += AppConst.ExtName;
            }
            if (abName.Contains("/")) {
                return abName;
            }
            //string[] paths = m_AssetBundleManifest.GetAllAssetBundles();  产生GC，需要缓存结果
            for (int i = 0; i < m_AllManifest.Length; i++) {
                int index = m_AllManifest[i].LastIndexOf('/');  
                string path = m_AllManifest[i].Remove(0, index + 1);    //字符串操作函数都会产生GC
                if (path.Equals(abName)) {
                    return m_AllManifest[i];
                }
            }
            Debug.LogError("GetRealAssetPath Error:>>" + abName);
            return null;
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        void LoadAsset<T>(string abName, string[] assetNames, Action<UObject[]> action = null, LuaFunction func = null) where T : UObject {
            abName = GetRealAssetPath(abName);

            //  定义一个加载请求
            LoadAssetRequest request = new LoadAssetRequest();
            request.assetType = typeof(T);
            request.assetNames = assetNames;    //  请求加载的资源文件名数组
            request.luaFunc = func;
            request.sharpFunc = action;

            List<LoadAssetRequest> requests = null; //  每次资源加载请求都将请求列表重置

            //  判断当前是否正在加载该资源包，若没有则调用 OnLoadAsset()，此时 requests 已被更新，指向该ab所对应的请求列表
            if (!m_LoadRequests.TryGetValue(abName, out requests)) {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                m_LoadRequests.Add(abName, requests);   //  资源加载请求字典：key:资源包名, value:加载请求
                StartCoroutine(OnLoadAsset<T>(abName)); //  加载指定的资源包
            }

            //  若正在加载，则把当前的加载请求添加进该资源包对应的请求列表中
            else {
                requests.Add(request);
            }
        }

        //  加载 Asset
        IEnumerator OnLoadAsset<T>(string abName) where T : UObject {
            //  从加载表中获取 AssetBundle，若为空，可能是 ab 不存在，也可能由于尚未加载，因此须先进行加载，再判断 ab 是否的确不存在
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);

            if (bundleInfo == null) {
                yield return StartCoroutine(OnLoadAssetBundle(abName, typeof(T)));  //  加载 AssetBundle
                bundleInfo = GetLoadedAssetBundle(abName);  //  获取 AssetBundle

                //  若该资源包的确不存在，则将当前加载命令从请求记录字典中移出，并退出协程
                if (bundleInfo == null) {
                    m_LoadRequests.Remove(abName);
                    Debug.LogError("OnLoadAsset--->>>" + abName);
                    yield break;
                }
            }

            List<LoadAssetRequest> list = null;

            //  如果获取不到对该 ab 的加载请求，将该条记录从字典中移出，然后退出协程
            if (!m_LoadRequests.TryGetValue(abName, out list)) {
                m_LoadRequests.Remove(abName);
                yield break;
            }

            //  list: abName 所对应的请求列表
            for (int i = 0; i < list.Count; i++) {
                string[] assetNames = list[i].assetNames;   //  每条请求对应要加载的资源文件名数组
                List<UObject> result = new List<UObject>();

                AssetBundle ab = bundleInfo.m_AssetBundle;
                for (int j = 0; j < assetNames.Length; j++) {
                    string assetPath = assetNames[j];
                    AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
                    yield return request;
                    result.Add(request.asset);

                    //T assetObj = ab.LoadAsset<T>(assetPath);
                    //result.Add(assetObj);
                }

                //  call backs
                if (list[i].sharpFunc != null) {
                    list[i].sharpFunc(result.ToArray());
                    list[i].sharpFunc = null;
                }
                if (list[i].luaFunc != null) {
                    list[i].luaFunc.Call((object)result.ToArray());
                    list[i].luaFunc.Dispose();
                    list[i].luaFunc = null;
                }
                bundleInfo.m_ReferencedCount++;
            }

            //  该 ab 所有请求处理完后，将其从请求字典中移出，表示对该资源包的加载已经结束
            m_LoadRequests.Remove(abName);
        }

        //  加载 AssetBundle
        IEnumerator OnLoadAssetBundle(string abName, Type type) {
            string url = m_BaseDownloadingURL + abName;

            WWW download = null;
            if (type == typeof(AssetBundleManifest))
                download = new WWW(url);
            else {
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);   //  获取依赖包

                //  如果该包存在对其他包的依赖，则先加载其依赖包
                if (dependencies.Length > 0) {
                    //  更新依赖表
                    m_Dependencies.Add(abName, dependencies);   //  key: assetBundle名，value: 该 assetBundle 所依赖的包名数组

                    //  加载依赖包
                    for (int i = 0; i < dependencies.Length; i++) {
                        string depName = dependencies[i];
                        AssetBundleInfo bundleInfo = null;
                        if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo)) {
                            bundleInfo.m_ReferencedCount++;
                        } else if (!m_LoadRequests.ContainsKey(depName)) {
                            yield return StartCoroutine(OnLoadAssetBundle(depName, type));
                        }
                    }
                }
                //  然后加载主体资源
                download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0);
            }
            yield return download;

            AssetBundle assetObj = download.assetBundle;
            if (assetObj != null) {
                //  更新加载表，当前包已被加载
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName) {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies) {
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null) return null;
            }
            return bundle;
        }

        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false) {
            abName = GetRealAssetPath(abName);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        void UnloadDependencies(string abName, bool isThorough) {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies) {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        void UnloadAssetBundleInternal(string abName, bool isThorough) {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;

            if (--bundle.m_ReferencedCount <= 0) {
                if (m_LoadRequests.ContainsKey(abName)) {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.m_AssetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Debug.Log(abName + " has been unloaded successfully");
            }
        }
    }
}
#else

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaFramework;
using LuaInterface;
using UObject = UnityEngine.Object;

namespace LuaFramework {
    public class ResourceManager : Manager {
        private string[] m_Variants = { };
        private AssetBundleManifest manifest;
        private AssetBundle shared, assetbundle;
        private Dictionary<string, AssetBundle> bundles;

        void Awake() {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize() {
            byte[] stream = null;
            string uri = string.Empty;
            bundles = new Dictionary<string, AssetBundle>();
            uri = Util.DataPath + AppConst.AssetDir;
            if (!File.Exists(uri)) return;
            stream = File.ReadAllBytes(uri);
            assetbundle = AssetBundle.CreateFromMemoryImmediate(stream);
            manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        public T LoadAsset<T>(string abname, string assetname) where T : UnityEngine.Object {
            abname = abname.ToLower();
            AssetBundle bundle = LoadAssetBundle(abname);
            return bundle.LoadAsset<T>(assetname);
        }

        public void LoadPrefab(string abName, string[] assetNames, LuaFunction func) {
            abName = abName.ToLower();
            List<UObject> result = new List<UObject>();
            for (int i = 0; i < assetNames.Length; i++) {
                UObject go = LoadAsset<UObject>(abName, assetNames[i]);
                if (go != null) result.Add(go);
            }
            if (func != null) func.Call((object)result.ToArray());
        }

        /// <summary>
        /// 载入AssetBundle
        /// </summary>
        /// <param name="abname"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string abname) {
            if (!abname.EndsWith(AppConst.ExtName)) {
                abname += AppConst.ExtName;
            }
            AssetBundle bundle = null;
            if (!bundles.ContainsKey(abname)) {
                byte[] stream = null;
                string uri = Util.DataPath + abname;
                Debug.LogWarning("LoadFile::>> " + uri);
                LoadDependencies(abname);

                stream = File.ReadAllBytes(uri);
                bundle = AssetBundle.CreateFromMemoryImmediate(stream); //关联数据的素材绑定
                bundles.Add(abname, bundle);
            } else {
                bundles.TryGetValue(abname, out bundle);
            }
            return bundle;
        }

        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        void LoadDependencies(string name) {
            if (manifest == null) {
                Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = manifest.GetAllDependencies(name);
            if (dependencies.Length == 0) return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            for (int i = 0; i < dependencies.Length; i++) {
                LoadAssetBundle(dependencies[i]);
            }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        string RemapVariantName(string assetBundleName) {
            string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();

            // If the asset bundle doesn't have variant, simply return.
            if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
                return assetBundleName;

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++) {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(m_Variants, curSplit[1]);
                if (found != -1 && found < bestFit) {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            if (bestFitIndex != -1)
                return bundlesWithVariant[bestFitIndex];
            else
                return assetBundleName;
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        void OnDestroy() {
            if (shared != null) shared.Unload(true);
            if (manifest != null) manifest = null;
            Debug.Log("~ResourceManager was destroy!");
        }
    }
}
#endif
