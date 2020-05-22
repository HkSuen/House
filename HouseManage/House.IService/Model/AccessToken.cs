using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace House.IService.Model
{
    public class AccessToken
    {
        /// <summary>
        /// 有效期
        /// </summary>
        private static int expiresIn;
        public int expires_in { get { return expiresIn; } set { expiresIn = value; } }

        private static DateTime? valid;
        private static string token;
        public string access_token
        {
            get
            {
                //减少200s的误差，以防请求刚好过期
                if (valid.HasValue && valid.Value.AddSeconds(expiresIn - 200) > DateTime.Now)
                {
                    return token;
                }
                return null;
            }
            set
            {
                valid = DateTime.Now;
                token = value;
            }
        }
    }
}
