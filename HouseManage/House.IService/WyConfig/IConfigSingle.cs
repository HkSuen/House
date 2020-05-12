using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.WyConfig
{
    public interface IConfigSingle 
    {
        string GetValue(string key);
    }
}
