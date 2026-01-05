using CoursePlatform.Application.Common;
using CoursePlatform.Application.DTOs.Course;
using CoursePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<CourseResponse>>>> Search(
        [FromQuery] string query = "",
        [FromQuery] string status = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var searchRequest = new CourseSearchRequest
        {
            Query = query,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var result = await _courseService.SearchAsync(searchRequest, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<PagedResult<CourseResponse>>.Fail(result.Error));
        }

        return Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(result.Value));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _courseService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<CourseResponse>.Fail(result.Error));
        }

        return Ok(ApiResponse<CourseResponse>.Ok(result.Value));
    }

    [HttpGet("{id:guid}/summary")]
    public async Task<ActionResult<ApiResponse<CourseSummaryResponse>>> GetSummary(Guid id, CancellationToken cancellationToken)
    {
        var result = await _courseService.GetSummaryAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<CourseSummaryResponse>.Fail(result.Error));
        }

        return Ok(ApiResponse<CourseSummaryResponse>.Ok(result.Value));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _courseService.CreateAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<CourseResponse>.Fail(result.Error));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, ApiResponse<CourseResponse>.Ok(result.Value, "Curso creado exitosamente."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Update(Guid id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _courseService.UpdateAsync(id, request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<CourseResponse>.Fail(result.Error));
        }

        return Ok(ApiResponse<CourseResponse>.Ok(result.Value, "Curso actualizado exitosamente."));
    }

    [HttpPatch("{id:guid}/publish")]
    public async Task<ActionResult<ApiResponse<object>>> Publish(Guid id, CancellationToken cancellationToken)
    {
        var result = await _courseService.PublishAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Curso publicado exitosamente."));
    }

    [HttpPatch("{id:guid}/unpublish")]
    public async Task<ActionResult<ApiResponse<object>>> Unpublish(Guid id, CancellationToken cancellationToken)
    {
        var result = await _courseService.UnpublishAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Curso despublicado exitosamente."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> SoftDelete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _courseService.SoftDeleteAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Curso eliminado exitosamente."));
    }

    [HttpDelete("{id:guid}/hard")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> HardDelete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _courseService.HardDeleteAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error));
        }

        return Ok(ApiResponse<object>.Ok(null, "Curso eliminado permanentemente."));
    }
}