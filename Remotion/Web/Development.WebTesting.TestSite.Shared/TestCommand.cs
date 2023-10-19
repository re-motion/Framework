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
using System.Web.UI;
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared
{
  public class TestCommand : Control
  {
    private readonly IRenderingFeatures _renderingFeatures;

    public string Text { get; set; }
    public CommandType CommandType { get; set; }
    public Command.EventCommandInfo EventCommandInfo { get; set; }
    public Command.HrefCommandInfo HrefCommandInfo { get; set; }
    public string ItemID { get; set; }

    public TestCommand ()
    {
      _renderingFeatures = SafeServiceLocator.Current.GetInstance<IRenderingFeatures>();
      EventCommandInfo = new Command.EventCommandInfo();
      HrefCommandInfo = new Command.HrefCommandInfo();
    }

    protected override void Render (HtmlTextWriter writer)
    {
      base.Render(writer);

      var command = new Command(CommandType)
                    {
                        EventCommand = EventCommandInfo,
                        HrefCommand = HrefCommandInfo,
                        ItemID = ItemID
                    };

      writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
      writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "underline");
      var postBackEvent = Page.ClientScript.GetPostBackEventReference(this, "additional") + ";";
      command.RenderBegin(writer, _renderingFeatures, postBackEvent, new string[0], null, null);

      writer.Write(Text);

      command.RenderEnd(writer);
    }
  }
}
