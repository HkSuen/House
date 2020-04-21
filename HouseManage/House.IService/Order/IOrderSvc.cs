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
        Dictionary<string, object> GetPayParamsByWxModel(wy_wxpay wxpay);

        /// <summary>
        /// 根据已经生成的订单生成支付参数
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="prePayId"></param>
        /// <param name="PaySecret"></param>
        /// <returns></returns>

        Dictionary<string, object> GetPayParamsByWxModel(string appId,string prePayId,string PaySecret);

        /// <summary>
        /// 获取支付的订单根据主键。
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        wy_wxpay GetWxModelById(string OrderId);

        /// <summary>
        ///  根据recoreId 及其他参数获取支付订单信息
        /// </summary>
        /// <param name="recoredId">recoredId</param>
        /// <param name="HouseId">房屋ID</param>
        /// <param name="UserID">商户ID</param>
        /// <param name="OpenId">用户ID</param>
        /// <param name="Type">缴费类型 备注:0物业费 1水费 2 电费</param>
        /// <returns></returns>
        wy_wxpay FindSingle(string recoredId,string HouseId,string UserId,string OpenId);

        /// <summary>
        /// 根据v_pay_record 生成 支付订单信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        wy_wxpay GetWxPay(OrderDto record);

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="wxpay"></param>
        /// <returns></returns>
        int Inert(wy_wxpay wxpay);

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
        wy_wxpay GetWxOrderDetail(string Id);
    }
}
