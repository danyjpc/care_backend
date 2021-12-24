using System.Collections.Generic;
using care_core.util;
using care_core.model;

namespace care_core.dto.AdmPermission
{
    public class AdmUserPermissionDto
    {
        public int permission_id { get; set;}
        public string name_permission { get; set;}
        public string alias { get; set;}
        public bool has_permissions { get; set; }

        public AdmUserPermissionDto(){

        }

        public AdmUserPermissionDto(int permission_id, string name_permission, string alias,bool has_permissions){
            this.permission_id = permission_id;
            this.name_permission = name_permission;
            this.alias = alias;
            this.has_permissions = has_permissions;
        }
    }
}