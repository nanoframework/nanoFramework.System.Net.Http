//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace HttpUnitTests
{
    [TestClass]
    public class HttpContentTest
    {
        [TestMethod]
        public void Dispose_BufferContentThenDisposeContent_BufferedStreamGetsDisposed()
        {
            MockContent content = new MockContent();
            content.LoadIntoBuffer();

            Type type = typeof(HttpContent);

            FieldInfo bufferedContentField = type.GetField("_buffer", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(bufferedContentField, "_buffer field shouldn't be null");

            MemoryStream bufferedContentStream = bufferedContentField.GetValue(content) as MemoryStream;
            Assert.IsNotNull(bufferedContentStream, "bufferedContentStream field shouldn't be null");

            content.Dispose();

            // The following line will throw an ObjectDisposedException if the buffered-stream was correctly disposed.
            Assert.ThrowsException(typeof(ObjectDisposedException),
                () =>
                {
                    _ = bufferedContentStream.Length.ToString();
                });
        }

        [TestMethod]
        public void LoadIntoBuffer_ContentLengthSmallerThanActualData_ActualDataLargerThanMaxSize_ThrowsException()
        {
            Assert.SkipTest("Skipping test because of missing implementation in HttpContent");

            // TODO
            // need to implement support for seeakable streams
            BufferTestConfig[] bufferTests = new BufferTestConfig[]
            {
                new BufferTestConfig(1, 100, 99, 1),
                new BufferTestConfig(1, 100, 50, 99),
                new BufferTestConfig(1, 100, 98, 98),
                new BufferTestConfig(1, 100, 99, 99),
                new BufferTestConfig(1, 100, 99, 98),
                new BufferTestConfig(3, 50, 100, 149),
                new BufferTestConfig(3, 50, 149, 149)
            };

            foreach (var testConfig in bufferTests)
            {
                Assert.IsTrue((testConfig.MaxSize >= 1 && testConfig.MaxSize <= (testConfig.NumberOfWrites * testConfig.SizeOfEachWrite) - 1), "Config values out of range.");

                Assert.ThrowsException(typeof(HttpRequestException),
                    () =>
                    {
                        LieAboutLengthContent c = new(
                            testConfig.NumberOfWrites,
                            testConfig.SizeOfEachWrite,
                            testConfig.ReportedLength);

                        c.LoadIntoBuffer();
                    });
            }
        }

        private class BufferTestConfig
        {
            public int NumberOfWrites { get; set; }
            public int SizeOfEachWrite { get; set; }
            public int ReportedLength { get; set; }
            public int MaxSize { get; set; }

            public BufferTestConfig(
                int numberOfWrites,
                int sizeOfEachWrite,
                int reportedLength,
                int maxSize1)
            {
                NumberOfWrites = numberOfWrites;
                SizeOfEachWrite = sizeOfEachWrite;
                ReportedLength = reportedLength;
                MaxSize = maxSize1;
            }
        }

        private sealed class LieAboutLengthContent : HttpContent
        {
            private readonly int _numberOfWrites, _sizeOfEachWrite, _reportedLength;

            public LieAboutLengthContent(int numberOfWrites, int sizeOfEachWrite, int reportedLength)
            {
                _numberOfWrites = numberOfWrites;
                _sizeOfEachWrite = sizeOfEachWrite;
                _reportedLength = reportedLength;
            }

            protected override void SerializeToStream(Stream stream)
            {
                byte[] bytes = new byte[_sizeOfEachWrite];

                for (int i = 0; i < _numberOfWrites; i++)
                {
                   stream.Write(bytes, 0, bytes.Length);
                }
            }

            protected override bool TryComputeLength(out long length)
            {
                length = _reportedLength;
                return true;
            }
        }
    }
}
