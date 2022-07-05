using MailguncApp;
using RestSharp;
using RestSharp.Authenticators;
using System;

namespace EmailConsoleApp
{
    static class EmailService
    {

        private const string APIKey = "f970ef1416aa897e007aa1d53361442b-50f43e91-32c9645e";
        private const string BaseUri = "https://api.mailgun.net/v3";
        private const string Domain = "sandbox03623db8e1a942c59ec4940d9146eb99.mailgun.org";
        private const string SenderAddress = "Support <warrantysupport@mss.ba>";
        private const string SenderDisplayName = "Sender Name";
        private const string Tag = "sampleTag";

        public static IRestResponse SendEmail(UserEmailOptions userEmailOptions)
        {
            Console.WriteLine("Entering SentEmail method.");


            Console.WriteLine("Setting an authenticator & baseURI");
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(BaseUri),
                Authenticator = new HttpBasicAuthenticator("api", APIKey),
            };


            Console.WriteLine("Preparing a request with parameters (resource, domain, from, to etc.)");


            RestRequest request = new RestRequest();
            request.AddParameter("domain", Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", SenderAddress);
            foreach (var toEmail in userEmailOptions.ToEmails)
            {
                request.AddParameter("to", toEmail);
            }

            request.AddParameter("subject", userEmailOptions.Subject);
            request.AddParameter("text", userEmailOptions.Body);
            //request.AddParameter("o:tag", Tag);
            request.Method = Method.POST;
            return client.Execute(request);
        }

    }
}