using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VacantionManager.Models.Entity
{
    public class HospitalLeaveModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int id { get; set; }

        [Column(TypeName = "date")]
        [Required]
        [Display(Name = "Enter start date:")]
        public DateTime startDate { get; set; }

        [Column(TypeName = "date")]
        [Required]
        [Display(Name = "Enter end date:")]
        public DateTime endDate { get; set; }

        [Column(TypeName = "date")]
        [Required]
        public DateTime appicationDate { get; set; }

        [Column(TypeName = "bit")]
        [Required]
        public bool approved { get; set; }

       
        [Required]
        public byte[] ambulatoryCard { get; set; }

        [Required]
        public UserModel applicant { get; set; }
    }
}
