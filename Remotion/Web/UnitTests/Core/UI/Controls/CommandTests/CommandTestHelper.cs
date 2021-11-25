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
using System.Text;
using System.Web;
using Moq;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.UI.Controls.CommandTests
{
  public class CommandTestHelper
  {
    // types

    // static members

    // member fields

    private readonly Mock<IWebSecurityAdapter> _mockWebSecurityAdapter;
    private readonly Mock<IWxeSecurityAdapter> _mockWxeSecurityAdapter;
    private readonly Mock<ISecurableObject> _mockSecurableObject;

    private readonly HttpContext _httpContext;
    private readonly HtmlTextWriterSingleTagMock _htmlWriter;

    private readonly Type _functionType;
    private readonly string _functionTypeName;
    private string _wxeFunctionParameters = "\"Value1\"";
    private string _toolTip = "This is a Tool Tip.";
    private string _href = "/test.html?Param1={0}&Param2={1}";
    private string _target = "_blank";
    private string _postBackEvent = "__doPostBack (\"Target\", \"Args\");";
    private string _onClick = "DoStuff();";
    private string _ownerControlClientID = "OwnerControlClientID";
    private string _itemID = "CommandID";

    // construction and disposing

    public CommandTestHelper ()
    {
      _httpContext = HttpContextHelper.CreateHttpContext("GET", "default.html", null);
      _httpContext.Response.ContentEncoding = Encoding.UTF8;

      _functionType = typeof(TestFunction);
      _functionTypeName = TypeUtility.GetPartialAssemblyQualifiedName(_functionType);

      _mockWebSecurityAdapter = new Mock<IWebSecurityAdapter>(MockBehavior.Strict);
      _mockWxeSecurityAdapter = new Mock<IWxeSecurityAdapter>(MockBehavior.Strict);
      _mockSecurableObject = new Mock<ISecurableObject>(MockBehavior.Strict);

      _htmlWriter = new HtmlTextWriterSingleTagMock();
    }

    // methods and properties

    public HttpContext HttpContext
    {
      get { return _httpContext; }
    }

    public HtmlTextWriterSingleTagMock HtmlWriter
    {
      get { return _htmlWriter; }
    }

    public IWebSecurityAdapter WebSecurityAdapter
    {
      get { return _mockWebSecurityAdapter.Object; }
    }

    public IWxeSecurityAdapter WxeSecurityAdapter
    {
      get { return _mockWxeSecurityAdapter.Object; }
    }

    public ISecurableObject SecurableObject
    {
      get { return _mockSecurableObject.Object; }
    }

    public void VerifyAll ()
    {
      _mockWebSecurityAdapter.Verify();
      _mockWxeSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
    }

    public void ExpectWebSecurityProviderHasAccess (ISecurableObject securableObject, Delegate handler, bool returnValue)
    {
      _mockWebSecurityAdapter.Setup(_ => _.HasAccess(securableObject, handler)).Returns(returnValue).Verifiable();
    }

    public void ExpectWxeSecurityProviderHasStatelessAccess (Type functionType, bool returnValue)
    {
      _mockWxeSecurityAdapter.Setup(_ => _.HasStatelessAccess(functionType)).Returns(returnValue).Verifiable();
    }


    public string ToolTip
    {
      get { return _toolTip; }
    }

    public string Href
    {
      get { return _href; }
    }

    public string WxeFunctionParameters
    {
      get { return _wxeFunctionParameters; }
    }

    public string Target
    {
      get { return _target; }
    }

    public string PostBackEvent
    {
      get { return _postBackEvent; }
    }

    public string OnClick
    {
      get { return _onClick; }
    }

    public string OwnerControlClientID
    {
      get { return _ownerControlClientID; }
    }

    public string ItemID
    {
      get { return _itemID; }
    }

    public Command CreateHrefCommand (IWebSecurityAdapter webSecurityAdapter = null, IWxeSecurityAdapter wxeSecurityAdapter = null)
    {
      Command command = new Command(CommandType.Href, webSecurityAdapter, wxeSecurityAdapter);
      InitializeHrefCommand(command);

      return command;
    }

    public Mock<Command> CreateHrefCommandAsPartialMock ()
    {
      var command = new Mock<Command>(CommandType.Href, (IWebSecurityAdapter)null, (IWxeSecurityAdapter)null) { CallBase = true };

      InitializeHrefCommand(command.Object);

      return command;
    }

    private void InitializeHrefCommand (Command command)
    {
      command.ItemID = _itemID;
      command.Type = CommandType.Href;
      command.ToolTip = _toolTip;
      command.HrefCommand.Href = _href;
      command.HrefCommand.Target = _target;
    }

    public Command CreateEventCommand (IWebSecurityAdapter webSecurityAdapter = null, IWxeSecurityAdapter wxeSecurityAdapter = null)
    {
      Command command = new Command(CommandType.Event, webSecurityAdapter, wxeSecurityAdapter);
      command.OwnerControl = CreateOwnerControl();
      InitializeEventCommand(command);

      return command;
    }

    public Mock<Command> CreateEventCommandAsPartialMock ()
    {
      var command = new Mock<Command>(CommandType.Event, (IWebSecurityAdapter)null, (IWxeSecurityAdapter)null) { CallBase = true };
      InitializeEventCommand(command.Object);

      return command;
    }

    private void InitializeEventCommand (Command command)
    {
      command.ItemID = _itemID;
      command.Type = CommandType.Event;
      command.ToolTip = _toolTip;
    }

    public Command CreateWxeFunctionCommand (IWebSecurityAdapter webSecurityAdapter = null, IWxeSecurityAdapter wxeSecurityAdapter = null)
    {
      Command command = new Command(CommandType.WxeFunction, webSecurityAdapter, wxeSecurityAdapter);
      command.OwnerControl = CreateOwnerControl();
      InitializeWxeFunctionCommand(command);

      return command;
    }

    public Mock<Command> CreateWxeFunctionCommandAsPartialMock ()
    {
      var command = new Mock<Command>(CommandType.WxeFunction, (IWebSecurityAdapter)null, (IWxeSecurityAdapter)null) { CallBase = true };

      InitializeWxeFunctionCommand(command.Object);

      return command;
    }

    private void InitializeWxeFunctionCommand (Command command)
    {
      command.ItemID = _itemID;
      command.Type = CommandType.WxeFunction;
      command.ToolTip = _toolTip;
      command.WxeFunctionCommand.TypeName = _functionTypeName;
      command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
      command.WxeFunctionCommand.Target = _target;
    }

    public Command CreateNoneCommand (IWebSecurityAdapter webSecurityAdapter = null, IWxeSecurityAdapter wxeSecurityAdapter = null)
    {
      Command command = new Command(CommandType.None, webSecurityAdapter, wxeSecurityAdapter);
      command.OwnerControl = CreateOwnerControl();
      InitializeNoneCommand(command);

      return command;
    }

    public Mock<Command> CreateNoneCommandAsPartialMock ()
    {
      var command = new Mock<Command>(CommandType.None, (IWebSecurityAdapter)null, (IWxeSecurityAdapter)null) { CallBase = true };
      InitializeNoneCommand(command.Object);

      return command;
    }

    private void InitializeNoneCommand (Command command)
    {
      command.ItemID = _itemID;
      command.Type = CommandType.None;
    }

    public void ExpectOnceOnHasAccess (Mock<Command> command, bool returnValue)
    {
      command.Setup(_ => _.HasAccess(_mockSecurableObject.Object)).Returns(returnValue).Verifiable();
    }

    private IControl CreateOwnerControl ()
    {
      var controlStub = new Mock<IControl>();
      controlStub.Setup(stub => stub.ClientID).Returns(_ownerControlClientID);
      return controlStub.Object;
    }
  }
}
