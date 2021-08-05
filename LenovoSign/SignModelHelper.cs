using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Helper;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using RestSharp;
 using Yinhe.ProcessingCenter; 
namespace LenovoSign
{
    public class SignModelHelper
    {
         
        public SignModelHelper()
        {
           
        }

        private static SignModelHelper curInstance = null;
        public static SignModelHelper Instance()
        {
            if (curInstance == null)
            {
                curInstance = new SignModelHelper();
            }
            return curInstance;
        }

        /// <summary>
        /// SMZDM签到
        /// </summary>
        /// <returns></returns>
        public void CheckIn_SMZDM()
        {
            var headSetDic = new Dictionary<string, string>();
           // headSetDic.Add("sec-ch-ua-mobile", "?0");
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
            var accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
 
            headSetDic.Add("Accept-Encoding", "gzip, deflate, br");
            headSetDic.Add("Accept-Language", "en,zh-CN;q=0.9,zh;q=0.8,en-AU;q=0.7");

            var timestamp = QuickMethodHelper.Instance().GetTimeStamp_JS();
            var url = $"https://zhiyou.smzdm.com/user/checkin/jsonp_checkin";
            var ret = url.UrlGetHtml(headers: headSetDic, config: option =>
            {
                option.Referer = "https://www.smzdm.com/";
                option.UserAgent = userAgent;
                option.Accept = accept;
                option.Cookie = GetCookie("SMZDM", "__ckguid=nrf5EfTx9u5HH6oBajQHH6; r_sort_type=score; wt3_sid=%3B999768690672041; __gads=ID=80937374b2fc5834-2285bd7445c50054:T=1608517316:RT=1608517316:S=ALNI_MaxvkZuLQF0a_j9fZaeAvy-CAJqfw; wt3_eid=%3B999768690672041%7C2160808454100683846%232161655189900333766; deeplink_url=smzdm%3A%2F%2Fyuanchuang%2Fa4dr39ew%3Fjson%3D%7B%22channelName%22%3A%22yuanchuang%22%2C%22linkVal%22%3A%22a4dr39ew%22%2C%22keyWord%22%3A%22%22%7D; __jsluid_s=3e2eeab5e62c22bf948fcd96d59ad42c; smzdm_user_source=B5A5EDD53D9CAC37F7A59F54432B8ABF; _gid=GA1.2.582438512.1627969950; ss_ab=ss10; device_id=99387190616279700446029036bcfc3911d28bcefa0140bb219c1c7a3; sess=AT-z6w0IromllMMghS2NVj%2FSK6E5xHJ5IB03Bh6PiqTwYjLGwdNZa%2FKyMR1Ndod7mszrGEhxhhbIPqqdjg5hM25O%2BFub6LmQ0Bb96ixMrWAsixvmAXL1W41GWE%3D; user=user%3A1813784979%7C1813784979; smzdm_id=1813784979; homepage_sug=a; s_his=Dell%20s2721dgf%20nanoips%2CDell%20s2721dgf%2C%E5%B0%8F%E7%B1%B32kips%20165hz%20hdr400%2C%E5%B0%8F%E7%B1%B3%20ips%20165hz%2C2k%20ips%20165hz%2C2k%20ips%20144hz%2C2k%20ips%20%20%E9%AB%98%E5%88%B7%2C2k%20ips%20%E6%B3%95%E4%BA%BA%EF%BC%9B%EF%BC%9B%2C3060ti%20%E6%98%BE%E7%A4%BA%E5%99%A8%2C%E6%98%BE%E7%A4%BA%E5%99%A8; Hm_lvt_9b7ac3d38f30fe89ff0b8a0546904e58=1627969949; zdm_qd=%7B%22referrer%22%3A%22https%3A%2F%2Fwww.baidu.com%2Flink%3Furl%3DjpR98PtE_2cruZ_UGU4NkeoObfgiYtBzSMCdte4sNwIdiAapYcohbKU0C7hhpF4o%26wd%3D%26eqid%3Df447473f0018baa700000006610b3dd4%22%7D; _zdmA.uid=ZDMA.KzFzB8eSs.1628142011.2419200; sensorsdata2015jssdkcross=%7B%22distinct_id%22%3A%221813784979%22%2C%22first_id%22%3A%221766573cdcb3c7-08dab960b875a5-7a1b34-1296000-1766573cdcc1b5%22%2C%22props%22%3A%7B%22%24latest_traffic_source_type%22%3A%22%E7%9B%B4%E6%8E%A5%E6%B5%81%E9%87%8F%22%2C%22%24latest_search_keyword%22%3A%22%E6%9C%AA%E5%8F%96%E5%88%B0%E5%80%BC_%E7%9B%B4%E6%8E%A5%E6%89%93%E5%BC%80%22%2C%22%24latest_referrer%22%3A%22%22%2C%22%24latest_landing_page%22%3A%22https%3A%2F%2Fwww.smzdm.com%2F%22%7D%2C%22%24device_id%22%3A%221766573cdcb3c7-08dab960b875a5-7a1b34-1296000-1766573cdcc1b5%22%7D; Hm_lpvt_9b7ac3d38f30fe89ff0b8a0546904e58=1628142011; _gat_UA-27058866-1=1; _ga=GA1.2.195338680.1608019857; _ga_09SRZM2FDD=GS1.1.1628142012.11.0.1628142028.0");
                //option.Cookie = "__ckguid=C564OT9yGlCAueSirNfRfP5; device_id=213070643316281440298504794b57eb1a92836aae37632441f2f7fcfe; homepage_sug=c; r_sort_type=score; __jsluid_s=ec7bb96ead89e37e92761da80c97b17b; Hm_lvt_9b7ac3d38f30fe89ff0b8a0546904e58=1628144033; zdm_qd=%7B%7D; _gid=GA1.2.389033793.1628144035; smzdm_user_source=94E3AED372237185F0309E739E9FA11D; sess=AT-ndbyueo5ELp3JyJI14L1X%2BR6tNWKrJPym8x1osvoaA7clUEn2Bz0Z3DF9gyPpyAWwmjBMeHaTB83IU1dVoEEwBn17cFgvRxMTs0VF4gOebEOL2u7BpRusXoA; user=user%3A2664303996%7C2664303996; smzdm_id=2664303996; sensorsdata2015jssdkcross=%7B%22distinct_id%22%3A%222664303996%22%2C%22first_id%22%3A%22178580518f0ac-0f7ebdb2eeeef9-71415a3b-2025000-178580518f19c%22%2C%22props%22%3A%7B%22%24latest_traffic_source_type%22%3A%22%E7%9B%B4%E6%8E%A5%E6%B5%81%E9%87%8F%22%2C%22%24latest_search_keyword%22%3A%22%E6%9C%AA%E5%8F%96%E5%88%B0%E5%80%BC_%E7%9B%B4%E6%8E%A5%E6%89%93%E5%BC%80%22%2C%22%24latest_referrer%22%3A%22%22%2C%22%24latest_landing_page%22%3A%22https%3A%2F%2Fzhiyou.smzdm.com%2Fuser%2F%22%7D%2C%22%24device_id%22%3A%22178580518f0ac-0f7ebdb2eeeef9-71415a3b-2025000-178580518f19c%22%7D; _gat_UA-27058866-1=1; _zdmA.uid=ZDMA.b8LvKWkRh.1628145323.2419200; Hm_lpvt_9b7ac3d38f30fe89ff0b8a0546904e58=1628145323; _ga=GA1.2.671831554.1616384583; _ga_09SRZM2FDD=GS1.1.1628144033.1.1.1628145353.0";
            });
            if (ret.Contains("data") && ret.Contains("slogan"))
            {
                var result = ret.GetBsonDocFromJson();
                var solgan = result.GetBsonDocument("data").Text("slogan").HtmlLoad().DocumentNode.InnerText;
                //SendMail("执行通知_sign_smzdm", solgan);
                Console.WriteLine(solgan);
            }
            else {
                SendMail("执行通知_sign_smzdm", "执行错误");
            }
        }

        public  string GetCookie(string className,string defaultCookie="")
        {
            var mongoOP = MongoOpCollection.GetNew121MongoOp_MT("SimpleCrawler");
            var hitCookieObj = mongoOP.FindOne("SimulateCookieCollection", Query.EQ("className", className));
            if (hitCookieObj != null)
            {

                defaultCookie= hitCookieObj.Text("cookie");
             }
            return defaultCookie;
        }



        public void SendMail(string title, string content, string sendDate = "", string arrivedUserIds = "1")
        {
            //转移到121数据库进行等待发送//后续改造为直接发送邮件
            var curMessageInfo = new Yinhe.ProcessingCenter.MessagePushEntity()
            {
                arrivedUserIds = arrivedUserIds,
                title = title,
                content = content,
                sendUserName = "job系统通知",
                sendDate = string.IsNullOrEmpty(sendDate) ? DateTime.Now.AddMinutes(2).ToString("yyyy-MM-dd HH:mm:ss") : sendDate

            };
            var mongop = MongoOpCollection.Get38MongoOp("WorkPlanManage");
            var dataOp = new DataOperation(mongop);
            Yinhe.ProcessingCenter.MessagePushQueueHelper pushQueueHelper = new Yinhe.ProcessingCenter.MessagePushQueueHelper(dataOp);
            pushQueueHelper.PushMessage(curMessageInfo);
        }
    }
}