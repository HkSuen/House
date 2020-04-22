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

namespace HouseManage.Controllers.Order
{
    public class OrdersController : ControllerBase
    {
        private IOrderSvc _order = null;
        private IXMLHelperSingle _xml = null;
        public OrdersController(IOrderSvc order, IXMLHelperSingle xml)
        {
            _order = order;
            _xml = xml;
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

        public JsonResult CreateOrder(string recordId, string houseId, string UId)
        {
            if (string.IsNullOrEmpty(recordId) || string.IsNullOrEmpty(houseId) || string.IsNullOrEmpty(UId))
            {
                return Data(ResultCode.PARAMS_IS_NULL, null, ResultCode.PARAMS_IS_NULL.GetEnumDescription());
            }
            //1.订单查询有无此数据,无数据默认创建新数据
            wy_wxpay pay = this._order.FindSingle(recordId, houseId, UId, OpenID);
            //原订单失效，异步更改状态
            if (pay == null || DateTime.Now > pay.PREPAY_ENDTIME)
            {
                //获取提醒订单信息
                var PayRecord = this._order.GetWxPay(recordId, houseId);
                pay = this._order.GetWxPay(PayRecord);
                pay.USER_IP = UserIP;
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
            string SUCCESS = "SUCCESS";
            string FAIL = "FAIL";
            string result = "<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>";
            try
            {
                Stream stream = Request.Body;
                byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string content = Encoding.UTF8.GetString(buffer);
                Dictionary<string, object> valuePairs = this._xml.XmlStrToDic(content);
            }
            catch (Exception ex)
            {
                return Content(string.Format(result, FAIL, FAIL), "text/xml");
            }
            return Content(result, "text/xml");
        }

        //[Authorize(Roles ="Admin")]
        public ActionResult OrderDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Error("没有查询到单据信息。");
            }
            wy_wxpay Model = this._order.GetWxOrderDetail(id);
            ViewBag.Type = CommonFiled.FeeTypeName(Model.FEE_TYPES);
            ViewBag.MoneyNum = CommonFiled.CmycurD(Model.TOTAL_FEE / 10);
            return View(Model);
        }

    }
}