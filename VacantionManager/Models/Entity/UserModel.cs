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
        [Required(ErrorMessage = "Username is required field!")]
        [Display(Name ="Enter username:")]
        public string username { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "First name is required! field")]
        [Display(Name = "Enter first name:")]
        public string firstName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "Last name is required field!")]
        [Display(Name = "Enter last name:")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Password is required field!")]
        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Enter password")]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}", ErrorMessage = "Password format doesn't match!\nIt must contains one uppercase and one lowercase letter, one symbol,\none number and it must be at least 8 symbols long!")]
        public string password { get; set; }

        [NotMapped]
        [Compare("password",ErrorMessage ="Passwords must match!")]
        [Display(Name = "Confirm your password:")]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}", ErrorMessage = "Password format doesn't match!\nIt must contains one uppercase and one lowercase letter, one symbol,\none number and it must be at least 8 symbols long!")]
        public string confirmPassword { get; set; }

        [Required]
        public RoleModel role { get; set; }
            
        public TeamModel team { get; set; }

        public virtual ICollection<TeamModel> leadedTeams { get; set; }

        public virtual ICollection<LeaveModel> leaves { get; set; }

        public virtual ICollection<HospitalLeaveModel> hospitalLeaves { get; set; }


    }
}
