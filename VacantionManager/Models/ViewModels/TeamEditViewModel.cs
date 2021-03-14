using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;

namespace VacantionManager.Models.ViewModels
{
    public class TeamEditViewModel
    {
        public TeamEditViewModel(object team,IDictionary<string, string> users, IEnumerable<string> projects)
        {
            this.team = (TeamModel)team;
            this.users = users;
            this.projects = projects;
        }

        public TeamModel team { get; set; }

        public IDictionary<string, string> users { get; set; }

        public IEnumerable<string> projects { get; set; }
    }
}
