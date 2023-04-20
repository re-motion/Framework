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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class IsColumnVisible_BocColumnRendererArrayBuilderTest
  {
    private Mock<WcagHelper> _wcagHelperStub;
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
      _wcagHelperStub = new Mock<WcagHelper>();
    }

    [Test]
    public void BocCommandColumnDefinition_IsWaiConformanceLevelARequired_True_And_CommandType_None ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand(CommandType.None);
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(true);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocCommandColumnDefinition_IsWaiConformanceLevelARequired_False_And_CommandType_Event ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand(CommandType.Event);
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocCommandColumnDefinition_IsWaiConformanceLevelARequired_True_And_CommandType_Event ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand(CommandType.Event);
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(true);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocCommandColumnDefinition_IsWaiConformanceLevelARequired_False_And_CommandType_WxeFunction ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand(CommandType.WxeFunction);
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf(typeof(NullColumnRenderer)));
    }


    [Test]
    public void BocCommandColumnDefinition_IsWaiConformanceLevelARequired_True_And_CommandType_WxeFunction ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand(CommandType.WxeFunction);
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(true);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequired_True ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(true);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequired_False_AndIsListReadOnly_True_AndEditMode_False ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      columnDefinition.Show = BocRowEditColumnDefinitionShow.Always;
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.IsListReadOnly = true;

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequired_False_And_IsListReadOnly_False_And_EditMode_True ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      columnDefinition.Show = BocRowEditColumnDefinitionShow.EditMode;
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.IsListReadOnly = false;

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequired_False_And_IsListReadOnly_True_And_EditMode_True ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      columnDefinition.Show = BocRowEditColumnDefinitionShow.EditMode;
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.IsListReadOnly = true;

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocRowEditModeColumnDefinition_IsListEditModeActive_False ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.IsListEditModeActive = false;

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocRowEditModeColumnDefinition_IsListEditModeActive_True ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.IsListEditModeActive = true;

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocDropDownMenuColumnDefinition_IsWaiConformanceLevelARequired_True ()
    {
      var columnDefinition = new BocDropDownMenuColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(true);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocDropDownMenuColumnDefinition_IsWaiConformanceLevelARequired_False_And_IsBrowserCapableOfScripting_False ()
    {
      var columnDefinition = new BocDropDownMenuColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.IsBrowserCapableOfScripting = false;

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
    }

    [Test]
    public void BocDropDownMenuColumnDefinition_IsWaiConformanceLevelARequired_False_And_IsBrowserCapableOfScripting_True ()
    {
      var columnDefinition = new BocDropDownMenuColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(columnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.IsBrowserCapableOfScripting = true;

      _wcagHelperStub.Setup(stub => stub.IsWaiConformanceLevelARequired()).Returns(false);

      var bocColumnRenderers = builder.CreateColumnRenderers();
      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf(typeof(NullColumnRenderer)));
    }

    private static ReadOnlyCollection<BocColumnDefinition> CreateColumnCollection (params BocColumnDefinition[] values) => new(values);
  }
}
