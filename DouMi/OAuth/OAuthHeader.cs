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
using System.Text;
using RestSharp;
using OAuth;

namespace DouMi.OAuth
{
    public class OAuthHeader
    {
        string apiKey = "0669e688d846e260133db39a88d4a720";
        string apiKeySecret = "0495ef4ab7cfc88c";
        string token = "";
        string tokenSecret = "";
        string uri = "";
        string method = "";
        OAuthBase oAuth = new OAuthBase();

        public OAuthHeader(string token, string tokenSecret, string uri, string method)
        {
            this.token = token;
            this.tokenSecret = tokenSecret;
            this.uri = uri;
            this.method = method;
        }

        public string getHeader()
        {
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizeUrl, normalizedRequestParameters;

            string sig = oAuth.GenerateSignature(
                new Uri(uri),
                apiKey,
                apiKeySecret,
                token,
                tokenSecret,
                method,
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);
            sig = HttpUtility.UrlEncode(sig);

            StringBuilder oauthHeader = new StringBuilder();
            oauthHeader.AppendFormat("OAuth realm=\"\", oauth_consumer_key={0}, ", apiKey);
            oauthHeader.AppendFormat("oauth_nonce={0}, ", nonce);
            oauthHeader.AppendFormat("oauth_timestamp={0}, ", timeStamp);
            oauthHeader.AppendFormat("oauth_signature_method={0}, ", "HMAC-SHA1");
            oauthHeader.AppendFormat("oauth_version={0}, ", "1.0");
            oauthHeader.AppendFormat("oauth_signature={0}, ", sig);
            oauthHeader.AppendFormat("oauth_token={0}", token);

            return oauthHeader.ToString();
        }
    }
}

