using System;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using care_core.Controllers.util;
using System.Linq;
using care_core.model;
using Serilog;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using care_core.repository.interfaces;
using Microsoft.Extensions.Logging;
using care_core.dto.AdmPermission;

namespace care_core.controllers
{
    [Route("rest/users/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmPermissionController : ControllerBase
    {
        private readonly IAdmPermission _admPermission;
        private readonly ILogger<AdmPermissionController> _logger;
        private readonly EntityDbContext _dbContext;
        private JsonResponse response;

        public AdmPermissionController(EntityDbContext dbContext, IAdmPermission admPermission, ILogger<AdmPermissionController> logger)
        {
            _admPermission = admPermission;
            _logger = logger;
            _dbContext = dbContext;
            response = new JsonResponse();
        }

        [HttpGet("{user_id}/permissions")]
        public IActionResult GetPermissions([FromRoute] int user_id)
        {
            try
            {
                AdmUser user = _dbContext.admUsers.Find(user_id);
                if(user == null){
                    response.code = "400";
                    response.msg = "User not found";
                    return new BadRequestObjectResult(response);
                }

                IEnumerable<Object> list = _admPermission.getAll(user_id);
                return new OkObjectResult(list);
                
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
            
        }

        [HttpPut("{user_id}/permissions")]
        public IActionResult PutPermissions(int user_id, List<AdmGroupPermissionDto> admGroupPermissionDto)
        {
            try
            {
                using (var scope = new TransactionScope())
                {

                    foreach (var admGPD in admGroupPermissionDto)
                    {
                        foreach (var listP in admGPD.permissions)
                        {
                            bool upd_hasOp = listP.has_permissions;
                            AdmUserPermission userPermission = _dbContext.admUserPermissions
                            .Where(x => x.user.user_id == user_id && x.module.module_id == admGPD.module_id
                                    && x.permission.permission_id == listP.permission_id)
                            .SingleOrDefault();

                            //Actualizar o Crear
                            if(userPermission != null){
                                _admPermission.UpdUserPermissions(userPermission, upd_hasOp);
                            }else if(userPermission == null){
                                _admPermission.persist(upd_hasOp, user_id, admGPD.module_id, listP.permission_id);
                            }
                        }

                    }

                    scope.Complete();
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400, "Record no found");
            }
        }
    }
}