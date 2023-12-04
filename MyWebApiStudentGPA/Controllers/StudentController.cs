using Microsoft.AspNetCore.Mvc;
using Core.Models.RequestModels;
using Core.Models.ResponseModels;
using Core.Interfaces;
using System.Collections.Generic;


[ApiController]
[Route("students")]
public class StudentController : ControllerBase
{
    private readonly IStudentDL _studentDL;

    public StudentController(IStudentDL studentDL)
    {
        _studentDL = studentDL;
    }

    [HttpPost]
    public ActionResult<StudentResponseDto> CreateStudent([FromBody] StudentRequestDto studentRequestDto)
    {
        var createdStudent = _studentDL.SaveStudent(studentRequestDto);
        return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.Id }, createdStudent);
    }

    [HttpPut("{student_id}")]
    public IActionResult UpdateStudent(int student_id, [FromBody] StudentRequestDto studentRequestDto)
    {
        // Validating the incoming data
        var updatedStudent = _studentDL.UpdateStudent(student_id, studentRequestDto);
        if (updatedStudent == null)
        {
            return NotFound();
        }

        return Ok(updatedStudent);
    }

    [HttpDelete("{student_id}")]
    public IActionResult DeleteStudent(int student_id)
    {
        var deletedStudent = _studentDL.DeleteStudent(student_id);
        if (deletedStudent == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{student_id}")]
    public ActionResult<StudentDetailSubjectResponseDto> GetStudent(int student_id)
    {
        var student = _studentDL.GetStudent(student_id);
        if (student == null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpGet]
    public ActionResult<IEnumerable<StudentResponseDto>> GetStudents()
    {
        var students = _studentDL.GetStudentsAsync();
        return Ok(students);
    }
}