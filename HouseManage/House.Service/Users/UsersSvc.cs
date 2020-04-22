using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Common;
using House.IService.Model.Dto;
using House.IService.Model.Enum;
using House.IService.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace House.Service.Users
{
    public class UsersSvc : IUsersSvc
    {
        private IDataConfig _db;
        public UsersSvc(IDataConfig config)
        {
            this._db = config;
        }

        public int WXRegister(string phone, string type, string openId)
        {
            int state = -1;
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(openId))
            {
                return state; //参数不正确
            }
            if (type == URole.Admin.GetEnumDescription()) // admin 
            {
                UpdateOpenId<ts_uidp_userinfo>(phone, openId, GetUserSingleByPhone, UpdateUserInfo, out state);
            }
            if (type == URole.Inspector.GetEnumDescription()) // inspector 
            {
                UpdateOpenId<wy_region_director>(phone, openId, GetDirSingleByPhone, UpdateRegionDirector, out state);
            }
            if (type == URole.Merchant.GetEnumDescription()) // merchant 
            {
                UpdateOpenId<wy_shopinfo>(phone, openId, GetShopSingleByPhone, UpdateShopInfo, out state);
            }
            return state;
        }

        private bool UpdateOpenId<T>(string phone, string openId, Func<string, T> Get, Func<T, bool> Update, out int state)
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
            return this._db.CurrentDb<wy_shopinfo>()
                .GetSingle(c => c.MOBILE_PHONE.Equals(phone));
        }

        public wy_region_director GetDirSingleByPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }
            return this._db.CurrentDb<wy_region_director>()
                .GetSingle(c => c.MOBILE.Equals(phone));
        }

        public ts_uidp_userinfo GetUserSingleByPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }
            return this._db.CurrentDb<ts_uidp_userinfo>()
                .GetSingle(c => c.PHONE_MOBILE.Equals(phone));
        }

        public bool UpdateShopInfo(wy_shopinfo shopinfo)
        {
            return UpdateInfo<wy_shopinfo>(shopinfo,
                c => new { c.OPEN_ID, c.MOBILE_PHONE },
                c => new { c.MOBILE_PHONE }) > 0;
        }

        public bool UpdateRegionDirector(wy_region_director director)
        {
            return UpdateInfo<wy_region_director>(director,
                c => new { c.WX_OPEN_ID, c.MOBILE },
                c => new { c.MOBILE }) > 0;
        }

        public bool UpdateUserInfo(ts_uidp_userinfo userinfo)
        {
            return UpdateInfo<ts_uidp_userinfo>(userinfo,
                c => new { c.WX_OPEN_ID, c.PHONE_MOBILE },
                c => new { c.PHONE_MOBILE }) > 0;
        }

        private int UpdateInfo<T>(T shopinfo, Expression<Func<T, object>> updateCols, Expression<Func<T, object>> whereCols)
            where T : class, new()
        {
            return this._db.Db().Updateable<T>(shopinfo)
                .UpdateColumns(updateCols)
                .WhereColumns(whereCols)
                .ExecuteCommand();
        }

        public UserDto FindUserByOpenId(string OpenId)
        {
            string Sql = @"SELECT *  FROM (
SELECT USER_ID AS ID, PHONE_MOBILE AS PHONE,WX_OPEN_ID AS OPENID, 'Admin' AS 'AUTHORITY', '1' AS 'Top' FROM ts_uidp_userinfo
  UNION
 SELECT RD_ID AS ID,MOBILE AS PHONE,WX_OPEN_ID AS OPENID,'Inspector' AS 'AUTHORITY', '2' AS 'Top' FROM wy_region_director
  UNION
 SELECT CZ_SHID AS ID,MOBILE_PHONE AS PHONE,OPEN_ID AS OPENID,'Merchant' AS 'AUTHORITY', '3' AS 'Top' FROM wy_shopinfo) AS uModel
    WHERE OPENID =  @OpenId ORDER BY TOP ASC";
            return this._db.Sql().SqlQuery<UserDto>(Sql,new { OpenId }).FirstOrDefault();
        }

    }
}
