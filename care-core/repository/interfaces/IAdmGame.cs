using System;
using System.Collections.Generic;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmGame
    {
        IEnumerable<AdmGame> getAll();

        IEnumerable<Object> reportGetAll();
        AdmGame getById(long id);
        long persist(AdmGame admGame);
        void upd(AdmGame admGame);
        void save();
    }
}