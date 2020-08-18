using Consul;
using MicroService.AggregateService.Model;
using MicroService.Core.Cluster;
using MicroService.Core.HttpClientConsul;
using MicroService.Core.Registry;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Services
{
    public class HttpTeamService : ITeamServiceClient
    {
        private readonly ConsulHttpClient consulHttpClient;

        private readonly string serviceSchme = "http";
        private readonly string serviceName = "Teamservice"; //服务名称
        private readonly string serviceLink = "/Teams"; //服务名称
        public HttpTeamService(ConsulHttpClient consulHttpClient)
        {
            this.consulHttpClient = consulHttpClient;
        }

        public async Task<IList<Teams>> GetTeams()
        {
            IList<Teams> teams = await consulHttpClient.GetAsync<IList<Teams>>(serviceSchme, serviceName, serviceLink);
            return teams;
        }

        public void InsertTeams(Teams team)
        {
            consulHttpClient.Post<Teams>(serviceSchme, serviceName, serviceLink, team);
        }
    }
}
