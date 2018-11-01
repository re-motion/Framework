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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Remotion.Mixins;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Design
{
  public class BindableObjectDataSourceDesigner : BocDataSourceDesigner
  {
    public BindableObjectDataSourceDesigner ()
    {
    }

    public override void Initialize (System.ComponentModel.IComponent component)
    {
      base.Initialize (component);

      BindableObjectTypeFinder typeFinder = new BindableObjectTypeFinder (component.Site);
      MixinConfiguration mixinConfiguration = typeFinder.GetMixinConfiguration (false);
      MixinConfiguration.SetActiveConfiguration (mixinConfiguration);

      IPropertyValueUIService propertyValueUIService = (IPropertyValueUIService) component.Site.GetService (typeof (IPropertyValueUIService));
      propertyValueUIService.AddPropertyValueUIHandler (PropertyValueUIHandler);
    }

    private void PropertyValueUIHandler (ITypeDescriptorContext context, PropertyDescriptor propDesc, ArrayList valueUIItemList)
    {
      if (propDesc.DisplayName == "TypeName")
      {
        string value = propDesc.GetValue (Component) as string;
        if (!IsValidTypeName (value))
        {
          Image image = new Bitmap (8, 8);
          using (Graphics g = Graphics.FromImage (image))
          {
            g.FillEllipse (Brushes.Red, 0, 0, 8, 8);
          }
          valueUIItemList.Add (
              new PropertyValueUIItem (
                  image, delegate { }, string.Format ("Could not load type '{0}'.", TypeUtility.ParseAbbreviatedTypeName (value))));
        }
      }
    }

    private bool IsValidTypeName (string value)
    {
      if (string.IsNullOrEmpty (value))
        return true;
      if (TypeUtility.GetType (value, false) == null)
        return false;
      return true;
    }
  }
}
