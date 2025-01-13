//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.IO;
using System.Threading;

namespace System.Net.Http
{
    /// <summary>
    /// Provides HTTP content based on a stream.
    /// </summary>
    public class StreamContent : HttpContent
    {
        private readonly Stream _content;
        private readonly int _bufferSize;
        private readonly long _startPosition;
        private bool _contentConsumed;

        /// <summary>
        /// Creates a new instance of the <see cref="StreamContent"/> class.
        /// </summary>
        /// <param name="content">The content used to initialize the <see cref="StreamContent"/>.</param>
        /// <remarks>
        /// The <see cref="StreamContent"/> object calls <see cref="Dispose"/> on the provided Stream object when <see cref="StreamContent.Dispose"/> is called.
        /// </remarks>
        public StreamContent(Stream content)
        : this(content, 4 * 1024)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StreamContent"/> class.
        /// </summary>
        /// <param name="content">The content used to initialize the <see cref="StreamContent"/>.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer for the <see cref="StreamContent"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="content"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="bufferSize"/> was less than or equal to zero.</exception>
        /// <remarks>
        /// The <see cref="StreamContent"/> object calls <see cref="Dispose"/> on the provided Stream object when <see cref="StreamContent.Dispose"/> is called.
        /// </remarks>
        public StreamContent(
            Stream content,
            int bufferSize)
        {
            _content = content ?? throw new ArgumentNullException();

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _bufferSize = bufferSize;

            if (content.CanSeek)
            {
                _startPosition = content.Position;
            }
        }

        /// <inheritdoc/>
        protected override void SerializeToStream(Stream stream)
        {
            PrepareContent();

            if (_content is IKnowWhenDone knowWhenDone)
            {
                // special case for InputNetworkStreamWrapper (which implements IKnowWhenDone), because Read()
                // returns 0 when there is no data available (rather than blocking, which would be standard), so
                // something like CopyTo would stop reading before the response is finished.

                byte[] buffer = new byte[_bufferSize];
                int read;
                int totalRead = 0;
                long contentLength = Headers.ContentLength;

                // occurs when there is no Content-Length header (i.e. chunked response)
                if (contentLength < 0)
                {
                    if (TryComputeLength(out long possibleLength))
                    {
                        contentLength = possibleLength;
                    }
                    else
                    {
                        contentLength = int.MaxValue;
                    }
                }

                bool isDone = false;
                while ((totalRead < contentLength) && !isDone)
                {
                    read = _content.Read(buffer, 0, _bufferSize);
                    isDone = knowWhenDone.IsDone;

                    if (read == 0 && !isDone)
                    {
                        // need to let the native layer get more data
                        Thread.Sleep(10);
                    }
                    else if (read > 0)
                    {
                        totalRead += read;
                        stream.Write(buffer, 0, read);
                    }
                }
            }
            else
            {
                _content.CopyTo(stream);
            }
        }

        /// <inheritdoc/>
        protected override Stream CreateContentReadStream()
        {
            if (_content is IKnowWhenDone)
            {
                // Special case for InputNetworkStreamWrapper:
                // Call the base method which buffers the entire response.
                // Due to the way InputNetworkStreamWrapper works, we can't really return the stream directly
                // (see comment in SerializeToStream)
                return base.CreateContentReadStream();
            }
            else
            {
                PrepareContent();
                return new ReadOnlyStream(_content);
            }
        }

        private void PrepareContent()
        {
            if (_contentConsumed)
            {
                if (!_content.CanSeek)
                {
                    throw new InvalidOperationException();
                }

                _content.Seek(_startPosition, SeekOrigin.Begin);
            }
            else
            {
                _contentConsumed = true;
            }
        }

        /// <inheritdoc/>
        protected internal override bool TryComputeLength(out long length)
        {
            if (!_content.CanSeek)
            {
                length = 0;
                return false;
            }

            length = _content.Length - _startPosition;

            return true;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
