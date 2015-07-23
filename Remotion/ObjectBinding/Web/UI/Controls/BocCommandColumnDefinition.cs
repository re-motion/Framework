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
  /// <summary> A column definition containing no data, only the <see cref="BocListItemCommand"/>. </summary>
  public class BocCommandColumnDefinition : BocCommandEnabledColumnDefinition
  {
    private string _text = string.Empty;
    private IconInfo _icon = new IconInfo();

    public BocCommandColumnDefinition ()
    {
    }

    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);
      
      return serviceLocator.GetInstance<IBocCommandColumnRenderer>();
    }

    /// <summary> Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinition"/>. </summary>
    /// <returns> Returns <see cref="Text"/>, followed by the the class name of the instance.  </returns>
    public override string ToString ()
    {
      string displayName = ItemID;
      if (string.IsNullOrEmpty (displayName))
        displayName = ColumnTitle;
      if (string.IsNullOrEmpty (displayName))
        displayName = Text;
      if (string.IsNullOrEmpty (displayName))
        return DisplayedTypeName;
      else
        return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
    }

    /// <summary> Gets or sets the text representing the command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string Text
    {
      get { return _text; }
      set { _text = value ?? string.Empty; }
    }

    /// <summary> 
    ///   Gets or sets the image representing the  command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo Icon
    {
      get { return _icon; }
      set
      {
        ArgumentUtility.CheckNotNull ("Icon", value);
        _icon = value;
      }
    }

    private bool ShouldSerializeIcon ()
    {
      return IconInfo.ShouldSerialize (_icon);
    }

    private void ResetIcon ()
    {
      _icon.Reset();
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "CommandColumnDefinition"; }
    }

    public override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      
      base.LoadResources (resourceManager, globalizationService);

      string key = ResourceManagerUtility.GetGlobalResourceKey (Text);
      if (!string.IsNullOrEmpty (key))
        Text = resourceManager.GetString (key);

      Icon.LoadResources (resourceManager);
    }
  }
}
