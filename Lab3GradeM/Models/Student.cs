using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lab3GradeM.Models
{
    public partial class Student
    {
        public Student()
        {
            Assignments = new HashSet<Assignment>();
        }

        [Key]
        public int StudentId { get; set; }
        [StringLength(50)]
        public string StudentName { get; set; } = null!;
        public int? ClassId { get; set; }

        [ForeignKey("ClassId")]
        [InverseProperty("Students")]
        public virtual Classroom? Class { get; set; }
        [InverseProperty("Student")]
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
