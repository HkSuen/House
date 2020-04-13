using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Test
{
    public interface IPeopleSvc
    {
        List<people> GetAll();
    }
}
