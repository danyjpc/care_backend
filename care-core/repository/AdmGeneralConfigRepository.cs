using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmGeneralConfigDto;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace care_core.repository
{
    public class AdmGeneralConfigRepository : IAdmGeneralConfig
    {
        private readonly EntityDbContext _dbContext;

        public AdmGeneralConfigRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IEnumerable<AdmGeneralConfigDto> getAll(int statusId)
        {
            IEnumerable<AdmGeneralConfigDto> configuraciones = _dbContext.admGeneralConfigs
                .Where(config => config.status.typology_id.Equals((statusId > 0 ? statusId : CareConstants.ESTADO_ACTIVO)))
                .Select(
                config => new AdmGeneralConfigDto()
                {
                    config_id = config.config_id,
                    config_name = config.config_name,
                    config_value = config.config_value,
                    date_created = config.date_created,
                    created_by_user = new AdmUserDto()
                    {
                        user_id = config.created_by_user.user_id,
                        email = config.created_by_user.person.email
                    },
                    status = new AdmTypologyDto()
                    {
                        typology_id = config.status.typology_id,
                        description = config.status.description
                    },
                    description = config.description
                    
                }
            ).OrderBy(x=>x.config_id).ToList();
            

            return configuraciones;
        }

        public AdmGeneralConfigDto getConfigById(int configId)
        {
            AdmGeneralConfigDto configuracion = _dbContext.admGeneralConfigs
                .Where(config => config.config_id.Equals(configId))
                .Select(
                    config => new AdmGeneralConfigDto()
                    {
                        config_id = config.config_id,
                        config_name = config.config_name,
                        config_value = config.config_value,
                        date_created = config.date_created,
                        created_by_user = new AdmUserDto()
                        {
                            user_id = config.created_by_user.user_id,
                            email = config.created_by_user.person.email
                        },
                        status = new AdmTypologyDto()
                        {
                            typology_id = config.status.typology_id,
                            description = config.status.description
                        },
                        description = config.description
                    
                    }
                ).OrderBy(x=>x.config_id).SingleOrDefault();
            

            return configuracion;
        }

        public int persist(AdmGeneralConfig admGeneralConfig)
        {
            
            _dbContext.Add(admGeneralConfig);
            save();
            
            return admGeneralConfig.config_id;
        }

        public int update(AdmGeneralConfigDto admGeneralConfigDto)
        {
            AdmGeneralConfig currentConfig = _dbContext.admGeneralConfigs.Find(admGeneralConfigDto.config_id);
            
            if (!string.IsNullOrEmpty(admGeneralConfigDto.config_name))
            {
                currentConfig.config_name = admGeneralConfigDto.config_name;
            }

            if (!string.IsNullOrEmpty(admGeneralConfigDto.config_value))
            {
                currentConfig.config_value = admGeneralConfigDto.config_value;
            }

            if (admGeneralConfigDto.status != null)
            {
                AdmTypology status = _dbContext.admTypologies.Find(admGeneralConfigDto.status.typology_id);
                if (status != null)
                {
                    currentConfig.status = status;
                }
            }
            
            if (!string.IsNullOrEmpty(admGeneralConfigDto.description))
            {
                currentConfig.description = admGeneralConfigDto.description;
            }
            

            _dbContext.Entry(currentConfig).State = EntityState.Modified;
            save();
            return currentConfig.config_id;
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}