using System;
using System.Collections.Generic;
using care_core.dto.AdmUser;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmUser
    {
        IEnumerable<Object> getAll();
        
        IEnumerable<AdmUserDto> getAllDto();
        Object getById(long id);

        int persist(AdmUser admUser);
        void upd (AdmUser admUser);

        void updPassword( AdmUser admUser);

        AdmUser findByCredentials(string email, string password);
        
        AdmUser findByEmail(string email);

        void passwordReset(AdmPerson admPerson);

        void save();
    }
}