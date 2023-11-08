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
using Remotion.Utilities;
using Remotion.Web.UI.Controls.ListMenuImplementation;
using Remotion.Web.UI.Controls.ListMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UI.Controls
{
  public class ListMenu : MenuBase, IListMenu
  {
    private ListMenuLineBreaks _lineBreaks = ListMenuLineBreaks.All;

    public ListMenu (IControl? ownerControl, Type[] supportedWebMenuItemTypes)
        : base(ownerControl, supportedWebMenuItemTypes)
    {
      EnableClientScript = true;
    }

    public ListMenu (IControl? ownerControl)
        : this(ownerControl, new[] { typeof(WebMenuItem) })
    {
    }

    public ListMenu ()
        : this(null)
    {
    }

    /// <inheritdoc />
    public WebString Heading { get; set; }

    /// <inheritdoc />
    public HeadingLevel? HeadingLevel { get; set; }

    public ListMenuLineBreaks LineBreaks
    {
      get { return _lineBreaks; }
      set { _lineBreaks = value; }
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      RegisterHtmlHeadContents(HtmlHeadAppender.Current);
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    protected virtual IListMenuRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IListMenuRenderer>();
    }

    protected virtual ListMenuRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      return new ListMenuRenderingContext(Page!.Context!, writer, this); // TODO RM-8118: not null assertion
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);

      for (int i = 0; i < MenuItems.Count; i++)
      {
        WebMenuItem menuItem = MenuItems[i];
        if (menuItem.Command != null)
        {
          menuItem.Command.RegisterForSynchronousPostBackOnDemand(
              this, i.ToString(), string.Format("ListMenu '{0}', MenuItem '{1}'", ID, menuItem.ItemID));
        }
      }
    }

    public bool EnableClientScript { get; set; }

    public bool HasClientScript
    {
      get { return EnableClientScript; }
    }

    /// <summary>
    /// Gets the Javascript that can be used to update the <see cref="MenuBase.MenuItems"/> of this <see cref="ListMenu"/>.
    /// </summary>
    /// <param name="getSelectionCount">
    ///   A reference to a Javascript function that returns the current selection count or the Javascript <c>null</c> value if there is no selection count. 
    ///   Must not be <see langword="null" /> or empty.
    /// </param>
    /// <returns>A Javascript statement, terminiated with a <c>;</c> (semicolon).</returns>
    public string GetUpdateScriptReference (string getSelectionCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty("getSelectionCount", getSelectionCount);

      return string.Format("ListMenu.Update ('#{0}', {1});", ClientID, getSelectionCount);
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "ListMenu"; }
    }
  }
}
