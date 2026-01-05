using CoursePlatform.Application.Common.Errors;
using CoursePlatform.Application.DTOs.Lesson;
using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Interfaces;
using CoursePlatform.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace CoursePlatform.Tests.Unit;

public class LessonServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<ILessonRepository> _mockLessonRepository;
    private readonly LessonService _lessonService;

    public LessonServiceTests()
    {
        _mockUnitOfWork = MockRepositoryFactory.CreateUnitOfWork();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockLessonRepository = new Mock<ILessonRepository>();

        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepository.Object);
        _mockUnitOfWork.Setup(u => u.Lessons).Returns(_mockLessonRepository.Object);

        _lessonService = new LessonService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CreateLessonRequest
        {
            CourseId = courseId,
            Title = "New Lesson",
            Order = 1
        };

        _mockCourseRepository.Setup(r => r.ExistsAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockLessonRepository.Setup(r => r.IsOrderUniqueAsync(
                courseId, 1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockLessonRepository.Setup(r => r.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _lessonService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("New Lesson");
        result.Value.Order.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CreateLessonRequest
        {
            CourseId = courseId,
            Title = "New Lesson",
            Order = 1
        };

        _mockCourseRepository.Setup(r => r.ExistsAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockLessonRepository.Setup(r => r.IsOrderUniqueAsync(
                courseId, 1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _lessonService.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.DuplicateOrder);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingCourse_ShouldFail()
    {
        // Arrange
        var nonExistingCourseId = Guid.NewGuid();
        var request = new CreateLessonRequest
        {
            CourseId = nonExistingCourseId,
            Title = "New Lesson",
            Order = 1
        };

        _mockCourseRepository.Setup(r => r.ExistsAsync(nonExistingCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _lessonService.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.CourseNotFound);
    }

    [Fact]
    public async Task UpdateAsync_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lesson = MockRepositoryFactory.CreateLesson(courseId, 1);
        var request = new UpdateLessonRequest
        {
            Title = "Updated Lesson",
            Order = 2
        };

        _mockLessonRepository.Setup(r => r.GetByIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lesson);

        _mockLessonRepository.Setup(r => r.IsOrderUniqueAsync(
                courseId, 2, lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockLessonRepository.Setup(r => r.Update(It.IsAny<Lesson>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _lessonService.UpdateAsync(lesson.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Updated Lesson");
        result.Value.Order.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lesson = MockRepositoryFactory.CreateLesson(courseId, 1);
        var request = new UpdateLessonRequest
        {
            Title = "Updated Lesson",
            Order = 2
        };

        _mockLessonRepository.Setup(r => r.GetByIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lesson);

        _mockLessonRepository.Setup(r => r.IsOrderUniqueAsync(
                courseId, 2, lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _lessonService.UpdateAsync(lesson.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.DuplicateOrder);
    }

    [Fact]
    public async Task SoftDeleteAsync_ExistingLesson_ShouldMarkAsDeleted()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lesson = MockRepositoryFactory.CreateLesson(courseId, 1);

        _mockLessonRepository.Setup(r => r.GetByIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lesson);

        _mockLessonRepository.Setup(r => r.Update(It.IsAny<Lesson>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _lessonService.SoftDeleteAsync(lesson.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        lesson.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteAsync_NonExistingLesson_ShouldFail()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        _mockLessonRepository.Setup(r => r.GetByIdAsync(nonExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lesson)null);

        // Act
        var result = await _lessonService.SoftDeleteAsync(nonExistingId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingLesson_ShouldReturnLesson()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lesson = MockRepositoryFactory.CreateLesson(courseId, 1);

        _mockLessonRepository.Setup(r => r.GetByIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lesson);

        // Act
        var result = await _lessonService.GetByIdAsync(lesson.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(lesson.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingLesson_ShouldFail()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        _mockLessonRepository.Setup(r => r.GetByIdAsync(nonExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lesson)null);

        // Act
        var result = await _lessonService.GetByIdAsync(nonExistingId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.NotFound);
    }

    [Fact]
    public async Task MoveUpAsync_FirstLesson_ShouldFail()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(3);
        var firstLesson = course.Lessons.OrderBy(l => l.Order).First();

        _mockLessonRepository.Setup(r => r.GetByIdAsync(firstLesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(firstLesson);

        _mockLessonRepository.Setup(r => r.GetByCourseIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course.Lessons);

        // Act
        var result = await _lessonService.MoveUpAsync(firstLesson.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.CannotMoveUp);
    }

    [Fact]
    public async Task MoveDownAsync_LastLesson_ShouldFail()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(3);
        var lastLesson = course.Lessons.OrderBy(l => l.Order).Last();

        _mockLessonRepository.Setup(r => r.GetByIdAsync(lastLesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastLesson);

        _mockLessonRepository.Setup(r => r.GetByCourseIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course.Lessons);

        // Act
        var result = await _lessonService.MoveDownAsync(lastLesson.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.CannotMoveDown);
    }

    [Fact]
    public async Task MoveUpAsync_MiddleLesson_ShouldSucceed()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(3);
        var lessons = course.Lessons.OrderBy(l => l.Order).ToList();
        var middleLesson = lessons[1];

        _mockLessonRepository.Setup(r => r.GetByIdAsync(middleLesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(middleLesson);

        _mockLessonRepository.Setup(r => r.GetByCourseIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course.Lessons);

        _mockLessonRepository.Setup(r => r.Update(It.IsAny<Lesson>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _lessonService.MoveUpAsync(middleLesson.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MoveDownAsync_MiddleLesson_ShouldSucceed()
    {
        // Arrange
        var course = MockRepositoryFactory.CreateCourseWithLessons(3);
        var lessons = course.Lessons.OrderBy(l => l.Order).ToList();
        var middleLesson = lessons[1];

        _mockLessonRepository.Setup(r => r.GetByIdAsync(middleLesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(middleLesson);

        _mockLessonRepository.Setup(r => r.GetByCourseIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course.Lessons);

        _mockLessonRepository.Setup(r => r.Update(It.IsAny<Lesson>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _lessonService.MoveDownAsync(middleLesson.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task HardDeleteAsync_ExistingLesson_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lesson = MockRepositoryFactory.CreateLesson(courseId, 1);

        _mockLessonRepository.Setup(r => r.GetByIdIncludingDeletedAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lesson);

        _mockLessonRepository.Setup(r => r.Delete(It.IsAny<Lesson>()));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _lessonService.HardDeleteAsync(lesson.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockLessonRepository.Verify(r => r.Delete(lesson), Times.Once);
    }

    [Fact]
    public async Task HardDeleteAsync_NonExistingLesson_ShouldFail()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        _mockLessonRepository.Setup(r => r.GetByIdIncludingDeletedAsync(nonExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lesson)null);

        // Act
        var result = await _lessonService.HardDeleteAsync(nonExistingId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Lesson.NotFound);
    }
}