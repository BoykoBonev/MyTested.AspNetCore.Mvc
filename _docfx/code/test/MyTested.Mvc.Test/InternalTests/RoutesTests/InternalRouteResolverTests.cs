﻿namespace MyTested.Mvc.Test.InternalTests.RoutesTests
{
    using System.IO;
    using System.Reflection;
    using Internal.Application;
    using Internal.Http;
    using Internal.Routes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Setups.Routes;
    using Xunit;

    public class InternalRouteResolverTests
    {
        [Fact]
        public void ResolveShouldResolveCorrectControllerAndActionWithDefaultRoute()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(HomeController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Home", routeInfo.ControllerName);
            Assert.Equal("Index", routeInfo.Action);
            Assert.Equal(0, routeInfo.ActionArguments.Count);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectControllerAndActionWithActionNameAttribute()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/AnotherName"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("AnotherName", routeInfo.Action);
            Assert.Equal(0, routeInfo.ActionArguments.Count);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithRouteAttribute()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/AttributeController/AttributeAction"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(RouteController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Route", routeInfo.ControllerName);
            Assert.Equal("Index", routeInfo.Action);
            Assert.Equal(0, routeInfo.ActionArguments.Count);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithParameter()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithParameters/5"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithParameters", routeInfo.Action);
            Assert.Equal(1, routeInfo.ActionArguments.Count);
            Assert.Equal(5, routeInfo.ActionArguments["id"].Value);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithParameterOfDifferentType()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithStringParameters/Test"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithStringParameters", routeInfo.Action);
            Assert.Equal(1, routeInfo.ActionArguments.Count);
            Assert.Equal("Test", routeInfo.ActionArguments["id"].Value);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithNonMatchingParameterOfDifferentType()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithParameters/Test"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithParameters", routeInfo.Action);
            Assert.Equal(0, routeInfo.ActionArguments.Count);
            Assert.False(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithParameterAndQueryString()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithMultipleParameters/5", queryString: "?text=test", contentType: ContentType.ApplicationJson));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithMultipleParameters", routeInfo.Action);
            Assert.Equal(3, routeInfo.ActionArguments.Count);
            Assert.Equal(5, routeInfo.ActionArguments["id"].Value);
            Assert.Equal("test", routeInfo.ActionArguments["text"].Value);
            Assert.True(routeInfo.ActionArguments.ContainsKey("model"));
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithSpecificMethod()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/GetMethod"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("GetMethod", routeInfo.Action);
            Assert.Equal(0, routeInfo.ActionArguments.Count);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithWrongSpecificMethod()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/GetMethod", "POST"));

            Assert.False(routeInfo.IsResolved);
            Assert.Equal("action could not be matched", routeInfo.UnresolvedError);
            Assert.Null(routeInfo.ControllerType);
            Assert.Null(routeInfo.ControllerName);
            Assert.Null(routeInfo.Action);
            Assert.Null(routeInfo.ActionArguments);
            Assert.Null(routeInfo.ModelState);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithFullQueryString()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/QueryString", "POST", "?first=test&second=5"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("QueryString", routeInfo.Action);
            Assert.Equal(2, routeInfo.ActionArguments.Count);
            Assert.Equal("test", routeInfo.ActionArguments["first"].Value);
            Assert.Equal(5, routeInfo.ActionArguments["second"].Value);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithPartialQueryString()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/QueryString", "POST", "?first=test"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("QueryString", routeInfo.Action);
            Assert.Equal(1, routeInfo.ActionArguments.Count);
            Assert.Equal("test", routeInfo.ActionArguments["first"].Value);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithMissingQueryString()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/QueryString", "POST"));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("QueryString", routeInfo.Action);
            Assert.Equal(0, routeInfo.ActionArguments.Count);
            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyJsonContentBody()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithModel/5", "POST", body: @"{""Integer"":5,""String"":""Test""}", contentType: ContentType.ApplicationJson));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithModel", routeInfo.Action);
            Assert.Equal(2, routeInfo.ActionArguments.Count);
            Assert.Equal(5, routeInfo.ActionArguments["id"].Value);
            Assert.True(routeInfo.ActionArguments.ContainsKey("model"));

            var model = routeInfo.ActionArguments["model"].Value as RequestModel;

            Assert.NotNull(model);
            Assert.Equal(5, model.Integer);
            Assert.Equal("Test", model.String);

            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithPartialJsonContentBody()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithModel/5", "POST", body: @"{""Integer"":5}", contentType: ContentType.ApplicationJson));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithModel", routeInfo.Action);
            Assert.Equal(2, routeInfo.ActionArguments.Count);
            Assert.Equal(5, routeInfo.ActionArguments["id"].Value);
            Assert.True(routeInfo.ActionArguments.ContainsKey("model"));

            var model = routeInfo.ActionArguments["model"].Value as RequestModel;

            Assert.NotNull(model);
            Assert.Equal(5, model.Integer);
            Assert.Equal(null, model.String);

            Assert.False(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithEmptyJsonContentBody()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithModel/5", "POST", contentType: ContentType.ApplicationJson));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithModel", routeInfo.Action);
            Assert.Equal(2, routeInfo.ActionArguments.Count);
            Assert.Equal(5, routeInfo.ActionArguments["id"].Value);
            Assert.True(routeInfo.ActionArguments.ContainsKey("model"));

            var model = routeInfo.ActionArguments["model"].Value as RequestModel;

            Assert.Null(model);

            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldResolveCorrectlyWithJsonContentBodyAndQueryString()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithMultipleParameters/5", "POST", body: @"{""Integer"":5,""String"":""Test""}", queryString: "?text=test", contentType: ContentType.ApplicationJson));

            Assert.True(routeInfo.IsResolved);
            Assert.Null(routeInfo.UnresolvedError);
            Assert.Equal(typeof(NormalController).GetTypeInfo(), routeInfo.ControllerType);
            Assert.Equal("Normal", routeInfo.ControllerName);
            Assert.Equal("ActionWithMultipleParameters", routeInfo.Action);
            Assert.Equal(3, routeInfo.ActionArguments.Count);
            Assert.Equal(5, routeInfo.ActionArguments["id"].Value);
            Assert.Equal("test", routeInfo.ActionArguments["text"].Value);
            Assert.True(routeInfo.ActionArguments.ContainsKey("model"));

            var model = routeInfo.ActionArguments["model"].Value as RequestModel;

            Assert.NotNull(model);
            Assert.Equal(5, model.Integer);
            Assert.Equal("Test", model.String);

            Assert.True(routeInfo.ModelState.IsValid);
        }

        [Fact]
        public void ResolveShouldReturnProperErrorWhenTwoActionsAreMatched()
        {
            var routeInfo = InternalRouteResolver.Resolve(
                TestApplication.RouteServices,
                TestApplication.Router,
                this.GetRouteContext("/Normal/ActionWithOverloads"));

            Assert.False(routeInfo.IsResolved);
            Assert.Equal(
                "exception was thrown when trying to select an action: 'Multiple actions matched. The following actions matched route data and had all constraints satisfied:\r\n\r\nMyTested.Mvc.Test.Setups.Routes.NormalController.ActionWithOverloads (MyTested.Mvc.Test)\r\nMyTested.Mvc.Test.Setups.Routes.NormalController.ActionWithOverloads (MyTested.Mvc.Test)'",
                routeInfo.UnresolvedError);
            Assert.Null(routeInfo.ControllerType);
            Assert.Null(routeInfo.ControllerName);
            Assert.Null(routeInfo.Action);
            Assert.Null(routeInfo.ActionArguments);
            Assert.Null(routeInfo.ModelState);
        }

        private RouteContext GetRouteContext(string url, string method = "GET", string queryString = null, string body = null, string contentType = null)
        {
            MyMvc.IsUsingDefaultConfiguration();

            var httpContext = new MockedHttpContext();
            httpContext.Request.Path = new PathString(url);
            httpContext.Request.QueryString = new QueryString(queryString);
            httpContext.Request.Method = method;
            httpContext.Request.ContentType = contentType;

            if (body != null)
            {
                httpContext.Request.Body = new MemoryStream();
                var streamWriter = new StreamWriter(httpContext.Request.Body);
                streamWriter.Write(body);
                streamWriter.Flush();
                httpContext.Request.Body.Position = 0;
            }

            return new RouteContext(httpContext);
        }
    }
}
