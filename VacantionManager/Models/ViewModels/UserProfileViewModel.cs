using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;

namespace VacantionManager.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel(object user,List<string> roles,List<string> teams)
        {
            this.user = (UserModel)user;
            this.roles = roles;
            this.teams = teams;
        }
        public UserModel user { get; set; }

        public  IEnumerable<string> roles { get; set; }

        public IEnumerable<string> teams { get; set; }


    }
}
