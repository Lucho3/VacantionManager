using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;

namespace VacantionManager.Models.ViewModels
{
    public class TeamDetailsViewModel
    {
        //todo
        public TeamDetailsViewModel(object team,IEnumerable<UserModel> users)
        {
            this.users = users;
            this.team = (TeamModel)team;
        }
        public TeamModel team { get; set; }
        public IEnumerable<UserModel> users  { get; set; }
        }
    }

