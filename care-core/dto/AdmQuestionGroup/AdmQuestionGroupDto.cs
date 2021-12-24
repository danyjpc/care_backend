
using care_core.dto.AdmTypology;
using care_core.util;
using care_core.dto.AdmUser;

namespace care_core.dto.AdmQuestionGroup
{
    public class AdmQuestionGroupDto
    {
        public int group_id { get; set; }
        public string name_group { get; set; }
        public AdmUserDto created_by { get; set; } = new AdmUserDto(); 
        public AdmQuestionDto [] questions { get; set;}

        public AdmQuestionGroupDto(){

        }

        public AdmQuestionGroupDto(int group_id, string name_group, AdmQuestionDto [] questions, AdmUserDto created_by )
        {
            this.group_id = group_id;
            this.name_group = name_group;
            this.questions = questions;
            this.created_by = created_by;

        }
    }
}