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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  public class TestBase
  {
    [SetUp]
    public virtual void SetUp ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), null);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectWithIdentityProviderAttribute), null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), null);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectWithIdentityProviderAttribute), null);
    }

    protected IPropertyInformation GetPropertyInfo (Type type, string propertyName)
    {
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      Assert.IsNotNull (propertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return PropertyInfoAdapter.Create(propertyInfo);
    }

    protected IPropertyInformation GetPropertyInfo (Type type, Type interfaceType, string propertyName)
    {
      PropertyInfo interfacePropertyInfo = interfaceType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      Assert.IsNotNull (interfacePropertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, interfaceType);
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      if (propertyInfo == null)
      {
        Type interfaceTypeDefinition = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
        string explicitName = interfaceTypeDefinition.FullName.Replace ("`1", "<T>") + "." + interfacePropertyInfo.Name;
        propertyInfo = type.GetProperty (explicitName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        Assert.IsNotNull (propertyInfo, "Property '{0}' (or '{1}') was not found on type '{2}'.", propertyName, explicitName, type);
      }
      
      var introducedMemberAttributes = propertyInfo.GetCustomAttributes (typeof (IntroducedMemberAttribute), true);
      if (introducedMemberAttributes.Length > 0)
      {
        var introducedMemberAttribute = introducedMemberAttributes[0] as IntroducedMemberAttribute;
        var interfaceProperty = PropertyInfoAdapter.Create(introducedMemberAttribute.IntroducedInterface.GetProperty (introducedMemberAttribute.InterfaceMemberName));
        var mixinProperty = interfaceProperty.FindInterfaceImplementation (introducedMemberAttribute.Mixin);
        var interfaceImplementation = new InterfaceImplementationPropertyInformation (mixinProperty, interfaceProperty);

        return new MixinIntroducedPropertyInformation (interfaceImplementation);
      }
      else
      {
        var propertyInfoAdapter = PropertyInfoAdapter.Create(propertyInfo);
        var interfaceDeclaration = propertyInfoAdapter.FindInterfaceDeclarations().SingleOrDefault();
        if (interfaceDeclaration != null)
          return new InterfaceImplementationPropertyInformation (propertyInfoAdapter, interfaceDeclaration);
        else
          return propertyInfoAdapter;
      }
    }

    protected Type GetUnderlyingType (PropertyReflector reflector)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (reflector, typeof (PropertyReflector), "GetUnderlyingType");
    }

    protected PropertyBase.Parameters GetPropertyParameters (
        IPropertyInformation property,
        BindableObjectProvider provider,
        IBindablePropertyReadAccessStrategy bindablePropertyReadAccessStrategy = null,
        IBindablePropertyWriteAccessStrategy bindablePropertyWriteAccessStrategy = null,
        BindableObjectGlobalizationService bindableObjectGlobalizationService = null)
    {
      var reflector = new PropertyReflector (
          property,
          provider,
          MockRepository.GenerateStub<IDefaultValueStrategy>(),
          bindablePropertyReadAccessStrategy ?? SafeServiceLocator.Current.GetInstance<IBindablePropertyReadAccessStrategy>(),
          bindablePropertyWriteAccessStrategy ?? SafeServiceLocator.Current.GetInstance<IBindablePropertyWriteAccessStrategy>(),
          bindableObjectGlobalizationService ?? SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>());

      return (PropertyBase.Parameters) PrivateInvoke.InvokeNonPublicMethod (
          reflector,
          typeof (PropertyReflector),
          "CreateParameters",
          GetUnderlyingType (reflector));
    }

    protected BindableObjectProvider CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ()
    {
      return new BindableObjectProvider(BindableObjectMetadataFactory.Create(),MockRepository.GenerateStub<IBusinessObjectServiceFactory>());
    }

    protected PropertyBase.Parameters CreateParameters (
        BindableObjectProvider businessObjectProvider,
        IPropertyInformation propertyInfo,
        Type underlyingType,
        Type concreteType,
        IListInfo listInfo,
        bool isRequired,
        bool isReadOnly,
        IBindablePropertyReadAccessStrategy bindablePropertyReadAccessStrategy = null,
        IBindablePropertyWriteAccessStrategy bindablePropertyWriteAccessStrategy = null,
        BindableObjectGlobalizationService bindableObjectGlobalizationService = null)
    {
      return new PropertyBase.Parameters (
          businessObjectProvider,
          propertyInfo,
          underlyingType,
          new Lazy<Type> (() => concreteType),
          listInfo,
          isRequired,
          isReadOnly,
          new BindableObjectDefaultValueStrategy(),
          bindablePropertyReadAccessStrategy ?? SafeServiceLocator.Current.GetInstance<IBindablePropertyReadAccessStrategy>(),
          bindablePropertyWriteAccessStrategy ?? SafeServiceLocator.Current.GetInstance<IBindablePropertyWriteAccessStrategy>(),
          bindableObjectGlobalizationService ?? SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>());
    }
  }
}
