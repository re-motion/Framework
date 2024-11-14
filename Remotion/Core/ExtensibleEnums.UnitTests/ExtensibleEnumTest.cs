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
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.ServiceLocation;

namespace Remotion.ExtensibleEnums.UnitTests
{
  [TestFixture]
  public class ExtensibleEnumTest
  {
    private ExtensibleEnumDefinitionCache _extensibleEnumDefinitionCache;

    [SetUp]
    public void SetUp ()
    {
      _extensibleEnumDefinitionCache = SafeServiceLocator.Current.GetInstance<ExtensibleEnumDefinitionCache>();
    }

    [Test]
    public void Initialization_IDOnly ()
    {
      Assert.That(EnumWithDifferentCtors.Values.IDOnly().ValueName, Is.EqualTo("ValueName"));
      Assert.That(EnumWithDifferentCtors.Values.IDOnly().DeclarationSpace, Is.Null);
      Assert.That(EnumWithDifferentCtors.Values.IDOnly().ID, Is.EqualTo("ValueName"));
    }

    [Test]
    public void Initialization_DeclarationSpaceAndName ()
    {
      Assert.That(EnumWithDifferentCtors.Values.DeclarationSpaceAndName().ValueName, Is.EqualTo("ValueName"));
      Assert.That(EnumWithDifferentCtors.Values.DeclarationSpaceAndName().DeclarationSpace, Is.EqualTo("DeclarationSpace"));
      Assert.That(EnumWithDifferentCtors.Values.DeclarationSpaceAndName().ID, Is.EqualTo("DeclarationSpace.ValueName"));
    }

    [Test]
    public void Initialization_NameAndNullDeclarationSpace ()
    {
      Assert.That(EnumWithDifferentCtors.Values.NameAndNullDeclarationSpace().ValueName, Is.EqualTo("ValueName"));
      Assert.That(EnumWithDifferentCtors.Values.NameAndNullDeclarationSpace().DeclarationSpace, Is.Null);
      Assert.That(EnumWithDifferentCtors.Values.NameAndNullDeclarationSpace().ID, Is.EqualTo("ValueName"));
    }

    [Test]
    public void Initialization_NameAndEmptyDeclarationSpace ()
    {
      Assert.That(EnumWithDifferentCtors.Values.NameAndEmptyDeclarationSpace().ValueName, Is.EqualTo("ValueName"));
      Assert.That(EnumWithDifferentCtors.Values.NameAndEmptyDeclarationSpace().DeclarationSpace, Is.Empty);
      Assert.That(EnumWithDifferentCtors.Values.NameAndEmptyDeclarationSpace().ID, Is.EqualTo("ValueName"));
    }

    [Test]
    public void Initialization_DeclaringTypeAndName ()
    {
      Assert.That(EnumWithDifferentCtors.Values.DeclaringTypeAndName().ValueName, Is.EqualTo("ValueName"));
      Assert.That(EnumWithDifferentCtors.Values.DeclaringTypeAndName().DeclarationSpace, Is.EqualTo(typeof(EnumWithDifferentCtorsExtensions).FullName));
      Assert.That(
          EnumWithDifferentCtors.Values.DeclaringTypeAndName().ID,
          Is.EqualTo("Remotion.ExtensibleEnums.UnitTests.TestDomain.EnumWithDifferentCtorsExtensions.ValueName"));
    }

    [Test]
    public void Initialization_CurrentMethod ()
    {
      Assert.That(EnumWithDifferentCtors.Values.CurrentMethod().ValueName, Is.EqualTo("CurrentMethod"));
      Assert.That(EnumWithDifferentCtors.Values.CurrentMethod().DeclarationSpace, Is.EqualTo(typeof(EnumWithDifferentCtorsExtensions).FullName));
      Assert.That(
          EnumWithDifferentCtors.Values.CurrentMethod().ID,
          Is.EqualTo("Remotion.ExtensibleEnums.UnitTests.TestDomain.EnumWithDifferentCtorsExtensions.CurrentMethod"));
    }

    [Test]
    public void EquatableEqualsTrue ()
    {
      IEquatable<Color> value1 = new Color("ID");
      var value2 = new Color("ID");

      Assert.That(value1.Equals(value2), Is.True);
    }

    [Test]
    public void EquatableEqualsFalse_DifferentIDs ()
    {
      IEquatable<Color> value1 = new Color("ID1");
      var value2 = new Color("ID2");

      Assert.That(value1.Equals(value2), Is.False);
    }

    [Test]
    public void EquatableEqualsFalse_DifferentTypes ()
    {
      IEquatable<Color> value1 = new Color("ID");
      var value2 = new MetallicColor("ID");

      Assert.That(value1.Equals(value2), Is.False);
    }

    [Test]
    public void EquatableEqualsFalse_Null ()
    {
      IEquatable<Color> value = new Color("ID");

      Assert.That(value.Equals(null), Is.False);
    }

    [Test]
    public void Equals_True ()
    {
      var value1 = new Color("ID");
      var value2 = new Color("ID");

      Assert.That(value1.Equals((object)value2), Is.True);
    }

    [Test]
    public void Equals_False_DifferentIDs ()
    {
      var value1 = new Color("ID1");
      var value2 = new Color("ID2");

      Assert.That(value1.Equals((object)value2), Is.False);
    }

    [Test]
    public void Equals_False_DifferentTypes ()
    {
      var value1 = new Color("ID");
      var value2 = new MetallicColor("ID");

      Assert.That(value1.Equals((object)value2), Is.False);
    }

    [Test]
    public void Equals_False_Null ()
    {
      Color value = new Color("ID");

      Assert.That(value.Equals((object)null), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var value1 = new Color("ID");
      var value2 = new Color("ID");

      Assert.That(value1.GetHashCode(), Is.EqualTo(value2.GetHashCode()));
    }

    [Test]
    public void EqualsOperator_True ()
    {
      var value1 = new Color("ID");
      var value2 = new Color("ID");

      Assert.That(value1 == value2, Is.True);
      Assert.That(value1 != value2, Is.False);
    }

    [Test]
    public void EqualsOperator_True_Nulls ()
    {
      Color value1 = null;
      Color value2 = null;

      Assert.That(value1 == value2, Is.True);
      Assert.That(value1 != value2, Is.False);
    }

    [Test]
    public void EqualsOperator_False_DifferentIDs ()
    {
      var value1 = new Color("ID1");
      var value2 = new Color("ID2");

      Assert.That(value1 == value2, Is.False);
      Assert.That(value1 != value2, Is.True);
    }

    [Test]
    public void EqualsOperator_False_DifferentTypes ()
    {
      var value1 = new Color("ID");
      var value2 = new MetallicColor("ID");

      Assert.That(value1 == value2, Is.False);
      Assert.That(value1 != value2, Is.True);
    }

    [Test]
    public void EqualsOperator_False_Null ()
    {
      Color value = new Color("ID");

      Assert.That(value == null, Is.False);
      Assert.That(value != null, Is.True);

      Assert.That(null == value, Is.False);
      Assert.That(null != value, Is.True);
    }

    [Test]
    public void ToString_ReturnsFullID ()
    {
      var value = new EnumWithDifferentCtors("Prefix", "ValueName");

      Assert.That(value.ToString(), Is.EqualTo("Prefix.ValueName"));
    }

    [Test]
    public void Values ()
    {
      Assert.That(Color.Values, Is.Not.Null);
    }

    [Test]
    public void Values_FromCache ()
    {
      Assert.That(Color.Values, Is.SameAs(_extensibleEnumDefinitionCache.GetDefinition(typeof(Color))));
    }

    [Test]
    public void Values_IntegrationTest ()
    {
      var valueInfos = Color.Values.GetValueInfos();
      Assert.That(valueInfos.Select(info => info.Value).ToArray(),
          Is.EqualTo(new[] {
              Color.Values.Green(),
              Color.Values.LightBlue(),
              Color.Values.LightRed(),
              Color.Values.Red(),
              Color.Values.RedMetallic(),
          }));
    }

    [Test]
    public void Values_Ordering_IntegrationTest ()
    {
      var valueInfos = Planet.Values.GetValueInfos();
      Assert.That(
          valueInfos.Select(info => info.Value).ToArray(),
          Is.EqualTo(new[] {
              Planet.Values.Mercury(),
              Planet.Values.Venus(),
              Planet.Values.Earth(),
              Planet.Values.Mars(),
              Planet.Values.Jupiter(),
              Planet.Values.Saturn(),
              Planet.Values.Uranus(),
              Planet.Values.Neptune(),
              Planet.Values.Pluto(),
              Planet.Values.Eris() }));
    }

    [Test]
    public void GetEnumType ()
    {
      var value = new Color("Red");
      Assert.That(value.GetEnumType(), Is.SameAs(typeof(Color)));
    }

    [Test]
    public void GetEnumType_DerivedType ()
    {
      var value = new MetallicColor("RedMetallic");
      Assert.That(value.GetEnumType(), Is.SameAs(typeof(Color)));
    }

    [Test]
    public void GetValueInfo ()
    {
      var value = new Color("Red");
      Assert.That(value.GetValueInfo(), Is.SameAs(Color.Values.GetValueInfoByID("Red")));
    }
  }
}
