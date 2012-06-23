using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using OAuth;
using System.Text;
using RestSharp;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DouMi.OAuth
{
    public class DoubanOAuth
    {
        string apiKey = "0669e688d846e260133db39a88d4a720";
        string apiKeySecret = "0495ef4ab7cfc88c";
        string requestToken = "";
        string requestTokenSecret = "";
        string accessToken = "";
        string accessTokenSecret = "";
        string userId = "";
        private IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;

        static string doubanWebBaseUrl = "http://www.douban.com";
        static string doubanApiBaseUrl = "http://api.douban.com";
        static string requestTokenResource = "/service/auth/request_token";
        static string accessTokenResource = "/service/auth/access_token";
        static string sayingResource = "/miniblog/saying";
        public static string authorizationUri = "http://www.douban.com/service/auth/authorize?oauth_token=";

        Uri requestTokenUri = new Uri(doubanWebBaseUrl + requestTokenResource);
        Uri accessTokenUri = new Uri(doubanWebBaseUrl + accessTokenResource);
        Uri miniblogUri = new Uri(doubanApiBaseUrl + sayingResource);
        
        OAuthBase oAuth = new OAuthBase();

        public bool isAuthed = false;

        public DoubanOAuth()
        {
            if (userSettings.Contains("accessToken") && userSettings.Contains("accessTokenSecret"))
            {
                accessToken = ReadFromIsolatedStorage("accessToken");
                accessTokenSecret = ReadFromIsolatedStorage("accessTokenSecret");
                userId = ReadFromIsolatedStorage("doubanUserId");
                isAuthed = true;
            }
        }

        public void SaveToIsolatedStorage(string key, string value)
        {
            if (userSettings.Contains(key))
            {
                userSettings[key] = value;
            }
            userSettings.Add(key, value);
        }

        public string ReadFromIsolatedStorage(string key)
        {
            string value;
            if (userSettings.TryGetValue<string>(key, out value))
            {
                return value;
            }
            else
            {
                return "";
            }
        }

        public void DeleteIsolatedStorage(string key)
        {
            userSettings.Remove(key);
        }

        public void ResetAuth()
        {
            DeleteIsolatedStorage("accessToken");
            DeleteIsolatedStorage("accessTokenSecret");
            DeleteIsolatedStorage("username");
            DeleteIsolatedStorage("iconurl");
            DeleteIsolatedStorage("doubanUserId");
            isAuthed = false;
        }

        private Dictionary<string, string> parseResponse(string parameters)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                    if (!string.IsNullOrEmpty(s))
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(temp[0], temp[1]);
                        }
                        else result.Add(s, string.Empty);
            }

            return result;
        }

        public void GetRequestToken(Action<string, string> onSucess, Action onFail)
        {

            Uri uri = requestTokenUri;
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizeUrl, normalizedRequestParameters;

            // 签名
            string sig = oAuth.GenerateSignature(
                uri,
                apiKey,
                apiKeySecret,
                string.Empty,
                string.Empty,
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);
            //sig = HttpUtility.UrlEncode(sig);

            var client = new RestClient(doubanWebBaseUrl);
            var request = new RestRequest(requestTokenResource, Method.GET);
            request.AddParameter("oauth_consumer_key", apiKey);
            request.AddParameter("oauth_nonce", nonce);
            request.AddParameter("oauth_timestamp", timeStamp);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_version", "1.0");
            request.AddParameter("oauth_signature", sig);

            client.ExecuteAsync(request, (response) =>
            {
                if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode != HttpStatusCode.NotFound)
                {
                    Dictionary<string, string> responseValues = parseResponse(response.Content);
                    if (responseValues.ContainsKey("oauth_token") && responseValues.ContainsKey("oauth_token_secret"))
                    {
                        requestToken = responseValues["oauth_token"];
                        requestTokenSecret = responseValues["oauth_token_secret"];
                        onSucess(requestToken, requestTokenSecret);
                    }
                    else
                    {
                        onFail();
                    }
                }
                else
                {
                    onFail();
                }
            });
        }

        public void GetAccessToken(Action<string, string> onSucess, Action onFail)
        {
            Uri uri = accessTokenUri;
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizeUrl, normalizedRequestParameters;

            // 签名
            string sig = oAuth.GenerateSignature(
                uri,
                apiKey,
                apiKeySecret,
                requestToken,
                requestTokenSecret,
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);
            //sig = HttpUtility.UrlEncode(sig);


            var client = new RestClient(doubanWebBaseUrl);
            var request = new RestRequest("/service/auth/access_token", Method.GET);
            request.AddParameter("oauth_consumer_key", apiKey);
            request.AddParameter("oauth_nonce", nonce);
            request.AddParameter("oauth_timestamp", timeStamp);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_version", "1.0");
            request.AddParameter("oauth_signature", sig);
            request.AddParameter("oauth_token", requestToken);

            client.ExecuteAsync(request, (response) =>
            {
                if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode != HttpStatusCode.NotFound)
                {
                    Dictionary<string, string> responseValues = parseResponse(response.Content);
                    if (responseValues.ContainsKey("oauth_token") && responseValues.ContainsKey("oauth_token_secret"))
                    {
                        accessToken = responseValues["oauth_token"];
                        accessTokenSecret = responseValues["oauth_token_secret"];
                        userId = responseValues["douban_user_id"];
                        SaveToIsolatedStorage("accessToken", accessToken);
                        SaveToIsolatedStorage("accessTokenSecret", accessTokenSecret);
                        SaveToIsolatedStorage("doubanUserId", userId);
                        onSucess(accessToken, accessTokenSecret);
                        isAuthed = true;
                    }
                    else
                    {
                        onFail();
                    }
                }
                else
                {
                    onFail();
                }
            });
        }

        public void GetAuthUserInfo(Action<string, string> onSucess)
        {
            var client = new RestClient(doubanApiBaseUrl);

            var request = new RestRequest("/people/" + userId, Method.GET);

            OAuthHeader header = new OAuthHeader(accessToken, accessTokenSecret, doubanApiBaseUrl + "/people/" + userId + "?alt=json", "GET");
            request.AddHeader("Authorization", header.getHeader());
            request.AddParameter("alt", "json");
            client.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ResetAuth();
                }
                if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK)
                {
                    JObject json = JObject.Parse(response.Content);
                    try
                    {
                        string username = (string)json["title"]["$t"];
                        string iconurl = "";
                        JArray links = (JArray)json["link"];
                        foreach (JToken token in links.Children())
                        {
                            string rel = (string)token["@rel"];
                            if (rel == "icon")
                            {
                                iconurl = (string)token["@href"];
                            }
                        }
                        if (!iconurl.Contains("ul"))
                        {
                            iconurl = iconurl.Replace("/u", "/ul");
                        }

                        onSucess(username, iconurl);
                        SaveToIsolatedStorage("username", username);
                        SaveToIsolatedStorage("iconurl", iconurl);
                    }
                    catch (Exception eee)
                    { 
                    }
                    
                }
            });
        }

        public void sendMiniBlog(string content)
        {
            var client = new RestClient(doubanApiBaseUrl);

            var request = new RestRequest(sayingResource, Method.POST);
            request.RequestFormat = DataFormat.Xml;
            request.AddHeader("Content-Type", "application/atom+xml");

            OAuthHeader header = new OAuthHeader(accessToken, accessTokenSecret, doubanApiBaseUrl + sayingResource, "POST");
            request.AddHeader("Authorization", header.getHeader());

            StringBuilder requestBody = new StringBuilder("<?xml version='1.0' encoding='UTF-8'?>");
            requestBody.Append("<entry xmlns:ns0=\"http://www.w3.org/2005/Atom\" xmlns:db=\"http://www.douban.com/xmlns/\">");
            requestBody.Append("<content>" + content + "</content>");
            //requestBody.Append("<content>hello world from doumi :)</content>");
            requestBody.Append("</entry>");
            request.AddParameter("application/atom+xml", requestBody.ToString(), ParameterType.RequestBody);

            client.ExecuteAsync(request, (response) =>
            {
                if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode != HttpStatusCode.NotFound)
                {
                }
            });
        }


    }
}
