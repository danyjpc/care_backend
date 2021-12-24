using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_case", Schema = "public")]
    public class AdmCase
    {
        [Required]
        [Key, Column("case_id"), ForeignKey("case_id")]
        public long case_id { get; set; }

        [Column("aggressor_first_name")]
        public string aggressor_first_name { get; set; } = CareConstants.EMPTY_STRING;

        [Column("aggressor_last_name")]
        public string aggressor_last_name { get; set; } = CareConstants.EMPTY_STRING;

        [Column("aggressor_birthday")]
        public DateTime aggressor_birthday { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        [Column("aggressor_phone_number")]
        public long aggressor_phone_number { get; set; } = CareConstants.ZERO_DEFAULT;

        [Column("aggressor_cui")]
        public long aggressor_cui { get; set; }

        [Column("aggressor_state_id"), ForeignKey("aggressor_state_id") ]
        public AdmTypology aggressor_state { get; set; } = new AdmTypology() { };

        [Column("aggressor_address_line")]
        public string aggressor_address_line { get; set; } = CareConstants.EMPTY_STRING;

        [Column("aggressor_city_id"), ForeignKey("aggressor_city_id")]
        public AdmTypology aggressor_city { get; set; } = new AdmTypology() { };

        [Column("aggressor_occupation_id"), ForeignKey("aggressor_ocupation_id")]
        public AdmTypology aggressor_occupation { get; set; } = new AdmTypology() { };

        [Column("aggressor_marital_status_id"), ForeignKey("aggressor_marital_status_id")]
        public AdmTypology aggressor_marital_status { get; set; } = new AdmTypology() { };

        [Column("aggressor_work_place")]
        public string aggressor_work_place { get; set; } = CareConstants.EMPTY_STRING;

        [Column("act_date"), ForeignKey("act_date")]
        public DateTime act_date { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        [Column("act_place")]
        public string act_place { get; set; } = CareConstants.EMPTY_STRING;

        [Column("victim_relationship_id"), ForeignKey("victim_relationship_id")]
        public AdmTypology victim_relationship { get; set; } = new AdmTypology() { };

        [Column("act_description")]
        public string act_description { get; set; } = CareConstants.EMPTY_STRING;

        [Column("identified_violence_type_id"), ForeignKey("identified_violence_type_id")]
        public AdmTypology identified_violence_type { get; set; } = new AdmTypology() { };

        [Column("lives_with_aggressor")]
        public Boolean lives_with_aggressor { get; set; }

        [Column("accompaniment_type_id"), ForeignKey("accompaniment_type_id")]
        public AdmTypology accompaniment_type { get; set; } = new AdmTypology() { };

        [Column("institution_id"), ForeignKey("institution_id")]
        public AdmTypology institution { get; set; } = new AdmTypology() { };

        [Column("accompaniment_route_id"), ForeignKey("accompaniment_route_id")]
        public AdmTypology accompaniment_route { get; set; } = new AdmTypology() { };

        [Column("annotation")]
        public string annotation { get; set; } = CareConstants.EMPTY_STRING;

        [Column("recording_organization_id"), ForeignKey("recording_organization_id")]
        public AdmOrganization recording_organization { get; set; } = new AdmOrganization() { };

        //TODO change to organizationContact
        [Column("recorded_by"), ForeignKey("recorded_by")]
        public AdmOrganizationMember recorded_by_member { get; set; }

        [Column("victim_id"), ForeignKey("victim_id")]
        public AdmPerson victim { get; set; } = new AdmPerson() { };

        [Column("case_category_id"), ForeignKey("case_category_id")]
        public AdmTypology case_category { get; set; } = new AdmTypology() { };

        [Column("attached_victim_dpi")] 
        public Boolean attached_victim_dpi { get; set; } = CareConstants.FALSE;

        [Column("attached_victim_birth_certificate")]
        public Boolean attached_victim_birth_certificate { get; set; } = CareConstants.FALSE;

        [Column("attached_children_birth_certificate")]
        public Boolean attached_children_birth_certificate { get; set; } = CareConstants.FALSE;

        [Column("attached_marriage_certificate")]
        public Boolean attached_marriage_certificate { get; set; } = CareConstants.FALSE;

        [Column("attached_aggressor_dpi")]
        public Boolean attached_aggressor_dpi { get; set; } = CareConstants.FALSE;

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology() { };

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser();

        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
    }
}