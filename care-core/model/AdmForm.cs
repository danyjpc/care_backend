using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_form", Schema = "public")]
    public class AdmForm
    {
        [Required]
        [Key, Column("form_id")]
        public int form_id { get; set;}
        
        [Column("name_form")]
        public string name_form { get; set;} = CareConstants.EMPTY_STRING;
        [Column("is_public")]
        public bool is_public { get; set; } = CareConstants.FALSE;

        [Column("module_category_id"), ForeignKey("module_category_id")]
        public AdmModuleCategory module_category { get; set;} = new AdmModuleCategory (){};

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        
        
    }
}