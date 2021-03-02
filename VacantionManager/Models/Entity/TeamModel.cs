using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VacantionManager.Models.Entity
{
    public class TeamModel
    {
        public TeamModel()
        {
            this.devs = new HashSet<UserModel>();
        }

        [Key]
        [Column(TypeName = "int")]
        public int id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name = "Enter name:")]
        public string name { get; set; }

        [ForeignKey("UserModel")]
        [Required]
        public UserModel teamLeader { get; set; }

        [ForeignKey("ProjectModel")]
        [Required]
        public ProjectModel project { get; set; }

        public virtual ICollection<UserModel> devs { get; set; }
    }
}
