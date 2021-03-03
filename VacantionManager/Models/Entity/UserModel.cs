using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VacantionManager.Models.Entity
{
    public class UserModel
    {
        public UserModel()
        {
            this.leadedTeams = new HashSet<TeamModel>();
            this.leaves = new HashSet<LeaveModel>();
            this.hospitalLeaves = new HashSet<HospitalLeaveModel>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name ="Enter username:")]
        public string username { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name = "Enter first name:")]
        public string firstName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name = "Enter last name:")]
        public string lastName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Enter password name:")]
        public string password { get; set; }

        [NotMapped]
        [Compare("password")]
        public string confirmPassword { get; set; }

        [Required]
        public RoleModel role { get; set; }
            
        public TeamModel team { get; set; }

        public virtual ICollection<TeamModel> leadedTeams { get; set; }

        public virtual ICollection<LeaveModel> leaves { get; set; }

        public virtual ICollection<HospitalLeaveModel> hospitalLeaves { get; set; }


    }
}
