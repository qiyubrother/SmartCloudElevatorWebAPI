using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartCloudElevatorDataModel;
using SmartCloudElevatorWebAPI;

namespace SmartCloudElevatorWebAPI.Code
{
    public class SysHelper
    {
        public static int GetUserStatus(ControllerBase ctlr)
        {
            #region -97 无效的用户状态
            string authHeader = ctlr.Request.Headers["Authorization"];//Header中的token
            string token = authHeader.Substring("Bearer ".Length).Trim();
            IJwtDecoder decoder = new JwtDecoder(new JsonNetSerializer(), new JwtValidator(new JsonNetSerializer(), new UtcDateTimeProvider()), new JwtBase64UrlEncoder());
            var json = decoder.Decode(token, Env.SecretKey, verify: true); //token为之前生成的字符串
            var jo = JsonConvert.DeserializeObject<JObject>(json);

            var userId = jo["uid"].ToString().ToLower();
            var dc = new DataContext();
            var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == userId);
            if (u == null)
            {
                return -98;
            }
            if (u.Status != "Active")
            {
                return -97;
            }
            #endregion
            return 0;
        }

        public static int GetUserStatus(string userId)
        {
            #region -97 无效的用户状态
            var dc = new DataContext();
            var u = dc.Users.FirstOrDefault(x => x.UserId.ToLower() == userId.ToLower());
            if (u == null)
            {
                return -98;
            }
            if (u.Status != "Active")
            {
                return -97;
            }
            #endregion
            return 0;
        }

        public static string GetUserId(ControllerBase ctlr)
        {
            string authHeader = ctlr.Request.Headers["Authorization"];//Header中的token
            string token = authHeader.Substring("Bearer ".Length).Trim();
            IJwtDecoder decoder = new JwtDecoder(new JsonNetSerializer(), new JwtValidator(new JsonNetSerializer(), new UtcDateTimeProvider()), new JwtBase64UrlEncoder());
            var json = decoder.Decode(token, Env.SecretKey, verify: true); //token为之前生成的字符串
            var jo = JsonConvert.DeserializeObject<JObject>(json);

            return jo["uid"].ToString().ToLower();
        }
    }
}
