using House.IService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace House.IService.Model.Enum
{
    public enum FeeTypes
    {
        Property = 0,
        Water = 1,
        Electricity = 2,
    }

    public class Fee
    {
        public static Dictionary<string,int> Types
        {
            get
            {
                var userRole = new Dictionary<string, int>();
                userRole.Add("物业费", Convert.ToInt32(FeeTypes.Property));
                userRole.Add("水费", Convert.ToInt32(FeeTypes.Water));
                userRole.Add("电费", Convert.ToInt32(FeeTypes.Electricity));
                return userRole;
            }
        } 

        public static string GetKey(int Value)
        {
            return Types.Where(c=>c.Value == Value).Select(c => c.Key).FirstOrDefault();
        }
    }

}
