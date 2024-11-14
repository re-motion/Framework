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
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Globalization;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Can be used instead of <see cref="SmartLabel"/> controls 
///   (to label controls that do not implement ISmartControl).
/// </summary>
[ToolboxItemFilter("System.Web.UI")]
public class FormGridLabel: Label, ISmartControl
{
  private const string c_textViewStateKey = nameof(Text);
  private const string c_textWebStringViewStateKey = c_textViewStateKey + "_" + nameof(WebStringType);

  private bool _required = false;
  private string? _helpUrl = null;

  [Category("Appearance")]
  [Description("The text to be shown for the Label.")]
  [DefaultValue(typeof(WebString), "")]
  public new WebString Text
  {
    get
    {
      var value = (string?)ViewState[c_textViewStateKey];
      var type = (WebStringType?)ViewState[c_textWebStringViewStateKey] ?? WebStringType.PlainText;

      return type switch
      {
          WebStringType.PlainText => WebString.CreateFromText(value),
          WebStringType.Encoded => WebString.CreateFromHtml(value),
          _ => throw new InvalidOperationException(
              $"The value for key '{c_textWebStringViewStateKey}' in the ViewState contains invalid data '{type}'."),
      };
    }
    set
    {
      ViewState[c_textViewStateKey] = value.GetValue();
      ViewState[c_textWebStringViewStateKey] = value.Type;
    }
  }

  [Category("Behavior")]
  [DefaultValue(false)]
  [Description("Specifies whether this row will be marked as 'required' in FormGrids.")]
  public bool Required
  {
    get { return _required; }
    set { _required = value; }
  }

  [Category("Behavior")]
  [DefaultValue(null)]
  [Description("Specifies the relative URL to the row's help text.")]
  public string? HelpUrl
  {
    get { return _helpUrl; }
    set { _helpUrl = value ?? string.Empty; }
  }

  [Browsable(false)]
  public bool IsRequired
  {
    get { return _required; }
  }

  HelpInfo? ISmartControl.HelpInfo
  {
    get { return (_helpUrl != null) ? new HelpInfo(_helpUrl) : null; }
  }

  IEnumerable<BaseValidator> ISmartControl.CreateValidators ()
  {
    return Enumerable.Empty<BaseValidator>();
  }

  bool ISmartControl.UseLabel
  {
    get { return true; }
  }

  void ISmartControl.AssignLabels (IEnumerable<string> labelIDs)
  {
  }

  WebString ISmartControl.DisplayName
  {
    get { return Text; }
  }

  IPage? IControl.Page
  {
    get { return PageWrapper.CastOrCreate(base.Page); }
  }

  protected override HtmlTextWriterTag TagKey
  {
    get
    {
      if (string.IsNullOrEmpty(AssociatedControlID))
        return HtmlTextWriterTag.Span;
      return HtmlTextWriterTag.Label;
    }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    var resourceManager = ResourceManagerUtility.GetResourceManager(this, true);
    LoadResources(resourceManager);
  }

  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull("resourceManager", resourceManager);

    string? key = ResourceManagerUtility.GetGlobalResourceKey(Text.GetValue());
    if (!string.IsNullOrEmpty(key))
      Text = resourceManager.GetWebString(key, Text.Type);
  }

  void ISmartControl.RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
  {
  }

  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    string associatedControlID = AssociatedControlID;
    if (associatedControlID.Length != 0)
    {
      Control? control = this.FindControl(associatedControlID);
      if (control == null)
        throw new HttpException(string.Format("Unable to find the control with id '{0}' that is associated with the Label '{1}'.", associatedControlID, ID));
      writer.AddAttribute("for", control.ClientID);
    }
    AssociatedControlID = string.Empty;
    base.AddAttributesToRender(writer);
    AssociatedControlID = associatedControlID;
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    if (HasControls())
      base.RenderContents(writer);
    else
      Text.WriteTo(writer);
  }
}

}
