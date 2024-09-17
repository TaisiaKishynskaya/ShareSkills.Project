using App.Infrastructure.Mapping.Endpoints.Concrete;
using App.Services.Concrete;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Server.UnitTests.EndpointsTests;

public class MLTeacherRecommendationEndpointTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<TeacherBinaryTree>> _treeLoggerMock;
        private readonly Mock<ILogger<MLTeacherRecommendationEndpoint>> _endpointLoggerMock;
        private readonly MLTeacherRecommendationEndpoint _endpoint;

        public MLTeacherRecommendationEndpointTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _treeLoggerMock = new Mock<ILogger<TeacherBinaryTree>>();
            _endpointLoggerMock = new Mock<ILogger<MLTeacherRecommendationEndpoint>>();
            _endpoint = new MLTeacherRecommendationEndpoint();
        }

        [Fact]
        public async Task MapRoutes_ReturnsOk_WhenTeacherIsFound()
        {
            // Arrange
            var skill = "Math";
            var level = "Advanced";
            var classTime = "Morning";

            var skillId = Guid.NewGuid();
            var levelId = Guid.NewGuid();
            var classTimeId = Guid.NewGuid();
            var teacherId = Guid.NewGuid();

            var skillEntity = new SkillEntity { Id = skillId, Skill = skill };
            var levelEntity = new LevelEntity { Id = levelId, Name = level };
            var classTimeEntity = new ClassTimeEntity { Id = classTimeId, Name = classTime };

            _unitOfWorkMock.Setup(u => u.SkillRepository.GetTeacherSkillAsync(skill))
                .ReturnsAsync(skillEntity);
            _unitOfWorkMock.Setup(u => u.LevelRepository.GetTeacherLevelAsync(level))
                .ReturnsAsync(levelEntity);
            _unitOfWorkMock.Setup(u => u.ClassTimeRepository.GetTeacherClassTimeAsync(classTime))
                .ReturnsAsync(classTimeEntity);

            var teacherTree = new Mock<TeacherBinaryTree>(_treeLoggerMock.Object, _unitOfWorkMock.Object);
            teacherTree.Setup(t => t.Search(skillId, levelId, classTimeId))
                .Returns(new TeacherEntity { Id = teacherId });

            var routeBuilderMock = new Mock<IEndpointRouteBuilder>();
            routeBuilderMock
                .Setup(rb => rb.MapGet(It.IsAny<string>(), It.IsAny<RequestDelegate>()))
                .Callback<string, RequestDelegate>((_, requestDelegate) =>
                {
                    var httpContext = new DefaultHttpContext();
                    httpContext.Request.Query = new QueryCollection(
                        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                        {
                            ["skill"] = skill,
                            ["level"] = level,
                            ["classTime"] = classTime
                        });
                    httpContext.RequestServices = new ServiceCollection()
                        .AddSingleton(_unitOfWorkMock.Object)
                        .AddSingleton(_treeLoggerMock.Object)
                        .AddSingleton(_endpointLoggerMock.Object)
                        .BuildServiceProvider();

                    // Act
                    var result = requestDelegate(httpContext);
                    result.Wait(); 

                    // Assert
                    Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseBody = new StreamReader(httpContext.Response.Body).ReadToEnd();
                    Assert.Equal(teacherId.ToString(), responseBody);
                });

            // Act
            _endpoint.MapRoutes(routeBuilderMock.Object);
        }
    }
