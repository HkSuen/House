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
        Dictionary<string, object> GetShopInfoListByPage(string ShopName, int FWSX, string SSQY, string LSFGS, int page, int size);
        Dictionary<string, object> GetShopInfoList(string ShopName, int FWSX, string SSQY, string LSFGS);
        Dictionary<string, object> GetMerchantShopInfoListDetail(string FWID, string CZ_SHID); 
        Dictionary<string, object> GetMerchantShopJiaoFeiList(string FWID, string CZ_SHID); 
    }
}
