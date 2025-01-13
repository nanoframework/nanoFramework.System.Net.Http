//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;

namespace HttpUnitTests
{
    [TestClass]
    public class UriUnitTests
    {
        [TestMethod]
        public void NullCtor_Should_Throw_Exception()
        {
            Assert.ThrowsException(typeof(ArgumentNullException),
                () => _ = new Uri(null), "Expected ArgumentNullException");
        }

        [TestMethod]
        [DataRow("ftp://username:password@ftp.example.com")] // ftp with username and password
        [DataRow("ftp://ftp.example.com/folder/file.txt")] // ftp with file path
        [DataRow("ftp://username:password@ftp.example.com:22/folder/file.pe")] // ftp with username and password, custom post, and file
        [DataRow("\tftp://abc.com   ")] // ftp with tabs
        [DataRow("file://")] // Empty file path
        [DataRow("file:///")]
        [DataRow("file:////")]
        [DataRow("file://c")]
        [DataRow("file:///c")]
        [DataRow("file:////c")]
        [DataRow("file:///C:/path/to/file.txt")] // local file path
        [DataRow("file:///home/user/file.txt")] // unix file path
        [DataRow("file://hostname/share/path/file.txt")] // network share
        [DataRow("file:///\\\\server\\folder\\file.txt")] // Windows share style file
        [DataRow("file:///c:\\rbllog")] // Windows drive style file
        [DataRow("mailto:user@example.com")] // email
        [DataRow("mailto:user@example.com?subject=Hello&body=World")] // email with subject and body
        [DataRow("postgresql://user:password@localhost:5432/database")] // postgresql
        [DataRow("mongodb://user:password@localhost:27017/database")] // mongo db
        [DataRow("ws://example.com/socket")] // web socket
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5")] // web socket with query, path, and port
        [DataRow("wss://ws.pusherapp.com/app/?client=js&version=1.9.3&protocol=5")] // secure web socket with query, path, and port
        [DataRow("ssh://user@hostname:22")] // SSH
        [DataRow("telnet://user:pass@hostname:22")] // Telnet
        [DataRow("sftp://user@hostname/path/to/file")] // SFTP
        [DataRow("rtsp://media.example.com/video")] // RTSP
        [DataRow("http://[2001:db8::1]:8080")] // http with ipv6
        [DataRow("ftp://[2001:db8::1]/folder")] // ftp with ipv6
        [DataRow("urn:isbn:0262531968")] // isbn urn
        [DataRow("urn:uuid:123e4567-e89b-12d3-a456-426614174000")] // uuid urn
        [DataRow("urn:ietf:rfc:2648")] // IETF namespace urn
        [DataRow("urn:uci:I001+SBSi-B10000083052")] // UCI urn
        [DataRow("urn:isan:0000-0000-9E59-0000-O-0000-0000-2")] // ISAN urn
        [DataRow("urn:issn:0167-6423")] // ISSN urn
        [DataRow("urn:mpeg:mpeg7:schema:2001")] // mpeg urn
        [DataRow("urn:oid:2.216.840")] // urn oid
        [DataRow("uuid:6e8bc430-9c3a-11d9-9669-0800200c9a66")] // UUID
        [DataRow("geo:47.645812,-122.134544")] // geo location
        [DataRow("news:comp.infosystems.www.servers.unix")] // news
        [DataRow("tel:+1-816-555-1212")] // Telephone number
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256")] // h323 with user, ipv4, and query values
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one")] // ldap
        [DataRow("http://www.co1ntoso.com")] // Http
        [DataRow("http://user:password@www.co5ntoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Http with bells and whistles
        [DataRow("https://www.co3ntoso.com/Home/Index.htm?q1=v1&q2=v2")] // Https with path and query
        [DataRow("https://www.co4ntoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Https with path, query, fragment, and standard port
        [DataRow("https://www.co4ntoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Https with path, query, fragment, and non-standard port
        [DataRow("https://user:password@www.co5ntoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Https with user credentials and non-standard port
        [DataRow("h1tp://foo.com")] // typos
        [DataRow("http5://foo.com")] // typos
        [DataRow("HtTp://example.com")] // mixed case http
        [DataRow("FTp://example.com")] // mixed case ftp
        [DataRow("http://example.com/path%20with%20spaces")] // unusual but valid characters
        [DataRow("http://example.com/?q=complex%20query%21%40%23")] // including special characters
        [DataRow("file:///C:/path%20with%20spaces/file.txt")] // file path with encoded spaces
        [DataRow("magnet:?xt=urn:btih:1234567890abcdef")]
        [DataRow("http://example.com/path?redirect=192.168.0.1#section")] // ip in query
        [DataRow("http://example.com/path#192.168.0.1")] // ip in fragment
        [DataRow("http://example.com#")] // empty fragment
        [DataRow("http://example.com/../../etc/passwd")] // path traversal characters
        [DataRow("blob:http://example.com/550e8400-e29b-41d4-a716-446655440000")] // blob uri
        //[DataRow("urn:example:animal:ferret:nose?color=black")] // urn with query parameters  **urn parsing is not fully implemented
        public void UriCtor_Should_Not_Throw_Exception(string uri)
        {
            Console.WriteLine("uri: " + uri);
            Uri createdUri = new(uri);

            Assert.IsNotNull(createdUri);
        }

        [TestMethod]
        [DataRow("", typeof(ArgumentNullException), "ExpectedArgumentNullException")]
        [DataRow("foo", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("file:///c:", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("http:abc/d/", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("file:/server", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("http:", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("1ttp://foo.com", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("h@tp://foo.com", typeof(ArgumentException), "Expected ArgumentException")]
        public void UriCtor_Invalid_Should_Throw_Exception(string uri, Type exceptionType, string message)
        {
            try
            {
                Console.WriteLine("uri: " + uri);
                Assert.ThrowsException(exceptionType, () =>
                {
                    _ = new Uri(uri);
                }, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "#FragmentName")]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", "")]
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256", "")]
        public void UriCtor_Should_Parse_Fragment(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.Fragment, $"actual: {uri.Fragment} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("http://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", false)]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", false)]
        [DataRow("file://server/filename.ext", true)]
        public void UriCtor_IsUnc_Should_Be_Valid(string uriString, bool expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.IsUnc, $"actual: {uri.IsUnc} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("http://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", false)]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", false)]
        [DataRow("file://server/filename.ext", true)]
        public void UriCtor_IsFile_Should_Be_Valid(string uriString, bool expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.IsFile, $"actual: {uri.IsFile} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", false)]
        [DataRow("file://server/filename.ext", false)]
        [DataRow("file:///C:/path/to/file.txt", true)]
        [DataRow("mailto:John.Doe@example.com", false)]
        public void UriCtor_IsLoopback_Should_Be_Valid(string uriString, bool expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.IsLoopback, $"actual: {uri.IsLoopback} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName")]
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5")]
        [DataRow("\tftp://abc.com   ")]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one")]
        [DataRow("mailto:John.Doe@example.com")]
        [DataRow("news:comp.infosystems.www.servers.unix")]
        [DataRow("tel:+1-816-555-1212")]
        public void UriCtor_OriginalString_Should_Be_Valid(string uriString)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(uriString, uri.OriginalString, $"actual: {uri.OriginalString} expected: {uriString}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", 443)]
        [DataRow("ftp://user:password@ftp.contoso.com/directory/file.txt", 21)]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", 389)]
        [DataRow("news:comp.infosystems.www.servers.unix", -1)]
        [DataRow("tel:+1-816-555-1212", -1)]
        [DataRow("file:///home/user/file.txt", -1)]
        public void UriCtor_Port_Should_Be_Valid(string uriString, int expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.Port, $"actual: {uri.Port} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("http://example.com:5001/path#192.168.0.1", false)]
        [DataRow("http://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName", true)]
        [DataRow("ssh://user@hostname:22", true)]
        [DataRow("rtsp://media.example.com/video", true)]
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5", true)]
        [DataRow("ftp://[2001:db8::1]/folder", true)]
        [DataRow("telnet://192.0.2.16:80/", false)]
        [DataRow("file:///home/user/file.txt", true)]

        public void UriCtor_IsDefaultPort_Should_Return_Default_Port(string uriString, bool expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.IsDefaultPort, $"actual: {uri.IsDefaultPort} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "user:password")]
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5", "")]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", "user:password")]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", "")]
        [DataRow("mailto:John.Doe@example.com", "John.Doe")]
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256", "")]
        public void UriCtor_UserInfo_Should_Be_Valid(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.UserInfo, $"actual: {uri.UserInfo} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "https")]
        [DataRow("ws://ws.pusherapp.com/app/?client=js&version=1.9.3&protocol=5", "ws")]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", "ftp")]
        [DataRow("\thttp://abc.com   ", "http")]
        [DataRow("mailto:John.Doe@example.com", "mailto")]
        [DataRow("h323:caller@192.168.1.100", "h323")]
        [DataRow("file:///C:/path/to/file.txt", "file")]
        public void UriCtor_Scheme_Should_Be_Valid(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.Scheme, $"actual: {uri.Scheme} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "www.contoso.com")]
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5", "ws.pusherapp.com")]
        [DataRow("ftp://user:password@ftp.contoso.com/directory/file.txt", "ftp.contoso.com")]
        [DataRow("\tftp://abc.com   ", "abc.com")]
        [DataRow("file:///c", "")]
        [DataRow("file://c", "c")]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", "[2001:db8::7]")]
        [DataRow("mailto:John.Doe@example.com", "example.com")]
        [DataRow("news:comp.infosystems.www.servers.unix", "")]
        [DataRow("telnet://192.0.2.16:80/", "192.0.2.16")]
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256", "")]
        public void UriCtor_Host_Should_Be_Valid(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.Host, $"actual: {uri.Host} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "/Home/Index.htm")]
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5", "/app/")]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", "/directory/file.txt")]
        [DataRow("\tftp://abc.com   ", "/")]
        [DataRow("file:///", "/")]
        [DataRow("file:///c", "/c")]
        [DataRow("file:////", "//")]
        [DataRow("file://c", "/")]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", "/c=GB")]
        [DataRow("mailto:John.Doe@example.com", "")]
        [DataRow("news:comp.infosystems.www.servers.unix", "comp.infosystems.www.servers.unix")]
        [DataRow("tel:+1-816-555-1212", "+1-816-555-1212")]
        [DataRow("telnet://192.0.2.16:80/", "/")]
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256", "caller@192.168.1.100")]
        public void UriCtor_Absolute_Path_Should_Be_Valid(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com/Home/Index.htm?q1=v1&q2=v2#FragmentName", 2)]
        [DataRow("file:///", 1)]
        [DataRow("file://c", 2)]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", 4)]
        [DataRow("telnet://192.0.2.16:80/", 3)]
        [DataRow("tel:+1-816-555-1212", 0)]
        [DataRow("mailto:John.Doe@example.com", 2)]
        [DataRow("\tftp://abc.com   ", 2)]
        [DataRow("h323:caller@192.168.1.100", 0)]
        public void UriCtor_HostNameType_Should_Be_Valid(string uriString, int expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, (int)uri.HostNameType, $"actual: {uri.HostNameType} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", 3)]
        [DataRow("file:///", 2)]
        [DataRow("\tftp://abc.com   ", 2)]
        [DataRow("file:////c", 2)]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", 2)]
        [DataRow("mailto:John.Doe@example.com", 1)]
        public void UriCtor_SegmentsLength_Should_Be_Valid(string uriString, int expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "?q1=v1&q2=v2")]
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5", "?client=js&version=1.9.3&protocol=5")]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", "")]
        [DataRow("\tftp://abc.com   ", "")]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", "?objectClass?one")]
        [DataRow("telnet://192.0.2.16:80/", "")]
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256", "?codec=g729&bandwidth=256")]
        [DataRow("file:///", "")]
        public void UriCtor_Query_Should_Be_Valid(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.Query, $"actual: {uri.Query} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "/Home/Index.htm?q1=v1&q2=v2")]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", "/directory/file.txt")]
        [DataRow("\tftp://abc.com   ","/")]
        [DataRow("file:////c", "/")]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", "/c=GB?objectClass?one")]
        [DataRow("mailto:user@example.com?subject=Hello&body=World", "?subject=Hello&body=World")]
        [DataRow("news:comp.infosystems.www.servers.unix", "comp.infosystems.www.servers.unix")]
        [DataRow("tel:+1-816-555-1212", "+1-816-555-1212")]
        public void UriCtor_PathAndQuery_Should_Be_Valid(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            Assert.AreEqual(expectedValue, uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: {expectedValue}");
        }

        [TestMethod]
        [DataRow("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", "https://user:password@www.contoso.com/Home/Index.htm?q1=v1&q2=v2#FragmentName")]
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5", "ws://ws.pusherapp.com/app/?client=js&version=1.9.3&protocol=5")]
        [DataRow("ftp://user:password@ftp.contoso.com:21/directory/file.txt", "ftp://user:password@ftp.contoso.com/directory/file.txt")]
        [DataRow("\tftp://abc.com   ", "ftp://abc.com/")]
        [DataRow("file://", "file:///")]
        [DataRow("file:///", "file:///")]
        [DataRow("file:////", "file:////")]
        [DataRow("file:///c", "file:///c")]
        [DataRow("file://c", "file://c/")]
        [DataRow("file:////c", "file://c/")]
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one", "ldap://[2001:db8::7]/c=GB?objectClass?one")]
        [DataRow("mailto:John.Doe@example.com", "mailto:John.Doe@example.com")]
        [DataRow("news:comp.infosystems.www.servers.unix", "news:comp.infosystems.www.servers.unix")]
        [DataRow("tel:+1-816-555-1212", "tel:+1-816-555-1212")]
        [DataRow("telnet://192.0.2.16:80/", "telnet://192.0.2.16:80/")]
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256", "h323:caller@192.168.1.100?codec=g729&bandwidth=256")]
        public void UriCtor_AbsoluteUri_Should_Be_Valid(string uriString, string expectedValue)
        {
            Console.WriteLine(uriString);
            Uri uri = new(uriString);
            PrintUriPropertiesToConsole(uri);

            Assert.IsTrue(uri.IsAbsoluteUri);
            Assert.AreEqual(expectedValue, uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: {expectedValue}");
        }


        [TestMethod]
        public void UriCtor_CombineCtor_Should_ParseIntoUri()
        {
            Uri baseUri = new(@"http://www.contoso.com/");
            Uri uri = new(baseUri, "catalog/shownew.htm?date=today");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/catalog/shownew.htm", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /catalog/shownew.htm");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"www.contoso.com", uri.Host, $"actual: {uri.Host} expected: www.contoso.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString, $"actual: {uri.OriginalString} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"/catalog/shownew.htm?date=today", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /catalog/shownew.htm?date=today");
            Assert.AreEqual(80, uri.Port, $"actual: {uri.Port} expected: {80}");
            Assert.AreEqual(@"?date=today", uri.Query, $"actual: {uri.Query} expected: ?date=today");
            Assert.AreEqual(@"http", uri.Scheme, $"actual: {uri.Scheme} expected: http");
            Assert.AreEqual(3, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 3");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_CombineCtor_Absolute_Should_ParseIntoUri()
        {
            Uri baseUri = new(@"http://www.contoso.com/", UriKind.Absolute);
            var uri = new Uri(baseUri, "catalog/shownew.htm?date=today");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/catalog/shownew.htm", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /catalog/shownew.htm");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"www.contoso.com", uri.Host, $"actual: {uri.Host} expected: www.contoso.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString, $"actual: {uri.OriginalString} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"/catalog/shownew.htm?date=today", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /catalog/shownew.htm?date=today");
            Assert.AreEqual(80, uri.Port, $"actual: {uri.Port} expected: {80}");
            Assert.AreEqual(@"?date=today", uri.Query, $"actual: {uri.Query} expected: ?date=today");
            Assert.AreEqual(@"http", uri.Scheme, $"actual: {uri.Scheme} expected: http");
            Assert.AreEqual(3, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 3");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_Ctor_RelativeUri_Should_ParseIntoUri()
        {
            Uri uri = new("catalog/shownew.htm?date=today", UriKind.Relative);

            Console.WriteLine($"Fragment: {uri.Fragment}");
            Console.WriteLine($"HostNameType: {uri.HostNameType}");
            Console.WriteLine($"IsAbsoluteUri: {uri.IsAbsoluteUri}");
            Console.WriteLine($"IsDefaultPort: {uri.IsDefaultPort}");
            Console.WriteLine($"IsFile: {uri.IsFile}");
            Console.WriteLine($"OriginalString: {uri.OriginalString}");
            Console.WriteLine($"Query: {uri.Query}");
            Console.WriteLine($"UserInfo: {uri.UserInfo}");

            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.AbsolutePath; }, "No exception thrown when accessing AbsolutePath");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.AbsoluteUri; }, "No exception thrown when accessing AbsoluteUri");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Host; }, "No exception thrown when accessing Host");
            Assert.AreEqual(UriHostNameType.Unknown, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Unknown}");
            Assert.IsFalse(uri.IsAbsoluteUri, "IsAbsoluteUri was true, but expected false");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was true, but expected false");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.IsLoopback; }, "No exception thrown when accessing IsLoopback");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.IsUnc; }, "No exception thrown when accessing IsUnc");
            Assert.AreEqual(@"catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.PathAndQuery; }, "No exception thrown when PathAndQuery");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Port; }, "No exception thrown when accessing Port");
            Assert.AreEqual(@"?date=today", uri.Query, $"actual: {uri.Query} expected: ?date=today");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Scheme; }, "No exception thrown when accessing Scheme");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Segments.Length; }, "No exception thrown when accessing Scheme");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        private static void PrintUriPropertiesToConsole(Uri uri)
        {
            Console.WriteLine($"AbsolutePath: {uri.AbsolutePath}");
            Console.WriteLine($"AbsoluteUri: {uri.AbsoluteUri}");
            Console.WriteLine($"Fragment: {uri.Fragment}");
            Console.WriteLine($"Host: {uri.Host}");
            Console.WriteLine($"HostNameType: {uri.HostNameType}");
            Console.WriteLine($"IsAbsoluteUri: {uri.IsAbsoluteUri}");
            Console.WriteLine($"IsDefaultPort: {uri.IsDefaultPort}");
            Console.WriteLine($"IsFile: {uri.IsFile}");
            Console.WriteLine($"IsLoopback: {uri.IsLoopback}");
            Console.WriteLine($"IsUnc: {uri.IsUnc}");
            Console.WriteLine($"OriginalString: {uri.OriginalString}");
            Console.WriteLine($"PathAndQuery: {uri.PathAndQuery}");
            Console.WriteLine($"Port: {uri.Port}");
            Console.WriteLine($"Query: {uri.Query}");
            Console.WriteLine($"Scheme: {uri.Scheme}");
            Console.WriteLine($"Segments: {uri.Segments.Length}");
            Console.WriteLine($"UserInfo: {uri.UserInfo}");
        }
    }
}
