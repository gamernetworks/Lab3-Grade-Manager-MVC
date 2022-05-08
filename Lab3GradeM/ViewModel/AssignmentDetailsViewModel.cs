using Lab3GradeM.Models;

namespace Lab3GradeM.ViewModel
{
    public class AssignmentDetailsViewModel
    {
        public IQueryable<Assignment>? assignments { get; set; }
        public int assignmentId { get; set; }
        public Assignment? assignment { get; set; }
        public string? assignmentName { get; set; }
        public string? studentName { get; set; }
        public int studentId { get; set; }
        public int classroomId { get; set; }
        public string? classroomName { get; set; }               
    }
}
