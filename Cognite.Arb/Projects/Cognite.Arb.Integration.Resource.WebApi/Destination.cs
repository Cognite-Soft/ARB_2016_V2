using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Cognite.Arb.Integration.Business;
using Cognite.Arb.Server.Contract;

namespace Cognite.Arb.Integration.Resource.WebApi
{
    public class Destination : Integrator.IDestination
    {
        private const string SecurityTokenHeaderName = "SecurityToken";

        private readonly string _baseAddress;

        public Destination(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public void CreateCase(Integrator.CaseData caseData)
        {
            using (var client = CreateClient())
            {
                var createCaseInfo = new CreateCaseInfo
                {
                    Id = caseData.Id,
                    CaseManagerEmail = caseData.CaseManagerEmail,
                    ClaimantContact = caseData.ClaimantContact == null
                        ? new CreateCaseInfo.ContactData()
                        : new CreateCaseInfo.ContactData
                        {
                            Name = caseData.ClaimantContact.Name,
                            EMail = caseData.ClaimantContact.EMail,
                            Address = caseData.ClaimantContact.Address,
                            Phone = caseData.ClaimantContact.Phone,
                        },
                };

                var content = CreateHttpContent(createCaseInfo);
                string uri = string.Format("cases");
                var response = client.PostAsync(uri, content).Result;
                if (response.StatusCode == HttpStatusCode.Created) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new Exception("Forbidded");
                throw new Exception();
            }
        }

        private HttpClient CreateClient(string securityToken = "arb1234567890system")
        {
            var client = new HttpClient {BaseAddress = new Uri(_baseAddress)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (securityToken != null)
                client.DefaultRequestHeaders.Add(SecurityTokenHeaderName, securityToken);
            return client;
        }

        private static ObjectContent CreateHttpContent<T>(T value)
        {
            var content = new ObjectContent(typeof (T), value, new JsonMediaTypeFormatter());
            return content;
        }
    }
}