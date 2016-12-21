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
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectBoundEditableWebControlTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectBoundEditableWebControlTests
{
  [TestFixture]
  public class SaveValueToDomainModel
  {
    private TestableBusinessObjectBoundEditableWebControl _control;
    private IBusinessObjectClass _businessObjectClassStub;
    private IBusinessObjectProperty _propertyStub;
    private IBusinessObjectProperty _readOnlyPropertyStub;
    private IBusinessObjectDataSource _dataSourceStub;
    private IBusinessObject _businessObjectStub;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      _businessObjectClassStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      _dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      _dataSourceStub.BusinessObject = _businessObjectStub;
      _dataSourceStub.Mode = DataSourceMode.Edit;
      _dataSourceStub.Stub (_ => _.BusinessObjectClass).Return (_businessObjectClassStub);
      _propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      _propertyStub.Stub (_ => _.ReflectedClass).Return (_businessObjectClassStub);
      _readOnlyPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      _readOnlyPropertyStub.Stub (_ => _.ReflectedClass).Return (_businessObjectClassStub);
      _readOnlyPropertyStub.Stub (stub => stub.IsReadOnly (Arg<IBusinessObject>.Is.Anything)).Return (true);
      _control = new TestableBusinessObjectBoundEditableWebControl();
    }

    [Test]
    public void SaveValue ()
    {
      _control.DataSource = _dataSourceStub;
      _control.Property = _propertyStub;
      _control.Value = "test";

      Assert.That (_control.SaveValueToDomainModel(), Is.True);
      _dataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_propertyStub, "test"));
    }

    [Test]
    public void SaveValueAndDataSourceNull ()
    {
      _control.DataSource = null;
      _control.Property = _propertyStub;
      _control.Value = null;

      Assert.That (_control.SaveValueToDomainModel(), Is.False);
      _dataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments());
    }

    [Test]
    public void SaveValueAndPropertyNull ()
    {
      _control.DataSource = _dataSourceStub;
      _control.Property = null;
      _control.Value = null;

      Assert.That (_control.SaveValueToDomainModel(), Is.False);
      _dataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments());
    }

    [Test]
    public void SaveValueAndBusinessObjectNullAndValueNotNull ()
    {
      _dataSourceStub.BusinessObject = null;
      _control.DataSource = _dataSourceStub;
      _control.Property = _propertyStub;
      _control.Value = "value";

      Assert.That (_control.SaveValueToDomainModel(), Is.False);
      _businessObjectStub.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments());
    }

    [Test]
    public void SaveValueAndBusinessObjectNullAndValueNull ()
    {
      _dataSourceStub.BusinessObject = null;
      _control.DataSource = _dataSourceStub;
      _control.Property = _propertyStub;
      _control.Value = null;

      Assert.That (_control.SaveValueToDomainModel(), Is.True);
      _businessObjectStub.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments());
    }

    [Test]
    public void SaveValueAndControlIsReadOnly ()
    {
      _control.DataSource = _dataSourceStub;
      _control.Property = _propertyStub;
      _control.Value = "test";
      _control.ReadOnly = true;

      Assert.That (_control.SaveValueToDomainModel(), Is.True);
      _dataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_propertyStub, "test"));
    }

    [Test]
    public void SaveValueAndPropertyIsReadOnlyInDomainLayer ()
    {
      _control.ID = "TestID";
      _readOnlyPropertyStub.Stub (stub => stub.Identifier).Return ("TestProperty");
      _control.DataSource = _dataSourceStub;
      _control.Property = _readOnlyPropertyStub;
      _control.Value = null;

      Assert.That (
          () => _control.SaveValueToDomainModel(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The value of the TestableBusinessObjectBoundEditableWebControl 'TestID' could not be saved into the domain model "
              + "because the property 'TestProperty' is read only."));
      _dataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments());
    }
  }
}