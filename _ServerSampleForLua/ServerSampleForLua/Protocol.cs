using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSampleForLua {
    class Protocol {
        public enum ProtocolNum {
            Connect = 101,
            Exception = 102,
            Disconnect = 103,
            Message = 104
        }
    }
}
