﻿//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Runtime.Native;
using System;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net.Http
{
    /// <summary>
    /// Initializes a new instance of the HttpClient class.
    /// </summary>
    public partial class HttpClient : HttpMessageInvoker
    {
        private const HttpCompletionOption DefaultCompletionOption = HttpCompletionOption.ResponseContentRead;
        private Version _defaultRequestVersion = HttpRequestMessage.DefaultRequestVersion;

        private bool _operationStarted;
        private bool _disposed;

        private CancellationTokenSource _pendingRequestsCts;
        private HttpRequestHeaders _headers;
        private Uri _baseAddress;
        private TimeSpan _timeout;

       // private Version _defaultRequestVersion = HttpRequestMessage.DefaultRequestVersion;

        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        /// <value>
        /// The headers which should be sent with each request.
        /// </value>
        /// <remarks>
        /// Headers set on this property don't need to be set on request messages again. <see cref="DefaultRequestHeaders"/> should not be modified while there are outstanding requests, because it is not thread-safe.
        /// </remarks>
        public HttpRequestHeaders DefaultRequestHeaders => _headers ??= new HttpRequestHeaders();

        /// <summary>
        /// Gets or sets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </summary>
        /// <value>
        /// The base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </value>
        /// <exception cref="ArgumentException">Value is null or it not an absolute Uniform Resource Identifier (URI).</exception>
        /// <exception cref="InvalidOperationException">An operation has already been started on the current instance.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has been disposed.</exception>
        public Uri BaseAddress
        {
            get => _baseAddress;

            set
            {
                // It's OK to not have a base address specified, but if one is, it needs to be absolute.
                if (value is not null
                    && !value.IsAbsoluteUri)
                {
                    throw new ArgumentException();
                }

                CheckDisposedOrStarted();

                _baseAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        /// <value>
        /// The timespan to wait before the request times out.
        /// </value>
        /// <remarks>
        /// <para>The default value is 100,000 milliseconds (100 seconds).</para>
        /// <para>To set an infinite timeout, set the property value to <see cref="Threading.Timeout.InfiniteTimeSpan"/>.</para>
        /// <para>
        /// A Domain Name System (DNS) query may take up to 15 seconds to return or time out. If your request contains a host name that requires resolution and you set <see cref="Timeout"/> to a value less than 15 seconds, it may take 15 seconds or more before a <see cref="WebException"/> is thrown to indicate a timeout on your request.
        /// </para>
        /// <para>
        /// The same timeout will apply for all requests using this <see cref="HttpClient"/> instance. You may also set different timeouts for individual requests using a CancellationTokenSource on a task.Note that only the shorter of the two timeouts will apply.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">Value is null or it not an absolute Uniform Resource Identifier (URI).</exception>
        /// <exception cref="InvalidOperationException">An operation has already been started on the current instance.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has been disposed.</exception>
        public TimeSpan Timeout
        {
            get => _timeout;

            set
            {
                if (value != Threading.Timeout.InfiniteTimeSpan && (value <= TimeSpan.Zero || value.TotalMilliseconds > int.MaxValue))
                {
                    throw new ArgumentOutOfRangeException();
                }

                CheckDisposedOrStarted();

                _timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the root CA certificate used to authenticate with https servers.
        /// This certificate is used only for https connections; http connections do not require this.
        /// </summary>
        /// <remarks>
        /// This property is an extension from the full .NET required by nanoFramework.
        /// </remarks>
        public X509Certificate HttpsAuthentCert { get; set; }

        /// <summary>
        /// Gets or sets the TLS/SSL protocol used by the <see cref="HttpClient"/> class.
        /// </summary>
        /// <value>
        /// One of the values defined in the <see cref="Security.SslProtocols"/> enumeration. Default value is <see cref="SslProtocols.Tls12"/>.
        /// </value>
        /// <remarks>
        /// This property is an extension from the full .NET required by nanoFramework.
        /// </remarks>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12;

        #region Constructors

        public HttpClient() : base(new HttpClientHandler(), true)
        {
            _timeout = Threading.Timeout.InfiniteTimeSpan;
            //_maxResponseContentBufferSize = HttpContent.MaxBufferSize;
            _pendingRequestsCts = new CancellationTokenSource();

        }

        #endregion Constructors

        #region REST Send Overloads

        /// <summary>
        /// Send a GET request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Request operation has already started.</exception>
        public HttpResponseMessage Get(string requestUri) => Send(new HttpRequestMessage(HttpMethod.Get, requestUri), DefaultCompletionOption);

        /// <summary>
        /// Send a GET request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns></returns>
        public HttpResponseMessage Get(string requestUri, CancellationToken cancellationToken) => Get(requestUri, DefaultCompletionOption);

        /// <summary>
        /// Send a GET request to the specified Uri.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="completionOption"></param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="InvalidOperationException">Request operation has already started.</exception>
        public HttpResponseMessage Get(string requestUri, HttpCompletionOption completionOption) => Send(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);

        /// <summary>
        /// Send a GET request to the specified Uri.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="completionOption"></param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="InvalidOperationException">Request operation has already started.</exception>
        public HttpResponseMessage Get(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken) => Send(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);

      

        #endregion

        #region Advanced Send Overloads

        private HttpResponseMessage Send(
            HttpRequestMessage request,
            HttpCompletionOption completionOption)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            if (request.SetIsUsed())
            {
                throw new InvalidOperationException();
            }

            var uri = request.RequestUri;

            if (uri == null)
            {
                if (_baseAddress == null)
                {
                    throw new InvalidOperationException();
                }

                request.RequestUri = _baseAddress;
            }
            else if (!uri.IsAbsoluteUri)
            {
                if (_baseAddress == null)
                {
                    throw new InvalidOperationException();
                }

                request.RequestUri = new Uri(_baseAddress, uri.AbsoluteUri);
            }

            if (_headers != null)
            {
                request.Headers.AddHeaders(_headers);
            }

            return SendWorker(request, completionOption);
        }

        private HttpResponseMessage SendWorker(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            // TODO
            //using (var lcts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken))
            {
                // need to pass to the HttpWebRequest:
                // - timeout
                // - SSL protocol
                // - CA root certs
                if (_handler is HttpClientHandler clientHandler)
                {
                    clientHandler.SetWebRequestTimeout(_timeout);
                    clientHandler.SetWebRequestSslProcol(SslProtocols);
                    clientHandler.SetWebRequestHttpAuthCert(HttpsAuthentCert);
                }


                // TODO
                //lcts.CancelAfter(_timeout);
                //HttpResponseMessage response = base.Send(request, lcts.Token);
                HttpResponseMessage response = base.Send(request);

                //
                // Read the content when default HttpCompletionOption.ResponseContentRead is set
                //
                if (response.Content != null && (completionOption & HttpCompletionOption.ResponseHeadersRead) == 0)
                {
                    response.Content.LoadIntoBuffer();
                }

                return response;
            }
        }

        #endregion

        #region helper methods
        private void SetOperationStarted()
        {
            // This method flags the HttpClient instances as "active". I.e. we executed at least one request (or are
            // in the process of doing so). This information is used to lock-down all property setters. Once a
            // Send operation is started, no property can be changed.
            if (!_operationStarted)
            {
                _operationStarted = true;
            }
        }

        private Uri CreateUri(string uri) => string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri) => new HttpRequestMessage(method, uri) { Version = _defaultRequestVersion };

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();

            if (_operationStarted)
            {
                throw new InvalidOperationException();
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }
        }

        //private static void ThrowForNullResponse(HttpResponseMessage response)
        //{
        //    if (response is null)
        //    {
        //        throw new InvalidOperationException();
        //    }
        //}

        //private static bool ShouldBufferResponse(
        //    HttpCompletionOption completionOption,
        //    HttpRequestMessage request) =>
        //    completionOption == HttpCompletionOption.ResponseContentRead
        //    && !string.Equals(request.Method.Method, "HEAD");

        //private void PrepareRequestMessage(HttpRequestMessage request)
        //{
        //    Uri requestUri = null;

        //    if ((request.RequestUri == null) && (_baseAddress == null))
        //    {
        //        throw new InvalidOperationException();
        //    }

        //    if (request.RequestUri == null)
        //    {
        //        requestUri = _baseAddress;
        //    }
        //    else
        //    {
        //        // If the request Uri is an absolute Uri, just use it. Otherwise try to combine it with the base Uri.
        //        if (!request.RequestUri.IsAbsoluteUri)
        //        {
        //            if (_baseAddress == null)
        //            {
        //                throw new InvalidOperationException();
        //            }
        //            else
        //            {
        //                requestUri = new Uri(_baseAddress, request.RequestUri.AbsoluteUri);
        //            }
        //        }
        //    }

        //    // We modified the original request Uri. Assign the new Uri to the request message.
        //    if (requestUri != null)
        //    {
        //        request.RequestUri = requestUri;
        //    }

        //    // Add default headers
        //    if (_headers != null)
        //    {
        //        request.Headers.AddHeaders(_headers);
        //    }

        //    // TODO
        //    //return SendWorker(request, completionOption, cancellationToken);
        //}

        //private SendCancellationTokenSource PrepareCancellationTokenSource(CancellationToken cancellationToken)
        //{
        //    // We need a CancellationTokenSource to use with the request.  We always have the global
        //    // _pendingRequestsCts to use, plus we may have a token provided by the caller, and we may
        //    // have a timeout.  If we have a timeout or a caller-provided token, we need to create a new
        //    // CTS (we can't, for example, timeout the pending requests CTS, as that could cancel other
        //    // unrelated operations).  Otherwise, we can use the pending requests CTS directly.

        //    // Snapshot the current pending requests cancellation source. It can change concurrently due to cancellation being requested
        //    // and it being replaced, and we need a stable view of it: if cancellation occurs and the caller's token hasn't been canceled,
        //    // it's either due to this source or due to the timeout, and checking whether this source is the culprit is reliable whereas
        //    // it's more approximate checking elapsed time.
        //    CancellationTokenSource pendingRequestsCts = _pendingRequestsCts;

        //    bool hasTimeout = _timeout != Threading.Timeout.InfiniteTimeSpan;

        //    if (hasTimeout || cancellationToken.CanBeCanceled)
        //    {
        //        // TODO
        //        //CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, pendingRequestsCts.Token);
        //        CancellationTokenSource cts = new CancellationTokenSource();

        //        if (hasTimeout)
        //        {
        //            cts.CancelAfter(_timeout);
        //        }

        //        return new SendCancellationTokenSource(cts, true, pendingRequestsCts);
        //    }

        //    return new (pendingRequestsCts, false, pendingRequestsCts);
        //}


        internal class SendCancellationTokenSource
        {
            public CancellationTokenSource TokenSource { get; }
            public bool DisposeTokenSource { get; }
            public CancellationTokenSource PendingRequestsCts { get; }


            public SendCancellationTokenSource(
                CancellationTokenSource tokenSource,
                bool disposeTokenSource,
                CancellationTokenSource pendingRequestsCts)
            {
                TokenSource = tokenSource;
                DisposeTokenSource = disposeTokenSource;
                PendingRequestsCts = pendingRequestsCts;
            }
        }

        #endregion
    }
}
