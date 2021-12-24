
using System.Collections.Generic;
using care_core.dto.AdmForm;

namespace care_core.repository.interfaces
{
    public interface IAdmForm
    {
        IEnumerable<AdmFormDto> getAllByModule(int moduleId);
        IEnumerable<AdmFormDto> getById(int formId);
    }
}