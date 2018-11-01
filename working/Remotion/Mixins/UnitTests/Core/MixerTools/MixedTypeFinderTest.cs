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
using System.ComponentModel.Design;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTools;
using Remotion.Mixins.UnitTests.Core.MixerTools.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.MixerTools
{
  [TestFixture]
  public class MixedTypeFinderTest
  {
    private ClassContext _configuredClassContext1;
    private ClassContext _configuredClassContext2;
    private ClassContext _genericClassContext;
    private ClassContext _interfaceClassContext;

    private MixinConfiguration _configuration;

    private ITypeDiscoveryService _configuredTypeDiscoveryServiceStub;

    [SetUp]
    public void SetUp ()
    {
      _configuredClassContext1 = ClassContextObjectMother.Create(typeof (BaseType1), typeof (NullMixin));
      _configuredClassContext2 = ClassContextObjectMother.Create(typeof (NullTarget), typeof (NullMixin));
      _genericClassContext = ClassContextObjectMother.Create(typeof (GenericTargetClass<>), typeof (NullMixin));
      _interfaceClassContext = ClassContextObjectMother.Create(typeof (IBaseType2), typeof (NullMixin));

      var classContexts = new ClassContextCollection (_configuredClassContext1, _configuredClassContext2, _genericClassContext, _interfaceClassContext);
      _configuration = new MixinConfiguration (classContexts);

      _configuredTypeDiscoveryServiceStub = CreateTypeDiscoveryServiceStub (
          _configuredClassContext1.Type, 
          _configuredClassContext2.Type, 
          _genericClassContext.Type, 
          _interfaceClassContext.Type);
    }

    [Test]
    public void FindMixedTypes_ConfiguredContexts ()
    {
      var finder = new MixedTypeFinder (_configuredTypeDiscoveryServiceStub);
      var result = finder.FindMixedTypes (_configuration).ToArray ();

      Assert.That (result, Has.Member(_configuredClassContext1.Type));
      Assert.That (result, Has.Member (_configuredClassContext2.Type));
    }

    [Test]
    public void FindMixedTypes_ConfiguredContexts_NoGenerics ()
    {
      var finder = new MixedTypeFinder (_configuredTypeDiscoveryServiceStub);
      var result = finder.FindMixedTypes (_configuration).ToArray ();

      Assert.That (result, Has.No.Member (_genericClassContext.Type));
    }

    [Test]
    public void FindMixedTypes_ConfiguredContexts_NoInterfaces ()
    {
      var finder = new MixedTypeFinder (_configuredTypeDiscoveryServiceStub);
      var result = finder.FindMixedTypes (_configuration).ToArray ();

      Assert.That (result, Has.No.Member (_interfaceClassContext.Type));
    }

    [Test]
    public void FindMixedTypes_InheritedContexts ()
    {
      var finder = new MixedTypeFinder (CreateTypeDiscoveryServiceStub (typeof (DerivedNullTarget)));
      var result = finder.FindMixedTypes (_configuration);

      Assert.That (result, Has.Member (typeof (DerivedNullTarget)));
    }

    [Test]
    public void FindMixedTypes_InheritedContexts_NoTypesMarkedWithIgnoreAttribute ()
    {
      var finder = new MixedTypeFinder (CreateTypeDiscoveryServiceStub (typeof (ClassWithIgnoreAttribute)));
      var result = finder.FindMixedTypes (_configuration);

      Assert.That (result, Has.No.Member (typeof (ClassWithIgnoreAttribute)));
    }

    [Test]
    public void FindMixedTypes_InheritedContexts_NoDuplicates ()
    {
      var finder = new MixedTypeFinder (CreateTypeDiscoveryServiceStub (typeof (NullTarget)));
      var result = finder.FindMixedTypes (_configuration);

      Assert.That (result, Has.Member (typeof (NullTarget)));
    }

    [Test]
    public void FindMixedTypes_InheritedContexts_NoNonInherited ()
    {
      var finder = new MixedTypeFinder (CreateTypeDiscoveryServiceStub (typeof (object)));
      var result = finder.FindMixedTypes (_configuration);

      Assert.That (result, Has.No.Member (null));
      Assert.That (result, Has.No.Member (typeof (object)));
    }

    [Test]
    public void FindMixedTypes_InheritedContexts_NoGenerics ()
    {
      var finder = new MixedTypeFinder (CreateTypeDiscoveryServiceStub (typeof (GenericDerivedNullTarget<>)));
      var result = finder.FindMixedTypes (_configuration);

      Assert.That (result, Has.No.Member (typeof (GenericDerivedNullTarget<>)));
    }

    [Test]
    public void FindMixedTypes_InheritedContexts_NoInterfaces ()
    {
      var finder = new MixedTypeFinder (CreateTypeDiscoveryServiceStub (typeof (IDerivedIBaseType2)));
      var result = finder.FindMixedTypes (_configuration);

      Assert.That (result, Has.No.Member (typeof (IDerivedIBaseType2)));
    }

    [Test]
    public void FindMixedTypes_NoMixedTypes ()
    {
      var generatedType = TypeGenerationHelper.ForceTypeGeneration (typeof (object));
      var typeDiscoveryServiceStub = CreateTypeDiscoveryServiceStub (generatedType);

      var finder = new MixedTypeFinder (typeDiscoveryServiceStub);
      var result = finder.FindMixedTypes (_configuration).ToArray ();

      Assert.That (result, Is.Empty);
    }

    private ITypeDiscoveryService CreateTypeDiscoveryServiceStub (params Type[] stubResult)
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (stubResult);
      return typeDiscoveryServiceStub;
    }
  }
}
