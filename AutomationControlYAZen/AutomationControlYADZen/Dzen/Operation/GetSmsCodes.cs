using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AutomationControlYADZen.Dzen.BasicOperation;

    public static partial class GetSmsCodee
    {
        private static ResponseGetSmsCode _responseGetSmsCode;
        private static ResponseGetPhoneNomber _responseGetPhoneNomber;
        public static async Task<ResponseGetPhoneNomber> GetTelephoneNomber(string Host, string ApiGetPhoneNomber)
        {
            var addressGetPhoneNomber = new Uri(Host + ApiGetPhoneNomber);
            var client = new HttpClient() { BaseAddress = addressGetPhoneNomber } ;
            var response = await client.GetAsync(addressGetPhoneNomber, new CancellationToken());
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Assert.Fail($"{ApiGetPhoneNomber} отработала некорректно, дальнейшие действия бессмысленны!");
            }
            _responseGetPhoneNomber = JsonConvert.DeserializeObject<ResponseGetPhoneNomber>(stringResponse);
            return _responseGetPhoneNomber;
        }
        

        public static async Task<string> GetSmsCode(string Host, string ApiKey, string idNum)
        {
            string apiGetSmsCode = $"/getSmsCode/?apiKey={ApiKey}&idNum={idNum}";
            var addressGetSmsCode = new Uri(Host + apiGetSmsCode);
            var clientSms = new HttpClient() { BaseAddress= addressGetSmsCode};
            var responseSms = await clientSms.GetAsync(addressGetSmsCode, new CancellationToken());
            
            var stringResponseSms = await responseSms.Content.ReadAsStringAsync();
            if (responseSms.StatusCode != HttpStatusCode.OK)
            {
                Assert.Fail($"{apiGetSmsCode} отработала некорректно, дальнейшие действия бессмысленны!");
            }
            _responseGetSmsCode = JsonConvert.DeserializeObject<ResponseGetSmsCode>(stringResponseSms);
            return _responseGetSmsCode.SmsCode;
        }

        public static async Task TelNomberStatusEnd(string Host, string ApiKey, string idNum)
        {
            string apiTelStatusGetNewSms = $"/setStatus/?apiKey={ApiKey}&status=end&idNum={idNum}";
            var addressTelStatusGetNewSms = new Uri(Host + apiTelStatusGetNewSms);
            var telStatusGetNewSms = new HttpClient() { BaseAddress = addressTelStatusGetNewSms } ;
            var responseTelStatusGetNewSms = await telStatusGetNewSms.GetAsync(addressTelStatusGetNewSms, new CancellationToken());
            var stringResponseStatus = await responseTelStatusGetNewSms.Content.ReadAsStringAsync();
            if (responseTelStatusGetNewSms.StatusCode != HttpStatusCode.OK)
            {
                Assert.Fail($"{apiTelStatusGetNewSms} отработала некорректно, дальнейшие действия бессмысленны!");
            }
            Thread.Sleep(1000);
        }

        public static async Task TelNomberStatusSend(string Host, string ApiKey, string idNum)
        {
            string apiTelStatusGetNewSms = $"/setStatus/?apiKey={ApiKey}&status=send&idNum={idNum}";
            var addressTelStatusGetNewSms = new Uri(Host + apiTelStatusGetNewSms);
            var telStatusGetNewSms = new HttpClient() { BaseAddress = addressTelStatusGetNewSms } ;
            var responseTelStatusGetNewSms = await telStatusGetNewSms.GetAsync(addressTelStatusGetNewSms, new CancellationToken());
            var stringResponseStatus = await responseTelStatusGetNewSms.Content.ReadAsStringAsync();
            if (responseTelStatusGetNewSms.StatusCode != HttpStatusCode.OK)
            {
                Assert.Fail($"{apiTelStatusGetNewSms} отработала некорректно, дальнейшие действия бессмысленны!");
            }
        }
    }