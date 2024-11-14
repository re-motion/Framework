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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class InterceptedPropertyCollectorAdapterTest : StandardMappingTest
  {
    private InterceptedPropertyCollectorAdapter _adapter;

    private ClassDefinition _classDefinition;
    private Type _concreteBaseType;

    public override void SetUp ()
    {
      base.SetUp();

      _adapter = new InterceptedPropertyCollectorAdapter();

      _classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(MyDomainObject));
      _concreteBaseType = typeof(MyConcreteBaseType);
    }

    [Test]
    public void GetPropertyInterceptors_FiltersNonOverridableProperty ()
    {
      var result = _adapter.GetPropertyInterceptors(_classDefinition, _concreteBaseType);

      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty((MyDomainObject o) => o.NonOverridableProperty);
      CheckAbsence(result, property.GetGetMethod(), property.GetSetMethod());
    }

    [Test]
    public void GetPropertyInterceptors_UsesMostDerivedOverride ()
    {
      var result = _adapter.GetPropertyInterceptors(_classDefinition, _concreteBaseType);

      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty((MyConcreteBaseType o) => o.OverriddenProperty);
      CheckContains(result, property.GetGetMethod(), property.GetSetMethod());
    }

    [Test]
    public void GetPropertyInterceptors_ReturnsImplementingAccessor_ForAutomaticProperty ()
    {
      var result = _adapter.GetPropertyInterceptors(_classDefinition, _concreteBaseType).ToArray();

      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty((MyDomainObject o) => o.AutomaticProperty);
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();
      CheckContains(result, property.GetGetMethod(), property.GetSetMethod());

      var getterInterceptor = result.Single(ai => GetInterceptedAccessorMethod(ai).Equals(getter));
      var setterInterceptor = result.Single(ai => GetInterceptedAccessorMethod(ai).Equals(setter));
      Assert.That(getterInterceptor, Is.TypeOf<ImplementingGetAccessorInterceptor>());
      Assert.That(setterInterceptor, Is.TypeOf<ImplementingSetAccessorInterceptor>());
    }

    [Test]
    public void GetPropertyInterceptors_ReturnsWrappingAccessor_ForUserImplementedProperty ()
    {
      var result = _adapter.GetPropertyInterceptors(_classDefinition, _concreteBaseType).ToArray();

      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty((MyDomainObject o) => o.UserImplementedProperty);
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();
      CheckContains(result, property.GetGetMethod(), property.GetSetMethod());

      var getterInterceptor = result.Single(ai => GetInterceptedAccessorMethod(ai).Equals(getter));
      var setterInterceptor = result.Single(ai => GetInterceptedAccessorMethod(ai).Equals(setter));
      Assert.That(getterInterceptor, Is.TypeOf<WrappingAccessorInterceptor>());
      Assert.That(setterInterceptor, Is.TypeOf<WrappingAccessorInterceptor>());
    }

    [Test]
    public void GetPropertyInterceptors_ConsideresNonPublicProperty ()
    {
      var result = _adapter.GetPropertyInterceptors(_classDefinition, _concreteBaseType);

      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty((MyDomainObject o) => o.NonPublicProperty);
      CheckContains(result, property.GetGetMethod(true), property.GetSetMethod(true));
    }

    [Test]
    public void GetPropertyInterceptors_SupportsReadOnlyProperties ()
    {
      var result = _adapter.GetPropertyInterceptors(_classDefinition, _concreteBaseType);

      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty((MyDomainObject o) => o.ReadOnlyProperty);
      CheckContains(result, property.GetGetMethod());
    }

    [Test]
    public void GetPropertyInterceptors_SupportsWriteOnlyProperties ()
    {
      var result = _adapter.GetPropertyInterceptors(_classDefinition, _concreteBaseType);

      var property = typeof(MyDomainObject).GetProperty("WriteOnlyProperty");
      CheckContains(result, property.GetSetMethod());
    }

    private void CheckAbsence (IEnumerable<IAccessorInterceptor> accessorInterceptors, params MethodInfo[] expectedAbsentInterceptedAccessors)
    {
      ArgumentUtility.CheckNotNull("accessorInterceptors", accessorInterceptors);
      ArgumentUtility.CheckNotNull("expectedAbsentInterceptedAccessors", expectedAbsentInterceptedAccessors);

      var actualInterceptedAccessors = accessorInterceptors.Select(GetInterceptedAccessorMethod);
      Assert.That(actualInterceptedAccessors.Intersect(expectedAbsentInterceptedAccessors), Is.Empty);
    }

    private void CheckContains (IEnumerable<IAccessorInterceptor> accessorInterceptors, params MethodInfo[] expectedInterceptedAccessors)
    {
      ArgumentUtility.CheckNotNull("accessorInterceptors", accessorInterceptors);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("expectedInterceptedAccessors", expectedInterceptedAccessors);

      var actualInterceptedAccessors = accessorInterceptors.Select(GetInterceptedAccessorMethod);
      Assert.That(expectedInterceptedAccessors, Is.SubsetOf(actualInterceptedAccessors));
    }

    private MethodInfo GetInterceptedAccessorMethod (IAccessorInterceptor accessorInterceptor)
    {
      return (MethodInfo)PrivateInvoke.GetNonPublicField(accessorInterceptor, "_interceptedAccessorMethod");
    }

    [DBTable]
    [IncludeInMappingTestDomain]
    public class MyDomainObject : DomainObject
    {
      public int NonOverridableProperty
      {
        get { return 7; }
        set { Dev.Null = value; }
      }
      public virtual int OverriddenProperty { get; set; }
      public virtual int AutomaticProperty { get; set; }
      public virtual int UserImplementedProperty
      {
        get { return 7; }
        set { Dev.Null = value; }
      }
      protected internal virtual int NonPublicProperty { get; set; }
      public virtual int ReadOnlyProperty
      {
        get { return 7; }
      }
      public virtual int WriteOnlyProperty
      {
        set { Dev.Null = value; }
      }
    }

    [IncludeInMappingTestDomain]
    public class MyConcreteBaseType : MyDomainObject
    {
      public override int OverriddenProperty { get; set; }
    }
  }
}
