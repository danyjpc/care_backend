using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_tracing", Schema = "public")]
    public class AdmCaseTracing
    {
        [Required]
        [Key, Column("tracing_id")]
        public int tracing_id { get; set; }

        [Column("tracing_status_id"), ForeignKey("tracing_status_id")]
        public AdmTypology tracing_status { get; set; } = new AdmTypology(){};

        [Column("partner_relationship")]
        public string partner_relationship { get; set; } = CareConstants.EMPTY_STRING;

        [Column("children_relationship")]
        public string children_relationship { get; set; } = CareConstants.EMPTY_STRING;

        [Column("support_reason")]
        public string support_reason { get; set; } = CareConstants.EMPTY_STRING;

        [Column("tracing_diagnosis")]
        public string tracing_diagnosis { get; set; } = CareConstants.EMPTY_STRING;

        [Column("tracing_description")]
        public string tracing_description { get; set; } = CareConstants.EMPTY_STRING;

        [Column("observations")]
        public string observations { get; set; } = CareConstants.EMPTY_STRING;
        
        [Column ("case_id"), ForeignKey("case_id")]
        public AdmCase cases { get; set; } = new AdmCase(){};

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
    }
}