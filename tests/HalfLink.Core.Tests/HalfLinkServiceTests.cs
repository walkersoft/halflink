using HalfLink.Core.Entities;
using Moq;
using Shouldly;

namespace HalfLink.Core.Tests
{
    public class HalfLinkServiceTests
    {
        private HalfLinkService service;
        private readonly Mock<IHalfLinkRepository> linkRepoMock;
        private readonly Mock<IHalfLinkStatRepository> statRepoMock;

        public HalfLinkServiceTests()
        {
            linkRepoMock = new Mock<IHalfLinkRepository>();
            statRepoMock = new Mock<IHalfLinkStatRepository>();
            service = new HalfLinkService(linkRepoMock.Object, statRepoMock.Object);
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

        [Theory]
        [InlineData(" https://example.com")]
        [InlineData("https://example.com ")]
        [InlineData(" https://example.com ")]
        public async Task GivenValidUrlWithSurroundingWhitesspace_WhenSubmitted_HasWhiteSpaceRemoved(string url)
        {
            var link = await service.CreateLink(url);

            link.ShouldNotBeNull();
            link.FullLink.ShouldBe("https://example.com");
        }

        [Fact]
        public async Task GivenValidUrl_WhenSubmitted_SubmitsToLinkRepo()
        {
            var link = await service.CreateLink("https://example.com");

            linkRepoMock.Verify(repo => repo.CreateLink(link), Times.Once);
        }

        [Fact]
        public async Task GivenValidUrl_WhenSubmitted_ChecksForExistingHalfLink()
        {
            var link = await service.CreateLink("https://example.com");

            linkRepoMock.Verify(repo => repo.HalfLinkExists(link.HalfLink), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenValidUrl_WhenSubmitted_CreatesFirstStatEntry()
        {
            var link = await service.CreateLink("https://example.com");

            statRepoMock.Verify(repo => repo.CreateStat(It.IsAny<LinkStat>()), Times.Once);
        }

        [Fact]
        public async Task GivenValidUrl_WhenUnableToGenerateHalfLink_ThrowsException()
        {
            linkRepoMock.Setup(repo => repo.HalfLinkExists(It.IsAny<string>())).ReturnsAsync(true);

            var action = async () => await service.CreateLink("https://example.com");

            await action.ShouldThrowAsync<InvalidOperationException>();
            linkRepoMock.Verify(repo => repo.HalfLinkExists(It.IsAny<string>()), Times.Exactly(3));
            linkRepoMock.VerifyNoOtherCalls();
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
            linkRepoMock.Setup(repo => repo.GetLink(existingLink.HalfLink)).ReturnsAsync(existingLink);

            var link = await service.GetLink(existingLink.HalfLink);

            link.ShouldNotBeNull();
            link.FullLink.ShouldBe(existingLink.FullLink);
            linkRepoMock.Verify(repo => repo.GetLink(existingLink.HalfLink), Times.Once);
            linkRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GivenNonExistingHalfLink_WhenFetched_ReturnsNull()
        {
            var halfLink = "abc12345";
            linkRepoMock.Setup(repo => repo.GetLink(halfLink)).ReturnsAsync((Link?)null);

            var link = await service.GetLink(halfLink);

            link.ShouldBeNull();
            linkRepoMock.Verify(repo => repo.GetLink(halfLink), Times.Once);
            linkRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GivenNonExistingHalfLink_WhenFetchingStats_ReturnsEmpty()
        {
            var halfLink = "abc12345";
            linkRepoMock.Setup(repo => repo.GetLink(halfLink)).ReturnsAsync((Link?)null);
            statRepoMock.Setup(repo => repo.GetStats(It.IsAny<Guid>())).ReturnsAsync([]);

            var stats = await service.GetStats(halfLink);

            stats.ShouldNotBeNull();
            stats.ShouldBeEmpty();
            linkRepoMock.Verify(repo => repo.GetLink(halfLink), Times.Once);
            statRepoMock.Verify(repo => repo.GetStats(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GivenHalfLinkWasCreated_WhenFetchingStats_ReturnsStats()
        {
            var linkStub = new StubHalfLinkRepository();
            var statStub = new StubHalfLinkStatRepository();
            service = new HalfLinkService(linkStub, statStub);

            var link = await service.CreateLink("https://example.com");
            var stats = await service.GetStats(link.HalfLink);

            stats.ShouldNotBeNull();
            stats.Count().ShouldBe(1);
            var stat = stats.First();
            stat.Id.ShouldNotBe(default);
            stat.LinkId.ShouldBe(link.Id);
            stat.AccessedAt.ShouldBeGreaterThan(DateTime.MinValue);
            stat.Referrer.ShouldNotBeNullOrEmpty();
        }
    }
}
