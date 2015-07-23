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
using Microsoft.Practices.ServiceLocation;
using Remotion.ServiceLocation;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Hotkey;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A <c>LinkButton</c> using <c>&amp;</c> as access key prefix in <see cref="LinkButton.Text"/>. </summary>
  /// <include file='..\..\doc\include\UI\Controls\WebLinkButton.xml' path='WebLinkButton/Class/*' />
  [ToolboxData ("<{0}:WebLinkButton runat=server></{0}:WebLinkButton>")]
  [ToolboxItem (false)]
  public class WebLinkButton : LinkButton, IControl
  {
    private TextWithHotkey _textWithHotkey;

    protected override void Render (HtmlTextWriter writer)
    {
      _textWithHotkey = HotkeyParser.Parse (Text);

      base.Render (writer);
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      if (string.IsNullOrEmpty (AccessKey) && _textWithHotkey.Hotkey.HasValue)
        writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, HotkeyFormatter.FormatHotkey (_textWithHotkey));

      base.AddAttributesToRender (writer);
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);

      if (HasControls())
        base.RenderContents (writer);
      else
        writer.Write (HotkeyFormatter.FormatText (_textWithHotkey, false));
    }

    public new IPage Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }

    protected virtual IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    private IHotkeyFormatter HotkeyFormatter
    {
      get { return ServiceLocator.GetInstance<IHotkeyFormatter>(); }
    }
  }
}