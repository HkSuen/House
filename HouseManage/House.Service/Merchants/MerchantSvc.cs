using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Merchants;
using House.IService.Model;
using House.IService.Model.Enum;
using House.Service.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace House.Service.Merchants
{
   public class MerchantSvc: IMerchantSvc
    {
        private IDataConfig _Db = null;
        public MerchantSvc(IDataConfig dataConfig)
        {
            this._Db = dataConfig;
        }
        /// <summary>
        /// 数据字典
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Dictionary<string,object> GetBaseData(DictionaryModel model) {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var areaList = _Db.Db().Queryable<tax_dictionary>().Where(n => n.ParentCode == model.areaCode).Select(m=>new { value=m.Code,text=m.Name}).ToList();
            var companyList = _Db.Db().Queryable<tax_dictionary>().Where(n => n.ParentCode == model.companyCode).Select(m => new { value = m.Code, text = m.Name }).ToList();
            dic.Add("areaList",areaList);
            dic.Add("companyList", companyList);
            return dic;
        }
        /// <summary>
        /// 商铺信息下拉刷新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetMerchantList(wy_houseinfo model, string userID, URole role)
        {
           
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var list=_Db.Db().Queryable<wy_houseinfo>().Where(a => a.IS_DELETE == 0);
                if (role == URole.Admin)
                {
                    list = _Db.Db().Queryable<wy_houseinfo, ts_uidp_org, ts_uidp_org_user>((a, b, c) => new object[] {
                           JoinType.Inner,a.ORG_CODE.StartsWith(b.ORG_CODE),
                           JoinType.Inner,b.ORG_ID==c.ORG_ID&&c.USER_ID==userID
                    }).Where((a, b, c) => a.IS_DELETE == 0);
                }
                else if (role == URole.Inspector)
                {
                    list = _Db.Db().Queryable<wy_houseinfo, wy_region_director>((a, b) => new object[] {
                           JoinType.Inner,a.SSQY==b.SSQY&&b.RD_ID==userID,
                    }).Where((a, b) => a.IS_DELETE == 0);
                }
                if (!string.IsNullOrWhiteSpace(model.FWBH))
                {
                    list = list.Where(a => a.FWBH.Contains(model.FWBH));
                }
                if (model.FWSX != null&& model.FWSX!=-1)
                {
                    list = list.Where(a => a.FWSX == model.FWSX);
                }
                if (!string.IsNullOrWhiteSpace(model.SSQY))
                {
                    list = list.Where(a => a.SSQY == model.SSQY);
                }
                if (!string.IsNullOrWhiteSpace(model.LSFGS))
                {
                    list = list.Where(a => a.LSFGS == model.LSFGS);
                }
                var _list = list.ToList();
                var count = from p in _list where p.FWSX == 2 select p;
                var greenCount = (from p in _list where p.FWSX == 2 select p).Count();
                var grayCount = (from p in _list where p.FWSX == 0 select p).Count();
                var yellowCount = (from p in _list where p.FWSX == 1 select p).Count();

                var listNnew = list.Select(a => new { a.FWBH, a.FWMC,a.FWID,a.FWSX }).OrderBy(a => a.FWBH).Take(20).ToList();

                dic.Add("list", listNnew);
                dic.Add("greenCount", greenCount);
                dic.Add("grayCount", grayCount);
                dic.Add("yellowCount", yellowCount);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                dic.Add("greenCount", 0);
                dic.Add("grayCount", 0);
                dic.Add("yellowCount", 0);
            }
            return dic;
        }
        /// <summary>
        /// 商铺信息上拉加载
        /// </summary>
        /// <param name="FWBH"></param>
        /// <param name="FWSX"></param>
        /// <param name="SSQY"></param>
        /// <param name="LSFGS"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetMerchantListByPage(string FWBH, int FWSX, string SSQY, string LSFGS, int page, int size, string userID, URole role) {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                //var list = _Db.Db().Queryable<wy_houseinfo>();
                // list = list.Where(e => e.IS_DELETE == 0);
              
                var list = _Db.Db().Queryable<wy_houseinfo>().Where(a => a.IS_DELETE == 0);
                if (role == URole.Admin)
                {
                    list = _Db.Db().Queryable<wy_houseinfo, ts_uidp_org, ts_uidp_org_user>((a, b, c) => new object[] {
                           JoinType.Inner,a.ORG_CODE.StartsWith(b.ORG_CODE),
                           JoinType.Inner,b.ORG_ID==c.ORG_ID&&c.USER_ID==userID
                    }).Where((a, b, c) => a.IS_DELETE == 0);
                }
                else if (role == URole.Inspector)
                {
                    list = _Db.Db().Queryable<wy_houseinfo, wy_region_director>((a, b) => new object[] {
                           JoinType.Inner,a.SSQY==b.SSQY&&b.RD_ID==userID,
                    }).Where((a, b) => a.IS_DELETE == 0);
                }
                if (!string.IsNullOrWhiteSpace(FWBH))
                {
                    list = list.Where(a => a.FWBH.Contains(FWBH));
                }
                if (FWSX!=-1)
                {
                    list = list.Where(a => a.FWSX == FWSX);
                }
                if (!string.IsNullOrWhiteSpace(SSQY))
                {
                    list = list.Where(a => a.SSQY == SSQY);
                }
                if (!string.IsNullOrWhiteSpace(LSFGS))
                {
                    list = list.Where(a => a.LSFGS == LSFGS);
                }
                var _list = list.ToList();
                var count = from p in _list where p.FWSX == 2 select p;
                var greenCount = (from p in _list where p.FWSX == 2 select p).Count();
                var grayCount = (from p in _list where p.FWSX == 0 select p).Count();
                var yellowCount = (from p in _list where p.FWSX == 1 select p).Count();
                var listNnew = list.Select(a => new { a.FWBH, a.FWMC, a.FWID,a.FWSX }).OrderBy(a => a.FWBH).Skip((page - 1) * size).Take(size).ToList();

                dic.Add("list", listNnew);
                dic.Add("greenCount", greenCount);
                dic.Add("grayCount", grayCount);
                dic.Add("yellowCount", yellowCount);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                dic.Add("greenCount", 0);
                dic.Add("grayCount", 0);
                dic.Add("yellowCount", 0);
            }
            return dic;
        }
        /// <summary>
        /// 查询单已房屋信息
        /// </summary>
        /// <param name="FWID"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetMerchantListHouseDetail(string FWID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var obj = _Db.Db().Queryable<wy_houseinfo,tax_dictionary,tax_dictionary,tax_dictionary>((a,b,c,d)=>new object []{
                     JoinType.Left,a.LSFGS==b.Code&&b.ParentCode=="LSFGS",
                     JoinType.Left,a.JGLX==c.Code&&c.ParentCode=="JGLX",
                       JoinType.Left,a.SSQY==d.Code&&d.ParentCode=="SSQY",
                }).Where(a => a.FWID == FWID).Select((a,b,c,d)=>new {
                    a.FWBH,a.FWSX,SSQY=d.Name,a.JZMJ,a.ZLWZ,LSFGS=b.Name,a.ZCYZ,JGLX=b.Name,a.WATER_NUMBER,a.ELE_NUMBER
                }).First();
                dic.Add("list",obj);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                return dic;
            }
            return dic;
        }
        /// <summary>
        /// 查询单已商户信息
        /// </summary>
        /// <param name="FWID"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetMerchantListShopDetail(string FWID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo, wy_shopinfo>((a, b, c) => new object[]{
                     JoinType.Left,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.SUBLET_ID==c.CZ_SHID

                }).Where(a => a.FWID == FWID).Select((a, b, c) => new {
                    YZ_ZHXM=b.ZHXM,
                    YZ_ZHXB = b.ZHXB,
                    YZ_TELL =b.MOBILE_PHONE,
                    YZ_ISSBLET =b.IS_SUBLET,
                    YZ_MOBILE =b.TELEPHONE,
                    YZ_EMAIL =b.E_MAIL,
                    YZ_JYNR=b.JYNR
                    ,
                    Z_ZHXM = b.IS_SUBLET == 1 ? c.ZHXM : c.ZHXM,
                    Z_ZHXB = b.IS_SUBLET == 1 ? c.ZHXB : c.ZHXB,
                    Z_YZ_TELL = b.IS_SUBLET == 1 ? c.MOBILE_PHONE : c.MOBILE_PHONE,
                    Z_ISSBLET = b.IS_SUBLET == 1 ? c.IS_SUBLET : c.IS_SUBLET,
                    Z_MOBILE = b.IS_SUBLET == 1 ? c.TELEPHONE : c.TELEPHONE,
                    Z_ZEMAIL = b.IS_SUBLET == 1 ? c.E_MAIL : c.E_MAIL,
                    Z_JYNR = b.IS_SUBLET == 1 ? c.JYNR : c.JYNR
                }).First();
                dic.Add("list", obj);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                return dic;
            }
            return dic;
        }
        #region 商户查询

       
        public Dictionary<string, object> GetShopInfoListByPage(string ShopName, int FWSX, string SSQY, string LSFGS, int page, int size, string userID, URole role)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                
                var obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo,wy_leasinginfo>((a, b,c) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.LEASE_ID==c.LEASE_ID
                }).Where((a,b,c)=>a.IS_DELETE==0&&b.IS_DELETE==0).WhereIF(!string.IsNullOrWhiteSpace(ShopName),(a,b)=>b.ZHXM.Contains(ShopName))
                .WhereIF(FWSX>0, (a, b) => a.FWSX==FWSX)
                .WhereIF(!string.IsNullOrWhiteSpace(SSQY), (a, b) => a.SSQY==SSQY)
                .WhereIF(!string.IsNullOrWhiteSpace(LSFGS), (a, b) => a.LSFGS==LSFGS)
                .Select((a, b,c) => new {
                   a.FWMC,a.ZLWZ,b.CZ_SHID,b.ZHXM,a.FWID,
                   FWSX=a.FWSX==1?"出租":"出售",
                    ZLKSSJ=c.ZLKSSJ,
                    ZLZZSJ= c.ZLZZSJ
                }).OrderBy(a=>a.FWMC).Skip((page - 1) * size).Take(size).ToList();
                
                if (role == URole.Admin)
                {
                    obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo, wy_leasinginfo, ts_uidp_org, ts_uidp_org_user>((a, b, c,d,e) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.LEASE_ID==c.LEASE_ID,
                     JoinType.Inner,a.ORG_CODE.StartsWith(d.ORG_CODE),
                     JoinType.Inner,d.ORG_ID==e.ORG_ID&&e.USER_ID==userID
                }).Where((a, b, c) => a.IS_DELETE == 0 && b.IS_DELETE == 0).WhereIF(!string.IsNullOrWhiteSpace(ShopName), (a, b) => b.ZHXM.Contains(ShopName))
                .WhereIF(FWSX > 0, (a, b) => a.FWSX == FWSX)
                .WhereIF(!string.IsNullOrWhiteSpace(SSQY), (a, b) => a.SSQY == SSQY)
                .WhereIF(!string.IsNullOrWhiteSpace(LSFGS), (a, b) => a.LSFGS == LSFGS)
                .Select((a, b, c) => new {
                    a.FWMC,
                    a.ZLWZ,
                    b.CZ_SHID,
                    b.ZHXM,
                    a.FWID,
                    FWSX = a.FWSX == 1 ? "出租" : "出售",
                    ZLKSSJ = c.ZLKSSJ,
                    ZLZZSJ = c.ZLZZSJ
                }).OrderBy(a => a.FWMC).Skip((page - 1) * size).Take(size).ToList();
                }
                else if (role == URole.Inspector)
                {
                    obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo, wy_leasinginfo, wy_region_director>((a, b, c,d) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.LEASE_ID==c.LEASE_ID,
                     JoinType.Inner,a.SSQY==d.SSQY&&d.RD_ID==userID,
                }).Where((a, b, c) => a.IS_DELETE == 0 && b.IS_DELETE == 0).WhereIF(!string.IsNullOrWhiteSpace(ShopName), (a, b) => b.ZHXM.Contains(ShopName))
                .WhereIF(FWSX > 0, (a, b) => a.FWSX == FWSX)
                .WhereIF(!string.IsNullOrWhiteSpace(SSQY), (a, b) => a.SSQY == SSQY)
                .WhereIF(!string.IsNullOrWhiteSpace(LSFGS), (a, b) => a.LSFGS == LSFGS)
                .Select((a, b, c) => new {
                    a.FWMC,
                    a.ZLWZ,
                    b.CZ_SHID,
                    b.ZHXM,
                    a.FWID,
                    FWSX = a.FWSX == 1 ? "出租" : "出售",
                    ZLKSSJ = c.ZLKSSJ,
                    ZLZZSJ = c.ZLZZSJ
                }).OrderBy(a => a.FWMC).Skip((page - 1) * size).Take(size).ToList();
                }
                dic.Add("list", obj);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
            }
            return dic;
        }
        public Dictionary<string, object> GetShopInfoList(string ShopName, int FWSX, string SSQY, string LSFGS, string userID, URole role)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo,wy_leasinginfo>((a, b,c) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                      JoinType.Left,b.LEASE_ID==c.LEASE_ID
                }).Where((a, b, c) => a.IS_DELETE == 0 && b.IS_DELETE == 0).WhereIF(!string.IsNullOrWhiteSpace(ShopName), (a, b) => b.ZHXM.Contains(ShopName))
                .WhereIF(FWSX > 0, (a, b) => a.FWSX == FWSX)
                .WhereIF(!string.IsNullOrWhiteSpace(SSQY), (a, b) => a.SSQY == SSQY)
                .WhereIF(!string.IsNullOrWhiteSpace(LSFGS), (a, b) => a.LSFGS == LSFGS)
                .Select((a, b,c) => new {
                    a.FWMC,
                    a.ZLWZ,
                    b.CZ_SHID,
                    b.ZHXM,
                    a.FWID,
                    FWSX = a.FWSX == 1 ? "出租" : "出售",
                    c.ZLKSSJ,
                    c.ZLZZSJ
                }).OrderBy(a => a.FWMC).ToList();

                if (role == URole.Admin)
                {
                    obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo, wy_leasinginfo, ts_uidp_org, ts_uidp_org_user>((a, b, c, d, e) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.LEASE_ID==c.LEASE_ID,
                     JoinType.Inner,a.ORG_CODE.StartsWith(d.ORG_CODE),
                     JoinType.Inner,d.ORG_ID==e.ORG_ID&&e.USER_ID==userID
                }).Where((a, b, c) => a.IS_DELETE == 0 && b.IS_DELETE == 0).WhereIF(!string.IsNullOrWhiteSpace(ShopName), (a, b) => b.ZHXM.Contains(ShopName))
                .WhereIF(FWSX > 0, (a, b) => a.FWSX == FWSX)
                .WhereIF(!string.IsNullOrWhiteSpace(SSQY), (a, b) => a.SSQY == SSQY)
                .WhereIF(!string.IsNullOrWhiteSpace(LSFGS), (a, b) => a.LSFGS == LSFGS)
                .Select((a, b, c) => new {
                    a.FWMC,
                    a.ZLWZ,
                    b.CZ_SHID,
                    b.ZHXM,
                    a.FWID,
                    FWSX = a.FWSX == 1 ? "出租" : "出售",
                    ZLKSSJ = c.ZLKSSJ,
                    ZLZZSJ = c.ZLZZSJ
                }).OrderBy(a => a.FWMC).Take(20).ToList();
                }
                else if (role == URole.Inspector)
                {
                    obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo, wy_leasinginfo, wy_region_director>((a, b, c, d) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.LEASE_ID==c.LEASE_ID,
                     JoinType.Inner,a.SSQY==d.SSQY&&d.RD_ID==userID,
                }).Where((a, b, c) => a.IS_DELETE == 0 && b.IS_DELETE == 0).WhereIF(!string.IsNullOrWhiteSpace(ShopName), (a, b) => b.ZHXM.Contains(ShopName))
                .WhereIF(FWSX > 0, (a, b) => a.FWSX == FWSX)
                .WhereIF(!string.IsNullOrWhiteSpace(SSQY), (a, b) => a.SSQY == SSQY)
                .WhereIF(!string.IsNullOrWhiteSpace(LSFGS), (a, b) => a.LSFGS == LSFGS)
                .Select((a, b, c) => new {
                    a.FWMC,
                    a.ZLWZ,
                    b.CZ_SHID,
                    b.ZHXM,
                    a.FWID,
                    FWSX = a.FWSX == 1 ? "出租" : "出售",
                    ZLKSSJ = c.ZLKSSJ,
                    ZLZZSJ = c.ZLZZSJ
                }).OrderBy(a => a.FWMC).Take(20).ToList();
                }
                dic.Add("list", obj);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
            }
            return dic;
        }
        /// <summary>
        /// 商户详情页
        /// </summary>
        /// <param name="FWID"></param>
        /// <param name="CZ_SHID"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetMerchantShopInfoListDetail(string FWID,string CZ_SHID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo, wy_shopinfo>((a, b, c) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.SUBLET_ID==c.CZ_SHID

                }).Where(a => a.FWID == FWID).Select((a, b, c) => new {
                    a.FWMC,
                    a.ZLWZ,
                    FWSX = a.FWSX == 1 ? "出租" : "出售",
                    b.JYNR,
                    YZ_ZHXM = b.ZHXM,
                    YZ_ZHXB = b.ZHXB,
                    YZ_TELL = b.MOBILE_PHONE,
                    YZ_ISSBLET = b.IS_SUBLET,
                    YZ_MOBILE = b.TELEPHONE,
                    YZ_EMAIL = b.E_MAIL,
                    YZ_JYNR = b.JYNR
                    ,
                    Z_ZHXM = b.IS_SUBLET == 1 ? c.ZHXM : c.ZHXM,
                    Z_ZHXB = b.IS_SUBLET == 1 ? c.ZHXB : c.ZHXB,
                    Z_YZ_TELL = b.IS_SUBLET == 1 ? c.MOBILE_PHONE : c.MOBILE_PHONE,
                    Z_ISSBLET = b.IS_SUBLET == 1 ? c.IS_SUBLET : c.IS_SUBLET,
                    Z_MOBILE = b.IS_SUBLET == 1 ? c.TELEPHONE : c.TELEPHONE,
                    Z_ZEMAIL = b.IS_SUBLET == 1 ? c.E_MAIL : c.E_MAIL,
                    Z_JYNR = b.IS_SUBLET == 1 ? c.JYNR : c.JYNR
                }).First();
                dic.Add("list", obj);
                var list = _Db.Db().Queryable<wy_pay_record>().Where(a => a.CZ_SHID == CZ_SHID&&a.JFZT==1).Select(a=>new { a.JFRQ, a.JFJE, a.JFLX }).ToList();
                dic.Add("listOrder", list);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                return dic;
            }
            return dic;
        }
        
        public Dictionary<string, object> GetMerchantShopJiaoFeiList(string FWID, string CZ_SHID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var obj = _Db.Db().Queryable<wy_houseinfo, wy_shopinfo, wy_shopinfo>((a, b, c) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID,
                     JoinType.Left,b.SUBLET_ID==c.CZ_SHID

                }).Where(a => a.FWID == FWID).Select((a, b, c) => new {
                    a.FWMC,
                    a.ZLWZ,
                    FWSX = a.FWSX == 1 ? "出租" : "出售",
                    b.JYNR,
                }).First();
                dic.Add("list", obj);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                return dic;
            }
            return dic;
        }
        #endregion
    }
}
