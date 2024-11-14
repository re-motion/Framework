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
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class EnumerationReflectorTest
  {
    // types

    // static members

    // member fields

    private EnumerationReflector _enumerationReflector;
    private MetadataCache _cache;

    // construction and disposing

    public EnumerationReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _enumerationReflector = new EnumerationReflector();
      _cache = new MetadataCache();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_enumerationReflector, Is.InstanceOf(typeof(IEnumerationReflector)));
    }

    [Test]
    public void GetValues ()
    {
      Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues(typeof(DomainAccessTypes), _cache);

      Assert.That(values, Is.Not.Null);
      Assert.That(values.Count, Is.EqualTo(2));

      Assert.That(values[DomainAccessTypes.Journalize].Value, Is.EqualTo(0));
      Assert.That(values[DomainAccessTypes.Journalize].Name, Is.EqualTo("Journalize"));
      Assert.That(values[DomainAccessTypes.Journalize].ID, Is.EqualTo("00000002-0001-0000-0000-000000000000"));

      Assert.That(values[DomainAccessTypes.Archive].Value, Is.EqualTo(1));
      Assert.That(values[DomainAccessTypes.Archive].Name, Is.EqualTo("Archive"));
      Assert.That(values[DomainAccessTypes.Archive].ID, Is.EqualTo("00000002-0002-0000-0000-000000000000"));
    }

    [Test]
    public void GetValue ()
    {
      EnumValueInfo value = _enumerationReflector.GetValue(DomainAccessTypes.Journalize, _cache);

      Assert.That(value, Is.Not.Null);

      Assert.That(value.Value, Is.EqualTo(0));
      Assert.That(value.Name, Is.EqualTo("Journalize"));
      Assert.That(value.ID, Is.EqualTo("00000002-0001-0000-0000-000000000000"));
    }

    [Test]
    public void GetValuesFromCache ()
    {
      Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues(typeof(DomainAccessTypes), _cache);

      Assert.That(_cache.GetEnumValueInfo(DomainAccessTypes.Journalize), Is.SameAs(values[DomainAccessTypes.Journalize]));
      Assert.That(_cache.GetEnumValueInfo(DomainAccessTypes.Archive), Is.SameAs(values[DomainAccessTypes.Archive]));
    }

    [Test]
    public void GetMetadataWithInvalidType ()
    {
      Assert.That(
          () => new EnumerationReflector().GetValues(typeof(string), _cache),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The type 'System.String' is not an enumerated type.", "type"));
    }
  }
}
