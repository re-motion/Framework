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

namespace Remotion.Web.UI.Design
{

public class TypeDescriptorContext : ITypeDescriptorContext
{
  private IDesigner _designer;
  private IServiceProvider _provider;
  private PropertyDescriptor _propertyDescriptor;

  public TypeDescriptorContext (
      IDesigner designer,
      IServiceProvider provider,
      PropertyDescriptor propertyDescriptor)
  {
    _designer = designer;
    _provider = provider;
    _propertyDescriptor = propertyDescriptor;
  }

  private IComponentChangeService ComponentChangeService
  {
    get { return (IComponentChangeService) this.GetService (typeof (IComponentChangeService)); }
  }

  public object GetService (Type serviceType)
  {
    return _provider.GetService(serviceType);
  }

  public IContainer Container
  {
    get { return _designer.Component.Site.Container; }
  }

  public object Instance
  {
    get { return _designer.Component; }
  }

  public PropertyDescriptor PropertyDescriptor
  {
    get { return _propertyDescriptor; }
  }

  public void OnComponentChanged()
  {
    if (ComponentChangeService != null)
      ComponentChangeService.OnComponentChanged (Instance, PropertyDescriptor, null, null);
  }

  public bool OnComponentChanging()
  {
    if (ComponentChangeService != null)
    {
      try
      {
        ComponentChangeService.OnComponentChanging (Instance, PropertyDescriptor);
      }
      catch (CheckoutException e)
      {
        if (e == CheckoutException.Canceled)
          return false;
        throw e;
      }
    }
    return true;
  }
}

}
