using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
//using Grpc.Core.Logging;
using MicroService.AggregateService.Model;
using MicroService.AggregateService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using NLog;
using Servicecomb.Saga.Omega.Abstractions.Transaction;

namespace MicroService.AggregateService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AggregateController : ControllerBase
    {
        private readonly ITeamServiceClient teamServiceClient;
        private readonly IMemberServiceClient memberServiceClient;
        private readonly ICapPublisher capPublisher;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public AggregateController(ITeamServiceClient teamServiceClient, IMemberServiceClient memberServiceClient, ICapPublisher capPublisher)
        {
            this.teamServiceClient = teamServiceClient;
            this.memberServiceClient = memberServiceClient;
            this.capPublisher = capPublisher;
        }
        // GET: api/Aggregate
        [HttpGet]
        public async Task<ActionResult<IList<Teams>>> GetAsync()
        {
            IList<Teams> teams = await teamServiceClient.GetTeams();
            foreach (var team in teams)
            {
                IList<Members> members = await memberServiceClient.GetMembers(team.Id);
                team.Members = members;
            }
            logger.Info("信息获取成功");
            Console.WriteLine("信息获取成功");
            return Ok(teams);
        }

        // GET: api/Aggregate/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Aggregate
        //[HttpPost,SagaStart]
        [HttpPost]
        public ActionResult Post( string value)
        {
            Console.WriteLine($"添加团队信息和成员信息");
            //// 1、添加团队
            //var team = new Teams() { Name = "研发" };
            //teamServiceClient.InsertTeams(team);

            //// 2、添加团队成员
            //var member = new Members() { FirstName = "xzj", NickName = "xzj-1", TeamId = team.Id };
            //memberServiceClient.InsertMembers(member);

            Video video = new Video() { MemberId = 1, VideoUrl = "http://localhost:8888/1232133321" };

            capPublisher.PublishAsync<Video>("videoevent", video);

            return Ok("添加成功");
        }

        // PUT: api/Aggregate/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
