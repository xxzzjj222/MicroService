using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroService.MVCClient.Models;
using System.Net.Http;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace MicroService.MVCClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            #region token模式 
            {
                // 1、生成AccessToken
                // 1.1 客户端模式
                // 1.2 客户端用户密码模式
                // 1.3 客户端code状态码模式
                #region token模式
                //string access_token = await GetAccessToken();

                //// 2、使用AccessToken 进行资源访问
                //string result = await UseAccessToken(access_token);
                #endregion

                #region openid
                //1.获取token (id_token,access_token,refresh_token)
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                //Console.WriteLine($"accessToken:{accessToken}");
                // var refreshToken =await HttpContext.GetTokenAsync("refresh_token");
                //2.使用token
                var client = new HttpClient();
                client.SetBearerToken(accessToken);
                /*client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);*/
                var result = await client.GetStringAsync("http://localhost:5006/AggregateService/aggregate");
                //var result = await client.GetStringAsync("http://localhost:5000/teams");

                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 0.9));
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
                //var result = await client.PostAsync("http://localhost:5006/AggregateService/aggregate", null);
                #endregion
                //// 3、响应结果到页面
                //ViewData.Add("Json", await result.Content.ReadAsStringAsync());
                ViewData.Add("Json", result);
            }
            #endregion

            return View();
        }
        /// <summary>
        /// 生成token
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetAccessToken()
        {
            //1、建立连接
            HttpClient client = new HttpClient();
            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync("http://localhost:5008/");
            if (disco.IsError)
            {
                Console.WriteLine($"[DiscoveryDocumentResponse Error]: {disco.Error}");
            }

            // 1.1、通过客户端获取AccessToken
            //TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            //{
            //    Address=disco.TokenEndpoint,
            //    ClientId="client",
            //    ClientSecret="secret",
            //    Scope="TeamService"
            //});

            //1.2 通过客户端账号密码获取AccessToken
            TokenResponse tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client-password",
                ClientSecret = "secret",
                Scope = "TeamService",
                UserName = "xzj",
                Password = "123456"
            });

            ////1.3 通过code获取AccessToken
            //TokenResponse tokenResponse = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            //{
            //    Address=disco.TokenEndpoint, 
            //    ClientId= "client-code",
            //    ClientSecret="secret",
            //    Code="12",
            //    RedirectUri= "http://localhost:5008"
            //});

            if (tokenResponse.IsError)
            {
                //ClientId 与 ClientSecret 错误，报错：invalid_client
                //Scope 错误，报错：invalid_scope
                //UserName 与 Password 错误，报错：invalid_username_or_password
                string errorDesc = tokenResponse.ErrorDescription;
                if (string.IsNullOrEmpty(errorDesc)) 
                    errorDesc = "";
                if (errorDesc.Equals("invalid_username_or_password"))
                    Console.WriteLine("用户名或密码错误，请重新输入！");
                else
                    Console.WriteLine($"[TokenResponse Error]: {tokenResponse.Error}, [TokenResponse Error Description]: {errorDesc}");
            }
            else
            {
                Console.WriteLine($"Access Token: {tokenResponse.Json}");
                Console.WriteLine($"Access Token: {tokenResponse.RefreshToken}");
                Console.WriteLine($"Access Token: {tokenResponse.ExpiresIn}");
            }
            return tokenResponse.AccessToken;
        }

        public static async Task<string> UseAccessToken(string accessToken)
        {
            HttpClient apiClient = new HttpClient();
            apiClient.SetBearerToken(accessToken);
            HttpResponseMessage response = await apiClient.GetAsync("http://localhost:5006/ocelot/teams");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Request Error, StatusCode is : {response.StatusCode}");
                return $"API Request Error, StatusCode is : {response.StatusCode}";
            }
            else
            {
                string content =await response.Content.ReadAsStringAsync();
                Console.WriteLine("");
                Console.WriteLine($"Result: {JArray.Parse(content)}");

                return JArray.Parse(content).ToString();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
