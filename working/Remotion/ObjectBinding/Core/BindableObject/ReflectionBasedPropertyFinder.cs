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
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  public class ReflectionBasedPropertyFinder : IPropertyFinder
  {
    private readonly Type _concreteType;
    private readonly MultiDictionary<MethodInfo, MethodInfo> _interfaceMethodImplementations; // from implementation to declaration

    public ReflectionBasedPropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      _concreteType = concreteType;

      _interfaceMethodImplementations = GetInterfaceMethodImplementationCache ();
    }

    // Note: Should no longer be needed after re-bind is changed to explicitly support interfaces.
    private MultiDictionary<MethodInfo, MethodInfo> GetInterfaceMethodImplementationCache ()
    {
      var cache = new MultiDictionary<MethodInfo, MethodInfo> ();
      foreach (Type currentType in GetInheritanceHierarchy ())
      {
        foreach (Type interfaceType in currentType.GetInterfaces ())
        {
          InterfaceMapping mapping = currentType.GetInterfaceMap (interfaceType);
          for (int i = 0; i < mapping.TargetMethods.Length; ++i)
            cache.Add (mapping.TargetMethods[i], mapping.InterfaceMethods[i]);
        }
      }
      return cache;
    }

    public IEnumerable<IPropertyInformation> GetPropertyInfos ()
    {
      var propertyNames = new HashSet<string>();
      
      foreach (Type currentType in GetInheritanceHierarchy ())
      {
        var propertyInfos = currentType.FindMembers (
            MemberTypes.Property, 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, 
            PropertyFilter, 
            null);
        
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
          if (!propertyNames.Contains (propertyInfo.Name))
          {
            yield return GetPropertyInformation(propertyInfo);
            propertyNames.Add (propertyInfo.Name);
          }
        }
      }
    }

    // Note: The re-bind-specific IPropertyInformation implementations should no longer be necessary after re-bind is changed to explicitly support 
    //       interfaces. (Because re-bind will no longer represent explicit interface properties within the BusinessObjectClass for the class -
    //       the BusinessObjectClass for the interface must be used instead.)
    private IPropertyInformation GetPropertyInformation (PropertyInfo propertyInfo)
    {
      var introducedMemberAttributes = propertyInfo.GetCustomAttributes (typeof (IntroducedMemberAttribute), true);
      if (introducedMemberAttributes.Length > 0)
      {
        var introducedMemberAttribute = (IntroducedMemberAttribute) introducedMemberAttributes[0];
        var interfaceProperty = PropertyInfoAdapter.Create(introducedMemberAttribute.IntroducedInterface.GetProperty (introducedMemberAttribute.InterfaceMemberName));
        var mixinProperty = interfaceProperty.FindInterfaceImplementation (introducedMemberAttribute.Mixin);
        var interfaceImplementation = new InterfaceImplementationPropertyInformation (mixinProperty, interfaceProperty);

        return new MixinIntroducedPropertyInformation (interfaceImplementation);
      }
      else
      {
        var propertyInfoAdapter = PropertyInfoAdapter.Create(propertyInfo);
        var interfaceDeclaration = propertyInfoAdapter.FindInterfaceDeclarations().FirstOrDefault();
        if (interfaceDeclaration != null)
          return new InterfaceImplementationPropertyInformation (propertyInfoAdapter, interfaceDeclaration);
        else
          return propertyInfoAdapter;
      }
    }

    private IEnumerable<Type> GetInheritanceHierarchy ()
    {
      return _concreteType.CreateSequence (c => c.BaseType);
    }

    protected virtual bool PropertyFilter (MemberInfo memberInfo, object filterCriteria)
    {
      // properties explicitly marked invisible are ignored
      var attribute = AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (memberInfo, true);
      if (attribute != null && !attribute.Visible)
        return false;

      var propertyInfo = (PropertyInfo) memberInfo;

      // indexed properties are ignored
      if (propertyInfo.GetIndexParameters ().Length > 0)
        return false;

      // properties with a public getter are included, as long as that getter is not an infrastructure property
      var publicGetter = propertyInfo.GetGetMethod (false);
      if (publicGetter != null && !IsInfrastructureProperty (propertyInfo, publicGetter))
        return true;

      // property can be any interface implementation as long as it is not an infrastructure property
      var publicOrNonPublicGetter = publicGetter ?? propertyInfo.GetGetMethod (true);
      return publicOrNonPublicGetter != null
             && _interfaceMethodImplementations.ContainsKey (publicOrNonPublicGetter)
             && !_interfaceMethodImplementations[publicOrNonPublicGetter].TrueForAll (m => IsInfrastructureProperty (propertyInfo, m));
    }

    protected virtual bool IsInfrastructureProperty (PropertyInfo propertyInfo, MethodInfo accessorDeclaration)
    {
      return accessorDeclaration.DeclaringType.Assembly == typeof (IBusinessObject).Assembly
          || accessorDeclaration.DeclaringType.Assembly == typeof (BindableObjectClass).Assembly
          || accessorDeclaration.DeclaringType.Assembly == typeof (IMixinTarget).Assembly;
    }
  }
}
