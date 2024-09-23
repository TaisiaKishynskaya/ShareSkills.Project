using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Tests.Service.Tests
{
    public class SearchServiceTests : TestsService
    {
        private readonly SearchService _searchService;

        public SearchServiceTests()
        {
            _searchService = new SearchService(httpClient);
        }

        [Fact]
        public async Task SearchTeacher_ShouldReturnTeacher_WithCorrectId_WhenSuccesful()
        {
            var skill = "skill";
            var time = "time";
            var level = "level";

            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };

            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/get-teacher?skill={skill}&level={level}&classTime={time}")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = JsonContent.Create("1")
                            });
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(teacher)
                });

            var response = await _searchService.SearchTeacher(skill, time, level);
            Assert.Equal(JsonConvert.SerializeObject(teacher), JsonConvert.SerializeObject(response));
        }

        [Fact]
        public async Task SearchTeacher_ShouldReturnNull_WithIncorrectId_WhenSuccesful()
        {
            var skill = "skill";
            var time = "time";
            var level = "level";

            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };

            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/get-teacher?skill={skill}&level={level}&classTime={time}")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = JsonContent.Create("2")
                            });
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(teacher)
                });

            var response = await _searchService.SearchTeacher(skill, time, level);
            Assert.Null(response);
        }

        [Fact]
        public async Task SearchTeacher_ShouldReturnNull_WhenUnsuccesful()
        {
            var skill = "skill";
            var time = "time";
            var level = "level";

            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };

            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/get-teacher?skill={skill}&level={level}&classTime={time}")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
                            {
                            });
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                });

            var response = await _searchService.SearchTeacher(skill, time, level);
            Assert.Null(response);
        }

        [Fact]
        public async Task GetTeacherById_ShouldReturnTeacher_WhenSuccesful()
        {
            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };
            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/1")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = JsonContent.Create(teacher)
                            });
            var respond = await _searchService.GetTeacherById("1");
            Assert.Equal(JsonConvert.SerializeObject(teacher), JsonConvert.SerializeObject(respond));
        }

        [Fact]
        public async Task GetTeacherById_ShouldReturnNull_WhenUnsuccesful()
        {
            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };
            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/1")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
                            {
                            });
            var respond = await _searchService.GetTeacherById("1");
            Assert.Null(respond);
        }

        [Fact]
        public async Task GetTeachers_ShouldReturnTeachersList_WhenSuccesful()
        {
            var teachers = new List<Teacher> { new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" } };
            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = JsonContent.Create(teachers)
                            });
            var respond = await _searchService.GetTeachers();
            Assert.Equal(JsonConvert.SerializeObject(teachers), JsonConvert.SerializeObject(respond));
        }

        [Fact]
        public async Task GetTeachers_ShouldReturnNull_WhenUnsuccesful()
        {
            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };
            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
                            {
                            });
            var respond = await _searchService.GetTeachers();
            Assert.Null(respond);
        }

        [Fact]
        public async Task GetTeacherByEmail_ShouldReturnTeacher_WhenSuccesful()
        {
            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };
            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/get-by-email/email")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = JsonContent.Create("1")
                            });
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(teacher)
                });
            var respond = await _searchService.GetTeacherByEmail("email");
            Assert.Equal(JsonConvert.SerializeObject(teacher), JsonConvert.SerializeObject(respond));
        }

        [Fact]
        public async Task GetTeacherByEmail_ShouldReturnNull_WhenUnsuccesful()
        {
            var teacher = new Teacher { classTime = "time", email = "email", id = "1", level = "level", name = "name", rating = 1, skill = "skill", surname = "surname" };
            mockHttpMessageHandler
                            .When(HttpMethod.Get, $"{fakeBaseAddres}/teachers/get-by-email/email")
                            .Respond(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
                            {
                            });
            var respond = await _searchService.GetTeacherByEmail("email");
            Assert.Null(respond);
        }
    }
}
