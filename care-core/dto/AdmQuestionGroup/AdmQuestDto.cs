using System;
using System.Collections.Generic;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;


namespace care_core.dto.AdmQuestionGroup
{
    public class AdmQuestDto
    {
        public int question_id { get; set; }
        public string name_question { get; set; }
        public string type { get; set; }
        public bool use_custom_option { get; set; }
        public int typology_id { get; set; }

        public List<AdmQuestionOptionDto> options { get; set; }
        
        public int order_index { get; set; }


        public AdmQuestDto()
        {
        }

        public AdmQuestDto(int questionId, string nameQuestion, string type, bool use_custom_optio,
            int typology_id, List<AdmQuestionOptionDto> options)
        {
            this.question_id = questionId;
            this.name_question = nameQuestion;
            this.type = type;
            this.use_custom_option = use_custom_optio;
            this.typology_id = typology_id;
            this.options = options;
        }
    }
}