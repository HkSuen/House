using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Common;
using House.IService.Model.Enum;
using House.IService.Users;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace House.Service.Users
{
    public class UsersSvc :IUsersSvc
    {
        private IDataConfig _db;
        public UsersSvc(IDataConfig config)
        {
            this._db = config;
        }

        public int WXRegister(string phone, string type, string openId)
        {
            int state = -1;
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(phone) ||string.IsNullOrEmpty(openId))
            {
                return state; //参数不正确
            }
            if(type == URole.Admin.GetEnumDescription()) // admin 
            {
                UpdateOpenId<ts_uidp_userinfo>(phone, openId, GetUserSingleByPhone, UpdateUserInfo, out state);
            }
            if (type == URole.Inspector.GetEnumDescription()) // inspector 
            {
                UpdateOpenId<wy_region_director>(phone, openId, GetDirSingleByPhone, UpdateRegionDirector,out state);
            }
            if (type == URole.Merchant.GetEnumDescription()) // merchant 
            {
                UpdateOpenId<wy_shopinfo>(phone, openId, GetShopSingleByPhone, UpdateShopInfo,out state);
            }
            return state;
        }

        private bool UpdateOpenId<T>(string phone,string openId,Func<string,T> Get,Func<T,bool> Update,out int state)
        {
            dynamic UserInfo = Get(phone);
            if (null != UserInfo)
            {
                UserInfo.WX_OPEN_ID = openId;
                if (Update(UserInfo))
                {
                    state = 1; //更新成功
                    return true;
                }
                state = 2; //更新失败
                return false;
            }
            state = 3; //当前手机号码不存在
            return false;
        }

        public wy_shopinfo GetShopSingleByPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }
            return this._db.CurrentDb<wy_shopinfo>().GetSingle(c => c.MOBILE_PHONE.Equals(phone));
        }

        public wy_region_director GetDirSingleByPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }
            return this._db.CurrentDb<wy_region_director>().GetSingle(c => c.MOBILE.Equals(phone));
        }

        public ts_uidp_userinfo GetUserSingleByPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }
            return this._db.CurrentDb<ts_uidp_userinfo>().GetSingle(c => c.PHONE_MOBILE.Equals(phone));
        }

        public bool UpdateShopInfo(wy_shopinfo shopinfo)
        {
            return this._db.CurrentDb<wy_shopinfo>().Update(shopinfo);
        }

        public bool UpdateRegionDirector(wy_region_director director)
        {
            return this._db.CurrentDb<wy_region_director>().Update(director);
        }

        public bool UpdateUserInfo(ts_uidp_userinfo userinfo)
        {
            return this._db.CurrentDb<ts_uidp_userinfo>().Update(userinfo);
        }

    }
}
