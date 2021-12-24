using System;
using System.Collections.Generic;
using care_core.dto.AdmCase;
using care_core.dto.AdmQuestionGroup;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmOption
    {
        IEnumerable<AdmQuestionOptionDto> getAll(int estado);
        IEnumerable<AdmQuestionOptionDto> getAllByQuestion(int questionId, int estado);
        AdmQuestionOptionDto getOptionById(int optionId);
        int persist(AdmOption admOption);
        int update(AdmOption admOption);
        void save();
    }
}