//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

namespace CT.AccessToken.Client
{
    #region Usings

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Identity.Client;
    using RestSharp;
    using System.Configuration;
    using System.IO;

    #endregion

    public class AccessTokenClient
    {
        private const int MillisecondsTimeout = 100;

        #region Static Fields

        public static int maxrequestsize = 5000;   //service size is 5000
        public static int maxelements = 100;
        public static string CategoryID { get; set; }
        public static string AppId { get; set; }
        /// <summary>
        /// Application(client) ID value - Create an app at the app registration portal
        /// https://ms.portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade
        /// </summary>
        public static string ClientId { get; set; }
        public static string WorkspaceId { get; set; }
        public static string TenantId { get; set; }
        public static List<string> Scopes { get; set; } = new List<string>() { "api://72876cf4-6a8f-4e0f-b161-b34c56f0b509/access_as_user" };
        private static RestClient restClient { get; set; }


        /// <summary>
        /// Holds the setting whether to use a container offline
        /// </summary>
        public static bool UseCustomEndpoint { get; set; }
        /// <summary>
        /// Holds the value of the custom endpoint, the container
        /// </summary>
        public static string CustomEndpointUrl { get; set; }

        /// <summary>
        /// Holds the Azure subscription key
        /// </summary>
        public static string AzureKey { get; set; }

        /// <summary>
        /// End point address for V1 of Swagger UI API and Oauth V2 endpoint
        /// </summary>
        public static string EndPointAddressV1Prod { get; set; } = "https://custom-api.cognitive.microsofttranslator.com";
        public static string EndPointOauthV2 { get; set; }
        public static int Maxrequestsize { get => maxrequestsize; }
        public static int Maxelements { get => maxelements; }

        public enum ContentType { plain, HTML };
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Loads credentials from settings file.
        /// Doesn't need to be public, because it is called during Initialize();
        /// </summary>
        public void LoadCredentials()
        {
            restClient = new RestClient(EndPointAddressV1Prod);

            ClientId = ConfigurationManager.AppSettings["clientId"];
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                Console.WriteLine("Please supply a clientId. See Readme.txt.");
                return;
            }
            WorkspaceId = ConfigurationManager.AppSettings["workspaceId"];
            if (string.IsNullOrWhiteSpace(WorkspaceId))
            {
                Console.WriteLine("Please supply a clientId. See Readme.txt.");
                return;
            }
            TenantId = ConfigurationManager.AppSettings["tenantId"];
            if (string.IsNullOrWhiteSpace(TenantId))
            {
                Console.WriteLine("Please supply a tenantId.");
                return;
            }
            EndPointOauthV2 = $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0";
        }

        /// <summary>
        /// Generic "GET" method to return requested item the list of workspaces.
        /// 
        /// Invokation:  IRestResponse response = GetItem(token, "workspaces", Method.GET);
        /// 
        /// </summary>
        /// <param name="token">Access Token</param>
        /// <param name="item">Item requested, i.e., categories, workspaces, etc.</param>
        /// <param name="method">HTTP operation to invoke.</param>
        public IRestResponse GetItem(string token, string item)
        {
            RestRequest request = new RestRequest($"/api/texttranslator/v1.0/{item}", Method.GET);
            
            request.AddHeader("Authorization", "Bearer " + token);

            IRestResponse response = restClient.Execute(request);

            return response;
        }

        /// <summary>
        /// Parse JSON object.
        /// </summary>
        /// <param name="responseBody">JOSN object.</param>
        private static List<string> ParseJsonResult(string responseBody)
        {
            List<string> resultList = new List<string>();
            JArray jaresult;
            try
            {
                jaresult = JArray.Parse(responseBody);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine(responseBody);
                throw;
            }
            foreach (JObject result in jaresult)
            {
                string txt = (string)result.SelectToken("translations[0].text");
                resultList.Add(txt);
            }
            return resultList;
        }

        #endregion
    }

}
