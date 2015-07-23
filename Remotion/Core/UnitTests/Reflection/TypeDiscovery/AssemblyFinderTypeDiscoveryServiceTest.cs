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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection.TypeDiscovery
{
  [TestFixture]
  public class AssemblyFinderTypeDiscoveryServiceTest
  {
    private MockRepository _mockRepository;
    private IAssemblyFinder _finderMock;

    private readonly Assembly _testAssembly = typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly;
    private readonly Assembly _coreAssembly = typeof (AssemblyFinder).Assembly;
    private readonly Assembly _mscorlibAssembly = typeof (object).Assembly;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();

      _finderMock = _mockRepository.StrictMock<IAssemblyFinder> ();
    }

    [Test]
    public void GetTypes_UsesAssemblyFinder ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindAssemblies ()).Return (new Assembly[0]);

      _mockRepository.ReplayAll();
      service.GetTypes (typeof (object), true);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetTypes_ReturnsTypesFromFoundAssemblies ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindAssemblies ()).Return (new[] { _testAssembly, _coreAssembly });

      var allTypes = new List<Type>();
      allTypes.AddRange (_testAssembly.GetTypes ());
      allTypes.AddRange (_coreAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), true);

      Assert.That (allTypes, Is.SubsetOf (types));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithGlobalTypes_BaseTypeIsObject ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindAssemblies ()).Return (new[] { _testAssembly, _mscorlibAssembly });

      var allTypes = new List<Type> ();
      allTypes.AddRange (_testAssembly.GetTypes ());
      allTypes.AddRange (_mscorlibAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), false);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }
    

    [Test]
    public void GetTypes_WithGlobalTypes_BaseTypeIsNull ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindAssemblies ()).Return (new[] { _testAssembly, _mscorlibAssembly });

      var allTypes = new List<Type> ();
      allTypes.AddRange (_testAssembly.GetTypes ());
      allTypes.AddRange (_mscorlibAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (null, false);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithoutGlobalTypes ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindAssemblies ()).Return (new[] { _mscorlibAssembly });

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), true);
      Assert.That (types, Is.Empty);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithGlobalTypesButBaseTypeIsNotFromGac_SkippesGacLookUp ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindAssemblies()).Return (new[] { _testAssembly, _mscorlibAssembly });

      _mockRepository.ReplayAll();
      Dev.Null = service.GetTypes (typeof (object), true);
      _mockRepository.VerifyAll();

      _mockRepository.BackToRecordAll();
      _finderMock.Expect (mock => mock.FindAssemblies()).Return (new[] { _testAssembly, _mscorlibAssembly }).Repeat.Never();
      _mockRepository.ReplayAll();

      ICollection types = service.GetTypes (typeof (Base), false);
      Assert.That (types, Is.EquivalentTo (new []{typeof (Base), typeof (Derived1), typeof (Derived2)}));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetTypes_WithoutSpecificBase ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindAssemblies ()).Return (new[] { _testAssembly });

      var allTypes = new List<Type> (_testAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (null, true);
      Assert.That (allTypes, Is.SubsetOf (types));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithSpecificBase ()
    {
      var finderMock = _mockRepository.StrictMock<IAssemblyFinder> ();

      var service = new AssemblyFinderTypeDiscoveryService (finderMock);
      finderMock.Expect (mock => mock.FindAssemblies ()).Return (new[] { _testAssembly });

      var allTypes = new List<Type> ();
      allTypes.AddRange (_testAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (Base), true);
      Assert.That (types, Is.Not.EquivalentTo (allTypes));
      Assert.That (types, Is.EquivalentTo (new[] {typeof (Base), typeof (Derived1), typeof (Derived2)}));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithSealedType ()
    {
      var finderMock = _mockRepository.StrictMock<IAssemblyFinder>();
      _mockRepository.ReplayAll ();

      var service = new AssemblyFinderTypeDiscoveryService (finderMock);

      ICollection types = service.GetTypes (typeof (Sealed), true);
      Assert.That (types, Is.EquivalentTo (new[] { typeof (Sealed) }));
      finderMock.AssertWasNotCalled (mock => mock.FindAssemblies());
    }

    [Test]
    public void GetTypes_WithValueType ()
    {
      var finderMock = _mockRepository.StrictMock<IAssemblyFinder>();
      _mockRepository.ReplayAll ();

      var service = new AssemblyFinderTypeDiscoveryService (finderMock);

      ICollection types = service.GetTypes (typeof (ValueType), true);
      Assert.That (types, Is.EquivalentTo (new[] { typeof (ValueType) }));
      finderMock.AssertWasNotCalled (mock => mock.FindAssemblies());
    }

    public class Base { }
    public class Derived1 : Base { }
    public class Derived2 : Base { }
    public sealed class Sealed { }
    public struct ValueType { }
  }
}
