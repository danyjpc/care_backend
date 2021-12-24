using System;

namespace care_core.dto.AdmUser
{
    public class AdmUserEmailDto
    {
        public int user_id { get; set; }
        public string email { get; set; }
        public int status_id { get; set; }
        public int role_id { get; set;}
        public AdmUserEmailDto()
        {
        }
    }
}