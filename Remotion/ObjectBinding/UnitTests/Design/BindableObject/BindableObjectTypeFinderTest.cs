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
using System.ComponentModel.Design;
using Moq;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Design.BindableObject
{
  [TestFixture]
  public class BindableObjectTypeFinderTest
  {
    private Mock<IServiceProvider> _serviceProvider;
    private Mock<ITypeDiscoveryService> _typeDiscoveryService;

    [SetUp]
    public void SetUp ()
    {
      _serviceProvider = new Mock<IServiceProvider> (MockBehavior.Strict);
      _typeDiscoveryService = new Mock<ITypeDiscoveryService> (MockBehavior.Strict);
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_IncludeGac ()
    {
      _serviceProvider.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns (_typeDiscoveryService.Object).Verifiable();
      _typeDiscoveryService.Setup (_ => _.GetTypes (typeof (object), false))
          .Returns (
          new object[]
              {
                  typeof (ClassWithAllDataTypes),
                  typeof (SimpleValueType),
                  typeof (SimpleReferenceType),
                  typeof (ManualBusinessObject),
                  typeof (ClassWithIdentity),
                  typeof (ClassDerivedFromBindableObjectBase),
                  typeof (ClassDerivedFromBindableObjectWithIdentityBase),
              })
          .Verifiable();

      var finder = new BindableObjectTypeFinder (_serviceProvider.Object);
      List<Type> types = finder.GetTypes (true);

      Assert.That (types, Is.EquivalentTo (new[]
                                             {
                                                 typeof (ClassWithAllDataTypes),
                                                 typeof (ClassWithIdentity),
                                                 typeof (ClassDerivedFromBindableObjectBase),
                                                 typeof (ClassDerivedFromBindableObjectWithIdentityBase)
                                             }));

      _serviceProvider.Verify();
      _typeDiscoveryService.Verify();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_NotIncludeGac ()
    {
      _serviceProvider.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns (_typeDiscoveryService.Object).Verifiable();
      _typeDiscoveryService.Setup (_ => _.GetTypes (typeof (object), true))
          .Returns (
          new object[]
          {
              typeof (ClassWithAllDataTypes),
              typeof (SimpleValueType),
              typeof (SimpleReferenceType),
              typeof (ManualBusinessObject),
              typeof (ClassDerivedFromBindableObjectBase),
              typeof (ClassDerivedFromBindableObjectWithIdentityBase),
          })
          .Verifiable();

      var finder = new BindableObjectTypeFinder (_serviceProvider.Object);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.EquivalentTo (new[]
                                             {
                                                 typeof (ClassWithAllDataTypes),
                                                 typeof (ClassDerivedFromBindableObjectBase),
                                                 typeof (ClassDerivedFromBindableObjectWithIdentityBase),
                                             }));

      _serviceProvider.Verify();
      _typeDiscoveryService.Verify();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_IgnoresActiveMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        _serviceProvider.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns (_typeDiscoveryService.Object).Verifiable();
        _typeDiscoveryService.Setup (_ => _.GetTypes (typeof (object), true))
            .Returns (
            new object[]
                {
                    typeof (ClassWithAllDataTypes),
                    typeof (SimpleValueType),
                    typeof (SimpleReferenceType),
                    typeof (ManualBusinessObject),
                    typeof (ClassDerivedFromBindableObjectBase),
                    typeof (ClassDerivedFromBindableObjectWithIdentityBase),
                })
            .Verifiable();

        var finder = new BindableObjectTypeFinder (_serviceProvider.Object);
        List<Type> types = finder.GetTypes (false);

        Assert.That (types,
                     Is.EquivalentTo (new[]
                                        {
                                            typeof (ClassWithAllDataTypes),
                                            typeof (ClassDerivedFromBindableObjectBase),
                                            typeof (ClassDerivedFromBindableObjectWithIdentityBase),
                                        }));

        _serviceProvider.Verify();
        _typeDiscoveryService.Verify();
      }
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_GetsTypeInheritingMixinFromBase ()
    {
      _serviceProvider.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns (_typeDiscoveryService.Object).Verifiable();
      _typeDiscoveryService.Setup (_ => _.GetTypes (typeof (object), true))
          .Returns (
          new object[]
              {
                  typeof (DerivedBusinessObjectClassWithoutAttribute)
              })
          .Verifiable();

      var finder = new BindableObjectTypeFinder (_serviceProvider.Object);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.EquivalentTo (new[] { typeof (DerivedBusinessObjectClassWithoutAttribute) }));

      _serviceProvider.Verify();
      _typeDiscoveryService.Verify();
    }

    [Test]
    public void GetTypes_WithoutTypeDiscoveryService ()
    {
      _serviceProvider.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns ((object) null).Verifiable();

      var finder = new BindableObjectTypeFinder (_serviceProvider.Object);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.Empty);

      _serviceProvider.Verify();
      _typeDiscoveryService.Verify();
    }

    [Test]
    public void GetMixinConfiguration_IncludeGac ()
    {
      _serviceProvider.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns (_typeDiscoveryService.Object).Verifiable();
      _typeDiscoveryService.Setup (_ => _.GetTypes (typeof (object), false))
          .Returns (
          new object[]
              {
                  typeof (DerivedBusinessObjectClassWithoutAttribute),
                  typeof (SimpleBusinessObjectClass),
                  typeof (ClassWithIdentity),
                  typeof (ManualBusinessObject),
              })
          .Verifiable();

      var finder = new BindableObjectTypeFinder (_serviceProvider.Object);
      MixinConfiguration configuration = finder.GetMixinConfiguration (true);
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (3));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (BaseBusinessObjectClass)));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (DerivedBusinessObjectClassWithoutAttribute)), Is.False);
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (SimpleBusinessObjectClass)));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (ClassWithIdentity)));

      Assert.That (configuration.GetContext (typeof (BaseBusinessObjectClass)).Mixins.ContainsKey (typeof (BindableObjectMixin)));
      Assert.That (configuration.GetContext (typeof (ClassWithIdentity)).Mixins.ContainsKey (typeof (BindableObjectWithIdentityMixin)));

      _serviceProvider.Verify();
      _typeDiscoveryService.Verify();
    }

    [Test]
    public void GetMixinConfiguration_NotIncludeGac ()
    {
      _serviceProvider.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns (_typeDiscoveryService.Object).Verifiable();
      _typeDiscoveryService.Setup (_ => _.GetTypes (typeof (object), true)).Returns (new object[0]).Verifiable();

      var finder = new BindableObjectTypeFinder (_serviceProvider.Object);
      finder.GetMixinConfiguration (false);

      _serviceProvider.Verify();
      _typeDiscoveryService.Verify();
    }
  }
}
