using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lab3GradeM.Models
{
    public partial class Classroom
    {
        public Classroom()
        {
            Students = new HashSet<Student>();
        }

        [Key]
        public int ClassId { get; set; }
        [StringLength(50)]
        public string ClassName { get; set; } = null!;

        [InverseProperty("Class")]
        public virtual ICollection<Student> Students { get; set; }
    }
}
