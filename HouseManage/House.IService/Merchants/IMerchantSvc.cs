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
    }
}
