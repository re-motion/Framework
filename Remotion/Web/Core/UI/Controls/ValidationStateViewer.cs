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
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Collects the validation errors from all <see cref="FormGridManager"/> instances on the page
///   and displays the validation state.
/// </summary>
[ToolboxData("<{0}:ValidationStateViewer runat='server'></{0}:ValidationStateViewer>")]
[ToolboxItemFilter("System.Web.UI")]
public class ValidationStateViewer : WebControl, IControl
{
  // types

  /// <summary> A list of validation state viewer wide resources. </summary>
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources("Remotion.Web.Globalization.ValidationStateViewer")]
  public enum ResourceIdentifier
  {
    /// <summary>The summary message displayed if validation errors where found. </summary>
    NoticeText
  }

  // constants

  /// <summary> CSS-Class applied to the individual validation messages. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  private const string c_cssClassValidationMessage = "formGridValidationMessage";

  /// <summary> CSS-Class applied to the validation notice. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  private const string c_cssClassValidationNotice = "formGridValidationNotice";

  // static members

  // member fields

  /// <summary> Collection of <see cref="FormGridManager" /> instances in the page. </summary>
  private ArrayList? _formGridManagers;

  /// <summary>
  ///   The Text displayed if <see cref="ValidationStateViewer.ValidationErrorStyle"/> is set to 
  ///   <see cref="Remotion.Web.UI.Controls.ValidationErrorStyle.Notice"/>.
  /// </summary>
  private WebString _noticeText;
  /// <summary> The style in which the validation errors should be displayed on the page. </summary>
  private ValidationErrorStyle _validationErrorStyle = ValidationErrorStyle.Notice;
  private bool _showLabels = true;
  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this <see cref="ValidationStateViewer"/>. </summary>
  private ResourceManagerSet? _cachedResourceManager;

  // construction and disposing

  /// <summary> Initializes a new instance of the <see cref="ValidationStateViewer"/> class. </summary>
  public ValidationStateViewer ()
  {
  }

  /// <summary> Registers all instances of <see cref="FormGridManager"/>. </summary>
  /// <param name="parent"> Parent element of the FormGridManager objects. </param>
  private void PopulateFormGridManagerList (Control parent)
  {
    ArgumentUtility.CheckNotNull("parent", parent);

    //  Add all FormGridManager instances
    for (int i = 0; i < parent.Controls.Count; i++)
    {
      Control childControl = (Control)parent.Controls[i];
      FormGridManager? formGridManager = childControl as FormGridManager;

      if (formGridManager != null)
        _formGridManagers!.Add(formGridManager); // TODO RM-8118: Debug not null assertion

      bool isChildNamingContainer = childControl is INamingContainer;
      PopulateFormGridManagerList(childControl);
    }
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    IResourceManager resourceManager = GetResourceManager();
    LoadResources(resourceManager);
  }

  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull("resourceManager", resourceManager);

    string? key = ResourceManagerUtility.GetGlobalResourceKey(NoticeText.GetValue());
    if (!string.IsNullOrEmpty(key))
      NoticeText = resourceManager.GetWebString(key, NoticeText.Type);
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    switch (_validationErrorStyle)
    {
      case ValidationErrorStyle.Notice:
      {
        RenderValidationNotice(writer);
        break;
      }
      case ValidationErrorStyle.DetailedMessages:
      {
        RenderValidationMessages(writer);
        break;
      }
      case ValidationErrorStyle.HideErrors:
      {
        //  Do nothing
        break;
      }
      default:
      {
        //  Do nothing
        break;
      }
    }
  }

  /// <summary> Displays a short notice if validation errors where found. </summary>
  protected virtual void RenderValidationNotice (HtmlTextWriter writer)
  {
    bool isPageValid = true;
    for (int i = 0; i < Page!.Validators.Count; i++)
    {
      IValidator validator = (IValidator)Page.Validators[i];
      if (! validator.IsValid)
      {
        isPageValid = false;
        break;
      }
    }

    //  Enclose the validation error notice inside a div
    if (! isPageValid)
    {
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassValidationNotice);
      writer.RenderBeginTag(HtmlTextWriterTag.Div);

      WebString noticeText;
      if (_noticeText.IsEmpty)
      {
        IResourceManager resourceManager = GetResourceManager();
        noticeText = resourceManager.GetText(ResourceIdentifier.NoticeText);
      }
      else
        noticeText = _noticeText;

      noticeText.WriteTo(writer);
      writer.RenderEndTag();
    }
  }

  /// <summary> Displays the validation messages for each error. </summary>
  protected virtual void RenderValidationMessages (HtmlTextWriter writer)
  {
    _formGridManagers = new ArrayList();
    PopulateFormGridManagerList(NamingContainer);

    writer.AddStyleAttribute("border-spacing", "0");
    writer.AddStyleAttribute("border-collapse", "collapse");
    writer.AddStyleAttribute("border", "none");
    writer.RenderBeginTag(HtmlTextWriterTag.Table);
    for (int idxFormGridManagers = 0; idxFormGridManagers < _formGridManagers.Count; idxFormGridManagers++)
    {
      FormGridManager formGridManager = (FormGridManager)_formGridManagers[idxFormGridManagers]!; // TODO RM-8118: not null assertion
      ValidationError[] validationErrors = formGridManager.GetValidationErrors();
      //  Get validation messages
      for (int idxErrors = 0; idxErrors < validationErrors.Length; idxErrors++)
      {
        ValidationError validationError = (ValidationError)validationErrors[idxErrors];
        if (validationError == null)
          continue;

        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

        if (validationError.Labels  != null)
        {
          writer.AddStyleAttribute("padding-right", "0.3em");
          writer.RenderBeginTag(HtmlTextWriterTag.Td);
          for (int idxErrorLabels = 0; idxErrorLabels < validationError.Labels.Count; idxErrorLabels++)
          {
            var control = validationError.Labels[idxErrorLabels];
            var text = control switch
            {
                SmartLabel label => label.GetText(),
                FormGridLabel label => label.Text,
                Label label => WebString.CreateFromHtml(label.Text),
                LiteralControl literalControl => WebString.CreateFromHtml(literalControl.Text),
                _ => WebString.Empty
            };

            text.WriteTo(writer);
          }
          writer.RenderEndTag();
        }
        else
        {
          writer.RenderBeginTag(HtmlTextWriterTag.Td);
          writer.RenderEndTag();
        }

        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        validationError.ToHyperLink(CssClassValidationMessage).RenderControl(writer);
        writer.RenderEndTag();

        writer.RenderEndTag();
      }
    }
    writer.RenderEndTag();
  }

  /// <summary>
  ///   Find the <see cref="IResourceManager"/> for this <see cref="ValidationStateViewer"/>.
  /// </summary>
  /// <returns></returns>
  protected IResourceManager GetResourceManager ()
  {
    //  Provider has already been identified.
    if (_cachedResourceManager != null)
      return _cachedResourceManager;

    //  Get the resource managers

    IResourceManager localResourceManager = GlobalizationService.GetResourceManager(typeof(ResourceIdentifier));
    IResourceManager namingContainerResourceManager = ResourceManagerUtility.GetResourceManager(NamingContainer, true);
    _cachedResourceManager = ResourceManagerSet.Create(namingContainerResourceManager, localResourceManager);

    return _cachedResourceManager;
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public IGlobalizationService GlobalizationService
  {
    get
    {
      return SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
    }
  }

  /// <summary>
  ///   The Text displayed if <see cref="ValidationStateViewer.ValidationErrorStyle"/> is set to 
  ///   <see cref="Remotion.Web.UI.Controls.ValidationErrorStyle.Notice"/>
  /// </summary>
  /// <value> A string. </value>
  [Category("Appearance")]
  [Description("Sets the Text to be displayed if ValidationErrorStyle is set to Notice.")]
  [DefaultValue(typeof(WebString), "")]
  public WebString NoticeText
  {
    get { return _noticeText; }
    set { _noticeText = value; }
  }

  /// <summary> Gets or sets a value that defines how the validation errors are displayed on the page. </summary>
  /// <value> A symbol defined in the <see cref="ValidationErrorStyle"/>enumeration. </value>
  [Category("Behavior")]
  [Description("Defines how the validation messages are displayed.")]
  [DefaultValue(ValidationErrorStyle.Notice)]
  public ValidationErrorStyle ValidationErrorStyle
  {
    get { return _validationErrorStyle; }
    set { _validationErrorStyle = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to render the label associated with the erroneous control 
  ///   in front of the error message.
  /// </summary>
  /// <value> <see langword="true"/> to render the label. </value>
  [Category("Apearance")]
  [Description("true to render the label associated with the erroneous control in front of the error message.")]
  [DefaultValue(true)]
  public bool ShowLabels
  {
    get { return _showLabels; }
    set { _showLabels = value; }
  }

  IPage? IControl.Page
  {
    get { return PageWrapper.CastOrCreate(base.Page); }
  }

  /// <summary> CSS-Class applied to the individual validation messages. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  protected virtual string CssClassValidationMessage
  { get { return c_cssClassValidationMessage;} }

  /// <summary> CSS-Class applied to the validation notice. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  protected virtual string CssClassValidationNotice
  { get { return c_cssClassValidationNotice;} }
}


/// <summary> A list of possible ways to displau the validation messages. </summary>
public enum ValidationErrorStyle
{
  /// <summary> Display no messages. </summary>
  HideErrors,
  /// <summary> Display a short notice if validation errors where found. </summary>
  Notice,
  /// <summary> Display the individual validation messages provided by the FormGridManager. </summary>
  DetailedMessages
}

}
