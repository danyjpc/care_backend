using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_question", Schema = "public")]
    public class AdmQuestion
    {
        [Required]
        [Key, Column("question_id")]
        public int question_id { get; set;}
        
        [Column("name_question")]
        public string name_question { get; set;} = CareConstants.EMPTY_STRING;

        [Column("type")]
        public string type { get; set;} = CareConstants.EMPTY_STRING;

        [Column("use_custom_option")]
        public bool use_custom_option { get; set; } = CareConstants.TRUE;

        [Column("use_for_counter")]
        public bool use_for_counter { get; set; } = CareConstants.TRUE;

        [Column("group_id"), ForeignKey("group_id")]
        public AdmGroup group { get; set;} = new AdmGroup() {};

        [Column("typology_id"), ForeignKey("typology_id")]
        public AdmTypology typology { get; set;} = new AdmTypology(){};

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_create { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
        
        [Column("order_index")]
        public int orderIndex { get; set;}
    }
}