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
using System.Web.UI.WebControls;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public enum ListControlType
  {
    DropDownList = 0,
    ListBox = 1,
    RadioButtonList = 2
  }

  /// <summary>
  /// Specifies the behavior mode of the text box.
  /// 
  /// </summary>
  public enum BocTextBoxMode
  {
    SingleLine,
    MultiLine,
    /// <summary>
    /// Renders the value but sets the input type to masked.
    /// </summary>
    PasswordRenderMasked,
    /// <summary>
    /// Does not render the value, even in its masked form. User input is still displayed as masked.
    /// </summary>
    PasswordNoRender
  }

  public class ListControlStyle : Style
  {
    private class CompatibleListBox : ListBox
    {
      public override bool SupportsDisabledAttribute => true;
    }

    private ListControlType _controlType = ListControlType.DropDownList;
    private bool? _autoPostBack;
    private int? _listBoxRows;
    private int? _radioButtonListCellPadding;
    private int? _radioButtonListCellSpacing;
    private int? _radioButtonListRepeatColumns;
    private RepeatDirection _radioButtonListRepeatDirection = RepeatDirection.Vertical;
    private RepeatLayout _radionButtonListRepeatLayout = RepeatLayout.Table;
    private TextAlign _radioButtonListTextAlign = TextAlign.Right;
    private bool _radioButtonListNullValueVisible = true;
    private bool _dropDownListNullValueTextVisible = false;

    [Description("The type of control that is used in edit mode.")]
    [Category("Behavior")]
    [DefaultValue(ListControlType.DropDownList)]
    [NotifyParentProperty(true)]
    public ListControlType ControlType
    {
      get { return _controlType; }
      set { _controlType = value; }
    }

    [Description("Automatically postback to the server after the text is modified.")]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public bool? AutoPostBack
    {
      get { return _autoPostBack; }
      set { _autoPostBack = value; }
    }

    [Description("The number of visible rows to display.")]
    [Category("Appearance")]
    [DefaultValue(typeof(int?), "")]
    [NotifyParentProperty(true)]
    public int? ListBoxRows
    {
      get { return _listBoxRows; }
      set { _listBoxRows = value; }
    }

    [Description("The padding between each item.")]
    [Category("Layout")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public int? RadioButtonListCellPadding
    {
      get { return _radioButtonListCellPadding; }
      set { _radioButtonListCellPadding = value; }
    }

    [Description("The spacing between each item.")]
    [Category("Layout")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public int? RadioButtonListCellSpacing
    {
      get { return _radioButtonListCellSpacing; }
      set { _radioButtonListCellSpacing = value; }
    }

    [Description("The number of columns to use to lay out the items.")]
    [Category("Layout")]
    [DefaultValue(typeof(int?), "")]
    [NotifyParentProperty(true)]
    public int? RadioButtonListRepeatColumns
    {
      get { return _radioButtonListRepeatColumns; }
      set { _radioButtonListRepeatColumns = value; }
    }

    [Description("The direction in which items are laid out.")]
    [Category("Layout")]
    [DefaultValue(RepeatDirection.Vertical)]
    [NotifyParentProperty(true)]
    public RepeatDirection RadioButtonListRepeatDirection
    {
      get { return _radioButtonListRepeatDirection; }
      set { _radioButtonListRepeatDirection = value; }
    }

    [Description("Whether items are repeated in a table or in-flow.")]
    [Category("Layout")]
    [DefaultValue(RepeatLayout.Table)]
    [NotifyParentProperty(true)]
    public RepeatLayout RadionButtonListRepeatLayout
    {
      get { return _radionButtonListRepeatLayout; }
      set { _radionButtonListRepeatLayout = value; }
    }

    [Description("The alignment of the text label with respect to each item.")]
    [Category("Appearance")]
    [DefaultValue(TextAlign.Right)]
    [NotifyParentProperty(true)]
    public TextAlign RadioButtonListTextAlign
    {
      get { return _radioButtonListTextAlign; }
      set { _radioButtonListTextAlign = value; }
    }

    [Description("A flag that determines whether to show the null value in the radio button list.")]
    [Category("Behavior")]
    [DefaultValue(true)]
    [NotifyParentProperty(true)]
    public bool RadioButtonListNullValueVisible
    {
      get { return _radioButtonListNullValueVisible; }
      set { _radioButtonListNullValueVisible = value; }
    }

    /// <summary>
    /// Gets or sets a flag that determines whether to show the text for <see langword="null" /> in the drop-down list.
    /// </summary>
    /// <remarks>
    /// For some combinations of browser and screen reader, this flag must be set in order to properly announce <see langword="null" /> to the user.
    /// </remarks>
    [Description("A flag that determines whether to show the text for the null value in the drop-down list.")]
    [Category("Behavior")]
    [DefaultValue(false)]
    [NotifyParentProperty(true)]
    public bool DropDownListNullValueTextVisible
    {
      get { return _dropDownListNullValueTextVisible; }
      set { _dropDownListNullValueTextVisible = value; }
    }

    public ListControl Create (bool applyStyle)
    {
      ListControl control;
      switch (_controlType)
      {
        case ListControlType.DropDownList:
          control = new DropDownList();
          break;
        case ListControlType.ListBox:
          control = new CompatibleListBox();
          break;
        case ListControlType.RadioButtonList:
          control = new RadioButtonList();
          break;
        default:
          throw new NotSupportedException("Control type " + _controlType);
      }
      if (applyStyle)
        ApplyStyle(control);
      return control;
    }

    public void ApplyCommonStyle (ListControl listControl)
    {
      listControl.ApplyStyle(this);
      if (_autoPostBack != null)
        listControl.AutoPostBack = _autoPostBack.Value;
    }

    public void ApplyStyle (ListControl listControl)
    {
      if (listControl is ListBox)
        ApplyStyle((ListBox)listControl);
      else if (listControl is DropDownList)
        ApplyStyle((DropDownList)listControl);
      else if (listControl is RadioButtonList)
        ApplyStyle((RadioButtonList)listControl);
      else
        ApplyCommonStyle(listControl);
    }

    public void ApplyStyle (ListBox listBox)
    {
      ApplyCommonStyle(listBox);

      if (_listBoxRows != null)
        listBox.Rows = _listBoxRows.Value;
    }

    public void ApplyStyle (DropDownList dropDownList)
    {
      ApplyCommonStyle(dropDownList);
    }

    public void ApplyStyle (RadioButtonList radioButtonList)
    {
      ApplyCommonStyle(radioButtonList);

      if (_radioButtonListCellPadding != null)
        radioButtonList.CellPadding = _radioButtonListCellPadding.Value;

      if (_radioButtonListCellSpacing != null)
        radioButtonList.CellSpacing = _radioButtonListCellSpacing.Value;

      if (_radioButtonListRepeatColumns != null)
        radioButtonList.RepeatColumns = _radioButtonListRepeatColumns.Value;

      radioButtonList.RepeatDirection = _radioButtonListRepeatDirection;
      radioButtonList.TextAlign = _radioButtonListTextAlign;
      radioButtonList.RepeatLayout = _radionButtonListRepeatLayout;
    }
  }

  public class DropDownListStyle : Style
  {
    private bool? _autoPostBack;
    private bool _nullValueTextVisible;

    [Description("Automatically postback to the server after the text is modified.")]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public bool? AutoPostBack
    {
      get { return _autoPostBack; }
      set { _autoPostBack = value; }
    }

    /// <summary>
    /// Gets or sets a flag that determines whether to show the text for <see langword="null" />.
    /// </summary>
    /// <remarks>
    /// For some combinations of browser and screen reader, this flag must be set in order to properly announce <see langword="null" /> to the user.
    /// </remarks>
    [Description("A flag that determines whether to show the text for the null value.")]
    [Category("Behavior")]
    [DefaultValue(false)]
    [NotifyParentProperty(true)]
    public bool NullValueTextVisible
    {
      get { return _nullValueTextVisible; }
      set { _nullValueTextVisible = value; }
    }

    public void ApplyStyle (DropDownList dropDownList)
    {
      dropDownList.ApplyStyle(this);
      if (_autoPostBack != null)
        dropDownList.AutoPostBack = _autoPostBack.Value;
    }
  }

  /// <summary>
  /// Styles for single row TextBox controls.
  /// </summary>
  public class SingleRowTextBoxStyle : Style
  {
    private int? _columns;
    private int? _maxLength;
    private int? _maxLengthByPropertyConstraint;
    private int? _maxLengthFromDomainModel;
    private bool? _readOnly;
    private bool? _autoPostBack;
    private bool? _checkClientSideMaxLength;

    public virtual void ApplyStyle (TextBox textBox)
    {
      textBox.ApplyStyle(this);

      if (_maxLength != null && _checkClientSideMaxLength != false)
        textBox.MaxLength = _maxLength.Value;

      if (_columns != null)
        textBox.Columns = _columns.Value;

      if (_autoPostBack != null)
        textBox.AutoPostBack = _autoPostBack.Value;

      if (_readOnly != null)
        textBox.ReadOnly = _readOnly.Value;
    }

    public override void CopyFrom (Style s)
    {
      base.CopyFrom(s);
      SingleRowTextBoxStyle? ts = s as SingleRowTextBoxStyle;
      if (ts != null)
      {
        if (_checkClientSideMaxLength != false)
          _maxLength = ts.GetMaxLength();
        _columns = ts.Columns;
        _readOnly = ts.ReadOnly;
      }
    }

    [Description("The width of the textbox in characters.")]
    [Category("Appearance")]
    [DefaultValue(typeof(int?), "")]
    [NotifyParentProperty(true)]
    public int? Columns
    {
      get { return _columns; }
      set { _columns = value; }
    }

    [Description("The maximum number of characters that can be entered.")]
    [Category("Behavior")]
    [DefaultValue(typeof(int?), "")]
    [NotifyParentProperty(true)]
    public int? MaxLength
    {
      [Obsolete("Use SingleRowTextBoxStyle.GetMaxLength() instead.")]
      get { return _maxLength; }
      set { _maxLength = value; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? MaxLengthByPropertyConstraint
    {
      get => _maxLengthByPropertyConstraint;
      set => _maxLengthByPropertyConstraint = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? MaxLengthFromDomainModel
    {
      get => _maxLengthFromDomainModel;
      set => _maxLengthFromDomainModel = value;
    }

    [Description("Whether the text in the control can be changed or not.")]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public bool? ReadOnly
    {
      get { return _readOnly; }
      set { _readOnly = value; }
    }

    [Description("Automatically postback to the server after the text is modified.")]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public bool? AutoPostBack
    {
      get { return _autoPostBack; }
      set { _autoPostBack = value; }
    }

    [Description(
        "Whether the text in the control can exceed its max length during input. If true, MaxLength is only used for validation after the input is completed."
        )]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public bool? CheckClientSideMaxLength
    {
      get { return _checkClientSideMaxLength; }
      set { _checkClientSideMaxLength = value; }
    }

    public int? GetMaxLength ()
    {
      if (_maxLength.HasValue)
        return _maxLength.Value;

      if (_maxLengthByPropertyConstraint.HasValue)
        return _maxLengthByPropertyConstraint.Value;

      if (_maxLengthFromDomainModel.HasValue)
        return _maxLengthFromDomainModel.Value;

      return null;
    }
  }

  /// <summary>
  /// Styles for TextBox controls.
  /// </summary>
  public class TextBoxStyle : SingleRowTextBoxStyle
  {
    private const string c_scriptFileUrl = "TextBoxStyle.js";
    private static readonly string s_scriptFileKey = typeof(TextBoxStyle).GetFullNameChecked() + "_Script";

    private int? _rows;
    private PlainTextString _placeholder;
    private BocTextBoxMode _textMode;
    private readonly BocTextBoxMode _defaultTextMode = BocTextBoxMode.SingleLine;
    private bool? _wrap;
    private string _autoComplete;

    public TextBoxStyle (BocTextBoxMode defaultTextMode)
    {
      _defaultTextMode = defaultTextMode;
      _textMode = _defaultTextMode;
      _autoComplete = string.Empty;
    }

    public TextBoxStyle ()
        : this(BocTextBoxMode.SingleLine)
    {
    }

    public override void ApplyStyle (TextBox textBox)
    {
      base.ApplyStyle(textBox);

      if (_rows != null)
        textBox.Rows = _rows.Value;

      if (_wrap != null)
        textBox.Wrap = _wrap.Value;

      if (!_placeholder.IsEmpty)
        textBox.Attributes.Add("placeholder", _placeholder.GetValue());

      if (!string.IsNullOrEmpty(_autoComplete))
        textBox.Attributes.Add("autocomplete", _autoComplete);

      var maxLength = GetMaxLength();

      if (_textMode == BocTextBoxMode.MultiLine
          && maxLength != null
          && CheckClientSideMaxLength != false)
        textBox.Attributes.Add("onkeydown", "return TextBoxStyle.OnKeyDown (this, " + maxLength.Value + ");");

      textBox.TextMode = GetSystemWebTextMode();
    }

    private TextBoxMode GetSystemWebTextMode ()
    {
      switch (_textMode)
      {
        case BocTextBoxMode.SingleLine:
          return TextBoxMode.SingleLine;
        case BocTextBoxMode.MultiLine:
          return TextBoxMode.MultiLine;
        case BocTextBoxMode.PasswordRenderMasked:
        case BocTextBoxMode.PasswordNoRender:
          return TextBoxMode.Password;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RegisterJavaScriptInclude (IResourceUrlFactory resourceUrlFactory, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      var scriptUrl = resourceUrlFactory.CreateResourceUrl(typeof(TextBoxStyle), ResourceType.Html, c_scriptFileUrl);
      htmlHeadAppender.RegisterJavaScriptInclude(s_scriptFileKey, scriptUrl);
    }

    public override void CopyFrom (Style s)
    {
      base.CopyFrom(s);
      TextBoxStyle? ts = s as TextBoxStyle;
      if (ts != null)
      {
        Rows = ts.Rows;
        TextMode = ts.TextMode;
        Wrap = ts.Wrap;
        Placeholder = ts.Placeholder;
        AutoComplete = ts.AutoComplete;
      }
    }

    [Description("The number of lines to display for a multiline textbox.")]
    [Category("Behavior")]
    [DefaultValue(typeof(int?), "")]
    [NotifyParentProperty(true)]
    public int? Rows
    {
      get { return _rows; }
      set { _rows = value; }
    }

    [Description("The placeholder value of the textbox.")]
    [Category("Data")]
    [DefaultValue(typeof(string), "")]
    [NotifyParentProperty(true)]
    public PlainTextString Placeholder
    {
      get { return _placeholder; }
      set { _placeholder = value; }
    }

    ///  <summary>
    /// Defines the autocomplete behaviour of the textbox.
    /// Standard values are provided in <see cref="BocTextValueAutoCompleteAttribute"/>.
    /// </summary>
    [Description("The autocomplete behaviour of the textbox.")]
    [Category("Behavior")]
    [DefaultValue(typeof(string), "")]
    [NotifyParentProperty(true)]
    public string AutoComplete
    {
      get { return _autoComplete; }
      set { _autoComplete = StringUtility.NullToEmpty(value); }
    }

    [Description("The behavior mode of the textbox.")]
    [Category("Behavior")]
    [NotifyParentProperty(true)]
    public BocTextBoxMode TextMode
    {
      get { return _textMode; }
      set { _textMode = value; }
    }

    /// <summary> Controls the persisting of the <see cref="TextMode"/>. </summary>
    private bool ShouldSerializeTextMode ()
    {
      return _textMode != _defaultTextMode;
    }

    /// <summary> Sets the <see cref="TextMode"/> to its default value. </summary>
    private void ResetTextMode ()
    {
      _textMode = _defaultTextMode;
    }

    [Description("Gets or sets a value indicating whether the text should be wrapped in edit mode.")]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public bool? Wrap
    {
      get { return _wrap; }
      set { _wrap = value; }
    }
  }

  // obsolete since CompoundValidator supports Visible and EnableClientScript directly through ICompleteValidator
  ///// <summary>
  ///// Styles for validator controls.
  ///// </summary>
  //public class ValidatorStyle: Style
  //{
  //  public enum OptionalValidatorDisplay
  //  {
  //    Undefined = -1,
  //    Dynamic = ValidatorDisplay.Dynamic,
  //    Static = ValidatorDisplay.Static,
  //    None = ValidatorDisplay.None
  //  }
  //
  //  private OptionalValidatorDisplay _display = OptionalValidatorDisplay.Undefined;
  //  private NaBooleanEnum _enableClientScript = NaBooleanEnum.Undefined;
  //
  //  public void ApplyStyle (BaseValidator validator)
  //  {
  //    validator.ApplyStyle (this);
  //    NaBoolean enableClientScript = _enableClientScript;
  //    if (! enableClientScript.IsNull)
  //      validator.EnableClientScript = (bool) enableClientScript;
  //    if (_display == OptionalValidatorDisplay.Undefined)
  //      validator.Display = (ValidatorDisplay) _display;
  //  }
  //
  //  public override void CopyFrom (Style s)
  //  {
  //    base.CopyFrom (s);
  //    ValidatorStyle vs = s as ValidatorStyle;
  //    if (vs != null)
  //    {
  //      this.Display = vs.Display;
  //      this.EnableClientScript = vs.EnableClientScript;
  //    }
  //  }
  //
  //  [Description("How the validator is displayed.")]
  //  [Category("Appearance")]
  //  [DefaultValue (typeof (OptionalValidatorDisplay), "Undefined")]
  //  [NotifyParentProperty (true)]
  //  public OptionalValidatorDisplay Display
  //  {
  //    get { return _display; }
  //    set { _display = value; }
  //  }
  //
  //  [Description("Indicates whether to perform validation on the client in up-level browsers.")]
  //  [Category("Behavior")]
  //  [DefaultValue (true)]
  //  [NotifyParentProperty (true)]
  //  public NaBooleanEnum EnableClientScript
  //  {
  //    get { return _enableClientScript; }
  //    set { _enableClientScript = value; }
  //  }
  //}
}
