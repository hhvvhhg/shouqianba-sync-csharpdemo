using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows.Documents;
using System.Security.Cryptography;

namespace shouqianba_sync_csharpdemo.demo
{
    class sync
    {
        public static string api_domain = "https://api.shouqianba.com";
        public static JObject syncdemo(string terminal_sn, string terminal_key)
        {
            string url = api_domain + "/api/sync/order";

            Dictionary<string, object> item = new Dictionary<string, object>();
            item.Add("item_no", "");  //商品税收编码
            item.Add("item_name", ""); //发票项目名称（或商品名称）
            item.Add("quantity", "");  //数量, 默认为1。
            item.Add("row_type", "");  //发票行性质
            item.Add("specification", "");  //规格型号,可选
            item.Add("unit", "");  //单位
            item.Add("amount", "");  //价税合计金额，单位为分
            item.Add("tax_rate", "");   //税率
            item.Add("zero_rate_flag", "");  //0税率标识
            List<object> items = new List<object>();
            items.Add(item);


            JObject Jparams = new JObject();

            Jparams.Add(new JProperty("terminal_sn", terminal_sn)); //终端号
            Jparams.Add(new JProperty("client_sn", ""));   //商户系统订单号
            Jparams.Add(new JProperty("client_task_sn", ""));  //商户系统订单流水号
            Jparams.Add(new JProperty("client_time", 123));   //商户系统订单完成时间
            Jparams.Add(new JProperty("order_type", ""));  //订单类型
            Jparams.Add(new JProperty("client_original_sn", ""));  //原始订单号
            Jparams.Add(new JProperty("amount", ""));   //订单金额
            Jparams.Add(new JProperty("invoice_items", items));  //开票商品明细信息
            string sign = getSign(Jparams.ToString() + terminal_key);
            string result = httpUtil.httpPost(url, Jparams.ToString(), sign, terminal_sn);
            JObject retObj = JObject.Parse(result);

            string resCode = retObj["result_code"].ToString();
            Console.WriteLine("返回码：" + resCode);
            if (resCode.Equals("200"))
            {

                string responseStr = retObj["biz_response"].ToString();
                JObject terminal = JObject.Parse(responseStr);
                Console.WriteLine("返回信息:" + terminal);
                return terminal;
            }
            else
            {
                string errorMsg = retObj["error_message"].ToString();
                Console.WriteLine("返回信息：" + errorMsg);
            }
            return null;
        }


        public static string urlCode()
        {
            string vendorSn = "28910391282321983";
            string vendorKey = "7f3804007bf8408293b8ebb8157fc0fb";
            string wx_service_baseurl = "https://m.testalpha.wosai.com/wxservice/check.do";
            string appid = "2017891823646";
            string redirect_user_uri = "https://einvoice.testalpha.shouqianba.com/xxxx/xxxx/h5";
            string baseStateParameter = "p=1,100|ts=10000058909032923|cs=201709280028392|ct=1506484936867|ta=10000|bc=0";
            string sign = getSign(baseStateParameter + vendorKey);
            string a = vendorSn + " " + sign;
            string state = baseStateParameter + "|" + "a=" + a;
            string encodedState = Uri.EscapeDataString(state);
            string redirect = Uri.EscapeDataString(redirect_user_uri);
            string qrcodeUrl = wx_service_baseurl + "?appid=" + appid + "&redirect_user_uri=" + redirect + "&state=" + encodedState + "&e=y";

            return qrcodeUrl;
        }


        private static string getSign(string signStr)
        {
            string md5 = MD5(signStr);
            return md5;
        }
        public static string MD5(string value)
        {
            StringBuilder result = new StringBuilder("");
            string cl1 = value;
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            byte[] s = md5.ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(cl1));
            for (int i = 0; i < s.Length; i++)
            {
                result.AppendFormat("{0:x2}", s[i]);
            }
            return result.ToString();
        }
    }
}
