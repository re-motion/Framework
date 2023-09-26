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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.UnitTests.Reflection.TypeDiscovery
{
  [TestFixture]
  public class AssemblyFinderTypeDiscoveryServiceTest
  {
    private Mock<IAssemblyFinder> _finderMock;

    private readonly Assembly _testAssembly = typeof(AssemblyFinderTypeDiscoveryServiceTest).Assembly;
    private readonly Assembly _remotionAssembly = typeof(AssemblyFinder).Assembly;
    private readonly Assembly _dotnetAssembly = typeof(object).Assembly;

    [SetUp]
    public void SetUp ()
    {
      _finderMock = new Mock<IAssemblyFinder>(MockBehavior.Strict);
    }

    [Test]
    public void GetTypes_UsesAssemblyFinder ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new Assembly[0]).Verifiable();

      service.GetTypes(typeof(object), true);
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_ReturnsTypesFromFoundAssemblies ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly, _remotionAssembly }).Verifiable();

      var expectedTypes = new List<Type>();
      expectedTypes.AddRange(_testAssembly.GetTypes());
      expectedTypes.AddRange(_remotionAssembly.GetTypes());

      ICollection types = service.GetTypes(typeof(object), true);

      Assert.That(types, Is.SupersetOf(expectedTypes));
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithGlobalTypes_BaseTypeIsObject ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      var assemblies = new HashSet<Assembly>();
      assemblies.Add(_remotionAssembly);
      assemblies.Add(_dotnetAssembly);
      assemblies.Add(typeof(log4net.ILog).Assembly);
      assemblies.Add(typeof(System.Collections.Specialized.StringCollection).Assembly);
      assemblies.Add(typeof(System.Collections.CollectionBase).Assembly);
      assemblies.Add(typeof(System.IServiceProvider).Assembly);
      assemblies.Add(typeof(System.ComponentModel.Component).Assembly);
      assemblies.Add(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly);
      assemblies.Add(typeof(System.ComponentModel.TypeConverter).Assembly);
      assemblies.Add(typeof(System.Configuration.Provider.ProviderBase).Assembly);
      assemblies.Add(typeof(System.Diagnostics.TraceListener).Assembly);
      assemblies.Add(typeof(System.Text.RegularExpressions.Regex).Assembly);
      assemblies.Add(typeof(System.Xml.XmlNode).Assembly);
      var expectedTypes = new HashSet<Type>(assemblies.SelectMany(a => a.GetTypes()));

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(assemblies).Verifiable();

      ICollection types = service.GetTypes(typeof(object), excludeGlobalTypes: false);

      var missingTypes = expectedTypes.Except(types.Cast<Type>()).ToList();
      var extraTypes = types.Cast<Type>().Except(expectedTypes).ToList();
      Assert.That(Array.Empty<Type>(), Is.EquivalentTo(missingTypes));
      Assert.That(extraTypes, Is.EquivalentTo(Array.Empty<Type>()));

      _finderMock.Verify();
    }


    [Test]
    public void GetTypes_WithGlobalTypes_BaseTypeIsNull ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      var assemblies = new HashSet<Assembly>();
      assemblies.Add(_remotionAssembly);
      assemblies.Add(_dotnetAssembly);
      assemblies.Add(typeof(log4net.ILog).Assembly);
      assemblies.Add(typeof(System.Collections.Specialized.StringCollection).Assembly);
      assemblies.Add(typeof(System.Collections.CollectionBase).Assembly);
      assemblies.Add(typeof(System.IServiceProvider).Assembly);
      assemblies.Add(typeof(System.ComponentModel.Component).Assembly);
      assemblies.Add(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly);
      assemblies.Add(typeof(System.ComponentModel.TypeConverter).Assembly);
      assemblies.Add(typeof(System.Configuration.Provider.ProviderBase).Assembly);
      assemblies.Add(typeof(System.Diagnostics.TraceListener).Assembly);
      assemblies.Add(typeof(System.Text.RegularExpressions.Regex).Assembly);
      assemblies.Add(typeof(System.Xml.XmlNode).Assembly);
      var expectedTypes = new HashSet<Type>(assemblies.SelectMany(a => a.GetTypes()));

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(assemblies).Verifiable();

      ICollection types = service.GetTypes(null, excludeGlobalTypes: false);

      var missingTypes = expectedTypes.Except(types.Cast<Type>()).ToList();
      var extraTypes = types.Cast<Type>().Except(expectedTypes).ToList();
      Assert.That(Array.Empty<Type>(), Is.EquivalentTo(missingTypes));
      Assert.That(extraTypes, Is.EquivalentTo(Array.Empty<Type>()));

      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithoutGlobalTypes ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _dotnetAssembly }).Verifiable();

      var expectedTypes = new List<Type>();
      expectedTypes.AddRange(_dotnetAssembly.GetTypes());

      ICollection types = service.GetTypes(typeof(object), excludeGlobalTypes: true);
#if FEATURE_GAC
      Assert.That(types, Is.Empty);
#else
      Assert.That(types, Is.EquivalentTo(expectedTypes));
#endif

      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithGlobalTypesButBaseTypeIsNotFromGac_SkipsGacLookUp ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly, _dotnetAssembly }).Verifiable();

      Dev.Null = service.GetTypes(typeof(object), excludeGlobalTypes: true);
      _finderMock.Verify();

      _finderMock.Reset();
      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly, _dotnetAssembly }).Verifiable();

      ICollection types = service.GetTypes(typeof(Base), excludeGlobalTypes: false);
      Assert.That(types, Is.EquivalentTo(new []{typeof(Base), typeof(Derived1), typeof(Derived2)}));
      _finderMock.Verify(mock => mock.FindAssemblies(), Times.Never());
    }

    [Test]
    public void GetTypes_WithoutSpecificBase ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly }).Verifiable();

      var expectedTypes = new List<Type>(_testAssembly.GetTypes());

      ICollection types = service.GetTypes(null, true);
      Assert.That(types, Is.SupersetOf(expectedTypes));
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithSpecificBase ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly }).Verifiable();

      var excludedTypes = new List<Type>();
      excludedTypes.AddRange(_testAssembly.GetTypes());

      ICollection types = service.GetTypes(typeof(Base), true);
      Assert.That(types, Is.Not.EquivalentTo(excludedTypes));
      Assert.That(types, Is.EquivalentTo(new[] {typeof(Base), typeof(Derived1), typeof(Derived2)}));
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithOpenGenericType ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly }).Verifiable();

      ICollection types = service.GetTypes(typeof(OpenGenericBase<>), true);
      Assert.That(types, Is.EquivalentTo(new[] { typeof(OpenGenericBase<>), typeof(OpenGenericDerived<>), typeof(ClosedGenericDerived) }));
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithClosedGenericType ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly }).Verifiable();

      ICollection types = service.GetTypes(typeof(OpenGenericBase<int>), true);
      Assert.That(types, Is.Empty);
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithOpenGenericTypeFromGac ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _dotnetAssembly }).Verifiable();

      ICollection types = service.GetTypes(typeof(IReadOnlyDictionary<,>), excludeGlobalTypes: false);
      Assert.That(
          types,
          Is.SupersetOf(
              new[] { typeof(IReadOnlyDictionary<,>), typeof(Dictionary<,>) }));
      Assert.That(types.Cast<Type>().Where(t => t.IsConstructedGenericType), Is.Empty);
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithClosedGenericTypeFromGac ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _dotnetAssembly }).Verifiable();

      ICollection types = service.GetTypes(typeof(IReadOnlyDictionary<int, string>), excludeGlobalTypes: false);
      Assert.That(types, Is.Empty);
      _finderMock.Verify();
    }

    [Test]
    public void GetTypes_WithSealedType ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      ICollection types = service.GetTypes(typeof(Sealed), true);
      Assert.That(types, Is.EquivalentTo(new[] { typeof(Sealed) }));
      _finderMock.Verify(mock => mock.FindAssemblies(), Times.Never());
    }

    [Test]
    public void GetTypes_WithValueType ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      ICollection types = service.GetTypes(typeof(ValueType), true);
      Assert.That(types, Is.EquivalentTo(new[] { typeof(ValueType) }));
      _finderMock.Verify(mock => mock.FindAssemblies(), Times.Never());
    }

    [Test]
    public void GetTypes_WithGlobalBaseTypeAndExcludeGlobalTypes_IsIncludedInResult ()
    {
      var service = new AssemblyFinderTypeDiscoveryService(_finderMock.Object);

      _finderMock.Setup(mock => mock.FindAssemblies()).Returns(new[] { _testAssembly }).Verifiable();

      var objectTypes = service.GetTypes(typeof(object), true);
      Assert.That(objectTypes, Does.Contain(typeof(ExtendedHashtable)));

      var iCollectionTypes = service.GetTypes(typeof(ICollection), true);
      Assert.That(iCollectionTypes, Does.Contain(typeof(Hashtable)));

      _finderMock.Verify();
    }

    public class Base { }
    public class Derived1 : Base { }
    public class Derived2 : Base { }
    public sealed class Sealed { }
    public struct ValueType { }
    public class OpenGenericBase<T> { }
    public class OpenGenericDerived<T> : OpenGenericBase<T> { }
    public class ClosedGenericDerived : OpenGenericDerived<int> { }
    public class ExtendedHashtable : Hashtable { }
  }
}
