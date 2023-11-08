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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;

namespace Remotion.Web.UI.Controls
{

  public abstract class MenuTab : WebTab, IMenuTab
  {
    private SingleControlItemCollection _command;
    private MissingPermissionBehavior _missingPermissionBehavior;

    protected MenuTab (string itemID, WebString text, IconInfo? icon)
      : base(itemID, text, icon)
    {
      Initialize();
    }

    protected MenuTab ()
    {
      Initialize();
    }

    [MemberNotNull(nameof(_command))]
    private void Initialize ()
    {
      _command = new SingleControlItemCollection(new NavigationCommand(), new[] { typeof(NavigationCommand) });
    }

    public override IWebTabRenderer GetRenderer ()
    {
      return (IWebTabRenderer)SafeServiceLocator.Current.GetInstance<IMenuTabRenderer>();
    }

    protected TabbedMenu? TabbedMenu
    {
      get { return (TabbedMenu?)OwnerControl; }
    }

    public NameValueCollection GetUrlParameters ()
    {
      return TabbedMenu!.GetUrlParameters(this); // TODO RM-8118: not null assertion
    }

    /// <summary> Gets or sets the <see cref="NavigationCommand"/> rendered for this menu item. </summary>
    /// <value> A <see cref="NavigationCommand"/>. </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category("Behavior")]
    [Description("The command rendered for this menu item.")]
    [NotifyParentProperty(true)]
    public virtual NavigationCommand? Command
    {
      get { return (NavigationCommand?)_command.ControlItem; }
      set { _command.ControlItem = value; }
    }

    protected bool ShouldSerializeCommand ()
    {
      if (Command == null)
        return false;

      if (Command.IsDefaultType)
        return false;
      else
        return true;
    }

    /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
    /// <remarks> 
    ///   The default value is a <see cref="Command"/> object with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    protected void ResetCommand ()
    {
      if (Command != null)
      {
        Command = (NavigationCommand)Activator.CreateInstance(Command.GetType())!;
        Command.Type = CommandType.None;
      }
    }

    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Browsable(false)]
    public SingleControlItemCollection PersistedCommand
    {
      get { return _command; }
    }

    /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
    /// <remarks> 
    ///   Does not persist <see cref="Command"/> objects with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    protected bool ShouldSerializePersistedCommand ()
    {
      return ShouldSerializeCommand();
    }

    protected override void OnOwnerControlChanged ()
    {
      base.OnOwnerControlChanged();

      if (OwnerControl != null && !(OwnerControl is TabbedMenu))
        throw new InvalidOperationException("A SubMenuTab can only be added to a WebTabStrip that is part of a TabbedMenu.");

        _command.OwnerControl = OwnerControl;
    }

    public override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      base.LoadResources(resourceManager, globalizationService);
      if (Command != null)
        Command.LoadResources(resourceManager, globalizationService);
    }

    protected virtual MenuTab GetActiveTab ()
    {
      return this;
    }

    IMenuTab IMenuTab.GetActiveTab ()
    {
      return GetActiveTab();
    }

    public override void OnClick ()
    {
      base.OnClick();
      if (!IsSelected)
        IsSelected = true;
    }

    public override bool EvaluateVisible ()
    {
      if (!base.EvaluateVisible())
        return false;

      if (Command != null)
      {
        if (MissingPermissionBehavior == MissingPermissionBehavior.Invisible)
          return Command.HasAccess(null);
      }

      return true;
    }

    public override bool EvaluateEnabled ()
    {
      if (!base.EvaluateEnabled())
        return false;

      if (Command != null)
      {
        if (MissingPermissionBehavior == MissingPermissionBehavior.Disabled)
          return Command.HasAccess(null);
      }
      return true;
    }

    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [NotifyParentProperty(true)]
    [DefaultValue(MissingPermissionBehavior.Invisible)]
    public MissingPermissionBehavior MissingPermissionBehavior
    {
      get { return _missingPermissionBehavior; }
      set { _missingPermissionBehavior = value; }
    }
  }
}
