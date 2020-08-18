using MicroService.AggregateService.Model;
using MicroService.Core.HttpClientConsul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Services
{
    public class HttpMemberServiceClient : IMemberServiceClient
    {
        private readonly ConsulHttpClient consulHttpClient;

        private string schemem = "http";
        private string serviceName = "MemberService";
        private string serviceLink = "/Members";

        public HttpMemberServiceClient(ConsulHttpClient consulHttpClient)
        {
            this.consulHttpClient = consulHttpClient;
        }
        public async Task<IList<Members>> GetMembers(int teamId)
        {
            string urlLink = serviceLink + "?teamId=" + teamId;
            var members =await consulHttpClient.GetAsync<IList<Members>>(schemem, serviceName, urlLink);
            return members;
        }

        public void InsertMembers(Members member)
        {
            consulHttpClient.Post<Members>(schemem, serviceName, serviceLink, member);
        }
    }
}
