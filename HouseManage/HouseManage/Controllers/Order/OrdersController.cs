﻿using System;
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
using House.IService.Model.Dto;
using House.IService.Shop;
using Newtonsoft.Json;
using House.IService.Common.Message;

namespace HouseManage.Controllers.Order
{
    public class OrdersController : ControllerBase
    {
        private IOrderSvc _order = null;
        private IXMLHelperSingle _xml = null;
        private IWeChatPaySingle _pay = null;
        private IMyShopSvc _shop;
        private string SelfPayOrder = "0";
        ILogger<OrdersController> _log;
        public OrdersController(IOrderSvc order, IXMLHelperSingle xml, IWeChatPaySingle pay,
            ILogger<OrdersController> log, IMyShopSvc shop)
        {
            _order = order;
            _xml = xml;
            _pay = pay;
            _log = log;
            _shop = shop;
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
            int type = Convert.ToInt32(DetailInfo.JFLX);
            if (PropertyCosts() && CommonFiled.EnumFeeTypes(type) != FeeTypes.Property)
            {
                return Redirect("../MyShop/Payment?back=0");
            }
            DetailInfo.JFLXName = Fee.GetKey(Convert.ToInt32(DetailInfo.JFLX));
            ViewBag.Payee = CommonFiled.MchName(Convert.ToInt32(DetailInfo.JFLX));
            return View(DetailInfo);
        }

        private bool PropertyCosts()
        {
            return _shop.GetPayReminder(OpenID, ((int)FeeTypes.Property).ToString(),DateTime.Now) > 0;
        }

        public JsonResult CreateOrder(string recordId, string houseId, string UId, int WNum, double EPrice, string Type)
        {
            if (string.IsNullOrEmpty(houseId) || string.IsNullOrEmpty(UId))
            {
                return Data(ResultCode.PARAMS_IS_NULL, null, ResultCode.PARAMS_IS_NULL.GetEnumDescription());
            }
            //1.订单查询有无此数据,无数据默认创建新数据
            wy_wx_pay pay = null;
            if (!string.IsNullOrEmpty(recordId)) {
                pay = this._order.FindSingle(recordId, houseId, UId, OpenID); //自助缴费订单，每次都是新订单
            }
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
                OrderDto PayRecord = null;
                if (string.IsNullOrEmpty(recordId))
                {
                    PayRecord = this._order.GetWxPay(UId,houseId);
                }
                else
                {
                    PayRecord = this._order.GetWxPay(UId,recordId, houseId);
                }
                //校验是否为自助缴费订单
                if (PayRecord.Record == null || PayRecord.Record.RECORD_ID == null)
                {
                    PayRecord.Record = new v_pay_record()
                    {
                        OPEN_ID = OpenID,
                        JFLX = Type,
                        JFJE = EPrice, //默认物业费
                        RECORD_ID = SelfPayOrder //默认0为自助订单，非缴费提醒
                    };
                }
                pay = this._order.GetWxPay(PayRecord);
                pay.USER_IP = UserIP;
                //存储水量以及电量
                if (pay.FEE_TYPES != 0) //非物业费
                {
                    //非物业默认记录电的价格
                    pay.UNIT_PRICE = Convert.ToInt32(this._order.GetUnitPrice(CommonFiled.UnitPriceElectricSetKey) * 100);
                    pay.AMOUNT = Convert.ToInt32(EPrice);
                    pay.TOTAL_FEE = Convert.ToInt32(pay.UNIT_PRICE * pay.AMOUNT);
                    pay.TOTAL_FEE_CH = CommonFiled.CmycurD(Convert.ToDecimal((pay.UNIT_PRICE * pay.AMOUNT) / 100.00));
                    if (pay.FEE_TYPES == 1)
                    {
                        // 为水的时候需要记录单价
                        pay.UNIT_PRICE = Convert.ToInt32(this._order.GetUnitPrice(CommonFiled.UnitPriceWaterKey) * 100);
                        pay.AMOUNT = WNum;
                        pay.TOTAL_FEE = Convert.ToInt32(pay.UNIT_PRICE * pay.AMOUNT);
                        pay.TOTAL_FEE_CH = CommonFiled.CmycurD(Convert.ToDecimal((pay.UNIT_PRICE * pay.AMOUNT) / 100.00));
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
        [AllowAnonymous]
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
                        if (valuePairs.ContainsKey("out_trade_no") || valuePairs.ContainsKey("total_fee"))
                        {
                            string OId = valuePairs["out_trade_no"].ToString();
                            int fee = Convert.ToInt32(valuePairs["total_fee"]);
                            if (Payed(OId, fee, content))
                            {
                                resultBool = true;
                                result = string.Format(result, CommonFiled.SUCCESS, CommonFiled.SUCCESS);
                            }
                            else
                            {
                                _log.LogInformation("微信支付异步回调请求，订单ID不存在，或已支付订单，或金额不同。");
                            }
                        }
                        else
                        {
                            _log.LogInformation("微信支付异步回调请求，不包含out_trade_no，total_fee");
                        }
                    }
                }
                else
                {
                    _log.LogInformation("微信支付异步回调请求状态码Fail");
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "微信回调异常。");
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

        private bool Payed(string OrderId, int TotelFee, string content)
        {
            //验证订单信息金额
            wy_wx_pay pay = this._order.GetWxPayById(OrderId);
            if (pay != null && pay.STATUS == 0 && pay.TOTAL_FEE == Convert.ToInt32(TotelFee))
            {
                pay.STATUS = 2;
                pay.PAY_TIME = DateTime.Now;
                pay.STATUS_REMARK = content;
                if (this._order.Update(pay) > 0)
                {
                    SendMsg(pay);
                    //返回信息
                    Task.Run(() => //异步操作，防止返回超时，被调用两次
                    {
                        WXPaidAfter(pay);
                    });
                    return true;
                }
                else
                {
                    _log.LogInformation("微信支付异步回调请求:订单更新失败或已支付！");
                }
            }
            return false;
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
            ViewBag.MoneyNum = Convert.ToDouble((Model.TOTAL_FEE / 100.00));
            return View(Model);
        }

        public JsonResult GetUnitPriceOfWater()
        {
            return OK(this._order.GetUnitPrice(CommonFiled.UnitPriceWaterKey));
        }

        /// <summary>
        /// 电标价
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUnitPriceOfElectricSET()
        {
            return OK(this._order.GetUnitPrice(CommonFiled.UnitPriceElectricSetKey));
        }
        /// <summary>
        /// 电价
        /// </summary>
        /// <returns></returns>
        public double GetUnitPriceOfElectric()
        {
            return this._order.GetUnitPrice(CommonFiled.UnitPriceElectricKey);
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
                _log.LogError(ex, "微信支付成功后逻辑处理错误：[OrderID]:" + OrderId);
                return false;
            }
        }

        private bool WXPaidAfter(wy_wx_pay Order)
        { //根据订单ID
            try
            {
                _log.LogInformation($"支付成功，更新类型：{CommonFiled.FeeTypeName(Order.FEE_TYPES)}");
                //更新缴费记录表，如果为自主缴费记录，支付成功后，重新生成新数据
                //如果类型为物业费 // recoreId 不为空，证明为提醒订单，需要修改record表中的数据
                if (Order != null && (!string.IsNullOrEmpty(Order.RECORD_ID)) && Order.RECORD_ID != SelfPayOrder)
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
                _log.LogError(ex, "微信支付成功后逻辑处理错误：[OrderID]:" + Order.ORDER_ID);
                return false;
            }
        }
        #region 支付成功后，水电物业费处理
        private bool PropertyCost(wy_wx_pay Order)
        {
            _log.LogInformation($"【wy_pay_record】准备更新:{Order.RECORD_ID}");
            FeeTypes type = CommonFiled.EnumFeeTypes(Order.FEE_TYPES);
            var Res = this._order.UpdateRecoredJFZT(new wy_pay_record()
            {
                RECORD_ID = Order.RECORD_ID,
                JFZT = 1,
                JFRQ = Order.PAY_TIME,
                PAY_WAY = 1,
                JFJE = (Order.TOTAL_FEE / 100.00)
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
                    AddAmount = Convert.ToDouble(Order.TOTAL_FEE / 100.00),
                    UnitPrice = Order.UNIT_PRICE,
                    CreateDate = DateTime.Now
                });
                if (Res > 0)
                {
                    Task.Run(()=> {
                        InsertPayRecord(Order);
                    });
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
                double unitPrice = GetUnitPriceOfElectric();
                var Res = this._order.InsertElectricity(new wy_ele_recharge()
                {
                    id = Order.ID,
                    CreateDate = DateTime.Now,
                    address = Order.TYPES_ID,
                    cid = Order.TYPES_ID_ELE_COLL,
                    EleAmount = Order.AMOUNT,
                    Cost = Order.AMOUNT * unitPrice,
                    WYCost = Convert.ToDecimal(Order.TOTAL_FEE / 100.00),
                });
                if (Res > 0)
                {
                    Task.Run(() => {
                        InsertPayRecord(Order);
                    });
                    _log.LogInformation($"【wy_ele_recharge】更新成功:{Order.ID}");
                }
                else
                {
                    _log.LogError($"【wy_ele_recharge】更新失败:{Order.ID}");
                    return false;
                }
            }
            else
            {
                _log.LogInformation($"【wy_ele_recharge】订单已存在，过滤重复插入");
                return false;
            }
            return true;
        }

        private bool InsertPayRecord(wy_wx_pay Order)
        {
            _log.LogInformation($"【InsertPayRecord】准备插入新增支付记录:{Order.ID}");
            if (Order.RECORD_ID.Trim() == "0") //自主缴费
            {
                var record = new wy_pay_record()
                {
                    RECORD_ID = Order.ID,
                    JFLX = Order.FEE_TYPES.ToString(),
                    FWID = Order.HOUSE_ID,
                    JFJE = (Order.TOTAL_FEE / 100.00),
                    JFZT = 1,
                    SFTZ = 1,
                    JFRQ = Order.PAY_TIME,
                    PAY_WAY = 1,
                    SURPLUSVALUE = GetSurplus(Order.FEE_TYPES, Order.TYPES_ID, Order.TYPES_ID_ELE_COLL),
                    CREATE_TIME = Order.CREATE_TIME,
                    CZ_SHID = Order.USER_ID,
                    OPEN_ID = Order.OPEN_ID,
                    CONFIRM_RECIVEMONEY = 0,
                    JFCS = 0
                };
                var Res = _order.InsertRecord(record) > 0;
                if (Res)
                {
                    _log.LogInformation($"【InsertPayRecord】插入新增支付成功:{Order.ID}");
                }
                else
                {
                    _log.LogError($"【InsertPayRecord】插入新增支付失败:{Order.ID}");
                }
            }
            return true;
        }

        /// <summary>
        /// 获取水电的余额
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeId"></param>
        /// <param name="CollId"></param>
        /// <returns></returns>
        private double GetSurplus(int type, string typeId, string CollId)
        {
            if (type == 1)
            {
                var waterSurplus = _shop.GetWater(typeId);
                if (waterSurplus == null)
                    return 0;
                return Convert.ToDouble(waterSurplus.MeterAccflow);
            }
            if (type == 2)
            {
                var Surplus = _shop.GetElectricity(CollId, typeId);
                if (Surplus == null)
                    return 0;
                return Convert.ToDouble(Surplus.EleBalance);
            }
            return 0;
        }
        #endregion

        #endregion

        public ActionResult SelfPay()
        {
            return View();
        }


        public JsonResult CheckOrderStatus(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return Data(ResultCode.PARAMS_IS_NULL);
            }
            wy_wx_pay payOrder = this._order.GetWxPayById(orderId);
            if (payOrder.STATUS == 2) //已经支付的订单
            {
                return Data(ResultCode.SCCUESS, true);
            }
            if (payOrder.STATUS == 0) //订单生成，且未支付,需要主动check订单状态
            {
                Dictionary<string, object> Orders = this._order.FindOrder(orderId);
                if (Orders != null && Orders["return_code"].ToString() == "SUCCESS"
                    && Orders["return_code"].ToString() == "SUCCESS")
                {
                    //检查支付的订单金额
                    var totel_fee = Convert.ToInt32(Orders["total_fee"]);
                    var checkStatus = this._order.CheckOrder(orderId, totel_fee);
                    if (checkStatus)
                    {
                        Payed(orderId, totel_fee, JsonConvert.SerializeObject(Orders));
                    }
                    return Data(ResultCode.SCCUESS, checkStatus);
                }
                return Data(ResultCode.SCCUESS, false);
            }
            else //已经失效的订单和支付失败的订单直接返回false；
            {
                return Data(ResultCode.SCCUESS, false);
            }
        }

        private async void SendMsg(wy_wx_pay order)
        {
            var data = new
            {
                openId = OpenID,
                data = new
                {
                    first = $"尊敬的业主({order.SHOP_NAME}--{order.HOUSE_ADDRESS})您好，您已缴费成功。信息如下：",
                    keyword1 = order.PAY_TIME.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    keyword2 = order.REMARK,
                    keyword3 = Convert.ToDouble(order.TOTAL_FEE / 100.00) + "元",
                    remark = "如有疑问，请咨询物业管理处。"
                }
            };
            await PayMsg.SendMsg(JsonConvert.SerializeObject(data));
        }


        public JsonResult CheckRechargeStatus(string orderGuid, int type) {
            if (type == 1) //水
            {
                var result = this._order.GetW_Pay(orderGuid);
                if (result != null &&　result.PStatus.HasValue && result.PStatus.Value == 1) {
                    return Data(ResultCode.SCCUESS, "success");
                }
            }else if (type == 2) {  //电
                var result = this._order.GetElectricity(orderGuid);
                if (result !=null && string.IsNullOrEmpty(result.Pstatus) && result.Pstatus == "SUCCESS")
                {
                    return Data(ResultCode.SCCUESS, "success");
                }
            }
            return Data(ResultCode.SCCUESS, "fail");
        }

        /// <summary>
        /// 新单据界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Receipt(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Exception("没有查询到单据信息");
            }
            wy_wx_pay Model = this._order.GetWxOrderDetail(id);
            ViewBag.Type = CommonFiled.FeeTypeName(Model.FEE_TYPES);
            ViewBag.MoneyNum = Convert.ToDouble((Model.TOTAL_FEE / 100.00)).ToString("0.00");
            ViewBag.PayTime = Model.PAY_TIME.HasValue ? Model.PAY_TIME.Value.ToString("yyyy/MM/dd") : "xxxx/xx/xx";
            //ViewBag.EffectiveTime = Model.
            ViewBag.Payee = CommonFiled.MchName(Model.FEE_TYPES);
            return View(Model);
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<string> Msg()
        {
            var order = this._order.GetWxPayById("O360202005130955208684");
            //string data = JsonConvert.SerializeObject(new
            //{
            //    openId = "oAY4Pv8qLZLMySKDoKrv-Zz1xBZ0",
            //    data = new
            //    {
            //        first = $"尊敬的业主({order.SHOP_NAME}--{order.HOUSE_ADDRESS})您好，您已缴费成功。信息如下：",
            //        keyword1 = order.PAY_TIME.HasValue ? order.PAY_TIME.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
            //        keyword2 = order.REMARK,
            //        keyword3 = Convert.ToDouble(order.TOTAL_FEE / 100.00) + "元",
            //        remark = "如有疑问，请咨询物业管理处。"
            //    }
            //});
            //data = "{\"openId\":\"oAY4Pv8qLZLMySKDoKrv-Zz1xBZ0\",\"data\":{\"first\":\"尊敬的用户：您收到一个用水余量不足通知：\",\"keyword1\":\"那奇男\",\"keyword2\":\"清真小厨\",\"keyword3\":\"0吨\",\"remark\":\"避免影响用水，请尽快缴费！\"},\"templateId\":\"P_QmAggBg0gDCt1VUrO9e_jXdfWhKFf98pSPnj5kMww\",\"color\":\"#173177\",\"appurl\":null,\"appId\":null,\"pagepath\":null}";
            SendMsg(order);
            return null;
        }
    }
}