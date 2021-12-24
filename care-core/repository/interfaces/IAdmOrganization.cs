using System;
using System.Collections.Generic;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmOrganization
    {
        IEnumerable<Object> getAll();
        Object getById(long id);

        long persist(AdmOrganization admOrganization);
        void upd (AdmOrganization admOrganization);

        // *********** ORGANIZATION MEMBERS *********
        IEnumerable<Object> getAllMembers(int id);
        Object getByIdMember(int id, int member_id);

        long persistMember(AdmOrganizationMember admOrganizationMember);
        void updMember (AdmOrganizationMember admOrganizationMember);

        void save();
    }
}