using IceScheduler.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Utilities
{
    public class RavensUtilities
    {
        public static List<Team> GetRavensTeams()
        {
            return new List<Team>
            {
                new Team(Association.RichmondGirls, Division.Tyke, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Tyke, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Novice, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Novice, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Atom, Level.C, 3),
                new Team(Association.RichmondGirls, Division.Peewee, Level.A, 1),
                new Team(Association.RichmondGirls, Division.Peewee, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Peewee, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Bantam, Level.A, 1),
                new Team(Association.RichmondGirls, Division.Bantam, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Midget, Level.A, 1),
                new Team(Association.RichmondGirls, Division.Midget, Level.C, 1),
                new Team(Association.RichmondGirls, Division.Midget, Level.C, 2),
                new Team(Association.RichmondGirls, Division.Juvenile, Level.C, 1),
            };
        }

    }
}
