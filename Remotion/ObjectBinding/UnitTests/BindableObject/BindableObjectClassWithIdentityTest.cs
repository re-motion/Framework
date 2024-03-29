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
using Moq;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectClassWithIdentityTest : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = new BindableObjectProvider();
      _bindableObjectGlobalizationService = SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>();
    }

    [Test]
    public void Initialize ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity(
          MixinTypeUtility.GetConcreteMixedType(typeof(ClassWithIdentity)),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);

      Assert.That(bindableObjectClass.TargetType, Is.SameAs(typeof(ClassWithIdentity)));
      Assert.That(
          bindableObjectClass.Identifier,
          Is.EqualTo("Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithIdentity, Remotion.ObjectBinding.UnitTests"));
      Assert.That(bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That(bindableObjectClass.BusinessObjectProvider, Is.SameAs(_bindableObjectProvider));
    }

    [Test]
    public void GetObject_WithDefaultService ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity(
          MixinTypeUtility.GetConcreteMixedType(typeof(ClassWithIdentity)),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);
      var mockService = new Mock<IGetObjectService>(MockBehavior.Strict);
      var expected = new Mock<IBusinessObjectWithIdentity>();

      mockService.Setup(_ => _.GetObject(bindableObjectClass, "TheUniqueIdentifier")).Returns(expected.Object).Verifiable();

      _bindableObjectProvider.AddService(typeof(IGetObjectService), mockService.Object);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject("TheUniqueIdentifier");

      mockService.Verify();
      Assert.That(actual, Is.SameAs(expected.Object));
    }

    [Test]
    public void GetObject_WithCustomService ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity(
          MixinTypeUtility.GetConcreteMixedType(typeof(ClassWithIdentityAndGetObjectServiceAttribute)),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);
      var mockService = new Mock<ICustomGetObjectService>(MockBehavior.Strict);
      var expected = new Mock<IBusinessObjectWithIdentity>();

      mockService.Setup(_ => _.GetObject(bindableObjectClass, "TheUniqueIdentifier")).Returns(expected.Object).Verifiable();

      _bindableObjectProvider.AddService(typeof(ICustomGetObjectService), mockService.Object);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject("TheUniqueIdentifier");

      mockService.Verify();
      Assert.That(actual, Is.SameAs(expected.Object));
    }

    [Test]
    public void GetObject_WithoutService ()
    {
      var bindableObjectClass = new BindableObjectClassWithIdentity(
          MixinTypeUtility.GetConcreteMixedType(typeof(ClassWithIdentity)),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);
      Assert.That(
          () => bindableObjectClass.GetObject("TheUniqueIdentifier"),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The 'Remotion.ObjectBinding.BindableObject.IGetObjectService' required for loading objectes of type "
                  + "'Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithIdentity' is not registered with the "
                  + "'Remotion.ObjectBinding.BusinessObjectProvider' associated with this type."));
    }
  }
}
