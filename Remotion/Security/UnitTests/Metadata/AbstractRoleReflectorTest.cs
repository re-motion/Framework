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
using Moq;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class AbstractRoleReflectorTest
  {
    // types

    // static members

    // member fields

    private Mock<IEnumerationReflector> _enumeratedTypeReflectorMock;
    private AbstractRoleReflector _abstractRoleReflector;
    private MetadataCache _cache;

    // construction and disposing

    public AbstractRoleReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _enumeratedTypeReflectorMock = new Mock<IEnumerationReflector>(MockBehavior.Strict);
      _abstractRoleReflector = new AbstractRoleReflector(_enumeratedTypeReflectorMock.Object);
      _cache = new MetadataCache();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_abstractRoleReflector, Is.InstanceOf(typeof(IAbstractRoleReflector)));
      Assert.That(_abstractRoleReflector.EnumerationTypeReflector, Is.SameAs(_enumeratedTypeReflectorMock.Object));
    }

    [Test]
    public void GetAbstractRoles ()
    {
      Dictionary<Enum, EnumValueInfo> domainAbstractRoles = new Dictionary<Enum, EnumValueInfo>();
      domainAbstractRoles.Add(DomainAbstractRoles.Clerk, AbstractRoles.Clerk);
      domainAbstractRoles.Add(DomainAbstractRoles.Secretary, AbstractRoles.Secretary);

      Dictionary<Enum, EnumValueInfo> specialAbstractRoles = new Dictionary<Enum, EnumValueInfo>();
      specialAbstractRoles.Add(SpecialAbstractRoles.Administrator, AbstractRoles.Administrator);

      _enumeratedTypeReflectorMock.Setup(_ => _.GetValues(typeof(DomainAbstractRoles), _cache)).Returns(domainAbstractRoles).Verifiable();
      _enumeratedTypeReflectorMock.Setup(_ => _.GetValues(typeof(SpecialAbstractRoles), _cache)).Returns(specialAbstractRoles).Verifiable();

      List<EnumValueInfo> values = _abstractRoleReflector.GetAbstractRoles(typeof(File).Assembly, _cache);

      _enumeratedTypeReflectorMock.Verify();

      Assert.That(values, Is.Not.Null);
      Assert.That(values.Count, Is.EqualTo(3));
      Assert.That(values, Has.Member(AbstractRoles.Clerk));
      Assert.That(values, Has.Member(AbstractRoles.Secretary));
      Assert.That(values, Has.Member(AbstractRoles.Administrator));
    }

    [Test]
    public void GetAbstractRolesFromCache ()
    {
      AbstractRoleReflector reflector = new AbstractRoleReflector();
      List<EnumValueInfo> expectedAbstractRoles = reflector.GetAbstractRoles(typeof(File).Assembly, _cache);
      List<EnumValueInfo> actualAbstractRoles = _cache.GetAbstractRoles();

      Assert.That(expectedAbstractRoles.Count, Is.EqualTo(3));
      foreach (EnumValueInfo expected in expectedAbstractRoles)
        Assert.That(actualAbstractRoles, Has.Member(expected));
    }
  }
}
