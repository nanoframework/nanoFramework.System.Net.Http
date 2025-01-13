//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace HttpUnitTests
{
    [TestClass]
    public class StringContentTest
    {
        [TestMethod]
        public void Ctor_NullString_ThrowsArgumentNullException()
        {
            Assert.ThrowsException(typeof(ArgumentNullException),
                () => new StringContent(null));
        }

        [TestMethod]
        public void Ctor_EmptyString_Accept()
        {
            // Consider empty strings like null strings (null and empty strings should be treated equally).
            var content = new StringContent(string.Empty);
            using Stream result = content.ReadAsStream();
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void Ctor_DefineNoEncoding_DefaultEncodingUsed()
        {
            string sourceString = "\u00C4\u00E4\u00FC\u00DC";
            var content = new StringContent(sourceString);
            Encoding defaultStringEncoding = Encoding.UTF8;

            // If no encoding is defined, the default encoding is used: utf-8
            Assert.AreEqual("text/plain", content.Headers.ContentType.MediaType);
            Assert.AreEqual("utf-8", content.Headers.ContentType.CharSet);

            // Make sure the default encoding is also used when serializing the content.
            var destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(8, destination.Length);

            destination.Seek(0, SeekOrigin.Begin);
            string roundTrip = new StreamReader(destination).ReadToEnd();
            Assert.AreEqual(sourceString, roundTrip);
        }
    }
}
