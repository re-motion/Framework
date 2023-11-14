// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.SmartPageImplementation;

namespace Remotion.Web.UnitTests.Core.UI.SmartPageImplementation
{
  [TestFixture]
  public class SmartPageInfoTest
  {
    private Mock<ISmartPage> _smartPageMock;
    private SmartPageInfo _smartPageInfo;

    [SetUp]
    public void SetUp ()
    {
      _smartPageMock = new Mock<Page>().As<ISmartPage>();
      _smartPageInfo = new SmartPageInfo(_smartPageMock.Object);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesNull_AndDirtyStateEnabledIsTrue_AndPageIsDirtyIsFalse_AndNoTrackedControls_ReturnsEmptyResult ()
    {
      _smartPageMock.Setup(_ => _.IsDirty).Returns(false);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(null);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesNull_AndDirtyStateEnabledIsTrue_AndPageIsDirtyIsFalse_AndTrackedControlsHaveIsDirtyFalse_ReturnsEmptyResult ()
    {
      var editableControlStub = new Mock<IEditableControl>();
      editableControlStub.Setup(_ => _.IsDirty).Returns(false);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub.Object);
      _smartPageMock.Setup(_ => _.IsDirty).Returns(false);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(null);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesNull_AndDirtyStateEnabledIsTrue_AndPageIsDirtyIsTrue_AndNoTrackedControls_ReturnsCurrentPageDirtyState ()
    {
      _smartPageMock.Setup(_ => _.IsDirty).Returns(true);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(null);

      Assert.That(result, Is.EquivalentTo(new[] { SmartPageDirtyStates.CurrentPage }));
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesNull_AndDirtyStateEnabledIsTrue_AndPageIsDirtyIsFalse_AndTrackedControlsHaveIsDirtyTrue_ReturnsCurrentPageDirtyState ()
    {
      var editableControlStub1 = new Mock<IEditableControl>();
      editableControlStub1.Setup(_ => _.IsDirty).Returns(false);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub1.Object);

      var editableControlStub2 = new Mock<IEditableControl>();
      editableControlStub2.Setup(_ => _.IsDirty).Returns(true);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub2.Object);

      var editableControlStub3 = new Mock<IEditableControl>();
      editableControlStub3.Setup(_ => _.IsDirty).Returns(false);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub3.Object);

      _smartPageMock.Setup(_ => _.IsDirty).Returns(false);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(null);

      Assert.That(result, Is.EquivalentTo(new[] { SmartPageDirtyStates.CurrentPage }));
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesNull_AndDirtyStateEnabledIsFalse_AndPageIsDirtyIsTrue_AndNoTrackedControls_ReturnsEmptyResult ()
    {
      _smartPageMock.Setup(_ => _.IsDirty).Returns(true);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(false);

      var result = _smartPageInfo.GetDirtyStates(null);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesNull_AndDirtyStateEnabledIsFalse_AndPageIsDirtyIsFalse_AndTrackedControlsHaveIsDirtyTrue_ReturnsEmptyResult ()
    {
      var editableControlStub = new Mock<IEditableControl>();
      editableControlStub.Setup(_ => _.IsDirty).Returns(true);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub.Object);
      _smartPageMock.Setup(_ => _.IsDirty).Returns(false);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(false);

      var result = _smartPageInfo.GetDirtyStates(null);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesContainsCurrentPageDirtyState_AndDirtyStateEnabledIsTrue_AndPageIsDirtyIsTrue_AndNoTrackedControls_ReturnsCurrentPageDirtyState ()
    {
      _smartPageMock.Setup(_ => _.IsDirty).Returns(true);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(new[] { SmartPageDirtyStates.CurrentPage });

      Assert.That(result, Is.EquivalentTo(new[] { SmartPageDirtyStates.CurrentPage }));
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesContainsCurrentPageDirtyState_AndDirtyStateEnabledIsTrue_AndPageIsDirtyIsFalse_AndTrackedControlsHaveIsDirtyTrue_ReturnsCurrentPageDirtyState ()
    {
      var editableControlStub = new Mock<IEditableControl>();
      editableControlStub.Setup(_ => _.IsDirty).Returns(true);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub.Object);

      _smartPageMock.Setup(_ => _.IsDirty).Returns(false);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(new[] { SmartPageDirtyStates.CurrentPage });

      Assert.That(result, Is.EquivalentTo(new[] { SmartPageDirtyStates.CurrentPage }));
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesExcludingCurrentPageDirtyState_AndDirtyStateEnabledIsFalse_AndPageIsDirtyIsTrue_AndNoTrackedControls_ReturnsEmptyResult ()
    {
      _smartPageMock.Setup(_ => _.IsDirty).Returns(true);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(new[] { "SomeState" });

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesExcludesCurrentPageDirtyState_AndDirtyStateEnabledIsFalse_AndPageIsDirtyIsFalse_AndTrackedControlsHaveIsDirtyTrue_ReturnsEmptyResult ()
    {
      var editableControlStub = new Mock<IEditableControl>();
      editableControlStub.Setup(_ => _.IsDirty).Returns(true);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub.Object);
      _smartPageMock.Setup(_ => _.IsDirty).Returns(false);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(new[] { "SomeState" });

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesEmpty_AndDirtyStateEnabledIsFalse_AndPageIsDirtyIsTrue_AndNoTrackedControls_ReturnsEmptyResult ()
    {
      _smartPageMock.Setup(_ => _.IsDirty).Returns(true);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(Array.Empty<string>());

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDirtyStates_WithRequestedStatesEmpty_AndDirtyStateEnabledIsFalse_AndPageIsDirtyIsFalse_AndTrackedControlsHaveIsDirtyTrue_ReturnsEmptyResult ()
    {
      var editableControlStub = new Mock<IEditableControl>();
      editableControlStub.Setup(_ => _.IsDirty).Returns(true);
      _smartPageInfo.RegisterControlForDirtyStateTracking(editableControlStub.Object);
      _smartPageMock.Setup(_ => _.IsDirty).Returns(false);
      _smartPageMock.Setup(_ => _.IsDirtyStateEnabled).Returns(true);

      var result = _smartPageInfo.GetDirtyStates(Array.Empty<string>());

      Assert.That(result, Is.Empty);
    }
  }
}
