using System;
using System.Collections.Generic;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmProject
    {
        IEnumerable<Object> getAll();
        Object getById(long id);

        long persist(AdmProject admProject);
        void upd (AdmProject admProject);
        
        //********* Activities ***********

        IEnumerable<Object> getAllProyectActivities(int id);
        Object getByIdActivity(int project_id, int activity_id);

        long persist(AdmProjectActivity admProjectActivity);

        void updProjectActivity (AdmProjectActivity admProjectActivity);

        void save();
    }
}