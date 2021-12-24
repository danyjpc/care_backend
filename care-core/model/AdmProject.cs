using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_project", Schema = "public")]
    public class AdmProject
    {
        [Required]
        [Key, Column("project_id")]
        public int project_id { get; set;}

        [Column("name_project")]
        public string name_project { get; set; }
        
        [Column("date")]
        public DateTime date { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        [Column("organization_id"), ForeignKey("organization_id")]
        public AdmOrganization organization { get; set; } = new AdmOrganization (){};

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
    }
}