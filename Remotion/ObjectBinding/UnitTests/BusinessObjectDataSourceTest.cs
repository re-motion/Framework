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

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectDataSourceTest
  {
    private IBusinessObjectDataSource _dataSource;

    [SetUp]
    public void SetUp ()
    {
      _dataSource = new StubBusinessObjectDataSource(null);
    }

    [Test]
    public void GetBusinessObjectProvider ()
    {
      var stubBusinessObjectClass = new Mock<IBusinessObjectClass>();
      var stubBusinessObjectProvider = new Mock<IBusinessObjectProvider>();
      var dataSource = new StubBusinessObjectDataSource(stubBusinessObjectClass.Object);

      stubBusinessObjectClass.Setup(_ => _.BusinessObjectProvider).Returns(stubBusinessObjectProvider.Object);

      Assert.That(dataSource.BusinessObjectProvider, Is.SameAs(stubBusinessObjectProvider.Object));
    }

    [Test]
    public void GetBusinessObjectProvider_WithoutBusinessObjectClass ()
    {
      _dataSource = new StubBusinessObjectDataSource(null);
      Assert.That(_dataSource.BusinessObjectProvider, Is.Null);
    }

    [Test]
    public void RegisterAndGetBoundControls ()
    {
      var stubControl1 = new Mock<IBusinessObjectBoundControl>();
      var stubControl2 = new Mock<IBusinessObjectBoundControl>();

      _dataSource.Register(stubControl1.Object);
      _dataSource.Register(stubControl2.Object);
      Assert.That(_dataSource.GetAllBoundControls(), Is.EqualTo(new[] { stubControl1.Object, stubControl2.Object }));
    }

    [Test]
    public void Unregister ()
    {
      var stubControl1 = new Mock<IBusinessObjectBoundControl>();
      var stubControl2 = new Mock<IBusinessObjectBoundControl>();

      _dataSource.Register(stubControl1.Object);
      _dataSource.Register(stubControl2.Object);
      Assert.That(_dataSource.GetAllBoundControls(), Is.EqualTo(new[] { stubControl1.Object, stubControl2.Object }));

      _dataSource.Unregister(stubControl1.Object);
      Assert.That(_dataSource.GetAllBoundControls(), Is.EqualTo(new[] { stubControl2.Object }));

      _dataSource.Unregister(stubControl2.Object);
      Assert.That(_dataSource.GetAllBoundControls(), Is.EqualTo(new IBusinessObjectBoundControl[0]));
    }

    [Test]
    public void Register_SameTwice ()
    {
      var stubControl1 = new Mock<IBusinessObjectBoundControl>();
      var stubControl2 = new Mock<IBusinessObjectBoundControl>();

      stubControl1.Setup(_ => _.HasValidBinding).Returns(true);
      stubControl2.Setup(_ => _.HasValidBinding).Returns(true);

      _dataSource.Register(stubControl1.Object);
      _dataSource.Register(stubControl2.Object);
      _dataSource.Register(stubControl1.Object);
      Assert.That(_dataSource.GetAllBoundControls(), Is.EqualTo(new[] { stubControl1.Object, stubControl2.Object }));
    }

    [Test]
    public void Unegister_SameTwice ()
    {
      var stubControl1 = new Mock<IBusinessObjectBoundControl>();
      var stubControl2 = new Mock<IBusinessObjectBoundControl>();

      stubControl1.Setup(_ => _.HasValidBinding).Returns(true);
      stubControl2.Setup(_ => _.HasValidBinding).Returns(true);

      _dataSource.Register(stubControl1.Object);
      _dataSource.Register(stubControl2.Object);
      Assert.That(_dataSource.GetAllBoundControls(), Is.EqualTo(new[] { stubControl1.Object, stubControl2.Object }));

      _dataSource.Unregister(stubControl1.Object);
      _dataSource.Unregister(stubControl1.Object);
      Assert.That(_dataSource.GetAllBoundControls(), Is.EqualTo(new[] { stubControl2.Object }));
    }

    [Test]
    public void LoadValues ()
    {
      var mockControl1 = new Mock<IBusinessObjectBoundControl>(MockBehavior.Strict);
      var mockControl2 = new Mock<IBusinessObjectBoundControl>(MockBehavior.Strict);

      mockControl1.Setup(_ => _.HasValidBinding).Returns(true);
      mockControl2.Setup(_ => _.HasValidBinding).Returns(true);
      mockControl1.Setup(_ => _.LoadValue(true));
      mockControl2.Setup(_ => _.LoadValue(true));

      _dataSource.Register(mockControl1.Object);
      _dataSource.Register(mockControl2.Object);
      _dataSource.LoadValues(true);

      mockControl1.Verify();
      mockControl2.Verify();
    }

    [Test]
    public void SaveValues ()
    {
      var mockControl1 = new Mock<IBusinessObjectBoundEditableControl>(MockBehavior.Strict);
      var mockControl2 = new Mock<IBusinessObjectBoundEditableControl>(MockBehavior.Strict);

      mockControl1.Setup(_ => _.HasValidBinding).Returns(true);
      mockControl2.Setup(_ => _.HasValidBinding).Returns(true);
      mockControl1.Setup(_ => _.SaveValue(true)).Returns(true);
      mockControl2.Setup(_ => _.SaveValue(true)).Returns(true);

      _dataSource.Register(mockControl1.Object);
      _dataSource.Register(mockControl2.Object);
      var result = _dataSource.SaveValues(true);

      Assert.That(result, Is.True);
      mockControl1.Verify();
      mockControl2.Verify();
    }

    [Test]
    public void SaveValues_WithoutControls_ReturnsTrue ()
    {
      var result = _dataSource.SaveValues(true);

      Assert.That(result, Is.True);
    }

    [Test]
    public void SaveValues_WithNotAllControlsSavingTheValue_ReturnsFalse ()
    {
      var mockControl1 = new Mock<IBusinessObjectBoundEditableControl>(MockBehavior.Strict);
      var mockControl2 = new Mock<IBusinessObjectBoundEditableControl>(MockBehavior.Strict);

      mockControl1.Setup(_ => _.HasValidBinding).Returns(true);
      mockControl2.Setup(_ => _.HasValidBinding).Returns(true);
      mockControl1.Setup(_ => _.SaveValue(true)).Returns(false);
      mockControl2.Setup(_ => _.SaveValue(true)).Returns(true);

      _dataSource.Register(mockControl1.Object);
      _dataSource.Register(mockControl2.Object);
      var result = _dataSource.SaveValues(true);

      Assert.That(result, Is.False);
      mockControl1.Verify();
      mockControl2.Verify();
    }

    [Test]
    public void SetAndGetMode ()
    {
      _dataSource.Mode = DataSourceMode.Edit;
      Assert.That(_dataSource.Mode, Is.EqualTo(DataSourceMode.Edit));
    }

    [Test]
    public void GetBoundControlsWithValidBinding ()
    {
      var stubControl1 = new Mock<IBusinessObjectBoundControl>();
      var stubControl2 = new Mock<IBusinessObjectBoundControl>();
      var stubControl3 = new Mock<IBusinessObjectBoundControl>();

      stubControl1.Setup(_ => _.HasValidBinding).Returns(true);
      stubControl2.Setup(_ => _.HasValidBinding).Returns(false);
      stubControl3.Setup(_ => _.HasValidBinding).Returns(true);

      _dataSource.Register(stubControl1.Object);
      _dataSource.Register(stubControl2.Object);
      _dataSource.Register(stubControl3.Object);
      Assert.That(_dataSource.GetBoundControlsWithValidBinding(), Is.EquivalentTo(new[] { stubControl1.Object, stubControl3.Object }));
    }
  }
}