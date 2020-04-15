using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Merchants;
using House.IService.Model;
using House.Service.Common;
using System;
using System.Collections.Generic;
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
        public Dictionary<string,object> GetBaseData(DictionaryModel model) {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var areaList = _Db.Db().Queryable<tax_dictionary>().Where(n => n.ParentCode == model.areaCode).Select(m=>new { value=m.Code,text=m.Name}).ToList();
            var companyList = _Db.Db().Queryable<tax_dictionary>().Where(n => n.ParentCode == model.companyCode).Select(m => new { value = m.Code, text = m.Name }).ToList();
            dic.Add("areaList",areaList);
            dic.Add("companyList", companyList);
            return dic;
        }
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
                var listNnew = list.Select(m => new { m.FWBH, m.FWMC, m.FWID }).OrderBy(m => m.FWBH).Take(5).ToList();

                dic.Add("list", listNnew);
                dic.Add("count", listNnew.Count);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                dic.Add("count", 0);
            }
            return dic;
        }
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
                var listNnew = list.Select(m => new { m.FWBH, m.FWMC, m.FWID }).OrderBy(m => m.FWBH).Skip((page - 1) * size).Take(size).ToList();

                dic.Add("list", listNnew);
                dic.Add("count", listNnew.Count);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                dic.Add("count", 0);
            }
            return dic;
        }
    }
}
