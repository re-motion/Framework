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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Design.BindableObject
{
  [TestFixture]
  public class BindableObjectTypeFinderTest
  {
    private MockRepository _mockRepository;
    private IServiceProvider _serviceProvider;
    private ITypeDiscoveryService _typeDiscoveryService;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _serviceProvider = _mockRepository.StrictMock<IServiceProvider> ();
      _typeDiscoveryService = _mockRepository.StrictMock<ITypeDiscoveryService> ();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_IncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), false))
          .Return (
          new object[]
              {
                  typeof (ClassWithAllDataTypes),
                  typeof (SimpleValueType),
                  typeof (SimpleReferenceType),
                  typeof (ManualBusinessObject),
                  typeof (ClassWithIdentity),
                  typeof (ClassDerivedFromBindableObjectBase),
                  typeof (ClassDerivedFromBindableObjectWithIdentityBase),
              });

      _mockRepository.ReplayAll ();

      var finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (true);

      Assert.That (types, Is.EquivalentTo (new[]
                                             {
                                                 typeof (ClassWithAllDataTypes),
                                                 typeof (ClassWithIdentity),
                                                 typeof (ClassDerivedFromBindableObjectBase),
                                                 typeof (ClassDerivedFromBindableObjectWithIdentityBase)
                                             }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_NotIncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true))
          .Return (
          new object[]
          {
              typeof (ClassWithAllDataTypes),
              typeof (SimpleValueType),
              typeof (SimpleReferenceType),
              typeof (ManualBusinessObject),
              typeof (ClassDerivedFromBindableObjectBase),
              typeof (ClassDerivedFromBindableObjectWithIdentityBase),
          });

      _mockRepository.ReplayAll();

      var finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.EquivalentTo (new[]
                                             {
                                                 typeof (ClassWithAllDataTypes),
                                                 typeof (ClassDerivedFromBindableObjectBase),
                                                 typeof (ClassDerivedFromBindableObjectWithIdentityBase),
                                             }));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_IgnoresActiveMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
        Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true))
            .Return (
            new object[]
                {
                    typeof (ClassWithAllDataTypes),
                    typeof (SimpleValueType),
                    typeof (SimpleReferenceType),
                    typeof (ManualBusinessObject),
                    typeof (ClassDerivedFromBindableObjectBase),
                    typeof (ClassDerivedFromBindableObjectWithIdentityBase),
                });

        _mockRepository.ReplayAll();

        var finder = new BindableObjectTypeFinder (_serviceProvider);
        List<Type> types = finder.GetTypes (false);

        Assert.That (types,
                     Is.EquivalentTo (new[]
                                        {
                                            typeof (ClassWithAllDataTypes),
                                            typeof (ClassDerivedFromBindableObjectBase),
                                            typeof (ClassDerivedFromBindableObjectWithIdentityBase),
                                        }));

        _mockRepository.VerifyAll();
      }
    }

    [Test]
    public void GetTypes_WithTypeDiscoveryService_GetsTypeInheritingMixinFromBase ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true))
          .Return (
          new object[]
              {
                  typeof (DerivedBusinessObjectClassWithoutAttribute)
              });

      _mockRepository.ReplayAll ();

      var finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.EquivalentTo (new[] { typeof (DerivedBusinessObjectClassWithoutAttribute) }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithoutTypeDiscoveryService ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (null);

      _mockRepository.ReplayAll ();

      var finder = new BindableObjectTypeFinder (_serviceProvider);
      List<Type> types = finder.GetTypes (false);

      Assert.That (types, Is.Empty);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetMixinConfiguration_IncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), false))
          .Return (
          new object[]
              {
                  typeof (DerivedBusinessObjectClassWithoutAttribute),
                  typeof (SimpleBusinessObjectClass),
                  typeof (ClassWithIdentity),
                  typeof (ManualBusinessObject),
              });

      _mockRepository.ReplayAll ();

      var finder = new BindableObjectTypeFinder (_serviceProvider);
      MixinConfiguration configuration = finder.GetMixinConfiguration (true);
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (3));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (BaseBusinessObjectClass)));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (DerivedBusinessObjectClassWithoutAttribute)), Is.False);
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (SimpleBusinessObjectClass)));
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (ClassWithIdentity)));

      Assert.That (configuration.GetContext (typeof (BaseBusinessObjectClass)).Mixins.ContainsKey (typeof (BindableObjectMixin)));
      Assert.That (configuration.GetContext (typeof (ClassWithIdentity)).Mixins.ContainsKey (typeof (BindableObjectWithIdentityMixin)));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetMixinConfiguration_NotIncludeGac ()
    {
      Expect.Call (_serviceProvider.GetService (typeof (ITypeDiscoveryService))).Return (_typeDiscoveryService);
      Expect.Call (_typeDiscoveryService.GetTypes (typeof (object), true)).Return (new object[0]);

      _mockRepository.ReplayAll ();

      var finder = new BindableObjectTypeFinder (_serviceProvider);
      finder.GetMixinConfiguration (false);

      _mockRepository.VerifyAll ();
    }
  }
}
