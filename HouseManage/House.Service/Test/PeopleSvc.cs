using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Test;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.Service.Test
{
    /// <summary>
    /// Template 
    /// </summary>
    public class PeopleSvc : IPeopleSvc
    {
        private IDataConfig _Db = null;
        public PeopleSvc(IDataConfig dataConfig)
        {
            this._Db = dataConfig;
        }

        public List<people> GetAll()
        {
            var peo = _Db.CurrentDb<people>();
            return peo.GetList();
        }
    }
}
