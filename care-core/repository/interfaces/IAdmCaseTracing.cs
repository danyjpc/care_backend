using System;
using System.Collections.Generic;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmCaseTracing
    {
        IEnumerable<Object> getAll(int case_id);
        Object getCaseTracingById(int case_id, int tracing_id);
        long persist(AdmCaseTracing admCaseTracing);
        void upd(AdmCaseTracing admCaseTracing);
        void save();
    }
}