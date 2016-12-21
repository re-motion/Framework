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
using System.ComponentModel.Design;
using System.Drawing.Design;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{
public class TabbedMenuDesigner: WebControlDesigner, IServiceProvider
{
  private DesignerVerbCollection _verbs = null;

	public TabbedMenuDesigner()
	{
    _verbs = new DesignerVerbCollection();
    _verbs.Add (new DesignerVerb ("Edit Menu Tabs", new EventHandler(OnVerbEditFixedColumns)));
  }

  private void OnVerbEditFixedColumns (object sender, EventArgs e) 
  {
    TabbedMenu tabStripMenu = Component as TabbedMenu;
    if (tabStripMenu == null)
      throw new InvalidOperationException ("Cannot use TabbedMenuDesigner for objects other than TabbedMenu.");

    PropertyDescriptorCollection propertyDescriptors = TypeDescriptor.GetProperties (tabStripMenu);
    PropertyDescriptor propertyDescriptor = propertyDescriptors["Tabs"];

    TypeDescriptorContext context = new TypeDescriptorContext (this, this, propertyDescriptor);
    object value = propertyDescriptor.GetValue (Component);
    MainMenuTabCollectionEditor editor = null;
    editor = (MainMenuTabCollectionEditor) TypeDescriptor.GetEditor (value, typeof(UITypeEditor));
    editor.EditValue (context, this, value);
  }

  public override DesignerVerbCollection Verbs 
  {
    get { return _verbs; }
  }

  protected override object GetService (Type serviceType)
  {
    return base.GetService (serviceType);
  }

  object IServiceProvider.GetService (Type serviceType)
  {
    return GetService (serviceType);
  }
}

}
