using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;

namespace TripMoment.Web.Controllers
{
    public class GcmNotificationApiController : Controller
    {
        public bool ServerPusth(string message = "推播測試。")
        {
            string API_Key = "AIzaSyAFBFF6ljC6LmAA-uuFjH0pRXjQXIbysNP"; //Google Api Key, 確認後可以改放在web.config

            //將手機的RegId存入List, 格式確認後再從資料庫抓取
            List<string> registrationIDList = new List<string>();
            registrationIDList.Add("APA91bEsKDolLssir0b4W3luSU_uasdasdFFGjVUY95p1cwC6kedM_Q3WwdgZiS8te1Z9gWdFI9hvoqKJSc6MQtX_Cf31vhWt1pxg7_9EPPaeDQ9ktBoPnoSbwyO1WQMJDnQfOuxSOA0CLuouxaekRcrLDF817EMYQ");
            registrationIDList.Add("APA91bF_Sq1V7kW6bwmICymLKlUm4j3-lFhHWLYLqZ38p_xysddGLrVHbagM5m-HE6dXAcl-GMmH8FNE0y-1RTfsv1u9conLRYyRO4WznoPijXy0bWYmr-GE6YLPoWfe8u8gPzWDCQs-3Z4BaXMM6K05DSvDlrC0EQ");
            
            return HttpPostToGCM(registrationIDList, API_Key, message);
        }

		
        private bool HttpPostToGCM(List<string> regIds, string API_Key, string message)
        {
            bool result = true;
            string errorMessage = "";
            
            if (regIds != null && regIds.Count > 0)//防呆
            {
                try {
                    foreach (var regId in regIds)
                    {//一筆一筆發送, 也可以考慮改成一次發送多筆, 最好一次1000筆

                        //準備對GCM Server發出Http post
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
                        request.Method = "POST";
                        request.ContentType = "application/json;charset=utf-8;";
                        request.Headers.Add(string.Format("Authorization: key={0}", API_Key));

                        string RegistrationID = regId.ToString();
                        var postData =
                        new
                        {
                            data = new
                            {
                                Message = message //Message這個tag要讓前端開發人員知道
                            },
                            registration_ids = new string[] { RegistrationID }
                        };

                        string p = JsonConvert.SerializeObject(postData);//使用Json.Net 將Linq to json轉為字串

                        byte[] byteArray = Encoding.UTF8.GetBytes(p);//要發送的字串轉為byte[]
                        request.ContentLength = byteArray.Length;

                        Stream dataStream = request.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();

                        //發出Request
                        WebResponse response = request.GetResponse();
                        Stream responseStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(responseStream);
                        string responseStr = reader.ReadToEnd();
                        reader.Close();
                        responseStream.Close();
                        response.Close();

                    }//End foreach

                }
                catch (Exception ex)
                {
                    result = false;
                    errorMessage += ex.Message+",";
                }

            }//End if

            return result;
        }
    }
}