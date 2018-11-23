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
    public class HotelController : ControllerBase
    {
        #region POST: api/Hotel/Add
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Add")]
        public JsonResult Post([FromBody] Hotel v)
        {
            LogHelper.TraceTx($"api/Hotel/Add.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/Hotel/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.HotelName))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的酒店名称。" });
                    LogHelper.Error($"api/Hotel/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.City))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的城市。" });
                    LogHelper.Error($"api/Hotel/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion

                dc.Hotels.Add(new Hotel
                {
                    HotelId = v.HotelId,
                    HotelName = v.HotelName,
                    City = v.City,
                    Address = v.Address,
                    Comments = v.Comments
                });
                try
                {
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/Hotel/Add.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/Hotel/Delete.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/Hotel/Delete
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Delete")]
        public JsonResult Post([FromBody] DeleteHotel v)
        {
            LogHelper.TraceTx($"api/Hotel/Delete.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/Hotel/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.Hotels.FirstOrDefault(x => x.HotelId == v.HotelId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                        LogHelper.Error($"api/Hotel/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    dc.Hotels.Remove(h);

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/Hotel/Delete.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/Hotel/Delete.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/Hotel/Edit
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Edit")]
        public JsonResult Post([FromBody] EditHotel v)
        {
            LogHelper.TraceTx($"api/Hotel/Edit.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/Hotel/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewHotelName))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的酒店名称。" });
                    LogHelper.Error($"api/Hotel/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewCity))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的城市。" });
                    LogHelper.Error($"api/Hotel/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.Hotels.FirstOrDefault(x => x.HotelId == v.HotelId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                        LogHelper.Error($"api/Hotel/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    h.HotelName = v.NewHotelName;
                    h.City = v.NewCity;
                    h.Address = v.NewAddress;

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/Hotel/Edit.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/Hotel/Edit.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/Hotel/Search
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("Search")]
        public JsonResult Post([FromBody] InSearchHotel v)
        {
            LogHelper.TraceTx($"api/Hotel/Search.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                try
                {
                    #region 用户状态检查
                    var _sts = SysHelper.GetUserStatus(this);
                    if (_sts == -97)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                        LogHelper.Error($"api/Hotel/Search.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    else if (_sts == -98)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/Hotel/Search.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    #endregion
                    var rst = dc.Hotels.AsEnumerable();
                    if (!string.IsNullOrEmpty(v.HotelId))
                    {
                        rst = rst.Where(x => x.HotelId == v.HotelId);
                    }
                    if (!string.IsNullOrEmpty(v.HotelName))
                    {
                        rst = rst.Where(x => x.HotelName.Contains(v.HotelName, StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (!string.IsNullOrEmpty(v.City))
                    {
                        rst = rst.Where(x => x.City.ToLower() == v.City.ToLower());
                    }

                    var lst = new List<OutListItem>();
                    foreach (var r in rst)
                    {
                        lst.Add(new OutListItem { HotelId = r.HotelId, HotelName = r.HotelName, City = r.City, Address = r.Address });
                    }
                    var rx = new JsonResult(new OutSearchHotel { ErrorCode = "0", ErrorMessage = "执行成功。", HotelList = lst });
                    LogHelper.TraceRx($"api/Hotel/Search.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/Hotel/Search.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
    }
}

#region Hotel/Delete parameter class definition
public class DeleteHotel
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
}
#endregion
#region Hotel/Edit parameter class definition
public class EditHotel
{
    public string HotelId { get; set; }
    public string NewHotelName { get; set; }
    public string NewCity { get; set; }
    public string NewAddress { get; set; }
}
#endregion
#region Hotel/Search parameter class definition
/// <summary>
/// 输入条件
/// </summary>
public class InSearchHotel
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("HotelName")]
    public string HotelName { get; set; }
    [JsonProperty("City")]
    public string City { get; set; }
}
/// <summary>
/// 返回查询结果
/// </summary>
class OutSearchHotel : Out
{
    [JsonProperty("HotelList")]
    public List<OutListItem> HotelList { get; set; }
}
/// <summary>
/// 结果集数据项
/// </summary>
class OutListItem
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("HotelName")]
    public string HotelName { get; set; }
    [JsonProperty("City")]
    public string City { get; set; }
    [JsonProperty("Address")]
    public string Address { get; set; }
}
#endregion
