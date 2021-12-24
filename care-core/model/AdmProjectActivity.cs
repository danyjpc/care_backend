using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_project_activity", Schema = "public")]
    public class AdmProjectActivity
    {
        [Required]
        [Key, Column("project_activity_id")]
        public int project_activity_id { get; set; }

        [Column("activity_address")]
        public string activity_address { get; set; } = CareConstants.EMPTY_STRING;
        
        [Column("state_id"), ForeignKey("state_id")]
        public AdmTypology state { get; set; } = new AdmTypology() { };

        [Column("city_id"), ForeignKey("city_id")]
        public AdmTypology city { get; set; } = new AdmTypology() { };

        [Column("date")]
        public DateTime date { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        [Column ("activity_type_id"), ForeignKey("activity_type_id")]
        public AdmTypology activity_type { get; set; } = new AdmTypology(){};

        [Column("number_participant")]
        public int number_participant { get; set; } = CareConstants.ZERO_DEFAULT;

        [Column("activity_cost")]
        public int activity_cost { get; set; } = CareConstants.ZERO_DEFAULT;

        [Column("time_duration")]
        public int time_duration { get; set; } =CareConstants.ZERO_DEFAULT;

        [Column("main_contribution")]
        public string main_contribution { get; set; } = CareConstants.EMPTY_STRING;

        [Column("limit_challenge_solution")]
        public string limit_challenge_solution { get; set; } =CareConstants.EMPTY_STRING;

        [Column("project_id"), ForeignKey("project_id")]
        public AdmProject project { get; set; } = new AdmProject() {};

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
    }
}