using CoursePlatform.Application.Common;
using CoursePlatform.Application.DTOs.Lesson;
using CoursePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("course/{courseId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LessonResponse>>>> GetByCourseId(Guid courseId, CancellationToken cancellationToken)
    {
        var result = await _lessonService.GetByCourseIdAsync(courseId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<IEnumerable<LessonResponse>>.Fail(result.Error));
        }

        return Ok(ApiResponse<IEnumerable<LessonResponse>>.Ok(result.Value));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lessonService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<LessonResponse>.Fail(result.Error));
        }

        return Ok(ApiResponse<LessonResponse>.Ok(result.Value));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> Create([FromBody] CreateLessonRequest request, CancellationToken cancellationToken)
    {
        var result = await _lessonService.CreateAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<LessonResponse>.Fail(result.Error));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, ApiResponse<LessonResponse>.Ok(result.Value, "Lección creada exitosamente."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> Update(Guid id, [FromBody] UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        var result = await _lessonService.UpdateAsync(id, request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<LessonResponse>.Fail(result.Error));
        }

        return Ok(ApiResponse<LessonResponse>.Ok(result.Value, "Lección actualizada exitosamente."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> SoftDelete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lessonService.SoftDeleteAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Lección eliminada exitosamente."));
    }

    [HttpDelete("{id:guid}/hard")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lessonService.HardDeleteAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Lección eliminada permanentemente."));
    }

    [HttpPatch("{id:guid}/move-up")]
    public async Task<ActionResult<ApiResponse<object>>> MoveUp(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lessonService.MoveUpAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Lección movida hacia arriba."));
    }

    [HttpPatch("{id:guid}/move-down")]
    public async Task<ActionResult<ApiResponse<object>>> MoveDown(Guid id, CancellationToken cancellationToken)
    {
        var result = await _lessonService.MoveDownAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Lección movida hacia abajo."));
    }

    [HttpPost("course/{courseId:guid}/reorder")]
    public async Task<ActionResult<ApiResponse<object>>> Reorder(Guid courseId, [FromBody] List<ReorderLessonRequest> requests, CancellationToken cancellationToken)
    {
        var result = await _lessonService.ReorderAsync(courseId, requests, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Lecciones reordenadas exitosamente."));
    }
}