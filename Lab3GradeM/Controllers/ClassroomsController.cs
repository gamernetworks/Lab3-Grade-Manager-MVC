#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab3GradeM.Data;
using Lab3GradeM.Models;
using Lab3GradeM.ViewModel;

namespace Lab3GradeM.Controllers
{
    public class ClassroomsController : Controller
    {
        private readonly LAB3GMContext _context;

        public ClassroomsController(LAB3GMContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Classrooms.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var students = _context.Students.Where(s => s.ClassId == id);
            var classId = (int)id;
            var classroomName = await _context.Classrooms.FirstOrDefaultAsync(c => c.ClassId == id);

            TestViewModel model = new TestViewModel();
            model.students = students;
            model.classId = classId;
            model.classroomName = classroomName;

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassId,ClassName")] Classroom classroom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classroom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classroom);
        }

        // GET: Classrooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null)
            {
                return NotFound();
            }
            return View(classroom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassId,ClassName")] Classroom classroom)
        {
            if (id != classroom.ClassId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classroom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassroomExists(classroom.ClassId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(classroom);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroom = await _context.Classrooms
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (classroom == null)
            {
                return NotFound();
            }

            return View(classroom);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var removeStudents = _context.Students.Where(a => a.ClassId == id);
            foreach(var studentId in removeStudents)
            {
                var removeAssignments = _context.Assignments.Where(a => a.StudentId == studentId.StudentId);
                foreach(var a in removeAssignments)
                {
                    var assignment = await _context.Assignments.FindAsync(a.AssignmentId);
                    _context.Assignments.Remove(assignment);
                }
                var student = await _context.Students.FindAsync(studentId.StudentId);
                _context.Students.Remove(student);
            }

            var classroom = await _context.Classrooms.FindAsync(id);
            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ClassroomExists(int id)
        {
            return _context.Classrooms.Any(e => e.ClassId == id);
        }

        public async Task<IActionResult> AddStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroomName = await _context.Classrooms.FirstOrDefaultAsync(c => c.ClassId == id);
            var studentInfo = _context.Students.Include(s => s.StudentId).Where(s => s.ClassId == id);
            Student newStudent = new Student();
            int classroomId = (int)id;

            IEnumerable<Student> test = from s in _context.Students where s.ClassId == id select s;

            if (classroomName == null && studentInfo == null)
            {
                return NotFound();
            }

            return View(Tuple.Create(classroomName, test, newStudent, classroomId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int classId, string studentName)
        {
            Student newStudent = new Student();
            newStudent.ClassId = classId;
            newStudent.StudentName = studentName;

            if (ModelState.IsValid)
            {
                _context.Add(newStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Classrooms", new { id = classId });
            }

            ViewData["ClassId"] = new SelectList(_context.Students, "ClassId", "ClassId", newStudent.ClassId);
            return View(newStudent);
        }

        public async Task<IActionResult> DeleteStudent(int? studentId, int classId)
        {
            if (studentId == null)
            {
                return NotFound();
            }

            var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);
            int currentClassId = classId;
            var classroomName = await _context.Classrooms.FirstOrDefaultAsync(c => c.ClassId == classId);

            PassDelStudentViewModel model = new PassDelStudentViewModel();
            model.student = student;
            model.classId = classId;
            model.classroomName = classroomName;

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("DeleteStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentConfirmed(int studentId, int classId)
        {
            var student = await _context.Students.FindAsync(studentId);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Classrooms", new { id = classId });
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }

        public async Task<IActionResult> ViewAssignments(int? studentId, int? classId)
        {
            if (classId == null || studentId == null)
            {
                return NotFound();
            }

            AssignmentDetailsViewModel model = new AssignmentDetailsViewModel();
            var assignments = _context.Assignments.Where(s => s.StudentId == studentId).Where(c => c.Student.ClassId == classId);

            var a = await _context.Students.FindAsync(studentId);
            string studentName = a.StudentName;

            var b = await _context.Classrooms.FindAsync(classId);
            string classroomName = b.ClassName;

            model.assignments = assignments;
            model.studentName = studentName;
            model.studentId = (int)studentId;
            model.classroomId = (int)classId;
            model.classroomName = classroomName;

            if(model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public async Task<IActionResult> EditAssignment(int assignmentId, int studentId, int classId)
        {
            AssignmentDetailsViewModel model = new AssignmentDetailsViewModel();
            var a = _context.Assignments.Where(s => s.StudentId == studentId).Where(c => c.Student.ClassId == classId);
            var assignment = a.Where(a => a.AssignmentId == assignmentId);

            var b = await _context.Students.FindAsync(studentId);
            string studentName = b.StudentName;

            var c = await _context.Classrooms.FindAsync(classId);            
            string classroomName = c.ClassName;

            model.assignment = new Assignment();
            foreach (var x in assignment)
            {
                model.assignment.AssignmentId = x.AssignmentId;
                model.assignment.AssignmentName = x.AssignmentName;
                model.assignment.Grade = x.Grade;
                model.assignment.IsComplete = x.IsComplete;
            }

            model.studentId = studentId;
            model.studentName = studentName; 
            model.classroomId = classId;
            model.classroomName = classroomName;

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignment(int assignmentId, int classId, Assignment assignment)
        {
            var studentId = assignment.StudentId;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.AssignmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ViewAssignments", "Classrooms", new { studentId = studentId, classId = classId });
            }
            return View(assignment);
        }
        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentId == id);
        }
        public async Task<IActionResult> CreateAssignment(int? studentId, int? classId)
        {

            if (classId == null || studentId == null)
            {
                return NotFound();
            }

            AssignmentDetailsViewModel model = new();
            var assignments = _context.Assignments.Where(s => s.StudentId == studentId).Where(c => c.Student.ClassId == classId);

            var a = await _context.Students.FindAsync(studentId);
            string studentName = a.StudentName;

            var b = await _context.Classrooms.FindAsync(classId);
            string classroomName = b.ClassName;

            model.assignments = assignments;
            model.studentName = studentName;
            model.studentId = (int)studentId;
            model.classroomId = (int)classId;
            model.classroomName = classroomName;

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignment(int classId, int studentId, Assignment assignment)
        {
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewAssignments", "Classrooms", new { classid = classId, studentId = studentId });
        }

        public async Task<IActionResult> DeleteAssignment(int? studentId, int? classId)
        {
            if (classId == null || studentId == null)
            {
                return NotFound();
            }

            AssignmentDetailsViewModel model = new AssignmentDetailsViewModel();
            var assignments = _context.Assignments.Where(s => s.StudentId == studentId).Where(c => c.Student.ClassId == classId);

            foreach(var x in assignments)
            {
                model.assignmentName = x.AssignmentName;
                model.assignmentId = x.AssignmentId;
            }

            var a = await _context.Students.FindAsync(studentId);
            string studentName = a.StudentName;

            var b = await _context.Classrooms.FindAsync(classId);
            string classroomName = b.ClassName;

            model.assignments = assignments;
            model.studentName = studentName;
            model.studentId = (int)studentId;
            model.classroomId = (int)classId;
            model.classroomName = classroomName;

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssignment(int studentId, int classId, int assignmentId)
        {
            var delete = await _context.Assignments.FindAsync(assignmentId);
            _context.Assignments.Remove(delete);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewAssignments", "Classrooms", new { studentId = studentId, classId = classId });
        }

    }
}
