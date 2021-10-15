using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace AuthService
{
    [Des("权限管理", -1)]
    public class Permission : TABaseController
    {
        private PermissionBLL _permission;
        private UserBLL _user;
        private IOptions<AuthOption> _conf;
        private ITAServices _provider;
        public Permission(ITAServices provider, PermissionBLL permission, UserBLL user, IOptions<AuthOption> conf)
        {
            _provider = provider;
            _permission = permission;
            _user = user;
            _conf = conf;
        }
        /// <summary>
        /// 对权限进行排序
        /// </summary>
        /// <param name="list"></param>
        private void PermissionSortAndLow(List<Data_Permission> list)
        {
            list.Sort((x, y) => { return x.sort - y.sort; });
            foreach (Data_Permission p in list)
            {
                if (p.children != null)
                {
                    PermissionSortAndLow(p.children);
                }
            }
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public AjaxResult GetPermissions()
        {
            List<DescribeInfo> tlist = null;
            switch (_conf.Value.permission_from)
            {
                case 0:
                    tlist = Context.Application.FindAllDescribe();
                    break;
                case 1:
                    //从redis中取，微服务设置成这个,各个服务先调用FindAllDescribe上传权限数据到redis
                    tlist = _provider.GetService<AuthRedisHelper>().HashKeys<DescribeInfo>("MZPermissions");
                    break;
                case 2:
                    //从数据库中取，同1的作用类似
                    break;
                default:
                    tlist = new List<DescribeInfo>();
                    break;
            }
            Data_Permission parent = null;
            List<Data_Permission> datalist = new List<Data_Permission>();
            foreach (DescribeInfo db in tlist)
            {
                if (parent == null)
                {
                    Data_Permission dpparent = new Data_Permission();
                    dpparent.code = db.Code;
                    dpparent.title = db.Name;
                    dpparent.sort = db.Sort;
                    dpparent.children = new List<Data_Permission>();
                    parent = dpparent;
                    datalist.Add(dpparent);
                }
                else
                {
                    if (db.ParentCode == parent.code)
                    {
                        Data_Permission dper = new Data_Permission();
                        dper.code = db.Code;
                        dper.title = db.Name;
                        dper.sort = db.Sort;
                        parent.children.Add(dper);
                    }
                    else
                    {
                        Data_Permission dpparent = new Data_Permission();
                        dpparent.code = db.Code;
                        dpparent.title = db.Name;
                        dpparent.sort = db.Sort;
                        dpparent.children = new List<Data_Permission>();
                        parent = dpparent;
                        datalist.Add(dpparent);
                    }
                }
            }
            PermissionSortAndLow(datalist);
            return Success(MyAccess.Json.Json.Encode(datalist));
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AjaxResult GetRolePermission(long id)
        {
            return _permission.GetRolePermissions(id).ToAjaxResult(Context);
        }
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public AjaxResult GetRoles()
        {
            return _user.GetAllRole().ToAjaxResult(Context);
        }

        [Des("新增角色")]
        public AjaxResult PostRole(MZ_Role role)
        {
            ClientTokenInfo dataInfo = Context.Items["AuthToken"] as ClientTokenInfo;

            role.RoleType = 0;
            role.CreateUserId = dataInfo.UserId;
            role.CreateDate = DateTime.Now;
            return _user.AddRole(role).ToAjaxResult(Context);
        }
        [Des("修改角色")]
        public AjaxResult PutRole(long id, MZ_Role role)
        {
            ClientTokenInfo dataInfo = Context.Items["AuthToken"] as ClientTokenInfo;

            MZ_Role editRole = MyAccess.Aop.InterceptFactory.CreateEntityOp<MZ_Role>();
            editRole.RoleID = id;
            editRole.RoleName = role.RoleName;
            editRole.RoleDesc = role.RoleDesc;
            editRole.permissions = role.permissions;
            return _user.UpdateRole(editRole, dataInfo.UserId).ToAjaxResult(Context);
        }
        [Des("删除角色")]
        public AjaxResult DeleteRole(long id)
        {
            ClientTokenInfo dataInfo = Context.Items["AuthToken"] as ClientTokenInfo;
            return _user.DeleteRole(id, dataInfo.UserId).ToAjaxResult(Context);
        }
    }
}
