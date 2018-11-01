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
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides functionality to resolve <see cref="PropertyInfo"/> objects into child objects of <see cref="ClassDefinition"/>.
  /// </summary>
  public static class ReflectionBasedPropertyResolver
  {
    public static T ResolveDefinition<T> (IPropertyInformation propertyInformation, ClassDefinition classDefinition, Func<string, T> definitionGetter) 
        where T : class
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("definitionGetter", definitionGetter);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var matchingImplementations = GetMatchingDefinitions (propertyInformation, classDefinition, definitionGetter);

      if (matchingImplementations.Count > 1)
      {
        var implementingTypeNames = matchingImplementations.Select (tuple => "'" + tuple.Item1.DeclaringType + "'").OrderBy (name => name);
        var message = string.Format (
            "The property '{0}' is ambiguous, it is implemented by the following types valid in the context of class '{1}': {2}.",
            propertyInformation.Name,
            classDefinition.ClassType.Name,
            string.Join (", ", implementingTypeNames));
        throw new InvalidOperationException (message);
      }

      return matchingImplementations.Select (tuple => tuple.Item2).SingleOrDefault();
    }

    private static List<Tuple<IPropertyInformation, T>> GetMatchingDefinitions<T> (
        IPropertyInformation propertyInformation, 
        ClassDefinition classDefinition, 
        Func<string, T> definitionGetter) where T: class
    {
      IEnumerable<IPropertyInformation> propertyImplementationCandidates;
      if (propertyInformation.DeclaringType.IsInterface)
      {
        var implementingTypes = GetImplementingTypes (classDefinition, propertyInformation);
        propertyImplementationCandidates = implementingTypes
            .Select (propertyInformation.FindInterfaceImplementation)
            .Where (pi => pi != null);
      }
      else
      {
        propertyImplementationCandidates = new[] { propertyInformation };
      }

      return (from pi in propertyImplementationCandidates
              let propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (pi)
              let definition = definitionGetter (propertyIdentifier)
              where definition != null
              select Tuple.Create (pi, definition))
          .Distinct (new DelegateBasedEqualityComparer<Tuple<IPropertyInformation, T>> (
                         (x, y) => object.Equals (x.Item1, y.Item1), 
                         x => x.Item1.GetHashCode()))
          .ToList();
    }

    private static IEnumerable<Type> GetImplementingTypes (ClassDefinition classDefinition, IPropertyInformation interfaceProperty)
    {
      Assertion.IsTrue (interfaceProperty.DeclaringType.IsInterface);

      if (interfaceProperty.DeclaringType.IsAssignableFrom (TypeAdapter.Create (classDefinition.ClassType)))
        yield return classDefinition.ClassType;

      var implementingPersistentMixins = 
          from cd in classDefinition.CreateSequence (cd => cd.BaseClass)
          from mixinType in cd.PersistentMixins
          where interfaceProperty.DeclaringType.IsAssignableFrom (TypeAdapter.Create (mixinType))
          select mixinType;

      foreach (var implementingPersistentMixin in implementingPersistentMixins)
        yield return implementingPersistentMixin;
    }
  }
}