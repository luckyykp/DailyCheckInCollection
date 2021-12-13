using CommandLine;
using System;
using Helper;
using Yinhe.ProcessingCenter;
using System.Threading;
using MongoDB.Bson;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Generic;

namespace LenovoSign
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // TestTESLA();
            //QuickQuartzActionJob.Instance().Run((obj) => { /*TestTESLA*/(); }, "key", "124", "0 0 0-6 * * ? ");
            //Console.Read();
           // return;
            string username = "";
            string password = "";
            string mode = "";//smzdm

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
                case "tsl":
                    TestTESLA(username);//tesla

                    break;
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
            //执行小游戏签到
            var step5= LenovoAcitvieSign_H5(step3.Res.Lenovoid.ToString(), step3.Res.Sessionid, step3.Res.Cookieval);
            if (step4.Res.Success) Console.WriteLine("签到成功");
            var result = $"执行结果:{step4.Res.Success}_{username}_AcitvieSign执行结果『{step5}』";
            Console.WriteLine($"签到结果:{step4.Res.RewardTips}_{username}");
            SignModelHelper.Instance().SendMail("执行通知_sign", result);
            Console.WriteLine();
        }
        
        /// <summary>
        /// 种树小游戏登录并且完成自动任务
        /// </summary>
        public static bool LenovoAcitvieSign_H5(string lenovoid,string sessionid,string token)
        {
            try
            {
                var ssoHelper = new LenovoUtils_Membership(lenovoid, sessionid, token);
                if (ssoHelper.SSOCheck())
                {
                    var ret_check = ssoHelper.CheckIn();
                    var taskList = ssoHelper.QueryTaskList();
                    Console.WriteLine($"获取到任务{taskList.Count}");
                    var retryCount = 0;
                    while (taskList.Count > 0 && retryCount++ <= 2)
                    {
                        foreach (var doc in taskList)
                        {
                            var id = doc.Text("id");
                            if (doc.Text("finished") == "0")
                            {
                                var ret1 = ssoHelper.FinishUserTask(id);
                                if (ret1)
                                {
                                    Console.WriteLine($"完成任务{doc.Text("name")}_{doc.Text("bonusValue")}");
                                }
                                Thread.Sleep(1000);
                            }
                            if (doc.Text("prizeState") == "0")
                            {
                                Console.WriteLine($"领取奖励任务{doc.Text("name")}_{doc.Text("bonusValue")}");
                                var ret2 = ssoHelper.receivePrize(id);
                                Thread.Sleep(1000);
                            }
                        }
                        taskList = ssoHelper.QueryTaskList();
                        if (taskList.Count > 0)
                        {
                            Console.WriteLine($"剩余任务{taskList}");
                            Thread.Sleep(60000);//60秒后重试
                        }

                    }
                    //Console.WriteLine(ret.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;

        }
        public class Options
        {
            [Option('u', "username", Required = true, HelpText = "用户名/地区等")]
            public string username { get; set; }

            [Option('p', "password", Required = true, HelpText = "密码")]
            public string password { get; set; }

            [Option('m', "mode", Required = false, HelpText = "模式")]
            public string mode { get; set; } = "";
        }

        public static void TestAmazon()
        {
            var url = "https://www.amazon.cn/dp/B07MBQPQ62/?coliid=IOCT298XR3KYN&colid=22SP37IGDPKFX&ref_=lv_ov_lig_dp_it&th=1";
            var ret=url.UrlGetHtml();
            var htmlObj = ret.HtmlLoad();
            var twisterAvailability = htmlObj.GetElementbyId("availability");
            if (twisterAvailability!=null&&!string.IsNullOrEmpty(twisterAvailability.InnerText)&& !twisterAvailability.InnerText.Contains("目前无货"))
            {
                //发送邮件
                var result = $"执行结果:无内鬼快点交易";
                Console.WriteLine($"查找结果:{result}");
                SignModelHelper.Instance().SendMail("执行通知_sign", result);
            }
        }

        public static void TestTESLA(string place)
        {
            Console.WriteLine("start TestTESLAJobMonitor");
            Tesla_Work_Software(place);
            Console.WriteLine("end TestTESLAJobMonitor");
        }
        /// <summary>
        /// xm，泉州是否有匹配的work
        /// </summary>
        public static void Tesla_Work_Software(string place="厦门")
        {
            var info = new StringBuilder();
            try
            {
                var url_xm = $"https://www.tesla.cn/careers/search/?country=CN&location={HttpUtility.UrlEncode(place)}";
               // var url_qz = $"https://www.tesla.cn/careers/search/?country=CN&location={HttpUtility.UrlEncode("泉州")}";
                var keyWords = new string[] { "c#", "netcore", "softwareengineer", "go", "" };
              
                var url = "https://www.tesla.cn/cua-api/apps/careers/state?1=2";
                var ret = url.UrlGetHtml(config:(conf)=> {

                    conf.Accept = "application/json, text/plain, */*";
                    conf.Referer = "https://www.tesla.cn/careers/search/?country=CN&department=6&location=%E4%B8%8A%E6%B5%B7";
                    conf.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36";
                });
                if (string.IsNullOrEmpty(ret))
                {
                    
                    return;
                }
                //dp=6  软件开发工程师
                var root = ret.GetBsonDocFromJson();
                var listings = root.GetBsonDocumentList("listings").ToList();
                var lookUp = root.GetBsonDocument("lookup");
                var locations = lookUp.GetBsonDocument("locations");
                var departs = lookUp.GetBsonDocument("departments");
                var location_xms = locations.Elements.Where(c => c.Value.ToString().Contains(place)).Select(c => c.Name).ToList();
               // var location_qzs = locations.Elements.Where(c => c.Value.ToString().Contains("泉州")).Select(c => c.Name).ToList();
                var depart_software = departs.Elements.Where(c => c.Value.ToString().Contains("Engineering & Information Technology")).FirstOrDefault()?.Name.ToString();
                if (string.IsNullOrEmpty(depart_software))
                {
                    depart_software = "6";
                }

                var sofewareJobs = listings.Where(c => c.Text("dp") == depart_software).ToList();//软件开发

                var jobs_xm = sofewareJobs.Where(c => location_xms.Contains(c.Text("l"))).ToList();//厦门工作
               // var jobs_qz = sofewareJobs.Where(c => location_qzs.Contains(c.Text("l"))).ToList();//泉州工作
                
                if (jobs_xm.Count > 0)
                {
                    var jobDetail = jobs_xm.Aggregate("", (s1, s2) => $"{s1}\n{s2.Text("t")}_{s2.Text("dp")}_{s2.Text("l")}");
                    if (jobs_xm.Any(d => keyWords.Any(c => d.Text("t").Replace(" ", "").ToLower().Contains(c))))
                    {
                        jobDetail += "\nNetCore 匹配";
                    }
                    else
                    {
                        jobDetail += "\nNetCore 暂无";
                    }
                    info.AppendLine($"执行结果TESLA:厦门咸鱼翻身的机会来了注意查收 每日更新『{url_xm}』\n详情:{jobDetail}");
                }
                 
            }
            catch (Exception ex)
            {
                info.AppendLine(ex.Message+"请更新自动化程序");
            }
            //发送邮件
            if (!string.IsNullOrEmpty(info.ToString())) {
                Console.WriteLine($"查找结果:{info.ToString()}");
                SignModelHelper.Instance().SendMail("执行通知_sign", info.ToString());
            }
           
        }

 
    }
}