using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using House.IService.Common;

namespace House.IService.Model.Enum
{

    public enum URole
    {
        /// <summary>
        /// 商户
        /// </summary>
        [Description("Merchant")]
        Merchant,
        /// <summary>
        /// 检查员
        /// </summary>
        [Description("Inspector")]
        Inspector,
        /// <summary>
        /// 管理员
        /// </summary>
        [Description("Admin")]
        Admin
    }

    public class UserRole
    {
        //权限类型
        public static Dictionary<string, string> Role
        {
            get
            {
                var userRole = new Dictionary<string, string>();
                userRole.Add("Merchant", URole.Merchant.GetEnumDescription());
                userRole.Add("Inspector", URole.Inspector.GetEnumDescription());
                userRole.Add("Admin", URole.Admin.GetEnumDescription());
                return userRole;
            }
        }
    }
}
