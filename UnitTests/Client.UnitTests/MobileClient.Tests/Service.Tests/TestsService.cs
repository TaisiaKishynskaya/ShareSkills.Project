using MobileClient.Services;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Tests.Service.Tests
{
    public class TestsService
    {
        protected readonly MockHttpMessageHandler mockHttpMessageHandler;
        protected string fakeBaseAddres;
        protected HttpClient httpClient;
        protected Mock<IPreferencesService> mockPreferencesService;

        public TestsService()
        {
            fakeBaseAddres = "http://localhost:5115";
            mockHttpMessageHandler = new MockHttpMessageHandler();
            httpClient = new HttpClient(mockHttpMessageHandler);
            mockPreferencesService = new Mock<IPreferencesService>();
        }
    }
}
