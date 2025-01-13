using System.IO;

namespace System.Net.Http
{
    /// <summary>
    /// A wrapper stream that provides read-only access to an underlying stream.
    /// </summary>
    public class ReadOnlyStream : Stream
    {
        private readonly Stream _innerStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyStream"/> class.
        /// </summary>
        /// <param name="innerStream">The underlying stream to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="innerStream"/> is <c>null</c>.</exception>
        public ReadOnlyStream(Stream innerStream)
        {
            _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
        }

        /// <inheritdoc />
        public override bool CanRead
        {
            get { return _innerStream.CanRead; }
        }

        /// <inheritdoc />
        public override bool CanSeek
        {
            get { return _innerStream.CanSeek; }
        }

        /// <inheritdoc />
        public override long Length
        {
            get { return _innerStream.Length; }
        }

        /// <inheritdoc />
        /// <remarks>This stream is read-only and always returns <c>false</c>.</remarks>
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Position
        {
            get { return _innerStream.Position; }
            set { _innerStream.Position = value; }
        }

        /// <inheritdoc />
        public override int ReadTimeout
        {
            get { return _innerStream.ReadTimeout; }
            set { _innerStream.ReadTimeout = value; }
        }

        /// <inheritdoc />
        public override bool CanTimeout
        {
            get { return _innerStream.CanTimeout; }
        }

        /// <inheritdoc />
        /// <remarks>Setting or getting this property is not supported for a read-only stream.</remarks>
        /// <exception cref="NotSupportedException">Always thrown when accessing this property.</exception>
        public override int WriteTimeout
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerStream.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        /// <remarks>Flushing of a read-only stream is not supported.</remarks>
        /// <exception cref="NotSupportedException">Always thrown when called.</exception>
        public override void Flush() => throw new NotSupportedException();

        /// <inheritdoc />
        public override int Read(SpanByte buffer)
        {
            return _innerStream.Read(buffer);
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        /// <inheritdoc />
        public override int ReadByte()
        {
            return _innerStream.ReadByte();
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        /// <inheritdoc />
        /// <remarks>Setting the length of a read-only stream is not supported.</remarks>
        /// <exception cref="NotSupportedException">Always thrown when called.</exception>
        public override void SetLength(long value) =>
            throw new NotSupportedException();

        /// <inheritdoc />
        /// <remarks>Writing to a read-only stream is not supported.</remarks>
        /// <exception cref="NotSupportedException">Always thrown when called.</exception>
        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();

        /// <inheritdoc />
        /// <remarks>Writing a byte to a read-only stream is not supported.</remarks>
        /// <exception cref="NotSupportedException">Always thrown when called.</exception>
        public override void WriteByte(byte value) =>
            throw new NotSupportedException();
    }
}
