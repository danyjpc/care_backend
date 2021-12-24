using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_organization_member", Schema = "public")]
    public class AdmOrganizationMember
    {
        [Required]
        [Key, Column("organization_member_id")]
        public int organization_member_id { get; set; }

        [Column("name_organization_member")]
        public string name_organization_member { get; set; } = CareConstants.EMPTY_STRING;

        [Column("phone_number")]
        public long phone_number { get; set; } = CareConstants.ZERO_DEFAULT;

        [Column("email")]
        public string email { get; set; } = CareConstants.EMPTY_STRING;

        [Column("organization_id")]
        [ForeignKey("organization_id")]
        public AdmOrganization organization {get; set;} =new AdmOrganization(){};

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

    }
}