using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using WariusWebWernwedienung.Server.Controllers;
using Xunit;

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
        public void Post_StateUnderTest_ExpectedBehavior()
        {
            var sut = CreateRemoteControlController();
            var result = sut.GetLinks().ToList();
        }
    }
}
