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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition that acts as a placeholder for inserting a column for each property. </summary>
  public class BocAllPropertiesPlaceholderColumnDefinition : BocColumnDefinition
  {
    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator locator)
    {
      throw new NotSupportedException(GetType().Name + " cannot be rendered directly, but must be replaced by other column definitions.");
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string? ItemID
    {
      get { return base.ItemID; }
      set { base.ItemID = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override WebString ColumnTitle
    {
      get { return base.ColumnTitle; }
      set { base.ColumnTitle = value; }
    }

    /// <summary> Gets or sets the combined width of the generated column definitions. </summary>
    /// <value> A <see cref="Unit"/> providing the combined width of the generated columns when they are rendered. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Layout")]
    [Description("The width used for all generated property columns combined.")]
    [DefaultValue(typeof(Unit), "")]
    [NotifyParentProperty(true)]
    public new Unit Width
    {
      get { return base.Width; }
      set { base.Width = value; }
    }

    /// <summary> Gets or sets the CSS-class of the generated column definitions. </summary>
    /// <value> A <see cref="string"/> providing the CSS-class added to the class attribute when the columns are rendered. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Layout")]
    [Description("The CSS-class of the generated columns' cells.")]
    [DefaultValue("")]
    [NotifyParentProperty(true)]
    public new string CssClass
    {
      get { return base.CssClass; }
      set { base.CssClass = value; }
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "AllPropertiesPlaceholderColumnDefinition"; }
    }
  }
}
