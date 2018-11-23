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
    public class HotelManagementController : ControllerBase
    {
        #region POST: api/HotelManagement/SearchElevator
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("SearchElevator")]
        public JsonResult Post([FromBody] InHotelSearchElevator v)
        {
            LogHelper.TraceTx($"api/HotelManagement/SearchElevator.Tx::{JsonConvert.SerializeObject(v)}");
            if (string.IsNullOrEmpty(v.HotelId))
            {
                var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                LogHelper.Error($"api/HotelManagement/SearchElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                return rst;
            }
            using (var dc = new DataContext())
            {
                try
                {
                    #region 用户状态检查
                    var _sts = SysHelper.GetUserStatus(this);
                    if (_sts == -97)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                        LogHelper.Error($"api/HotelManagement/SearchElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    else if (_sts == -98)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/HotelManagement/SearchElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    #endregion
                    var rst = dc.HotelElevators.Where(x => x.HotelId == v.HotelId).ToList();

                    var lst = new List<OutHotelSearchElevatorListItem>();
                    foreach (var r in rst)
                    {
                        var moduleName = string.Empty;
                        var elevatorCompanyName = string.Empty;
                        var elevatorCompanyId = string.Empty;
                        var elevatorModuleItem = dc.ElevatorIdMudules.FirstOrDefault(x => x.ElevatorId == r.ElevatorId);
                        if (elevatorModuleItem != null)
                        {
                            moduleName = elevatorModuleItem.ModuleName == null ? string.Empty : elevatorModuleItem.ModuleName;
                            var elevatorCompany = dc.ElevatorCompanys.FirstOrDefault(x => x.ElevatorCompanyId == elevatorModuleItem.ElevatorCompanyId);
                            if (elevatorCompany != null)
                            {
                                elevatorCompanyId = elevatorCompany.ElevatorCompanyId;
                                elevatorCompanyName = elevatorCompany.Company;
                            }
                        }
                        lst.Add(new OutHotelSearchElevatorListItem { ElevatorId = r.ElevatorId, ModuleName = moduleName, ElevatorCompanyId = elevatorCompanyId, ElevatorCompany = elevatorCompanyName });
                    }

                    var rx = new JsonResult(new OutHotelSearchElevator { ErrorCode = "0", ErrorMessage = "执行成功。", ElevatorList = lst });
                    LogHelper.TraceRx($"api/HotelManagement/SearchElevator.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/SearchElevator.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/HotelManagement/SearchRobot
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("SearchRobot")]
        public JsonResult Post([FromBody] InHotelSearchRobot v)
        {
            LogHelper.TraceTx($"api/HotelManagement/SearchRobot.Tx::{JsonConvert.SerializeObject(v)}");
            if (string.IsNullOrEmpty(v.HotelId))
            {
                var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                LogHelper.Error($"api/HotelManagement/SearchRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                return rst;
            }
            using (var dc = new DataContext())
            {
                try
                {
                    #region 用户状态检查
                    var _sts = SysHelper.GetUserStatus(this);
                    if (_sts == -97)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                        LogHelper.Error($"api/HotelManagement/SearchRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    else if (_sts == -98)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/HotelManagement/SearchRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    #endregion
                    var rst = dc.HotelRobots.Where(x => x.HotelId == v.HotelId).ToList();

                    var lst = new List<OutHotelSearchRobotListItem>();
                    foreach (var r in rst)
                    {
                        var uniqueRobotSNs = dc.HotelRobots.Where(x => x.HotelId == r.HotelId).ToList();
                        var robotCompanyName = string.Empty;
                        var robotCompanyId = string.Empty;
                        var abbr = string.Empty;
                        var tag = string.Empty;
                        foreach (var uniqueRobotSN in uniqueRobotSNs)
                        {
                            var robotMapItem = dc.RobotMaps.FirstOrDefault(x => x.UniqueRobotSN == uniqueRobotSN.UniqueRobotSN);
                            if (robotMapItem != null)
                            {
                                var company = dc.RobotCompanys.FirstOrDefault(x => x.RobotCompanyID == robotMapItem.RobotCompanyId);
                                if (company != null)
                                {
                                    robotCompanyName = company.Company;
                                    robotCompanyId = company.RobotCompanyID;
                                    abbr = company.CompanyAbbreviation;
                                    tag = company.CompanyTag;
                                }
                                lst.Add(new OutHotelSearchRobotListItem
                                {
                                    RobotCompanyId = robotCompanyId,
                                    RobotCompany = robotCompanyName,
                                    RobotSN = robotMapItem.RobotSN,
                                    UniqueRobotSN = uniqueRobotSN.UniqueRobotSN,
                                    CompanyAbbreviation = abbr,
                                    CompanyTag = tag
                                });
                            }
                        }
                    }

                    var rx = new JsonResult(new OutHotelSearchRobot { ErrorCode = "0", ErrorMessage = "执行成功。", RobotList = lst });
                    LogHelper.TraceRx($"api/HotelManagement/SearchRobot.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/SearchRobot.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/HotelManagement/AddElevator
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("AddElevator")]
        public JsonResult Post([FromBody] InHotelAddElevator v)
        {
            LogHelper.TraceTx($"api/HotelManagement/AddElevator.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/HotelManagement/AddElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/AddElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/HotelManagement/AddElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.ElevatorId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的电梯ID。电梯ID不能为空。" });
                    LogHelper.Error($"api/HotelManagement/AddElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.ElevatorCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的电梯公司ID。" });
                    LogHelper.Error($"api/HotelManagement/AddElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    if (dc.HotelElevators.Any(x => x.ElevatorId == v.ElevatorId))
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-21", ErrorMessage = "无效的电梯ID。电梯ID已存在。" });
                        LogHelper.Error($"api/HotelManagement/AddElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    if (dc.ElevatorIdMudules.Any(x => x.ElevatorId == v.ElevatorId))
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-21", ErrorMessage = "无效的电梯ID。电梯ID已存在。" });
                        LogHelper.Error($"api/HotelManagement/AddElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }

                    dc.Add(new HotelElevator
                    {
                        HotelId = v.HotelId,
                        ElevatorId = v.ElevatorId
                    });
                    dc.Add(new ElevatorIdModule
                    {
                        ElevatorCompanyId = v.ElevatorCompanyId,
                        ModuleName = v.ModuleName,
                        ElevatorId = v.ElevatorId
                    });

                    dc.SaveChanges();

                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/HotelManagement/AddElevator.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/AddElevator.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/HotelManagement/AddRobot
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("AddRobot")]
        public JsonResult Post([FromBody] InHotelAddRobot v)
        {
            LogHelper.TraceTx($"api/HotelManagement/AddRobot.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/HotelManagement/AddRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/AddRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/HotelManagement/AddRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RobotSN))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的机器人SN。" });
                    LogHelper.Error($"api/HotelManagement/AddRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RobotCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的机器人公司ID。" });
                    LogHelper.Error($"api/HotelManagement/AddRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.UniqueRobotSN))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的唯一机器人SN。" });
                    LogHelper.Error($"api/HotelManagement/AddRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    if (dc.RobotMaps.Any(x => x.UniqueRobotSN == v.UniqueRobotSN))
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的唯一机器人SN。" });
                        LogHelper.Error($"api/HotelManagement/AddRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    dc.Add(new HotelRobot
                    {
                        HotelId = v.HotelId,
                        UniqueRobotSN = v.UniqueRobotSN
                    });
                    dc.Add(new RobotMap
                    {
                        RobotCompanyId = v.RobotCompanyId,
                        RobotSN = v.RobotSN,
                        UniqueRobotSN = v.UniqueRobotSN
                    });

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/HotelManagement/AddRobot.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/AddRobot.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/HotelManagement/DeleteElevator
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("DeleteElevator")]
        public JsonResult Post([FromBody] HotelDeleteElevator v)
        {
            LogHelper.TraceTx($"api/HotelManagement/DeleteElevator.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/HotelManagement/DeleteElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/DeleteElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/HotelManagement/DeleteElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.ElevatorId))
                {
                    var rst = new JsonResult(new { ErrorCode = "-2", ErrorMessage = "无效的电梯模块ID。" });
                    LogHelper.Error($"api/HotelManagement/DeleteElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.HotelElevators.FirstOrDefault(x => x.ElevatorId == v.ElevatorId && x.HotelId == v.HotelId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/HotelManagement/DeleteElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    dc.HotelElevators.Remove(h);
                    var em = dc.ElevatorIdMudules.FirstOrDefault(x => x.ElevatorId == v.ElevatorId);
                    if (em != null)
                    {
                        dc.ElevatorIdMudules.Remove(em);
                    }
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/HotelManagement/DeleteElevator.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/DeleteElevator.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/HotelManagement/DeleteRobot
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("DeleteRobot")]
        public JsonResult Post([FromBody] HotelDeleteRobot v)
        {
            LogHelper.TraceTx($"api/HotelManagement/DeleteRobot.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/HotelManagement/DeleteRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/DeleteRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/HotelManagement/DeleteRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.UniqueRobotSN))
                {
                    var rst = new JsonResult(new { ErrorCode = "-2", ErrorMessage = "无效的唯一机器人SN。" });
                    LogHelper.Error($"api/HotelManagement/DeleteRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.HotelRobots.FirstOrDefault(x => x.UniqueRobotSN == v.UniqueRobotSN && x.HotelId == v.HotelId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/HotelManagement/DeleteRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    dc.HotelRobots.Remove(h);
                    var rm = dc.RobotMaps.FirstOrDefault(x => x.UniqueRobotSN == v.UniqueRobotSN);
                    if (rm != null)
                    {
                        dc.RobotMaps.Remove(rm);
                    }
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/HotelManagement/DeleteRobot.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/DeleteRobot.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/HotelManagement/EditElevator
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("EditElevator")]
        public JsonResult Post([FromBody] InHotelEditElevator v)
        {
            LogHelper.TraceTx($"api/HotelManagement/EditElevator.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.ElevatorId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的电梯ID。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.ElevatorCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的电梯公司ID。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                var h1 = dc.HotelElevators.FirstOrDefault(x => x.HotelId == v.HotelId && x.ElevatorId == v.ElevatorId);
                if (h1 == null)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                var h2 = dc.ElevatorIdMudules.FirstOrDefault(x => x.ElevatorId == v.ElevatorId);
                if (h2 == null)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                h2.ModuleName = v.NewModuleName;
                try
                {
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/HotelManagement/EditElevator.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditElevator.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/HotelManagement/EditRobot
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("EditRobot")]
        public JsonResult Post([FromBody] InHotelEditRobot v)
        {
            LogHelper.TraceTx($"api/HotelManagement/EditRobot.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RobotSN))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的机器人SN。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RobotCompanyId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的机器人公司ID。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                var h1 = dc.HotelRobots.FirstOrDefault(x => x.HotelId == v.HotelId && x.UniqueRobotSN == v.NewUniqueRobotSN);
                if (h1 == null)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                var h2 = dc.RobotMaps.FirstOrDefault(x => x.RobotCompanyId == v.RobotCompanyId && x.RobotSN == v.RobotSN);
                if (h2 == null)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                h2.UniqueRobotSN = v.NewUniqueRobotSN;
                try
                {
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/HotelManagement/EditRobot.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/HotelManagement/EditRobot.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion

    }
}

#region HotelManagement/SearchElevator parameter class definition
/// <summary>
/// 输入条件
/// </summary>
public class InHotelSearchElevator
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
}
/// <summary>
/// 返回查询结果
/// </summary>
class OutHotelSearchElevator : Out
{
    [JsonProperty("ElevatorList")]
    public List<OutHotelSearchElevatorListItem> ElevatorList { get; set; }
}
/// <summary>
/// 结果集数据项
/// </summary>
class OutHotelSearchElevatorListItem
{
    [JsonProperty("ElevatorId")]
    public string ElevatorId { get; set; }
    [JsonProperty("ElevatorCompanyId")]
    public string ElevatorCompanyId { get; set; }
    [JsonProperty("ElevatorCompany")]
    public string ElevatorCompany { get; set; }
    [JsonProperty("ModuleName")]
    public string ModuleName { get; set; }
}
#endregion

#region HotelManagement/SearchRobot parameter class definition
/// <summary>
/// 输入条件
/// </summary>
public class InHotelSearchRobot
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
}
/// <summary>
/// 返回查询结果
/// </summary>
class OutHotelSearchRobot : Out
{
    [JsonProperty("RobotList")]
    public List<OutHotelSearchRobotListItem> RobotList { get; set; }
}
/// <summary>
/// 结果集数据项
/// </summary>
class OutHotelSearchRobotListItem
{
    [JsonProperty("RobotSN")]
    public string RobotSN { get; set; }
    [JsonProperty("UniqueRobotSN")]
    public string UniqueRobotSN { get; set; }
    [JsonProperty("RobotCompanyId")]
    public string RobotCompanyId { get; set; }
    [JsonProperty("RobotCompany")]
    public string RobotCompany { get; set; }
    [JsonProperty("CompanyAbbreviation")]
    public string CompanyAbbreviation { get; set; }
    [JsonProperty("CompanyTag")]
    public string CompanyTag { get; set; }
}
#endregion

#region HotelManagement/AddElevator parameter class definition
public class InHotelAddElevator
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("ElevatorId")]
    public string ElevatorId { get; set; }
    [JsonProperty("ModuleName")]
    public string ModuleName { get; set; }
    [JsonProperty("ElevatorCompanyId")]
    public string ElevatorCompanyId { get; set; }
}
#endregion

#region HotelManagement/AddRobot parameter class definition
public class InHotelAddRobot
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("RobotSN")]
    public string RobotSN { get; set; }
    [JsonProperty("RobotCompanyId")]
    public string RobotCompanyId { get; set; }
    [JsonProperty("UniqueRobotSN")]
    public string UniqueRobotSN { get; set; }
}
#endregion

#region HotelManagement/DeleteElevator parameter class definition
public class HotelDeleteElevator

{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("ElevatorId")]
    public string ElevatorId { get; set; }
}
#endregion

#region HotelManagement/DeleteRobot parameter class definition
public class HotelDeleteRobot
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("UniqueRobotSN")]
    public string UniqueRobotSN { get; set; }
}
#endregion

#region HotelManagement/EditElevator parameter class definition
public class InHotelEditElevator
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("ElevatorId")]
    public string ElevatorId { get; set; }
    [JsonProperty("NewModuleName")]
    public string NewModuleName { get; set; }
    [JsonProperty("ElevatorCompanyId")]
    public string ElevatorCompanyId { get; set; }
}
#endregion

#region HotelManagement/EditRobot parameter class definition
public class InHotelEditRobot
{
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("RobotSN")]
    public string RobotSN { get; set; }
    [JsonProperty("RobotCompanyId")]
    public string RobotCompanyId { get; set; }
    [JsonProperty("NewUniqueRobotSN")]
    public string NewUniqueRobotSN { get; set; }
}
#endregion