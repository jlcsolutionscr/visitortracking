using System.Collections.Generic;

namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class MessageData
    {
        public string MethodName { get; set; }
        public object Entity { get; set; }

        public Dictionary<string,object> Parameters { get; set; }
    }
}