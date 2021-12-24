using System.Collections.Generic;

namespace care_core.dto.AdmSurvey
{
    public class AdmSurveyStatsDto
    {
        public int total_surveys { get; set; }
       
        public List<AdmQuestionStatDto> questions { get; set; }

        public class AdmQuestionStatDto
        {
            public int question_id { get; set; }
            public string question_name { get; set; }
            public string question_type { get; set; }
            public Dictionary<string, int> stats { get; set; }
           
        }
    }
}