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
using System.Web.UI.WebControls;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A <c>LinkButton</c> using <c>&amp;</c> as access key prefix in <see cref="LinkButton.Text"/>. </summary>
  /// <include file='..\..\doc\include\UI\Controls\WebLinkButton.xml' path='WebLinkButton/Class/*' />
  [ToolboxData ("<{0}:WebLinkButton runat=server></{0}:WebLinkButton>")]
  [ToolboxItem (false)]
  public class WebLinkButton : LinkButton, IControl
  {
    private const string c_textViewStateKey = nameof (Text);
    private const string c_textWebStringViewStateKey = c_textViewStateKey + "_" + nameof (WebStringType);

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      if (string.IsNullOrEmpty (AccessKey))
        writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, AccessKey);

      base.AddAttributesToRender (writer);
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);

      if (HasControls())
        base.RenderContents (writer);
      else
        Text.Write(writer);
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }

    [Category("Appearance")]
    [Description("WebLinkButton_Text")]
    [DefaultValue("")]
    public new WebString Text
    {
      get
      {
        var value = (string?) ViewState[c_textViewStateKey];
        var type = (WebStringType?) ViewState[c_textWebStringViewStateKey] ?? WebStringType.PlainText;

        return type switch
        {
            WebStringType.PlainText => WebString.CreateFromText (value),
            WebStringType.Encoded => WebString.CreateFromHtml (value),
            _ => throw new InvalidOperationException (
                $"The value for key '{c_textWebStringViewStateKey}' in the ViewState contains invalid data '{type}'."),
        };
      }
      set
      {
        ViewState[nameof (Text)] = value.GetValue();
        ViewState[c_textWebStringViewStateKey] = value.Type;
      }
    }
  }
}