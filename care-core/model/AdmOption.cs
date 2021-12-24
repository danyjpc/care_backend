using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_option", Schema = "public")]
    public class AdmOption
    {
        [Required]
        [Key, Column("option_id")]
        public int option_id { get; set;}
        
        [Column("value")]
        public string value { get; set;} = CareConstants.EMPTY_STRING;

        [Column("question_id"), ForeignKey("question_id")]
        public AdmQuestion question { get; set; } = new AdmQuestion(){};
        
        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

    }
}