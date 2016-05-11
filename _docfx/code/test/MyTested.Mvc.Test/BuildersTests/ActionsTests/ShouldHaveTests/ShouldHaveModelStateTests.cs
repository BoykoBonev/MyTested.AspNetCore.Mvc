﻿namespace MyTested.Mvc.Test.BuildersTests.ActionsTests.ShouldHaveTests
{
    using System;
    using Exceptions;
    using Setups;
    using Setups.Controllers;
    using Setups.Models;
    using Xunit;
    
    public class ShouldHaveModelStateTests
    {
        [Fact]
        public void ShouldHaveModelStateForShouldChainCorrectly()
        {
            var requestModelWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.ModelStateCheck(requestModelWithErrors))
                .ShouldHave()
                .ModelStateFor<RequestModel>(modelState => modelState
                    .ContainingNoErrorFor(r => r.NonRequiredString)
                    .ContainingErrorFor(r => r.Integer)
                    .ContainingErrorFor(r => r.RequiredString));
        }

        [Fact]
        public void ShouldHaveValidModelStateShouldBeValidWithValidRequestModel()
        {
            var requestModel = TestObjectFactory.GetValidRequestModel();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.ModelStateCheck(requestModel))
                .ShouldHave()
                .ValidModelState();
        }

        [Fact]
        public void ShouldHaveValidModelStateShouldThrowExceptionWithInvalidRequestModel()
        {
            var requestModelWithErrors = TestObjectFactory.GetRequestModelWithErrors();
            
            Test.AssertException<ModelErrorAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.ModelStateCheck(requestModelWithErrors))
                        .ShouldHave()
                        .ValidModelState();
                }, 
                "When calling ModelStateCheck action in MvcController expected to have valid model state with no errors, but it had some.");
        }

        [Fact]
        public void ShouldHaveInvalidModelStateShouldBeValidWithInvalidRequestModel()
        {
            var requestModelWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.ModelStateCheck(requestModelWithErrors))
                .ShouldHave()
                .InvalidModelState();
        }

        [Fact]
        public void ShouldHaveInvalidModelStateShouldBeValidWithInvalidRequestModelAndCorrectNumberOfErrors()
        {
            var requestModelWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.ModelStateCheck(requestModelWithErrors))
                .ShouldHave()
                .InvalidModelState(2);
        }

        [Fact]
        public void ShouldHaveInvalidModelStateShouldBeInvalidWithInvalidRequestModelAndIncorrectNumberOfErrors()
        {
            var requestModelWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            Test.AssertException<ModelErrorAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.ModelStateCheck(requestModelWithErrors))
                        .ShouldHave()
                        .InvalidModelState(5);
                }, 
                "When calling ModelStateCheck action in MvcController expected to have invalid model state with 5 errors, but in fact contained 2.");
        }

        [Fact]
        public void ShouldHaveInvalidModelStateShouldThrowExceptionWithValidRequestModel()
        {
            var requestModel = TestObjectFactory.GetValidRequestModel();
            
            Test.AssertException<ModelErrorAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.ModelStateCheck(requestModel))
                        .ShouldHave()
                        .InvalidModelState();
                }, 
                "When calling ModelStateCheck action in MvcController expected to have invalid model state, but was in fact valid.");
        }

        [Fact]
        public void ShouldHaveInvalidModelStateShouldThrowExceptionWithValidRequestModelAndProvidedNumberOfErrors()
        {
            var requestModel = TestObjectFactory.GetValidRequestModel();

            Test.AssertException<ModelErrorAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.ModelStateCheck(requestModel))
                        .ShouldHave()
                        .InvalidModelState(withNumberOfErrors: 5);
                }, 
                "When calling ModelStateCheck action in MvcController expected to have invalid model state with 5 errors, but in fact contained 0.");
        }

        [Fact]
        public void AndShouldWorkCorrectlyWithValidModelState()
        {
            var requestModel = TestObjectFactory.GetValidRequestModel();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.ModelStateCheck(requestModel))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void AndShouldWorkCorrectlyWithInvalidModelState()
        {
            var requestModelWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.ModelStateCheck(requestModelWithErrors))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void AndProvideModelShouldThrowExceptionWhenIsCalledOnTheRequest()
        {
            var requestModelWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            Test.AssertException<InvalidOperationException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.ModelStateCheck(requestModelWithErrors))
                        .ShouldHave()
                        .ModelStateFor<RequestModel>(modelState => modelState
                            .ContainingNoErrorFor(r => r.NonRequiredString)
                            .ContainingErrorFor(r => r.Integer)
                            .ContainingErrorFor(r => r.RequiredString)
                            .ShouldPassFor()
                            .TheModel(model => model != null));
                }, 
                "AndProvideTheModel can be used when there is response model from the action.");
        }
    }
}
