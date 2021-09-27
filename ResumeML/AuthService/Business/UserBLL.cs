using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthService
{
    public class UserBLL
    {
        private UserDAL _user;
        private PermissionDAL _permission;
        public UserBLL(PermissionDAL permission, UserDAL user)
        {
            _permission = permission;
            _user = user;
        }
        private string[] GetUserRoles(long uid)
        {
            List<string> roles = _permission.GetRolePermissions(uid);
            List<UserPermission> usr_permissions = _permission.GetUserPermissions(uid);

            HashSet<string> hs = new HashSet<string>();
            foreach(string s in roles)
            {
                hs.Add(s);
            }
            foreach(UserPermission up in usr_permissions)
            {
                if (up.RightType == 1)
                {
                    hs.Remove(up.RightCode);
                }
                else
                {
                    hs.Add(up.RightCode);
                }
            }
            string[] rt = new string[hs.Count];
            hs.CopyTo(rt);
            return rt;
        }
        public virtual BusResponse<Data_UserInfo> GetUserInfo(long uid)
        {
            AdminInfo info = _user.GetAdminById(uid);
            if (info == null)
            {
                return BusResponse<Data_UserInfo>.Error(-101, "用户信息不存在");
            }
            Data_UserInfo usrInfo = new Data_UserInfo();
            usrInfo.name = info.RealName;
            usrInfo.introduction = info.Introduction;
            usrInfo.avatar = info.Avatar;
            usrInfo.roles = GetUserRoles(uid);
            return BusResponse<Data_UserInfo>.Success(usrInfo);
        }
    }
}
