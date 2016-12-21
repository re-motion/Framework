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
using System.ComponentModel;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class BocCommand : Command
  {
    /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class BocHrefCommandInfo : HrefCommandInfo
    {
      /// <summary> Initalizes a new instance </summary>
      public BocHrefCommandInfo ()
      {
      }

      /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
      /// <value> 
      ///   The URL to link to when the rendered command is clicked. The default value is an empty <see cref="String"/>. 
      /// </value>
      [Description ("The hyperlink reference of the command. Use {0} to insert the Business Object's ID.")]
      public override string Href
      {
        get { return base.Href; }
        set { base.Href = value; }
      }
    }

    /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class BocWxeFunctionCommandInfo : WxeFunctionCommandInfo
    {
      /// <summary> Initalizes a new instance </summary>
      public BocWxeFunctionCommandInfo ()
      {
      }

      /// <summary> 
      ///   Gets or sets the comma separated list of parameters passed to the WxeFunction when the rendered 
      ///   command is clicked.
      /// </summary>
      /// <remarks>
      ///   The following reference parameters can be added to the list of parameters.
      ///   <list type="table">
      ///     <listheader>
      ///       <term> Name </term>
      ///       <description> Contents </description>
      ///     </listheader>
      ///     <item>
      ///       <term> id </term>
      ///       <description> The ID, if the object is of type <see cref="IBusinessObjectWithIdentity"/>. </description>
      ///     </item>
      ///     <item>
      ///       <term> object </term>
      ///       <description> The <see cref="IBusinessObject"/> itself. </description>
      ///     </item>
      ///     <item>
      ///       <term> parent </term>
      ///       <description> The containing <see cref="IBusinessObject"/>. </description>
      ///     </item>
      ///     <item>
      ///       <term> parentproperty </term>
      ///       <description> The <see cref="IBusinessObjectReferenceProperty"/> used to acess the object. </description>
      ///     </item>
      ///   </list>
      /// </remarks>
      /// <value> 
      ///   The comma separated list of parameters passed to the WxeFunction when the rendered 
      ///   command is clicked. The default value is an empty <see cref="String"/>. 
      /// </value>
      [Description (
          "A comma separated list of parameters for the command. The following reference parameters are available: id, object, parent, parentproperty."
          )]
      public override string Parameters
      {
        get { return base.Parameters; }
        set { base.Parameters = value; }
      }
    }

    [Browsable (false)]
    public new BocCommandClickEventHandler Click;

    private bool _hasClickFired;
    private BocHrefCommandInfo _hrefCommand;
    private BocWxeFunctionCommandInfo _wxeFunctionCommand;

    public BocCommand ()
        : this (CommandType.None, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
    {
    }

    public BocCommand (CommandType defaultType)
        : this (defaultType, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
    {
    }

    public BocCommand (
        CommandType defaultType,
        [CanBeNull] IWebSecurityAdapter webSecurityAdapter,
        [CanBeNull] IWxeSecurityAdapter wxeSecurityAdapter)
        : base (defaultType, webSecurityAdapter, wxeSecurityAdapter)
    {
      _hrefCommand = new BocHrefCommandInfo();
      _wxeFunctionCommand = new BocWxeFunctionCommandInfo();
    }

    /// <summary> Fires the <see cref="Click"/> event. </summary>
    public virtual void OnClick (IBusinessObject businessObject)
    {
      base.OnClick();
      if (_hasClickFired)
        return;
      _hasClickFired = true;
      if (Click != null)
      {
        BocCommandClickEventArgs e = new BocCommandClickEventArgs (this, businessObject);
        Click (OwnerControl, e);
      }
    }

    /// <summary> Renders the opening tag for the command. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
    /// <param name="renderingFeatures"> The rendering features to use. </param>
    /// <param name="postBackLink">
    ///   The string rendered in the <c>href</c> tag of the anchor element when the command type is <see cref="CommandType.Event"/> or 
    ///   <see cref="CommandType.WxeFunction"/>. This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back.
    /// </param>
    /// <param name="onClick"> 
    ///   The string rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="businessObjectID">
    ///   An identifier for the <see cref="IBusinessObject"/> to which the rendered command is applied.
    /// </param>
    /// <param name="securableObject">
    ///   The <see cref="ISecurableObject"/> for which security is evaluated. Use <see landword="null"/> if security is stateless or not evaluated.
    /// </param>
    public void RenderBegin (
        HtmlTextWriter writer,
        IRenderingFeatures renderingFeatures,
        string postBackLink,
        string onClick,
        string businessObjectID,
        ISecurableObject securableObject)
    {
      RenderBegin (writer, renderingFeatures, postBackLink, new[] { businessObjectID }, onClick, securableObject);
    }

    /// <summary>
    ///   Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommand"/>.
    /// </summary>
    /// <param name="wxePage"> The <see cref="IWxePage"/> where this command is rendered on. </param>
    /// <param name="businessObject">
    ///   The <see cref="IBusinessObject"/> in the row where the command was clicked.
    /// </param>
    public void ExecuteWxeFunction (IWxePage wxePage, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("wxePage", wxePage);
      if (! wxePage.IsReturningPostBack)
      {
        NameObjectCollection parameters = PrepareWxeFunctionParameters (businessObject);
        ExecuteWxeFunction (wxePage, parameters);
      }
    }

    private NameObjectCollection PrepareWxeFunctionParameters (IBusinessObject businessObject)
    {
      NameObjectCollection parameters = new NameObjectCollection();

      parameters["object"] = businessObject;
      if (businessObject is IBusinessObjectWithIdentity)
        parameters["id"] = ((IBusinessObjectWithIdentity) businessObject).UniqueIdentifier;
      if (OwnerControl != null)
      {
        if (OwnerControl.DataSource != null && OwnerControl.Value != null)
          parameters["parent"] = OwnerControl.DataSource.BusinessObject;
        if (OwnerControl.Property != null)
          parameters["parentproperty"] = OwnerControl.Property;
      }

      return parameters;
    }

    /// <summary> The <see cref="BocHrefCommandInfo"/> used when rendering the command as a hyperlink. </summary>
    /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Href"/>. </remarks>
    /// <value> A <see cref="BocHrefCommandInfo"/> object. </value>
    public override HrefCommandInfo HrefCommand
    {
      get { return _hrefCommand; }
      set { _hrefCommand = (BocHrefCommandInfo) value; }
    }

    /// <summary>
    ///   The <see cref="BocWxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
    /// </summary>
    /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>. </remarks>
    /// <value> A <see cref="BocWxeFunctionCommandInfo"/> object. </value>
    public override WxeFunctionCommandInfo WxeFunctionCommand
    {
      get { return _wxeFunctionCommand; }
      set { _wxeFunctionCommand = (BocWxeFunctionCommandInfo) value; }
    }

    /// <summary> Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this object belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public new IBusinessObjectBoundWebControl OwnerControl
    {
      get { return (IBusinessObjectBoundWebControl) base.OwnerControlImplementation; }
      set { base.OwnerControlImplementation = value; }
    }

    protected override IControl OwnerControlImplementation
    {
      get { return OwnerControl; }
      set { OwnerControl = (IBusinessObjectBoundWebControl) value; }
    }
  }

  /// <summary>
  ///   Represents the method that handles the <see cref="BocCommand.Click"/> event
  ///   raised when clicking on a <see cref="Command"/> of type <see cref="CommandType.Event"/>.
  /// </summary>
  public delegate void BocCommandClickEventHandler (object sender, BocCommandClickEventArgs e);

  /// <summary> Provides data for the <see cref="BocCommand.Click"/> event. </summary>
  public class BocCommandClickEventArgs : CommandClickEventArgs
  {
    private readonly IBusinessObject _businessObject;

    public BocCommandClickEventArgs (BocCommand command, IBusinessObject businessObject)
        : base (command)
    {
      _businessObject = businessObject;
    }

    /// <summary> The <see cref="BocCommand"/> that caused the event. </summary>
    public new BocCommand Command
    {
      get { return (BocCommand) base.Command; }
    }

    /// <summary>
    ///   The <see cref="IBusinessObject"/> on which the rendered command is applied on.
    /// </summary>
    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }
  }
}
