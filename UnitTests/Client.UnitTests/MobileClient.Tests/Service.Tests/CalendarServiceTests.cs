﻿using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MobileClient.Services;

namespace MobileClient.Tests.Service.Tests
{
    public class CalendarServiceTests : TestsService
    {
        private readonly CalendarService _calendarService;

        public CalendarServiceTests()
        {
            mockPreferencesService.Setup(p => p.Get("jwt", It.IsAny<string>())).Returns("1");
            _calendarService = new CalendarService(httpClient, mockPreferencesService.Object);
        }

        [Fact]
        public async Task UpdateCalendar_ShouldReturnMeetingsList_WhenSuccesful()
        {
            var meetings = new List<Meeting>{ new Meeting { Id=new Guid(), Name="name", DateTime=DateTime.Now, Description="desc", OwnerId= new Guid(), ForeignId= new Guid() }};
            var expectedResponseJson = JsonContent.Create(meetings);
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/meetings")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(meetings)
                });
            var response = await _calendarService.UpdateCalendar();
            var expectedResponse = await expectedResponseJson.ReadFromJsonAsync<List<Meeting>>();
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(response));
        }

        [Fact]
        public async Task UpdateCalendar_ShouldReturnNull_WhenUnsuccesful()
        {
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/users/1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                });
            var response = await _calendarService.UpdateCalendar();
            Assert.Null(response);
        }

        [Fact]
        public async Task GetIdByEmail_ShouldReturnId_WhenSuccesful()
        {
            var fakeEmail = "test@email.com";
            var fakeUserId = new GetEmailResponse { userId = "1" };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(fakeUserId)
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, $"{fakeBaseAddres}/getId?email={fakeEmail}")
                .Respond(req => expectedResponse);
            var response = await _calendarService.GetIdByEmail(fakeEmail);
            Assert.Equal("1", response);
        }

        [Fact]
        public async Task GetIdByEmail_ShouldReturnNull_WhenUnsuccesful()
        {
            var fakeEmail = "test@email.com";
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, $"{fakeBaseAddres}/getId?email={fakeEmail}")
                .Respond(req => expectedResponse);
            var response = await _calendarService.GetIdByEmail(fakeEmail);
            Assert.Null(response);
        }

        [Fact]
        public async Task AddMeeting_ShouldReturnTrue_WhenSuccesful()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
            };
            var date = DateTime.Now;
            mockHttpMessageHandler
                .When(HttpMethod.Post, $"{fakeBaseAddres}/meetings")
                .Respond(req => expectedResponse);
            var response = await _calendarService.AddMeeting(date, "email", "title");
            Assert.True(response);
        }

        [Fact]
        public async Task AddMeeting_ShouldReturnFalse_WhenUnsuccesful()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, $"{fakeBaseAddres}/meetings")
                .Respond(req => expectedResponse);
            var response = await _calendarService.AddMeeting(DateTime.Now, "email", "title");
            Assert.False(response);
        }

        [Fact]
        public async Task GetMeetingInfo_ShouldReturnInfo_WithForeignId_WhenSuccesful()
        {
            var fakeId = "1";
            var fakeRole = "90c08b8a-fa4c-445e-9f66-717bf2bfcf72";
            var meeting = new Meeting { Id = new Guid(), Name = "name", DateTime = DateTime.Now, Description = "desc", OwnerId = new Guid(), ForeignId = new Guid() };
            var user = new User { Email = "email", Id = "1", Name = "name", Surname = "surname", PasswordHash = "pass", Role = "student" };
            var expectedMeetingResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(meeting)
            };
            var expectedUserResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(user)
            };


            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/meetings/{fakeId}")
                .Respond(req => expectedMeetingResponse);
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/users/{meeting.ForeignId}")
                .Respond(req => expectedUserResponse);

            var response = await _calendarService.GetMeetingInfo(fakeId, fakeRole);
            Assert.Equal(JsonConvert.SerializeObject((meeting, user)), JsonConvert.SerializeObject(response));
        }

        [Fact]
        public async Task GetMeetingInfo_ShouldReturnInfo_WithOwnerId_WhenSuccesful()
        {
            var fakeId = "1";
            var fakeRole = "another";
            var meeting = new Meeting { Id = new Guid(), Name = "name", DateTime = DateTime.Now, Description = "desc", OwnerId = new Guid(), ForeignId = new Guid() };
            var user = new User { Email = "email", Id = "1", Name = "name", Surname = "surname", PasswordHash = "pass", Role = "student" };
            var expectedMeetingResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(meeting)
            };
            var expectedUserResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(user)
            };


            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/meetings/{fakeId}")
                .Respond(req => expectedMeetingResponse);
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/users/{meeting.OwnerId}")
                .Respond(req => expectedUserResponse);

            var response = await _calendarService.GetMeetingInfo(fakeId, fakeRole);
            Assert.Equal(JsonConvert.SerializeObject((meeting, user)), JsonConvert.SerializeObject(response));
        }

        [Fact]
        public async Task GetMeetingInfo_ShouldReturnNull_WhenUnsuccesful()
        {
            var fakeId = "1";
            var fakeRole = "90c08b8a-fa4c-445e-9f66-717bf2bfcf72";
            var meeting = new Meeting { Id = new Guid(), Name = "name", DateTime = DateTime.Now, Description = "desc", OwnerId = new Guid(), ForeignId = new Guid() };
            var user = new User { Email = "email", Id = "1", Name = "name", Surname = "surname", PasswordHash = "pass", Role = "student" };
            var expectedMeetingResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(meeting)
            };
            var expectedUserResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(user)
            };


            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/meetings/{fakeId}")
                .Respond(req => expectedMeetingResponse);
            mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/users/{meeting.ForeignId}")
                .Respond(req => expectedUserResponse);

            var response = await _calendarService.GetMeetingInfo(fakeId, fakeRole);
            Assert.Null(response);
        }
    }
}