using System;
using System.Collections.Generic;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmPerson
    {
        IEnumerable<Object> getAll(int estado);
        AdmPerson getById(int person_id);
        long persist(AdmPerson admPerson);
        void upd(AdmPerson admPerson);
        void save();

        AdmPerson findByDpi(long personDpi);
        
        AdmPerson findByEmail(string personMail, bool useDevsMail);

        AdmPerson persistDefaultValues(AdmPerson admPerson);

        Object findPersonByCui(int personCui);
        Object findPersonByEmail(string email);

    }
}