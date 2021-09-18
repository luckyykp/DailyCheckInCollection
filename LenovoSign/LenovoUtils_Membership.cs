using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestSharp;
using Helper;
using System.Security.Cryptography;
using SimpleCrawler;
using MongoDB.Bson;

namespace LenovoSign
{
    /// <summary>
    /// https://mmembership.lenovo.com.cn/index.html#/app
    /// </summary>
    public class LenovoUtils_Membership
    {
        
        CrawlSettings settings = new CrawlSettings();
        private const string UserAgent = "Apache-HttpClient/UNAVAILABLE(java 1.5)";
        private string cookie = "leid=1.eJfpp2xqGDk; LA_F_T_10000001=1622454306629; LA_C_Id=_ck21053117450616316148190146972; LA_M_W_10000001=_ck21053117450616316148190146972%7C10000001%7C%7C%7C; LA_C_C_Id=_sk202105311745030.77073600.4178; _ga=GA1.3.175278496.1622454309; OUTFOX_SEARCH_USER_ID_NCOO=12481159.564909633; s_cc=true; s_nr=1622518737830; s_sq=%5B%5BB%5D%5D; LA_F_T_10000071=1622518737843; LA_V_T_N_S_10000071=1622518737843; LA_V_T_N_10000071=1622518737843; LA_R_T_10000071=1622518737843; LA_M_10000071=%3A%3Amarket_type_free_search%3A%3A%3A%3Abaidu%3A%3A%3A%3A%3A%3A%3A%3Awww.baidu.com%3A%3A%3A%3Apmf_from_free_search; LA_M_T_10000071=pmf_from_free_search; LA_M_W_10000071=_ck21053117450616316148190146972%7C10000071%7C%7C%7C; pt_63f31cff=uid=d1VdgU9/tDtndhQS0gfo2A&nid=1&vid=ZKWMuKxLYshd8ttmKG4ARA&vn=1&pvn=1&sact=1622518738193&to_flag=0&pl=56gdVo-onhe1KOi3gymE7A*pt*1622518738193; pt_s_63f31cff=vt=1622518738193&cad=; LA_F_T_10000008=1623295456620; LA_M_10000008=%3A%3Amarket_type_free_search%3A%3A%3A%3Abaidu%3A%3A%3A%3A%3A%3A%3A%3Awww.baidu.com%3A%3A%3A%3Apmf_from_free_search; LA_M_T_10000008=pmf_from_free_search; LA_F_T_10000231=1623310440586; LA_M_W_10000231=_ck21053117450616316148190146972%7C10000231%7C%7C%7C; LA_V_T_N_10000001=1623310473325; LA_V_T_N_S_10000001=1623310473325; LA_R_C_10000008=1; LA_R_T_10000008=1623727977642; LA_V_T_N_S_10000008=1623727977642; LA_V_T_N_10000008=1623727977642; LA_M_W_10000008=_ck21053117450616316148190146972%7C10000008%7C%7Cpc_1144%7Cpc_pc; UM_distinctid=17ae619e8f191a-079ddd50515c0c-6373264-13c680-17ae619e8f2b09; LA_F_T_10000152=1627362963209; LA_R_T_10000152=1627362963209; LA_V_T_10000152=1627362963209; LA_M_W_10000152=_ck21053117450616316148190146972%7C10000152%7C%7Cpc_%7Cpc_child; LA_M_10000001=in-push%3A%3Alxsj%3A%3Az00021891t008%3A%3A%3A%3A%3A%3A%3A%3Awww.lenovo.com.cn%3A%3A%3A%3Apmf_from_adv; LA_M_T_10000001=pmf_from_adv; LA_R_C_10000231=4; LA_R_T_10000231=1631777243144; LA_M_T_10000231=pmf_from_adv; LA_M_10000231=in-push%3A%3Ahtldy1%3A%3Az00022414t001%3A%3A%3A%3A%3A%3A%3A%3Acn.bing.com%3A%3A%3A%3Apmf_from_adv; LA_R_C_10000001=8; LA_R_T_10000001=1631777261696; LA_V_T_10000001=1631777261696; qrtoken=l1984-00b6a360-5e00-4e5d-afd1-0762995cd96e; _gid=GA1.3.1245256041.1631777262; cerpreg-passport=\"|2|1631777339|1634369339|bGVub3ZvSWQ6MTE6MTAyMDQ0NTg0MDh8bG9naW5OYW1lOjExOjE1OTU5MjY2ODIzfG1lbWJlcklkOjEwOjE2MzQzNjkzMzl8Z3JvdXBDb2RlOjE6MXxpc0xlbm92bzoxOjA=|jYTkK2olI2rIt2NvXlUVi46y64yXy3KiDmnf3tGLDYKtNX//3f238tewPycCFzVOxqdH4MtgHVWGS37bV2HovuIMxSE9xiv1t9qCYtrzA68QVjVEYWgZhmTk0kx9VeIuHA9NzXkM4ATFRLwLYzjOqh0U6JegwaMkyeWJf1dQWiLmUvilQLelt4KL6MzO6KTtvH2YQDwltHpWyL7Rm2iIuL+U+cTHFKKbKpIJQg/X2zBa4nFJ/X3FRL/K2PCkX/b8I8jqAAONmYJujJXEsalV+ssX424SsqIghipACPNFxpwe6v5e8a2Fqto7M0kwerwQb7eDAe3mkhO0cOqd2JufZA==|\"; ar=2; lenovoId=10204458408; serviceToken=Bearer%20eyJhbGciOiJIUzI1NiJ9.eyJlbmNyeU1zZyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlzcyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlhdCI6MTYzMTY5MTA2MCwianRpIjoiNGMwYThiNDljMTY2NGZhMWE1MTNkM2RjMDNhMzllMGEifQ.8KDrmgFyOmNoUsoXunoPm5N8pCt0ZvEqoMcuj-weJdo; LA_V_T_10000231=1631781599191";

        private string accessToken = "\"|2|1631789182|1634381182|bGVub3ZvSWQ6MTE6MTAyMDQ0NTg0MDh8bG9naW5OYW1lOjExOjE1OTU5MjY2ODIzfG1lbWJlcklkOjEwOjE2MzQzODExODJ8Z3JvdXBDb2RlOjE6MXxpc0xlbm92bzoxOjA=|Gq3cPORz5P0XTJLN7dGjvYLCyZmpszrEkU5NBqnp5AXbThxdN3uSdpM7btOHxX+14i2yRysqKY6p7HKmnyq+kKnVu0cAwf9ury6uEYr6SrNM26aWWcKxDzPbUvCIqI4MP0Amuu5hmat99ze+n1e++Eee9AqA5ozg6RaYtuAZINyl3uRJfVpkhkqa+xUzJr1DDo8YqGpa1BMuJjlpAuybOw+/Lu7DWrbCj9PwNLNBuYL6bALpdjzgq/RjQrHZo+lgo7sdilHPktbTDgkJc6Z9pITYg98xn7/Hpw/l5vtbFbAcLoXh+o4/wLrRZgv+tZvjA5VVff81i20D9Pd8bKf8/A==|\"";
        private string serviceToken = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJlbmNyeU1zZyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlzcyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlhdCI6MTYzMTY5MTA2MCwianRpIjoiNGMwYThiNDljMTY2NGZhMWE1MTNkM2RjMDNhMzllMGEifQ.8KDrmgFyOmNoUsoXunoPm5N8pCt0ZvEqoMcuj-weJdo";
        private string serviceAuthentication= "eyJhbGciOiJIUzI1NiJ9.eyJzZXJ2aWNlTmFtZSI6IjM5MiIsInNlcnZpY2VLZXkiOiIxMzQ1MTEyZGI4YmE0MjBhYjI1MzNjOTc1NjgzNjBjNCIsInNlcnZpY2VUeXBlIjoiMSIsInNlcnZpY2VBcHAiOiIxODMiLCJzZXJ2aWNlQ2x1c3RlciI6IjEwIiwianRpIjoiOWU2ODQ0M2E2ZDFiNDllNzk1YTNlZGFmODJhZmQxNjEiLCJpYXQiOjE1NTU5MDU5NDl9.rYH1XF9xjUgW9w-4XD6OxVzal_iK3qLvPxzkPBfo0fI";
         private readonly WebProxy _proxy;

      
        //用于生成公钥
        private string[] global_g=new string[]{"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJB","L7qpP6mG6ZHdDKEIdTqQDo/WQ","6NaWftXwOTHnnbnwUEX2/2jI4qALxRWMliYI80cszh6","ySbap0KIljDCN","w0CAwEAAQ=="};
        //用于生成公钥
        private string[] global_e = new string[] { "A", "b", "C", "D", "" };

        //用于生成签名原始数据
        private string[] global_h = new string[] { "cD", "BT", "Uzn", "Po", "Luu", "Yhc", "Cj", "FP", "al", "Tq" };
        private string lenovoId = "10204458408";


        public LenovoUtils_Membership(string _lenovoId,string _sessionId, string token)
        {
            if (!string.IsNullOrEmpty(_lenovoId)) {
                lenovoId = _lenovoId;
            }

            //if (!string.IsNullOrEmpty(_sessionId))
            //{
            //    serviceAuthentication = _sessionId;
            //}
            if (!string.IsNullOrEmpty(token))
            {
                accessToken = token;
            }
        }
        public LenovoUtils_Membership(string _lenovoId )
        {
            if (!string.IsNullOrEmpty(_lenovoId))
            {
                lenovoId = _lenovoId;
            }
        }
        public LenovoUtils_Membership()
        {
         
        }
        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <returns></returns>
        private string GetPublicKey()
        {
            var sb = new StringBuilder();
            for (var index=0;index<global_e.Length;index++)
            {
                sb.Append($"{global_g[index]}{global_e[index]}");
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取随机8位数字
        /// 21935551
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<int> GetRandomDigial(int count = 8)
        {
            var ret = new List<int>();
            var rnd = new Random();
            for (var index = 0; index <= count - 1; index++) {
                var nexDouble = rnd.NextDouble();
                var pow=Math.Pow(10, Math.E);
                var value = nexDouble * pow % 10;
                ret.Add((int)Math.Floor(value));
            }
            return ret;
        }
        /// <summary>
        /// 获取随机8位字符串映射后的字符串
        /// 21935551:UznBTTqPoYhcYhcYhcBT
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string  GetRandomStr()
        {
            var sb = new StringBuilder();
            var numList = GetRandomDigial(8);
           // numList = new List<int> { 2, 1, 9, 3, 5, 5, 5, 1 };
            var numStr = string.Join("", numList);//
            var rnd = new Random();
            for (var index = 0; index <= numList.Count - 1; index++)
            {
                sb.Append($"{global_h[numList[index]]}");
            }
            return $"{numStr}:{sb.ToString()}";
        }
        /// <summary>
        /// 获取加密字符串
        /// 21935551:UznBTTqPoYhcYhcYhcBT
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string  GetSignKey()
        {
            var signKey_before = GetRandomStr();
            var publicKey = GetPublicKey();
            var publicKey_fix = "-----BEGIN PUBLIC KEY-----\n";
            publicKey_fix += publicKey;
            publicKey_fix += "\n-----END PUBLIC KEY-----";
            var signKey_after = RSAHelper.RSAEncrypt(signKey_before, publicKey_fix);
            return signKey_after;
        }


        public LenovoUtils_Membership( WebProxy proxy = null)
        {
           
            _proxy = proxy;
        }
        public bool SSOCheck()
        {
            string url = $"https://mmembership.lenovo.com.cn/member-center-api/v2/access/ssoCheck?lenovoId={lenovoId}&unionId=&clientId=2";
            var ret = PostData(url);
            if (ret.Html.Contains("success"))
            {
                var jsonDoc = ret.Html.GetBsonDocFromJson().GetBsonDocument("data");
                var token = jsonDoc.Text("serviceToken");
                settings.AccessToken = token;
                Console.WriteLine($"ssoCheck:{settings.AccessToken}");
                settings.SimulateCookies = ret.Cookie;
                return true;
            }
            else {
                Console.WriteLine($"ssoCheck:登录失败");
                return false;
            }
        }

        public bool CheckIn()
        {
            var url = $"https://mmembership.lenovo.com.cn/member-center/member-center-core/v4/checkin/checkIn?lenovoId={lenovoId}";
            var ret = PostData(url);
            if (ret.Html.Contains("code"))
            {
                //var msg = ret.Html.GetBsonDocFromJson().Text("msg");
                Console.WriteLine(ret.Html);
                return true;
            }
            return false;
        }
        public List<BsonDocument> QueryTaskList()
        {
            var taskIds_filter = new string[] {"25", "26","39"};
           var url=$"https://mmembership.lenovo.com.cn/member-center/member-center-core/v1/task/queryTaskList?lenovoId={lenovoId}";
            var ret = PostData(url);
            if (ret.StatusCode == HttpStatusCode.OK)
            {
                return ret.Html.GetBsonDocFromJson().GetBsonDocumentList("data").Where(c=>c.Text("prizeState") =="0").ToList();
            }
            return new List<BsonDocument>();
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool FinishUserTask(string taskId)
        { 
            var url= $"https://mmembership.lenovo.com.cn/member-center/member-center-core/v1/task/finishUserTask?taskId={taskId}&lenovoId={lenovoId}";
            var ret = PostData(url);
            if (ret.Html.Contains("code"))
            {
                //var msg = ret.Html.GetBsonDocFromJson().Text("msg");
                Console.WriteLine(ret.Html);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool receivePrize(string taskId)
        {
            var url = $"https://mmembership.lenovo.com.cn//member-center/member-center-core/v1/task/receivePrize?lenovoId={lenovoId}&taskId={taskId}";
            var ret = PostData(url);
            if (ret.Html.Contains("code"))
            {
                //var msg = ret.Html.GetBsonDocFromJson().Text("msg");
                Console.WriteLine(ret.Html);
                return true;
            }
            return false;
        }
        public HttpResult PostData(string url)
        {
            var HeadSetDic = new Dictionary<string, string>();
            //HeadSetDic.Add("Host", "mmembership.lenovo.com.cn");

            HeadSetDic.Add("accesstoken", accessToken);
            HeadSetDic.Add("signkey", GetSignKey());
            HeadSetDic.Add("servicetoken",!string.IsNullOrEmpty( settings.AccessToken) ? settings.AccessToken: serviceToken);
            HeadSetDic.Add("sec-ch-ua-mobile", "?0");

            HeadSetDic.Add("tenantid", "25");
            HeadSetDic.Add("service-authentication", serviceAuthentication);
            HeadSetDic.Add("lenovoid", lenovoId);
            HeadSetDic.Add("clientid", "2");
            HeadSetDic.Add("Origin", "https");
            HeadSetDic.Add("Sec-Fetch-Site", "same-origin");
            HeadSetDic.Add("Sec-Fetch-Mode", "cors");
            HeadSetDic.Add("Sec-Fetch-Dest", "empty");

            HeadSetDic.Add("Accept-Encoding", "gzip, deflate, br");
            HeadSetDic.Add("Accept-Language", "en,zh-CN;q=0.9,zh;q=0.8,en-AU;q=0.7");
            // HeadSetDic.Add("Cookie", "leid=1.eJfpp2xqGDk; LA_F_T_10000001=1622454306629; LA_C_Id=_ck21053117450616316148190146972; LA_M_W_10000001=_ck21053117450616316148190146972%7C10000001%7C%7C%7C; LA_C_C_Id=_sk202105311745030.77073600.4178; _ga=GA1.3.175278496.1622454309; OUTFOX_SEARCH_USER_ID_NCOO=12481159.564909633; s_cc=true; s_nr=1622518737830; s_sq=%5B%5BB%5D%5D; LA_F_T_10000071=1622518737843; LA_V_T_N_S_10000071=1622518737843; LA_V_T_N_10000071=1622518737843; LA_R_T_10000071=1622518737843; LA_M_10000071=%3A%3Amarket_type_free_search%3A%3A%3A%3Abaidu%3A%3A%3A%3A%3A%3A%3A%3Awww.baidu.com%3A%3A%3A%3Apmf_from_free_search; LA_M_T_10000071=pmf_from_free_search; LA_M_W_10000071=_ck21053117450616316148190146972%7C10000071%7C%7C%7C; pt_63f31cff=uid=d1VdgU9/tDtndhQS0gfo2A&nid=1&vid=ZKWMuKxLYshd8ttmKG4ARA&vn=1&pvn=1&sact=1622518738193&to_flag=0&pl=56gdVo-onhe1KOi3gymE7A*pt*1622518738193; pt_s_63f31cff=vt=1622518738193&cad=; LA_F_T_10000008=1623295456620; LA_M_10000008=%3A%3Amarket_type_free_search%3A%3A%3A%3Abaidu%3A%3A%3A%3A%3A%3A%3A%3Awww.baidu.com%3A%3A%3A%3Apmf_from_free_search; LA_M_T_10000008=pmf_from_free_search; LA_F_T_10000231=1623310440586; LA_M_W_10000231=_ck21053117450616316148190146972%7C10000231%7C%7C%7C; LA_V_T_N_10000001=1623310473325; LA_V_T_N_S_10000001=1623310473325; LA_R_C_10000008=1; LA_R_T_10000008=1623727977642; LA_V_T_N_S_10000008=1623727977642; LA_V_T_N_10000008=1623727977642; LA_M_W_10000008=_ck21053117450616316148190146972%7C10000008%7C%7Cpc_1144%7Cpc_pc; UM_distinctid=17ae619e8f191a-079ddd50515c0c-6373264-13c680-17ae619e8f2b09; LA_F_T_10000152=1627362963209; LA_R_T_10000152=1627362963209; LA_V_T_10000152=1627362963209; LA_M_W_10000152=_ck21053117450616316148190146972%7C10000152%7C%7Cpc_%7Cpc_child; LA_M_10000001=in-push%3A%3Alxsj%3A%3Az00021891t008%3A%3A%3A%3A%3A%3A%3A%3Awww.lenovo.com.cn%3A%3A%3A%3Apmf_from_adv; LA_M_T_10000001=pmf_from_adv; LA_R_C_10000231=4; LA_R_T_10000231=1631777243144; LA_M_T_10000231=pmf_from_adv; LA_M_10000231=in-push%3A%3Ahtldy1%3A%3Az00022414t001%3A%3A%3A%3A%3A%3A%3A%3Acn.bing.com%3A%3A%3A%3Apmf_from_adv; LA_R_C_10000001=8; LA_R_T_10000001=1631777261696; LA_V_T_10000001=1631777261696; qrtoken=l1984-00b6a360-5e00-4e5d-afd1-0762995cd96e; _gid=GA1.3.1245256041.1631777262; cerpreg-passport=\"|2|1631777339|1634369339|bGVub3ZvSWQ6MTE6MTAyMDQ0NTg0MDh8bG9naW5OYW1lOjExOjE1OTU5MjY2ODIzfG1lbWJlcklkOjEwOjE2MzQzNjkzMzl8Z3JvdXBDb2RlOjE6MXxpc0xlbm92bzoxOjA=|jYTkK2olI2rIt2NvXlUVi46y64yXy3KiDmnf3tGLDYKtNX//3f238tewPycCFzVOxqdH4MtgHVWGS37bV2HovuIMxSE9xiv1t9qCYtrzA68QVjVEYWgZhmTk0kx9VeIuHA9NzXkM4ATFRLwLYzjOqh0U6JegwaMkyeWJf1dQWiLmUvilQLelt4KL6MzO6KTtvH2YQDwltHpWyL7Rm2iIuL+U+cTHFKKbKpIJQg/X2zBa4nFJ/X3FRL/K2PCkX/b8I8jqAAONmYJujJXEsalV+ssX424SsqIghipACPNFxpwe6v5e8a2Fqto7M0kwerwQb7eDAe3mkhO0cOqd2JufZA==|\"; ar=2; lenovoId=10204458408; serviceToken=Bearer%20eyJhbGciOiJIUzI1NiJ9.eyJlbmNyeU1zZyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlzcyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlhdCI6MTYzMTY5MTA2MCwianRpIjoiNGMwYThiNDljMTY2NGZhMWE1MTNkM2RjMDNhMzllMGEifQ.8KDrmgFyOmNoUsoXunoPm5N8pCt0ZvEqoMcuj-weJdo; LA_V_T_10000231=1631781599191");

           var ret = UrlGet(url,headers: HeadSetDic, postData: "{}", config: option =>
            {
                // option.ContentType = "application/json";
                option.Accept = "*/*";
               // option.Cookie = "leid=1.eJfpp2xqGDk; LA_F_T_10000001=1622454306629; LA_C_Id=_ck21053117450616316148190146972; LA_M_W_10000001=_ck21053117450616316148190146972%7C10000001%7C%7C%7C; LA_C_C_Id=_sk202105311745030.77073600.4178; _ga=GA1.3.175278496.1622454309; OUTFOX_SEARCH_USER_ID_NCOO=12481159.564909633; s_cc=true; s_nr=1622518737830; s_sq=%5B%5BB%5D%5D; LA_F_T_10000071=1622518737843; LA_V_T_N_S_10000071=1622518737843; LA_V_T_N_10000071=1622518737843; LA_R_T_10000071=1622518737843; LA_M_10000071=%3A%3Amarket_type_free_search%3A%3A%3A%3Abaidu%3A%3A%3A%3A%3A%3A%3A%3Awww.baidu.com%3A%3A%3A%3Apmf_from_free_search; LA_M_T_10000071=pmf_from_free_search; LA_M_W_10000071=_ck21053117450616316148190146972%7C10000071%7C%7C%7C; pt_63f31cff=uid=d1VdgU9/tDtndhQS0gfo2A&nid=1&vid=ZKWMuKxLYshd8ttmKG4ARA&vn=1&pvn=1&sact=1622518738193&to_flag=0&pl=56gdVo-onhe1KOi3gymE7A*pt*1622518738193; pt_s_63f31cff=vt=1622518738193&cad=; LA_F_T_10000008=1623295456620; LA_M_10000008=%3A%3Amarket_type_free_search%3A%3A%3A%3Abaidu%3A%3A%3A%3A%3A%3A%3A%3Awww.baidu.com%3A%3A%3A%3Apmf_from_free_search; LA_M_T_10000008=pmf_from_free_search; LA_F_T_10000231=1623310440586; LA_M_W_10000231=_ck21053117450616316148190146972%7C10000231%7C%7C%7C; LA_V_T_N_10000001=1623310473325; LA_V_T_N_S_10000001=1623310473325; LA_R_C_10000008=1; LA_R_T_10000008=1623727977642; LA_V_T_N_S_10000008=1623727977642; LA_V_T_N_10000008=1623727977642; LA_M_W_10000008=_ck21053117450616316148190146972%7C10000008%7C%7Cpc_1144%7Cpc_pc; UM_distinctid=17ae619e8f191a-079ddd50515c0c-6373264-13c680-17ae619e8f2b09; LA_F_T_10000152=1627362963209; LA_R_T_10000152=1627362963209; LA_V_T_10000152=1627362963209; LA_M_W_10000152=_ck21053117450616316148190146972%7C10000152%7C%7Cpc_%7Cpc_child; LA_M_10000001=in-push%3A%3Alxsj%3A%3Az00021891t008%3A%3A%3A%3A%3A%3A%3A%3Awww.lenovo.com.cn%3A%3A%3A%3Apmf_from_adv; LA_M_T_10000001=pmf_from_adv; LA_R_C_10000231=4; LA_R_T_10000231=1631777243144; LA_M_T_10000231=pmf_from_adv; LA_M_10000231=in-push%3A%3Ahtldy1%3A%3Az00022414t001%3A%3A%3A%3A%3A%3A%3A%3Acn.bing.com%3A%3A%3A%3Apmf_from_adv; LA_R_C_10000001=8; LA_R_T_10000001=1631777261696; LA_V_T_10000001=1631777261696; qrtoken=l1984-00b6a360-5e00-4e5d-afd1-0762995cd96e; _gid=GA1.3.1245256041.1631777262; cerpreg-passport=\"|2|1631777339|1634369339|bGVub3ZvSWQ6MTE6MTAyMDQ0NTg0MDh8bG9naW5OYW1lOjExOjE1OTU5MjY2ODIzfG1lbWJlcklkOjEwOjE2MzQzNjkzMzl8Z3JvdXBDb2RlOjE6MXxpc0xlbm92bzoxOjA=|jYTkK2olI2rIt2NvXlUVi46y64yXy3KiDmnf3tGLDYKtNX//3f238tewPycCFzVOxqdH4MtgHVWGS37bV2HovuIMxSE9xiv1t9qCYtrzA68QVjVEYWgZhmTk0kx9VeIuHA9NzXkM4ATFRLwLYzjOqh0U6JegwaMkyeWJf1dQWiLmUvilQLelt4KL6MzO6KTtvH2YQDwltHpWyL7Rm2iIuL+U+cTHFKKbKpIJQg/X2zBa4nFJ/X3FRL/K2PCkX/b8I8jqAAONmYJujJXEsalV+ssX424SsqIghipACPNFxpwe6v5e8a2Fqto7M0kwerwQb7eDAe3mkhO0cOqd2JufZA==|\"; ar=2; lenovoId=10204458408; serviceToken=Bearer%20eyJhbGciOiJIUzI1NiJ9.eyJlbmNyeU1zZyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlzcyI6IkVMa0s5REZsOXdPY0lGRXhnVCt6QUE9PSIsImlhdCI6MTYzMTY5MTA2MCwianRpIjoiNGMwYThiNDljMTY2NGZhMWE1MTNkM2RjMDNhMzllMGEifQ.8KDrmgFyOmNoUsoXunoPm5N8pCt0ZvEqoMcuj-weJdo; LA_V_T_10000231=1631781599191";
                //  option.Accept = "*/*";
                option.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36";
                option.Referer = "https";
            });
            return ret;
        }

        public   HttpResult UrlGet( string url, Dictionary<string, string> headers = null, string postData = "", WebProxy proxy = null, Action<HttpItem> config = null)
        {

            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项    
                Method = "get",//URL     可选项 默认为Get   
                Allowautoredirect = true,
                KeepAlive = true,

            };

            //设定协议
            if (proxy != null)
            {
                item.WebProxy = proxy;
            }
            if (!string.IsNullOrEmpty(postData))
            {
                item.Postdata = postData;
                item.Method = "post";
                if (postData.Contains("{"))
                {
                    item.PostEncoding = Encoding.UTF8;
                }

            }
            if (config != null)
            {
                config?.Invoke(item);
            }
            if (headers != null)
            {
                foreach (var key in headers.Keys)
                {
                    var value = headers[key].Trim();

                    switch (key)
                    {
                        case "Accept":
                            item.Accept = value;
                            break;
                        case "ContentType":
                            item.ContentType = value;
                            break;
                        case "UserAgent":
                            item.UserAgent = value;
                            break;
                        case "Refer":
                            item.Referer = value;
                            break;
                        case "Cookie":
                            item.Cookie = value;
                            break;

                        default:
                            item.Header.Add(key, value);
                            break;
                    }

                }

            }

            HttpHelper http = new HttpHelper();
            //请求的返回值对象
            HttpResult result = http.GetHtml(item);
            return result;
        }

    }
     
}