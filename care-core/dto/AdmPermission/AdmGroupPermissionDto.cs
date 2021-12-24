using System.Collections.Generic;
using care_core.util;
using care_core.model;

namespace care_core.dto.AdmPermission
{
    public class AdmGroupPermissionDto
    {
        public int module_id { get; set;}
        public string name_module { get; set;}
        
        public AdmUserPermissionDto [] permissions { get; set; } 

        public AdmGroupPermissionDto(){

        }

        public AdmGroupPermissionDto(int module_id, string name_module, AdmUserPermissionDto [] permissions){
            this.module_id = module_id;
            this.name_module = name_module;
            this.permissions = permissions;
        }
    }
}