using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace AuthService
{
    [Des("权限设置")]
    public class Permission : TABaseController
    {
        private PermissionBLL _permission;
        public Permission(PermissionBLL permission)
        {
            _permission = permission;
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
            return Success();
        }

        [HttpPost]
        [Des("新增角色")]
        public AjaxResult PostRole()
        {
            return Success();
        }
        [HttpPut]
        [Des("修改角色")]
        public AjaxResult PutRole()
        {
            return Success();
        }
        [HttpDelete]
        [Des("删除角色")]
        public AjaxResult DeleteRole()
        {
            return Success();
        }
    }
}
