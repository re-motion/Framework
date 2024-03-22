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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="ClassIDProvider"/> provides the class-id for a given <see cref="Type"/>.
  /// </summary>
  [ImplementationFor(typeof(IClassIDProvider), Lifetime = LifetimeKind.Singleton)]
  public class ClassIDProvider : IClassIDProvider
  {
    public string GetClassID (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var attribute = AttributeUtility.GetCustomAttribute<ClassIDAttribute>(type, false);
      return attribute != null ? attribute.ClassID : type.Name;
    }
  }
}
