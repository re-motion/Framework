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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Calendar = System.Web.UI.WebControls.Calendar;

namespace Remotion.Web.UI.Controls
{

/// <summary> 
///   Displayes a <see cref="Calendar"/> and updates an input control with the newly selected date.
/// </summary>
/// <remarks>
///   <para> 
///     The form is designed to be displayed inside an IFrame.
///   </para>
///   <para> 
///     The inherited aspx form must contain a control derived from <see cref="Calendar"/>
///     named <c>Calendar</c>.
///   </para>
///   <para> 
///     The inherited aspx form's form instance must named <c>Form</c>.
///   </para>
///   <para>
///     Open the date picker using the <c>ShowDatePicker</c> function located in <c>DatePicker.js</c>.
///   </para>
///   <para>
///     Popup does not work on FireFox, Internet Explorer 5.1 and below
///   </para>
/// </remarks>
public class DatePickerPage : Page
{
  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources("Remotion.Web.Globalization.DatePickerPage")]
  public enum ResourceIdentifier
  {
    /// <summary>The tool tip text for the calendar popup.</summary>
    TitleText,

    /// <summary>The tool tip text for the select the previous month button.</summary>
    PreviousMonthTitle,

    /// <summary>The tool tip text for the select the next month button.</summary>
    NextMonthTitle,
  }

  public const string CultureParameterName = "Culture";
  public const string UICultureParameterName = "UICulture";

  protected HtmlHeadContents HtmlHeadContents = null!;
  protected Calendar Calendar = null!;
  /// <summary> Preserves the target control's ID during post backs. </summary>
  private HtmlInputHidden TargetIDField = null!;
  /// <summary> Preserves the frame's ID in the parent page during post backs. </summary>
  private HtmlInputHidden DatePickerIDField = null!;
  /// <summary> Contains the date to be selected in the calendar. </summary>
  private HtmlInputHidden DateValueField = null!;

  protected override void OnPreInit (EventArgs e)
  {
    base.OnPreInit(e);

    var cultureName = Request.QueryString[CultureParameterName];
    if (!string.IsNullOrWhiteSpace(cultureName))
      Culture = cultureName.Trim();

    var uiCultureName = Request.QueryString[UICultureParameterName];
    if (!string.IsNullOrWhiteSpace(uiCultureName))
      UICulture = uiCultureName.Trim();
  }

  override protected void OnInit (EventArgs e)
  {
    Assertion.DebugIsNotNull(Page, "Page must not be null.");

    if (Form == null)
      throw new HttpException(this.GetType().GetFullNameSafe() + " does not initialize field 'Form'.");
    if (HtmlHeadContents == null)
      throw new HttpException(this.GetType().GetFullNameSafe() + " does not initialize field 'HtmlHeadContents'.");
    if (Calendar == null)
      throw new HttpException(this.GetType().GetFullNameSafe() + " does not initialize field 'Calendar'.");

    Calendar.SelectionChanged += new EventHandler(Calendar_SelectionChanged);

    TargetIDField = new HtmlInputHidden();
    TargetIDField.ID = "TargetIDField";
    TargetIDField.EnableViewState = false;
    Form.Controls.Add(TargetIDField);

    DatePickerIDField = new HtmlInputHidden();
    DatePickerIDField.ID = "DatePickerIDField";
    DatePickerIDField.EnableViewState = false;
    Form.Controls.Add(DatePickerIDField);

    DateValueField = new HtmlInputHidden();
    DateValueField.ID = "DateValueField";
    DateValueField.EnableViewState = false;
    Form.Controls.Add(DateValueField);

    //  Force the creation of the postback function
    Page.ClientScript.GetPostBackEventReference(this, "");

#if !NETFRAMEWORK
    var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
    var resourceManager = globalizationService.GetResourceManager(typeof(ResourceIdentifier));
    Calendar.TitleText = resourceManager.GetString(ResourceIdentifier.TitleText);
    Calendar.PreviousMonthTitle = resourceManager.GetString(ResourceIdentifier.PreviousMonthTitle);
    Calendar.NextMonthTitle = resourceManager.GetString(ResourceIdentifier.NextMonthTitle);
#endif

    base.OnInit(e);

    RegisterHtmlHeadContents(HtmlHeadAppender.Current);
	}

  private void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
  {
    var renderer = SafeServiceLocator.Current.GetInstance<IDatePickerPageRenderer>();
    renderer.RegisterHtmlHeadContents(htmlHeadAppender);
  }

  protected override void OnLoad (EventArgs e)
  {
    string dateValue;
    if (IsPostBack)
    {
      dateValue = DateValueField.Value;
    }
    else
    {
      dateValue = Request.Params["DateValueField"]!; // TODO RM-8118: not null assertion
      TargetIDField.Value = Request.Params["TargetIDField"]!; // TODO RM-8118: not null assertion
      DatePickerIDField.Value = Request.Params["DatePickerIDField"]!; // TODO RM-8118: not null assertion
    }

    //  Initalize the calendar
    try
    {
      if (! string.IsNullOrEmpty(dateValue))
      {
        Calendar.SelectedDate = DateTime.Parse(dateValue);
        Calendar.VisibleDate = Calendar.SelectedDate;
      }
    }
    catch (FormatException)
    {
      //  Do nothing since user wishes to pick a valid date using the calendar
    }
    DateValueField.Value = string.Empty;

    base.OnLoad(e);
  }

  private void Calendar_SelectionChanged (object? sender, EventArgs e)
  {
    Assertion.DebugIsNotNull(Page, "Page must not be null.");

    string key = "Calendar_SelectionChanged";
    string script = "DatePickerFrame.Calendar_SelectionChanged ('" + Calendar.SelectedDate.ToShortDateString() + "');\r\n";
    if (!Page.ClientScript.IsStartupScriptRegistered(key))
      Page.ClientScript.RegisterStartupScript(typeof(DatePickerPage), key, script, true);
  }
}

}
