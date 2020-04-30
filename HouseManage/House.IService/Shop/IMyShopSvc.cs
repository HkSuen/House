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

        /// <summary>
        /// 获取最新的一条未缴费提醒数据
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        v_pay_record GetPayReminder(string openId);

        /// <summary>
        /// 获取缴费通知单
        /// </summary>
        /// <param name="OpenId"></param>
        /// <param name="Type"></param>
        /// <param name="payState"></param>
        /// <param name="page"></param>
        /// <returns></returns>

        List<wy_pay_record> GetPayRecord(string OpenId, string Type, int? payState,PageModel page);

        /// <summary>
        /// 根据水表编号获取水的最新信息
        /// </summary>
        /// <param name="wId"></param>
        /// <returns></returns>
        wy_w_amount GetWater(string WId);

        /// <summary>
        /// 根据采集器ID和电的ID获取最新的电数据
        /// </summary>
        /// <param name="CollectId"></param>
        /// <param name="ElectricityId"></param>
        /// <returns></returns>
        wy_ele_balance GetElectricity(string CollectId, string ElectricityId);

    }
}
