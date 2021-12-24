using System;
using System.Collections.Generic;
using System.Linq;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using care_core.dto.AdmPermission;

using Microsoft.EntityFrameworkCore;

namespace care_core.repository
{
    public class AdmPermissionRepository : IAdmPermission
    {
        private readonly EntityDbContext _dbContext;

        public AdmPermissionRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Object> getAll(int user_id)
        {
            //Se obtiene el listado de los modulos
            var listmodules = _dbContext.admModules.Where(x=>x.status.typology_id==CareConstants.DEFAULT_STATUS).ToArray();
            var listpermissions = _dbContext.admPermissions.Where(x=>x.status.typology_id==CareConstants.DEFAULT_STATUS).ToArray();

            //Lista de los premisos que se devolvera agrupado por modulos
            List<AdmGroupPermissionDto> listGPermissions = new List<AdmGroupPermissionDto>();


            foreach (var item in listmodules)
            {
                var listpermiss = _dbContext.admUserPermissions
                .Where(x => x.user.user_id == user_id && x.module.module_id == item.module_id)
                .Select(
                    listsp => new AdmUserPermissionDto
                    {
                        has_permissions = listsp.has_permissions,
                        permission_id = listsp.permission.permission_id,
                        name_permission = listsp.permission.name_permission,
                        alias = listsp.permission.alias
                    }
                ).ToArray();

                //Si el usuario no tiene permisos se le agregaran
                if (listpermiss.Length == 0)
                {
                    List<AdmUserPermissionDto> listpermiss2 = new List<AdmUserPermissionDto>();

                    foreach (var permis in listpermissions)
                    {
                        listpermiss2.Add(new AdmUserPermissionDto()
                        {
                            has_permissions = false,
                            permission_id = permis.permission_id,
                            name_permission = permis.name_permission,
                            alias = permis.alias
                        });
                    }
                    //Se agregan los permisos
                    listGPermissions.Add(new AdmGroupPermissionDto()
                    {
                        module_id = item.module_id,
                        name_module = item.name_module,
                        permissions = listpermiss2.ToArray()
                    });

                }
                //Se agregan los permisos que tenga el usuario asignado
                if (listpermiss.Length != 0)
                {
                    listGPermissions.Add(new AdmGroupPermissionDto()
                    {
                        module_id = item.module_id,
                        name_module = item.name_module,
                        permissions = listpermiss.ToArray()
                    });
                }

            }

            return listGPermissions;
        }

        public int persist(bool upd_hasOp, int user_id, int module_id, int permission_id)
        {
            AdmUserPermission admUserPermission = new AdmUserPermission();

            AdmUser user = _dbContext.admUsers.Find(user_id);
            AdmModule module = _dbContext.admModules.Find(module_id);
            AdmPermission permission = _dbContext.admPermissions.Find(permission_id);

            
            admUserPermission.has_permissions=upd_hasOp;
            admUserPermission.user = user;
            admUserPermission.module = module;
            admUserPermission.permission = permission;

            _dbContext.Add(admUserPermission);
            
            save();

            return admUserPermission.user_permission_id;

        }

        public void UpdUserPermissions(AdmUserPermission admUserPermission, bool upd_hasOp)
        {
            AdmUserPermission updUserPermission = _dbContext.admUserPermissions.Find(admUserPermission.user_permission_id);

            updUserPermission.has_permissions = upd_hasOp;

            _dbContext.Entry(updUserPermission).State = EntityState.Modified;
            save();

        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}
