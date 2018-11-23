using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;

namespace SmartCloudElevatorWebAPI.Code
{
    public class SmsHelper
    {
        public static bool SendVerificationCode(string mobile, string verificationCode)
        {
            //产品名称:云通信短信API产品,开发者无需替换
            const String product = "Dysmsapi";
            //产品域名,开发者无需替换
            const String domain = "dysmsapi.aliyuncs.com";
            // TODO 此处需要替换成开发者自己的AK(在阿里云访问控制台寻找)
            const String accessKeyId = "LTAIv1bohHspatsR";
            const String accessKeySecret = "8Gcs1G94fE2griHOdXdOhNBZODUoDz";

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            SendSmsResponse response = null;

            //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
            request.PhoneNumbers = mobile;
            //必填:短信签名-可在短信控制台中找到
            request.SignName = "自助机";
            //必填:短信模板-可在短信控制台中找到
            request.TemplateCode = "SMS_149421985";
            request.TemplateParam = "{\"code\":\"{verificationCode}\"}".Replace("{verificationCode}", verificationCode);
            request.OutId = "yourOutId";
            //请求失败这里会抛ClientException异常
            response = acsClient.GetAcsResponse(request);
            System.Threading.Thread.Sleep(3000);

            return response.Code == "OK";
        }
    }
}
