using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace House.IService.Model
{
    public class JsApiTicket
    {
        private static int code;
        public int errcode { get { return code; } set{ code = value; } }

        private static string msg;
        public string errmsg { get => msg; set { msg = value; } }

        private static int expiresIn;
        public int expires_in { get => expiresIn; set { expiresIn = value; } }

        private static DateTime? valid;

        private static string api_ticket;
        public string ticket
        {
            get
            {
                //减少200s的误差，以防请求刚好过期
                if (valid.HasValue && valid.Value.AddSeconds(expiresIn - 200) > DateTime.Now)
                {
                    return api_ticket;
                }
                return null;
            }
            set
            {
                valid = DateTime.Now;
                api_ticket = value;
            }
        }
    }
}
