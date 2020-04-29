using Common.JWTHelper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Common.AppSetting
{
    public class AppSetting
    {
        private static IConfiguration Config => new JWTHelper().Config;
        public static string GetSection(string key)
        {
            return Config[key].ToString();
        }
    }
}
