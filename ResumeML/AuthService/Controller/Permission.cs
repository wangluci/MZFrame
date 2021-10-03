using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace AuthService
{
    [Des("权限设置")]
    public class Permission : TABaseController
    {
        private PermissionBLL _permission;
        private UserBLL _user;
        public Permission(PermissionBLL permission, UserBLL user)
        {
            _permission = permission;
            _user = user;
        }
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public AjaxResult GetPermissions()
        {
            List<DescribeInfo> tlist = Context.Application.FindAllDescribe();
            Data_Permission parent = null;
            List<Data_Permission> datalist = new List<Data_Permission>();
            foreach (DescribeInfo db in tlist)
            {
                if (parent == null)
                {
                    Data_Permission dpparent = new Data_Permission();
                    dpparent.code = db.Code;
                    dpparent.title = db.Name;
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
                        parent.children.Add(dper);
                    }
                    else
                    {
                        Data_Permission dpparent = new Data_Permission();
                        dpparent.code = db.Code;
                        dpparent.title = db.Name;
                        dpparent.children = new List<Data_Permission>();
                        parent = dpparent;
                        datalist.Add(dpparent);
                    }
                }
            }
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
        public AjaxResult PostRole(string name, string description)
        {
            ClientTokenInfo dataInfo = Context.Items["AuthToken"] as ClientTokenInfo;

            MZ_Role role = new MZ_Role();
            role.RoleName = name;
            role.RoleDesc = description;
            role.RoleType = 0;
            role.CreateUserId = dataInfo.UserId;
            role.CreateDate = DateTime.Now;
            return _user.AddRole(role).ToAjaxResult(Context);
        }
        [Des("修改角色")]
        public AjaxResult PutRole(long id, string name, string description)
        {
            ClientTokenInfo dataInfo = Context.Items["AuthToken"] as ClientTokenInfo;

            MZ_Role role = MyAccess.Aop.InterceptFactory.CreateEntityOp<MZ_Role>();
            role.RoleID = id;
            role.RoleName = name;
            role.RoleDesc = description;

            return _user.UpdateRole(role, dataInfo.UserId).ToAjaxResult(Context);
        }
        [Des("删除角色")]
        public AjaxResult DeleteRole(long id)
        {
            ClientTokenInfo dataInfo = Context.Items["AuthToken"] as ClientTokenInfo;
            return _user.DeleteRole(id, dataInfo.UserId).ToAjaxResult(Context);
        }
    }
}
