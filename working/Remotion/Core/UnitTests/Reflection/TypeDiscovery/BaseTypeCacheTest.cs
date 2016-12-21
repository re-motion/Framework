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
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery;
using Remotion.UnitTests.Reflection.TypeDiscovery.BaseTypeCacheTestDomain;

namespace Remotion.UnitTests.Reflection.TypeDiscovery
{
  [TestFixture]
  public class BaseTypeCacheTest
  {
    private readonly Type[] _testDomain =
    {
        typeof (Cat), typeof (Pet), typeof (Dog), typeof (MaineCoon), typeof (Ragdoll), typeof (Siberian),
        typeof (ILongHairedBreed), typeof (IHamster),
        typeof (ValueTypeWithInterface), typeof (IInterfaceForValueType)
    };

    [Test]
    public void GetTypes_ForObject_ReturnsAllTypes ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (object)), Is.EquivalentTo (_testDomain.Concat (new[] { typeof (object),  typeof (ValueType), typeof (ICloneable) })));
    }

    [Test]
    public void GetTypes_ForObject_ContainsIntefaceWithoutImplementations ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (object)), Contains.Item (typeof (IHamster)));
    }

    [Test]
    public void GetTypes_SubHierarchy ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (
          baseTypeCache.GetTypes (typeof (Cat)),
          Is.EquivalentTo (new[] { typeof (Cat), typeof (MaineCoon), typeof (Ragdoll), typeof (Siberian) }));
    }

    [Test]
    public void GetTypes_WholeHierarchy ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (
          baseTypeCache.GetTypes (typeof (Pet)),
          Is.EquivalentTo (new[] { typeof (Cat), typeof (Pet), typeof (Dog), typeof (MaineCoon), typeof (Ragdoll), typeof (Siberian) }));
    }

    [Test]
    public void GetTypes_NoDescendingTypes ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (MaineCoon)), Is.EqualTo (new[] { typeof (MaineCoon) }));
    }

    [Test]
    public void GetTypes_ForInterface_ReturnsInterfaceAndImplementations ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (
          baseTypeCache.GetTypes (typeof (ILongHairedBreed)),
          Is.EquivalentTo (new[] { typeof (ILongHairedBreed), typeof (MaineCoon), typeof (Siberian) }));
    }

    [Test]
    public void GetTypes_ForInterfaceWithoutImplementations_ReturnsInterface ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (IHamster)), Is.EquivalentTo (new[] { typeof (IHamster) }));
    }

    [Test]
    public void GetTypes_ForInterfaceWithImplementationButNotPartOfTestDomain_ReturnsImplementations ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (ICloneable)), Is.EquivalentTo (new[] { typeof (Siberian) }));
    }

    [Test]
    public void GetTypes_ForTypeNotPartOfTestDomain_ReturnsEmptyList ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (BaseTypeCacheTest)), Is.Empty);
    }

    [Test]
    public void GetTypes_ForInterfaceOnValueType_ReturnsInterfaceAndImplementations ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (
          baseTypeCache.GetTypes (typeof (IInterfaceForValueType)),
          Is.EquivalentTo (new[] { typeof (IInterfaceForValueType), typeof (ValueTypeWithInterface) }));
    }

    [Test]
    public void GetTypes_ForValueType_ReturnsType ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (ValueTypeWithInterface)), Is.EquivalentTo (new[] { typeof (ValueTypeWithInterface) }));
    }

    [Test]
    public void GetTypes_ForValueTypeFromGac_ReturnsEmptyList ()
    {
      var baseTypeCache = BaseTypeCache.Create (_testDomain);

      Assert.That (baseTypeCache.GetTypes (typeof (Decimal)), Is.Empty);
    }
  }
}