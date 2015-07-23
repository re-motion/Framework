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
using Microsoft.Practices.ServiceLocation;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition that renders a <see cref="DropDownMenu"/> in the cell. </summary>
  public class BocDropDownMenuColumnDefinition : BocColumnDefinition
  {
    private string _menuTitleText = string.Empty;
    private IconInfo _menuTitleIcon;

    public BocDropDownMenuColumnDefinition ()
    {
      _menuTitleIcon = new IconInfo();
    }

    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);
      
      return serviceLocator.GetInstance<IBocDropDownMenuColumnRenderer> ();
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "DropDownMenuColumnDefinition"; }
    }

    /// <summary> Gets or sets the text displayed as the menu title. </summary>
    /// <value> A <see cref="string"/> displayed as the menu's title. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The menu title, can be empty.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string MenuTitleText
    {
      get { return _menuTitleText; }
      set { _menuTitleText = value ?? string.Empty; }
    }

    /// <summary> Gets or sets the icon displayed in the menu's title field. </summary>
    /// <value> An <see cref="IconInfo"/> displayed in the menu's title field. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("An icon displayed in the menu's title field, can be empty.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public IconInfo MenuTitleIcon
    {
      get { return _menuTitleIcon; }
      set { _menuTitleIcon = value; }
    }


    public override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      
      base.LoadResources (resourceManager, globalizationService);

      string key = ResourceManagerUtility.GetGlobalResourceKey (MenuTitleText);
      if (!string.IsNullOrEmpty (key))
        MenuTitleText = resourceManager.GetString (key);

      if (MenuTitleIcon != null)
        MenuTitleIcon.LoadResources (resourceManager);
    }
  }
}
