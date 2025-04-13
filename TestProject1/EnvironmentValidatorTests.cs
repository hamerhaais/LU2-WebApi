using LU2_WebApi.Models;
using LU2_WebApi.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LU2_WebApi.Tests
{
    [TestClass]
    public class EnvironmentValidatorTests
    {
        private EnvironmentValidator validator;

        [TestInitialize]
        public void Setup()
        {
            validator = new EnvironmentValidator();
        }

        [TestMethod]
        public void ValidateEnvironment_WithValidName_ShouldNotThrow()
        {
            var env = new Environment2D
            {
                Name = "Woonkamer"
            };

            validator.Validate(env);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateEnvironment_WithEmptyName_ShouldThrow()
        {
            var env = new Environment2D
            {
                Name = ""
            };

            validator.Validate(env);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateEnvironment_WithTooLongName_ShouldThrow()
        {
            var env = new Environment2D
            {
                Name = new string('x', 101) // 101 tekens
            };

            validator.Validate(env);
        }
    }
}
