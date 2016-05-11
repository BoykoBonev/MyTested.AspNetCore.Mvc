﻿namespace MyTested.Mvc.Test.BuildersTests.RoutesTests
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Setups;
    using Setups.Routes;
    using Setups.Startups;
    using Xunit;

    public class RouteTestBuilderTests
    {
        [Fact]
        public void ToActionShouldNotThrowExceptionWithCorrectAction()
        {
            MyMvc
                .Routes()
                .ShouldMap("/")
                .ToAction("Index");
        }

        [Fact]
        public void ToActionShouldThrowExceptionWithIncorrectAction()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/")
                        .ToAction("Home");
                },
                "Expected route '/' to match Home action but in fact matched Index.");
        }

        [Fact]
        public void ToControllerShouldNotThrowExceptionWithCorrectController()
        {
            MyMvc
                .Routes()
                .ShouldMap("/")
                .ToController("Home");
        }

        [Fact]
        public void ToControllerAndActionShouldNotThrowExceptionWithCorrectActionAndController()
        {
            MyMvc
                .Routes()
                .ShouldMap("/")
                .To("Index", "Home");
        }

        [Fact]
        public void ToControllerWithGenericShouldNotThrowExceptionWithCorrectController()
        {
            MyMvc
                .Routes()
                .ShouldMap("/")
                .To<HomeController>();
        }

        [Fact]
        public void ToControllerShouldThrowExceptionWithIncorrectControllerType()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/")
                        .To<RouteController>();
                },
                "Expected route '/' to match RouteController but in fact matched HomeController.");
        }

        [Fact]
        public void ToControllerShouldThrowExceptionWithIncorrectAction()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/")
                        .ToController("Index");
                },
                "Expected route '/' to match Index controller but in fact matched Home.");
        }

        [Fact]
        public void ToRouteValueShouldNotThrowExceptionWithCorrectRouteValueKey()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/Contact/1")
                .ToRouteValue("id");
        }

        [Fact]
        public void ToRouteValueShouldThrowExceptionWithIncorrectRouteValueKey()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Home/Contact/1")
                        .ToRouteValue("name");
                },
                "Expected route '/Home/Contact/1' to contain route value with 'name' key but such was not found.");
        }

        [Fact]
        public void ToRouteValueShouldNotThrowExceptionWithCorrectRouteValue()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/Contact/1")
                .ToRouteValue("id", 1);
        }

        [Fact]
        public void ToRouteValueShouldThrowExceptionWithIncorrectRouteValue()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Home/Contact/1")
                        .ToRouteValue("id", 2);
                },
                "Expected route '/Home/Contact/1' to contain route value with 'id' key and the provided value but the value was different.");
        }

        [Fact]
        public void ToRouteValuesShouldNotThrowExceptionWithCorrectRouteValues()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/Contact/1")
                .ToRouteValues(new { controller = "Home", action = "Contact", id = 1 });
        }

        [Fact]
        public void ToRouteValuesShouldNotMakeCountCheckWithProvidedLambda()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/Contact/1")
                .To<HomeController>(c => c.Contact(1))
                .AndAlso()
                .ToRouteValues(new { id = 1 });
        }

        [Fact]
        public void ToRouteValuesShouldNotThrowExceptionWithCorrectRouteValuesAsDictionary()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/Contact/1")
                .ToRouteValues(new Dictionary<string, object>
                {
                    ["controller"] = "Home",
                    ["action"] = "Contact",
                    ["id"] = 1
                });
        }

        [Fact]
        public void ToRouteValuesShouldThrowExceptionWithIncorrectRouteValues()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Home/Contact/1")
                        .ToRouteValues(new { controller = "Home", action = "Index", id = 1 });
                },
                "Expected route '/Home/Contact/1' to contain route value with 'action' key and the provided value but the value was different.");
        }

        [Fact]
        public void ToRouteValuesShouldThrowExceptionWithIncorrectRouteValuesWithSingleCountError()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Home/Contact/1")
                        .ToRouteValues(new { id = 1 });
                },
                "Expected route '/Home/Contact/1' to contain 1 route value but in fact found 3.");
        }

        [Fact]
        public void ToRouteValuesShouldThrowExceptionWithIncorrectRouteValuesWithMultipleCountError()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Home/Contact/1")
                        .ToRouteValues(new { id = 1, query = "invalid", another = "another", fourth = "test" });
                },
                "Expected route '/Home/Contact/1' to contain 4 route values but in fact found 3.");
        }

        [Fact]
        public void ToDataTokenShouldNotThrowExceptionWithCorrectDataTokenKey()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/Test")
                .ToDataToken("random");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokenShouldThrowExceptionWithIncorrectDataTokenKey()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Test")
                        .ToDataToken("name");
                },
                "Expected route '/Test' to contain data token with 'name' key but such was not found.");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokenShouldNotThrowExceptionWithCorrectDataToken()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/Test")
                .ToDataToken("random", "value");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokenShouldThrowExceptionWithIncorrectDataToken()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Test")
                        .ToDataToken("random", 2);
                },
                "Expected route '/Test' to contain data token with 'random' key and the provided value but the value was different.");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokensShouldNotThrowExceptionWithCorrectDataTokens()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/Test")
                .ToDataTokens(new { random = "value", another = "token" });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokensShouldNotThrowExceptionWithCorrectDataTokensAsDictionary()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/Test")
                .ToDataTokens(new Dictionary<string, object>
                {
                    ["random"] = "value",
                    ["another"] = "token"
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokensShouldThrowExceptionWithIncorrectDataTokens()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Test")
                        .ToDataTokens(new { random = "value", another = "invalid" });
                },
                "Expected route '/Test' to contain data token with 'another' key and the provided value but the value was different.");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokenShouldThrowExceptionWithIncorrectDataTokensWithSingleCountError()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Test")
                        .ToDataTokens(new { id = 1 });
                },
                "Expected route '/Test' to contain 1 data token but in fact found 2.");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToDataTokensShouldThrowExceptionWithIncorrectDataTokensWithMultipleCountError()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Test")
                        .ToDataTokens(new { id = 1, query = "invalid", another = "another", fourth = "test" });
                },
                "Expected route '/Test' to contain 4 data tokens but in fact found 2.");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithEmptyPath()
        {
            MyMvc
                .Routes()
                .ShouldMap("/")
                .To<HomeController>(c => c.Index());
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndAction()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/AsyncMethod")
                .To<HomeController>(c => c.AsyncMethod());
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionIfNoLocationIsSet()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request.WithMethod(HttpMethod.Post))
                .To<HomeController>(c => c.Index());
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithPartialPath()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home")
                .To<HomeController>(c => c.Index());
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithNormalPath()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/Index")
                .To<HomeController>(c => c.Index());
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithNormalPathAndRouteParameter()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Home/Contact/1")
                .To<HomeController>(c => c.Contact(1));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithNoModel()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Normal/ActionWithMultipleParameters/1")
                .To<NormalController>(c => c.ActionWithMultipleParameters(1, With.No<string>(), With.No<RequestModel>()));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithQueryString()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Normal/ActionWithMultipleParameters/1?text=test")
                .To<NormalController>(c => c.ActionWithMultipleParameters(1, "test", With.No<RequestModel>()));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithRequestModelAsString()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithLocation("/Normal/ActionWithMultipleParameters/1")
                    .WithMethod(HttpMethod.Post)
                    .WithJsonBody(@"{""Integer"":1,""String"":""Text""}"))
                .To<NormalController>(c => c.ActionWithMultipleParameters(
                    1,
                    With.No<string>(),
                    new RequestModel
                    {
                        Integer = 1,
                        String = "Text"
                    }));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithRequestModelAsInstance()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithLocation("/Normal/ActionWithMultipleParameters/1")
                    .WithMethod(HttpMethod.Post)
                    .WithJsonBody(new RequestModel
                    {
                        Integer = 1,
                        String = "Text"
                    }))
                .To<NormalController>(c => c.ActionWithMultipleParameters(
                    1,
                    With.No<string>(),
                    new RequestModel
                    {
                        Integer = 1,
                        String = "Text"
                    }));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithRequestModelAsObject()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithLocation("/Normal/ActionWithMultipleParameters/1")
                    .WithMethod(HttpMethod.Post)
                    .WithJsonBody(new
                    {
                        Integer = 1,
                        String = "Text"
                    }))
                .To<NormalController>(c => c.ActionWithMultipleParameters(
                    1,
                    With.No<string>(),
                    new RequestModel
                    {
                        Integer = 1,
                        String = "Text"
                    }));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithActionNameAttribute()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Normal/AnotherName")
                .To<NormalController>(c => c.ActionWithChangedName());
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithRouteAttributes()
        {
            MyMvc
                .Routes()
                .ShouldMap("/AttributeController/AttributeAction")
                .To<RouteController>(c => c.Index());
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithRouteAttributesWithParameter()
        {
            MyMvc
                .Routes()
                .ShouldMap("/AttributeController/Action/1")
                .To<RouteController>(c => c.Action(1));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithPocoController()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Poco/Action/1")
                .To<PocoController>(c => c.Action(1));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithEmptyArea()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/Files")
                .To<DefaultController>(c => c.Test("None"));

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithNonEmptyArea()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/Files/Default/Download/Test")
                .To<DefaultController>(c => c.Download("Test"));

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithDefaultValues()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/CustomRoute")
                .To<NormalController>(c => c.FromRouteAction(new RequestModel { Integer = 1, String = "test" }));

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithRouteConstraints()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Normal/ActionWithConstraint/5")
                .To<NormalController>(c => c.ActionWithConstraint(5));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithConventions()
        {
            MyMvc
                .Routes()
                .ShouldMap("/ChangedController/ChangedAction?ChangedParameter=1")
                .To<ConventionsController>(c => c.ConventionsAction(1));
        }

        [Fact]
        public void ToShouldThrowExceptionWithInvalidRoute()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/Normal/ActionWithModel/1")
                        .To<HomeController>(c => c.Index());
                },
                "Expected route '/Normal/ActionWithModel/1' to match Index action in HomeController but action could not be matched.");
        }

        [Fact]
        public void ToShouldThrowExceptionWithDifferentAction()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/")
                        .To<HomeController>(c => c.Contact(1));
                },
                "Expected route '/' to match Contact action in HomeController but instead matched Index action.");
        }

        [Fact]
        public void ToShouldThrowExceptionWithDifferentController()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/")
                        .To<RouteController>(c => c.Index());
                },
                "Expected route '/' to match Index action in RouteController but instead matched HomeController.");
        }

        [Fact]
        public void ToShouldResolveNonExistingRouteWithInvalidGetMethod()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Normal/ActionWithModel/1")
                .ToNonExistingRoute();
        }

        [Fact]
        public void ToNonExistingRouteShouldThrowExceptionWithValidOne()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap("/")
                        .ToNonExistingRoute();
                },
                "Expected route '/' to be non-existing but in fact it was resolved successfully.");
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithCorrectHttpMethod()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Normal/ActionWithModel/1"))
                .To<NormalController>(c => c.ActionWithModel(1, With.No<RequestModel>()));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithFromRouteAction()
        {
            MyMvc.StartsFrom<RoutesStartup>();

            MyMvc
                .Routes()
                .ShouldMap("/CustomRoute")
                .To<NormalController>(c => c.FromRouteAction(new RequestModel
                {
                    Integer = 1,
                    String = "test"
                }));

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithFromQueryAction()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Normal/FromQueryAction?Integer=1&String=test")
                .To<NormalController>(c => c.FromQueryAction(new RequestModel
                {
                    Integer = 1,
                    String = "test"
                }));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithFromFormAction()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithLocation("/Normal/FromFormAction")
                    .WithFormField("Integer", "1")
                    .WithFormField("String", "test"))
                .To<NormalController>(c => c.FromFormAction(new RequestModel
                {
                    Integer = 1,
                    String = "test"
                }));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithFromHeaderAction()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithLocation("/Normal/FromHeaderAction")
                    .WithHeader("MyHeader", "MyHeaderValue"))
                .To<NormalController>(c => c.FromHeaderAction("MyHeaderValue"));
        }

        [Fact]
        public void ToShouldResolveCorrectControllerAndActionWithFromServicesAction()
        {
            MyMvc
                .Routes()
                .ShouldMap("/Normal/FromServicesAction")
                .To<NormalController>(c => c.FromServicesAction(From.Services<IActionSelector>()));
        }

        [Fact]
        public void ShouldMapWithRequestShouldWorkCorrectly()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());
            request.Path = "/Normal/FromServicesAction";

            MyMvc
                .Routes()
                .ShouldMap(request)
                .To<NormalController>(c => c.FromServicesAction(From.Services<IActionSelector>()));
        }

        [Fact]
        public void ShouldMapWithUriShouldWorkCorrectly()
        {
            MyMvc
                .Routes()
                .ShouldMap(new Uri("/Normal/FromServicesAction", UriKind.Relative))
                .To<NormalController>(c => c.FromServicesAction(From.Services<IActionSelector>()));
        }

        [Fact]
        public void ToValidModelStateShouldNotThrowExceptionWithValidModelState()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithPath("/Normal/ActionWithModel/5")
                    .WithMethod(HttpMethod.Post)
                    .WithJsonBody(@"{""Integer"":5,""String"":""Test""}"))
                .ToValidModelState();
        }

        [Fact]
        public void ToValidModelStateShouldThrowExceptionWithUnresolvedAction()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap(request => request
                            .WithPath("/Normal/Invalid/5")
                            .WithMethod(HttpMethod.Post)
                            .WithJsonBody(@"{""Integer"":5,""String"":""Test""}"))
                        .ToValidModelState();
                },
                "Expected route '/Normal/Invalid/5' to have valid model state with no errors but action could not be matched.");
        }

        [Fact]
        public void ToValidModelStateShouldThrowExceptionWithInvalidModelState()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap(request => request
                            .WithPath("/Normal/ActionWithModel/5")
                            .WithMethod(HttpMethod.Post)
                            .WithJsonBody(@"{""Integer"":5}"))
                        .ToValidModelState();
                },
                "Expected route '/Normal/ActionWithModel/5' to have valid model state with no errors but it had some.");
        }

        [Fact]
        public void ToInvalidModelStateShouldNotThrowExceptionWithInvalidModelState()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithPath("/Normal/ActionWithModel/5")
                    .WithMethod(HttpMethod.Post)
                    .WithJsonBody(@"{""Integer"":5}"))
                .ToInvalidModelState();
        }
        
        [Fact]
        public void ToInvalidModelStateShouldNotThrowExceptionWithInvalidModelStateAndCorrectNumberOfErrors()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithPath("/Normal/ActionWithModel/5")
                    .WithMethod(HttpMethod.Post)
                    .WithJsonBody(@"{""Integer"":5}"))
                .ToInvalidModelState(withNumberOfErrors: 1);
        }

        [Fact]
        public void ToInvalidModelStateShouldThrowExceptionWithValidModelState()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap(request => request
                            .WithPath("/Normal/ActionWithModel/5")
                            .WithMethod(HttpMethod.Post)
                            .WithJsonBody(@"{""Integer"":5,""String"":""Test""}"))
                        .ToInvalidModelState();
                },
                "Expected route '/Normal/ActionWithModel/5' to have invalid model state but was in fact valid.");
        }
        
        [Fact]
        public void ToInvalidModelStateShouldThrowExceptionWithIncorrectNumberOfErrors()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap(request => request
                            .WithPath("/Normal/ActionWithModel/5")
                            .WithMethod(HttpMethod.Post)
                            .WithJsonBody(@"{""Integer"":5}"))
                        .ToInvalidModelState(withNumberOfErrors: 3);
                },
                "Expected route '/Normal/ActionWithModel/5' to have invalid model state with 3 errors but in fact contained 1.");
        }
        
        [Fact]
        public void ToInvalidModelStateShouldThrowExceptionWithIncorrectOneNumberOfErrors()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap(request => request
                            .WithPath("/Normal/ActionWithModel/5")
                            .WithMethod(HttpMethod.Post)
                            .WithJsonBody(@"{""Integer"":5,""String"":""Test""}"))
                        .ToInvalidModelState(withNumberOfErrors: 1);
                },
                "Expected route '/Normal/ActionWithModel/5' to have invalid model state with 1 error but in fact contained 0.");
        }

        [Fact]
        public void ToInvalidModelStateShouldThrowExceptionWithUnresolvedAction()
        {
            Test.AssertException<RouteAssertionException>(
                () =>
                {
                    MyMvc
                        .Routes()
                        .ShouldMap(request => request
                            .WithPath("/Normal/Invalid/5")
                            .WithMethod(HttpMethod.Post)
                            .WithJsonBody(@"{""Integer"":5,""String"":""Test""}"))
                        .ToInvalidModelState();
                },
                "Expected route '/Normal/Invalid/5' to have invalid model state but action could not be matched.");
        }

        [Fact]
        public void UltimateCrazyModelBindingTest()
        {
            MyMvc
                .Routes()
                .ShouldMap(request => request
                    .WithLocation("/Normal/UltimateModelBinding/100?myQuery=Test")
                    .WithMethod(HttpMethod.Post)
                    .WithJsonBody(new
                    {
                        Integer = 1,
                        String = "MyBodyValue"
                    })
                    .WithFormField("MyField", "MyFieldValue")
                    .WithHeader("MyHeader", "MyHeaderValue"))
                .To<NormalController>(c => c.UltimateModelBinding(
                    new ModelBindingModel
                    {
                        Body = new RequestModel { Integer = 1, String = "MyBodyValue" },
                        Form = "MyFieldValue",
                        Route = 100,
                        Query = "Test",
                        Header = "MyHeaderValue"
                    },
                    From.Services<IUrlHelperFactory>()));
        }
    }
}
