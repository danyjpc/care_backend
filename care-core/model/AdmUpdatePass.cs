using System;

namespace care_core.model
{
    public class AdmUpdatePass{
        public int user_id { get; set; }
        public string old_pass { get; set; }
        public string new_pass { get; set; }   
        public string confirm_pass{ get; set; }
    }
}