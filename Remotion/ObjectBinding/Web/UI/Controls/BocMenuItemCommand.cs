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
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

public class BocMenuItemCommand: BocCommand
{
  /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class MenuItemHrefCommandInfo: BocCommand.BocHrefCommandInfo
  {
    /// <summary> Initalizes a new instance </summary>
    public MenuItemHrefCommandInfo()
    {
    }

    /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
    /// <value> 
    ///   The URL to link to when the rendered command is clicked. The default value is an empty <see cref="String"/>. 
    /// </value>
    [Description ("The hyperlink reference of the command.")]
    public override string Href 
    {
      get { return base.Href; }
      set { base.Href = value; }
    }
  }

  /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class MenuItemWxeFunctionCommandInfo: BocCommand.BocWxeFunctionCommandInfo
  {
    /// <summary> Create a new instance. </summary>
    public MenuItemWxeFunctionCommandInfo()
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
    ///       <term> indices </term>
    ///       <description> The indices of the selected <see cref="IBusinessObject"/> instances. </description>
    ///     </item>
    ///     <item>
    ///       <term> ids </term>
    ///       <description> The IDs, if the selected objects are of type <see cref="IBusinessObjectWithIdentity"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> objects </term>
    ///       <description> The selected <see cref="IBusinessObject"/> instances themself. </description>
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
    [Description ("A comma separated list of parameters for the command. The following reference parameters are available: indices, ids, objects, parent, parentproperty.")]
    public override string Parameters
    {
      get { return base.Parameters; }
      set { base.Parameters = value; }
    }
  }

  private bool _hasClickFired = false;
  private MenuItemHrefCommandInfo _hrefCommand;
  private MenuItemWxeFunctionCommandInfo _wxeFunctionCommand;

  [Browsable (false)]
  public new WebMenuItemClickEventHandler Click;

  public BocMenuItemCommand ()
      : this (CommandType.Event, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
  {
  }

  public BocMenuItemCommand (CommandType defaultType)
      : this (defaultType, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
  {
  }

  public BocMenuItemCommand (
      CommandType defaultType,
      [CanBeNull] IWebSecurityAdapter webSecurityAdapter,
      [CanBeNull] IWxeSecurityAdapter wxeSecurityAdapter)
      : base (defaultType, webSecurityAdapter, wxeSecurityAdapter)
  {
    _hrefCommand = new MenuItemHrefCommandInfo();
    _wxeFunctionCommand = new MenuItemWxeFunctionCommandInfo();
  }

  /// <summary> Fires the <see cref="Click"/> event. </summary>
  public virtual void OnClick (BocMenuItem menuItem)
  {
    base.OnClick (null);
    if (_hasClickFired)
      return;
    _hasClickFired = true;
    if (Click != null)
    {
      WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
      Click (OwnerControl, e);
    }
  }

  /// <summary>
  ///   Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommand"/>.
  /// </summary>
  /// <param name="wxePage"> The <see cref="IWxePage"/> where this command is rendered on. </param>
  /// <param name="listIndices"> 
  ///   The array of indices for the <see cref="IBusinessObject"/> instances on which the rendered 
  ///   command is applied on.
  /// </param>
  /// <param name="businessObjects"> 
  ///   The array of <see cref="IBusinessObject"/> instances on which the rendered command is applied on.
  /// </param>
  public void ExecuteWxeFunction (IWxePage wxePage, int[] listIndices, IBusinessObject[] businessObjects)
  {
    ArgumentUtility.CheckNotNull ("wxePage", wxePage);
    if (!wxePage.IsReturningPostBack)
    {
      NameObjectCollection parameters = PrepareWxeFunctionParameters (listIndices, businessObjects);
      ExecuteWxeFunction (wxePage, parameters);
    }
  }

  private NameObjectCollection PrepareWxeFunctionParameters (int[] listIndices, IBusinessObject[] businessObjects)
  {
    NameObjectCollection parameters = new NameObjectCollection();
    
    parameters["indices"] = listIndices;
    parameters["objects"] = businessObjects;
    if (businessObjects.Length > 0 && businessObjects[0] is IBusinessObjectWithIdentity)
    {
      string[] ids = new string[businessObjects.Length];
      for (int i = 0; i < businessObjects.Length; i++)
        ids[i] = ((IBusinessObjectWithIdentity) businessObjects[i]).UniqueIdentifier;
      parameters["ids"] = ids;
    }
    if (OwnerControl != null)
    {
      if (OwnerControl.DataSource != null && OwnerControl.Value != null)
        parameters["parent"] = OwnerControl.DataSource.BusinessObject;
      if (OwnerControl.Property != null)
        parameters["parentproperty"] = OwnerControl.Property;
    }

    return parameters;
  }

  /// <summary>
  ///   The <see cref="MenuItemHrefCommandInfo"/> used when rendering the command the command as a hyperlink.
  /// </summary>
  /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Href"/>. </remarks>
  /// <value> A <see cref="MenuItemHrefCommandInfo"/> object. </value>
  public override HrefCommandInfo HrefCommand
  {
    get { return _hrefCommand; }
    set { _hrefCommand = (MenuItemHrefCommandInfo) value; }
  }

  /// <summary>
  ///   The <see cref="MenuItemWxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>. </remarks>
  /// <value> A <see cref="MenuItemWxeFunctionCommandInfo"/> object. </value>
  public override WxeFunctionCommandInfo WxeFunctionCommand
  {
    get { return _wxeFunctionCommand; }
    set { _wxeFunctionCommand = (MenuItemWxeFunctionCommandInfo) value; }
  }
}

}
