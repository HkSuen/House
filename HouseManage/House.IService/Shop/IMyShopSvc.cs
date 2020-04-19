using Data.MSSQL.Model.Data;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Shop
{
    public interface IMyShopSvc
    {
        /// <summary>
        /// 根据OPENID获取当前用户的商铺信息
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        List<object> GetShopDetails(string OpenId);

        /// <summary>
        /// 获取缴费提醒通知单
        /// </summary>
        /// <param name="OpenId"></param>
        /// <param name="Type"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        List<v_pay_record> GetPayReminder(string OpenId, string Type, PageModel page);
    }
}
