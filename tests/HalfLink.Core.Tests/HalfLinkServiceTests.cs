using Moq;
using Shouldly;

namespace HalfLink.Core.Tests
{
    public class HalfLinkServiceTests
    {
        private readonly HalfLinkService service;
        private readonly Mock<IHalfLinkRepository> repoMock;

        public HalfLinkServiceTests()
        {
            repoMock = new Mock<IHalfLinkRepository>();
            service = new HalfLinkService(repoMock.Object);
        }

        [Theory]
        [InlineData("https://example.com")]
        [InlineData("http://example.com")]
        [InlineData("www.example.com")]
        [InlineData("example.com")]
        [InlineData("https://example.com/segment")]
        [InlineData("http://example.com/segment")]
        [InlineData("www.example.com/segment")]
        [InlineData("example.com/segment")]
        [InlineData("https://example.com/?query=param")]
        [InlineData("http://example.com/?query=param")]
        [InlineData("www.example.com/?query=param")]
        [InlineData("example.com/?query=param")]
        public async Task GivenValidUrl_WhenSubmitted_CreatesCompleteHalfLink(string url)
        {
            var link = await service.CreateHalfLink(url);

            link.ShouldNotBeNull();
            link.Id.ShouldNotBe(default);
            link.FullLink.ShouldStartWith("http");
            link.HalfLink.Length.ShouldBe(8);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("http://")]
        [InlineData("http://.foobar")]
        public async Task GivenInvalidUrl_WhenSubmitted_ThrowsException(string url)
        {
            var action = async () => await service.CreateHalfLink(url);

            await action.ShouldThrowAsync<ArgumentException>();
        }
    }
}
