using Data.MSSQL;
using SqlSugar;
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

        /// <summary>
        /// 默认Using方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public T DBAble<T>(Func<SqlSugarClient, T> func) {
            using (var db = _db.Db()) {
                return func(db);
            }
        }

    }
}
