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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Remotion.Web.UnitTests.Core.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{

[TestFixture]
public class NavigationCommandTest
{
  private HttpContext _currentHttpContext;
  private Type _functionType;
  private string _functionTypeName;
  private NavigationCommand _hrefCommand;
  private NavigationCommand _eventCommand;
  private NavigationCommand _wxeFunctionCommand;
  private NavigationCommand _noneCommand;
  private string _toolTip;
  private string _href;
  private string _wxeFunctionParameter1Value;
  private string _wxeFunctionParameters;
  private string _target;
  private string _postBackEvent;
  private string _onClick;
  private HtmlTextWriterSingleTagMock _writer;

  [SetUp]
  public virtual void SetUp ()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext("GET", "default.html", null);
    _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
    HttpContextHelper.SetCurrent(_currentHttpContext);

    _functionType = typeof(TestFunction);
    _functionTypeName = TypeUtility.GetPartialAssemblyQualifiedName(_functionType);
    _wxeFunctionParameter1Value = "Value1";
    _wxeFunctionParameters = "\"Value1\"";

    _toolTip = "This is a Tool Tip.";
    _href = "test.html?Param1={0}&Param2={1}";
    _target = "_blank";
    _postBackEvent = "__doPostBack (\"Target\", \"Args\");";
    _onClick = "return false;";

    _hrefCommand = new NavigationCommand();
    _hrefCommand.Type = CommandType.Href;
    _hrefCommand.ToolTip = _toolTip;
    _hrefCommand.HrefCommand.Href = _href;
    _hrefCommand.HrefCommand.Target = _target;

    _eventCommand = new NavigationCommand();
    _eventCommand.Type = CommandType.Event;
    _eventCommand.ToolTip = _toolTip;

    _wxeFunctionCommand = new NavigationCommand();
    _wxeFunctionCommand.Type = CommandType.WxeFunction;
    _wxeFunctionCommand.ToolTip = _toolTip;
    _wxeFunctionCommand.WxeFunctionCommand.TypeName = _functionTypeName;
    _wxeFunctionCommand.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
    _wxeFunctionCommand.WxeFunctionCommand.Target = _target;

    _noneCommand = new NavigationCommand();
    _noneCommand.Type = CommandType.None;

    _writer = new HtmlTextWriterSingleTagMock();
  }

  [TearDown]
  public virtual void TearDown ()
  {
    WebConfigurationMock.Current = null;
    UrlMappingConfiguration.SetCurrent(null);
    HttpContextHelper.SetCurrent(null);
  }

  [Test]
  public void RenderWxeFunctionCommand ()
  {
    using var _ = CreateWxeUrlSettingsScope();

    NameValueCollection additionalUrlParameters = new NameValueCollection();
    additionalUrlParameters.Add("Parameter2", "Value2");

    string expectedHref = _wxeFunctionCommand.GetWxeFunctionPermanentUrl(additionalUrlParameters);
    string expectedOnClick = _onClick;

    _wxeFunctionCommand.RenderBegin(
        _writer,
        RenderingFeatures.Default,
        _postBackEvent,
        new string[0],
        _onClick,
        null,
        additionalUrlParameters,
        false,
        new Style(),
        new NameValueCollection());

    Assert.IsNotNull(_writer.Tag, "Missing Tag");
    Assert.That(_writer.Tag, Is.EqualTo(HtmlTextWriterTag.A), "Wrong Tag");

    Assert.IsNotNull(_writer.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
    Assert.That(_writer.Attributes[HtmlTextWriterAttribute.Href], Is.EqualTo(expectedHref), "Wrong Href");

    Assert.IsNotNull(_writer.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
    Assert.That(_writer.Attributes[HtmlTextWriterAttribute.Onclick], Is.EqualTo(expectedOnClick), "Wrong OnClick");

    Assert.IsNotNull(_writer.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
    Assert.That(_writer.Attributes[HtmlTextWriterAttribute.Title], Is.EqualTo(_toolTip), "Wrong Title");

    Assert.IsNotNull(_writer.Attributes[HtmlTextWriterAttribute.Target], "Missing Target");
    Assert.That(_writer.Attributes[HtmlTextWriterAttribute.Target], Is.EqualTo(_target), "Wrong Target");
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithDefaultWxeHandler ()
  {
    using var _ = CreateWxeUrlSettingsScope();

    string wxeHandler = SafeServiceLocator.Current.GetInstance<WxeUrlSettings>().DefaultWxeHandler;

    string expectedUrl = wxeHandler.TrimStart('~');
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add("Parameter1", _wxeFunctionParameter1Value);
    expectedQueryString.Add(WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
    expectedQueryString.Add(WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    NavigationCommand command = new NavigationCommand();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
    string url = command.GetWxeFunctionPermanentUrl();

    Assert.That(url, Is.Not.Null);
    Assert.That(url, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeByTypeName ()
  {
    string resource = "~/Test.wxe";

    UrlMappingConfiguration.SetCurrent(UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
    UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(_functionType, resource));
    string parameter1 = "Value1";

    string expectedUrl = resource.TrimStart('~');
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add("Parameter1", parameter1);
    expectedQueryString.Add(WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    NavigationCommand command = new NavigationCommand();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    string url = command.GetWxeFunctionPermanentUrl();

    Assert.That(url, Is.Not.Null);
    Assert.That(url, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeByMappingID ()
  {
    string mappingID = "Test";
    string resource = "~/Test.wxe";

    UrlMappingConfiguration.SetCurrent(UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
    UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(mappingID, _functionType, resource));
    string parameter1 = "Value1";

    string expectedUrl = resource.TrimStart('~');
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add("Parameter1", parameter1);
    expectedQueryString.Add(WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    NavigationCommand command = new NavigationCommand();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.MappingID = mappingID;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    string url = command.GetWxeFunctionPermanentUrl();

    Assert.That(url, Is.Not.Null);
    Assert.That(url, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeAndInvalidTypeNameMappingIDCombination ()
  {
    string mappingID = "Test";
    string resource = "~/Test.wxe";
    Type functionWithNestingType = typeof(TestFunctionWithNesting);

    UrlMappingConfiguration.SetCurrent(UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
    UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(mappingID, functionWithNestingType, resource));
    string parameter1 = "Value1";

    NavigationCommand command = new NavigationCommand();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.MappingID = mappingID;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    Assert.That(
        () => command.GetWxeFunctionPermanentUrl(),
        Throws.InvalidOperationException);
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithDefaultWxeHandlerAndAdditionalUrlParameters ()
  {
    using var _ = CreateWxeUrlSettingsScope();

    string wxeHandler = SafeServiceLocator.Current.GetInstance<WxeUrlSettings>().DefaultWxeHandler;

    NameValueCollection additionalUrlParameters = new NameValueCollection();
    additionalUrlParameters.Add("Parameter2", "Value2");

    string expectedUrl = wxeHandler.TrimStart('~');
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add("Parameter1", _wxeFunctionParameter1Value);
    expectedQueryString.Add(WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
    expectedQueryString.Add(additionalUrlParameters);
    expectedQueryString.Add(WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    NavigationCommand command = new NavigationCommand();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
    string url = command.GetWxeFunctionPermanentUrl(additionalUrlParameters);

    Assert.That(url, Is.Not.Null);
    Assert.That(url, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeAndAdditionalUrlParameters ()
  {
    string resource = "~/Test.wxe";

    UrlMappingConfiguration.SetCurrent(UrlMappingConfigurationUtility.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
    UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(_functionType, resource));
    string parameter1 = "Value1";

    NameValueCollection additionalUrlParameters = new NameValueCollection();
    additionalUrlParameters.Add("Parameter2", "Value2");

    string expectedUrl = resource.TrimStart('~');
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add("Parameter1", parameter1);
    expectedQueryString.Add(WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
    expectedQueryString.Add(additionalUrlParameters);
    expectedUrl += UrlUtility.FormatQueryString(expectedQueryString);

    NavigationCommand command = new NavigationCommand();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    string url = command.GetWxeFunctionPermanentUrl(additionalUrlParameters);

    Assert.That(url, Is.Not.Null);
    Assert.That(url, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithoutDefaultWxeHandler ()
  {
    using var _ = CreateWxeUrlSettingsScope(defaultWxeHandler: null);
    string parameter1 = "Hello World!";

    NavigationCommand command = new NavigationCommand();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    Assert.That(
        () => command.GetWxeFunctionPermanentUrl(),
        Throws.InstanceOf<WxeException>());
  }

  private ServiceLocatorScope CreateWxeUrlSettingsScope (string defaultWxeHandler = "WxeHandler.ashx")
  {
    var wxeUrlSettings = WxeUrlSettings.Create(1024, defaultWxeHandler);

    var serviceLocator = DefaultServiceLocator.Create();
    serviceLocator.RegisterSingle(() => wxeUrlSettings);
    return new ServiceLocatorScope(serviceLocator);
  }
}

}
