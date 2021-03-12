using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;

namespace VacantionManager.Models.ViewModels
{
    public class TeamViewModel
    {
        public TeamViewModel(IDictionary<string,string> users, IEnumerable<string> projects)
        {
            this.users = users;
            this.projects = projects;
        }
        public IDictionary<string,string> users { get; set; }

        public IEnumerable<string> projects { get; set; }
    }
}
