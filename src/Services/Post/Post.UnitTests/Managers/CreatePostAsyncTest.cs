using Moq;
using Post.Api.Application.Models;
using Post.Api.Database.Entities;
using Post.Api.Infrastructure.Exceptions;
using Post.Api.Infrastructure.MessageBroker;
using Post.Api.Interfaces;
using Post.Api.Managers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Post.Managers.UnitTests
{
    public class CreatePostAsyncTest
    {
        private readonly Mock<IAuthorManager> _authorManager;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IPublisher> _publisher;
        private readonly PostCreateRequest _fakePostCreateRequest;
        private readonly AuthorResponse _fakeAuthorResponse;
        private readonly DateTime CreationTime;

        public CreatePostAsyncTest()
        {
            _authorManager = new Mock<IAuthorManager>();
            _postRepository = new Mock<IPostRepository>();
            _publisher = new Mock<IPublisher>();
            CreationTime = DateTime.UtcNow;
            _fakeAuthorResponse = new AuthorResponse
            {
                FirstName = "Aaron",
                LastName = "Cesaro",
                NickName = "nan01",
                EmailAddress = "fakeAuthor@mail.com",
                Biography = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent pellentesque urna quis elit eleifend cursus. Donec euismod magna quam, quis malesuada mi dictum a",
                CreatedAt = CreationTime,
                UpdatedAt = CreationTime
            };
            _fakePostCreateRequest = new PostCreateRequest
            {
                Title = "Fake Analisys",
                Summary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent pellentesque urna quis elit eleifend cursus. Donec euismod magna quam, quis malesuada mi dictum a. Donec euismod felis tincidunt.",
                ImageUrl = "https://pbs.twimg.com/profile_images/1021763565194235904/ElgtjZDA_400x400.jpg",
                PostData = new PostData
                {
                    CompanyDescription = new Api.Database.Models.CompanyDescription
                    {
                        Symbol = "KOD.L",
                        Name = "Kodal Minerls",
                        Description = "Kodal Minerals PLC (LON:KOD) is a junior explorer with a focus on West Africa, and in particular lithium and gold. The company is run by experienced exploration geologist Bernard Aylward, helped by Robert Wooldridge, who is a partner in UK broking house SP Angel. Steve Zaninovich became project manager in November 2018 to oversee the Bougouni Lithium project feasibility study and development plan. Steve is a Mining and Construction engineer with extensive direct experience in West Africa having been involved in projects in Ghana, Mauritania, Senegal, Burkina Faso, Cote d’Ivoire and Mali. He was previously Business Development manager for Lycopodium in Australia and has been on the board of Gryphon Resources, Indiana Resources and Maximus Resources",
                        Founded = "2009",
                        Exchange = "LON",
                        Sector = "Materials",
                        Industry = "Metal Mining",
                        Address = "Prince Frederick House 35 - 39 Maddox Street W1S 2PP United Kingdom",
                        PhoneNumber = "+44 2034632260",
                        WebsiteUrl = "https://www.kodalminerals.com/",
                        MarketCap = 34.630M,
                        Employees = 150,
                        Volume = 2_810_887_631,
                        AverageVolume = 13884.92M,
                        SharesOutstanding = 15_810_000,
                        FloatShares = 8_123_000,
                        LastPrice = 0.22M,
                        Beta = 1.66M,
                        PeRatio = 0,
                        Eps = 0
                    },
                    CompanyThesis = new Api.Database.Models.CompanyThesis(),
                    CompanyValuation = new Api.Database.Models.CompanyValuation(),
                    CompanyFinancials = new Api.Database.Models.CompanyFinancials(),
                    CompanyOther = new Api.Database.Models.CompanyOther(),
                }
            };
        }

        [Fact]
        public async Task Create_post_throws_exception_when_author_not_found()
        {
            // Arrange
            _authorManager.Setup(x => x.GetAuthorByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new AuthorNotFoundException());
            _postRepository.Setup(x => x.InsertPostAsync(It.IsAny<PostEntity>())).ReturnsAsync(Guid.NewGuid());

            // Act
            var postManager = new PostManager(_postRepository.Object, _authorManager.Object, _publisher.Object);

            // Assert
            await Assert.ThrowsAsync<AuthorNotFoundException>(() => postManager.CreatePostAsync(_fakePostCreateRequest));
        }

        [Fact]
        public async Task Create_post_throws_exception_when_post_not_inserted()
        {
            // Arrange
            _authorManager.Setup(x => x.GetAuthorByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new AuthorResponse());
            _postRepository.Setup(x => x.InsertPostAsync(It.IsAny<PostEntity>())).ThrowsAsync(new Exception());

            // Act
            var postManager = new PostManager(_postRepository.Object, _authorManager.Object, _publisher.Object);

            // Assert
            await Assert.ThrowsAsync<PostNotProcessedException>(() => postManager.CreatePostAsync(_fakePostCreateRequest));
        }

        [Fact]
        public async Task Create_post_is_valid()
        {
            // Arrange
            var fakeAuthorId = Guid.NewGuid();
            var fakepostId = Guid.NewGuid();
            _fakeAuthorResponse.Id = fakeAuthorId;
            _authorManager.Setup(x => x.GetAuthorByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_fakeAuthorResponse);
            _postRepository.Setup(x => x.InsertPostAsync(It.IsAny<PostEntity>())).ReturnsAsync(fakepostId);

            // Act
            var postManager = new PostManager(_postRepository.Object, _authorManager.Object, _publisher.Object);
            var processedPostId = await postManager.CreatePostAsync(_fakePostCreateRequest);

            // Assert
            Assert.Equal(fakepostId.ToString(), processedPostId.ToString());
        }
    }
}