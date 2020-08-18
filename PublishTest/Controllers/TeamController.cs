using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PublishTest.Models;
using PublishTest.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PublishTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private ITeamService teamService;

        public TeamController(ITeamService teamService)
        {
            this.teamService = teamService;
        }
        // GET: api/<TeamController>
        [HttpGet]
        public ActionResult<IEnumerable<Team>> GetTeams()
        {
            return teamService.GetTeams().ToList();
        }

        // GET api/<TeamController>/5
        [HttpGet("{id}")]
        public Team GetTeam(int id)
        {
            return teamService.GetTeamById(id);
        }
        [HttpGet("test/{name}")]
        public string Test(string name)
        {
            return $"hello {name}";
        }

        // POST api/<TeamController>
        [HttpPost]
        public ActionResult<Team> Post(Team team)
        {
            teamService.Create(team);
            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // PUT api/<TeamController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TeamController>/5
        [HttpDelete("{id}")]
        public ActionResult<Team> Delete(int id)
        {
            var team = teamService.GetTeamById(id);
            if (team==null)
            {
                return NotFound();
            }
            teamService.Delete(team);
            return team;
        }
    }
}
