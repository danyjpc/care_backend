using System;

namespace care_core.Controllers.util
{
    public class JsonResponse
    {
        public long id { get; set; }

        //public Guid key { get; set; }
        public string code { get; set; }
        public string msg { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(id)}={id.ToString()}, {nameof(code)}={code}, {nameof(msg)}={msg}}}";
        }
    }
}