using System;
using System.Collections.Generic;
using care_core.dto.AdmAnswerDto;
using care_core.dto.AdmCase;
using care_core.dto.AdmForm;
using care_core.dto.AdmGeneralConfigDto;
using care_core.dto.AdmQuestionGroup;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmGeneralConfig
    {
        IEnumerable<AdmGeneralConfigDto> getAll(int statusId);
        AdmGeneralConfigDto getConfigById(int configId);
        int persist(AdmGeneralConfig admGeneralConfig);
        int update(AdmGeneralConfigDto admGeneralConfigDto);
        void save();
    }
}