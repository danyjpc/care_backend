using System;
using System.Collections.Generic;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmOrganizationMember
    {
        IEnumerable<Object> getAll();
        Object getById(long id);

        long persist(AdmOrganizationMember admOrganizationMember);
        void upd (AdmOrganizationMember admOrganizationMember);

        void save();
    }
}