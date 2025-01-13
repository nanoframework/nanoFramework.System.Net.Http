//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;
using System.Net.Http;

namespace HttpUnitTests
{
    [TestClass]
    public class ByteArrayContentTest
    {
        [TestMethod]
        public void Ctor_NullSourceArray_ThrowsArgumentNullException()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => new ByteArrayContent(null));
        }

        [TestMethod]
        public void Ctor_NullSourceArrayWithRange_ThrowsArgumentNullException()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => new ByteArrayContent(null, 0, 1));
        }

        [TestMethod]
        public void Ctor_EmptySourceArrayWithRange_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[0], 0, 1));
        }

        [TestMethod]
        public void Ctor_StartIndexTooBig_ThrowsArgumentOufOfRangeException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 5, 1));
        }

        [TestMethod]
        public void Ctor_StartIndexNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], -1, 1));
        }

        [TestMethod]
        public void Ctor_LengthTooBig_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 1, 5));
        }

        [TestMethod]
        public void Ctor_LengthPlusOffsetCauseIntOverflow_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 1, int.MaxValue));
        }

        [TestMethod]
        public void Ctor_LengthNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 0, -1));
        }

        [TestMethod]
        public void ContentLength_UseWholeSourceArray_LengthMatchesArrayLength()
        {
            var contentData = new byte[10];
            var content = new ByteArrayContent(contentData);

            Assert.AreEqual(contentData.Length, content.Headers.ContentLength);
        }

        [TestMethod]
        public void ContentLength_UsePartialSourceArray_LengthMatchesArrayLength()
        {
            var contentData = new byte[10];
            var content = new ByteArrayContent(contentData, 5, 3);

            Assert.AreEqual(3, content.Headers.ContentLength);
        }

        [TestMethod]
        public void ReadAsStreamAsync_EmptySourceArray_Succeed()
        {
            var content = new ByteArrayContent(new byte[0]);
            using Stream stream = content.ReadAsStream();
            Assert.AreEqual(0, stream.Length);
        }

        [TestMethod]
        public void ReadAsStream_Call_MemoryStreamWrappingByteArrayReturned()
        {
            var contentData = new byte[10];
            var content = new ByteArrayContent(contentData, 5, 3);

            Stream stream = content.ReadAsStream();
            Assert.IsFalse(stream.CanWrite);
            Assert.AreEqual(3, stream.Length);
        }

        [TestMethod]
        public void CopyTo_NullDestination_ThrowsArgumentNullException()
        {
            byte[] contentData = CreateSourceArray();
            var content = new ByteArrayContent(contentData);

            Assert.ThrowsException(typeof(ArgumentNullException),
                () =>
                {
                    content.CopyTo(null);
                });
        }

        [TestMethod]
        public void CopyTo_UseWholeSourceArray_WholeContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            var content = new ByteArrayContent(contentData);

            using var destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(contentData.Length, destination.Length);
            CheckResult(destination, 0);
        }

        [TestMethod]
        public void CopyTo_UsePartialSourceArray_PartialContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            var content = new ByteArrayContent(contentData, 3, 5);

            using var destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(5, destination.Length);
            CheckResult(destination, 3);
        }

        [TestMethod]
        public void CopyTo_UseEmptySourceArray_NothingCopied()
        {
            var contentData = new byte[0];
            var content = new ByteArrayContent(contentData, 0, 0);

            using var destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.AreEqual(0, destination.Length);
        }

        #region Helper methods

        private static byte[] CreateSourceArray()
        {
            var contentData = new byte[10];
            for (int i = 0; i < contentData.Length; i++)
            {
                contentData[i] = (byte)(i % 256);
            }
            return contentData;
        }

        private static void CheckResult(Stream destination, byte firstValue)
        {
            destination.Position = 0;
            var destinationData = new byte[destination.Length];
            int read = destination.Read(destinationData, 0, destinationData.Length);

            Assert.AreEqual(destinationData.Length, read);
            Assert.AreEqual(firstValue, destinationData[0]);

            for (int i = 1; i < read; i++)
            {
                Assert.IsTrue((destinationData[i] == (destinationData[i - 1] + 1)) ||
                    ((destinationData[i] == 0) && (destinationData[i - 1] != 0)));
            }
        }

        #endregion
    }
}
