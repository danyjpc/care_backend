using System;
using System.Collections.Generic;
using care_core.dto.AdmCase;
using care_core.dto.AdmForm;
using care_core.dto.AdmQuestionGroup;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmModuleCategory
    {
        int persist(AdmModuleCategory admModuleCategory);
        void save();
    }
}