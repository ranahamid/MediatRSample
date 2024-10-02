using Xunit;
using Moq;
using MediatR;
using MediatRSample.Controllers;
using MediatRHandler.Entities;
using MediatRHandler.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRSample.Tests
{
    public class CustomerControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CustomerController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetCustomer_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = 1;
            var expectedCustomer = new Customer { Id = customerId, FirstName = "John Doe" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerRequest>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedCustomer);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCustomer.Id, result.Id);
            Assert.Equal(expectedCustomer.FirstName, result.FirstName);
        }

        [Fact]
        public async Task GetCustomer_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerRequest>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsCustomerId_WhenCustomerIsCreated()
        {
            // Arrange
            var newCustomer = new Customer { FirstName = "Jane Doe" };
            var expectedCustomerId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedCustomerId);

            // Act
            var result = await _controller.CreateCustomer(newCustomer);

            // Assert
            Assert.Equal(expectedCustomerId, result);
        }

        [Fact]
        public async Task CreateCustomer_ThrowsException_WhenRequestIsInvalid()
        {
            // Arrange
            Customer invalidCustomer = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.CreateCustomer(invalidCustomer));
        }
    }
}