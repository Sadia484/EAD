using DL.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApiStudentGPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentSubjectController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentSubjectController(StudentDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AssignSubjectToStudent([FromBody] StudentSubjectDbDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!StudentAndSubjectExist(payload.SID, payload.SubjectId))
            {
                return NotFound("Student or Subject not found.");
            }

            _context.StudentSubjects.Add(payload);
            await _context.SaveChangesAsync();

            return Ok(payload);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] StudentSubjectDbDto updatedPayload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!AssignmentExists(id))
            {
                return NotFound("Assignment not found.");
            }

            var existingAssignment = await _context.StudentSubjects.FindAsync(id);
            existingAssignment.GPA = updatedPayload.GPA;
            existingAssignment.Marks = updatedPayload.Marks;

            await _context.SaveChangesAsync();

            return Ok(existingAssignment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var existingAssignment = await _context.StudentSubjects.FindAsync(id);

            if (existingAssignment == null)
            {
                return NotFound("Assignment not found.");
            }

            _context.StudentSubjects.Remove(existingAssignment);
            await _context.SaveChangesAsync();

            return Ok("Assignment deleted successfully.");
        }

        [HttpGet("~/api/students/{studentId}/subjects")]
        public IActionResult GetStudentSubjects(int studentId)
        {
            var subjects = _context.StudentSubjects
                .Where(ss => ss.SID == studentId)
                .Select(ss => new { ss.SubjectId, ss.GPA, ss.Marks })
                .ToList();

            return Ok(subjects);
        }

        [HttpGet("~/api/students/{studentId}/subjects/{subjectId}/marks")]
        public IActionResult GetMarksForSubject(int studentId, int subjectId)
        {
            var marks = _context.StudentSubjects
                .FirstOrDefault(ss => ss.SID == studentId && ss.SubjectId == subjectId)?
                .Marks;

            if (marks == null)
            {
                return NotFound("Marks for the specified subject and student not found.");
            }

            return Ok(new { Marks = marks });
        }

        [HttpGet("~/api/students/{studentId}/marks")]
        public IActionResult GetAllMarksForStudent(int studentId)
        {
            var marksForStudent = _context.StudentSubjects
                .Where(ss => ss.SID == studentId)
                .Select(ss => new { ss.SubjectId, ss.Marks })
                .ToList();

            if (marksForStudent.Count == 0)
            {
                return NotFound("No marks found for the specified student.");
            }

            return Ok(marksForStudent);
        }

        [HttpGet("~/api/students/{studentId}/gpa")]
        public IActionResult GetGPAForStudent(int studentId)
        {
            var studentSubjects = _context.StudentSubjects
                .Where(ss => ss.SID == studentId)
                .ToList();

            if (studentSubjects.Count == 0)
            {
                return NotFound("No subjects found for the specified student.");
            }

            double totalCredits = 0;
            double totalGPA = 0;

            foreach (var subject in studentSubjects)
            {
                totalCredits += 1;
                totalGPA += subject.GPA;
            }

            double currentGPA = totalGPA / totalCredits;

            return Ok(new { GPA = currentGPA });
        }

        // Helper methods

        private bool StudentAndSubjectExist(int studentId, int subjectId)
        {
            return _context.Students.Any(s => s.Id == studentId) &&
                   _context.Subjects.Any(sub => sub.Id == subjectId);
        }

        private bool AssignmentExists(int assignmentId)
        {
            return _context.StudentSubjects.Any(a => a.Id == assignmentId);
        }
    }
}