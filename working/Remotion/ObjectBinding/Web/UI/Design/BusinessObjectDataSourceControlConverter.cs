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
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Design
{
public class BusinessObjectDataSourceControlConverter : StringConverter
{
  public BusinessObjectDataSourceControlConverter()
  {
  }

  private object[] GetDataSourceControls (object instance, IContainer container)
  {
    ComponentCollection components = container.Components;
    ArrayList dataSources = new ArrayList();

    ICollection controls = null;
    if (instance is Array)
      controls = (Array) instance;
    else
      controls = new IBusinessObjectBoundWebControl[] {(IBusinessObjectBoundWebControl) instance};

    for (int idxComponents = 0; idxComponents < components.Count; idxComponents++)
    {
      IComponent component = (IComponent) components[idxComponents];
      IBusinessObjectDataSourceControl dataSource = component as IBusinessObjectDataSourceControl;
      if (dataSource != null && ! string.IsNullOrEmpty (dataSource.ID))
      {
        bool hasSelfReference = false;
        foreach (IBusinessObjectBoundWebControl control in controls)
        {
          if (dataSource == control)
          {
            hasSelfReference = true;
            break;
          }
        }
        if (! hasSelfReference)
          dataSources.Add (dataSource.ID);
      }
    }

    dataSources.Sort(Comparer.Default);
    return dataSources.ToArray();
  }

  public override TypeConverter.StandardValuesCollection GetStandardValues (ITypeDescriptorContext context)
  {
    if ((context != null) && (context.Container != null))
    {
      object[] dataSources = GetDataSourceControls (context.Instance, context.Container);
      if (dataSources != null)
        return new TypeConverter.StandardValuesCollection (dataSources);
    }
    return null;
  }

  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return true;
  }
}
}
