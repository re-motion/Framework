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
using System.Collections.Generic;
using System.Linq;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="ClassDefinitionCollectionFactory"/> is used to get the list of <see cref="ClassDefinition"/>s for a set of types. It automatically
  /// sets base classes and derived classes correctly.
  /// </summary>
  public class ClassDefinitionCollectionFactory
  {
    private readonly IMappingObjectFactory _mappingObjectFactory;

    public ClassDefinitionCollectionFactory (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull ("mappingObjectFactory", mappingObjectFactory);

      _mappingObjectFactory = mappingObjectFactory;
    }

    public ClassDefinition[] CreateClassDefinitionCollection (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var inheritanceHierarchyFilter = new InheritanceHierarchyFilter (types.ToArray());
      var leafTypes = inheritanceHierarchyFilter.GetLeafTypes();

      var classDefinitions = new Dictionary<Type, ClassDefinition>();
      foreach (var type in leafTypes)
        GetClassDefinition (classDefinitions, type);
      
      SetDerivedClasses (classDefinitions.Values);

      return classDefinitions.Values.ToArray();
    }

    public ClassDefinition GetClassDefinition (IDictionary<Type, ClassDefinition> classDefinitions, Type classType)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("classType", classType);

      if (classDefinitions.ContainsKey (classType))
        return classDefinitions[classType];

      var baseClassDefinition = GetBaseClassDefinition (classDefinitions, classType);
      var classDefinition = _mappingObjectFactory.CreateClassDefinition (classType, baseClassDefinition);
      classDefinitions.Add (classDefinition.ClassType, classDefinition);

      return classDefinition;
    }

    private ClassDefinition GetBaseClassDefinition (IDictionary<Type, ClassDefinition> classDefinitions, Type type)
    {
      if (ReflectionUtility.IsInheritanceRoot (type))
        return null;

      return GetClassDefinition (classDefinitions, type.BaseType);
    }

    private void SetDerivedClasses (IEnumerable<ClassDefinition> classDefinitions)
    {
      var classesByBaseClass = (from classDefinition in classDefinitions
                                where classDefinition.BaseClass != null
                                group classDefinition by classDefinition.BaseClass)
          .ToDictionary (grouping => grouping.Key, grouping => (IEnumerable<ClassDefinition>) grouping);

      foreach (var classDefinition in classDefinitions)
      {
        IEnumerable<ClassDefinition> derivedClasses;
        if (!classesByBaseClass.TryGetValue (classDefinition, out derivedClasses))
          derivedClasses = Enumerable.Empty<ClassDefinition> ();

        classDefinition.SetDerivedClasses (derivedClasses);
      }
    }
  }
}