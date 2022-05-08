using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lab3GradeM.Models
{
    public partial class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }
        [StringLength(50)]
        public string AssignmentName { get; set; } = null!;
        public int? Grade { get; set; }
        public bool? IsComplete { get; set; }
        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]
        [InverseProperty("Assignments")]
        public virtual Student? Student { get; set; }
    }
}
