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
using Moq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Design;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Logging;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class DesignTime : TestBase
  {
    private BindableObjectDataSource _dataSource;
    private Mock<ISite> _stubSite;
    private Mock<IDesignerHost> _mockDesignerHost;
    private Mock<ITypeResolutionService> _typeResolutionServiceMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _stubSite = new Mock<ISite>();
      _stubSite.Setup (_ => _.DesignMode).Returns (true);
      _dataSource.Site = _stubSite.Object;

      _mockDesignerHost = new Mock<IDesignerHost> (MockBehavior.Strict);
      _stubSite.Setup (_ => _.GetService (typeof (IDesignerHost))).Returns (_mockDesignerHost.Object);

      var helperStub = new Mock<IDesignModeHelper>();
      helperStub.Setup (_ => _.DesignerHost).Returns (_mockDesignerHost.Object);

      _typeResolutionServiceMock = new Mock<ITypeResolutionService> (MockBehavior.Strict);

      _mockDesignerHost.Setup (_ => _.GetService (typeof (ITypeResolutionService))).Returns (_typeResolutionServiceMock.Object);

      var typeDiscoveryServiceStub = new Mock<ITypeDiscoveryService>();
      typeDiscoveryServiceStub.Setup (_ => _.GetTypes (It.IsAny<Type>(), It.IsAny<bool>())).Returns (Assembly.GetExecutingAssembly ().GetTypes ());
      _mockDesignerHost.Setup (_ => _.GetService (typeof (ITypeDiscoveryService))).Returns (typeDiscoveryServiceStub.Object);

      // initialize IoC and mixin infrastructure to remove sideeffects in test.
      MixinTypeUtility.GetConcreteMixedType (typeof (SimpleBusinessObjectClass));

      DesignerUtility.SetDesignMode (helperStub.Object);
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
      _typeResolutionServiceMock.Setup (
              _ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests", true))
          .Returns (typeof (SimpleBusinessObjectClass))
          .Verifiable();

      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.Type = typeof (SimpleBusinessObjectClass);
      Assert.That (_dataSource.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));

      _mockDesignerHost.Verify();
      _typeResolutionServiceMock.Verify();
    }

    [Test]
    public void GetType_WithNull ()
    {
      _dataSource.Type = null;
      Assert.That (_dataSource.Type, Is.Null);

      _mockDesignerHost.Verify();
      _typeResolutionServiceMock.Verify();
    }

    [Test]
    public void GetBusinessObjectClass ()
    {
      _typeResolutionServiceMock.Setup (
              _ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests", true))
          .Returns (typeof (SimpleBusinessObjectClass))
          .Verifiable();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass))));

      _mockDesignerHost.Verify();
      _typeResolutionServiceMock.Verify (_ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests", true), Times.AtLeastOnce());
    }

    [Test]
    public void GetBusinessObjectClass_SameTwice ()
    {
      _typeResolutionServiceMock.Setup (
              _ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests", true))
          .Returns (typeof (SimpleBusinessObjectClass))
          .Verifiable();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual, Is.SameAs (_dataSource.BusinessObjectClass));

      _mockDesignerHost.Verify();
      _typeResolutionServiceMock.Verify();
    }

    [Test]
    public void GetBusinessObjectClass_WithNonBindableType ()
    {
      _typeResolutionServiceMock.Setup (
              _ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass, Remotion.ObjectBinding.UnitTests", true))
          .Returns (typeof (StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass))
          .Verifiable();

      _dataSource.Type = typeof (StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass);

      Assert.That (
          () => _dataSource.BusinessObjectClass,
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo (
                  "The type 'Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass' is not a bindable object implementation. "
                  +"It must either have a mixin derived from BindableObjectMixinBase<T> applied "
                  +"or implement the IBusinessObject interface and apply the BindableObjectBaseClassAttribute.", "type"));
      _mockDesignerHost.Verify();
      _typeResolutionServiceMock.Verify (_ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass, Remotion.ObjectBinding.UnitTests", true), Times.AtLeastOnce());
    }

    [Test]
    public void GetBusinessObjectClass_WithTypeDerivedFromBaseClass ()
    {
      _typeResolutionServiceMock.Setup (
              _ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.ClassDerivedFromBindableObjectBase, Remotion.ObjectBinding.UnitTests", true))
          .Returns (typeof (ClassDerivedFromBindableObjectBase))
          .Verifiable();

      _dataSource.Type = typeof (ClassDerivedFromBindableObjectBase);
      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectBase))));

      _mockDesignerHost.Verify();
      _typeResolutionServiceMock.Verify (_ => _.GetType (
              "Remotion.ObjectBinding.UnitTests.TestDomain.ClassDerivedFromBindableObjectBase, Remotion.ObjectBinding.UnitTests", true), Times.AtLeastOnce());
    }
  }
}
