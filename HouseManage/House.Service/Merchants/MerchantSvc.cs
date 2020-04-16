using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Merchants;
using House.IService.Model;
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
        public Dictionary<string, object> GetMerchantList(wy_houseinfo model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var list = _Db.Db().Queryable<wy_houseinfo>();
                if (!string.IsNullOrWhiteSpace(model.FWBH))
                {
                    list = list.Where(e => e.FWBH.Contains(model.FWBH));
                }
                if (model.FWSX != null&& model.FWSX!=-1)
                {
                    list = list.Where(e => e.FWSX == model.FWSX);
                }
                if (!string.IsNullOrWhiteSpace(model.SSQY))
                {
                    list = list.Where(e => e.SSQY == model.SSQY);
                }
                if (!string.IsNullOrWhiteSpace(model.LSFGS))
                {
                    list = list.Where(e => e.LSFGS == model.LSFGS);
                }
                var _list = list.ToList();
                var count = from p in _list where p.FWSX == 2 select p;
                var greenCount = (from p in _list where p.FWSX == 2 select p).Count();
                var grayCount = (from p in _list where p.FWSX == 0 select p).Count();
                var yellowCount = (from p in _list where p.FWSX == 1 select p).Count();

                var listNnew = list.Select(m => new { m.FWBH, m.FWMC, m.FWID,m.FWSX }).OrderBy(m => m.FWBH).Take(20).ToList();

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
        public Dictionary<string, object> GetMerchantListByPage(string FWBH, int FWSX, string SSQY, string LSFGS, int page, int size) {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var list = _Db.Db().Queryable<wy_houseinfo>();
                if (!string.IsNullOrWhiteSpace(FWBH))
                {
                    list = list.Where(e => e.FWBH.Contains(FWBH));
                }
                if (FWSX!=-1)
                {
                    list = list.Where(e => e.FWSX == FWSX);
                }
                if (!string.IsNullOrWhiteSpace(SSQY))
                {
                    list = list.Where(e => e.SSQY == SSQY);
                }
                if (!string.IsNullOrWhiteSpace(LSFGS))
                {
                    list = list.Where(e => e.LSFGS == LSFGS);
                }
                var _list = list.ToList();
                var count = from p in _list where p.FWSX == 2 select p;
                var greenCount = (from p in _list where p.FWSX == 2 select p).Count();
                var grayCount = (from p in _list where p.FWSX == 0 select p).Count();
                var yellowCount = (from p in _list where p.FWSX == 1 select p).Count();
                var listNnew = list.Select(m => new { m.FWBH, m.FWMC, m.FWID,m.FWSX }).OrderBy(m => m.FWBH).Skip((page - 1) * size).Take(size).ToList();

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
    }
}
