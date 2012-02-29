using Mercury.Data.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Mercury.Data.Tests
{
    
    
    /// <summary>
    ///This is a test class for StringExtensionsTest and is intended
    ///to contain all StringExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringExtensionsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for FormatWith
        ///</summary>
        [TestMethod()]
        public void FormatWithTest()
        {
            string format = string.Empty; // TODO: Initialize to an appropriate value
            object source = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = StringExtensions.FormatWith(format, source);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FormatWith
        ///</summary>
        [TestMethod()]
        public void FormatWithTest1()
        {
            string format = string.Empty; // TODO: Initialize to an appropriate value
            IFormatProvider provider = null; // TODO: Initialize to an appropriate value
            object source = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = StringExtensions.FormatWith(format, provider, source);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringWithFormatReplacesNamedValue()
        {
            var testObject = new
            {
                FirstName = "Todd",
                Things = new[]{
                    new { Description = "This is a test 1" },
                    new { Description = "Test be this 2" }
                }
            };

            string aFormat = "Hello {FirstName}! Welcome to the test.";
            string expected = "Hello Todd! Welcome to the test.";
            string actual;

            actual = testObject.ToStringWithFormat(aFormat);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringWithFormatReplacesCollections()
        {
            var testObject = new
            {
                FirstName = "Todd",
                Things = new[]{
                    new { Description = "This is a test 1" },
                    new { Description = "Test be this 2" }
                }
            };

            string aFormat = "Hello {FirstName}! Welcome to the test <ul>{Things:<li>{Description}</li>}</ul>.";
            string expected = "Hello Todd! Welcome to the test <ul><li>This is a test 1</li><li>Test be this 2</li></ul>.";
            string actual;

            actual = testObject.ToStringWithFormat(aFormat);

            Assert.AreEqual(expected, actual);
        }

        ///// <summary>
        /////A test for ToString
        /////</summary>
        //[TestMethod()]
        //public void ToStringTest1()
        //{
        //    object anObject = null; // TODO: Initialize to an appropriate value
        //    string aFormat = string.Empty; // TODO: Initialize to an appropriate value
        //    IFormatProvider formatProvider = null; // TODO: Initialize to an appropriate value
        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = StringExtensions.ToString(anObject, aFormat, formatProvider);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
