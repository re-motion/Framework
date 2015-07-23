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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample.ReferenceDataSourceTestDomain
{
#pragma warning disable 612,618
  public class ReferenceDataSourceTestDefaultValueService : IDefaultValueService
  {
    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      return typeof (ReferenceDataSourceTestDomainBase).IsAssignableFrom (property.PropertyType);
    }

    public IBusinessObject Create (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return (IBusinessObject) ObjectFactory.Create (property.PropertyType);
    }

    public bool IsDefaultValue (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, IBusinessObject value, IBusinessObjectProperty[] emptyProperties)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNullAndType<ReferenceDataSourceTestDomainBase> ("value", value);
      ArgumentUtility.CheckNotNull ("emptyProperties", emptyProperties);

      // NOP

      return true;
    }
  }
#pragma warning restore 612,618
}