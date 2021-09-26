using Common;
using System;

namespace AuthService
{
    public class UserBLL
    {
        private UserDAL _user;
        public UserBLL(UserDAL user)
        {
            _user = user;

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

            return BusResponse<Data_UserInfo>.Success();
        }
    }
}
