using System;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmPerson;
using care_core.dto.AdmTypology;

namespace care_core.dto.AdmUser
{
    public class AdmUserDto
    {
        public int user_id { get; set; }
        public string password { get; set; }
        public AdmOrganizationDto organization { get; set; }
        public AdmPersonDto person { get; set; }
        public string email { get; set; }
        public AdmTypologyDto role { get; set; }
        public AdmTypologyDto status { get; set; }
        public DateTime? date_create { get; set; }
        public AdmUserDto()
        {
        }
    }
}