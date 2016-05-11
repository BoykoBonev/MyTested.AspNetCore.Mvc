﻿namespace MyTested.Mvc.Test.BuildersTests.ShouldPassForTests
{
    using Exceptions;
    using Setups;
    using Setups.Controllers;
    using Xunit;

    public class ShouldPassForTestBuilderWithInvokedActionTests
    {
        [Fact]
        public void CaughtExceptionAssertionsShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.FullOkAction())
                .ShouldReturn()
                .Ok()
                .ShouldPassFor()
                .TheCaughtException(exception =>
                {
                    Assert.Null(exception);
                });
        }

        [Fact]
        public void CaughtExceptionPredicateShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.FullOkAction())
                .ShouldPassFor()
                .TheCaughtException(exception => exception == null);
        }

        [Fact]
        public void CaughtExceptionPredicateShouldThrowExceptionWithInvalidTest()
        {
            Test.AssertException<InvalidAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.FullOkAction())
                        .ShouldPassFor()
                        .TheCaughtException(exception => exception != null);
                },
                "Expected the caught exception to pass the given predicate but it failed.");
        }
        
        [Fact]
        public void CaughtExceptionPredicateShouldThrowExceptionWithInvalidTestAndActualException()
        {
            Test.AssertException<InvalidAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.ActionWithException())
                        .ShouldPassFor()
                        .TheCaughtException(exception => exception == null);
                },
                "Expected the NullReferenceException to pass the given predicate but it failed.");
        }

        [Fact]
        public void HttpResponseAssertionsShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.FullOkAction())
                .ShouldReturn()
                .Ok()
                .ShouldPassFor()
                .TheHttpResponse(response =>
                {
                    Assert.NotNull(response);
                });
        }

        [Fact]
        public void HttpResponsePredicateShouldWorkCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.FullOkAction())
                .ShouldPassFor()
                .TheHttpResponse(response => response != null);
        }

        [Fact]
        public void HttpResponsePredicateShouldThrowExceptionWithInvalidTest()
        {
            Test.AssertException<InvalidAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.FullOkAction())
                        .ShouldPassFor()
                        .TheHttpResponse(response => response == null);
                },
                "Expected the HttpResponse to pass the given predicate but it failed.");
        }
    }
}
