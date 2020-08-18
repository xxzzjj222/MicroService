using PublishTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishTest.Repositories
{
    public interface ITeamRepository
    {
        IEnumerable<Team> GetTeams();
        Team GetTeamById(int id);
        void Create(Team team);
        void Update(Team team);
        void Delete(Team team);
        bool TeamExists(int id);
    }
}
