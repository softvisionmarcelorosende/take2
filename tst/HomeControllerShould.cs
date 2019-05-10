using System;
using Xunit;
using Take2.Source.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace tst
{
    public class HomeControllerShould
    {
        HomeController _sut;
        public HomeControllerShould()
        {
            _sut = new HomeController();
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
