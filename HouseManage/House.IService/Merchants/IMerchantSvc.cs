using Data.MSSQL.Model.Data;
using House.IService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Merchants
{
   public interface IMerchantSvc
    {
        Dictionary<string, object> GetBaseData(DictionaryModel model);

        Dictionary<string, object> GetMerchantList(wy_houseinfo model);
        Dictionary<string, object> GetMerchantListByPage(string FWBH, int FWSX, string SSQY, string LSFGS, int page, int sixe);
        Dictionary<string, object> GetMerchantListHouseDetail(string FWID);

        Dictionary<string, object> GetMerchantListShopDetail(string FWID);
    }
}
