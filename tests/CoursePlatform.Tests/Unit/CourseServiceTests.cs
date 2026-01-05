using CoursePlatform.Application.Common.Errors;
using CoursePlatform.Application.DTOs.Course;
using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Domain.Interfaces;
using CoursePlatform.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace CoursePlatform.Tests.Unit;

public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _mockUnitOfWork = MockRepositoryFactory.CreateUnitOfWork();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepository.Object);
        _courseService = new CourseService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateCourseRequest
        {
            Title = "New Course"
        };

        _mockCourseRepository.Setup(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _courseService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("New Course");
        result.Value.Status.Should().Be("Draft");
    }

    [Fact]
    public async Task PublishAsync_WithLessons_ShouldSucceed()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(3);

        _mockCourseRepository.Setup(r => r.GetByIdWithLessonsAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _mockCourseRepository.Setup(r => r.Update(It.IsAny<Course>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _courseService.PublishAsync(course.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        course.Status.Should().Be(CourseStatus.Published);
    }

    [Fact]
    public async Task PublishAsync_WithoutLessons_ShouldFail()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithoutLessons();

        _mockCourseRepository.Setup(r => r.GetByIdWithLessonsAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishAsync(course.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Course.CannotPublish);
        course.Status.Should().Be(CourseStatus.Draft);
    }

    [Fact]
    public async Task PublishAsync_WithAllLessonsDeleted_ShouldFail()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(2);
        foreach (var lesson in course.Lessons)
        {
            lesson.IsDeleted = true;
        }

        _mockCourseRepository.Setup(r => r.GetByIdWithLessonsAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishAsync(course.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Course.CannotPublish);
    }

    [Fact]
    public async Task SoftDeleteAsync_ExistingCourse_ShouldMarkAsDeleted()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(1);

        _mockCourseRepository.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _mockCourseRepository.Setup(r => r.Update(It.IsAny<Course>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _courseService.SoftDeleteAsync(course.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        course.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingCourse_ShouldFail()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        _mockCourseRepository.Setup(r => r.GetByIdWithLessonsAsync(nonExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.GetByIdAsync(nonExistingId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Course.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ExistingCourse_ShouldUpdateTitle()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(1);
        var request = new UpdateCourseRequest
        {
            Title = "Updated Title"
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _mockCourseRepository.Setup(r => r.Update(It.IsAny<Course>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _courseService.UpdateAsync(course.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UnpublishAsync_PublishedCourse_ShouldChangeToDraft()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(1);
        course.Status = CourseStatus.Published;

        _mockCourseRepository.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _mockCourseRepository.Setup(r => r.Update(It.IsAny<Course>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _courseService.UnpublishAsync(course.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        course.Status.Should().Be(CourseStatus.Draft);
    }

    [Fact]
    public async Task HardDeleteAsync_NonExistingCourse_ShouldFail()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        _mockCourseRepository.Setup(r => r.GetByIdIncludingDeletedAsync(nonExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.HardDeleteAsync(nonExistingId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Course.NotFound);
    }
}