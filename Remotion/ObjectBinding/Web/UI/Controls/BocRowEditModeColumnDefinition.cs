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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition used for switching between edit mode and returning from it via save and cancel. </summary>
  public class BocRowEditModeColumnDefinition : BocColumnDefinition
  {
    private WebString _editText = WebString.Empty;
    private IconInfo _editIcon = new IconInfo();
    private WebString _saveText = WebString.Empty;
    private IconInfo _saveIcon = new IconInfo();
    private WebString _cancelText = WebString.Empty;
    private IconInfo _cancelIcon = new IconInfo();
    private BocRowEditColumnDefinitionShow _show = BocRowEditColumnDefinitionShow.EditMode;

    public BocRowEditModeColumnDefinition ()
    {
    }

    /// <summary>
    ///   Determines when the column is shown to the user in regard of the <see cref="BocList"/>'s read-only setting.
    /// </summary>
    /// <value> 
    ///   One of the <see cref="BocRowEditColumnDefinitionShow"/> enumeration values. 
    ///   The default is <see cref="BocRowEditColumnDefinitionShow.EditMode"/>.
    /// </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("Determines when to show the column to the user in regard to the BocList's read-only setting.")]
    [DefaultValue(BocRowEditColumnDefinitionShow.EditMode)]
    [NotifyParentProperty(true)]
    public BocRowEditColumnDefinitionShow Show
    {
      get { return _show; }
      set { _show = value; }
    }

    /// <summary> Gets or sets the text representing the edit command in the rendered page. </summary>
    /// <value>A <see cref="WebString"/> representing the edit command.</value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("The text representing the edit command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue(typeof(WebString), "")]
    [NotifyParentProperty(true)]
    public WebString EditText
    {
      get { return _editText; }
      set { _editText = value; }
    }

    /// <summary>
    ///  Gets or sets the image representing the edit command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the edit command. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Appearance")]
    [Description("The image representing the edit command in the rendered page.")]
    [NotifyParentProperty(true)]
    public IconInfo EditIcon
    {
      get { return _editIcon; }
      set
      {
        ArgumentUtility.CheckNotNull("EditIcon", value);
        _editIcon = value;
      }
    }

    private bool ShouldSerializeEditIcon ()
    {
      return IconInfo.ShouldSerialize(_editIcon);
    }

    private void ResetEditIcon ()
    {
      _editIcon.Reset();
    }


    /// <summary> Gets or sets the text representing the save command in the rendered page. </summary>
    /// <value>A <see cref="WebString"/> representing the save command.</value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("The text representing the save command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue(typeof(WebString), "")]
    [NotifyParentProperty(true)]
    public WebString SaveText
    {
      get { return _saveText; }
      set
      {
        _saveText = value;
      }
    }

    /// <summary> 
    ///   Gets or sets the image representing the save command in the rendered page. Must not be <see langword="null"/>.
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the save command. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Appearance")]
    [Description("The relative url to image representing the save command in the rendered page.")]
    [NotifyParentProperty(true)]
    public IconInfo SaveIcon
    {
      get { return _saveIcon; }
      set
      {
        ArgumentUtility.CheckNotNull("SaveIcon", value);
        _saveIcon = value;
      }
    }

    private bool ShouldSerializeSaveIcon ()
    {
      return IconInfo.ShouldSerialize(_saveIcon);
    }

    private void ResetSaveIcon ()
    {
      _saveIcon.Reset();
    }

    /// <summary> Gets or sets the text representing the cancel command in the rendered page. </summary>
    /// <value>A <see cref="WebString"/> representing the cancel command.</value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("The text representing the cancel command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue(typeof(WebString), "")]
    [NotifyParentProperty(true)]
    public WebString CancelText
    {
      get { return _cancelText; }
      set
      {
        _cancelText = value;
      }
    }

    /// <summary> 
    ///   Gets or sets the image representing the cancel command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the cancel command. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Appearance")]
    [Description("The image representing the cancel command in the rendered page.")]
    [NotifyParentProperty(true)]
    public IconInfo CancelIcon
    {
      get { return _cancelIcon; }
      set
      {
        ArgumentUtility.CheckNotNull("CancelIcon", value);
        _cancelIcon = value;
      }
    }

    private bool ShouldSerializeCancelIcon ()
    {
      return IconInfo.ShouldSerialize(_cancelIcon);
    }

    private void ResetCancelIcon ()
    {
      _cancelIcon.Reset();
    }

    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull("serviceLocator", serviceLocator);

      return serviceLocator.GetInstance<IBocRowEditModeColumnRenderer>();
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "RowEditModeColumnDefinition"; }
    }

    public override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      base.LoadResources(resourceManager, globalizationService);

      string? key = ResourceManagerUtility.GetGlobalResourceKey(EditText.GetValue());
      if (!string.IsNullOrEmpty(key))
        EditText = resourceManager.GetWebString(key, EditText.Type);

      key = ResourceManagerUtility.GetGlobalResourceKey(SaveText.GetValue());
      if (!string.IsNullOrEmpty(key))
        SaveText = resourceManager.GetWebString(key, SaveText.Type);

      key = ResourceManagerUtility.GetGlobalResourceKey(CancelText.GetValue());
      if (!string.IsNullOrEmpty(key))
        CancelText = resourceManager.GetWebString(key, CancelText.Type);

      if (EditIcon != null)
        EditIcon.LoadResources(resourceManager);
      if (SaveIcon != null)
        SaveIcon.LoadResources(resourceManager);
      if (CancelIcon != null)
        CancelIcon.LoadResources(resourceManager);
    }
  }
}
