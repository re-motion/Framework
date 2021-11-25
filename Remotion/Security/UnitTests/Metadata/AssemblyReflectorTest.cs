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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class AssemblyReflectorTest
  {
    // types

    // static members

    // member fields

    private Mock<IClassReflector> _classReflectorMock;
    private Mock<IAbstractRoleReflector> _abstractRoleReflectorMock;
    private Mock<IAccessTypeReflector> _accessTypeReflectorMock;
    private AssemblyReflector _assemblyReflector;
    private MetadataCache _cache;

    // construction and disposing

    public AssemblyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _accessTypeReflectorMock = new Mock<IAccessTypeReflector>(MockBehavior.Strict);
      _classReflectorMock = new Mock<IClassReflector>(MockBehavior.Strict);
      _abstractRoleReflectorMock = new Mock<IAbstractRoleReflector>(MockBehavior.Strict);
      _assemblyReflector = new AssemblyReflector(_accessTypeReflectorMock.Object, _classReflectorMock.Object, _abstractRoleReflectorMock.Object);
      _cache = new MetadataCache();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_assemblyReflector.ClassReflector, Is.SameAs(_classReflectorMock.Object));
      Assert.That(_assemblyReflector.AccessTypeReflector, Is.SameAs(_accessTypeReflectorMock.Object));
      Assert.That(_assemblyReflector.AbstractRoleReflector, Is.SameAs(_abstractRoleReflectorMock.Object));
    }

    [Test]
    public void GetMetadata ()
    {
      Assembly securityAssembly = typeof(IAccessTypeReflector).Assembly;
      Assembly assembly = typeof(File).Assembly;

      _accessTypeReflectorMock
          .Setup(_ => _.GetAccessTypesFromAssembly(securityAssembly, _cache))
          .Returns(new List<EnumValueInfo>(new EnumValueInfo[] {AccessTypes.Read, AccessTypes.Write}))
          .Verifiable();
      _accessTypeReflectorMock
        .Setup(_ => _.GetAccessTypesFromAssembly(assembly, _cache))
        .Returns(new List<EnumValueInfo>(new EnumValueInfo[] { AccessTypes.Journalize, AccessTypes.Archive }))
        .Verifiable();
      _abstractRoleReflectorMock.Setup(_ => _.GetAbstractRoles(securityAssembly, _cache)).Returns(new List<EnumValueInfo>()).Verifiable();
      _abstractRoleReflectorMock
          .Setup(_ => _.GetAbstractRoles(assembly, _cache))
          .Returns(new List<EnumValueInfo>(new EnumValueInfo[] { AbstractRoles.Clerk, AbstractRoles.Secretary, AbstractRoles.Administrator }))
          .Verifiable();
      _classReflectorMock.Setup(_ => _.GetMetadata(typeof(File), _cache)).Returns(new SecurableClassInfo()).Verifiable();
      _classReflectorMock.Setup(_ => _.GetMetadata(typeof(PaperFile), _cache)).Returns(new SecurableClassInfo()).Verifiable();

      _assemblyReflector.GetMetadata(assembly, _cache);

      _accessTypeReflectorMock.Verify();
      _classReflectorMock.Verify();
      _abstractRoleReflectorMock.Verify();
    }
  }
}
