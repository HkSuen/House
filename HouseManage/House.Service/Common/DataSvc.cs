using Data.MSSQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.Service.Common
{
    public class DataSvc
    {
        protected IDataConfig _db = null;
        public DataSvc(IDataConfig config) {
            _db = config;
        }
    }
}
