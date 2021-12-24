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
    public class AdmModuleCategoryRepository : IAdmModuleCategory
    {
        private readonly EntityDbContext _dbContext;

        public AdmModuleCategoryRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int persist(AdmModuleCategory admModuleCategory)
        {
            _dbContext.Add(admModuleCategory);
            this.save();

            return admModuleCategory.module_category_id;
        }
        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}