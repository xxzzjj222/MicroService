using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core.HttpClientPolly
{
    public static class PollyHttpClientServiceCollectionExtensions
    {
        public static IServiceCollection AddPollyHttpClient(this IServiceCollection services, string name, Action<PollyHttpClientOptions> action)
        {
            //1.创建选择配置项
            PollyHttpClientOptions options = new PollyHttpClientOptions();
            action(options);

            //2.配置httpclient,熔断降级策略
            services.AddHttpClient(name)

            //2.1 降级策略
            .AddPolicyHandler(Policy<HttpResponseMessage>.HandleInner<Exception>().FallbackAsync(options.httpResponseMessage, async b =>
             {
                 // 1、降级打印异常
                 Console.WriteLine($"服务{name}开始降级,异常消息：{b.Exception.Message}");
                 // 2、降级后的数据
                 Console.WriteLine($"服务{name}降级内容响应：{options.httpResponseMessage.Content.ToString()}");
                 await Task.CompletedTask;
             }))

            //2.2 熔断策略
            .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().CircuitBreakerAsync(options.CircuitBreakerOpenFallCount, TimeSpan.FromSeconds(options.CircuitBreakerDownTime), (ex, ts) =>
             {
                 Console.WriteLine($"服务{name}断路器开启，异常消息：{ex.Exception.Message}");
                 Console.WriteLine($"服务{name}断路器开启时间：{ts.TotalSeconds}s");
             }, () =>
             {
                 Console.WriteLine($"服务{name}断路器关闭");
             }, () =>
             {
                 Console.WriteLine($"服务{name}断路器半开启(时间控制，自动开关)");
             }))
            //2.3 重试策略
            .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().RetryAsync(options.RetryCount))

            //2.4 超时策略
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(options.TimeoutTime)));

            return services;
        }
    }
}
