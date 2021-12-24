
using System.Collections.Generic;
using care_core.dto.AdmTypology;
using care_core.util;

namespace care_core.dto.AdmQuestionGroup
{
    public class AdmGroupDto
    {
        public int group_id { get; set;}
        public string name_group { get; set;}
        
        public List<AdmQuestionDto> questions { get; set; }

        public AdmGroupDto(){

        }

        public AdmGroupDto(int groupId, string nameGroup)
        {
            group_id = groupId;
            name_group = nameGroup;
        }

        public AdmGroupDto(int groupId, string nameGroup, List<AdmQuestionDto> questions)
        {
            group_id = groupId;
            name_group = nameGroup;
            this.questions = questions;
        }
    }
}