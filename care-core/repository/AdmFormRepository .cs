using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmCase;
using care_core.dto.AdmForm;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmOrganizationMember;
using care_core.dto.AdmPerson;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace care_core.repository
{
    public class AdmFormRepository : IAdmForm
    {
        private readonly EntityDbContext _dbContext;

        public AdmFormRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IEnumerable<AdmFormDto> getAllByModule(int moduleId)
        {
            IEnumerable<AdmFormDto> forms = _dbContext.admForms.Where(x => x.status.typology_id == (CareConstants.ESTADO_ACTIVO)
                && x.module_category.module.module_id == moduleId)
                .Select(
                    form => new AdmFormDto()
                    {
                        form_id = form.form_id,
                        name_form = form.name_form,
                        
                        category = new AdmCategDto()
                        {
                            category_id = form.module_category.category.category_id,
                            name_category = form.module_category.category.name_category
                        },
                        module = new AdmModulDto()
                        {
                            module_id = form.module_category.module.module_id,
                            name_module = form.module_category.module.name_module
                        },
                        status = new AdmTypologyDto()
                        {
                            typology_id = form.status.typology_id,
                            description = form.status.description
                        },
                        created_by_user = new AdmUserDto()
                        {
                            user_id = form.created_by_user.user_id,
                            email = form.created_by_user.person.email
                        },
                        date_created = form.date_create
                    }
                ).OrderBy(x=>x.form_id).ToList();

            return forms;
        }

        public IEnumerable<AdmFormDto> getById(int formId)
        {
            IEnumerable<AdmFormDto> forms = _dbContext.admForms.Where(x => x.status.typology_id == (CareConstants.ESTADO_ACTIVO)
                                                                           && x.form_id == formId)
                .Select(
                    form => new AdmFormDto()
                    {
                        form_id = form.form_id,
                        name_form = form.name_form,
                        
                        category = new AdmCategDto()
                        {
                            category_id = form.module_category.category.category_id,
                            name_category = form.module_category.category.name_category
                        },
                        module = new AdmModulDto()
                        {
                            module_id = form.module_category.module.module_id,
                            name_module = form.module_category.module.name_module
                        },
                        status = new AdmTypologyDto()
                        {
                            typology_id = form.status.typology_id,
                            description = form.status.description
                        },
                        created_by_user = new AdmUserDto()
                        {
                            user_id = form.created_by_user.user_id,
                            email = form.created_by_user.person.email
                        },
                        date_created = form.date_create
                    }
                ).OrderBy(x=>x.form_id).ToList();

            return forms;
        }
    }
}