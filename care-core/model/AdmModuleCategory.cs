using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_module_category", Schema = "public")]
    public class AdmModuleCategory
    {
        [Required]
        [Key, Column("module_category_id")]
        public int module_category_id { get; set;}
        
        [Column("module_id"), ForeignKey("module_id")]
        public AdmModule module { get; set; } = new AdmModule () {};

        [Column("category_id"), ForeignKey("category_id")]
        public AdmCategory category { get; set; } = new AdmCategory(){}; 
        
    }
}