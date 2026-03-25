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

        [Fact]
        public async Task GivenValidUrl_WhenSubmitted_CreatesCompleteHalfLink()
        {
            var url = "https://example.com";

            var link = await service.CreateHalfLink(url);

            link.ShouldNotBeNull();
            link.Id.ShouldNotBe(default);
            link.FullLink.ShouldBe(url);
            link.HalfLink.Length.ShouldBe(8);
        }

        [Theory]
        [InlineData("")]
        [InlineData("foobar")]
        [InlineData("http://invalid-url")]
        [InlineData("https://www.invalid-url")]
        public async Task GivenInvalidUrl_WhenSubmitted_ThrowsException(string url)
        {
            var action = async () => await service.CreateHalfLink(url);

            await action.ShouldThrowAsync<ArgumentException>();
        }
    }
}
