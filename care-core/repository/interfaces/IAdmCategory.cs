using System;
using System.Collections.Generic;
using care_core.dto.AdmCase;
using care_core.dto.AdmForm;
using care_core.dto.AdmQuestionGroup;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmCategory
    {
        IEnumerable<AdmCategoryDto> getAll(int estado);
        AdmCategoryDto getCategoryById(int module_id, int categoryId);
        int persist(int module_id, AdmCategory admCategory);
        int update(AdmCategoryDto admCategory);
        void save();
    }
}