using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Domain.Interfaces;
using Moq;

namespace CoursePlatform.Tests.Mocks;

public static class MockRepositoryFactory
{
    public static Mock<IUnitOfWork> CreateUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCourseRepository = CreateCourseRepository();
        var mockLessonRepository = CreateLessonRepository();

        mockUnitOfWork.Setup(u => u.Courses).Returns(mockCourseRepository.Object);
        mockUnitOfWork.Setup(u => u.Lessons).Returns(mockLessonRepository.Object);
        mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mockUnitOfWork.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        return mockUnitOfWork;
    }

    public static Mock<ICourseRepository> CreateCourseRepository()
    {
        var mockRepo = new Mock<ICourseRepository>();

        mockRepo.Setup(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockRepo.Setup(r => r.Update(It.IsAny<Course>()));

        mockRepo.Setup(r => r.Delete(It.IsAny<Course>()));

        mockRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return mockRepo;
    }

    public static Mock<ILessonRepository> CreateLessonRepository()
    {
        var mockRepo = new Mock<ILessonRepository>();

        mockRepo.Setup(r => r.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockRepo.Setup(r => r.Update(It.IsAny<Lesson>()));

        mockRepo.Setup(r => r.Delete(It.IsAny<Lesson>()));

        mockRepo.Setup(r => r.IsOrderUniqueAsync(
                It.IsAny<Guid>(), 
                It.IsAny<int>(), 
                It.IsAny<Guid?>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return mockRepo;
    }

    public static Course CreateCourseWithLessons(int lessonCount = 3)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course",
            Status = CourseStatus.Draft,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Lessons = new List<Lesson>()
        };

        for (int i = 1; i <= lessonCount; i++)
        {
            course.Lessons.Add(new Lesson
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                Title = $"Lesson {i}",
                Order = i,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Course = course
            });
        }

        return course;
    }

    public static Course CreateCourseWithoutLessons()
    {
        return new Course
        {
            Id = Guid.NewGuid(),
            Title = "Empty Course",
            Status = CourseStatus.Draft,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Lessons = new List<Lesson>()
        };
    }

    public static Lesson CreateLesson(Guid courseId, int order = 1)
    {
        return new Lesson
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = $"Test Lesson {order}",
            Order = order,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}