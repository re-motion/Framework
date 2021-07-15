// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.Utilities
{

  [TestFixture]
  public class UrlUtilityTest
  {
    private Encoding _currentEncoding;

    [SetUp]
    public virtual void SetUp ()
    {
      _currentEncoding = System.Text.Encoding.UTF8;
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithEmptyPath_ResultingPathIsEmpty ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, ""),
          Is.EqualTo (""));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootedPath_ResultingPathIsInput ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "/AppDir/path"),
          Is.EqualTo ("/AppDir/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootedPathUsingBackslashes_ResultingPathIsInputWithForwardSlashes ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "\\AppDir\\path"),
          Is.EqualTo ("/AppDir/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootedPathUsingBackslashesAndForwardSlashes_ResultingPathIsInputWithForwardSlashes ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "/AppDir\\path"),
          Is.EqualTo ("/AppDir/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRelativePath_ResultingPathIsAppendedToRequestFolderWithOriginalCasing ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/AppdIr/folder/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "part1/part2"),
          Is.EqualTo ("/AppdIr/folder/part1/part2"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRelativePathAndBackslashes_ResultingPathIsAppendedToRequestFolderWithOriginalCasingAndUsesForwardSlashes ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/AppdIr/folder/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "part1\\part2"),
          Is.EqualTo ("/AppdIr/folder/part1/part2"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRelativePath_RequestToRoot ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost"), "/");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "part1/part2"),
          Is.EqualTo ("/part1/part2"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRelativePath_RequestToRootWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/"), "/");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "part1/part2"),
          Is.EqualTo ("/part1/part2"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRelativePath_UsesPathFromRequestUrl ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/_%20_%C3%84_%C3%A4_/_%D6_%f6_/"), "/_ _Ä_ä_");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "path"),
          Is.EqualTo ("/_%20_%C3%84_%C3%A4_/_%D6_%f6_/path"),
          "This test fails in Visual Studio because some environments change the URL to use upper case escape sequences. This started somewhere in 2015. Other environments (nunit console, IIS) seem unaffected.");
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~"),
          Is.EqualTo ("/appDir/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndTrailingSlash_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/"),
          Is.EqualTo ("/appDir/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndMultiplePathParts_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/part1/part2"),
          Is.EqualTo ("/appDir/part1/part2"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndPathIshDot_SkipsPart ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/."),
          Is.EqualTo ("/appDir"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndBeginsWithDot_SkipsPart ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/./part2"),
          Is.EqualTo ("/appDir/part2"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndPathEndsWithDot_SkipsPart ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/part1/."),
          Is.EqualTo ("/appDir/part1"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndSecondRootOperator_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/~"),
          Is.EqualTo ("/appDir/~"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndSecondRootOperator_AndTrailingSlash_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/~/"),
          Is.EqualTo ("/appDir/~/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndContainsRootOperatorAndBackslashes_ContainsRootOperatorAndUsesForwardSlashes ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/~/part1/part2/"),
          Is.EqualTo ("/appDir/~/part1/part2/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndTraversesRelativeParentDirectory_CollapsesDirectories ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/part1/../part2/"),
          Is.EqualTo ("/appDir/part2/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndTraversesAppDirectory_CollapsesDirectories ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/../part2/"),
          Is.EqualTo ("/part2/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndContainsRootOperator_ContainsRootOperator ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/~/part1/part2/"),
          Is.EqualTo ("/appDir/~/part1/part2/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndTraversesRelativeParentDirectoryPastRootOperator_DoesNotContainRootOperator ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/~/part1/../../part2/"),
          Is.EqualTo ("/appDir/part2/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndTraversesAppDirectoryPastRootOperator_DoesNotContainRootOperator ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/~/../../part1/part2/"),
          Is.EqualTo ("/part1/part2/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_AndEndsPathWithTrailingSlash_KeepsTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/part1/part2/"),
          Is.EqualTo ("/appDir/part1/part2/"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_UsesVirtualApplicationPathFromUrl ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/AppdiR/file?x=y"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/AppdiR/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_RequestToRoot ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost"), "/");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_RequestToRootWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/"), "/");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_RequestToRootWithApplicationPathNull_SubstitutesTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost"), null);

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_RequestToVirtualApplicationPathRoot ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/appDir/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_RequestToVirtualApplicationPathRootWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/"), "/appDir");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/appDir/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_UsesVirtualApplicationPathFromUrl_ComparesUsingDecodedPath ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/_%20_%C3%84_%C3%A4_/file"), "/_ _Ä_ä_");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/_%20_%C3%84_%C3%A4_/path"),
          "This test fails in Visual Studio because some environments change the URL to use upper case escape sequences. This started somewhere in 2015. Other environments (nunit console, IIS) seem unaffected.");
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_UsesVirtualApplicationPathFromUrl_ComparesUsingDecodedPathWithTrailingSlash ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/_%20_%C3%84_%C3%A4_/file"), "/_ _Ä_ä_/");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/_%20_%C3%84_%C3%A4_/path"),
          "This test fails in Visual Studio because some environments change the URL to use upper case escape sequences. This started somewhere in 2015. Other environments (nunit console, IIS) seem unaffected.");
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_UsesVirtualApplicationPathFromRootUrl_ComparesUsingDecodedPath ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/_%20_%C3%84_%C3%A4_"), "/_ _Ä_ä_");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/_%20_%C3%84_%C3%A4_/path"),
          "This test fails in Visual Studio because some environments change the URL to use upper case escape sequences. This started somewhere in 2015. Other environments (nunit console, IIS) seem unaffected.");
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_UsesVirtualApplicationPathFromRootUrlWithTrailingSlash_ComparesUsingDecodedPath ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/_%20_%C3%84_%C3%A4_/"), "/_ _Ä_ä_");

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/_%20_%C3%84_%C3%A4_/path"),
          "This test fails in Visual Studio because some environments change the URL to use upper case escape sequences. This started somewhere in 2015. Other environments (nunit console, IIS) seem unaffected.");
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithRootOperator_DecodedVirtualApplicationPathsDoNotMatch_ThrowsInvalidOperationException ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/AppDir1/file"), "/AppDir");

      Assert.That (
          () =>
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Throws.InvalidOperationException
                .With.Message.StartsWith ("Cannot calculate the application path when the request URL does not start with the application path."));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithCookiebasedSession_ResolvedUrl ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      var sessionStub = new Mock<HttpSessionStateBase>();
      sessionStub.Setup (_ => _.IsCookieless).Returns (false);
      httpContextStub.Setup (_ => _.Session).Returns (sessionStub.Object);

      Assert.That (
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Is.EqualTo ("/appDir/path"));
    }

    [Test]
    public void ResolveUrlCaseSensitive_WithCookielessSession_ThrowsInvalidOperationException ()
    {
      var httpContextStub = CreateHttpContextStub (new Uri ("http://localhost/appDir/file?x=y"), "/appDir");

      var sessionStub = new Mock<HttpSessionStateBase>();
      sessionStub.Setup (_ => _.IsCookieless).Returns (true);
      httpContextStub.Setup (_ => _.Session).Returns (sessionStub.Object);

      Assert.That (
          () =>
          UrlUtility.ResolveUrlCaseSensitive (httpContextStub.Object, "~/path"),
          Throws.InvalidOperationException
                .With.Message.EqualTo ("Cookieless sessions are not supported for resolving URLs."));
    }

    private Mock<HttpContextBase> CreateHttpContextStub (Uri url, string applicationPath)
    {
      var httpRequestStub = new Mock<HttpRequestBase>();
      httpRequestStub.Setup (_ => _.Url).Returns (url);
      httpRequestStub.Setup (_ => _.ApplicationPath).Returns (applicationPath);

      var httpContextStub = new Mock<HttpContextBase>();
      httpContextStub.Setup (_ => _.Request).Returns (httpRequestStub.Object);

      var httpContextProviderStub = new Mock<IHttpContextProvider>();
      httpContextProviderStub.Setup (_ => _.GetCurrentHttpContext()).Returns (httpContextStub.Object);

      return httpContextStub;
    }

    [Test]
    public void AddParameter_ToEmptyUrl ()
    {
      string url = string.Empty;
      string parameter = "Parameter1";
      string value = "Value1ä#";

      string expected = string.Format (
          "{0}?{1}={2}",
          url,
          parameter,
          HttpUtility.UrlEncode (value, _currentEncoding));

      string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void AddParameter_ToUrl ()
    {
      string url = "http://localhost/Default.html";
      string parameter = "Parameter1";
      string value = "Value1ä#";

      string expected = string.Format (
          "{0}?{1}={2}",
          url,
          parameter,
          HttpUtility.UrlEncode (value, _currentEncoding));

      string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void AddParameter_WithParameterValueIsEmpty_ReturnsUrlWithEmptyParameterValue ()
    {
      string url = "http://localhost/Default.html";
      string parameter = "Parameter1";

      string expected = string.Format ("{0}?{1}=", url, parameter);

      string actual = UrlUtility.AddParameter (url, parameter, "", _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void AddParameter_WithParameterValueIsNull_ThrowsArgumentNullException ()
    {
      string url = "http://localhost/Default.html";
      string parameter = "Parameter1";

      Assert.That (() => UrlUtility.AddParameter (url, parameter, null, _currentEncoding), Throws.Exception.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void AddParameter_WithParameterNameIsNull_ReturnsUrlWithOnlyTheParameterValue ()
    {
      string url = "http://localhost/Default.html";
      string value = "Value1ä#";

      string expected = string.Format (
          "{0}?{1}",
          url,
          HttpUtility.UrlEncode (value, _currentEncoding));

      string actual = UrlUtility.AddParameter (url, null, value, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void AddParameter_WithParameterNameIsEmpty_ThrowsArgumentException ()
    {
      string url = "http://localhost/Default.html";
      var value = "Value";

      Assert.That (
          () => UrlUtility.AddParameter (url, "", value, _currentEncoding),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo ("Parameter 'name' cannot be empty.", "name"));
    }

    [Test]
    public void AddParameter_ToUrlWithExistingQueryString ()
    {
      string url = "http://localhost/Default.html?Parameter2=Value2";
      string parameter = "Parameter1";
      string value = "Value1ä#";

      string expected = string.Format (
          "{0}&{1}={2}",
          url,
          parameter,
          HttpUtility.UrlEncode (value, _currentEncoding));

      string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void AddParameter_ToUrlWithQuestionmark ()
    {
      string url = "http://localhost/Default.html?";
      string parameter = "Parameter1";
      string value = "Value1ä#";

      string expected = string.Format (
          "{0}{1}={2}",
          url,
          parameter,
          HttpUtility.UrlEncode (value, _currentEncoding));

      string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void AddParameter_ToUrlWithAmpersand ()
    {
      string url = "http://localhost/Default.html?Parameter2=Value2&";
      string parameter = "Parameter1";
      string value = "Value1ä#";

      string expected = string.Format (
          "{0}{1}={2}",
          url,
          parameter,
          HttpUtility.UrlEncode (value, _currentEncoding));

      string actual = UrlUtility.AddParameter (url, parameter, value, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }


    [Test]
    public void AddParametersToUrl ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string parameter3 = null;
      string parameter4 = "Parameter4";
      string parameter5 = "Parameter5";
      string value1 = "Value1ä#";
      string value2a = "Value2a";
      string value2b = "Value2b";
      string value3 = "Value3";
      string value4 = string.Empty;
      string value5 = null;

      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (parameter1, value1);
      queryString.Add (parameter2, value2a);
      queryString.Add (parameter2, value2b);
      queryString.Add (parameter3, value3);
      queryString.Add (parameter4, value4);
      queryString.Add (parameter5, value5);

      string expected = string.Format (
          "{0}?{1}={2}&{3}={4}&{5}={6}&{8}&{9}=&{11}=",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2a, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2b, _currentEncoding),
          parameter3,
          HttpUtility.UrlEncode (value3, _currentEncoding),
          parameter4,
          HttpUtility.UrlEncode (value4, _currentEncoding),
          parameter5,
          value5);

      string actual = UrlUtility.AddParameters (url, queryString, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void AddParametersToUrlNoParameters ()
    {
      string url = "http://localhost/Default.html";

      NameValueCollection queryString = new NameValueCollection();
      string expected = url;

      string actual = UrlUtility.AddParameters (url, queryString, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }


    [Test]
    public void FormatQueryString ()
    {
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (parameter1, value1);
      queryString.Add (parameter2, value2);

      string expected = string.Format (
          "?{0}={1}&{2}={3}",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.FormatQueryString (queryString, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void FormatQueryStringNoParameters ()
    {
      NameValueCollection queryString = new NameValueCollection();
      string expected = string.Empty;

      string actual = UrlUtility.FormatQueryString (queryString, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }


    [Test]
    public void DeleteParameterFromUrlWithLastParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{1}={2}&{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string expected = string.Format (
          "{0}?{1}={2}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter2, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithFirstParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{1}={2}&{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string expected = string.Format (
          "{0}?{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithParameterNamePartOfUrl ()
    {
      string url = "http://localhost/Parameter1.html";
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{1}={2}&{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string expected = string.Format (
          "{0}?{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithParameterNamePartOfOtherParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#Parameter2";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{1}={2}&{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string expected = string.Format (
          "{0}?{1}={2}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter2, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithSingleParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string value1 = "Value1ä#";

      string original = string.Format (
          "{0}?{1}={2}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding));

      string expected = url;

      string actual = UrlUtility.DeleteParameter (original, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithNoUrl ()
    {
      string parameter1 = "Parameter1";
      string value1 = "Value1ä#";

      string original = string.Format (
          "?{0}={1}",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding));

      string expected = string.Empty;

      string actual = UrlUtility.DeleteParameter (original, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithUnknownParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string value1 = "Value1ä#";
      string parameter2 = "Parameter2";

      string original = string.Format (
          "{0}?{1}={2}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding));

      string expected = original;

      string actual = UrlUtility.DeleteParameter (original, parameter2, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithEmptyLastParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{1}={2}&{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          string.Empty);

      string expected = string.Format (
          "{0}?{1}={2}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter2, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithEmptyFirstParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{1}={2}&{3}={4}",
          url,
          parameter1,
          string.Empty,
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string expected = string.Format (
          "{0}?{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithEmptyLastParameterName ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string parameter2 = null;
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{1}={2}&{4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string expected = string.Format (
          "{0}?{1}={2}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter2, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithEmptyFirstParameterName ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = null;
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string original = string.Format (
          "{0}?{2}&{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string expected = string.Format (
          "{0}?{3}={4}",
          url,
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.DeleteParameter (original, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithNoParameter ()
    {
      string url = "http://localhost/Default.html";
      string parameter1 = "Parameter1";
      string expected = url;

      string actual = UrlUtility.DeleteParameter (url, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrlWithQuestionMark ()
    {
      string url = "http://localhost/Default.html?";
      string parameter1 = "Parameter1";
      string expected = url;

      string actual = UrlUtility.DeleteParameter (url, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void DeleteParameterFromUrl_WithParameterNameIsEmpty_ThrowsArgumentException ()
    {
      string url = "http://localhost/Default.html";

      Assert.That (
          () => UrlUtility.DeleteParameter (url, "", _currentEncoding),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo ("Parameter 'name' cannot be empty.", "name"));
    }

    [Test]
    public void GetParameterFromLastValue ()
    {
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string url = string.Format (
          "http://localhost/Default.html?{0}={1}&{2}={3}",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.GetParameter (url, parameter2, _currentEncoding);
      Assert.That (actual, Is.EqualTo (value2));
    }

    [Test]
    public void GetParameterFromFirstValue ()
    {
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = "Value2";

      string url = string.Format (
          "http://localhost/Default.html?{0}={1}&{2}={3}",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2,
          HttpUtility.UrlEncode (value2, _currentEncoding));

      string actual = UrlUtility.GetParameter (url, parameter1, _currentEncoding);
      Assert.That (actual, Is.EqualTo (value1));
    }

    [Test]
    public void GetParameterFromMissingValue ()
    {
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";

      string url = string.Format (
          "http://localhost/Default.html?{0}={1}",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding));

      string actual = UrlUtility.GetParameter (url, parameter2, _currentEncoding);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void GetParameterFromEmptyValue ()
    {
      string parameter1 = "Parameter1";
      string parameter2 = "Parameter2";
      string value1 = "Value1ä#";
      string value2 = string.Empty;

      string url = string.Format (
          "http://localhost/Default.html?{0}={1}&{2}=",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          parameter2);

      string actual = UrlUtility.GetParameter (url, parameter2, _currentEncoding);
      Assert.That (actual, Is.EqualTo (value2));
    }

    [Test]
    public void GetParameter_WithMissingParameter_ReturnsNull ()
    {
      string parameter1 = "Parameter1";
      string value1 = "Value1ä#";

      string url = string.Format (
          "http://localhost/Default.html?{0}={1}",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding));

      string actual = UrlUtility.GetParameter (url, "SomeParameter", _currentEncoding);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void GetParameter_WithNullParameterName_ReturnsParameterValue ()
    {
      string parameter1 = "Parameter1";
      string value1 = "Value1ä#";
      string value2 = "Value2#ä";

      string url = string.Format (
          "http://localhost/Default.html?{0}={1}&{2}",
          parameter1,
          HttpUtility.UrlEncode (value1, _currentEncoding),
          value2);

      string actual = UrlUtility.GetParameter (url, null, _currentEncoding);
      Assert.That (actual, Is.EqualTo (value2));
    }

    [Test]
    public void GetParameterToUrl_WithParameterNameIsEmpty_ThrowsArgumentException ()
    {
      string url = "http://localhost/Default.html";

      Assert.That (
          () => UrlUtility.GetParameter (url, "", _currentEncoding),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo ("Parameter 'name' cannot be empty.", "name"));
    }
  }

}
