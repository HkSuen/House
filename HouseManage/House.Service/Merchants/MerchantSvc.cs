using Data.MSSQL;
using House.IService.Merchants;
using System;
using System.Collections.Generic;
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

    }
}
