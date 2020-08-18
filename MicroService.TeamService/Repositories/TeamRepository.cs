﻿using MicroService.TeamService.Context;
using MicroService.TeamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.TeamService.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        public TeamContext teamContext;

        public TeamRepository(TeamContext teamContext)
        {
            this.teamContext = teamContext;
        }
        public void Create(Team team)
        {
            teamContext.Teams.Add(team);
            teamContext.SaveChanges();
        }

        public void Delete(Team team)
        {
            teamContext.Teams.Remove(team);
            teamContext.SaveChanges();
        }

        public Team GetTeamById(int id)
        {
            return teamContext.Teams.Find(id);
        }

        public IEnumerable<Team> GetTeams()
        {
            return teamContext.Teams.ToList();
        }

        public bool TeamExists(int id)
        {
            return teamContext.Teams.Any(e => e.Id == id);
        }

        public void Update(Team team)
        {
            teamContext.Teams.Update(team);
            teamContext.SaveChanges();
        }
    }
}
