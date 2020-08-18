using MicroService.Core.Cluster;
using MicroService.Core.Registry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroService.Core.HttpClientConsul
{
    public class ConsulHttpClient
    {
        private readonly IServiceDiscovery serviceDiscovery;
        private readonly ILoadBalance loadBalance;
        private readonly IHttpClientFactory httpClientFactory;

        public ConsulHttpClient(IServiceDiscovery serviceDiscovery, ILoadBalance loadBalance, IHttpClientFactory httpClientFactory)
        {
            this.serviceDiscovery = serviceDiscovery;
            this.loadBalance = loadBalance;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<T> GetAsync<T>(string servicescheme, string serviceName, string serviceLink)
        {
            //1、获取服务地址
            IList<ServiceUrl> serviceUrls = await serviceDiscovery.Discovery(serviceName);

            //2、服务均衡服务
            ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

            //for (int i = 0; i < 100; i++)
            //{
                //Thread.Sleep(1000);
                //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                //3、建立请求
                HttpClient httpClient = httpClientFactory.CreateClient("micro");
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(servicescheme + "://"+serviceUrl.Url + serviceLink);
                //HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("http://localhost:5000" + serviceLink);

                //4、json转换成对象
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string json = await httpResponseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    Console.WriteLine($"降级处理：{await httpResponseMessage.Content.ReadAsStringAsync()}");
                    //throw new Exception($"{serviceName}服务调用错误");
                }
            //}
            return default(T);
        }

        public  T Post<T>(string servicescheme,string serviceName,string serviceLink,object paramData=null)
        {
            //获取服务
            IList<ServiceUrl> serviceUrls =  serviceDiscovery.Discovery(serviceName).Result;

            //负载均衡服务
            ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

            //建立请求
            Console.WriteLine($"请求路径：{servicescheme}+://{serviceUrl.Url}");
            HttpClient httpClient = httpClientFactory.CreateClient("micro");

            //转换为json
            HttpContent hc = new StringContent(JsonConvert.SerializeObject(paramData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = httpClient.PostAsync(servicescheme + "://" + serviceUrl.Url + serviceLink, hc).Result;

            //json转为对象
            if (response.StatusCode==HttpStatusCode.OK ||response.StatusCode==HttpStatusCode.Created)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                //进行自定义异常处理，这个地方进行了降级处理
                throw new Exception($"{serviceName}服务调用错误:{response.Content.ReadAsStringAsync()}");
            }

        }
    }
}
