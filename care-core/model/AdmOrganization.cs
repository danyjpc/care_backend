using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_organization", Schema = "public")]
    public class AdmOrganization
    {
        [Required]
        [Key, Column("organization_id")]
        public int organization_id { get; set; }

        [Column("name_organization")]
        public string name_organization { get; set;} = CareConstants.EMPTY_STRING;

        [Column("abbreviation")]
        public string abbreviation { get; set; } = CareConstants.EMPTY_STRING;

        [Column("url")]
        public string url { get; set; } = CareConstants.EMPTY_STRING;

        [Column("date")]
        public DateTime date { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;


        [Column("type_organization_id"), ForeignKey("type_organization_id")]
        public AdmTypology organization_type { get; set; } = new AdmTypology(){};

        [Column("responsible_organization")]
        public string organization_responsible { get; set; } = CareConstants.EMPTY_STRING;

        [Column("phone_number")]
        public long phone_number { get; set;} = CareConstants.ZERO_DEFAULT;

        [Column("email")]
        public string email { get; set; } = CareConstants.DEFAULT_AT;

        [Column("state_id"), ForeignKey("state_id")]
        public AdmTypology state { get; set; } = new AdmTypology() { };

        [Column("city_id"), ForeignKey("city_id")]
        public AdmTypology city { get; set; } = new AdmTypology() { };

        [Column("address")]
        public string address { get; set; } = CareConstants.EMPTY_STRING;

        [Column("frequency_meeting_id"), ForeignKey("frequency_meeting_id")]
        public AdmTypology frequency_meeting { get; set; } = new AdmTypology() { };

        [Column("participation_space_id"), ForeignKey("participation_space_id")]
        public AdmTypology participation_space { get; set; } = new AdmTypology() { }; 

        [Column("main_problem")]
        public string main_problem { get; set; } = CareConstants.EMPTY_STRING;
        
        [Column("action_perform")]
        public string action_perform { get; set; } =CareConstants.EMPTY_STRING;

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology();

        [Column("created_by"), ForeignKey("created_by")]
        //public AdmUser created_by_user { get; set; } = new AdmUser() {};
        public int created_by { get; set; } = CareConstants.ZERO_DEFAULT;

        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
    }
}