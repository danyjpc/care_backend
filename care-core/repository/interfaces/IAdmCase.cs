using System;
using System.Collections.Generic;
using care_core.dto.AdmCase;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmCase
    {
        IEnumerable<AdmCaseDto> getAll(int estado);
        AdmCaseDto getCaseById(int caseId);
        long persist(AdmCase admCase);
        void upd(AdmCase admCase);
        void save();
    }
}