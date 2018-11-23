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
    public class ElevatorCompanyController : ControllerBase
    {
        #region POST: api/ElevatorCompany/Add
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Add")]
        public JsonResult Post([FromBody] InElevatorCompany v)
        {
            LogHelper.TraceTx($"api/ElevatorCompany/Add.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/ElevatorCompany/Add.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/ElevatorCompany/Add.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.ElevatorCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "错误的电梯公司ID。" });
                    LogHelper.Error($"api/ElevatorCompany/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.ElevatorCompany))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "错误的电梯公司名称。" });
                    LogHelper.Error($"api/ElevatorCompany/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion

                dc.ElevatorCompanys.Add(new ElevatorCompany
                {
                    ElevatorCompanyId = v.ElevatorCompanyId,
                    Company = v.ElevatorCompany
                });
                try
                {
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/ElevatorCompany/Add.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;

                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/ElevatorCompany/Add.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/ElevatorCompany/Edit
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Edit")]
        public JsonResult Post([FromBody] InElevatorCompanyEdit v)
        {
            LogHelper.TraceTx($"api/ElevatorCompany/Edit.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.ElevatorCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的电梯公司ID。" });
                    LogHelper.Error($"api/ElevatorCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewElevatorCompany))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的电梯公司名称。" });
                    LogHelper.Error($"api/ElevatorCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.ElevatorCompanys.FirstOrDefault(x => x.ElevatorCompanyId == v.ElevatorCompanyId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的电梯公司ID。" });
                        LogHelper.Error($"api/ElevatorCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    h.Company = v.NewElevatorCompany;

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/ElevatorCompany/Edit.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/ElevatorCompany/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/ElevatorCompany/Delete
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Delete")]
        public JsonResult Post([FromBody] DeleteElevatorCompany v)
        {
            LogHelper.TraceTx($"api/ElevatorCompany/Delete.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.ElevatorCompanyId))
                {
                    var rst = new JsonResult(new { ErrorCode = "-1", ErrorMessage = "错误的电梯公司ID。" });
                    LogHelper.Error($"api/ElevatorCompany/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.ElevatorCompanys.FirstOrDefault(x => x.ElevatorCompanyId == v.ElevatorCompanyId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "错误的电梯公司ID。" });
                        LogHelper.Error($"api/ElevatorCompany/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    dc.ElevatorCompanys.Remove(h);

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/ElevatorCompany/Delete.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/ElevatorCompany/Delete.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/ElevatorCompany/Search
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("Search")]
        public JsonResult Post([FromBody] InSearchElevatorCompany v)
        {
            LogHelper.TraceTx($"api/ElevatorCompany/Search.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                try
                {
                    var rst = dc.ElevatorCompanys.AsEnumerable();
                    if (!string.IsNullOrEmpty(v.ElevatorCompanyId))
                    {
                        rst = rst.Where(x => x.ElevatorCompanyId == v.ElevatorCompanyId);
                    }
                    if (!string.IsNullOrEmpty(v.ElevatorCompany))
                    {
                        rst = rst.Where(x => x.Company.Contains(v.ElevatorCompany, StringComparison.CurrentCultureIgnoreCase));
                    }

                    var lst = new List<OutElevatorCompanyListItem>();
                    foreach (var r in rst)
                    {
                        lst.Add(new OutElevatorCompanyListItem { ElevatorCompanyId = r.ElevatorCompanyId, ElevatorCompany = r.Company });
                    }

                    var rx = new JsonResult(new OutSearchElevatorCompany { ErrorCode = "0", ErrorMessage = "执行成功。", ElevatorCompanyList = lst });
                    LogHelper.TraceRx($"api/ElevatorCompany/Search.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/ElevatorCompany/Search.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
    }
}

#region ElevatorCompany/Add parameter class definition
public class InElevatorCompany
{
    public string ElevatorCompanyId { get; set; }

    public string ElevatorCompany { get; set; }
}
#endregion

#region ElevatorCompany/Edit parameter class definition
public class InElevatorCompanyEdit
{
    public string ElevatorCompanyId { get; set; }

    public string NewElevatorCompany { get; set; }
}
#endregion

#region ElevatorCompany/Delete parameter class definition
public class DeleteElevatorCompany
{
    [JsonProperty("ElevatorCompanyId")]
    public string ElevatorCompanyId { get; set; }
}
#endregion

#region ElevatorCompany/Search parameter class definition
/// <summary>
/// 输入条件
/// </summary>
public class InSearchElevatorCompany
{
    [JsonProperty("ElevatorCompanyId")]
    public string ElevatorCompanyId { get; set; }
    [JsonProperty("ElevatorCompany")]
    public string ElevatorCompany { get; set; }
}
/// <summary>
/// 返回查询结果
/// </summary>
class OutSearchElevatorCompany : Out
{
    [JsonProperty("ElevatorCompanyList")]
    public List<OutElevatorCompanyListItem> ElevatorCompanyList { get; set; }
}
/// <summary>
/// 结果集数据项
/// </summary>
class OutElevatorCompanyListItem
{
    [JsonProperty("ElevatorCompanyId")]
    public string ElevatorCompanyId { get; set; }
    [JsonProperty("ElevatorCompany")]
    public string ElevatorCompany { get; set; }
}
#endregion
