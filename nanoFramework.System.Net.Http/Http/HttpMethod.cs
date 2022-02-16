﻿using System;

namespace System.Net.Http
{
    /// <summary>
    /// A helper class for retrieving and comparing standard HTTP methods and for creating new HTTP methods.
    /// </summary>
    /// <remarks>
    /// The most common usage of <see cref="HttpMethod"/> is to use one of the static properties on this class. However, if an app needs a different value for the HTTP method, the <see cref="HttpMethod"/> constructor initializes a new instance of the <see cref="HttpMethod"/> with an HTTP method that the app specifies.
    /// </remarks>
    public partial class HttpMethod
    {
        private readonly string _method;

        private static readonly HttpMethod s_getMethod = new("GET");
        private static readonly HttpMethod s_putMethod = new("PUT");
        private static readonly HttpMethod s_postMethod = new("POST");
        private static readonly HttpMethod s_deleteMethod = new("DELETE");
        private static readonly HttpMethod s_headMethod = new("HEAD");
        private static readonly HttpMethod s_optionsMethod = new("OPTIONS");
        private static readonly HttpMethod s_patchMethod = new("PATCH");

        /// <summary>
        /// Represents an HTTP GET protocol method.
        /// </summary>
        public static HttpMethod Get
        {
            get { return s_getMethod; }
        }

        /// <summary>
        /// Represents an HTTP PUT protocol method.
        /// </summary>
        public static HttpMethod Put
        {
            get { return s_putMethod; }
        }

        /// <summary>
        /// Represents an HTTP POST protocol method.
        /// </summary>
        public static HttpMethod Post
        {
            get { return s_postMethod; }
        }

        /// <summary>
        /// Represents an HTTP DELETE protocol method.
        /// </summary>
        public static HttpMethod Delete
        {
            get { return s_deleteMethod; }
        }

        /// <summary>
        /// Represents an HTTP HEAD protocol method.
        /// </summary>
        public static HttpMethod Head
        {
            get { return s_headMethod; }
        }

        /// <summary>
        /// Represents an HTTP OPTIONS protocol method.
        /// </summary>
        public static HttpMethod Options
        {
            get { return s_optionsMethod; }
        }

        /// <summary>
        /// Represents an HTTP PATCH protocol method.
        /// </summary>
        public static HttpMethod Patch
        {
            get { return s_patchMethod; }
        }

        /// <summary>
        /// An HTTP method.
        /// </summary>
        /// <value>An HTTP method represented as a <see cref="string"/>.</value>
        public string Method
        {
            get { return _method; }
        }

        /// <summary>
        /// Initializes a new instance of the HttpMethod class with a specific HTTP method.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// If an app needs a different value for the HTTP method from one of the static properties, the HttpMethod constructor initializes a new instance of the HttpMethod with an HTTP method that the app specifies.
        /// </remarks>
        public HttpMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentException();
            }

            _method = method;
        }
    }
}
