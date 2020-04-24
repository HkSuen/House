using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Model.Dto
{
    public class UserDto
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PHONE { get; set; }
        /// <summary>
        /// OPENid
        /// </summary>
        public string OPENID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string AUTHORITY { get; set; }
    }
}
