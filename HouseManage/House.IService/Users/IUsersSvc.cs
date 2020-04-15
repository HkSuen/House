using Data.MSSQL.Model.Data;
using House.IService.Model.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Users
{
    public interface IUsersSvc
    {
        /// <summary>
        /// 微信用户首次绑定
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="type"></param>
        /// <param name="openId"></param>
        /// <returns> -1:参数不正确， 1:更新成功 ， 2:更新失败 , 3:当前手机号码不存在 </returns>
        int WXRegister(string phone,string type,string openId);

        /// <summary>
        /// 根据手机号码获取商户信息
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        wy_shopinfo GetShopSingleByPhone(string phone);

        /// <summary>
        /// 根据手机号码获取商户信息
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        wy_region_director GetDirSingleByPhone(string phone);

        /// <summary>
        /// 根据手机号码获取商户信息
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        ts_uidp_userinfo GetUserSingleByPhone(string phone);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="shopinfo"></param>
        /// <returns></returns>
        bool UpdateShopInfo(wy_shopinfo shopinfo);

        /// <summary>
        /// 更新检查员的表格
        /// </summary>
        /// <param name="director"></param>
        /// <returns></returns>
        bool UpdateRegionDirector(wy_region_director director);

        /// <summary>
        /// 更新管理员的信息
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        bool UpdateUserInfo(ts_uidp_userinfo userinfo);

        /// <summary>
        /// 根据OPENID查询当期用户
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        UserDto FindUserByOpenId(string OpenId);
    }
}
