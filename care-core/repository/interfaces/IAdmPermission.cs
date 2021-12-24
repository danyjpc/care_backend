using System;
using System.Collections.Generic;
using care_core.dto.AdmPermission;
using care_core.model;


namespace care_core.repository.interfaces
{
    public interface IAdmPermission
    {
        IEnumerable<Object> getAll(int user_id);
        //AdmCategoryDto getCategoryById(int module_id, int categoryId);
        int persist(bool upd_hasOp, int user_id, int module_id, int permission_id);
        void UpdUserPermissions( AdmUserPermission admUserPermission, bool upd_hasOp);
        void save();
    }
}