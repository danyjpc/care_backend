using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_answer", Schema = "public")]
    public class AdmAnswer
    {
        [Required]
        [Key, Column("answer_id")]
        public int answer_id { get; set;}

        [Column("survey_id"), ForeignKey("survey_id")]
        public AdmSurvey survey { get; set;} = new AdmSurvey() {};

        [Column("question_id"), ForeignKey("question_id")]
        public AdmQuestion question { get; set; } = new AdmQuestion(){};

        [Column("answer")]
        public string answer { get; set;} = CareConstants.EMPTY_STRING;
        
        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
        
        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};
        
    }
}