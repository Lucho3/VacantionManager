using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;

namespace VacantionManager.Models.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public ProjectDetailsViewModel(object project, IEnumerable<TeamModel> teams)
        {
            this.teams = teams;
            this.project = (ProjectModel)project;
        }
        public ProjectModel project { get; set; }
        public IEnumerable<TeamModel> teams { get; set; }
    }
}

