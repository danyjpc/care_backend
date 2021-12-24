using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_user_permission", Schema = "public")]
    public class AdmUserPermission
    {
        [Required]
        [Key, Column("user_permission_id")]
        public int user_permission_id { get; set; }

        [Column("has_permissions")]
        public bool has_permissions { get; set; } = CareConstants.TRUE;
        
        [Column("user_id"), ForeignKey("user_id")]
        public AdmUser user { get; set; } = new AdmUser () {};

        [Column("module_id"), ForeignKey("module_id")]
        public AdmModule module { get; set; } = new AdmModule () {};

        [Column("permission_id"), ForeignKey("permission_id")]
        public AdmPermission permission { get; set; } = new AdmPermission () {};
    }
}