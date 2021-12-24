using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using care_core.dto.AdmForm;
using Microsoft.Extensions.Logging;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/modules/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmCategoyController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmCategory _admCategory;
        private JsonResponse response;

        public AdmCategoyController(EntityDbContext dbContext, IAdmCategory admCategory)
        {
            _dbContext = dbContext;
            _admCategory = admCategory;
            response = new JsonResponse();
        }

        /*[HttpGet()]
        public IActionResult GetAllCategories([FromQuery]int estado_id)
        {
            IEnumerable<AdmCategoryDto> categories = _admCategory.getAll(estado_id);
            return new OkObjectResult(categories);
        }*/

        [HttpGet("{module_id}/category/{category_id}")]
        public IActionResult GetCategoriesById([FromRoute] int module_id, [FromRoute] int category_id)
        {
            
            var categories = _admCategory.getCategoryById(module_id, category_id);
            if (categories == null)
            {
                response.code = "404";
                response.msg = "Category not found";
                return new NotFoundObjectResult(response);
            }
            
            return new OkObjectResult(categories);
        }
        
        //Persist category
        [HttpPost("{module_id}/category")]
        public IActionResult PersistCategory([FromRoute] int module_id, [FromBody] AdmCategoryDto categoryDto)
        {
            try
            {
                //CHECKING IF STATUS VALUE IS VALID
                AdmTypology status = _dbContext.admTypologies.Find(categoryDto.status.typology_id) ??
                                     _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);

                //CHECKING IF USER IS VALID
                AdmUser user = _dbContext.admUsers.Find(categoryDto.created_by_user.user_id);
                if (user == null)
                {
                    response.code = "400";
                    response.msg = "User not found";
                    return new BadRequestObjectResult(response);
                }

                AdmCategory category = new AdmCategory();
                using (var scope = new TransactionScope())
                {
                    category.color = categoryDto.color;
                    category.created_by_user = user;
                    category.date_create = CsnFunctions.now();
                    category.icon = categoryDto.icon;
                    category.name_category = categoryDto.name_category;
                    category.status = status;
                    category.category_id = _admCategory.persist(module_id, category);
                    
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = category.category_id;
                    scope.Complete();

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return new NoContentResult();
            }
        }

        [HttpPut("{module_id}/category/{category_id}")]
        public IActionResult updateCategory([FromBody] AdmCategoryDto categoryDto, [FromRoute] int category_id)
        {
            try
            {
                if (category_id != categoryDto.category_id)
                {
                    response.code = "400";
                    response.msg = "Incorrect ID";
                    return new BadRequestObjectResult(response);
                }

                AdmCategory category = new AdmCategory();
                using (var scope = new TransactionScope())
                {
                    category.category_id = _admCategory.update(categoryDto);
                    scope.Complete();
                }

                response.msg = "Success";
                response.code = "Ok";
                response.id = category.category_id;
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                response.msg = "Error";
                response.code = "Fail";
                return StatusCode(400, response);
            }
        }
    }
}