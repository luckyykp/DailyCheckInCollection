using CommandLine;
using System;
using Helper;
using Yinhe.ProcessingCenter;
using System.Threading;

namespace LenovoSign
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string username = "";
            string password = "";
            string mode = "";
            Parser.Default.ParseArguments<Options>(args)
               .WithParsed(o =>
               {
                   username = o.username;
                   password = o.password;
                   mode = o.mode;
               });
            Console.WriteLine(mode);
            switch (mode.ToLower())
            {
                case "smzdm":
                    SignModelHelper.Instance().CheckIn_SMZDM();//什么值得买签到
                   
                    break;
                default:
                    LenovoSign(args);//联想签到
                    break;
            }
            Console.WriteLine("3秒后退出");
            Thread.Sleep(3000);
           // LenovoSign(args);//联想签到
        }

        public static void LenovoSign(string[] args)
        {
            string username = "";
            string password = "";
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                Parser.Default.ParseArguments<Options>(args)
                  .WithParsed(o =>
                  {
                      username = o.username;
                      password = o.password;
                  });


            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("用户名密码必须提供");
                // Console.ReadLine();
                return;
            }

            var baseInfoModel = new LenovoBaseInfoModel
            {
                imei = "16213049994264620",
                phoneincremental = "eng.073325",
                phoneproduct = "havoc",
                phonedisplay = "QQ3A.25.001",
                appVersion = "V5.0.1",
                phoneModel = "Redmi 6 Plus",
                lenovoClubChannel = "xiaomi",
                phonebrand = "Xiaomi",
                androidsdkversion = "29",
                loginName = "",
                phoneManufacturer = "Xiaomi",
                systemVersion = "10"
            };

            var t = new LenovoUtils(username, password, baseInfoModel);
            var step1 = t.Login();
            var step2 = t.GetToken(step1);
            var step3 = t.GetSessionId(step2);
            var step4 = t.DaySign(step3.Res.Lenovoid, step3.Res.Sessionid, step3.Res.Token);

            if (step4.Res.Success) Console.WriteLine("签到成功");
            var result = $"执行结果:{step4.Res.Success}_{username}";
            Console.WriteLine($"签到结果:{step4.Res.RewardTips}_{username}");
            SignModelHelper.Instance().SendMail("执行通知_sign", result);
            Console.WriteLine();
        }


        public class Options
        {
            [Option('u', "username", Required = true, HelpText = "用户名")]
            public string username { get; set; }

            [Option('p', "password", Required = true, HelpText = "密码")]
            public string password { get; set; }

            [Option('m', "mode", Required = false, HelpText = "模式")]
            public string mode { get; set; }
        }

    }
}