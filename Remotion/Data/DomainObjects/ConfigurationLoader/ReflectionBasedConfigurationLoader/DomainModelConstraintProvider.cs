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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="DomainModelConstraintProvider"/> is the default implementation of <see cref="IDomainModelConstraintProvider"/>.
  /// It uses the mapping attributes to resolve the constraints.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IDomainModelConstraintProvider), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class DomainModelConstraintProvider : IDomainModelConstraintProvider
  {
    public DomainModelConstraintProvider ()
    {
    }

    public bool IsNullable (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var attribute = propertyInfo.GetCustomAttribute<INullablePropertyAttribute>(true);
      return attribute == null || attribute.IsNullable;
    }

    public int? GetMaxLength (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var attribute = propertyInfo.GetCustomAttribute<ILengthConstrainedPropertyAttribute>(true);
      return attribute != null ? attribute.MaximumLength : null;
    }
  }
}
