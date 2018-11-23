using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using JWT;
using JWT.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartCloudElevatorDataModel;
using SmartCloudElevatorWebAPI;
using SmartCloudElevatorWebAPI.Code;

namespace SmartCloudElevatorWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region POST: api/User/Add
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Add")]
        public JsonResult Post([FromBody] InUser v)
        {
            LogHelper.TraceTx($"api/User/Add.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.UserId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                    LogHelper.Error($"api/User/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.UserName))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的用户名。" });
                    LogHelper.Error($"api/User/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.Mobile)
                    || v.Mobile.Length != 11
                    || v.Mobile[0] != '1'
                    || !v.Mobile.All(x => "0123456789".Contains(x)))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的手机号。" });
                    LogHelper.Error($"api/User/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.RoleId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的角色。" });
                    LogHelper.Error($"api/User/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.HotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-5", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/User/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (v.Status != "Active" && v.Status != "Disabled")
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-6", ErrorMessage = "无效的状态。" });
                    LogHelper.Error($"api/User/Add.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion

                dc.Users.Add(new User
                {
                    HotelId = v.HotelId,
                    Mobile = v.Mobile,
                    Name = v.UserName,
                    RoleId = v.RoleId,
                    Status = v.Status,
                    UserId = v.UserId
                });

                try
                {
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功。" });
                    LogHelper.TraceRx($"api/User/Add.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/Add.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/Delete
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Delete")]
        public JsonResult Post([FromBody] DeleteUser v)
        {
            LogHelper.TraceTx($"api/User/Delete.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.UserId))
                {
                    var rst = new JsonResult(new { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                    LogHelper.Error($"api/User/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.Users.FirstOrDefault(x => x.UserId == v.UserId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                        LogHelper.Error($"api/User/Delete.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    dc.Users.Remove(h);

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功" });
                    LogHelper.TraceRx($"api/User/Delete.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/Delete.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/Edit
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ActionName("Edit")]
        public JsonResult Post([FromBody] InUserEdit v)
        {
            LogHelper.TraceTx($"api/User/Edit.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.UserId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                    LogHelper.Error($"api/User/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewUserName))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的用户名。" });
                    LogHelper.Error($"api/User/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewRoleId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的角色。" });
                    LogHelper.Error($"api/User/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewHotelId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的酒店ID。" });
                    LogHelper.Error($"api/User/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (v.NewStatus != "Active" && v.NewStatus != "Disabled")
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的状态。" });
                    LogHelper.Error($"api/User/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var h = dc.Users.FirstOrDefault(x => x.UserId == v.UserId);
                    if (h == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                        LogHelper.Error($"api/User/Edit.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    h.Name = v.NewUserName;
                    h.RoleId = v.NewRoleId;
                    h.HotelId = v.NewHotelId;
                    h.Status = v.NewStatus;

                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "执行成功" });
                    LogHelper.TraceRx($"api/User/Edit.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/Edit.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/Search
        [HttpPost]
        [Authorize(Policy = "HotelAdmin")]
        [ActionName("Search")]
        public JsonResult Post([FromBody] InSearchUser v)
        {
            LogHelper.TraceTx($"api/User/Search.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                try
                {
                    #region 用户状态检查
                    var _sts = SysHelper.GetUserStatus(this);
                    if (_sts == -97)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                        LogHelper.Error($"api/User/Search.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    else if (_sts == -98)
                    {
                        var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/User/Search.Error::{JsonConvert.SerializeObject(_result.Value)}");
                        return _result;
                    }
                    #endregion
                    var rst = dc.Users.AsEnumerable();
                    if (!string.IsNullOrEmpty(v.UserId))
                    {
                        rst = rst.Where(x => x.UserId.Contains(v.UserId));
                    }
                    if (!string.IsNullOrEmpty(v.UserName))
                    {
                        rst = rst.Where(x => x.Name.Contains(v.UserName, StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (!string.IsNullOrEmpty(v.Mobile))
                    {
                        rst = rst.Where(x => x.Mobile.Contains(v.Mobile));
                    }
                    if (!string.IsNullOrEmpty(v.RoleId))
                    {
                        rst = rst.Where(x => x.RoleId.ToLower() == v.RoleId.ToLower());
                    }
                    var lst = new List<OutSearchUserItem>();
                    foreach (var r in rst)
                    {
                        var d = dc.Hotels.FirstOrDefault(x => x.HotelId == r.HotelId);
                        lst.Add(new OutSearchUserItem { UserId = r.UserId, UserName = r.Name, Mobile = r.Mobile, HotelId = r.HotelId, HotelName = d?.HotelName, RoleId = r.RoleId, Status = r.Status });
                    }
                    var rx = new JsonResult(new OutSearchUser { ErrorCode = "0", ErrorMessage = "执行成功", UserList = lst });
                    LogHelper.TraceRx($"api/User/Search.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/Search.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ChangePassword")]
        public JsonResult Post([FromBody] InChangePassword v)
        {
            LogHelper.TraceTx($"api/User/ChangePassword.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/User/ChangePassword.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/ChangePassword.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.Password))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的密码。" });
                    LogHelper.Error($"api/User/ChangePassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    string authHeader = this.Request.Headers["Authorization"];//Header中的token
                    string token = authHeader.Substring("Bearer ".Length).Trim();
                    IJwtDecoder decoder = new JwtDecoder(new JsonNetSerializer(), new JwtValidator(new JsonNetSerializer(), new UtcDateTimeProvider()), new JwtBase64UrlEncoder());
                    var json = decoder.Decode(token, Env.SecretKey, verify: true); //token为之前生成的字符串
                    var jo = JsonConvert.DeserializeObject<JObject>(json);

                    var userId = jo["uid"].ToString().ToLower();
                    var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == userId);
                    if (u == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                        LogHelper.Error($"api/User/ChangePassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    u.Pwd = v.Password;
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "OK" });
                    LogHelper.TraceRx($"api/User/ChangePassword.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/ChangePassword.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/SetPassword
        [HttpPost]
        [AllowAnonymous]
        [ActionName("SetPassword")]
        public JsonResult Post([FromBody] InFirstLoginSetPassword v)
        {
            LogHelper.TraceTx($"api/User/SetPassword.Tx::{JsonConvert.SerializeObject(v)}");
            #region 参数检查
            if (string.IsNullOrEmpty(v.UserId))
            {
                var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                LogHelper.Error($"api/User/SetPassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                return rst;
            }
            if (string.IsNullOrEmpty(v.Password))
            {
                var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的密码。" });
                LogHelper.Error($"api/User/SetPassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                return rst;
            }
            #endregion
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(v.UserId);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/User/SetPassword.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/SetPassword.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion

                try
                {
                    var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == v.UserId.ToLower());
                    if (u == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                        LogHelper.Error($"api/User/SetPassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    if (!string.IsNullOrEmpty(u.Pwd))
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "用户不是首次登陆。" });
                        LogHelper.Error($"api/User/SetPassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    u.Pwd = v.Password;
                    dc.SaveChanges();

                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "OK" });
                    LogHelper.TraceRx($"api/User/SetPassword.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/SetPassword.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ResetPassword")]
        public JsonResult Post([FromBody] InResetPassword v)
        {
            LogHelper.TraceTx($"api/User/ResetPassword.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 参数检查
                if (string.IsNullOrEmpty(v.UserId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                    LogHelper.Error($"api/User/ResetPassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == v.UserId.ToLower());
                    if (u == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                        LogHelper.Error($"api/User/ResetPassword.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    u.Pwd = string.Empty;
                    dc.SaveChanges();
                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "OK" });
                    LogHelper.TraceRx($"api/User/ResetPassword.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/ResetPassword.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/IsFirstLogin
        [HttpPost]
        [AllowAnonymous]
        [ActionName("IsFirstLogin")]
        public JsonResult Post([FromBody] InIsFirstLogin v)
        {
            LogHelper.TraceTx($"api/User/IsFirstLogin.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/User/IsFirstLogin.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/IsFirstLogin.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.UserId))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的用户ID。" });
                    LogHelper.Error($"api/User/IsFirstLogin.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == v.UserId.ToLower());

                    var rx = new JsonResult(new OutIsFirstLogin { ErrorCode = "0", ErrorMessage = "执行成功。", IsFirstLogin = u != null && string.IsNullOrEmpty(u.Pwd) ? "Yes" : "No" });
                    LogHelper.TraceRx($"api/User/IsFirstLogin.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/IsFirstLogin.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
        #region POST: api/User/SendVerificationCode
        [HttpPost]
        [AllowAnonymous]
        [ActionName("SendVerificationCode")]
        public JsonResult Post([FromBody] InSendVerificationCode v)
        {
            LogHelper.TraceTx($"api/User/SendVerificationCode.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(v.UserId);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/User/SendVerificationCode.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/SendVerificationCode.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.Mobile)
                    || v.Mobile.Length != 11
                    || v.Mobile[0] != '1'
                    || !v.Mobile.All(x => "0123456789".Contains(x)))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的手机号。" });
                    LogHelper.Error($"api/User/SendVerificationCode.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    // 发送验证码
                    var code = new Random((int)TimeHelper.ConvertDateTimeToInt(DateTime.Now)).Next(100000, 1000000).ToString(); // 6位验证码
                    SmsHelper.SendVerificationCode(v.Mobile, code);
                    var rx = new JsonResult(new OutSendVerificationCode { ErrorCode = "0", ErrorMessage = "执行成功。", VerificationCode = GetMD52v2(code), Expired = TimeHelper.ConvertDateTimeToInt(DateTime.Now.AddMinutes(5)).ToString() /* 5分后过期 */, VerificationCodeOriginValue = code });
                    LogHelper.TraceRx($"api/User/SendVerificationCode.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "发送验证码失败。" });
                    LogHelper.Error($"api/User/SendVerificationCode.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        private string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        public string GetMD52v2(string str)
        {
            //创建 MD5对象
            MD5 md5 = MD5.Create();//new MD5();
            //开始加密
            //需要将字符串转换成字节数组
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            //md5.ComputeHash(buffer);

            //返回一个加密好的字节数组
            byte[] MD5Buffer = md5.ComputeHash(buffer);

            //将字节数组 转换成字符串

            /*
            字节数组  --->字符串
             * 1、将字节数组中的每个元素按照指定的编码格式解析成字符串
             * 2、直接ToString()
             * 3、将字节数组中的每个元素都ToString()
             */
            //return Encoding.GetEncoding("GBK").GetString(MD5Buffer);

            string strNew = "";
            for (int i = 0; i < MD5Buffer.Length; i++)
            {
                strNew += MD5Buffer[i].ToString("x2");
            }
            return strNew;
        }
        #endregion
        #region POST: api/User/Login
        public UserController(IOptions<JwtIssuerOptions> jwtOptions, ILoggerFactory loggerFactory, IDistributedCache distributedCache)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            //_logger = loggerFactory.CreateLogger<UserController>();

            //_distributedCache = distributedCache;

        }
        private readonly JwtIssuerOptions _jwtOptions;
        [HttpPost]
        [AllowAnonymous]
        [ActionName("Login")]
        public async Task<JsonResult> UserLoginAsync([FromBody]ApplicationUser applicationUser)
        {
            LogHelper.TraceTx($"api/User/Login.Tx::{JsonConvert.SerializeObject(applicationUser)}");
            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                var rst = new JsonResult(new OutLogin { ErrorCode = "-1", ErrorMessage = "Invalid username or password", AccessToken = "" });
                LogHelper.Error(JsonConvert.SerializeObject(rst.Value));
                return rst;
            }
            #region 用户状态检查
            var _sts = SysHelper.GetUserStatus(applicationUser.UserId);
            if (_sts == -97)
            {
                var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                LogHelper.Error(JsonConvert.SerializeObject(_result.Value));
                return _result;
            }
            else if (_sts == -98)
            {
                var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                LogHelper.Error(JsonConvert.SerializeObject(_result.Value));
                return _result;
            }
            #endregion
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst(JwtRegisteredClaimNames.Sid),
                identity.FindFirst("Role"),
                identity.FindFirst("HotelId"),
                new Claim("uid", applicationUser.UserId),
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var rx = new JsonResult(new OutLogin { ErrorCode = "0", ErrorMessage = "OK", AccessToken = encodedJwt });
            LogHelper.TraceRx($"api/User/Login.Rx::{JsonConvert.SerializeObject(rx.Value)}");

            return rx;
        }

        private void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
        private long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToLocalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        private Task<ClaimsIdentity> GetClaimsIdentity(ApplicationUser user)
        {
            using (var dc = new SmartCloudElevatorDataModel.DataContext())
            {
                var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == user.UserId.ToLower() && x.Pwd.ToLower() == user.Password.ToLower());
                if (u == null) return Task.FromResult<ClaimsIdentity>(null);
                return Task.FromResult(new ClaimsIdentity(
                    new GenericIdentity(user.UserId.ToLower(), "Token"),
                    new[] {
                        new Claim("Role", u.RoleId),
                        new Claim("HotelId", u.HotelId == null ? "" : u.HotelId)
                    }
                    ));
            }
        }
        #endregion
        #region POST: api/User/ChangeMobile
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ChangeMobile")]
        public JsonResult Post([FromBody] InChangeMobile v)
        {
            LogHelper.TraceTx($"api/User/ChangeMobile.Tx::{JsonConvert.SerializeObject(v)}");
            using (var dc = new DataContext())
            {
                #region 用户状态检查
                var _sts = SysHelper.GetUserStatus(this);
                if (_sts == -97)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-97", ErrorMessage = "无效的用户状态。" });
                    LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                else if (_sts == -98)
                {
                    var _result = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(_result.Value)}");
                    return _result;
                }
                #endregion
                #region 参数检查
                if (string.IsNullOrEmpty(v.OldMobile))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的旧手机号。" });
                    LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.NewMobile))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-2", ErrorMessage = "无效的新手机号。" });
                    LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                if (string.IsNullOrEmpty(v.Password))
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的密码。" });
                    LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(rst.Value)}");
                    return rst;
                }
                #endregion
                try
                {
                    var userId = SysHelper.GetUserId(this);
                    var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == userId);
                    if (u == null)
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-4", ErrorMessage = "无效的用户ID。" });
                        LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    if (string.IsNullOrEmpty(u.Pwd))
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-5", ErrorMessage = "用户不是首次登陆。" });
                        LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    if (u.Pwd.ToLower() != v.Password.ToLower())
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-3", ErrorMessage = "无效的密码。" });
                        LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    if (u.Mobile.ToLower() != v.OldMobile.ToLower())
                    {
                        var rst = new JsonResult(new Out { ErrorCode = "-1", ErrorMessage = "无效的旧手机号。" });
                        LogHelper.Error($"api/User/ChangeMobile.Error::{JsonConvert.SerializeObject(rst.Value)}");
                        return rst;
                    }
                    u.Mobile = v.NewMobile;
                    dc.SaveChanges();

                    var rx = new JsonResult(new Out { ErrorCode = "0", ErrorMessage = "OK" });
                    LogHelper.TraceRx($"api/User/Add.Rx::{JsonConvert.SerializeObject(rx.Value)}");
                    return rx;
                }
                catch (Exception ex)
                {
                    var rst = new JsonResult(new Out { ErrorCode = "-98", ErrorMessage = "系统异常。" });
                    LogHelper.Error($"api/User/ChangeMobile.Exception::{JsonConvert.SerializeObject(rst.Value)}");
                    LogHelper.Error(ex.StackTrace);
                    return rst;
                }
            }
        }
        #endregion
    }
}
#region User/Add parameter class definition
public class InUser
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string HotelId { get; set; }
    public string Mobile { get; set; }
    public string RoleId { get; set; }
    public string Status { get; set; }
}
#endregion
#region User/Delete parameter class definition
public class DeleteUser
{
    [JsonProperty("UserId")]
    public string UserId { get; set; }
}
#endregion
#region User/Edit parameter class definition
public class InUserEdit
{
    public string UserId { get; set; }
    public string NewUserName { get; set; }
    public string NewRoleId { get; set; }
    public string NewHotelId { get; set; }
    public string NewStatus { get; set; }
}
#endregion
#region User/Search parameter class definition
/// <summary>
/// 输入条件
/// </summary>
public class InSearchUser
{
    [JsonProperty("UserId")]
    public string UserId { get; set; }
    [JsonProperty("UserName")]
    public string UserName { get; set; }
    [JsonProperty("Mobile")]
    public string Mobile { get; set; }
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("RoleId")]
    public string RoleId { get; set; }
}
/// <summary>
/// 返回查询结果
/// </summary>
class OutSearchUser : Out
{
    [JsonProperty("UserList")]
    public List<OutSearchUserItem> UserList { get; set; }
}
/// <summary>
/// 结果集数据项
/// </summary>
class OutSearchUserItem
{
    [JsonProperty("UserId")]
    public string UserId { get; set; }
    [JsonProperty("UserName")]
    public string UserName { get; set; }
    [JsonProperty("Mobile")]
    public string Mobile { get; set; }
    [JsonProperty("HotelId")]
    public string HotelId { get; set; }
    [JsonProperty("HotelName")]
    public string HotelName { get; set; }
    [JsonProperty("RoleId")]
    public string RoleId { get; set; }
    [JsonProperty("Status")]
    public string Status { get; set; }

}
#endregion
#region User/ChangePasword parameter class definition
public class InChangePassword
{
    public string Password { get; set; }
}
#endregion
#region User/SetPassword parameter class definition
public class InFirstLoginSetPassword
{
    public string UserId { get; set; }
    public string Password { get; set; }
}
#endregion
#region User/ResetPassword parameter class definition
public class InResetPassword
{
    public string UserId { get; set; }
}
#endregion
#region User/IsFirstLogin parameter class definition
public class InIsFirstLogin
{
    public string UserId { get; set; }
}

public class OutIsFirstLogin : Out
{
    [JsonProperty("IsFirstLogin")]
    public string IsFirstLogin { get; set; }
}
#endregion
#region User/SendVerificationCode parameter class definition
public class InSendVerificationCode
{
    public string Mobile { get; set; }
    public string UserId { get; set; }
}

public class OutSendVerificationCode : Out
{
    [JsonProperty("VerificationCode")]
    public string VerificationCode { get; set; }
    [JsonProperty("Expired")]
    public string Expired { get; set; }
    [JsonProperty("VerificationCodeOriginValue")]
    public string VerificationCodeOriginValue { get; set; }
}
#endregion
#region User/Login parameter class definition
public class ApplicationUser
{
    public string UserId { get; set; }
    public string Password { get; set; }
}

class OutLogin : Out
{
    [JsonProperty("AccessToken")]
    public string AccessToken { get; set; }
}
#endregion
#region User/ChangePasword parameter class definition
public class InChangeMobile
{
    public string Password { get; set; }
    public string OldMobile { get; set; }
    public string NewMobile { get; set; }
}
#endregion
