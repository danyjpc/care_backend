using System.Collections.Generic;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmTypology
    {
        IEnumerable<AdmTypology> getAll(long parent_id, bool showInSurvey);
        AdmTypology getById(int? id);
        long persist(AdmTypology admTypology);
        void upd(AdmTypology admTypology);
        void save();
    }
}