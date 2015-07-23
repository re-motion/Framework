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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectReferenceDataSourceControlTests
{
  [TestFixture]
  public class Validate : BocTest
  {
    private IBusinessObjectReferenceProperty _referencePropertyStub;
    private IBusinessObjectDataSource _referencedDataSourceStub;
    private BusinessObjectReferenceDataSourceControl _dataSourceControl;

    public override void SetUp ()
    {
      base.SetUp();

      _referencedDataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource> ();
      _referencedDataSourceStub.BusinessObject = MockRepository.GenerateStub<IBusinessObject> ();
      _referencedDataSourceStub.Mode = DataSourceMode.Edit;
      _referencePropertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty> ();
      _referencePropertyStub.Stub (stub => stub.ReferenceClass).Return (MockRepository.GenerateStub<IBusinessObjectClass> ());
      _referencePropertyStub.Stub (stub => stub.ReflectedClass).Return (MockRepository.GenerateStub<IBusinessObjectClass> ());

      _dataSourceControl = new BusinessObjectReferenceDataSourceControl ();
      _dataSourceControl.DataSource = _referencedDataSourceStub;
      _dataSourceControl.Property = _referencePropertyStub;
      _dataSourceControl.BusinessObject = MockRepository.GenerateStub<IBusinessObject> ();

      Assert.That (_dataSourceControl.IsReadOnly, Is.False);
    }
    
    [Test]
    public void NoBoundControls_ReturnsTrue ()
    {
      Assert.That (_dataSourceControl.Validate (), Is.True);
    }

    [Test]
    public void AllBoundControlsValid_ReturnsTrue ()
    {
      var firstControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl>();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (true);
      ((IValidatableControl) firstControlStub).Stub (stub => stub.Validate ()).Return (true);
      _dataSourceControl.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (true);
      ((IValidatableControl) secondControlStub).Stub (stub => stub.Validate ()).Return (true);
      _dataSourceControl.Register (secondControlStub);

      Assert.That (_dataSourceControl.Validate (), Is.True);
    }

    [Test]
    public void NotAllBoundControlsValid_ReturnsFalse ()
    {
      var firstControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (true);
      ((IValidatableControl) firstControlStub).Stub (stub => stub.Validate ()).Return (true);
      _dataSourceControl.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (true);
      ((IValidatableControl) secondControlStub).Stub (stub => stub.Validate ()).Return (false);
      _dataSourceControl.Register (secondControlStub);

      var thirdControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      thirdControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      thirdControlStub.Stub (stub => stub.HasValue).Return (true);
      ((IValidatableControl) thirdControlStub).Stub (stub => stub.Validate ()).Return (true);
      _dataSourceControl.Register (thirdControlStub);

      Assert.That (_dataSourceControl.Validate (), Is.False);
    }

#pragma warning disable 612,618
    [Test]
    public void SupportsDefaultValue_True_AllBoundControlsEmpty_NotAllBoundControlsValid_ReturnsTrue ()
    {
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments().Return (true);

      var firstControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (false);
      ((IValidatableControl) firstControlStub).Stub (stub => stub.Validate ()).Return (true);
      _dataSourceControl.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (false);
      ((IValidatableControl) secondControlStub).Stub (stub => stub.Validate ()).Return (false);
      _dataSourceControl.Register (secondControlStub);

      Assert.That (_dataSourceControl.Validate (), Is.True);
    }
#pragma warning restore 612,618

#pragma warning disable 612,618
    [Test]
    public void IsRequired_True_AllBoundControlsEmpty_NotAllBoundControlsValid_ReturnsFalse ()
    {
      _dataSourceControl.Required = true;

      var firstControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (false);
      ((IValidatableControl) firstControlStub).Stub (stub => stub.Validate ()).Return (true);
      _dataSourceControl.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateMock<IBusinessObjectBoundControl, IValidatableControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (false);
      ((IValidatableControl) secondControlStub).Stub (stub => stub.Validate ()).Return (false);
      _dataSourceControl.Register (secondControlStub);

      Assert.That (_dataSourceControl.Validate (), Is.False);

      _referencePropertyStub.AssertWasNotCalled (stub => stub.SupportsDefaultValue);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments());
    }
#pragma warning restore 612,618
  }
}