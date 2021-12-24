using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_general_config", Schema = "public")]
    public class AdmGeneralConfig
    {
        [Required]
        [Key, Column("config_id")]
        public int config_id { get; set;}

        [Column("config_name")]
        public string config_name { get; set;} = CareConstants.EMPTY_STRING;

        [Column("config_value")]
        public string config_value { get; set;} = CareConstants.EMPTY_STRING;

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
        
        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};
        
        [Column("description")]
        public string description { get; set;} = CareConstants.EMPTY_STRING;
        
    }
}