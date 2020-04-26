using Data.MSSQL.Model.Data;
using House.IService.Model.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Order
{
    public interface IOrderSvc
    {
        /// <summary>
        /// 生成新订单并生成支付参数
        /// </summary>
        /// <param name="wxpay"></param>
        /// <returns></returns>
        Dictionary<string, object> GetPayParamsByWxModel(wy_wx_pay wxpay);

        /// <summary>
        /// 根据已经生成的订单生成支付参数
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="prePayId"></param>
        /// <param name="PaySecret"></param>
        /// <returns></returns>

        Dictionary<string, object> GetPayParamsByWxModel(string appId,string prePayId,string PaySecret);

        /// <summary>
        /// 获取支付的订单根据微信Order
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        wy_wx_pay GetWxPayById(string OrderId);

        /// <summary>
        ///  根据recoreId 及其他参数获取支付订单信息
        /// </summary>
        /// <param name="recoredId">recoredId</param>
        /// <param name="HouseId">房屋ID</param>
        /// <param name="UserID">商户ID</param>
        /// <param name="OpenId">用户ID</param>
        /// <param name="Type">缴费类型 备注:0物业费 1水费 2 电费</param>
        /// <returns></returns>
        wy_wx_pay FindSingle(string recoredId,string HouseId,string UserId,string OpenId);

        /// <summary>
        /// 根据v_pay_record 生成 支付订单信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        wy_wx_pay GetWxPay(OrderDto record);

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="wxpay"></param>
        /// <returns></returns>
        int Inert(wy_wx_pay wxpay);

        /// <summary>
        /// 更新订单状态信息
        /// </summary>
        /// <param name="wxpay"></param>
        /// <returns></returns>
        int Update(wy_wx_pay wxpay);


        /// <summary>
        /// 获取Recore提醒信息
        /// </summary>
        /// <param name="recoredId"></param>
        /// <param name="HouseId"></param>
        /// <returns></returns>
        v_pay_record GetRecord(string recordId, string HouseId);

        /// <summary>
        /// 获取Recore提醒信息转成订单数据
        /// </summary>
        /// <param name="recoredId"></param>
        /// <param name="HouseId"></param>
        /// <returns></returns>
        OrderDto GetWxPay(string recordId, string HouseId);

        /// <summary>
        /// 根据参数获取支付展示详情
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="HouseId"></param>
        /// <param name="RecordId"></param>
        /// <returns></returns>
        List<object> GetPayDetails(string UserId,string HouseId,string RecordId);

        /// <summary>
        /// 根据订单ID获取微信支付的详细内容
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        wy_wx_pay GetWxOrderDetail(string Id);

        /// <summary>
        /// 插入水表数据
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        int InsertW_Pay(wy_w_pay waterOrder);

        /// <summary>
        /// 插入电的数据
        /// </summary>
        /// <param name="Balance"></param>
        /// <returns></returns>
        int InsertElectricity(wy_ele_recharge electricityOrder);


        /// <summary>
        /// 获取水的单价
        /// </summary>
        /// <param name="WaterKey"></param>
        /// <returns></returns>
        double GetUnitPrice(string WaterKey);

        /// <summary>
        /// 更新pay_record提醒表中的状态信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        int UpdateRecoredJFZT(wy_pay_record record);

        /// <summary>
        /// 根据条件获取水费单是否已经支付
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        int W_PayCount(string OrderId);

        /// <summary>
        /// 根据条件获取电费单是否已经支付
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        int ElectricityCount(string OrderId);

    }
}
