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
using System.Collections.Specialized;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;

namespace Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="MenuTab"/> in quirks mode.
  /// <seealso cref="IMenuTab"/>
  /// </summary>
  [ImplementationFor(typeof(IMenuTabRenderer), Lifetime = LifetimeKind.Singleton)]
  public class MenuTabRenderer : WebTabRenderer, IMenuTabRenderer
  {
    public MenuTabRenderer (IHotkeyFormatter hotkeyFormatter, IRenderingFeatures renderingFeatures)
        : base(hotkeyFormatter, renderingFeatures)
    {
    }

    protected override Command RenderBeginTagForCommand (WebTabStripRenderingContext renderingContext, IWebTab tab, bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull("style", style);

      var menuTab = ((IMenuTab)tab).GetActiveTab();
      var command = GetRenderingCommand(isEnabled, menuTab);

      var additionalUrlParameters = menuTab.GetUrlParameters();
      var backupID = command.ItemID;
      var backupAccessKey = command.AccessKey;

      try
      {
        if (string.IsNullOrEmpty(command.ItemID) && !string.IsNullOrEmpty(tab.ItemID))
          command.ItemID = tab.ItemID + "_Command";

        if (string.IsNullOrEmpty(command.AccessKey))
        {
          var accessKey = HotkeyFormatter.GetAccessKey(tab.Text);
          if (accessKey.HasValue)
            command.AccessKey = accessKey.Value.ToString();
        }

        command.RenderBegin(
            renderingContext.Writer,
            RenderingFeatures,
            tab.GetPostBackClientEvent(),
            new string[0],
            string.Empty,
            null,
            additionalUrlParameters,
            false,
            style,
            new NameValueCollection(0));

        return command;
      }
      finally
      {
        command.ItemID = backupID;
        command.AccessKey = backupAccessKey;
      }
    }

    private Command GetRenderingCommand (bool isEnabled, IMenuTab activeTab)
    {
      if (isEnabled && activeTab.EvaluateEnabled())
        return activeTab.Command!; // TODO RM-8118: not null assertion

      return new Command(CommandType.None) { OwnerControl = activeTab.OwnerControl };
    }
  }
}
