using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using WariusWebWernwedienung.Server.Controllers;

namespace Unittests.Server.Controllers
{
    public class RemoteControlControllerTests
    {
        private readonly IConfiguration _subConfiguration;

        public RemoteControlControllerTests()
        {
            _subConfiguration = Substitute.For<IConfiguration>();
            _subConfiguration.GetConnectionString("").ReturnsForAnyArgs("D:\\Scripts");
        }

        private RemoteControlController CreateRemoteControlController()
        {
            return new RemoteControlController(_subConfiguration);
        }

        [Fact]
        public async Task GetLinks_ShouldWork()
        {
            var sut = CreateRemoteControlController();
            var result = (await sut.GetLinks()).ToList();
            result.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Navigation_ShouldWork()
        {
            var sut = CreateRemoteControlController();
            var result = (await sut.GetLinks()).ToList();
            await sut.Navigate(result[0]);
        }
    }
}
