﻿namespace MusicStore.Test.Controllers
{
    using Microsoft.Extensions.Caching.Memory;
    using Mocks;
    using Models;
    using MusicStore.Controllers;
    using MyTested.Mvc;
    using Xunit;

    using HttpMethod = System.Net.Http.HttpMethod;

    public class AccountControllerTest
    {
        [Fact]
        public void AccountControllerShouldHaveAuthorizeFilter()
        {
            MyMvc
                .Controller<AccountController>()
                .ShouldHave()
                .Attributes(attrs => attrs
                    .RestrictingForAuthorizedRequests());
        }

        [Fact]
        public void GetLoginShouldHaveAllowAnonymousFilter()
        {
            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(With.No<string>()))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .AllowingAnonymousRequests());
        }

        [Fact]
        public void GetLoginShouldHaveCorrectViewBagEntriesWithReturnUrlAndShouldReturnCorrectView()
        {
            const string returnUrl = "MyReturnUrl";

            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(returnUrl))
                .ShouldHave()
                .ViewBag(viewBag => viewBag
                    .ContainingEntry("ReturnUrl", returnUrl))
                .AndAlso()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void PostLoginShouldHaveCorrectActionFilters()
        {
            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(
                    new LoginViewModel { Email = "Test", Password = "Test" },
                    With.No<string>()))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .AllowingAnonymousRequests()
                    .ValidatingAntiForgeryToken());
        }

        [Fact]
        public void PostLoginShouldReturnDefaultViewWithInvalidModel()
        {
            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(
                    With.Default<LoginViewModel>(),
                    With.No<string>()))
                .ShouldHave()
                .ModelStateFor<LoginViewModel>(modelState => modelState
                    .ContainingErrorFor(m => m.Email)
                    .ContainingErrorFor(m => m.Password))
                .AndAlso()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void PostLoginShouldReturnRedirectToActionWithValidUserName()
        {
            var model = new LoginViewModel
            {
                Email = MockedSignInManager.ValidUser,
                Password = MockedSignInManager.ValidUser
            };
            
            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(
                    model,
                    With.No<string>()))
                .ShouldReturn()
                .Redirect()
                .To<HomeController>(c => c.Index(
                    With.No<MusicStoreContext>(),
                    With.No<IMemoryCache>()));
        }
        
        [Fact]
        public void PostLoginShouldReturnRedirectToLocalWithValidUserNameAndReturnUrl()
        {
            var model = new LoginViewModel
            {
                Email = MockedSignInManager.ValidUser,
                Password = MockedSignInManager.ValidUser
            };

            var returnUrl = "/Store/Index";

            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(
                    model,
                    returnUrl))
                .ShouldReturn()
                .Redirect()
                .ToUrl(returnUrl);
        }
        
        [Fact]
        public void PostLoginShouldReturnRedirectWithTwoFactor()
        {
            var model = new LoginViewModel
            {
                Email = MockedSignInManager.TwoFactorRequired,
                Password = MockedSignInManager.TwoFactorRequired,
                RememberMe = true
            };

            var returnUrl = "/Store/Index";

            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(
                    model,
                    returnUrl))
                .ShouldReturn()
                .Redirect()
                .To<AccountController>(c => c.SendCode(model.RememberMe, returnUrl));
        }
        
        [Fact]
        public void PostLoginShouldReturnViewWithLockout()
        {
            var model = new LoginViewModel
            {
                Email = MockedSignInManager.LockedOutUser,
                Password = MockedSignInManager.LockedOutUser
            };

            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(
                    model,
                    With.No<string>()))
                .ShouldReturn()
                .View("Lockout");
        }

        [Fact]
        public void PostLoginShouldReturnReturnViewWithInvalidCredentials()
        {
            var model = new LoginViewModel
            {
                Email = "Invalid@invalid.com",
                Password = "Invalid"
            };

            MyMvc
                .Controller<AccountController>()
                .Calling(c => c.Login(
                    model,
                    With.No<string>()))
                .ShouldHave()
                .ModelStateFor<ValidationSummary>(modelState => modelState
                    .ContainingError(string.Empty)
                    .ThatEquals("Invalid login attempt."))
                .AndAlso()
                .ShouldReturn()
                .View(model);
        }
    }
}