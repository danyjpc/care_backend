using System;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;

namespace care_core.dto.AdmGeneralConfigDto
{
    public class AdmGeneralConfigDto
    {
        public int config_id { get; set; }

        public string config_name { get; set; }

        public string config_value { get; set; }

        public AdmUserDto created_by_user { get; set; }

        public DateTime date_created { get; set; }

        public AdmTypologyDto status { get; set; }

        public string description { get; set; }
    }
}