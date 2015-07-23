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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectDataSourceTest
  {
    private MockRepository _mockRepository;
    private IBusinessObjectDataSource _dataSource;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _dataSource = new StubBusinessObjectDataSource (null);
    }

    [Test]
    public void GetBusinessObjectProvider ()
    {
      var stubBusinessObjectClass = _mockRepository.Stub<IBusinessObjectClass>();
      var stubBusinessObjectProvider = _mockRepository.Stub<IBusinessObjectProvider>();
      var dataSource = new StubBusinessObjectDataSource (stubBusinessObjectClass);

      SetupResult.For (stubBusinessObjectClass.BusinessObjectProvider).Return (stubBusinessObjectProvider);
      _mockRepository.ReplayAll();

      Assert.That (dataSource.BusinessObjectProvider, Is.SameAs (stubBusinessObjectProvider));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectProvider_WithoutBusinessObjectClass ()
    {
      _dataSource = new StubBusinessObjectDataSource (null);
      Assert.That (_dataSource.BusinessObjectProvider, Is.Null);
    }

    [Test]
    public void RegisterAndGetBoundControls ()
    {
      var stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl>();
      var stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl>();

      _mockRepository.ReplayAll();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      Assert.That (_dataSource.GetAllBoundControls(), Is.EqualTo (new[] { stubControl1, stubControl2 }));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Unregister ()
    {
      var stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl>();
      var stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl>();

      _mockRepository.ReplayAll();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      Assert.That (_dataSource.GetAllBoundControls(), Is.EqualTo (new[] { stubControl1, stubControl2 }));

      _dataSource.Unregister (stubControl1);
      Assert.That (_dataSource.GetAllBoundControls(), Is.EqualTo (new[] { stubControl2 }));

      _dataSource.Unregister (stubControl2);
      Assert.That (_dataSource.GetAllBoundControls(), Is.EqualTo (new IBusinessObjectBoundControl[0]));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Register_SameTwice ()
    {
      var stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl>();
      var stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl>();

      SetupResult.For (stubControl1.HasValidBinding).Return (true);
      SetupResult.For (stubControl2.HasValidBinding).Return (true);
      _mockRepository.ReplayAll();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      _dataSource.Register (stubControl1);
      Assert.That (_dataSource.GetAllBoundControls(), Is.EqualTo (new[] { stubControl1, stubControl2 }));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Unegister_SameTwice ()
    {
      var stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl>();
      var stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl>();

      SetupResult.For (stubControl1.HasValidBinding).Return (true);
      SetupResult.For (stubControl2.HasValidBinding).Return (true);
      _mockRepository.ReplayAll();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      Assert.That (_dataSource.GetAllBoundControls(), Is.EqualTo (new[] { stubControl1, stubControl2 }));

      _dataSource.Unregister (stubControl1);
      _dataSource.Unregister (stubControl1);
      Assert.That (_dataSource.GetAllBoundControls(), Is.EqualTo (new[] { stubControl2 }));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadValues ()
    {
      var mockControl1 = _mockRepository.StrictMock<IBusinessObjectBoundControl>();
      var mockControl2 = _mockRepository.StrictMock<IBusinessObjectBoundControl>();

      SetupResult.For (mockControl1.HasValidBinding).Return (true);
      SetupResult.For (mockControl2.HasValidBinding).Return (true);
      mockControl1.LoadValue (true);
      mockControl2.LoadValue (true);
      _mockRepository.ReplayAll();

      _dataSource.Register (mockControl1);
      _dataSource.Register (mockControl2);
      _dataSource.LoadValues (true);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void SaveValues ()
    {
      var mockControl1 = _mockRepository.StrictMock<IBusinessObjectBoundEditableControl>();
      var mockControl2 = _mockRepository.StrictMock<IBusinessObjectBoundEditableControl>();

      SetupResult.For (mockControl1.HasValidBinding).Return (true);
      SetupResult.For (mockControl2.HasValidBinding).Return (true);
      SetupResult.For (mockControl1.SaveValue (true)).Return (true);
      SetupResult.For (mockControl2.SaveValue (true)).Return (true);
      _mockRepository.ReplayAll();

      _dataSource.Register (mockControl1);
      _dataSource.Register (mockControl2);
      var result = _dataSource.SaveValues (true);

      Assert.That (result, Is.True);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void SaveValues_WithoutControls_ReturnsTrue ()
    {
      var result = _dataSource.SaveValues (true);

      Assert.That (result, Is.True);
    }

    [Test]
    public void SaveValues_WithNotAllControlsSavingTheValue_ReturnsFalse ()
    {
      var mockControl1 = _mockRepository.StrictMock<IBusinessObjectBoundEditableControl>();
      var mockControl2 = _mockRepository.StrictMock<IBusinessObjectBoundEditableControl>();

      SetupResult.For (mockControl1.HasValidBinding).Return (true);
      SetupResult.For (mockControl2.HasValidBinding).Return (true);
      SetupResult.For (mockControl1.SaveValue (true)).Return (false);
      SetupResult.For (mockControl2.SaveValue (true)).Return (true);
      _mockRepository.ReplayAll();

      _dataSource.Register (mockControl1);
      _dataSource.Register (mockControl2);
      var result = _dataSource.SaveValues (true);

      Assert.That (result, Is.False);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void SetAndGetMode ()
    {
      _dataSource.Mode = DataSourceMode.Edit;
      Assert.That (_dataSource.Mode, Is.EqualTo (DataSourceMode.Edit));
    }

    [Test]
    public void GetBoundControlsWithValidBinding ()
    {
      var stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl>();
      var stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl>();
      var stubControl3 = _mockRepository.Stub<IBusinessObjectBoundControl>();

      SetupResult.For (stubControl1.HasValidBinding).Return (true);
      SetupResult.For (stubControl2.HasValidBinding).Return (false);
      SetupResult.For (stubControl3.HasValidBinding).Return (true);
      _mockRepository.ReplayAll();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      _dataSource.Register (stubControl3);
      Assert.That (_dataSource.GetBoundControlsWithValidBinding(), Is.EquivalentTo (new[] { stubControl1, stubControl3 }));

      _mockRepository.VerifyAll();
    }
  }
}