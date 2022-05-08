namespace Lab3GradeM.Models
{
    public class TestViewModel
    {
        public IQueryable<Student>? students { get; set; }
        public int classId { get; set; } 
        public Classroom classroomName { get; set; } = new Classroom();
    }
}
