using PublishTest.Context;
using PublishTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishTest.Repositories
{
    public class TeamRepository:ITeamRepository
    {
        public TestContext testContext;

        public TeamRepository(TestContext testContext)
        {
            this.testContext = testContext;
        }

        public void Create(Team team)
        {
            testContext.Teams.Add(team);
            testContext.SaveChanges();
        }

        public void Delete(Team team)
        {
            testContext.Teams.Remove(team);
            testContext.SaveChanges();
        }

        public Team GetTeamById(int id)
        {
            return testContext.Teams.Find(id);
        }

        public IEnumerable<Team> GetTeams()
        {
            return testContext.Teams.ToList();
        }

        public bool TeamExists(int id)
        {
            return testContext.Teams.Any(e => e.Id == id);
        }

        public void Update(Team team)
        {
            testContext.Teams.Update(team);
            testContext.SaveChanges();
        }
    }
}
