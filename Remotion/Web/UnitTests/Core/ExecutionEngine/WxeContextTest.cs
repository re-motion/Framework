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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{

[TestFixture]
public class WxeContextTest
{
  private HttpContext _currentHttpContext;
  private Type _functionType;
  private string _functionTypeName;
  private string _resource;
  private NameValueCollection _queryString;

  [SetUp]
  public virtual void SetUp ()
  {
    _queryString = new NameValueCollection();
    _queryString.Add(WxeHandler.Parameters.ReturnUrl, "/Root.wxe");

    _currentHttpContext = WxeContextFactory.CreateHttpContext(_queryString);

    _functionType = typeof(TestFunction);
    _functionTypeName = TypeUtility.GetPartialAssemblyQualifiedName(_functionType);
    _resource = "~/Test.wxe";

    UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
    UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(_functionType, _resource));
  }

  [TearDown]
  public virtual void TearDown ()
  {
    WebConfigurationMock.Current = null;
    UrlMappingConfiguration.SetCurrent(null);
    WxeContext.SetCurrent(null!);
  }

  [Test]
  public void GetStaticPermanentUrlWithDefaultWxeHandlerWithoutMappingForFunctionType ()
  {
    UrlMappingConfiguration.SetCurrent(null);

    using var _ = CreateWxeUrlSettingsScopeWithDefaultWxeHandler();

    string wxeHandler = SafeServiceLocator.Current.GetInstance<WxeUrlSettings>().DefaultWxeHandler;
    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), wxeHandler);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add(WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    string permanentUrl = WxeContext.GetPermanentUrl(new HttpContextWrapper(_currentHttpContext), _functionType, new NameValueCollection());
    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetStaticPermanentUrlWithDefaultWxeHandlerForMappedFunctionType ()
  {
    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    string permanentUrl = WxeContext.GetPermanentUrl(new HttpContextWrapper(_currentHttpContext), _functionType, new NameValueCollection());
    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetStaticPermanentUrlWithEmptyQueryString ()
  {
    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    string permanentUrl = WxeContext.GetPermanentUrl(new HttpContextWrapper(_currentHttpContext), _functionType, new NameValueCollection());
    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetStaticPermanentUrlWithQueryString ()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add(parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add(queryString);
    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    string permanentUrl = WxeContext.GetPermanentUrl(new HttpContextWrapper(_currentHttpContext), _functionType, queryString);

    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetStaticPermanentUrlWithQueryStringExceedingMaxLength ()
  {
    using var _ = CreateWxeUrlSettingsScopeWithDefaultWxeHandler(maxLength: 100);

    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add(parameterName, parameterValue);
    Assert.That(
        () => WxeContext.GetPermanentUrl(new HttpContextWrapper(_currentHttpContext), _functionType, queryString),
        Throws.InstanceOf<WxePermanentUrlTooLongException>());
  }

  [Test]
  public void GetStaticPermanentUrlWithoutWxeHandler ()
  {
    using var _ = CreateWxeUrlSettingsScopeWithDefaultWxeHandler(maxLength: 100);

    WebConfigurationMock.Current = null;
    UrlMappingConfiguration.SetCurrent(null);
    Assert.That(
        () => WxeContext.GetPermanentUrl(new HttpContextWrapper(_currentHttpContext), _functionType, new NameValueCollection()),
        Throws.InstanceOf<WxeException>());
  }

  [Test]
  public void GetPermanentUrlWithEmptyQueryString ()
  {
    var wxeContext = CreateWxeContext();

    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    string permanentUrl = wxeContext.GetPermanentUrl(_functionType, new NameValueCollection(), false);
    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetPermanentUrlWithQueryString ()
  {
    var wxeContext = CreateWxeContext();

    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add(parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add(queryString);
    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    string permanentUrl = wxeContext.GetPermanentUrl(_functionType, queryString, false);

    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetPermanentUrlWithQueryStringExceedingMaxLength ()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();

    var wxeContext = CreateWxeContext(100);

    queryString.Add(parameterName, parameterValue);
    Assert.That(
        () => wxeContext.GetPermanentUrl(_functionType, queryString, false),
        Throws.InstanceOf<WxePermanentUrlTooLongException>());
  }

  [Test]
  public void GetPermanentUrlWithQueryStringAndParentPermanentUrl ()
  {
    var wxeContext = CreateWxeContext();

    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add(parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add(queryString);

    string parentUrl = _currentHttpContext.Request.Url.AbsolutePath;
    parentUrl += UrlUtility.FormatQueryString(_currentHttpContext.Request.QueryString);
    expectedQueryString.Add(WxeHandler.Parameters.ReturnUrl, parentUrl);

    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    string permanentUrl = wxeContext.GetPermanentUrl(_functionType, queryString, true);

    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetPermanentUrlWithParentPermanentUrlAndRemoveBothReturnUrls ()
  {
    var wxeContext = CreateWxeContext();

    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add(parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add(queryString);

    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    string permanentUrl = wxeContext.GetPermanentUrl(_functionType, queryString, false);

    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetPermanentUrlWithParentPermanentUrlAndRemoveInnermostReturnUrl ()
  {
    var wxeContext = CreateWxeContext(100, "DefaultWxeHandler.ashx");

    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add(parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add(queryString);

    string parentUrl = _currentHttpContext.Request.Url.AbsolutePath;
    parentUrl += UrlUtility.FormatQueryString(_currentHttpContext.Request.QueryString);
    parentUrl = UrlUtility.DeleteParameter(parentUrl, WxeHandler.Parameters.ReturnUrl, Encoding.UTF8);
    expectedQueryString.Add(WxeHandler.Parameters.ReturnUrl, parentUrl);

    string expectedUrl = UrlUtility.ResolveUrlCaseSensitive(new HttpContextWrapper(_currentHttpContext), _resource);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    string permanentUrl = wxeContext.GetPermanentUrl(_functionType, queryString, true);

    Assert.That(permanentUrl, Is.Not.Null);
    Assert.That(permanentUrl, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetPermanentUrlWithExistingReturnUrl ()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    var wxeContext = CreateWxeContext();

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add(parameterName, parameterValue);
    queryString.Add(WxeHandler.Parameters.ReturnUrl, "");
    Assert.That(
        () => wxeContext.GetPermanentUrl(_functionType, queryString, true),
        Throws.ArgumentException);
  }

  private ServiceLocatorScope CreateWxeUrlSettingsScopeWithDefaultWxeHandler (int maxLength = 1024)
  {
    var wxeUrlSettings = WxeUrlSettings.Create(null, maxLength, "WxeHandler.ashx");

    var serviceLocator = DefaultServiceLocator.Create();
    serviceLocator.RegisterSingle(() => wxeUrlSettings);
    return new ServiceLocatorScope(serviceLocator);
  }

  private WxeContext CreateWxeContext (int? maximumUrlLength = null, string defaultWxeHandler = null)
  {
    return new WxeContext(
        new HttpContextWrapper(_currentHttpContext),
        new WxeFunctionStateManager(new HttpSessionStateWrapper(_currentHttpContext.Session)),
        new WxeFunctionState(new TestFunction(), 20, false),
        _queryString,
        WxeUrlSettings.Create(maximumUrlLength: maximumUrlLength, defaultWxeHandler: defaultWxeHandler),
        new WxeLifetimeManagementSettings());
  }
}
}
