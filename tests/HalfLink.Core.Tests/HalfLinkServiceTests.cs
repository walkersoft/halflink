using HalfLink.Core.Entities;
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
            var link = await service.CreateLink(url);

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
            var action = async () => await service.CreateLink(url);

            await action.ShouldThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GivenValidUrl_WhenSubmitted_SubmitsToLinkRepo()
        {
            var link = await service.CreateLink("https://example.com");

            repoMock.Verify(repo => repo.CreateLink(link), Times.Once);
        }

        [Fact]
        public async Task GivenValidUrl_WhenSubmitted_ChecksForExistingHalfLink()
        {
            var link = await service.CreateLink("https://example.com");

            repoMock.Verify(repo => repo.HalfLinkExists(link.HalfLink), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenValidUrl_WhenUnableToGenerateHalfLink_ThrowsException()
        {
            repoMock.Setup(repo => repo.HalfLinkExists(It.IsAny<string>())).ReturnsAsync(true);

            var action = async () => await service.CreateLink("https://example.com");

            await action.ShouldThrowAsync<InvalidOperationException>();
            repoMock.Verify(repo => repo.HalfLinkExists(It.IsAny<string>()), Times.Exactly(3));
            repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GivenExistingHalfLink_WhenFetched_ReturnsLink()
        {
            var existingLink = new Link
            {
                Id = Guid.NewGuid(),
                FullLink = "https://example.com",
                HalfLink = "abc12345"
            };
            repoMock.Setup(repo => repo.HalfLinkExists(existingLink.HalfLink)).ReturnsAsync(true);
            repoMock.Setup(repo => repo.GetLink(existingLink.HalfLink)).ReturnsAsync(existingLink);

            var link = await service.GetLink(existingLink.HalfLink);

            link.ShouldNotBeNull();
            link.FullLink.ShouldBe(existingLink.FullLink);
            repoMock.Verify(repo => repo.HalfLinkExists(existingLink.HalfLink), Times.Once);
            repoMock.Verify(repo => repo.GetLink(existingLink.HalfLink), Times.Once);
            repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GivenNonExistingHalfLink_WhenFetched_ReturnsNull()
        {
            var halfLink = "abc12345";
            repoMock.Setup(repo => repo.HalfLinkExists(halfLink)).ReturnsAsync(true);

            var link = await service.GetLink(halfLink);

            link.ShouldBeNull();
            repoMock.Verify(repo => repo.HalfLinkExists(halfLink), Times.Once);
            repoMock.VerifyNoOtherCalls();
        }
    }
}
