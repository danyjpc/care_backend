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
    public class AdmModuleRepository : IAdmModule
    {
        private readonly EntityDbContext _dbContext;

        public AdmModuleRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AdmModuleDto> getAll(int estado)
        {
            IEnumerable<AdmModuleDto> modulos = _dbContext.admModules.Where(x => x.status.typology_id == (estado > 0 ? estado : x.status.typology_id))
                .Select(
                modulo => new AdmModuleDto()
                {
                    module_id = modulo.module_id,
                    icon = modulo.icon,
                    name_module = modulo.name_module,
                    picture = modulo.picture,
                    date_create = modulo.date_create,
                    status = new AdmTypologyDto()
                    {
                        typology_id = modulo.status.typology_id,
                        description = modulo.status.description
                    },
                    created_by_user = new AdmUserDto()
                    {
                        user_id = modulo.created_by_user.user_id,
                        email = modulo.created_by_user.person.email
                    }
                }
            ).ToList();

            //appending group to evety quest

            return modulos;
        }
        
        public AdmModuleDto getModuleById(int moduleId)
        {
            var modulos = _dbContext.admModules.Where(x => x.module_id == moduleId)
                .Select(
                    modulo => new AdmModuleDto()
                    {
                        module_id = modulo.module_id,
                        icon = modulo.icon,
                        name_module = modulo.name_module,
                        picture = modulo.picture,
                        date_create = modulo.date_create,
                        status = new AdmTypologyDto()
                        {
                            typology_id = modulo.status.typology_id,
                            description = modulo.status.description
                        },
                        created_by_user = new AdmUserDto()
                        {
                            user_id = modulo.created_by_user.user_id,
                            email = modulo.created_by_user.person.email
                        }
                    }
                ).SingleOrDefault();

            //appending group to evety quest

            return modulos;
        }

        public int persist(AdmModule admModule)
        {
            _dbContext.Add(admModule);
            this.save();

            return admModule.module_id;
        }
        public int update(AdmModuleDto admModuleDto)
        {
            AdmModule currentModule = _dbContext.admModules.Find(admModuleDto.module_id);
            if (admModuleDto.icon != null)
            {
                currentModule.icon = admModuleDto.icon;
            }
            if (admModuleDto.name_module != null)
            {
                currentModule.name_module = admModuleDto.name_module;
            }
            if (admModuleDto.picture != null)
            {
                currentModule.picture = admModuleDto.picture;
            }

            if (admModuleDto.status.typology_id != null)
            {
                AdmTypology status = _dbContext.admTypologies.Find(admModuleDto.status.typology_id);
                if (status != null)
                {
                    currentModule.status = status;
                }
            }
            
            _dbContext.Entry(currentModule).State = EntityState.Modified;
            save();
            return currentModule.module_id;
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}