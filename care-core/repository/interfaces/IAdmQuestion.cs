using System;
using System.Collections.Generic;
using care_core.dto.AdmCase;
using care_core.dto.AdmQuestionGroup;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmQuestion
    {
        IEnumerable<AdmQuestionDto> getAll(int estado);
        IEnumerable<AdmQuestionDto> getAllByGroup(int form_id, int groupId, int estado);
        IEnumerable<AdmQuestionDto> getAllByForm(int form_id);
        AdmQuestionDto getQuestionById(int questionId);
        int persist(AdmQuestion admQuestion);
        int update(AdmQuestion admQuestion);
        void save();
    }
}