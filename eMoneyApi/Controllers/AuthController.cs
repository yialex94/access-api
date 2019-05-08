using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Web.Mvc;
using eMoneyApi.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace eMoneyApi.Controllers
{
    public class AuthController : Controller
    {
        // Creates An instance Of ApplicationUserManager Class
        private ApplicationUserManager _userManager;

        // Method To Grab User 
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // Method To Generate Client Assertion Token
        public string GetClientAssertionToken(string clientId, X509Certificate2 certificate, string tokenEndpoint)
        {
            SecurityToken st = null;
            SecurityTokenHandler sth = null;
            SecurityTokenDescriptor std = null;

            sth = new JwtSecurityTokenHandler { SetDefaultTimesOnTokenCreation = false };
            std = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, clientId) }),
                SigningCredentials = new SigningCredentials(new X509SecurityKey(certificate), SecurityAlgorithms.RsaSha256Signature),
                Audience = tokenEndpoint,
                Expires = DateTime.UtcNow.AddMinutes(600), //for 10 hrs
                Issuer = clientId
            };
            st = sth.CreateToken(std);

            return sth.WriteToken(st);
        }

        // Method To Generate Bearer Token 
        public eMoneyAuthInfo GetAccessToken(string certName, string api_clientId, string authTokenURL)
        {
            eMoneyAuthInfo info = new eMoneyAuthInfo();
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadOnly);
            X509Certificate selectedCertificate = null;

            foreach (X509Certificate cert in store.Certificates)
            {
                if (cert.Subject.Contains(certName))
                {
                    selectedCertificate = cert;
                    break;
                }
            }

            try
            {
                info.ClientAssertion = GetClientAssertionToken(api_clientId, (X509Certificate2)selectedCertificate, authTokenURL);
                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", info.ClientAssertion },
                    { "scope", "API" }
                });

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(authTokenURL);
                    client.DefaultRequestHeaders.Clear();

                    HttpResponseMessage response = client.PostAsync(authTokenURL, requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string callResponse = response.Content.ReadAsStringAsync().Result;
                        eMoneyAccessToken x = Newtonsoft.Json.JsonConvert.DeserializeObject<eMoneyAccessToken>(callResponse);
                        if (x != null)
                        {
                            info.BearerToken = x;
                        }
                    }
                }
                return info;
            }
            catch (Exception e)
            {
                throw new Exception("Error occurred in getting an Access Token.\r\n" + e.ToString());
            }
        }

        // Method To Call GET All Clients
        public object ClientGetRequest(string endpointURL, string accessToken)
        {
            object result = new object();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpointURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                client.DefaultRequestHeaders.Add("apikey", "umSwXJvGj1qOvckia9xa8PDGWIqkQqSs");

                HttpResponseMessage response = client.GetAsync(endpointURL).Result;
                if (response.IsSuccessStatusCode)
                {
                    string callResponse = response.Content.ReadAsStringAsync().Result;
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<Clients_Request>(callResponse);
                }
            }
            return result;
        }

        // Method To Call GET Specific Client
        public object GetSpecificClient(string endpointURL, string accessToken)
        {
            object result = new object();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpointURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                client.DefaultRequestHeaders.Add("Apikey", "Ayi1234");

                HttpResponseMessage response = client.GetAsync(endpointURL).Result;
                if (response.IsSuccessStatusCode)
                {
                    string callResponse = response.Content.ReadAsStringAsync().Result;
                    
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<DetailedClient>(callResponse, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
            }
            return result;
        }

        public object UpdateClientAPI(string endpointURL, string accessToken, DetailedClient updatedClient)
        {
            object result = new object();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpointURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("Apikey", "Ayi1234");

                var myContent = JsonConvert.SerializeObject(updatedClient, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = client.PutAsync(endpointURL, byteContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    string callResponse = response.Content.ReadAsStringAsync().Result;
                    result = callResponse;
                }
            }
            return result;
        }   

        // Method To Call Get All Plans For Respective Client
        public object PlanGetRequest(string endpointURL, string accessToken)
        {
            object result = new object();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpointURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = client.GetAsync(endpointURL).Result;
                if (response.IsSuccessStatusCode)
                {
                    string callResponse = response.Content.ReadAsStringAsync().Result;
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<Plan_Request>(callResponse);
                }
            }
            return result;
        }



        // Action To Navigate Between Different API Calls
        public ActionResult Navigate(string token)
        {
            return View();
        }

        // GET: /Auth/Index
        public ActionResult Index()
        {
            ClientAssertionInfoViewModel vm = new ClientAssertionInfoViewModel();
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            vm.Cert_Name = currentUser.Cert_Name;
            vm.API_ClientID = currentUser.ClientID;
            vm.Auth_Url = currentUser.Auth_Url;
            return View(vm);
        }

        // POST: /Auth/Index
        [HttpPost]
        public async Task<ActionResult> Index(ClientAssertionInfoViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                    currentUser.Cert_Name = model.Cert_Name;
                    currentUser.ClientID = model.API_ClientID;
                    currentUser.Auth_Url = model.Auth_Url;
                    await SaveChangesToDB();
                }
                var a = GetAccessToken(model.Cert_Name, model.API_ClientID, model.Auth_Url);
                Session["token"] = a.BearerToken.access_token;
                return RedirectToAction("GetClients", "Auth");

            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Auth", "Index"));
            }
        }

        // GET: /Auth/GetClients
        
        [HttpGet]
        public ActionResult GetClients(string token)
        {
            ClientInformation client = new ClientInformation();

            var endpointURL = "https://api-externalbeta2.emaplan.com/api/clients";
            client.clients = (Clients_Request)ClientGetRequest(endpointURL, Session["token"].ToString());

            return View(client);

        }
     

        // GET: /Auth/GetSpecificClient/{clientId}
        [HttpGet]
        public ActionResult GetSpecificClient(string id)
        {
            DetailedClient client = new DetailedClient();
            var endpointURL = "https://api-externalbeta2.emaplan.com/api/clients/" + id;
            client = (DetailedClient)GetSpecificClient(endpointURL, Session["token"].ToString());

            return View(client);
        }

        // GET: /Auth/Edit
        public ActionResult EditClient(string id)
        {
            DetailedClient client = new DetailedClient();
            var endpointURL = "https://api-externalbeta2.emaplan.com/api/clients/" + id;
            client = (DetailedClient)GetSpecificClient(endpointURL, Session["token"].ToString());

            return View(client);
        }

        [HttpPost]
        public ActionResult EditClient(DetailedClient info)
        {
            var endpointURL = "https://api-externalbeta2.emaplan.com/api/clients/" + info.id;       

            var updateCall = UpdateClientAPI(endpointURL, Session["token"].ToString(), info);

            return RedirectToAction("EditClient", "Auth");
        }

        // GET: /Auth/GetPlans/{clientId}
        [HttpGet]
        public ActionResult GetPlans(string id)
        {
            Plan_Request plan = new Plan_Request();

            var endpointURL = "https://api-externalbeta2.emaplan.com/api/clients/" + id + "/plans";
            plan = (Plan_Request)PlanGetRequest(endpointURL, Session["token"].ToString());

            return View(plan);
        }

        // Method to Save Changes To DbContext
        private async Task<int> SaveChangesToDB()
        {
            var DBContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            return await DBContext.SaveChangesAsync();
        }

    }
}


