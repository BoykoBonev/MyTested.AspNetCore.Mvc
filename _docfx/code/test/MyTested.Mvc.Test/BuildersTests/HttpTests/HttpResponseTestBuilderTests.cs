﻿namespace MyTested.Mvc.Test.BuildersTests.HttpTests
{
    using Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.Primitives;
    using Setups;
    using Setups.Controllers;
    using Setups.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    public class HttpResponseTestBuilderTests
    {
        [Fact]
        public void VoidActionShouldNotThrowExceptionWithCorrectResponse()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomResponseAction())
                .ShouldHave()
                .HttpResponse(response => response
                    .WithContentType(ContentType.ApplicationJson)
                    .AndAlso()
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .AndAlso()
                    .ContainingHeader("TestHeader", "TestHeaderValue")
                    .ContainingCookie("TestCookie", "TestCookieValue", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Domain = "testdomain.com",
                        Expires = new DateTimeOffset(new DateTime(2016, 1, 1, 1, 1, 1)),
                        Path = "/"
                    }))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void ActionShouldNotThrowExceptionWithCorrectResponse()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldReturnEmpty();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response
                    .WithContentType(ContentType.ApplicationJson)
                    .AndAlso()
                    .WithContentLength(100)
                    .AndAlso()
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .AndAlso()
                    .ContainingHeader("TestHeader", "TestHeaderValue")
                    .ContainingCookie("TestCookie", "TestCookieValue", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Domain = "testdomain.com",
                        Expires = new DateTimeOffset(new DateTime(2016, 1, 1, 1, 1, 1)),
                        Path = "/"
                    }));
        }

        [Fact]
        public void WithContentLengthShouldThrowExceptionWithInvalidContentLength()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomResponseBodyWithBytesAction())
                        .ShouldHave()
                        .HttpResponse(response => response.WithContentLength(10));
                },
                "When calling CustomResponseBodyWithBytesAction action in MvcController expected HTTP response content length to be 10, but instead received 100.");
        }

        [Fact]
        public void WithContentTypeShouldThrowExceptionWithInvalidContentType()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomResponseBodyWithBytesAction())
                        .ShouldHave()
                        .HttpResponse(response => response.WithContentType(ContentType.ApplicationXml));
                },
                "When calling CustomResponseBodyWithBytesAction action in MvcController expected HTTP response content type to be 'application/xml', but instead received 'application/json'.");
        }

        [Fact]
        public void WithResponseCookieBuilderShouldNotThrowExceptionWithCorrectCookie()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response
                    .ContainingCookie(cookie => cookie
                        .WithName("TestCookie")
                        .AndAlso()
                        .WithValue("TestCookieValue")
                        .AndAlso()
                        .WithSecure(true)
                        .AndAlso()
                        .WithHttpOnly(true)
                        .AndAlso()
                        .WithMaxAge(null)
                        .AndAlso()
                        .WithDomain("testdomain.com")
                        .AndAlso()
                        .WithExpired(new DateTimeOffset(new DateTime(2016, 1, 1, 1, 1, 1)))
                        .AndAlso()
                        .WithPath("/")));
        }

        [Fact]
        public void WithResponseCookieBuilderShouldNotThrowExceptionWithCorrectValuePredicate()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response
                    .ContainingCookie(cookie => cookie
                        .WithName("TestCookie")
                        .AndAlso()
                        .WithValue(value => value.StartsWith("TestCookie"))));
        }

        [Fact]
        public void WithResponseCookieBuilderShouldNotThrowExceptionWithCorrectValueAssertions()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response
                    .ContainingCookie(cookie => cookie
                        .WithName("TestCookie")
                        .AndAlso()
                        .WithValue(value =>
                        {
                            Assert.True(value.StartsWith("TestCookie"));
                        })));
        }

        [Fact]
        public void ContainingCookieByKeyShouldThrowExceptionWithInvalidKey()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingCookie("Invalid"));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response cookies to contain cookie with 'Invalid' name, but such was not found.");
        }

        [Fact]
        public void ContainingCookieByKeyValueShouldThrowExceptionWithInvalidValue()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingCookie("TestCookie", "Invalid"));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response cookies to contain cookie with 'TestCookie' name and 'Invalid' value, but the value was 'TestCookieValue'.");
        }

        [Fact]
        public void ContainingCookieWithOptionsShouldThrowExceptionWithInvalidOptions()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                       .Controller<MvcController>()
                       .Calling(c => c.CustomVoidResponseAction())
                       .ShouldHave()
                       .HttpResponse(response => response.ContainingCookie("TestCookie", "TestCookieValue", new CookieOptions
                       {
                           HttpOnly = false,
                           Secure = true,
                           Domain = "testdomain.com",
                           Expires = new DateTimeOffset(new DateTime(2016, 1, 1, 1, 1, 1)),
                           Path = "/"
                       }));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response cookies to contain cookie with the provided options - 'TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure', but in fact they were different - 'TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly'.");
        }

        [Fact]
        public void ContainingCookiesShouldNotThrowExceptionWithValidCookies()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingCookies(new Dictionary<string, string> { ["TestCookie"] = "TestCookieValue", ["AnotherCookie"] = "TestCookieValue" }));
        }

        [Fact]
        public void ContainingCookiesShouldNotThrowExceptionWithValidCookiesAsObject()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingCookies(new { TestCookie = "TestCookieValue", AnotherCookie = "TestCookieValue" }));
        }

        [Fact]
        public void ContainingCookiesShouldThrowExceptionWithOneInvalidCookie()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingCookies(new Dictionary<string, string> { ["TestCookie"] = "TestCookieValue" }));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response cookies to have 1 item, but instead found 2.");
        }

        [Fact]
        public void ContainingCookiesShouldThrowExceptionWithMoreInvalidCookies()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingCookies(new Dictionary<string, string> { ["TestCookie"] = "TestCookieValue", ["AnotherCookie"] = "TestCookieValue", ["YetAnotherCookie"] = "TestCookieValue", }));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response cookies to have 3 items, but instead found 2.");
        }

        [Fact]
        public void WithNoCookiesShouldThrowException()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.OkResultAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingCookie("Test"));
                },
                "When calling OkResultAction action in MvcController expected HTTP response headers to contain header with 'Set-Cookie' name, but such was not found.");
        }

        [Fact]
        public void WithEmptyCookiesShouldThrowException()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomCookieHeadersAction(null))
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingCookie("Test"));
                },
                "When calling CustomCookieHeadersAction action in MvcController expected HTTP response to have set cookies, but none were found.");
        }

        [Fact]
        public void WithInvalidCookiesShouldThrowException()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomCookieHeadersAction(string.Empty))
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingCookie("Test"));
                },
                "When calling CustomCookieHeadersAction action in MvcController expected HTTP response to have valid cookie values, but some of them were invalid.");
        }

        [Fact]
        public void WithResponseCookieBuilderWithoutNameShouldThrowInvalidOperationException()
        {
            Test.AssertException<InvalidOperationException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response
                            .ContainingCookie(cookie => cookie
                                .WithValue("TestCookieValue")
                                .WithSecure(true)
                                .WithHttpOnly(true)
                                .WithMaxAge(null)
                                .WithDomain("testdomain.com")
                                .WithExpired(new DateTimeOffset(new DateTime(2016, 1, 1, 1, 1, 1)))
                                .WithPath("/")));
                },
                "Cookie name must be provided. 'WithName' method must be called on the cookie builder in order to run this test case successfully.");
        }

        [Fact]
        public void WithResponseCookieBuilderShouldThrowExceptionWithIncorrectCookie()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response
                            .ContainingCookie(cookie => cookie
                                .WithName("TestCookie")
                                .WithValue("TestCookieValue12")
                                .WithSecure(true)
                                .WithHttpOnly(true)
                                .WithMaxAge(null)
                                .WithDomain("testdomain.com")
                                .WithExpired(new DateTimeOffset(new DateTime(2016, 1, 1, 1, 1, 1)))
                                .WithPath("/")));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response cookies to contain cookie with 'TestCookie' name and 'TestCookie=TestCookieValue12; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly' value, but the value was 'TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly'.");
        }

        [Fact]
        public void ContainingHeaderShouldThrowExceptionWithInvalidHeaderName()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingHeader("Invalid"));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response headers to contain header with 'Invalid' name, but such was not found.");
        }

        [Fact]
        public void HeaderValuesShouldThrowExceptionWithInvalidHeaderValue()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response
                            .ContainingHeader("TestHeader", "Invalid"));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response headers to contain header with 'TestHeader' name and 'Invalid' value, but the value was 'TestHeaderValue'.");
        }

        [Fact]
        public void HeaderValuesShouldThrowExceptionWithInvalidHeaderValues()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response
                            .ContainingHeader("MultipleTestHeader", "Invalid"));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response headers to contain header with 'MultipleTestHeader' name and 'Invalid' value, but the values were 'FirstMultipleTestHeaderValue,AnotherMultipleTestHeaderValue'.");
        }

        [Fact]
        public void ContainingHeaderWithMultipleValuesShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingHeader("TestHeader", new List<string> { "TestHeaderValue" }));
        }

        [Fact]
        public void ContainingHeaderWithMultipleValuesAsParamsShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingHeader("TestHeader", new string[] { "TestHeaderValue" }));
        }

        [Fact]
        public void ContainingHeaderWithMultipleValuesShouldThrowExceptionWithInvalidSingleCount()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingHeader("MultipleTestHeader", new string[] { "Invalid" }));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response headers to contain header with 'MultipleTestHeader' name and 1 value, but instead found 2.");
        }

        [Fact]
        public void ContainingHeaderWithMultipleValuesShouldThrowExceptionWithInvalidCount()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingHeader("MultipleTestHeader", new string[] { "Invalid", "Second", "Third" }));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response headers to contain header with 'MultipleTestHeader' name and 3 values, but instead found 2.");
        }

        [Fact]
        public void ContainingHeadersWithHeadersDictionaryShouldWorkCorrectly()
        {
            var headers = new HeaderDictionary
            {
                ["TestHeader"] = "TestHeaderValue",
                ["AnotherTestHeader"] = "AnotherTestHeaderValue",
                ["MultipleTestHeader"] = new[] { "FirstMultipleTestHeaderValue", "AnotherMultipleTestHeaderValue" },
                ["Content-Type"] = "application/json",
                ["Content-Length"] = "100",
                ["Set-Cookie"] = new[] { "TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly", "AnotherCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly" },
            };

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingHeaders(headers));
        }

        [Fact]
        public void ContainingHeadersWithHeadersDictionaryShouldThrowExceptionWithInvalidCount()
        {
            var headers = new HeaderDictionary
            {
                ["Set-Cookie"] = new[] { "TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly", "AnotherCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly" },
            };


            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingHeaders(headers));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response headers to have 1 item, but instead found 6.");
        }

        [Fact]
        public void ContainingHeadersWithHeadersDictionaryShouldThrowExceptionWithInvalidMultipleCount()
        {
            var headers = new HeaderDictionary
            {
                ["TestHeader"] = "Test",
                ["Set-Cookie"] = new[] { "TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly", "AnotherCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly" },
            };


            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomVoidResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.ContainingHeaders(headers));
                },
                "When calling CustomVoidResponseAction action in MvcController expected HTTP response headers to have 2 items, but instead found 6.");
        }

        [Fact]
        public void ContainingHeadersWithHeadersObjectShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingHeaders(new
                {
                    TestHeader = "TestHeaderValue",
                    AnotherTestHeader = "AnotherTestHeaderValue",
                    MultipleTestHeader = new[] { "FirstMultipleTestHeaderValue", "AnotherMultipleTestHeaderValue" },
                    Content_Type = "application/json",
                    Content_Length = "100",
                    Set_Cookie = new[] { "TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly", "AnotherCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly" },
                }));
        }

        [Fact]
        public void ContainingHeadersWithHeadersStringDicrionaryShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingHeaders(new Dictionary<string, string>
                {
                    ["TestHeader"] = "TestHeaderValue",
                    ["AnotherTestHeader"] = "AnotherTestHeaderValue",
                    ["MultipleTestHeader"] = "FirstMultipleTestHeaderValue,AnotherMultipleTestHeaderValue",
                    ["Content-Type"] = "application/json",
                    ["Content-Length"] = "100",
                    ["Set-Cookie"] = "TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly,AnotherCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly"
                }));
        }

        [Fact]
        public void ContainingHeadersWithHeadersStringValuesDicrionaryShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingHeaders(new Dictionary<string, StringValues>
                {
                    ["TestHeader"] = "TestHeaderValue",
                    ["AnotherTestHeader"] = "AnotherTestHeaderValue",
                    ["MultipleTestHeader"] = new[] { "FirstMultipleTestHeaderValue", "AnotherMultipleTestHeaderValue" },
                    ["Content-Type"] = "application/json",
                    ["Content-Length"] = "100",
                    ["Set-Cookie"] = new[] { "TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly", "AnotherCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly" }
                }));
        }

        [Fact]
        public void ContainingHeadersWithHeadersEnumerableDicrionaryShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomVoidResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.ContainingHeaders(new Dictionary<string, IEnumerable<string>>
                {
                    ["TestHeader"] = new[] { "TestHeaderValue" },
                    ["AnotherTestHeader"] = new[] { "AnotherTestHeaderValue" },
                    ["MultipleTestHeader"] = new[] { "FirstMultipleTestHeaderValue", "AnotherMultipleTestHeaderValue" },
                    ["Content-Type"] = new[] { "application/json" },
                    ["Content-Length"] = new[] { "100" },
                    ["Set-Cookie"] = new[] { "TestCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly", "AnotherCookie=TestCookieValue; expires=Thu, 31 Dec 2015 23:01:01 GMT; domain=testdomain.com; path=/; secure; httponly" }
                }));
        }

        [Fact]
        public void WithBodyAsStreamShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomResponseBodyWithBytesAction())
                .ShouldHave()
                .HttpResponse(response => response.WithBody(new MemoryStream(new byte[] { 1, 2, 3 })));
        }

        [Fact]
        public void WithBodyAsStreamShouldThrowExceptionWithIncorrectStream()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomResponseBodyWithBytesAction())
                        .ShouldHave()
                        .HttpResponse(response => response.WithBody(new MemoryStream(new byte[] { 1, 2, 3, 4 })));
                },
                "When calling CustomResponseBodyWithBytesAction action in MvcController expected HTTP response body to have contents as the provided ones, but instead received different result.");
        }

        [Fact]
        public void WithBodyOfTypeShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.WithBodyOfType<RequestModel>(ContentType.ApplicationJson));
        }

        [Fact]
        public void WithBodyOfTypeShouldThrowExceptionWithIncorrectBodyAsPrimitiveType()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.WithBodyOfType<int>(ContentType.ApplicationJson));
                },
                "When calling CustomResponseAction action in MvcController expected HTTP response body to be of Int32 type when using 'application/json', but in fact it was not.");
        }

        [Fact]
        public void WithBodyShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.WithBody(new RequestModel { Integer = 1, RequiredString = "Text" }, ContentType.ApplicationJson));
        }

        [Fact]
        public void WithStringBodyShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomResponseBodyWithStringBody())
                .ShouldHave()
                .HttpResponse(response => response.WithStringBody("Test"));
        }

        [Fact]
        public void WithJsonBodyShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.WithJsonBody(new RequestModel { Integer = 1, RequiredString = "Text" }));
        }

        [Fact]
        public void WithJsonBodyShouldThrowExceptionWithWrongBody()
        {
            Test.AssertException<HttpResponseAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.CustomResponseAction())
                        .ShouldHave()
                        .HttpResponse(response => response.WithJsonBody(new RequestModel { Integer = 2, RequiredString = "Text" }));
                },
                "When calling CustomResponseAction action in MvcController expected HTTP response body to be the given object, but in fact it was different.");
        }

        [Fact]
        public void WithJsonBodyAsStringShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.CustomResponseAction())
                .ShouldHave()
                .HttpResponse(response => response.WithJsonBody(@"{""Integer"":1,""RequiredString"":""Text""}"));
        }
    }
}
