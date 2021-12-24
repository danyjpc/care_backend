using System;
using System.Collections.Generic;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.util;

namespace care_core.dto.AdmQuestionGroup
{
    public class AdmQuestionDto
    {
        public int question_id { get; set; }
        public string name_question { get; set; }
        public string? type { get; set; }
        public bool? use_custom_option { get; set; }
        public bool? use_for_counter { get; set; }
        public int? typology_id { get; set; }

        public AdmTypologyDto? status = new AdmTypologyDto() { };

        public AdmUserDto? created_by_user = new AdmUserDto() { };

        public List<AdmQuestionOptionDto>? options { get; set; }

        public DateTime date_created { get; set; }
        
        public int? order_index { get; set; }


        public AdmQuestionDto()
        {
        }

        public AdmQuestionDto(int questionId, string nameQuestion, string type, bool use_custom_optio,
            int typology_id,
            List<AdmQuestionOptionDto> options)
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