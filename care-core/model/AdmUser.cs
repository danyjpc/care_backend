using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_user", Schema = "public")]
    public class AdmUser
    {
        [Required]
        [Key, Column("user_id")]
        public int user_id { get; set; }

        [Column("password")]
        [MaxLength(1000)]
        public string password { get; set; } = CareConstants.EMPTY_STRING;

        [Column("organization_id")]
        [ForeignKey("organization_id")]
        public AdmOrganization organization {get; set;} =new AdmOrganization(){};

        [Column("person_id")]
        [ForeignKey("person_id")]
        public AdmPerson person{ get; set; }
       
        [Column("role_id"), ForeignKey("role_id")]
        public AdmTypology role { get; set; } = new AdmTypology() {};
        
        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
        
    }
}