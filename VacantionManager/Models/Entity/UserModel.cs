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
        [Key]
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
        [Required]
        public string confirmPassword { get; set; }

        [ForeignKey("RoleModel")]
        [Required]
        public RoleModel role { get; set; }

        [ForeignKey("TeamModel")]
        [Required]
        public TeamModel team { get; set; }


    }
}
