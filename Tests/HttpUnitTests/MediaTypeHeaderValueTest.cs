//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.Net.Http.Http.Headers;

namespace HttpUnitTests
{
    [TestClass]
    internal class MediaTypeHeaderValueTests
    {
        [TestMethod]
        public void Ctor_MediaTypeNull_Throw()
        {
            Assert.ThrowsException(typeof(ArgumentException),
                () =>
                {
                    new MediaTypeHeaderValue(null);
                });
        }

        [TestMethod]
        public void Ctor_MediaTypeEmpty_Throw()
        {
            // null and empty should be treated the same. So we also throw for empty strings.
            Assert.ThrowsException(typeof(ArgumentException),
                 () =>
                 {
                     new MediaTypeHeaderValue(string.Empty);
                 });
        }

        [TestMethod]
        public void Ctor_MediaTypeInvalidFormat_ThrowFormatException()
        {
            AssertFormatException("text/plain; charset=utf-8; ");
            AssertFormatException("text/plain;");
            AssertFormatException("text/plain;charset=utf-8"); // ctor takes only media-type name, no parameters
        }

        [TestMethod]
        public void Ctor_MediaTypeValidFormat_SuccessfullyCreated()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            Assert.AreEqual("text/plain", mediaType.MediaType);
            Assert.IsNull(mediaType.CharSet);
        }

        [TestMethod]
        public void MediaType_SetAndGetMediaType_MatchExpectations()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            Assert.AreEqual("text/plain", mediaType.MediaType);

            mediaType.MediaType = "application/xml";
            Assert.AreEqual("application/xml", mediaType.MediaType);
        }

        [TestMethod]
        public void CharSet_SetCharSetAndValidateObject_ParametersEntryForCharSetAdded()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain")
            {
                CharSet = "mycharset"
            };
            Assert.AreEqual("mycharset", mediaType.CharSet);

            mediaType.CharSet = null;
            Assert.IsNull(mediaType.CharSet);
            mediaType.CharSet = null; 
        }

        [TestMethod]
        public void ToString_UseDifferentMediaTypes_AllSerializedCorrectly()
        {
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("text/plain");
            Assert.AreEqual("text/plain", mediaType.ToString());

            mediaType.CharSet = "utf-8";
            Assert.AreEqual("text/plain; charset=utf-8", mediaType.ToString());

            mediaType.CharSet = null;
            Assert.AreEqual("text/plain", mediaType.ToString());
        }

        [TestMethod]
        public void GetHashCode_UseMediaTypeWithoutParameters_SameOrDifferentHashCodes()
        {
            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/plain");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("text/plain");
            mediaType2.CharSet = "utf-8";
            MediaTypeHeaderValue mediaType3 = new MediaTypeHeaderValue("text/plain");

            Assert.AreNotEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode());
            Assert.AreNotEqual(mediaType1.GetHashCode(), mediaType3.GetHashCode());
            Assert.AreNotEqual(mediaType2.GetHashCode(), mediaType3.GetHashCode());
        }

        [TestMethod]
        public void Equals_UseMediaTypeWithoutParameters_EqualOrNotEqualNoExceptions()
        {
            MediaTypeHeaderValue mediaType1 = new MediaTypeHeaderValue("text/plain");
            MediaTypeHeaderValue mediaType2 = new MediaTypeHeaderValue("text/plain");
            mediaType2.CharSet = "utf-8";
            MediaTypeHeaderValue mediaType7 = new MediaTypeHeaderValue("text/other");

            Assert.IsFalse(mediaType1.Equals(mediaType2), "No params vs. charset.");
            Assert.IsFalse(mediaType2.Equals(mediaType1), "charset vs. no params.");
            Assert.IsFalse(mediaType1.Equals(null), "No params vs. <null>.");
            Assert.IsFalse(mediaType1.Equals(mediaType7), "text/plain vs. text/other.");
        }

        [TestMethod]
        public void Parse_SetOfValidValueStrings_ParsedCorrectly()
        {
            MediaTypeHeaderValue expected = new MediaTypeHeaderValue("text/plain");
            CheckValidParse(" text/plain", expected);
            CheckValidParse(" text/plain ", expected);
            CheckValidParse("text/plain", expected);

            // We don't have to test all possible input strings, since most of the pieces are handled by other parsers.
            // The purpose of this test is to verify that these other parsers are combined correctly to build a 
            // media-type parser.
            expected.CharSet = "utf-8";

            OutputHelper.WriteLine($"Expecting {expected}");
            CheckValidParse("text/plain; charset=utf-8", expected);
            CheckValidParse(" text/plain ;charset=utf-8", expected);
        }

        [TestMethod]
        public void Parse_SetOfInvalidValueStrings_Throws()
        {
            CheckInvalidParse("", typeof(FormatException));
            CheckInvalidParse("  ", typeof(FormatException));
            CheckInvalidParse(null, typeof(ArgumentNullException));
            CheckInvalidParse("textplain", typeof(FormatException));
            CheckInvalidParse("textplain;charset=utf-8", typeof(FormatException));
        }

        #region Helper methods

        private static void AssertFormatException(string mediaType)
        {
            Assert.ThrowsException(typeof(FormatException),
                () => 
                { 
                    new MediaTypeHeaderValue(mediaType); 
                });
        }

        private void CheckValidParse(
            string input,
            MediaTypeHeaderValue expectedResult)
        {
            MediaTypeHeaderValue result = MediaTypeHeaderValue.Parse(input);
            Assert.AreEqual(expectedResult.ToString(), result.ToString());
        }

        private void CheckInvalidParse(string input, Type exceptionType)
        {
            Assert.ThrowsException(exceptionType,
                () => 
                { 
                    MediaTypeHeaderValue.Parse(input); 
                });
        }

        #endregion

    }
}
