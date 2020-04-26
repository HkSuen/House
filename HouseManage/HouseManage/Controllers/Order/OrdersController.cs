using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.MSSQL.Model.Data;
using House.IService.Common.XML;
using House.IService.Order;
using HouseManage.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using House.IService.Common;
using House.IService.Model.Enum;
using Microsoft.Extensions.Logging;

namespace HouseManage.Controllers.Order
{
    public class OrdersController : ControllerBase
    {
        private IOrderSvc _order = null;
        private IXMLHelperSingle _xml = null;
        private IWeChatPaySingle _pay = null;
        ILogger<OrdersController> _log;
        public OrdersController(IOrderSvc order, IXMLHelperSingle xml, IWeChatPaySingle pay, ILogger<OrdersController> log)
        {
            _order = order;
            _xml = xml;
            _pay = pay;
            _log = log;
        }

        public ActionResult Order(string r, string f, string u)
        {
            if (string.IsNullOrEmpty(r) || string.IsNullOrEmpty(f) || string.IsNullOrEmpty(u))
            {
                return Error("请求参数错误！");
            }
            dynamic DetailInfo = this._order.GetPayDetails(u, f, r).FirstOrDefault();
            if (null == DetailInfo)
            {
                return Error("未找到订单信息！");
            }
            DetailInfo.JFLXName = Fee.GetKey(Convert.ToInt32(DetailInfo.JFLX));
            ViewBag.Payee = CommonFiled.MchName(Convert.ToInt32(DetailInfo.JFLX));
            return View(DetailInfo);
        }

        public JsonResult CreateOrder(string recordId, string houseId, string UId, int WNum, int EPrice)
        {
            if (string.IsNullOrEmpty(recordId) || string.IsNullOrEmpty(houseId) || string.IsNullOrEmpty(UId))
            {
                return Data(ResultCode.PARAMS_IS_NULL, null, ResultCode.PARAMS_IS_NULL.GetEnumDescription());
            }
            //1.订单查询有无此数据,无数据默认创建新数据
            wy_wx_pay pay = this._order.FindSingle(recordId, houseId, UId, OpenID);
            //原订单失效，异步更改状态
            bool UpdateTime = pay != null && DateTime.Now > pay.PREPAY_ENDTIME;
            if (pay == null || UpdateTime)
            {
                if (UpdateTime) //异步更新过期订单信息，并生成新的订单信息
                {
                    Task.Run(() =>
                    {
                        pay.STATUS = 1;
                        this._order.Update(pay);
                    });
                }
                //获取提醒订单信息
                var PayRecord = this._order.GetWxPay(recordId, houseId);
                pay = this._order.GetWxPay(PayRecord);
                pay.USER_IP = UserIP;
                //存储水量以及电量
                if (pay.FEE_TYPES != 0) //非物业费
                {
                    //非物业默认记录电的价格
                    pay.TOTAL_FEE = EPrice * 100; //从元转变为分
                    if (pay.FEE_TYPES == 1)
                    {
                        // 为水的时候需要记录单价
                        pay.UNIT_PRICE = Convert.ToInt32(this._order.GetUnitPrice(CommonFiled.UnitPriceWaterKey) * 100);
                        pay.AMOUNT = WNum;
                        pay.TOTAL_FEE = Convert.ToInt32(pay.UNIT_PRICE * 100 * pay.AMOUNT);
                    }
                }
                if (pay.TOTAL_FEE <= 0) //如果订单生成为0元，直接视为无效订单，禁止生成。
                {
                    return Data(ResultCode.PARAMS_IS_INVALID);
                }
                //将订单存到数据库
                if (this._order.Inert(pay) <= 0)
                {
                    return Data(ResultCode.DATA_IS_WRONG);
                };
            }
            // 这里需要过滤订单信息
            return Data(ResultCode.SCCUESS, new { sign = this._order.GetPayParamsByWxModel(pay) });
        }

        /// <summary>
        /// 微信异步通知支付
        /// </summary>
        /// <returns></returns>
        public ActionResult PayResult()
        {
            string result = "<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>";
            bool resultBool = false;
            try
            {
                _log.LogInformation("微信支付异步回调请求开始");
                Stream stream = Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string content = Encoding.UTF8.GetString(buffer);
                _log.LogInformation("微信支付异步回调请求[Body]:" + content);
                Dictionary<string, object> valuePairs = this._xml.XmlStrToDic(content);
                //验证返回code
                if (valuePairs["return_code"].ToString() == CommonFiled.SUCCESS
                    && CommonFiled.SUCCESS == valuePairs["result_code"].ToString())
                {
                    //校验签名
                    if (this._pay.CheckWxSign(valuePairs))
                    {
                        if (valuePairs.ContainsKey("out_trade_no") || valuePairs.ContainsKey("cash_fee"))
                        {
                            //验证订单信息金额
                            wy_wx_pay pay = this._order.GetWxPayById(valuePairs["out_trade_no"].ToString());
                            if (pay.STATUS == 0 && pay.TOTAL_FEE == Convert.ToInt32(valuePairs["cash_fee"]))
                            {
                                pay.STATUS = 2;
                                pay.PAY_TIME = DateTime.Now;
                                pay.STATUS_REMARK = content;
                                if (this._order.Update(pay) > 0)
                                {
                                    //返回信息
                                    Task.Run(() => //异步操作，防止返回超时，被调用两次
                                    {
                                        WXPaidAfter(pay);
                                    });
                                    resultBool = true;
                                    result = string.Format(result, CommonFiled.SUCCESS, CommonFiled.SUCCESS);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "微信回调PayError!!");
                result = string.Format(result, CommonFiled.FAIL, ex.Message);
                return Content(result, "text/xml");
            }
            if (!resultBool)
            {
                result = string.Format(result, CommonFiled.FAIL, CommonFiled.FAIL);
            }
            _log.LogInformation("微信支付异步回调请求[Retrun]:" + result);
            return Content(result, "text/xml");
        }

        //[Authorize(Roles ="Admin")]
        public ActionResult OrderDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Exception("没有查询到单据信息");
            }
            wy_wx_pay Model = this._order.GetWxOrderDetail(id);
            ViewBag.Type = CommonFiled.FeeTypeName(Model.FEE_TYPES);
            ViewBag.MoneyNum = CommonFiled.CmycurD(Model.TOTAL_FEE / 10);
            return View(Model);
        }

        public JsonResult GetUnitPriceOfWater()
        {
            return OK(this._order.GetUnitPrice(CommonFiled.UnitPriceWaterKey));
        }


        #region 支付成功后，后续处理逻辑

        /// <summary>
        /// 微信支付的后续操作
        /// </summary>
        /// <returns></returns>
        private bool WXPaidAfter(string OrderId)
        { //根据订单ID
            try
            {
                return WXPaidAfter(this._order.GetWxPayById(OrderId));
            }
            catch (Exception ex)
            {
                _log.LogError(ex,"微信支付成功后逻辑处理错误：[OrderID]:" + OrderId);
                return false;
            }
        }

        private bool WXPaidAfter(wy_wx_pay Order)
        { //根据订单ID
            try
            {
                _log.LogInformation($"支付成功，更新类型：{CommonFiled.FeeTypeName(Order.FEE_TYPES)}");
                //如果类型为物业费 // recoreId 不为空，证明为提醒订单，需要修改record表中的数据
                if (Order != null && (string.IsNullOrEmpty(Order.RECORD_ID)))
                {
                    return PropertyCost(Order);
                }
                //如果类型为水费，
                if (CommonFiled.EnumFeeTypes(Order.FEE_TYPES) == FeeTypes.Water)
                {
                    return WaterCost(Order);
                }
                //如果类型为电费
                if (CommonFiled.EnumFeeTypes(Order.FEE_TYPES) == FeeTypes.Electricity)
                {
                    return ElectricityCost(Order);
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex,"微信支付成功后逻辑处理错误：[OrderID]:" + Order.ORDER_ID);
                return false;
            }
        }
        #region 支付成功后，水电物业费处理
        private bool PropertyCost(wy_wx_pay Order) {
            _log.LogInformation($"【wy_pay_record】准备更新:{Order.RECORD_ID}");
            var Res = this._order.UpdateRecoredJFZT(new wy_pay_record()
            {
                RECORD_ID = Order.RECORD_ID,
                JFZT = 1,
                JFRQ = DateTime.Now
            });
            if (Res > 0)
            {
                _log.LogInformation($"【wy_pay_record】更新成功:{Order.RECORD_ID}");
            }
            else
            {
                _log.LogError($"【wy_pay_record】更新失败:{Order.RECORD_ID}，订单：{Order.ID}");
                return false;
            }
            return true;
        }
        private bool WaterCost(wy_wx_pay Order)
        {
            _log.LogInformation($"【wy_w_pay】准备插入:{Order.ID}");
            var Pay = this._order.W_PayCount(Order.ID);
            if (Pay <= 0) // 未生成过订单
            {
                var Res = this._order.InsertW_Pay(new wy_w_pay()
                {
                    GUID = Order.ID,
                    MeterID = Order.TYPES_ID,
                    RechargeVolume = Convert.ToDouble(Order.AMOUNT),
                    AddAmount = Convert.ToDouble(Order.TOTAL_FEE * 100),
                    UnitPrice = Order.UNIT_PRICE,
                    CreateDate = DateTime.Now
                });
                if (Res > 0)
                {
                    _log.LogInformation($"【wy_w_pay】更新成功:{Order.ID}");
                }
                else
                {
                    _log.LogError($"【wy_w_pay】更新失败:{Order.ID}");
                    return false;
                }
            }
            return true;
        }
        private bool ElectricityCost(wy_wx_pay Order)
        {
            _log.LogInformation($"【wy_ele_recharge】准备插入:{Order.ID}");
            var Pay = this._order.ElectricityCount(Order.ID);
            if (Pay <= 0)
            {
                var Res = this._order.InsertElectricity(new wy_ele_recharge()
                {
                    id = Order.ID,
                    CreateDate = DateTime.Now,
                    address = Order.TYPES_ID,
                    cid = Order.TYPES_ID_ELE_COLL,
                    Cost = Convert.ToDouble(Order.TOTAL_FEE * 100),
                });
                if (Res > 0)
                {
                    _log.LogInformation($"【wy_ele_recharge】更新成功:{Order.ID}");
                }
                else
                {
                    _log.LogError($"【wy_ele_recharge】更新失败:{Order.ID}");
                    return false;
                }
            }
            return true;
        }
        #endregion

        #endregion

    }
}