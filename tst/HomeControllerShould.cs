using System;
using Xunit;
using Take2.Source.Controllers;
using Microsoft.AspNetCore.Mvc;
using Take2.Source.Models;
using Microsoft.Extensions.Options;
using Moq;
namespace tst
{
    public class HomeControllerShould
    {
        HomeController _sut;
        public HomeControllerShould()
        {
            var settings = new Settings()
            {
                BackgroundColor = "White",
                FontColor = "Black",
                FontSize = 72,
                Message = "Welcome To Text"
            };

            var mock = new Mock<IOptionsSnapshot<Settings>>();
            mock.Setup(m => m.Value).Returns(settings);

            _sut = new HomeController(mock.Object);
        }
        [Fact]
        public void ShowHome()
        {
            var result = _sut.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ShowPrivacy()
        {
            var result = _sut.Privacy();
            Assert.IsType<ViewResult>(result);
        }
    }
}
