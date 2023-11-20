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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition using <see cref="BocCustomColumnDefinitionCell"/> for rendering the data. </summary>
  public class BocCustomColumnDefinition : BocColumnDefinition, IBusinessObjectClassSource, IBocSortableColumnDefinition, IBocColumnDefinitionWithRowHeaderSupport, IBocColumnDefinitionWithValidationSupport
  {
    private readonly PropertyPathBinding _propertyPathBinding;
    private BocCustomColumnDefinitionCell? _customCell;
    private string _customCellType = string.Empty;
    private string _customCellArgument = string.Empty;
    private bool _isSortable;
    private bool _isRowHeader;
    private BocCustomColumnDefinitionMode _mode;

    public BocCustomColumnDefinition ()
    {
      _propertyPathBinding = new PropertyPathBinding();
    }

    /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
    protected override void OnOwnerControlChanged ()
    {
      _propertyPathBinding.OwnerControl = OwnerControl;
      base.OnOwnerControlChanged();
    }

    /// <summary> Gets or sets the <see cref="BocCustomColumnDefinitionCell"/> to be used for rendering. </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public BocCustomColumnDefinitionCell CustomCell
    {
      get
      {
        if (_customCell == null)
        {
          if (string.IsNullOrEmpty(_customCellType))
          {
            throw new InvalidOperationException(
                string.Format(
                    "Neither a CustomCell nor a CustomCellType has been specified for BocCustomColumnDefinition '{0}' in BocList '{1}'.",
                    ItemID,
                    OwnerControl!.ID));
          }
          Type type = WebTypeUtility.GetType(_customCellType, true)!;
          _customCell = (BocCustomColumnDefinitionCell)Activator.CreateInstance(type)!;
        }
        return _customCell;
      }
      set { _customCell = value; }
    }

    /// <summary> Gets or sets the type of the <see cref="BocCustomColumnDefinitionCell"/> to be used for rendering. </summary>
    /// <remarks>
    ///    Optionally uses the abbreviated type name as defined in <see cref="TypeUtility.ParseAbbreviatedTypeName"/>. 
    /// </remarks>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Format")]
    [Description("The BocCustomColumnDefinitionCell to be used for rendering.")]
    //  No default value
    [NotifyParentProperty(true)]
    public string CustomCellType
    {
      get { return _customCellType; }
      set { _customCellType = value ?? string.Empty; }
    }

    /// <summary> 
    ///   Gets or sets the comma seperated name/value pairs to set the <see cref="BocCustomColumnDefinitionCell"/>'s 
    ///   properties. 
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Format")]
    [Description("The comma seperated name/value pairs to set the BocCustomColumnDefinitionCell's properties (property=value).")]
    [DefaultValue("")]
    [NotifyParentProperty(true)]
    public string CustomCellArgument
    {
      get { return _customCellArgument; }
      set { _customCellArgument = value ?? string.Empty; }
    }

    [NotNull]
    public IBusinessObjectPropertyPath GetPropertyPath ()
    {
      return _propertyPathBinding.GetPropertyPath();
    }

    public void SetPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      _propertyPathBinding.SetPropertyPath(propertyPath);
    }

    [DefaultValue(false)]
    [Category("Data")]
    public bool IsDynamic
    {
      get { return _propertyPathBinding.IsDynamic; }
      set { _propertyPathBinding.IsDynamic = value; }
    }

    /// <summary>
    ///   Gets or sets the string representation of the <see cref="IBusinessObjectPropertyPath"/>. 
    ///   Must not be <see langword="null"/> or emtpy.
    /// </summary>
    /// <value> A <see cref="string"/> representing the <see cref="IBusinessObjectPropertyPath"/>. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Data")]
    [Description("The string representation of the Property Path. Must not be emtpy.")]
    //  No default value
    [NotifyParentProperty(true)]
    public string? PropertyPathIdentifier
    {
      get { return _propertyPathBinding.PropertyPathIdentifier; }
      set { _propertyPathBinding.PropertyPathIdentifier = value; }
    }


    /// <summary> Gets or sets a flag that determines whether to enable sorting for this columns. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("A flag determining whether to enable sorting for this columns.")]
    [DefaultValue(false)]
    [NotifyParentProperty(true)]
    public bool IsSortable
    {
      get { return _isSortable; }
      set { _isSortable = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to use the value of this column as a row header. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("A flag determining whether to use the value if this column as a row header.")]
    [DefaultValue(true)]
    [NotifyParentProperty(true)]
    public bool IsRowHeader
    {
      get { return _isRowHeader; }
      set { _isRowHeader = value; }
    }

    /// <summary> 
    ///   Gets or sets the <see cref="BocCustomColumnDefinitionMode"/> that determines how the cells are rendered.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("Determines how the cells are rendered.")]
    [DefaultValue(BocCustomColumnDefinitionMode.NoControls)]
    [NotifyParentProperty(true)]
    public BocCustomColumnDefinitionMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }

    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull("serviceLocator", serviceLocator);

      return serviceLocator.GetInstance<IBocCustomColumnRenderer>();
    }

    /// <summary> Gets the displayed value of the column title. </summary>
    /// <remarks> 
    ///   If <see cref="BocColumnDefinition.ColumnTitle"/> is empty or <see langowrd="null"/>, 
    ///   the <c>DisplayName</c> of the <see cref="IBusinessObjectProperty"/> is returned.
    /// </remarks>
    /// <value> A <see cref="string"/> representing this column's title row contents. </value>
    public override WebString ColumnTitleDisplayValue
    {
      get
      {
        if (!ColumnTitle.IsEmpty)
          return ColumnTitle;

        IBusinessObjectPropertyPath propertyPath;
        try
        {
          propertyPath = _propertyPathBinding.GetPropertyPath();
        }
        catch (ParseException)
        {
          // gracefully recover in column header
          return WebString.Empty;
        }

        var lastProperty = propertyPath.Properties.LastOrDefault();
        if (lastProperty == null)
          return WebString.Empty;

        return WebString.CreateFromText(lastProperty.DisplayName);
      }
    }

    /// <summary> The human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "CustomColumnDefinition"; }
    }

    IBusinessObjectClass? IBusinessObjectClassSource.BusinessObjectClass
    {
      get { return _propertyPathBinding.BusinessObjectClass; }
    }

    IComparer<BocListRow> IBocSortableColumnDefinition.CreateCellValueComparer ()
    {
      Assertion.IsNotNull(OwnerControl, "OwnerControl must not be null.");

      var args = new BocCustomCellArguments((IBocList)OwnerControl, this);
      return CustomCell.CreateCellValueComparerInternal(args);
    }

    public virtual IValidationFailureMatcher GetValidationFailureMatcher ()
    {
      Assertion.IsNotNull(OwnerControl, "OwnerControl must not be null.");

      var args = new BocCustomCellArguments((IBocList)OwnerControl, this);
      return CustomCell.GetValidationFailureMatcherInternal(args);
    }
  }

  public enum BocCustomColumnDefinitionMode
  {
    /// <summary> Rendering will be done using the <see cref="BocCustomColumnDefinitionCell.Render"/> method. </summary>
    NoControls,
    /// <summary> 
    ///   Only the modifiable row will contain a control. The other rows will be rendered using the 
    ///   <see cref="BocCustomColumnDefinitionCell.Render"/> method. 
    /// </summary>
    ControlInEditedRow,
    /// <summary> All rows will contain controls. </summary>
    ControlsInAllRows
  }

  /// <summary> Represents the method that handles the <see cref="BocList.CustomCellClick"/> event. </summary>
  public delegate void BocCustomCellClickEventHandler (object sender, BocCustomCellClickEventArgs e);

  /// <summary> Provides data for the <see cref="BocList.CustomCellClick"/> event. </summary>
  public class BocCustomCellClickEventArgs : EventArgs
  {
    private readonly IBusinessObject _businessObject;
    private readonly BocCustomColumnDefinition _column;
    private readonly string? _argument;

    public BocCustomCellClickEventArgs (
        BocCustomColumnDefinition column,
        IBusinessObject businessObject,
        string? argument)
    {
      _businessObject = businessObject;
      _column = column;
      _argument = argument;
    }

    /// <summary> The <see cref="IBusinessObject"/> on which the rendered command is applied on. </summary>
    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }

    /// <summary> The <see cref="BocCustomColumnDefinition"/> to which the command belongs. </summary>
    public BocCustomColumnDefinition Column
    {
      get { return _column; }
    }

    /// <summary> 
    ///   The argument generated by the <see cref="BocCustomColumnDefinitionCell.Render"/> method when registering
    ///   for a <c>click</c> event.
    /// </summary>
    public string? Argument
    {
      get { return _argument; }
    }
  }

  /// <summary> Derive custom cell renderers from this class. </summary>
  /// <remarks>
  ///   <note type="caution"> 
  ///     The same instance of the <b>BocCustomColumnDefinitionCell</b> type is used for all rows in the column.
  ///     It should therefor not be used to save the state of more than one row between the phases of the post back 
  ///     cycle.
  ///   </note>
  /// </remarks>
  public abstract class BocCustomColumnDefinitionCell
  {
    private BocCustomCellArguments? _arguments;

    /// <summary> Get the javascript code that invokes <see cref="OnClick"/> when called. </summary>
    /// <param name="eventArgument"> The event argument to be passed to <see cref="OnClick"/>. </param>
    /// <returns> The script invoking <see cref="OnClick"/> when called. </returns>
    protected string GetPostBackClientEvent (string eventArgument)
    {
      var renderArguments = _arguments as BocCustomCellRenderArguments;
      if (renderArguments == null)
        throw new InvalidOperationException("GetPostBackClientEvent can only be called from DoRender method.");

      var postBackClientEvent = renderArguments.List.GetCustomCellPostBackClientEvent(
          renderArguments.ColumnIndex,
          new BocListRow(renderArguments.ListIndex, renderArguments.BusinessObject),
          eventArgument);
      return postBackClientEvent + renderArguments.OnClick;
    }

    protected void RegisterForSynchronousPostBack (BocListRow row, string eventArgument)
    {
      ArgumentUtility.CheckNotNull("row", row);

      var preRenderArguments = _arguments as BocCustomCellPreRenderArguments;
      if (preRenderArguments == null)
        throw new InvalidOperationException("RegisterForSynchronousPostBack can only be called from OnPreRender method.");

      _arguments!.List.RegisterCustomCellForSynchronousPostBack(preRenderArguments.ColumnIndex, row, eventArgument);
    }

    internal Control CreateControlInternal (BocCustomCellArguments arguments)
    {
      InitArguments(arguments);
      Control control = CreateControl(arguments);
      if (control == null)
      {
        throw new NullReferenceException(
            string.Format(
                "{0}.CreateControl(BocCustomCellArguments) evaluated null, but a Control was expected.", GetType()));
      }
      return control;
    }

    /// <summary> Override this method to create the <see cref="T:Control"/> of a custom cell. </summary>
    /// <param name="arguments"> The <see cref="BocCustomCellArguments"/>. </param>
    /// <returns> A <see cref="T:Control"/>. Must not return <see langword="null"/>. </returns>
    /// <remarks> 
    ///   This method is called for each cell containing a <see cref="T:Control"/>.
    ///   <note type="inheritinfos"> Do not call the base implementation when overriding this method. </note>
    /// </remarks>
    protected virtual Control CreateControl (BocCustomCellArguments arguments)
    {
      throw new NotImplementedException(
          string.Format(
              "{0}: An implementation of 'CreateControl' is required if the 'BocCustomColumnDefinition.Mode' property "
              + "is set to '{1}' or '{2}'.",
              GetType().Name,
              BocCustomColumnDefinitionMode.ControlInEditedRow,
              BocCustomColumnDefinitionMode.ControlsInAllRows));
    }

    internal void Init (BocCustomCellArguments arguments)
    {
      InitArguments(arguments);
      OnInit(arguments);
    }

    /// <summary> Override this method to initialize a custom column. </summary>
    /// <remarks> This method is called for each column if it contains at least one <see cref="T:Control"/>. </remarks>
    protected virtual void OnInit (BocCustomCellArguments arguments)
    {
    }

    internal void Load (BocCustomCellLoadArguments arguments)
    {
      InitArguments(arguments);
      OnLoad(arguments);
    }

    /// <summary> Override this method to process the load phase. </summary>
    /// <param name="arguments"> The <see cref="BocCustomCellLoadArguments"/>. </param>
    /// <remarks> This method is called for each cell containing a <see cref="T:Control"/>. </remarks>
    protected virtual void OnLoad (BocCustomCellLoadArguments arguments)
    {
    }

    internal void Click (BocCustomCellClickArguments arguments, string? eventArgument)
    {
      InitArguments(arguments);
      OnClick(arguments, eventArgument);
    }

    /// <summary> Override this method to process a click event generated by <see cref="GetPostBackClientEvent"/>. </summary>
    /// <param name="arguments"> The <see cref="BocCustomCellClickArguments"/>. </param>
    /// <param name="eventArgument"> The argument passed to <see cref="GetPostBackClientEvent"/>. </param>
    protected virtual void OnClick (BocCustomCellClickArguments arguments, string? eventArgument)
    {
    }

    internal void Validate (BocCustomCellValidationArguments arguments)
    {
      InitArguments(arguments);
      OnValidate(arguments);
    }

    /// <summary> Override this method to process the validation of the editable custom cell during edit mode. </summary>
    /// <param name="arguments"> The <see cref="BocCustomCellValidationArguments"/>. </param>
    protected virtual void OnValidate (BocCustomCellValidationArguments arguments)
    {
    }

    internal void PreRender (BocCustomCellPreRenderArguments arguments)
    {
      InitArguments(arguments);
      OnPreRender(arguments);
    }

    /// <summary> Override this method to prerender a custom column. </summary>
    /// <remarks> This method is called for each column during the <b>PreRender</b> phase of the <see cref="BocList"/>. </remarks>
    protected virtual void OnPreRender (BocCustomCellPreRenderArguments arguments)
    {
    }

    public void RenderInternal (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
    {
      InitArguments(arguments);
      Render(writer, arguments);
    }

    /// <summary> Override this method to render a custom cell. </summary>
    /// <remarks> 
    ///   Use <see cref="GetPostBackClientEvent"/> to render the code that invokes <see cref="OnClick"/>. 
    ///   <note type="inheritinfos"> Do not call the base implementation when overriding this method. </note>
    /// </remarks>
    protected virtual void Render (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
    {
      throw new NotImplementedException(
          string.Format(
              "{0}: An implementation of 'Render' is required if the 'BocCustomColumnDefinition.Mode' property "
              + "is set to '{1}' or '{2}'.",
              GetType().Name,
              BocCustomColumnDefinitionMode.NoControls,
              BocCustomColumnDefinitionMode.ControlInEditedRow));
    }

    [NotNull]
    internal IComparer<BocListRow> CreateCellValueComparerInternal (BocCustomCellArguments arguments)
    {
      InitArguments(arguments);
      return CreateCellValueComparer(arguments);
    }

    [NotNull]
    internal IValidationFailureMatcher GetValidationFailureMatcherInternal (BocCustomCellArguments arguments)
    {
      InitArguments(arguments);
      return GetValidationFailureMatcher(arguments);
    }

    /// <summary>
    /// Override this method to change the implementation of <see cref="IComparer{T}"/> instantiated for comparing the two <see cref="BocListRow"/>
    /// instances based on this <see cref="BocCustomColumnDefinitionCell"/>.
    /// </summary>
    /// <returns>An implementation of <see cref="IComparer{T}"/>, typed to <see cref="BocListRow"/>.</returns>
    /// <remarks>The default type created is <see cref="BusinessObjectPropertyPathBasedComparer"/>.</remarks>
    [NotNull]
    protected virtual IComparer<BocListRow> CreateCellValueComparer (BocCustomCellArguments arguments)
    {
      ArgumentUtility.CheckNotNull("arguments", arguments);

      return arguments.ColumnDefinition.GetPropertyPath().CreateComparer();
    }

    /// <summary>
    /// Override this method to change the implementation of <see cref="Remotion.ObjectBinding.Validation.IValidationFailureMatcher"/> instantiated to match all failures pertaining to this <see cref="BocListRow"/>
    /// instances based on this <see cref="BocCustomColumnDefinitionCell"/>.
    /// </summary>
    /// <returns>An implementation of <see cref="Remotion.ObjectBinding.Validation.IValidationFailureMatcher"/>.</returns>
    /// <remarks>The default type created is <see cref="Remotion.ObjectBinding.Validation.BusinessObjectPropertyPathValidationFailureMatcher"/>.</remarks>
    [NotNull]
    protected virtual IValidationFailureMatcher GetValidationFailureMatcher (BocCustomCellArguments arguments)
    {
      ArgumentUtility.CheckNotNull(nameof(arguments), arguments);

      var propertyPath = arguments.ColumnDefinition.GetPropertyPath();
      return new BusinessObjectPropertyPathValidationFailureMatcher(propertyPath);
    }

    private void InitArguments (BocCustomCellArguments arguments)
    {
      _arguments = arguments;

      string propertyValuePairs = arguments.ColumnDefinition.CustomCellArgument;
      if (! string.IsNullOrEmpty(propertyValuePairs))
      {
        NameValueCollection values = new NameValueCollection();
        StringUtility.ParsedItem[] items = StringUtility.ParseSeparatedList(propertyValuePairs, ',');
        for (int i = 0; i < items.Length; i++)
        {
          string[] pair = items[i].Value.Split(new[] { '=' }, 2);
          if (pair.Length == 2)
          {
            string key = pair[0].Trim();
            string value = pair[1].Trim(' ', '\"');
            values.Add(key, value);
          }
        }
        PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < properties.Length; i++)
        {
          PropertyInfo property = properties[i];
          string? strval = values[property.Name];
          if (strval != null)
          {
            try
            {
              if (strval.Length >= 2 && strval[0] == '\"' && strval[strval.Length - 1] == '\"')
                strval = strval.Substring(1, strval.Length - 2);
              object? value = StringUtility.Parse(property.PropertyType, strval, CultureInfo.InvariantCulture);
              property.SetValue(this, value, new object[0]);
            }
            catch (Exception e)
            {
              throw new ApplicationException("Property " + property.Name + ": " + e.Message, e);
            }
          }
        }
      }
    }
  }

  /// <summary>
  ///   Contains the arguments provided to the <see cref="BocCustomColumnDefinitionCell.CreateControl"/>,
  ///   <see cref="BocCustomColumnDefinitionCell.OnInit"/>, and  
  ///   <see cref="BocCustomColumnDefinitionCell.OnPreRender(BocCustomCellPreRenderArguments)"/> methods.
  /// </summary>
  public class BocCustomCellArguments
  {
    private readonly IBocList _list;
    private readonly BocCustomColumnDefinition _columnDefinition;

    public BocCustomCellArguments (
        IBocList list,
        BocCustomColumnDefinition columnDefinition)
    {
      _list = list;
      _columnDefinition = columnDefinition;
    }

    /// <summary> Gets the <see cref="BocList"/> containing the column. </summary>
    public IBocList List
    {
      get { return _list; }
    }

    /// <summary> Gets the column definition of the column that should be rendered or that was clicked. </summary>
    public BocCustomColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }
  }

  /// <summary> Contains the arguments provided to the <see cref="BocCustomColumnDefinitionCell.OnLoad"/> method. </summary>
  public class BocCustomCellLoadArguments : BocCustomCellArguments
  {
    private readonly IBusinessObject _businessObject;
    private readonly int _listIndex;
    private readonly Control _control;

    public BocCustomCellLoadArguments (
        BocList list,
        IBusinessObject businessObject,
        BocCustomColumnDefinition columnDefinition,
        int listIndex,
        Control control)
        : base(list, columnDefinition)
    {
      _businessObject = businessObject;
      _listIndex = listIndex;
      _control = control;
    }

    /// <summary> Gets the <see cref="IBusinessObject"/> of the row being loaded. </summary>
    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }

    /// <summary> Gets the list index of the current business object. </summary>
    public int ListIndex
    {
      get { return _listIndex; }
    }

    /// <summary> Gets the <see cref="T:Control"/> of this row. </summary>
    public Control Control
    {
      get { return _control; }
    }
  }

  /// <summary> Contains the arguments provided to the <see cref="BocCustomColumnDefinitionCell.OnClick"/> method. </summary>
  public class BocCustomCellClickArguments : BocCustomCellArguments
  {
    private readonly IBusinessObject _businessObject;

    public BocCustomCellClickArguments (
        BocList list,
        IBusinessObject businessObject,
        BocCustomColumnDefinition columnDefinition)
        : base(list, columnDefinition)
    {
      _businessObject = businessObject;
    }

    /// <summary> Gets the <see cref="IBusinessObject"/> that was clicked. </summary>
    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }
  }

  /// <summary> Contains the arguments provided to the <see cref="BocCustomColumnDefinitionCell.OnClick"/> method. </summary>
  public class BocCustomCellValidationArguments : BocCustomCellArguments
  {
    private readonly IBusinessObject _businessObject;
    private readonly Control _control;
    private bool _isValid;

    public BocCustomCellValidationArguments (
        BocList list,
        IBusinessObject businessObject,
        BocCustomColumnDefinition columnDefinition,
        Control control)
        : base(list, columnDefinition)
    {
      _businessObject = businessObject;
      _control = control;
      _isValid = true;
    }

    /// <summary> Gets the <see cref="IBusinessObject"/> that was clicked. </summary>
    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }

    /// <summary> Gets the <see cref="T:Control"/> of this row. </summary>
    public Control Control
    {
      get { return _control; }
    }

    /// <summary> Gets or sets a flag that specifies whether the cell's control contains a valid value. </summary>
    public bool IsValid
    {
      get { return _isValid; }
      set { _isValid = value; }
    }
  }

  public class BocCustomCellPreRenderArguments : BocCustomCellArguments
  {
    private readonly int _columnIndex;

    public BocCustomCellPreRenderArguments (IBocList list, BocCustomColumnDefinition columnDefinition, int columnIndex)
        : base(list, columnDefinition)
    {
      _columnIndex = columnIndex;
    }

    /// <summary> Gets the index of the pre-rendered column. </summary>
    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    public IEnumerable<BocListRow> GetRowsToRender ()
    {
      return List.GetRowsToRender().Select(row =>row.Row);
    }
  }

  /// <summary> Contains the arguments provided to the <see cref="BocCustomColumnDefinitionCell.Render"/> method. </summary>
  public class BocCustomCellRenderArguments : BocCustomCellArguments
  {
    private readonly int _columnIndex;
    private readonly IBusinessObject _businessObject;
    private readonly int _listIndex;
    private IReadOnlyCollection<string> _headerIDs;
    private readonly string _onClick;

    public BocCustomCellRenderArguments (
        IBocList list,
        IBusinessObject businessObject,
        BocCustomColumnDefinition columnDefinition,
        int columnIndex,
        int listIndex,
        IReadOnlyCollection<string> headerIDs,
        string onClick)
        : base(list, columnDefinition)
    {
      _columnIndex = columnIndex;
      _businessObject = businessObject;
      _listIndex = listIndex;
      _headerIDs = headerIDs;
      _onClick = onClick;
    }

    /// <summary> Gets the index of the rendered column. </summary>
    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    /// <summary> Gets the <see cref="IBusinessObject"/> that should be rendered. </summary>
    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }

    /// <summary> The list index of the current business object. </summary>
    public int ListIndex
    {
      get { return _listIndex; }
    }

    public IReadOnlyCollection<string> HeaderIDs
    {
      get { return _headerIDs; }
    }

    /// <summary> Gets client script code that prevents row selection. For use with hyperlinks. </summary>
    /// <remarks>
    ///   The function tasked with preventing the row from being selected/highlighted when clicking on the link 
    ///   itself instead of the row.
    ///   <note>
    ///     This string should be rendered in the <b>OnClick</b> client-side event of hyperlinks. It is not used for 
    ///     post-backs javascript calls.
    ///   </note>
    /// </remarks>
    public string OnClick
    {
      get { return _onClick; }
    }
  }
}
