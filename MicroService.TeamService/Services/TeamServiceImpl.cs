using MicroService.TeamService.Models;
using MicroService.TeamService.Repositories;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MicroService.TeamService.Services
{
    public class TeamServiceImpl : ITeamService
    {
        public readonly ITeamRepository teamRepository;
        public TeamServiceImpl(ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository;
        }

        [Compensable(nameof(Delete))]
        public void Create(Team team)
        {
            teamRepository.Create(team);
        }

        public void Delete(Team team)
        {
            teamRepository.Delete(team);
        }

        public Team GetTeamById(int id)
        {
            return teamRepository.GetTeamById(id);
        }

        public IEnumerable<Team> GetTeams()
        {
            return teamRepository.GetTeams();
        }

        public bool TeamExists(int id)
        {
            return teamRepository.TeamExists(id);
        }

        public void Update(Team team)
        {
            teamRepository.Update(team);
        }
    }
}
