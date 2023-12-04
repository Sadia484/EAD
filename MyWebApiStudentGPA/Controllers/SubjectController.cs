using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DL.DbModels;
using Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApiStudentGPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public SubjectController(StudentDbContext context)
        {
            _context = context;
        }

        // POST /api/subjects
        [HttpPost]
        public async Task<IActionResult> AddSubject([FromBody] SubjectDbDto subject)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetSubject), new { id = subject.Id }, subject);
            }

            return BadRequest(ModelState);
        }

        // PUT /api/subjects/{subject_id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditSubject(int id, [FromBody] SubjectDbDto updatedSubject)
        {
            if (id != updatedSubject.Id)
            {
                return BadRequest();
            }

            _context.Entry(updatedSubject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE /api/subjects/{subject_id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }

        // GET /api/subject/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubject(int id)
        {
            var subject = await _context.Students.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return Ok(subject);
        }

        // GET /api/students
        [HttpGet]
        public IActionResult GetAllSubjects()
        {
            var subjects = _context.Subjects.ToList();
            return Ok(subjects);
        }
    }
}