using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public class PostData
    {
        public string num { get; set; }
    }
    public static class Function1
    {
        static public Dictionary<int, string> mapDigits = new Dictionary<int, string>()
        {
            { 1, " One" },
            { 2, " Two" },
            { 3, " Three" },
            { 4, " Four" },
            { 5, " Five" },
            { 6, " Six" },
            { 7, " Seven" },
            { 8, " Eight" },
            { 9, " Nine" },
            { 10, " Ten" },
            { 11, " Eleven" },
            { 12, " Twelve" },
            { 13, " Thirteen" },
            { 14, " Fourteen" },
            { 15, " Fifteen" },
            { 16, " Sixteen" },
            { 17, " Seventeen" },
            { 18, " Eighteen" },
            { 19, " Nineteen" },
        };

            static public Dictionary<int, string> mapTens = new Dictionary<int, string>()
        {
            { 2, " Twenty" },
            { 3, " Thirty" },
            { 4, " Forty" },
            { 5, " Fifty" },
            { 6, " Sixty" },
            { 7, " Seventy" },
            { 8, " Eighty" },
            { 9, " Ninety" },
        };

            static public Dictionary<int, string> mapK = new Dictionary<int, string>()
        {
            { 1, "" },
            { 2, " Thousand" },
            { 3, " Million" },
            { 4, " Billion" },
            { 5, " Trillion" },
            { 6, " Quadrillion" },
            { 7, " Quintillion" },
            { 8, " Sextillion" },
            { 9, " Septillion" },
            { 10, " Octillion" },
            { 11, " Nonillion" },
            { 12, " Decillion" },
            { 13, " Undecillion" },
            { 14, " Duodecillion" },
            { 15, " Tredecillion" },
            { 16, " Quattuordecillion" },
            { 17, " Quindecillion" },
            { 18, " Sexdecillion" },
            { 19, " Septendecillion" },
            { 20, " Octodecillion" },
            { 21, " Novemdecillion" },
            { 22, " Vigintillion" },
        };


        static public string NumberToWords(string snum)
        {
            int len = snum.Length;
            if (len == 0) return "EMPTY";
            if(len == 1)
            {
                int val = int.Parse(snum);
                if (val == 0) return "Zero";
                else return mapDigits[val];
            }

            int k = 0;
            Stack<int> stack = new Stack<int>();
            string res = string.Empty;
            while (snum.Length > 0)
            {
                string laststr;
                if (snum.Length <= 3)
                {
                    laststr = snum;
                    snum = "";
                } else
                {
                    laststr = snum.Substring(snum.Length - 3);
                    snum = snum.Substring(0, snum.Length - 3);
                }
                     
                stack.Push(int.Parse(laststr));
                ++k;
            }

            while (stack.Count > 0)
            {
                int currTriple = stack.Pop();
                string currTripleStr = ConvertTripleToStr(currTriple);
                res += currTripleStr + (String.IsNullOrEmpty(currTripleStr) ? "" : mapK[k]);
                --k;
            }

            return res.Trim();
        }

        static public string ConvertTripleToStr(int n)
        {
            string res = string.Empty;

            if (n >= 100)
            {
                res += mapDigits[n / 100] + " Hundred";
                n %= 100;
            }

            if (n >= 20)
            {
                res += mapTens[n / 10];
                n %= 10;
            }

            if (n > 0)
            {
                res += mapDigits[n];
            }

            return res;
        }
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "num/{num}")]HttpRequestMessage req, String num, TraceWriter log)
        {
            try
            {
                // parse query parameter
                //string num = req.GetQueryNameValuePairs()
                //    .FirstOrDefault(q => string.Compare(q.Key, "num", true) == 0)
                //    .Value;

                //if (num == null)
                //{
                //    // Get request body
                //    dynamic data = await req.Content.ReadAsAsync<PostData>();
                //    num = data?.num;
                //}
                if (num == null)
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a num on the query string or in the request body");
                else
                {
                    var myObj = new { val = FunctionApp1.Function1.NumberToWords(num) };
                    var jsonToReturn = JsonConvert.SerializeObject(myObj);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(jsonToReturn, System.Text.Encoding.UTF8, "application/json")
                    };
                }
            } catch(OverflowException ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a num which is of maximum 64 bit");
            } catch(Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a num which less than 10 power 66");
            }

        }
    }
}
