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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class RunTime:TestBase
  {
    private BindableObjectDataSource _dataSource;
    private BindableObjectProvider _provider;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _provider = new BindableObjectProvider();
      BusinessObjectProvider.SetProvider(typeof(BindableObjectProviderAttribute), _provider);
      BusinessObjectProvider.SetProvider(typeof(BindableObjectWithIdentityProviderAttribute), _provider);
    }

    [Test]
    public void GetAndSetBusinessObject ()
    {
      var businessObject = new Mock<IBusinessObject>();
      ((IBusinessObjectDataSource)_dataSource).BusinessObject = businessObject.Object;
      Assert.That(((IBusinessObjectDataSource)_dataSource).BusinessObject, Is.SameAs(businessObject.Object));
    }

    [Test]
    public void GetAndSetType ()
    {
      Assert.That(_dataSource.Type, Is.Null);
      _dataSource.Type = typeof(SimpleBusinessObjectClass);
      Assert.That(_dataSource.Type, Is.EqualTo(typeof(SimpleBusinessObjectClass)));
    }

    [Test]
    public void GetAndSetType_WithNull ()
    {
      _dataSource.Type = null;
      Assert.That(_dataSource.Type, Is.Null);
    }

    [Test]
    public void GetBusinessObjectClass_WithoutType ()
    {
      Assert.That(_dataSource.Type, Is.Null);
      Assert.That(_dataSource.BusinessObjectClass, Is.Null);
    }

    [Test]
    public void GetBusinessObjectClass_WithValidType ()
    {
      _dataSource.Type = typeof(SimpleBusinessObjectClass);
      Type type = typeof(SimpleBusinessObjectClass);
      ArgumentUtility.CheckNotNull("type", type);
      Assert.That(_dataSource.BusinessObjectClass, Is.SameAs(BindableObjectProviderTestHelper.GetBindableObjectClass(type)));
    }

    [Test]
    public void GetBusinessObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      _dataSource.Type = typeof(StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass);
      Assert.That(
          () => _dataSource.BusinessObjectClass,
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The type 'Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass' is not a bindable object implementation. "
                  + "It must either have a mixin derived from BindableObjectMixinBase<T> applied "
                  + "or implement the IBusinessObject interface and apply the BindableObjectBaseClassAttribute.", "type"));
    }
  }
}
