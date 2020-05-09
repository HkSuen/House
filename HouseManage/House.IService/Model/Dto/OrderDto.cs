using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Model.Dto
{
    public class OrderDto
    {
        public v_pay_record Record { get; set; }
        public wy_houseinfo Houseinfo { get; set; }
        public wy_shopinfo Shopinfo { get; set; }
        public wy_ropertycosts Costs { get; set; }
    }
}
