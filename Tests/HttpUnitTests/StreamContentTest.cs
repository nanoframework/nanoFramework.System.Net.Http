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
    public class StreamContentTest
    {
        [TestMethod]
        public void Ctor_NullStream_ThrowsArgumentNullException()
        {
            Assert.ThrowsException(typeof(ArgumentNullException),
                () => new StreamContent(null));
        }

        [TestMethod]
        public void Ctor_ZeroBufferSize_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                () => new StreamContent(new MemoryStream(), 0));
        }

        [TestMethod]
        public void Ctor_NullStreamAndZeroBufferSize_ThrowsArgumentNullException()
        {
            Assert.ThrowsException(typeof(ArgumentNullException),
                () => new StreamContent(null, 0));
        }

        [TestMethod]
        public void ContentLength_SetStreamSupportingSeeking_StreamLengthMatchesHeaderValue()
        {
            var source = new MockStream(new byte[10], true, true);
            var content = new StreamContent(source);

            Assert.AreEqual(source.Length, content.Headers.ContentLength);
        }

        [TestMethod]
        public void ContentLength_SetStreamSupportingSeekingPartiallyConsumed_StreamLengthMatchesHeaderValueMinusConsumed()
        {
            int consumed = 4;
            var source = new MockStream(new byte[10], true, true);
            source.Read(new byte[consumed], 0, consumed);
            var content = new StreamContent(source);

            Assert.AreEqual(source.Length - consumed, content.Headers.ContentLength);
        }

        [TestMethod]
        public void Dispose_UseMockStreamSourceAndDisposeContent_MockStreamGotDisposed()
        {
            var source = new MockStream(new byte[10]);
            var content = new StreamContent(source);
            content.Dispose();

            Assert.AreEqual(1, source.DisposeCount);
        }

        [TestMethod]
        public void CopyToAsync_NullDestination_ThrowsArgumentnullException()
        {
            var source = new MockStream(new byte[10]);
            var content = new StreamContent(source);
            Assert.ThrowsException(typeof(ArgumentNullException),
                () =>
                {
                    content.CopyTo(null);
                });
        }

        [TestMethod]

        public void CopyTo_CallMultipleTimesWithStreamSupportingSeeking_ContentIsSerializedMultipleTimes()
        {
            var source = new MockStream(new byte[10], true, true);
            var content = new StreamContent(source);

            using var destination1 = new MemoryStream();
            content.CopyTo(destination1);
            Assert.AreEqual(source.Length, destination1.Length);

            using var destination2 = new MemoryStream();
            content.CopyTo(destination2);
            Assert.AreEqual(source.Length, destination2.Length);
        }

        [TestMethod]
        public void CopyTo_CallMultipleTimesWithStreamSupportingSeekingPartiallyConsumed_ContentIsSerializedMultipleTimesFromInitialPoint()
        {
            int consumed = 4;
            var source = new MockStream(new byte[10], true, true);
            source.Read(new byte[consumed], 0, consumed);
            var content = new StreamContent(source);

            using var destination1 = new MemoryStream();
            content.CopyTo(destination1);
            Assert.AreEqual(source.Length - consumed, destination1.Length);

            using var destination2 = new MemoryStream();
            content.CopyTo(destination2);
            Assert.AreEqual(source.Length - consumed, destination2.Length);
        }

        [TestMethod]
        public void CopyTo_CallMultipleTimesWithStreamNotSupportingSeeking_ThrowsInvalidOperationException()
        {
            var source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            var content = new StreamContent(source);

            using var destination1 = new MemoryStream();
            content.CopyTo(destination1);

            // Use hardcoded expected length, since source.Length would throw (source stream gets disposed if non-seekable).
            Assert.AreEqual(10, destination1.Length);

            // Note that the InvalidOperationException is thrown in CopyToAsync(). It is not thrown inside the task.
            using var destination2 = new MemoryStream();
            Assert.ThrowsException(typeof(InvalidOperationException),
                () =>
                {
                    content.CopyTo(destination2);
                });
        }

        [TestMethod]
        public void CopyTo_CallMultipleTimesWithStreamNotSupportingSeekingButBufferedStream_ContentSerializedOnceToBuffer()
        {
            var source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            var content = new StreamContent(source);

            // After loading the content into a buffer, we should be able to copy the content to a destination stream
            // multiple times, even though the stream doesn't support seeking.
            content.LoadIntoBuffer();

            using var destination1 = new MemoryStream();
            content.CopyTo(destination1);
            // Use hardcoded expected length, since source.Length would throw (source stream gets disposed if non-seekable)
            Assert.AreEqual(10, destination1.Length);

            using var destination2 = new MemoryStream();
            content.CopyTo(destination2);
            Assert.AreEqual(10, destination2.Length);
        }

        [TestMethod]
        public void CopyTo_CallMultipleTimesWithStreamNotSupportingSeekingButBufferedStreamPartiallyConsumed_ContentSerializedOnceToBuffer()
        {
            int consumed = 4;
            var source = new MockStream(new byte[10], false, true); // doesn't support seeking.
            source.Read(new byte[consumed], 0, consumed);
            var content = new StreamContent(source);

            // After loading the content into a buffer, we should be able to copy the content to a destination stream
            // multiple times, even though the stream doesn't support seeking.
            content.LoadIntoBuffer();

            using var destination1 = new MemoryStream();
            content.CopyTo(destination1);
            // Use hardcoded expected length, since source.Length would throw (source stream gets disposed if non-seekable).
            Assert.AreEqual(10 - consumed, destination1.Length);

            using var destination2 = new MemoryStream();
            content.CopyTo(destination2);
            Assert.AreEqual(10 - consumed, destination2.Length);
        }

        [TestMethod]
        public void CopyTo_NoLoadIntoBuffer_NotBuffered()
        {
            var initialSourceContent = new byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var sourceContent = new byte[10];
            Array.Copy(initialSourceContent, sourceContent, 10);

            var sourceStream = new MockStream(sourceContent, true, true);

            var content = new StreamContent(sourceStream);

            using var destination1 = new MemoryStream();
            content.CopyTo(destination1);

            Assert.AreEqual(10, destination1.Length);
            CollectionAssert.AreEqual(initialSourceContent, destination1.ToArray());

            // replace source content to ensure data was not buffered in the content
            var replacedSourceContent = new byte[10] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            Array.Copy(replacedSourceContent, sourceContent, 10);

            using var destination2 = new MemoryStream();
            content.CopyTo(destination2);
            Assert.AreEqual(10, destination2.Length);
            CollectionAssert.AreEqual(replacedSourceContent, destination2.ToArray());
        }

        [TestMethod]
        public void CopyTo_LoadIntoBuffer_Buffered()
        {
            var initialSourceContent = new byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var sourceContent = new byte[10];
            Array.Copy(initialSourceContent, sourceContent, 10);

            var sourceStream = new MockStream(sourceContent, true, true);

            var content = new StreamContent(sourceStream);

            // buffer so changing the source stream doesn't change the Content
            content.LoadIntoBuffer();

            using var destination1 = new MemoryStream();
            content.CopyTo(destination1);

            Assert.AreEqual(10, destination1.Length);
            CollectionAssert.AreEqual(initialSourceContent, destination1.ToArray());

            // replace source content to ensure data was buffered in the content
            var replacedSourceContent = new byte[10] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            Array.Copy(replacedSourceContent, sourceContent, 10);

            using var destination2 = new MemoryStream();
            content.CopyTo(destination2);
            Assert.AreEqual(10, destination2.Length);
            CollectionAssert.AreEqual(initialSourceContent, destination2.ToArray());
        }

        [TestMethod]
        public void ContentReadStream_GetProperty_ReturnOriginalStream()
        {
            var source = new MockStream(new byte[10]);
            var content = new StreamContent(source);

            using Stream stream = content.ReadAsStream();

            Assert.IsFalse(stream.CanWrite);
            Assert.AreEqual(source.Length, stream.Length);
            Assert.AreEqual(0, source.ReadCount);
            Assert.AreNotSame(source, stream);
        }

        [TestMethod]
        public void ContentReadStream_GetProperty_LoadIntoBuffer_ReturnOriginalStream()
        {
            var source = new MockStream(new byte[10]);
            var content = new StreamContent(source);

            using Stream stream = content.ReadAsStream();
            Assert.IsFalse(stream.CanWrite);
            Assert.AreEqual(source.Length, stream.Length);
            Assert.AreEqual(0, source.ReadCount);
            Assert.AreNotSame(source, stream);
        }

        [TestMethod]
        public void ContentReadStream_GetPropertyPartiallyConsumed_ReturnOriginalStream()
        {
            int consumed = 4;
            var source = new MockStream(new byte[10]);
            source.Read(new byte[consumed], 0, consumed);
            var content = new StreamContent(source);

            using Stream stream = content.ReadAsStream();
            Assert.IsFalse(stream.CanWrite);
            Assert.AreEqual(source.Length, stream.Length);
            Assert.AreEqual(1, source.ReadCount);
            Assert.AreEqual(consumed, stream.Position);
            Assert.AreNotSame(source, stream);
        }

        [TestMethod]
        public void ContentReadStream_CheckResultProperties_ValuesRepresentReadOnlyStream()
        {
            byte[] data = new byte[10];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }

            var source = new MockStream(data);
            var content = new StreamContent(source);
            using Stream contentReadStream = content.ReadAsStream();

            // The following checks verify that the stream returned passes all read-related properties to the
            // underlying MockStream and throws when using write-related members.

            Assert.IsFalse(contentReadStream.CanWrite);
            Assert.IsTrue(contentReadStream.CanRead);
            Assert.AreEqual(source.Length, contentReadStream.Length);

            Assert.AreEqual(1, source.CanSeekCount);
            OutputHelper.WriteLine(contentReadStream.CanSeek.ToString());
            Assert.AreEqual(2, source.CanSeekCount);

            contentReadStream.Position = 3; // No exception.
            Assert.AreEqual(3, contentReadStream.Position);

            byte byteOnIndex3 = (byte)contentReadStream.ReadByte();
            Assert.AreEqual(data[3], byteOnIndex3);

            byte[] byteOnIndex4 = new byte[1];
            int result = contentReadStream.Read(byteOnIndex4, 0, 1);
            Assert.AreEqual(1, result);

            Assert.AreEqual(data[4], byteOnIndex4[0]);

            byte[] byteOnIndex5 = new byte[1];
            Assert.AreEqual(1, contentReadStream.Read(byteOnIndex5, 0, 1));
            Assert.AreEqual(data[5], byteOnIndex5[0]);

            byte[] byteOnIndex6 = new byte[1];
            Assert.AreEqual(1, contentReadStream.Read(new SpanByte(byteOnIndex6, 0, 1)));
            Assert.AreEqual(data[6], byteOnIndex6[0]);

            contentReadStream.ReadTimeout = 123;
            Assert.AreEqual(123, source.ReadTimeout);
            Assert.AreEqual(123, contentReadStream.ReadTimeout);

            Assert.AreEqual(0, source.CanTimeoutCount);
            OutputHelper.WriteLine(contentReadStream.CanTimeout.ToString());
            Assert.AreEqual(1, source.CanTimeoutCount);

            Assert.AreEqual(0, source.SeekCount);
            contentReadStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(1, source.SeekCount);

            Assert.ThrowsException(typeof(NotSupportedException), () => { contentReadStream.WriteTimeout = 5; });
            Assert.ThrowsException(typeof(NotSupportedException), () => contentReadStream.WriteTimeout.ToString());
            Assert.ThrowsException(typeof(NotSupportedException), () => contentReadStream.Flush());
            Assert.ThrowsException(typeof(NotSupportedException), () => contentReadStream.SetLength(1));
            Assert.ThrowsException(typeof(NotSupportedException), () => contentReadStream.Write(null, 0, 0));
            Assert.ThrowsException(typeof(NotSupportedException), () => contentReadStream.WriteByte(1));

            Assert.AreEqual(0, source.DisposeCount);
            contentReadStream.Dispose();
            Assert.AreEqual(1, source.DisposeCount);
        }

        #region Helper methods

        private class MockStream : MemoryStream
        {
            private bool _canSeek;
            private bool _canRead;
            private int _readTimeout;

            public int DisposeCount { get; private set; }
            public int BufferSize { get; private set; }
            public int ReadCount { get; private set; }
            public int CanSeekCount { get; private set; }
            public int CanTimeoutCount { get; private set; }
            public int SeekCount { get; private set; }

            public override bool CanSeek
            {
                get
                {
                    CanSeekCount++;
                    return _canSeek;
                }
            }

            public override bool CanRead
            {
                get { return _canRead; }
            }

            public override int ReadTimeout
            {
                get { return _readTimeout; }
                set { _readTimeout = value; }
            }

            public override bool CanTimeout
            {
                get
                {
                    CanTimeoutCount++;
                    return base.CanTimeout;
                }
            }

            public MockStream(byte[] data)
                : this(data, true, true)
            {
            }

            public MockStream(byte[] data, bool canSeek, bool canRead)
                : base(data)
            {
                _canSeek = canSeek;
                _canRead = canRead;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                ReadCount++;
                SetBufferSize(count);
                return base.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin loc)
            {
                SeekCount++;
                return base.Seek(offset, loc);
            }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }

            private void SetBufferSize(int count)
            {
                if (BufferSize == 0)
                {
                    BufferSize = count;
                }
            }
        }

        #endregion
    }
}
