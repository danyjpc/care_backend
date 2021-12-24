using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_module", Schema = "public")]
    public class AdmModule
    {
        [Required]
        [Key, Column("module_id")]
        public int module_id { get; set;}
        
        [Column("name_module")]
        public string name_module { get; set;} = CareConstants.EMPTY_STRING;
        
        [Column("icon")]
        public string icon { get; set;} = CareConstants.EMPTY_STRING;
        
        [Column("picture")]
        public string picture { get; set;} = CareConstants.EMPTY_STRING;

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
    }
}