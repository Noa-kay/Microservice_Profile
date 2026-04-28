using MarketingNotificationService.Data.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using student_profile.BLL;
using student_profile.Controllers;
using student_profile.DTOs;

namespace student_profile.Tests;

public class UsersControllerTests
{
    [Fact]
    public async Task Create_PublishesGraduatesCountWithCorrectTotalUsers()
    {
        // Arrange
        var expectedCount = 5;
        var userService = new Mock<IUserService>();
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var createdUser = new UserDto { Id = Guid.NewGuid() };
        userService
            .Setup(s => s.CreateAsync(It.IsAny<UserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserCreatedResult(createdUser, expectedCount));

        graduates_count? publishedMessage = null;
        publishEndpoint
            .Setup(p => p.Publish(
                It.IsAny<graduates_count>(),
                It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((message, _) =>
            {
                publishedMessage = (graduates_count)message;
            })
            .Returns(Task.CompletedTask);

        var controller = new UsersController(userService.Object, publishEndpoint.Object);

        // Act
        var response = await controller.Create(new UserDto(), CancellationToken.None);

        // Assert
        var result = Assert.IsType<CreatedAtActionResult>(response.Result);
        var payload = Assert.IsType<UserCreatedResult>(result.Value);
        Assert.Equal(expectedCount, payload.TotalUsers);
        Assert.NotNull(publishedMessage);
        Assert.Equal(expectedCount, publishedMessage!.TotalGraduates);
    }
}
