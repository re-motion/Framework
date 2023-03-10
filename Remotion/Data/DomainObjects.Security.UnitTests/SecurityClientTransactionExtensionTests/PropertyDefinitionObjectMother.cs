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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests
{
  public static class PropertyDefinitionObjectMother
  {
    public static PropertyDefinition CreatePropertyDefinition (PropertyInfo propertyInfo, Type classType = null)
    {
      return new PropertyDefinition(
          MappingConfiguration.Current.GetTypeDefinition(classType ?? typeof(SecurableObject)),
          PropertyInfoAdapter.Create(propertyInfo),
          "Test",
          false,
          false,
          null,
          StorageClass.Persistent,
          propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null);
    }
  }
}
