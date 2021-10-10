using Common;
using System;
using System.Collections.Generic;

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
            List<string> roles = _permission.GetRolePermissionsByUser(uid);
            List<MZ_UserPermission> usr_permissions = _permission.GetUserPermissions(uid);

            HashSet<string> hs = new HashSet<string>();
            foreach (string s in roles)
            {
                hs.Add(s);
            }
            foreach (MZ_UserPermission up in usr_permissions)
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
            MZ_AdminInfo info = _user.GetAdminById(uid);
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
        public virtual BusResponse<List<MZ_Role>> GetAllRole()
        {
            return BusResponse<List<MZ_Role>>.Success(_user.GetAllRole());
        }
        public virtual BusResponse<long> AddRole(MZ_Role role)
        {
            try
            {
                long id = _user.AddRole(role);
                return BusResponse<long>.Success(id);
            }
            catch (Exception ex)
            {
                return BusResponse<long>.Error(-12, ex.Message);
            }

        }
        public virtual BusResponse<string> UpdateRole(MZ_Role role, long uid)
        {
            try
            {
                bool canupdate = _permission.IsRoot(uid);
                if (!canupdate)
                {
                    MZ_Role old = _user.GetRole(role.RoleID);
                    canupdate = old.CreateUserId.Equals(uid);
                }
                if (!canupdate)
                {
                    return BusResponse<string>.Error(-11, "无权编辑当前角色");
                }

                _user.UpdateRole(role);
                return BusResponse<string>.Success(null);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(-12, ex.Message);
            }
        }
        public virtual BusResponse<string> DeleteRole(long id, long uid)
        {
            try
            {
                bool canupdate = _permission.IsRoot(uid);
                if (!canupdate)
                {
                    MZ_Role old = _user.GetRole(id);
                    canupdate = old.CreateUserId.Equals(uid);
                }
                if (!canupdate)
                {
                    return BusResponse<string>.Error(-11, "无权删除当前角色");
                }

                _user.DeleteRole(id);
                return BusResponse<string>.Success(null);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(-12, ex.Message);
            }
        }
    }
}
