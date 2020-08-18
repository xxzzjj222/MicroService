using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroService.TeamService.Context;
using MicroService.TeamService.Models;
using MicroService.TeamService.Services;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace MicroService.TeamService.Controllers
{
    [Route("[controller]")]
    //[Authorize]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService teamService;
        private readonly IConfiguration configuration;
        private readonly TeamContext teamContext;

        public TeamsController(ITeamService teamService,IConfiguration configuration, TeamContext teamContext)
        {
            this.teamService = teamService;
            this.configuration = configuration;
            this.teamContext = teamContext;
        }

        // GET: api/Teams
        [HttpGet]
        public ActionResult<IEnumerable<Team>> GetTeams()
        {
            Console.WriteLine("查询团队信息");
            Console.WriteLine(configuration["Consul_test"]);
            teamContext.Database.GetDbConnection().ConnectionString = configuration.GetConnectionString("DefaultConnection");
            string isEnableCache = configuration["IsEnableCache"];
            if (isEnableCache == "true")
            {
                return new List<Team>();
            }
            else
            {
                return teamService.GetTeams().ToList();
            }
        }
        [HttpGet("test/{name}")]
        public ActionResult Test(string name)
        {
            return Ok("Hello : " + name);
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public ActionResult<Team> GetTeam(int id)
        {
            var team =  teamService.GetTeamById(id);

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutTeam(int id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            try
            {
                teamService.Create(team);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Teams
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public  ActionResult<Team> PostTeam(Team team)
        {
            teamService.Create(team);

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public  ActionResult<Team> DeleteTeam(int id)
        {
            var team = teamService.GetTeamById(id);
            if (team == null)
            {
                return NotFound();
            }

            teamService.Delete(team);
            return team;
        }

        private bool TeamExists(int id)
        {
            return teamService.TeamExists(id);
        }
    }
}
