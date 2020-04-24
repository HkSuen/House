using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Common;
using House.IService.Model.Dto;
using House.IService.Model.Enum;
using House.IService.Order;
using House.Service.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace House.Service.Order
{
    public class OrderSvc : DataSvc, IOrderSvc
    {
        private IWeChatPaySingle _pay = null;
        public OrderSvc(IDataConfig config, IWeChatPaySingle paySingle)
            : base(config)
        {
            _pay = paySingle;
        }

        public Dictionary<string, object> GetPayParamsByWxModel(wy_wx_pay wxpay)
        {
            wxpay.ID = Guid.NewGuid().ToString("N");
            wxpay.ORDER_ID = $"{new Random().Next(100, 999)}{DateTime.Now.ToString("yyyyMMddHHmmssffff")}";
            wxpay.STATUS = 0;//订单生成
            wxpay.CREATE_TIME = DateTime.Now;
            return this._pay.GetPrepaySign(wxpay);
        }

        public Dictionary<string, object> GetPayParamsByWxModel(string appId, string prePayId, string PaySecret)
        {
            throw new NotImplementedException();
        }

        public wy_wx_pay GetWxPayById(string OrderId)
        {
            return this._db.CurrentDb<wy_wx_pay>().GetSingle(c => c.ORDER_ID == OrderId);
        }

        public wy_wx_pay FindSingle(string recoredId, string HouseId, string UserId, string OpenId)
        {
            return this._db.Db().Queryable<wy_wx_pay>().OrderBy(c=>c.CREATE_TIME,OrderByType.Desc)
                .First(c => c.RECORD_ID == recoredId
            && HouseId == c.HOUSE_ID && c.USER_ID == UserId && c.OPEN_ID == OpenId);
        }

        public v_pay_record GetRecord(string recordId, string HouseId)
        {
            var conditions = Expressionable.Create<v_pay_record>();
            if (!string.IsNullOrEmpty(recordId))
            {
                conditions = conditions.And(c => c.RECORD_ID == recordId);
            }
            if (string.IsNullOrEmpty(HouseId))
            {
                conditions = conditions.And(c => c.FWID == HouseId);
            }
            return this._db.CurrentDb<v_pay_record>().GetSingle(conditions.ToExpression());
        }

        public OrderDto GetWxPay(string recordId, string HouseId)
        {
            var conditions = Expressionable.Create<v_pay_record, wy_houseinfo, wy_shopinfo>();
            if (!string.IsNullOrEmpty(recordId))
            {
                conditions = conditions.And((re, ho, sh) => re.RECORD_ID == recordId);
            }
            if (string.IsNullOrEmpty(HouseId))
            {
                conditions = conditions.And((re, ho, sh) => re.FWID == HouseId);
            }
            OrderDto Data = this._db.Db().Queryable<v_pay_record, wy_houseinfo, wy_shopinfo>((re, ho, sh) => new object[] {
                JoinType.Left,re.FWID == ho.FWID,
                JoinType.Left,re.CZ_SHID == sh.CZ_SHID
            }).Where(conditions.ToExpression())
            .Select((re, ho, sh) => new OrderDto
            {
                Record = re,
                Houseinfo = ho,
                Shopinfo = sh
            }).First();
            return Data;
        }

        public int Inert(wy_wx_pay wxpay)
        {
            if (string.IsNullOrEmpty(wxpay.ID))
            {
                wxpay.ID = Guid.NewGuid().ToString("N");
            }
            return this._db.Db().Insertable(wxpay).ExecuteCommand();
        }

        public List<object> GetPayDetails(string UserId, string HouseId, string RecordId)
        {
            var conditions = Expressionable.Create<wy_pay_record, wy_houseinfo, wy_shopinfo, wy_wx_pay>();
            if (!string.IsNullOrEmpty(UserId))
            {
                conditions = conditions.And((record, house, shop, wxpay) => record.CZ_SHID == UserId);
            }
            if (!string.IsNullOrEmpty(RecordId))
            {
                conditions = conditions.And((record, house, shop, wxpay) => record.RECORD_ID == RecordId);
            }
            if (!string.IsNullOrEmpty(HouseId))
            {
                conditions = conditions.And((record, house, shop, wxpay) => record.FWID == HouseId);
            }
            //conditions = conditions.And((record, house, shop, wxpay) =>house.IS_DELETE == 0 && shop.IS_DELETE == 0);
            var List = this._db.Db().Queryable<wy_pay_record, wy_houseinfo, wy_shopinfo, wy_wx_pay>((record, house, shop, wxpay) => new object[] {
                JoinType.Left,record.FWID == house.FWID,
                JoinType.Left,record.CZ_SHID == shop.CZ_SHID,
                JoinType.Left,wxpay.RECORD_ID == record.RECORD_ID
            }).Where(conditions.ToExpression())
            .Select<object>((record, house, shop, wxpay) => new
            {
                RECORD_ID = record.RECORD_ID, //record id
                record.FWID,
                CZ_SHID = record.CZ_SHID,
                shop.SHOP_NAME,// shop name 
                JFLX = record.JFLX,
                JFLXName = string.Empty, //预留字段
                shop.MOBILE_PHONE, //用户手机号码
                house.FWBH,//房屋编号
                house.FWMC, //房屋名称
                house.WATER_NUMBER, //水表编号
                house.ELE_NUMBER, //电表编号
                JFJE = record.JFJE,
                record.YXQS,
                record.YXQZ,
                record.JFZT,
                wxpay.ID
            }).ToList();
            return List;
        }

        public wy_wx_pay GetWxPay(OrderDto oder)
        {
            v_pay_record record = oder.Record;
            var pay = new wy_wx_pay();
            pay.ID = CommonFiled.guid;
            pay.APP_ID = CommonFiled.appID;
            pay.ORDER_ID = CommonFiled.ABC + CommonFiled.orderId;
            pay.RECORD_ID = record.RECORD_ID;
            pay.HOUSE_ID = record.FWID;
            pay.USER_ID = record.CZ_SHID; //用户ID
            pay.OPEN_ID = record.OPEN_ID;
            pay.FEE_TYPES = Convert.ToInt32(record.JFLX);
            pay.TOTAL_FEE = record.JFJE.HasValue ? Convert.ToInt32(record.JFJE.Value * 100) : 0;
            pay.REMARK = $"自助缴费_{Fee.Types.SingleOrDefault(c => c.Value == Convert.ToInt32(record.JFLX)).Key}";
            pay.STATUS = 0;
            pay.CREATE_TIME = DateTime.Now;
            pay.MECH_ID = CommonFiled.MchId(record.JFLX);
            pay.NONCE_STR = CommonFiled.guid;
            pay.PREPAY_TIME = DateTime.Now;
            pay.PREPAY_ENDTIME = DateTime.Now.AddHours(2);
            pay.TRADE_TYPE = CommonFiled.JSAPI;
            pay.HOUSE_NAME = oder.Houseinfo.FWMC;
            pay.HOUSE_ADDRESS = oder.Houseinfo.ZLWZ;
            pay.HOUSE_AREA = oder.Houseinfo.JZMJ;
            pay.USER_NAME = oder.Shopinfo.ZHXM;
            pay.SHOP_NAME = oder.Shopinfo.SHOP_NAME;
            pay.TOTAL_FEE_CH = CommonFiled.CmycurD((pay.TOTAL_FEE / 100));
            pay.MECH_NAME = CommonFiled.MchName(pay.FEE_TYPES);
            return pay;
        }

        public wy_wx_pay GetWxOrderDetail(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                return this._db.CurrentDb<wy_wx_pay>().GetSingle(pay => pay.ID == Id);
            }
            return null;
        }

        public int Update(wy_wx_pay wxpay)
        {
            int Result = 0;
            using (var db = this._db.Db())
            { //sql锁，正确处理避免死锁
                Result = db.Updateable(wxpay).UpdateColumns(c => new { c.STATUS, c.PAY_TIME, c.STATUS_REMARK })
                     .With(SqlWith.UpdLock)
                    .ExecuteCommand();
            }
            return Result;
        }

        public int InsertWater(wy_w_amount Amount)
        {
            Amount.MeterID = CommonFiled.guid;
            return 0;
        }

        public int InsertElectricity(wy_ele_balance Balance)
        {
            throw new NotImplementedException();
        }

        public double GetUnitPrice(string WaterKey)
        {
            using (var db = this._db.Db()) {
                var price = db.Queryable<ts_uidp_config>().First(c => c.CONF_CODE == WaterKey);
                return Convert.ToDouble(price.CONF_VALUE);
            }
        }
    }
}
