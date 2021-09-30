using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace AuthService
{
    [Des("权限设置")]
    public class Permission : TABaseController
    {
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public AjaxResult GetPermissions()
        {
            List<DescribeInfo> tlist = Context.Application.FindAllDescribe();
            return Success();
        }
        [HttpGet]
        [Des("查看角色")]
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
