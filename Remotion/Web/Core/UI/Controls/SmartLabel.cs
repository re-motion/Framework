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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

[ToolboxItemFilter("System.Web.UI")]
public class SmartLabel: WebControl, IControl
{
  private string? _forControl = null;
  private string? _parentLabel;
  private WebString _text;

  public SmartLabel ()
  {
  }

  /// <summary>
  ///   The ID of the control to display a label for.
  /// </summary>
  [Category("Behavior")]
  public string? ForControl
  {
    get { return _forControl; }
    set { _forControl = value; }
  }

  /// <summary>
  /// The ID of the parent <see cref="SmartLabel"/> used when creating hierarchical labels.
  /// The parent <see cref="SmartLabel"/> must be located within the same <see cref="Control.NamingContainer"/>.
  /// </summary>
  [Category("Behavior")]
  public string? ParentLabel
  {
    get { return _parentLabel; }
    set { _parentLabel = value; }
  }

  /// <summary>
  ///   Gets or sets the text displayed if the <see cref="SmartLabel"/> is not bound to an 
  ///   <see cref="ISmartControl "/> or the <see cref="ISmartControl"/> does provide a 
  ///   <see cref="ISmartControl.DisplayName"/>.
  /// </summary>
  [Category("Appearance")]
  [Description("The text displayed if the SmartLabel is not bound to an ISmartControl or the ISmartControl does provide a DisplayName.")]
  [DefaultValue(null)]
  public WebString Text
  {
    get { return _text; }
    set { _text = value; }
  }

  protected override HtmlTextWriterTag TagKey
  {
    get
    {
      var clientID = GetClientIDForTarget();

      if (string.IsNullOrEmpty(clientID))
        return HtmlTextWriterTag.Span;
      return HtmlTextWriterTag.Label;
    }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    var resourceManager = ResourceManagerUtility.GetResourceManager(this, true);
    LoadResources(resourceManager);

    if (!string.IsNullOrEmpty(ForControl))
    {
      var smartControl = NamingContainer.FindControl(ForControl) as ISmartControl;
      if (smartControl != null)
      {
        var labels = this.CreateSequenceWithCycleCheck(
            GetParentLabel,
            label => new HttpException(string.Format("SmartLabel '{0}' forms a cyclic reference via it's ParentLabel property.", label.ParentLabel)));

        var labelIDs = labels.Select(label => label.ClientID).Reverse();
        smartControl.AssignLabels(labelIDs);
      }
    }

    static SmartLabel? GetParentLabel (SmartLabel label)
    {
      if (label.ParentLabel == null)
        return null;

      var control = label.NamingContainer.FindControl(label.ParentLabel);
      return control switch
      {
          SmartLabel smartLabel => smartLabel,
          null => throw new HttpException(string.Format("Unable to find the control with id '{0}' referenced as ParentLabel for the label '{1}'.", label.ParentLabel, label.ID)),
          _ => throw new HttpException(string.Format("Control with id '{0}' referenced as ParentLabel for the label '{1}' is not a SmartLabel.", label.ParentLabel, label.ID))
      };
    }
  }

  protected override void Render (HtmlTextWriter writer)
  {
    RenderBeginTag(writer);
    var text = GetText();
    // Do not HTML encode
    text.WriteTo(writer);
    RenderEndTag(writer);
  }

  public WebString GetText ()
  {
    if (!_text.IsEmpty)
      return _text;

    string? forControlBackUp = ForControl;
    ForControl = ForControl ?? string.Empty;
    WebString text;

    if (ForControl == string.Empty)
    {
      text = WebString.CreateFromText("[Label]");
    }
    else
    {
      ISmartControl? smartControl = NamingContainer.FindControl(ForControl) as ISmartControl;
      if (smartControl != null && !smartControl.DisplayName.IsEmpty)
        text = smartControl.DisplayName;
      else
        text = WebString.CreateFromText("[Label for " + ForControl + "]");
    }
    ForControl = forControlBackUp;
    return text;
  }

  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    base.AddAttributesToRender(writer);

    var clientID = GetClientIDForTarget();

    if (!string.IsNullOrEmpty(clientID))
      writer.AddAttribute(HtmlTextWriterAttribute.For, clientID);

    // TODO: add <a href="ToName(target.ClientID)"> ...
    // ToName: '.' -> '_'
  }

  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull("resourceManager", resourceManager);

    string? key = ResourceManagerUtility.GetGlobalResourceKey(Text.GetValue());
    if (!string.IsNullOrEmpty(key))
      Text = resourceManager.GetWebString(key, Text.Type);
  }

  private string? GetClientIDForTarget ()
  {
    Control? target = ControlHelper.FindControl(NamingContainer, ForControl);

    return target switch
    {
      ISmartControl { UseLabel: false } => null,
      IFocusableControl focusableControl => focusableControl.FocusID,
      null => null,
      _ => target.ClientID
    };
  }

  public new IPage? Page
  {
    get { return PageWrapper.CastOrCreate(base.Page); }
  }
}
}
