using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmForm;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace care_core.repository
{
    public class AdmCategoryRepository : IAdmCategory
    {
        private readonly EntityDbContext _dbContext;

        public AdmCategoryRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AdmCategoryDto getCategoryById(int module_id, int categoryId)
        {
            var categorias = _dbContext.admModuleCategories
                .Where(x =>x.module.module_id ==module_id  && x.category.category_id == categoryId)
                .Select(
                    categoria => new AdmCategoryDto()
                    {
                        category_id = categoria.category.category_id,
                        name_category = categoria.category.name_category,
                        icon = categoria.category.icon,
                        color = categoria.category.color,
                        date_create = categoria.category.date_create,
                        status = new AdmTypologyDto()
                        {
                            typology_id = categoria.category.status.typology_id,
                            description = categoria.category.status.description
                        },
                        created_by_user = new AdmUserDto()
                        {
                            user_id = categoria.category.created_by_user.user_id,
                            email = categoria.category.created_by_user.person.email
                        }
                    }
                ).SingleOrDefault();

            //appending group to evety quest

            return categorias;
        }

        public int persist(int module_id, AdmCategory admCategory)
        {
            AdmModuleCategory admModuleCategory = new AdmModuleCategory();

            _dbContext.Add(admCategory);
            save();

            AdmModule module = _dbContext.admModules.Find(module_id);
            //AdmCategory category =  _dbContext.admCategories.Find(admCategory.category_id);

            admModuleCategory.category = admCategory;
            admModuleCategory.module = module;

            _dbContext.Add(admModuleCategory);
            save();

            return admCategory.category_id;
        }

        public int update(AdmCategoryDto admCategoryDto)
        {
            AdmCategory currentCategory = _dbContext.admCategories.Find(admCategoryDto.category_id);
            if (admCategoryDto.icon != null)
            {
                currentCategory.icon = admCategoryDto.icon;
            }

            if (admCategoryDto.name_category != null)
            {
                currentCategory.name_category = admCategoryDto.name_category;
            }

            if (admCategoryDto.color != null)
            {
                currentCategory.color = admCategoryDto.color;
            }

            if (admCategoryDto.status.typology_id != null)
            {
                AdmTypology status = _dbContext.admTypologies.Find(admCategoryDto.status.typology_id);
                if (status != null)
                {
                    currentCategory.status = status;
                }
            }

            _dbContext.Entry(currentCategory).State = EntityState.Modified;
            save();
            return currentCategory.category_id;
        }

        public IEnumerable<AdmCategoryDto> getAll(int estado)
        {
            IEnumerable<AdmCategoryDto> categorias = _dbContext.admCategories
                .Where(x => x.status.typology_id == (estado > 0 ? estado : x.status.typology_id))
                .Select(
                    categoria => new AdmCategoryDto()
                    {
                        category_id = categoria.category_id,
                        name_category = categoria.name_category,
                        icon = categoria.icon,
                        color = categoria.color,
                        date_create = categoria.date_create,
                        status = new AdmTypologyDto()
                        {
                            typology_id = categoria.status.typology_id,
                            description = categoria.status.description
                        },
                        created_by_user = new AdmUserDto()
                        {
                            user_id = categoria.created_by_user.user_id,
                            email = categoria.created_by_user.person.email
                        }
                    }
                ).ToList();
            return categorias;
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}