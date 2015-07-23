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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Design;
using Remotion.Development.UnitTesting;
using Remotion.Logging;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class DesignTime : TestBase
  {
    private BindableObjectDataSource _dataSource;
    private MockRepository _mockRepository;
    private ISite _stubSite;
    private IDesignerHost _mockDesignerHost;
    private ITypeResolutionService _typeResolutionServiceMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _mockRepository = new MockRepository();
      _stubSite = _mockRepository.Stub<ISite>();
      SetupResult.For (_stubSite.DesignMode).Return (true);
      _dataSource.Site = _stubSite;

      _mockDesignerHost = _mockRepository.StrictMock<IDesignerHost>();
      SetupResult.For (_stubSite.GetService (typeof (IDesignerHost))).Return (_mockDesignerHost);

      var helperStub = _mockRepository.Stub<IDesignModeHelper> ();
      SetupResult.For (helperStub.DesignerHost).Return (_mockDesignerHost);

      _typeResolutionServiceMock = _mockRepository.StrictMock<ITypeResolutionService> ();
      SetupResult.For (_mockDesignerHost.GetService (typeof (ITypeResolutionService))).Return (_typeResolutionServiceMock);

      var typeDiscoveryServiceStub = _mockRepository.Stub<ITypeDiscoveryService> ();
      SetupResult.For (typeDiscoveryServiceStub.GetTypes (null, false)).IgnoreArguments ().Return (Assembly.GetExecutingAssembly ().GetTypes ());
      SetupResult.For (_mockDesignerHost.GetService (typeof (ITypeDiscoveryService))).Return (typeDiscoveryServiceStub);

      // initialize IoC and mixin infrastructure to remove sideeffects in test.
      MixinTypeUtility.GetConcreteMixedType (typeof (SimpleBusinessObjectClass));

      DesignerUtility.SetDesignMode (helperStub);
    }

    [TearDown]
    public override void TearDown ()
    {
      base.TearDown();
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    public void GetAndSetType ()
    {
      Expect.Call (
          _typeResolutionServiceMock.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests", true))
          .Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll();

      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.Type = typeof (SimpleBusinessObjectClass);
      Assert.That (_dataSource.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetType_WithNull ()
    {
      _mockRepository.ReplayAll();

      _dataSource.Type = null;
      Assert.That (_dataSource.Type, Is.Null);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectClass ()
    {
      Expect.Call (
          _typeResolutionServiceMock.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests", true))
          .Return (typeof (SimpleBusinessObjectClass))
          .Repeat.AtLeastOnce();
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass))));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectClass_SameTwice ()
    {
      SetupResult.For (
          _typeResolutionServiceMock.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests", true))
          .Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual, Is.SameAs (_dataSource.BusinessObjectClass));

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The type 'Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass' is not a bindable object implementation. "
        +"It must either have a mixin derived from BindableObjectMixinBase<T> applied "
        +"or implement the IBusinessObject interface and apply the BindableObjectBaseClassAttribute.\r\nParameter name: type")]
    public void GetBusinessObjectClass_WithNonBindableType ()
    {
      Expect.Call (
          _typeResolutionServiceMock.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass, Remotion.ObjectBinding.UnitTests", true))
          .Return (typeof (StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass))
          .Repeat.AtLeastOnce();
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass);
      Dev.Null = _dataSource.BusinessObjectClass;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectClass_WithTypeDerivedFromBaseClass ()
    {
      Expect.Call (
          _typeResolutionServiceMock.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.ClassDerivedFromBindableObjectBase, Remotion.ObjectBinding.UnitTests", true))
          .Return (typeof (ClassDerivedFromBindableObjectBase))
          .Repeat.AtLeastOnce ();
      _mockRepository.ReplayAll ();

      _dataSource.Type = typeof (ClassDerivedFromBindableObjectBase);
      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectBase))));

      _mockRepository.VerifyAll ();
    }
  }
}
