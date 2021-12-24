using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_category", Schema = "public")]
    public class AdmCategory
    {
        [Required]
        [Key, Column("category_id")]
        public int category_id { get; set;}
        
        [Column("name_category")]
        public string name_category { get; set;} = CareConstants.EMPTY_STRING;

        [Column("icon")]
        public string icon { get; set; } = CareConstants.EMPTY_STRING;

        [Column("color")]
        public string color { get; set; } = CareConstants.EMPTY_STRING;
        
        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

    }
}