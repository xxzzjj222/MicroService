using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MicroService.IdentityServer4
{
    public class Config
    {
        /// <summary>
        /// api资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("TeamService", "TeamService api 需要被保护")
            };
        }
        /// <summary>
        /// 客户端
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {

            // 1、client认证模式
            // 2、client用户密码认证模式
            // 3、授权码认证模式(code)
            // 4、简单认证模式(js)
            return new List<Client>
            {
                // 1.没有交互性用户，使用 clientid/secret 实现认证。
                new Client
                {
                    ClientId="client",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    ClientSecrets=
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes={ "TeamService","MemberService"}
                },
                // 2、client用户密码认证模式
                new Client
                {
                    ClientId="client-password",
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    ClientSecrets=
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes={ "TeamService", "MemberService" }
                },
                // 3、授权码认证模式(code)
                new Client
                {
                    ClientId="client-code",
                    AllowedGrantTypes=GrantTypes.Code,
                    ClientSecrets=
                    {
                        new Secret("secret".Sha256())
                    },
                    RequireConsent=false,
                    RequirePkce=true,

                    RedirectUris={ "http://localhost:5010/signin-oidc"}, // 1、客户端地址
                    PostLogoutRedirectUris={ "http://localhost:5010/signout-callback-oidc"},// 2、登录退出地址

                    AllowedScopes=new List<string>{
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "TeamService" // 启用服务授权支持
                    },

                    // 增加授权访问
                    AllowOfflineAccess=true
                }
            };
        }
        /// <summary>
        /// openid身份资源
        /// </summary>
        public static IEnumerable<IdentityResource> Ids => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        /// <summary>
        /// 测试用户
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId="1",
                    Username="xzj",
                    Password="123456"
                },
                new TestUser
                {
                    SubjectId="818727",
                    Username="xxzzjj",
                    Password="123456",
                    Claims=
                    {
                        new Claim(JwtClaimTypes.Name, "xxzzjj"),
                        new Claim(JwtClaimTypes.GivenName, "xxzzjj"),
                        new Claim(JwtClaimTypes.FamilyName, "xxzzjj"),
                        new Claim(JwtClaimTypes.Email, "xxzzjj@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://xxzzjj.com"),
                        //new Claim(JwtClaimTypes.Address, @"{ '城市': '杭州', '邮政编码': '310000' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };
        }
    }
}
