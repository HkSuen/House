using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Merchants;
using House.IService.Model;
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
            Expression<Func<wy_houseinfo, bool>> expression = p => true;
            if (!string.IsNullOrWhiteSpace(model.FWBH))
            {
                expression = p=> expression.Compile()(p)&&p.FWBH.Contains(model.FWBH);
            }
            if (model.FWSX!=null) {
                expression = p => expression.Compile()(p) && p.FWSX==model.FWSX; 
            }
            if (!string.IsNullOrWhiteSpace(model.SSQY))
            {
                expression = p => expression.Compile()(p) && p.SSQY==model.SSQY;
            }
            if (!string.IsNullOrWhiteSpace(model.LSFGS))
            {
                expression = p => expression.Compile()(p) && p.LSFGS==model.LSFGS;
            }
            var list = _Db.Db().Queryable<wy_houseinfo>().Where(expression).Select(m => new {  m.FWBH,m.FWMC, m.FWID }).OrderBy(m=>m.FWBH).Take(1).ToList();
           
            dic.Add("list", list);
            dic.Add("count", list.Count);
            return dic;
        }
    }
}
