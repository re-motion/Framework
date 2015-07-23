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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  //  TODO: BocListItemCommand: Move long comment blocks to xml-file
  /// <summary> A <see cref="BocListItemCommand"/> defines an action the user can invoke on a datarow. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class BocListItemCommand : BocCommand
  {
    /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class ListItemHrefCommandInfo : BocHrefCommandInfo
    {
      /// <summary> Initalizes a new instance </summary>
      public ListItemHrefCommandInfo ()
      {
      }

      /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
      /// <value> 
      ///   The URL to link to when the rendered command is clicked. The default value is 
      ///   an empty <see cref="String"/>. 
      /// </value>
      [Description (
          "The hyperlink reference of the command. Use {0} to insert the Business Object's index in the list and {1} to insert the Business Object's ID."
          )]
      public override string Href
      {
        get { return base.Href; }
        set { base.Href = value; }
      }
    }

    /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class ListItemWxeFunctionCommandInfo : BocWxeFunctionCommandInfo
    {
      /// <summary> Initalizes a new instance </summary>
      public ListItemWxeFunctionCommandInfo ()
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
      ///       <term> index </term>
      ///       <description> 
      ///         The index of the <see cref="IBusinessObject"/> in the <see cref="IBusinessObjectProperty"/>.
      ///       </description>
      ///     </item>
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
          "A comma separated list of parameters for the command. The following reference parameters are available: index, id, object, parent, parentproperty."
          )]
      public override string Parameters
      {
        get { return base.Parameters; }
        set { base.Parameters = value; }
      }
    }


    public new BocListItemCommandClickEventHandler Click;
    private IBocListItemCommandState _commandState;
    private string _commandStateType = string.Empty;
    private ListItemHrefCommandInfo _hrefCommand;
    private ListItemWxeFunctionCommandInfo _wxeFunctionCommand;

    public BocListItemCommand ()
        : this (CommandType.None, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
    {
    }

    public BocListItemCommand (CommandType defaultType)
        : this (defaultType, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
    {
    }

    public BocListItemCommand (
        CommandType defaultType,
        [CanBeNull] IWebSecurityAdapter webSecurityAdapter,
        [CanBeNull] IWxeSecurityAdapter wxeSecurityAdapter)
        : base (defaultType, webSecurityAdapter, wxeSecurityAdapter)
    {
      _hrefCommand = new ListItemHrefCommandInfo();
      _wxeFunctionCommand = new ListItemWxeFunctionCommandInfo();
    }

    /// <summary> Fires the <see cref="Click"/> event. </summary>
    public virtual void OnClick (BocCommandEnabledColumnDefinition column, int listIndex, IBusinessObject businessObject)
    {
      if (Click != null)
      {
        BocListItemCommandClickEventArgs e = new BocListItemCommandClickEventArgs (this, column, listIndex, businessObject);
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
    /// <param name="listIndex">
    ///   An index that indentifies the <see cref="IBusinessObject"/> on which the rendered command is applied on.
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
        int listIndex,
        string businessObjectID,
        ISecurableObject securableObject)
    {
      base.RenderBegin (writer, renderingFeatures, postBackLink, new string[] { listIndex.ToString(), businessObjectID }, onClick, securableObject);
    }

    /// <summary>
    ///   Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommand"/>.
    /// </summary>
    /// <param name="wxePage"> The <see cref="IWxePage"/> where this command is rendered on. </param>
    /// <param name="listIndex"> 
    ///   The index of the <see cref="IBusinessObject"/> in the row where the command was clicked.
    /// </param>
    /// <param name="businessObject">
    ///   The <see cref="IBusinessObject"/> in the row where the command was clicked.
    /// </param>
    public void ExecuteWxeFunction (IWxePage wxePage, int listIndex, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("wxePage", wxePage);
      if (!wxePage.IsReturningPostBack)
      {
        NameObjectCollection parameters = PrepareWxeFunctionParameters (listIndex, businessObject);
        ExecuteWxeFunction (wxePage, parameters);
      }
    }

    private NameObjectCollection PrepareWxeFunctionParameters (int listIndex, IBusinessObject businessObject)
    {
      NameObjectCollection parameters = new NameObjectCollection();

      parameters["index"] = listIndex;
      parameters["object"] = businessObject;
      if (businessObject is IBusinessObjectWithIdentity)
        parameters["id"] = ((IBusinessObjectWithIdentity) businessObject).UniqueIdentifier;
      else
        parameters["id"] = null;
      if (OwnerControl != null)
      {
        if (OwnerControl.DataSource != null && OwnerControl.Value != null)
          parameters["parent"] = OwnerControl.DataSource.BusinessObject;
        if (OwnerControl.Property != null)
          parameters["parentproperty"] = OwnerControl.Property;
      }

      return parameters;
    }

    /// <summary> The <see cref="ListItemHrefCommandInfo"/> used when rendering the command as a hyperlink. </summary>
    /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Href"/>. </remarks>
    /// <value> A <see cref="ListItemHrefCommandInfo"/> object. </value>
    public override HrefCommandInfo HrefCommand
    {
      get { return _hrefCommand; }
      set { _hrefCommand = (ListItemHrefCommandInfo) value; }
    }

    /// <summary>
    ///   The <see cref="ListItemWxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
    /// </summary>
    /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>. </remarks>
    /// <value> A <see cref="ListItemWxeFunctionCommandInfo"/> object. </value>
    public override WxeFunctionCommandInfo WxeFunctionCommand
    {
      get { return _wxeFunctionCommand; }
      set { _wxeFunctionCommand = (ListItemWxeFunctionCommandInfo) value; }
    }

    /// <summary> The <see cref="IBocListItemCommandState"/> to be used for evaluating whether to render the command. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IBocListItemCommandState CommandState
    {
      get
      {
        if (_commandState == null && !string.IsNullOrEmpty (_commandStateType))
        {
          Type type = WebTypeUtility.GetType (_commandStateType, true);
          _commandState = (IBocListItemCommandState) Activator.CreateInstance (type, null);
        }
        return _commandState;
      }
      set { _commandState = value; }
    }

    /// <summary> 
    ///   Gets or sets the type of the <see cref="IBocListItemCommandState"/> to be used for evaluating whether to 
    ///   render the command. 
    /// </summary>
    /// <remarks>
    ///    Optionally uses the abbreviated type name as defined in <see cref="TypeUtility.ParseAbbreviatedTypeName"/>. 
    /// </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The IBocListItemCommandState to be used for evaluating whether to render the command.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string CommandStateType
    {
      get { return _commandStateType; }
      set { _commandStateType = (value ?? string.Empty).Trim(); }
    }
  }

  /// <summary>
  ///   Represents the method that handles the <see cref="BocListItemCommand.Click"/> event
  ///   raised when clicking on a <see cref="Command"/> of type <see cref="CommandType.Event"/>.
  /// </summary>
  public delegate void BocListItemCommandClickEventHandler (object sender, BocListItemCommandClickEventArgs e);

  /// <summary> Provides data for the <see cref="BocListItemCommand.Click"/> event. </summary>
  public class BocListItemCommandClickEventArgs : BocCommandClickEventArgs
  {
    private BocCommandEnabledColumnDefinition _column;
    private int _listIndex;

    public BocListItemCommandClickEventArgs (
        BocListItemCommand command,
        BocCommandEnabledColumnDefinition column,
        int listIndex,
        IBusinessObject businessObject)
        : base (command, businessObject)
    {
      _column = column;
      _listIndex = listIndex;
    }

    /// <summary> The <see cref="BocListItemCommand"/> that caused the event. </summary>
    public new BocListItemCommand Command
    {
      get { return (BocListItemCommand) base.Command; }
    }

    /// <summary> The <see cref="BocCommandEnabledColumnDefinition"/> to which the command belongs. </summary>
    public BocCommandEnabledColumnDefinition Column
    {
      get { return _column; }
    }

    /// <summary> An index that identifies the <see cref="IBusinessObject"/> on which the rendered command is applied on. </summary>
    public int ListIndex
    {
      get { return _listIndex; }
    }
  }

  /// <summary> 
  ///   This interface allows for the customized disabling of the <see cref="BocCommandEnabledColumnDefinition"/>'s 
  ///   <see cref="BocListItemCommand"/>.
  /// </summary>
  public interface IBocListItemCommandState
  {
    /// <summary> 
    ///   Evaluates whether the <paramref name="columnDefinition"/>'s command should be enabled for the 
    ///   <paramref name="businessObject"/>.
    /// </summary>
    /// <remarks>
    ///   Only executed if all other conditions (<see cref="Command.Type"/>, <see cref="Command.Show"/>, ...) 
    ///   for enabling the command are met. 
    /// </remarks>
    /// <param name="list"> The <see cref="BocList"/> containing the column. </param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> to be rendered. </param>
    /// <param name="columnDefinition"> The column definition of the rendered column. </param>
    /// <returns> 
    ///   <see langword="true"/> if the <paramref name="columnDefinition"/>'s command should be enabled for the 
    ///   <paramref name="businessObject"/>. 
    /// </returns>
    bool IsEnabled (IBocList list, IBusinessObject businessObject, BocCommandEnabledColumnDefinition columnDefinition);
  }
}
