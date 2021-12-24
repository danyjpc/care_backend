using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_group", Schema = "public")]
    public class AdmGroup
    {
        [Required]
        [Key, Column("group_id")]
        public int group_id { get; set; }

        [Column("name_group")]
        public string name_group { get; set; }
        
        [Column("form_id"), ForeignKey("form_id")]
        public AdmForm form { get; set; } = new AdmForm() { };

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology() { };

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() { };

        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
        
    }
}