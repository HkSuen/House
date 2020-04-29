using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService
{
    public interface IReCheckReviewSvc
    {

        Dictionary<string, object> GetRecheckReviewData(string openId);
        Dictionary<string, object> GetRecheckReviewDataDetail(string resultid);
        string ReviewCheckConfirm(string resultId, string rwbh, string fwbh, string fwmc, string jcr_openid);

    }
}
