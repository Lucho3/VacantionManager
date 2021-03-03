using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VacantionManager.Models.Entity
{
    public class RoleModel
    {
        public RoleModel()
        {
            this.users = new HashSet<UserModel>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name = "Enter name:")]
        public string name { get; set; }

        public virtual ICollection<UserModel> users { get; set; }
    }
}
