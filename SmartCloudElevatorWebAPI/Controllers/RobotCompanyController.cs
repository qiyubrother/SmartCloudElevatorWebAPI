using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartCloudElevatorDataModel;
using SmartCloudElevatorWebAPI;
using SmartCloudElevatorWebAPI.Code;

namespace SmartCloudElevatorWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RobotCompanyController : ControllerBase
    {
        #region POST: api/RobotCompany/Add
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Add")]
        public JsonResult Post([FromBody] InRobotCompany v)
        {
            LogHelper.TraceTx($"api/RobotCompany/Add.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.RobotCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的机器人公司ID。" });
                    LogHelper.Error($"api/RobotCompany/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RobotCompany))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的机器人公司名称。" });
                    LogHelper.Error($"api/RobotCompany/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RobotCompanyAbbreviation))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的机器人公司缩写。" });
                    LogHelper.Error($"api/RobotCompany/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RobotCompanyTag))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的机器人公司标识。" });
                    LogHelper.Error($"api/RobotCompany/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion

                dc.RobotCompanys.Add(new RobotCompany
                {
                    RobotCompanyID = v.RobotCompanyId,
                    Company = v.RobotCompany,
                    CompanyAbbreviation = v.RobotCompanyAbbreviation,
                    CompanyTag = v.RobotCompanyTag
                });
                try
                {
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/RobotCompany/Add.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/RobotCompany/Add.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/RobotCompany/Delete
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Delete")]
        public JsonResult Post([FromBody] DeleteRobotCompany v)
        {
            LogHelper.TraceTx($"api/RobotCompany/Delete.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.RobotCompanyId))
                {
                    var rst = new JsonResult(new { ErrorCode = "-1", ErrorMessage = "无效的机器人公司ID。" });
                    LogHelper.Error($"api/RobotCompany/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.RobotCompanys.FirstOrDefault(x => x.RobotCompanyID == v.RobotCompanyId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的机器人公司ID。" });
                        LogHelper.Error($"api/RobotCompany/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    dc.RobotCompanys.Remove(h);

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/RobotCompany/Delete.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/RobotCompany/Delete.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/RobotCompany/Edit
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Edit")]
        public JsonResult Post([FromBody] InRobotCompanyEdit v)
        {
            LogHelper.TraceTx($"api/RobotCompany/Edit.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.RobotCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的机器人公司ID。" });
                    LogHelper.Error($"api/RobotCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewRobotCompany))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的机器人公司名称。" });
                    LogHelper.Error($"api/RobotCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewRobotCompanyAbbreviation))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的机器人公司名称缩写。" });
                    LogHelper.Error($"api/RobotCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewRobotCompanyTag))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的机器人公司标识。" });
                    LogHelper.Error($"api/RobotCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.RobotCompanys.FirstOrDefault(x => x.RobotCompanyID == v.RobotCompanyId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的机器人公司ID。" });
                        LogHelper.Error($"api/RobotCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    h.Company = v.NewRobotCompany;
                    h.CompanyAbbreviation = v.NewRobotCompanyAbbreviation;
                    h.CompanyTag = v.NewRobotCompanyTag;

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/RobotCompany/Edit.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/RobotCompany/Edit.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/RobotCompany/Search
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("Search")]
        public JsonResult Post([FromBody] InSearchRobotCompany v)
        {
            LogHelper.TraceTx($"api/RobotCompany/Search.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                try
                {
                    #region 用户状态检查
                    var _sts = SysHelper.GetUserStatus(this);
                    if (_sts == -97)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                        LogHelper.Error($"api/RobotCompany/Search.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    else if (_sts == -98)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/RobotCompany/Search.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    #endregion
                    var rst = dc.RobotCompanys.AsEnumerable();
                    if (!string.IsNullOrEmpty(v.RobotCompanyId))
                    {
                        rst = rst.Where(x => x.RobotCompanyID == v.RobotCompanyId);
                    }
                    if (!string.IsNullOrEmpty(v.RobotCompany))
                    {
                        rst = rst.Where(x => x.Company.Contains(v.RobotCompany, StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (!string.IsNullOrEmpty(v.RobotCompanyAbbreviation))
                    {
                        rst = rst.Where(x => x.CompanyAbbreviation.ToLower() == v.RobotCompanyAbbreviation.ToLower());
                    }
                    if (!string.IsNullOrEmpty(v.RobotCompanyTag))
                    {
                        rst = rst.Where(x => x.CompanyTag.ToLower() == v.RobotCompanyTag.ToLower());
                    }
                    var lst = new List<OutSearchRobotCompanyItem>();
                    foreach (var r in rst)
                    {
                        lst.Add(new OutSearchRobotCompanyItem { RobotCompanyId = r.RobotCompanyID, RobotCompany = r.Company, RobotCompanyAbbreviation = r.CompanyAbbreviation, RobotCompanyTag = r.CompanyTag });
                    }

                    var rx = new JsonResult(new OutSearchRobotCompany { ErrorCode = "0", ErrorMessage = "执行成功。", RobotCompanyList = lst });
                    LogHelper.TraceRx($"api/RobotCompany/Search.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/RobotCompany/Search.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
    }
}

#region RobotCompany/Add parameter class definition
public class InRobotCompany
{
    [JsonProperty("RobotCompanyId")]
    public string RobotCompanyId { get; set; }
    [JsonProperty("RobotCompany")]
    public string RobotCompany { get; set; }
    [JsonProperty("RobotCompanyAbbreviation")]
    public string RobotCompanyAbbreviation { get; set; }
    [JsonProperty("RobotCompanyTag")]
    public string RobotCompanyTag { get; set; }
}
#endregion

#region RobotCompany/Delete parameter class definition
public class DeleteRobotCompany
{
    [JsonProperty("RobotCompanyId")]
    public string RobotCompanyId { get; set; }
}
#endregion

#region RobotCompany/Edit parameter class definition
public class InRobotCompanyEdit
{
    public string RobotCompanyId { get; set; }
    public string NewRobotCompany { get; set; }
    public string NewRobotCompanyAbbreviation { get; set; }
    public string NewRobotCompanyTag { get; set; }
}
#endregion

#region RobotCompany/Search parameter class definition
/// <summary>
/// 输入条件
/// </summary>
public class InSearchRobotCompany
{
    [JsonProperty("RobotCompanyId")]
    public string RobotCompanyId { get; set; }
    [JsonProperty("RobotCompany")]
    public string RobotCompany { get; set; }
    [JsonProperty("RobotCompanyAbbreviation")]
    public string RobotCompanyAbbreviation { get; set; }
    [JsonProperty("RobotCompanyTag")]
    public string RobotCompanyTag { get; set; }
}

public class OutSearchRobotCompanyItem
{
    [JsonProperty("RobotCompanyId")]
    public string RobotCompanyId { get; set; }
    [JsonProperty("RobotCompany")]
    public string RobotCompany { get; set; }
    [JsonProperty("RobotCompanyAbbreviation")]
    public string RobotCompanyAbbreviation { get; set; }
    [JsonProperty("RobotCompanyTag")]
    public string RobotCompanyTag { get; set; }
}
/// <summary>
/// 返回查询结果
/// </summary>
class OutSearchRobotCompany : Out
{
    [JsonProperty("RobotCompanyList")]
    public List<OutSearchRobotCompanyItem> RobotCompanyList { get; set; }
}
#endregion
