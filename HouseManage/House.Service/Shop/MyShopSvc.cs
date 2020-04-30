using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Shop;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace House.Service.Shop
{
    public class MyShopSvc : IMyShopSvc
    {
        public IDataConfig _db;
        public MyShopSvc(IDataConfig data)
        {
            this._db = data;
        }
        public List<object> GetShopDetails(string OpenId)
        {
            var List = _db.Db().Queryable<wy_shopinfo, wy_houseinfo, tax_dictionary, tax_dictionary, tax_dictionary>((sh, ho, d1,d2,d3) => new object[] {
                JoinType.Left,sh.FWID == ho.FWID,
                JoinType.Left,ho.LSFGS == d1.Code && d1.ParentCode == "LSFGS",
                JoinType.Left,ho.JGLX == d2.Code && d2.ParentCode == "JGLX",
                JoinType.Left,ho.SSQY == d3.Code && d3.ParentCode == "SSQY",
            }).Where((sh, ho, d1,d2,d3) => sh.OPEN_ID == OpenId && sh.IS_DELETE == 0 && ho.IS_DELETE == 0)
            .Select<object>((sh, ho, d1,d2,d3) => new
            {
                ho.FWID, ho.FWSX,ho.FWMC, ho.FWBH, SSQY = d3.Name, ho.JZMJ, LSFGS = d1.Name,
                ho.ZLWZ, JGLX = d2.Name, ho.ZCYZ, sh.CZ_SHID, sh.SHOP_NAME, sh.SHOPBH, sh.ZHXM,
                sh.JYNR,sh.SHOP_STATUS,ho.WATER_NUMBER,ho.ELE_NUMBER,ho.CID
            }).ToList();
            return List;
        }

        public v_pay_record GetPayReminder(string openId)
        {
            var conditions = Expressionable.Create<v_pay_record>();
            conditions = conditions.And(c => c.OPEN_ID == openId);
            return _db.Db().Queryable<v_pay_record>().Where(conditions.ToExpression())
                .OrderBy(c => c.CREATE_TIME).First();
        }

        public List<v_pay_record> GetPayReminder(string OpenId, string Type, PageModel page)
        {
            var conditions = Expressionable.Create<v_pay_record>();
            if (!string.IsNullOrEmpty(OpenId))
            {
                conditions = conditions.And(c => c.OPEN_ID == OpenId);
            }
            if (!string.IsNullOrEmpty(Type) && Type != "-1")
            {
                conditions = conditions.And(c => c.JFLX == Type.Trim());
            }
            conditions = conditions.And(c => c.JFZT == 0);
            var list = _db.CurrentDb<v_pay_record>().GetPageList(conditions.ToExpression(),page,c=>c.CREATE_TIME,OrderByType.Desc);
            return list;
        }

        public List<wy_pay_record> GetPayRecord(string OpenId, string Type,int? payState, PageModel page)
        {
            var conditions = Expressionable.Create<wy_pay_record>();
            if (!string.IsNullOrEmpty(OpenId))
            {
                conditions = conditions.And(c => c.OPEN_ID == OpenId);
            }
            if (!string.IsNullOrEmpty(Type) && Type != "-1")
            {
                conditions = conditions.And(c => c.JFLX == Type.Trim());
            }
            if (payState.HasValue)
            {
                conditions = conditions.And(c => c.JFZT == payState.Value);
            }
            var list = _db.CurrentDb<wy_pay_record>().GetPageList(conditions.ToExpression(), page, c => c.CREATE_TIME, OrderByType.Desc);
            return list;
        }

        public wy_w_amount GetWater(string WId)
        {
            if (string.IsNullOrEmpty(WId))
            {
                return null;
            }
            var NewWaterInfo = _db.Db().Queryable<wy_w_amount>().Where(c => c.MeterID == WId)
                .OrderBy(c => c.CreateDate).First();
            return NewWaterInfo;
        }

        public wy_ele_balance GetElectricity(string CollectId, string ElectricityId)
        {
            if (string.IsNullOrEmpty(CollectId) || string.IsNullOrEmpty(ElectricityId))
            {
                return null;
            }
            var NewElectricityInfo = _db.Db().Queryable<wy_ele_balance>()
                .Where(c => c.address == ElectricityId && c.cid == CollectId && c.Ustatus == "1")
                .OrderBy(c => c.UpdateDate).First();
            return NewElectricityInfo;
        }
    }
}
