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
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class AssemblyReflectorTest
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IClassReflector _classReflectorMock;
    private IAbstractRoleReflector _abstractRoleReflectorMock;
    private IAccessTypeReflector _accessTypeReflectorMock;
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
      _mocks = new MockRepository ();
      _accessTypeReflectorMock = _mocks.StrictMock<IAccessTypeReflector> ();
      _classReflectorMock = _mocks.StrictMock<IClassReflector> ();
      _abstractRoleReflectorMock = _mocks.StrictMock<IAbstractRoleReflector> ();
      _assemblyReflector = new AssemblyReflector (_accessTypeReflectorMock, _classReflectorMock, _abstractRoleReflectorMock);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_assemblyReflector.ClassReflector, Is.SameAs (_classReflectorMock));
      Assert.That (_assemblyReflector.AccessTypeReflector, Is.SameAs (_accessTypeReflectorMock));
      Assert.That (_assemblyReflector.AbstractRoleReflector, Is.SameAs (_abstractRoleReflectorMock));
    }

    [Test]
    public void GetMetadata ()
    {
      Assembly securityAssembly = typeof (IAccessTypeReflector).Assembly;
      Assembly assembly = typeof (File).Assembly;

      Expect
          .Call (_accessTypeReflectorMock.GetAccessTypesFromAssembly(securityAssembly, _cache))
          .Return (new List<EnumValueInfo> (new EnumValueInfo[] {AccessTypes.Read, AccessTypes.Write}));
      Expect
        .Call (_accessTypeReflectorMock.GetAccessTypesFromAssembly (assembly, _cache))
        .Return (new List<EnumValueInfo> (new EnumValueInfo[] { AccessTypes.Journalize, AccessTypes.Archive }));
      Expect.Call (_abstractRoleReflectorMock.GetAbstractRoles (securityAssembly, _cache)).Return (new List<EnumValueInfo> ());
      Expect
          .Call (_abstractRoleReflectorMock.GetAbstractRoles (assembly, _cache))
          .Return (new List<EnumValueInfo> (new EnumValueInfo[] { AbstractRoles.Clerk, AbstractRoles.Secretary, AbstractRoles.Administrator }));
      Expect.Call (_classReflectorMock.GetMetadata (typeof (File), _cache)).Return (new SecurableClassInfo());
      Expect.Call (_classReflectorMock.GetMetadata (typeof (PaperFile), _cache)).Return (new SecurableClassInfo ());
      _mocks.ReplayAll ();

      _assemblyReflector.GetMetadata (assembly, _cache);

      _mocks.VerifyAll ();
    }
  }
}
