using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService.Common;
using House.IService.WyConfig;
using House.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.Service.WyConfig
{
    public class ConfigSingle : DataSvc, IConfigSingle
    {
        public ConfigSingle(IDataConfig config)
            : base(config)
        {
        }
        public string GetValue(string key)
        {
            using (var db = this._db.Db())
            {
                var configValue = db.Queryable<ts_uidp_config>().First(c => c.CONF_CODE == key);
                return configValue?.CONF_VALUE;
            }
        }


        #region 短信和微信消息推送的Check校验
        private bool CheckStatus(string state)
        {
            if(!string.IsNullOrEmpty(state) && state.ToLower() == "true")
            {
                return true;
            }
            return false;
        }
        public bool STATUS(string key) {
            return CheckStatus(GetValue(key));
        }
        #endregion


    }
}
