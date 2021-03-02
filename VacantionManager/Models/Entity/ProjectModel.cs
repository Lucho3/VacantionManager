using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VacantionManager.Models.Entity
{
    public class ProjectModel
    {
        public ProjectModel()
        {
            this.workingTeams = new HashSet<TeamModel>();
        }

        [Key]
        [Column(TypeName = "int")]
        public int id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name = "Enter name:")]
        public string name { get; set; }

        [Column(TypeName = "text")]
        [Required]
        [Display(Name = "Enter description:")]
        public string description { get; set; }

        public virtual ICollection<TeamModel> workingTeams { get; set; }
    }
}
