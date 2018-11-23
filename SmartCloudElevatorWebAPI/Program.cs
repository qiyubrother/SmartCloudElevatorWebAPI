using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SmartCloudElevatorWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 初始化系统日志
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"logs\SmartCloudElevatorWebAPI.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            #endregion

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseUrls("https://192.168.0.6:5039/") // 指定默认端口号
                .UseStartup<Startup>();
    }
}
