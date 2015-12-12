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
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Design;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

[ToolboxItemFilter("System.Web.UI")]
public class SmartLabel: WebControl, IControl
{
  private string _forControl = null;
  private string _text = null;

  public SmartLabel()
    : base (HtmlTextWriterTag.Label)
  {
  }

  /// <summary>
  ///   The ID of the control to display a label for.
  /// </summary>
  [TypeConverter (typeof (SmartControlToStringConverter))]
  [Category ("Behavior")]
  public string ForControl
  {
    get { return _forControl; }
    set { _forControl = value; }
  }

  /// <summary>
  ///   Gets or sets the text displayed if the <see cref="SmartLabel"/> is not bound to an 
  ///   <see cref="ISmartControl "/> or the <see cref="ISmartControl"/> does provide a 
  ///   <see cref="ISmartControl.DisplayName"/>.
  /// </summary>
  [Category ("Appearance")]
  [Description ("The text displayed if the SmartLabel is not bound to an ISmartControl or the ISmartControl does provide a DisplayName.")]
  [DefaultValue (null)]
  public string Text
  {
    get { return _text; }
    set { _text = value; }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    var resourceManager = ResourceManagerUtility.GetResourceManager (this, true);
    LoadResources (resourceManager);
  }

  protected override void Render(HtmlTextWriter writer)
  {
    this.RenderBeginTag (writer);
    string text = GetText();
    // Do not HTML encode
    writer.Write (text);
    this.RenderEndTag (writer);
  }

  public string GetText()
  {
    if (! string.IsNullOrEmpty (_text))
      return _text;

    string forControlBackUp = ForControl;
    ForControl = ForControl ?? string.Empty;
    string text = string.Empty;

    if (ForControl == string.Empty)
    {
      text = "[Label]";
    }
    else
    {
      ISmartControl smartControl = NamingContainer.FindControl (ForControl) as ISmartControl;
      if (smartControl != null && smartControl.DisplayName != null)
        text = smartControl.DisplayName;
      else
        text = "[Label for " + ForControl + "]";
    }
    ForControl = forControlBackUp;
    return text;
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);

    if (! ControlHelper.IsDesignMode (this))
    {
      Control target = ControlHelper.FindControl (NamingContainer, ForControl);
      bool useLabel = false;
      string clientID = null;
      if (target is ISmartControl && target is IFocusableControl)
      {
        clientID = ((IFocusableControl)target).FocusID;
        useLabel = ((ISmartControl)target).UseLabel;
      }
      else if (target != null)
      {
        clientID = target.ClientID;
        useLabel = ! (target is DropDownList || target is HtmlSelect);
      }

      if (useLabel && !string.IsNullOrEmpty (clientID))
        writer.AddAttribute (HtmlTextWriterAttribute.For, clientID);

      // TODO: add <a href="ToName(target.ClientID)"> ...
      // ToName: '.' -> '_'
    }
  }

  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

    if (ControlHelper.IsDesignMode (this))
      return;

    string key = ResourceManagerUtility.GetGlobalResourceKey (Text);
    if (!string.IsNullOrEmpty (key))
      Text = resourceManager.GetString (key);
  }

  public new IPage Page
  {
    get { return PageWrapper.CastOrCreate (base.Page); }
  }
}
}
