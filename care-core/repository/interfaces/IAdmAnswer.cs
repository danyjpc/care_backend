using System;
using System.Collections.Generic;
using care_core.dto.AdmAnswerDto;
using care_core.dto.AdmCase;
using care_core.dto.AdmForm;
using care_core.dto.AdmQuestionGroup;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmAnswer
    {
        IEnumerable<AdmAnswerDto> getAll();
        IEnumerable<AdmAnswerDto> getAllByForm(int form_id);
        AdmAnswerDto getAnswerById(int answerId);
        int persist(AdmAnswer admAnswer);
        int update(AdmAnswer admAnswer);
        public IEnumerable<AdmAnswerDto> getAllCompleteFormInfo();
        IEnumerable<AdmAnswer> getAnswersByQuestion(int question_id);
        void save();
    }
}