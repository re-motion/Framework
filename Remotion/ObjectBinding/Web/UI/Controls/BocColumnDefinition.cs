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
  /// <summary> A BocColumnDefinition defines how to display a column of a list. </summary>
  public abstract class BocColumnDefinition : BusinessObjectControlItem
  {
    private string _itemID = string.Empty;
    private WebString _columnTitle = WebString.Empty;
    private IconInfo _columnTitleIcon = new IconInfo();
    private BocColumnTitleStyle _columnTitleStyle = BocColumnTitleStyle.Text;
    private Unit _width = Unit.Empty;
    private string _cssClass = string.Empty;

    protected BocColumnDefinition ()
    {
    }

    public IBocColumnRenderer GetRenderer (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull("serviceLocator", serviceLocator);

      return GetRendererInternal(serviceLocator);
    }

    protected abstract IBocColumnRenderer GetRendererInternal (IServiceLocator locator);


    public override string ToString ()
    {
      string? displayName = ItemID;
      if (string.IsNullOrEmpty(displayName))
        displayName = ColumnTitle.ToString();
      if (string.IsNullOrEmpty(displayName))
        return DisplayedTypeName;
      else
        return string.Format("{0}: {1}", displayName, DisplayedTypeName);
    }

    /// <summary> Gets the programmatic name of the <see cref="BocColumnDefinition"/>. </summary>
    /// <value> A <see cref="string"/> providing an identifier for the <see cref="BocColumnDefinition"/>. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Misc")]
    [Description("The programmatic name of the column definition.")]
    [DefaultValue("")]
    [NotifyParentProperty(true)]
    [ParenthesizePropertyName(true)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override string? ItemID
    {
      get { return _itemID; }
      set { _itemID = value ?? string.Empty; }
    }

    /// <summary> Gets the displayed value of the column title. </summary>
    /// <remarks> Override this property to change the way the column title text is generated. </remarks>
    /// <value> A <see cref="WebString"/> representing this column's title row contents. </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public virtual WebString ColumnTitleDisplayValue
    {
      get { return ColumnTitle; }
    }

    /// <summary> Gets the displayed image of the column title. </summary>
    /// <remarks> Override this property to change the way the column title icon is generated. </remarks>
    /// <value> A <see cref="IconInfo"/> representing this column's title icon. </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public virtual IconInfo ColumnTitleIconDisplayValue
    {
      get { return ColumnTitleIcon; }
    }

    /// <summary> Gets or sets the text displayed in the column title. </summary>
    /// <remarks>
    ///   Override this property to add validity checks to the set accessor.
    /// </remarks>
    /// <value> A <see cref="WebString"/> representing the manually set title of this column. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("The manually assigned value of the column title, can be empty.")]
    [DefaultValue(typeof(WebString), "")]
    [NotifyParentProperty(true)]
    public virtual WebString ColumnTitle
    {
      get { return _columnTitle; }
      set { _columnTitle = value; }
    }

    /// <summary>
    ///   Gets or sets the image displayed in the title cell of the column. Must not be <see langword="null"/>.
    ///   Only displayed if <see cref="BocColumnTitleStyle"/>.<see cref="BocColumnTitleStyle.Icon"/> is set in <see cref="ColumnTitleStyle"/>.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Appearance")]
    [Description("The image displayed in the column title in the rendered page.")]
    [NotifyParentProperty(true)]
    public IconInfo ColumnTitleIcon
    {
      get { return _columnTitleIcon; }
      set
      {
        ArgumentUtility.CheckNotNull("Icon", value);
        _columnTitleIcon = value;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="BocColumnTitleStyle"/> that is used to determine how the column title cell is displayed.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Appearance")]
    [Description("The style determining how the column title is displayed in the rendered page.")]
    [NotifyParentProperty(true)]
    public BocColumnTitleStyle ColumnTitleStyle
    {
      get { return _columnTitleStyle; }
      set { _columnTitleStyle = value; }
    }

    /// <summary> Gets or sets the width of the column definition. </summary>
    /// <value> A <see cref="Unit"/> providing the width of this column when it is rendered. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Layout")]
    [Description("The width of the rendered column.")]
    [DefaultValue(typeof(Unit), "")]
    [NotifyParentProperty(true)]
    public Unit Width
    {
      get { return _width; }
      set { _width = value; }
    }

    /// <summary> Gets or sets the CSS-class of the column definition. </summary>
    /// <value> A <see cref="string"/> providing the CSS-class added to the class attribute when this column is rendered. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Layout")]
    [Description("The CSS-class of the rendered column's cells.")]
    [DefaultValue("")]
    [NotifyParentProperty(true)]
    public string CssClass
    {
      get { return _cssClass; }
      set { _cssClass = value ?? string.Empty; }
    }


    /// <summary> Gets the human readable name of this type. </summary>
    protected virtual string DisplayedTypeName
    {
      get { return "ColumnDefinition"; }
    }

    public override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      base.LoadResources(resourceManager, globalizationService);

      string? key = ResourceManagerUtility.GetGlobalResourceKey(ColumnTitle.GetValue());
      if (!string.IsNullOrEmpty(key))
        ColumnTitle = resourceManager.GetWebString(key, ColumnTitle.Type);
    }
  }
}
