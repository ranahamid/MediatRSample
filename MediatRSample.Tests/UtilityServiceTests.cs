using MediatRSample.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.Server;
using FluentAssertions; 
using System.Diagnostics;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Microsoft.AspNetCore.Routing;

namespace MediatRSample.Tests
{
    public class UtilityServiceTests
    {
        private readonly HttpClient _httpClient;
        private readonly WireMockServer _mockServer;
        private readonly UtilityService _utilityService;

        public UtilityServiceTests()
        {
            _mockServer = WireMockServer.Start();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_mockServer.Urls[0])
            };
            _utilityService = new UtilityService(_httpClient);
        }

        public void Dispose()
        {
            _mockServer.Stop();
            _httpClient.Dispose();
        }

        //Positive Case
        [Fact]
        public async Task GivenValidResident_WhenGetResidentUtilityBalanceByIdAsyncIsInvoked_ThenValidResidentUtilityBalanceIsReturned()
        {
            //Arrange
            var customerId = 23;
            var faker = new ResidentUtilityDtoFaker(customerId);
            var residentBalance = faker.Generate();
            _mockServer.Given(Request.Create().UsingGet().WithPath($"/balances/v2/{customerId}"))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyAsJson(residentBalance));

            //Act
            var balanceResponse = await _utilityService.GetResidentUtilityBalanceByIdAsync(customerId);

            //Assert
            balanceResponse.Should().NotBeNull();
            balanceResponse.ElectricityBalance.Should().Be(residentBalance.ElectricityBalance);
            balanceResponse.TrashBalance.Should().Be(residentBalance.TrashBalance);
            balanceResponse.WaterBalance.Should().Be(residentBalance.WaterBalance);
        }
        //Negative Case
        [Fact]
        public async Task GivenInValidResident_WhenGetResidentUtilityBalanceByIdAsyncIsInvoked_ThenNullIsReturned()
        {
            //Arrange
            var customerId = 42;
            _mockServer.Given(Request.Create().UsingGet().WithPath($"/balances/v2/{customerId}"))
                                 .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.NotFound));

            //Act
            var balanceResponse = await _utilityService.GetResidentUtilityBalanceByIdAsync(customerId);

            //Assert
            balanceResponse.Should().BeNull();
        }

        [Fact]
        public async Task GivenValidResident_WhenGetResidentUtilityBalanceByIdAsyncIsInvoked_ShouldHandleDelayedResponses()
        {
            //Arrange
            var customerId = 23;
            var faker = new ResidentUtilityDtoFaker(customerId);
            var residentBalance = faker.Generate();
            _mockServer.Given(Request.Create().UsingGet().WithPath($"/balances/v2/{customerId}"))
                                 .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK)
                                 .WithBodyAsJson(residentBalance).WithDelay(TimeSpan.FromMilliseconds(200)));

            //Act
            var watch = new Stopwatch();
            watch.Start();
            var balanceResponse = await _utilityService.GetResidentUtilityBalanceByIdAsync(customerId);
            watch.Stop();

            //Assert
            balanceResponse.Should().NotBeNull();
            watch.ElapsedMilliseconds.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GivenValidResident_WhenGetResidentUtilityBalanceByIdAsyncIsInvoked_ShouldHandleFaults()
        {
            //Arrange
            var customerId = 23;
            var faker = new ResidentUtilityDtoFaker(customerId);
            var residentBalance = faker.Generate();
            _mockServer.Given(Request.Create().UsingGet().WithPath($"/balances/v2/{customerId}"))
                                 .RespondWith(Response.Create().WithFault(FaultType.MALFORMED_RESPONSE_CHUNK));

            //Act
            var balanceResponse = await _utilityService.GetResidentUtilityBalanceByIdAsync(customerId);

            //Assert
            balanceResponse.Should().BeNull();
        }
        [Fact]
        public void With_Korean_Locale()
        {
            var lorem = new Bogus.DataSets.Lorem(locale: "ko");
            var s = lorem.Sentence(5);
            Console.WriteLine(lorem.Sentence(5));
        }
    }
}
