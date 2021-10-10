using Common;
using System.Collections.Generic;

namespace AuthService
{
    public class PermissionBLL
    {
        private PermissionDAL _permission;
        public PermissionBLL(PermissionDAL permission)
        {
            _permission = permission;
        }
        public virtual BusResponse<List<string>> GetRolePermissions(long role)
        {
            List<string> tlist = _permission.GetRolePermissions(role);
            return BusResponse<List<string>>.Success(tlist);
        }
    }
}
