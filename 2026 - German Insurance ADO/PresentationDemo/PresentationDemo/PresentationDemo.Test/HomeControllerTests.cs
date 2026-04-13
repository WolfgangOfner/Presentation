using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PresentationDemo.Controllers;
using PresentationDemo.Models;
using System.Diagnostics;

namespace PresentationDemo.Test
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public void Error_ReturnsErrorViewModel_WithRequestId()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            Assert.Equal("test-trace-id", model.RequestId);
        }

        [Fact]
        public void Error_ReturnsErrorViewModel_WithActivityCurrentId_WhenActivityExists()
        {
            // Arrange
            var activity = new Activity("TestActivity");
            activity.Start();
            
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            Assert.Equal(Activity.Current?.Id, model.RequestId);

            // Cleanup
            activity.Stop();
        }

        [Fact]
        public void Error_HasCorrectResponseCacheAttribute()
        {
            // Arrange
            var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));

            // Act
            var attribute = methodInfo?.GetCustomAttributes(typeof(ResponseCacheAttribute), false)
                .FirstOrDefault() as ResponseCacheAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(0, attribute.Duration);
            Assert.Equal(ResponseCacheLocation.None, attribute.Location);
            Assert.True(attribute.NoStore);
        }

        [Fact]
        public void Constructor_InitializesLogger()
        {
            // Arrange & Act
            var controller = new HomeController(_mockLogger.Object);

            // Assert
            Assert.NotNull(controller);
        }
    }
}
