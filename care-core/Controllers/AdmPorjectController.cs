using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/projects")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmProjectController : ControllerBase
    {
        private readonly IAdmProject _admProject;
        private readonly ILogger<AdmProjectController> _logger;
        private JsonResponse response;
        
        public AdmProjectController(IAdmProject admProject, ILogger<AdmProjectController> logger)
        {
            _admProject = admProject;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable <Object> projects = _admProject.getAll();
            return new OkObjectResult(projects);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] long id)
        {
            Object project = _admProject.getById(id);
            return new OkObjectResult(project);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdmProject admproject)
        {           
            admproject.project_id = 0;
            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _admProject.persist(admproject);
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = id;
                    scope.Complete();

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return StatusCode(400);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] AdmProject admProject, [FromRoute] int id)
        {
            if(id != admProject.project_id){
                response.msg = "Incorrect ID";
                response.code = "Bad Request";
                response.id = id;

                return StatusCode(400, response);
            }
            using(var scope =new TransactionScope()){
                _admProject.upd(admProject);
                scope.Complete();
                return new OkResult();
            }
        }

        //***Actividades***
        [HttpPost("activity")]
        public IActionResult CreateProjectActivity([FromBody] AdmProjectActivity admProjectActivity)
        {                
            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _admProject.persist(admProjectActivity);
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = id;
                    scope.Complete();

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return StatusCode(400);
            }
        }

        [HttpGet("{id}/activities")]
        public IActionResult GetAllProjectActivities(int id)
        {
            IEnumerable <Object> projectactivities = _admProject.getAllProyectActivities(id);
            return new OkObjectResult(projectactivities);
        }

        [HttpGet("{id}/activity/{activity_id}")]
        public IActionResult GetByIdProjectActivity([FromRoute] int id, [FromRoute] int activity_id)
        {
            Object projectactivity = _admProject.getByIdActivity(id, activity_id);
            return new OkObjectResult(projectactivity);
        }

        [HttpPut("activity/{activity_id}")]
        public IActionResult PutProyectActivity([FromBody] AdmProjectActivity admProjectActivity, [FromRoute] int activity_id)
        {
            if(activity_id != admProjectActivity.project_activity_id){
                response.msg = "Incorrect ID";
                response.code = "Bad Request";
                response.id =activity_id;

                return StatusCode(400, response);
            }
            using(var scope =new TransactionScope()){
                _admProject.updProjectActivity(admProjectActivity);
                scope.Complete();
                return new OkResult();
            }
        }
    }
}