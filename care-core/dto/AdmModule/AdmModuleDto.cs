using System;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.util;

namespace care_core.dto.AdmForm
{
    public class AdmModuleDto
    {
        public int module_id { get; set;}
        public string name_module { get; set;}
        public string icon { get; set;}
        public string picture { get; set;}
        public AdmTypologyDto status =new AdmTypologyDto(){};
        public AdmUserDto created_by_user = new AdmUserDto() {};
        public DateTime date_create { get; set; }

        public AdmModuleDto()
        {
        }

        public AdmModuleDto(int moduleId, string nameModule, string icon, string picture, DateTime dateCreate)
        {
            module_id = moduleId;
            name_module = nameModule;
            this.icon = icon;
            this.picture = picture;
            date_create = dateCreate;
        }
    }
}