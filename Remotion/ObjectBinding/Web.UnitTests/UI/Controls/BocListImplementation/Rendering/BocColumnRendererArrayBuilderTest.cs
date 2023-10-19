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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererArrayBuilderTest
  {
    private StubColumnDefinition _stubColumnDefinition;
    private IServiceLocator _serviceLocator;
    private Mock<WcagHelper> _wcagHelperStub;
    private StubValueColumnDefinition _stubValueColumnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
      _stubColumnDefinition = new StubColumnDefinition();
      _stubValueColumnDefinition = new StubValueColumnDefinition();
      _wcagHelperStub = new Mock<WcagHelper>();
    }

    [Test]
    public void GetColumnRenderers ()
    {
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(_stubColumnDefinition), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(StubColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(_stubColumnDefinition));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AlwaysVisibility_WithoutFailures_ContainsNullColumnRenderer ()
    {
      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(new Mock<IBocListValidationFailureRepository>().Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.Always
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(BocValidationErrorIndicatorColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AlwaysVisibility_WithListFailures_ContainsValidationErrorColumnRenderer ()
    {
      var validationRepositoryStub = new Mock<IBocListValidationFailureRepository>();
      validationRepositoryStub.Setup(_ => _.GetListFailureCount()).Returns(1);

      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(validationRepositoryStub.Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.Always
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(BocValidationErrorIndicatorColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AlwaysVisibility_WithRowOrCellFailures_ContainsValidationErrorColumnRenderer ()
    {
      var validationRepositoryStub = new Mock<IBocListValidationFailureRepository>();
      validationRepositoryStub.Setup(_ => _.GetListFailureCount()).Returns(0);
      validationRepositoryStub.Setup(_ => _.GetRowAndCellFailureCount()).Returns(1);

      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(validationRepositoryStub.Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.Always
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(BocValidationErrorIndicatorColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AnyValidationFailureVisibility_WithoutFailures_ContainsNullColumnRenderer ()
    {
      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(new Mock<IBocListValidationFailureRepository>().Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.AnyValidationFailure
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AnyValidationFailureVisibility_WithListFailures_ContainsValidationErrorColumnRenderer ()
    {
      var validationRepositoryStub = new Mock<IBocListValidationFailureRepository>();
      validationRepositoryStub.Setup(_ => _.GetListFailureCount()).Returns(1);

      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(validationRepositoryStub.Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.AnyValidationFailure
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(BocValidationErrorIndicatorColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AnyValidationFailureVisibility_WithRowOrCellFailures_ContainsValidationErrorColumnRenderer ()
    {
      var validationRepositoryStub = new Mock<IBocListValidationFailureRepository>();
      validationRepositoryStub.Setup(_ => _.GetListFailureCount()).Returns(0);
      validationRepositoryStub.Setup(_ => _.GetRowAndCellFailureCount()).Returns(1);

      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(validationRepositoryStub.Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.AnyValidationFailure
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(BocValidationErrorIndicatorColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AnyRowOrCellValidationFailureVisibility_WithoutFailures_ContainsNullColumnRenderer ()
    {
      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(new Mock<IBocListValidationFailureRepository>().Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.AnyRowOrCellValidationFailure
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AnyRowOrCellValidationFailureVisibility_WithListFailures_ContainsNullColumnRenderer ()
    {
      var validationRepositoryStub = new Mock<IBocListValidationFailureRepository>();
      validationRepositoryStub.Setup(_ => _.GetListFailureCount()).Returns(1);
      validationRepositoryStub.Setup(_ => _.GetRowAndCellFailureCount()).Returns(0);

      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(validationRepositoryStub.Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.AnyRowOrCellValidationFailure
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(NullColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }


    [Test]
    public void GetColumnRenderers_WithBocValidationErrorIndicatorColumnDefinition_AnyRowOrCellValidationFailureVisibility_WithRowOrCellFailures_ContainsValidationErrorColumnRenderer ()
    {
      var validationRepositoryStub = new Mock<IBocListValidationFailureRepository>();
      validationRepositoryStub.Setup(_ => _.GetListFailureCount()).Returns(0);
      validationRepositoryStub.Setup(_ => _.GetRowAndCellFailureCount()).Returns(2);

      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(validationRepositoryStub.Object);

      var validationErrorColumn = new BocValidationErrorIndicatorColumnDefinition
                                  {
                                    OwnerControl = bocListStub.Object,
                                    Visibility = BocValidationErrorIndicatorColumnDefinitionVisibility.AnyRowOrCellValidationFailure
                                  };

      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(validationErrorColumn), _serviceLocator, _wcagHelperStub.Object);

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(PrivateInvoke.GetNonPublicField(bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf(typeof(BocValidationErrorIndicatorColumnRenderer)));
      Assert.That(bocColumnRenderers[0].ColumnDefinition, Is.SameAs(validationErrorColumn));
      Assert.That(bocColumnRenderers[0].ColumnIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_IsClientSideSortingEnabled_False_And_HasSortingKeys_False ()
    {
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(_stubColumnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.SortingOrder = new List<BocListSortingOrderEntry>(new[] { new BocListSortingOrderEntry(_stubColumnDefinition, SortingDirection.Ascending) });
      builder.IsClientSideSortingEnabled = false;
      builder.HasSortingKeys = false;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].SortingDirection, Is.EqualTo(SortingDirection.None));
      Assert.That(bocColumnRenderers[0].OrderIndex, Is.EqualTo(-1));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_IsClientSideSortingEnabledTrue_And_HasSortingKeys_False ()
    {
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(_stubColumnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.SortingOrder = new List<BocListSortingOrderEntry>(new[] { new BocListSortingOrderEntry(_stubColumnDefinition, SortingDirection.Ascending) });
      builder.IsClientSideSortingEnabled = true;
      builder.HasSortingKeys = false;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].SortingDirection, Is.EqualTo(SortingDirection.Ascending));
      Assert.That(bocColumnRenderers[0].OrderIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_IsClientSideSortingEnabled_False_And_HasSortingKeys_True ()
    {
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(_stubColumnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.SortingOrder = new List<BocListSortingOrderEntry>(new[] { new BocListSortingOrderEntry(_stubColumnDefinition, SortingDirection.Ascending) });
      builder.IsClientSideSortingEnabled = false;
      builder.HasSortingKeys = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].SortingDirection, Is.EqualTo(SortingDirection.Ascending));
      Assert.That(bocColumnRenderers[0].OrderIndex, Is.EqualTo(0));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_SeveralColumns ()
    {
      var columns = new[] { _stubColumnDefinition, new StubColumnDefinition(), new StubColumnDefinition(), new StubColumnDefinition() };
      var builder = new BocColumnRendererArrayBuilder(new ReadOnlyCollection<BocColumnDefinition>(columns), _serviceLocator, _wcagHelperStub.Object);
      builder.SortingOrder =
          new List<BocListSortingOrderEntry>(
              new[]
              {
                  new BocListSortingOrderEntry(columns[0], SortingDirection.Ascending),
                  new BocListSortingOrderEntry(columns[2], SortingDirection.Descending),
                  new BocListSortingOrderEntry(columns[3], SortingDirection.None)
              });
      builder.IsClientSideSortingEnabled = true;
      builder.HasSortingKeys = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(4));
      Assert.That(bocColumnRenderers[0].SortingDirection, Is.EqualTo(SortingDirection.Ascending));
      Assert.That(bocColumnRenderers[0].OrderIndex, Is.EqualTo(0));
      Assert.That(bocColumnRenderers[1].SortingDirection, Is.EqualTo(SortingDirection.None));
      Assert.That(bocColumnRenderers[1].OrderIndex, Is.EqualTo(-1));
      Assert.That(bocColumnRenderers[2].SortingDirection, Is.EqualTo(SortingDirection.Descending));
      Assert.That(bocColumnRenderers[2].OrderIndex, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[3].SortingDirection, Is.EqualTo(SortingDirection.None));
      Assert.That(bocColumnRenderers[3].OrderIndex, Is.EqualTo(-1));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_SeveralMixedColumns ()
    {
      var columns = new BocColumnDefinition[]
                    { _stubColumnDefinition, new StubColumnDefinition(), new StubValueColumnDefinition(), new StubColumnDefinition() };
      var builder = new BocColumnRendererArrayBuilder(new ReadOnlyCollection<BocColumnDefinition>(columns), _serviceLocator, _wcagHelperStub.Object);
      builder.SortingOrder =
          new List<BocListSortingOrderEntry>(
              new[]
              {
                  new BocListSortingOrderEntry((IBocSortableColumnDefinition)columns[0], SortingDirection.Ascending),
                  new BocListSortingOrderEntry((IBocSortableColumnDefinition)columns[2], SortingDirection.Descending),
                  new BocListSortingOrderEntry((IBocSortableColumnDefinition)columns[3], SortingDirection.None)
              });
      builder.IsClientSideSortingEnabled = true;
      builder.HasSortingKeys = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(4));
      Assert.That(bocColumnRenderers[0].SortingDirection, Is.EqualTo(SortingDirection.Ascending));
      Assert.That(bocColumnRenderers[0].OrderIndex, Is.EqualTo(0));
      Assert.That(bocColumnRenderers[1].SortingDirection, Is.EqualTo(SortingDirection.None));
      Assert.That(bocColumnRenderers[1].OrderIndex, Is.EqualTo(-1));
      Assert.That(bocColumnRenderers[2].SortingDirection, Is.EqualTo(SortingDirection.Descending));
      Assert.That(bocColumnRenderers[2].OrderIndex, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[3].SortingDirection, Is.EqualTo(SortingDirection.None));
      Assert.That(bocColumnRenderers[3].OrderIndex, Is.EqualTo(-1));
    }

    [Test]
    public void GetColumnRenderers_BocValueColumnDefinition_EnableIcon_False ()
    {
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(_stubValueColumnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.EnableIcon = false;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.False);
    }

    [Test]
    public void GetColumnRenderers_BocValueColumnDefinition_EnableIcon_True ()
    {
      var builder = new BocColumnRendererArrayBuilder(CreateColumnCollection(_stubValueColumnDefinition), _serviceLocator, _wcagHelperStub.Object);
      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(1));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.True);
    }

    [Test]
    public void GetColumnRenderers_SeveralBocValueColumnDefinitions_EnableIcon_True ()
    {
      var builder =
          new BocColumnRendererArrayBuilder(
              CreateColumnCollection(_stubValueColumnDefinition, new StubValueColumnDefinition(), new StubValueColumnDefinition()),
              _serviceLocator,
              _wcagHelperStub.Object);
      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(3));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.True);
      Assert.That(bocColumnRenderers[1].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[2].ShowIcon, Is.False);
    }

    [Test]
    public void GetColumnRenderers_ColumnDefinitionWithRowIconPreferred_EnablesIconOnColumnRendererWithRowIconPreferred ()
    {
      var builder = new BocColumnRendererArrayBuilder(
          CreateColumnCollection(
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Preferred},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic}),
          _serviceLocator,
          _wcagHelperStub.Object);

      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(3));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[1].ShowIcon, Is.True);
      Assert.That(bocColumnRenderers[2].ShowIcon, Is.False);
    }

    [Test]
    public void GetColumnRenderers_ColumnDefinitionWithRowIconDisabled_EnablesIconOnFirstColumnRendererWithRowIconAutomatic ()
    {
      var builder = new BocColumnRendererArrayBuilder(
          CreateColumnCollection(
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Disabled},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Disabled},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic}),
          _serviceLocator,
          _wcagHelperStub.Object);

      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(3));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[1].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[2].ShowIcon, Is.True);
    }

    [Test]
    public void GetColumnRenderers_ColumnDefinitionWithRowIconAutomatic_EnablesIconOnFirstColumnRendererWithRowIconAutomatic ()
    {
      var builder = new BocColumnRendererArrayBuilder(
          CreateColumnCollection(
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic}),
          _serviceLocator,
          _wcagHelperStub.Object);

      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(3));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.True);
      Assert.That(bocColumnRenderers[1].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[2].ShowIcon, Is.False);
    }

    [Test]
    public void GetColumnRenderers_ColumnDefinitionWithRowIcon_Automatic_Disabled_Preferred_EnablesIconOnColumnRendererWithRowIconPreferred ()
    {
      var builder = new BocColumnRendererArrayBuilder(
          CreateColumnCollection(
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Disabled},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Preferred}),
          _serviceLocator,
          _wcagHelperStub.Object);

      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(3));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[1].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[2].ShowIcon, Is.True);
    }

    [Test]
    public void GetColumnRenderers_ColumnDefinitionWithRowIcon_Preferred_Automatic_Preferred_EnablesIconOnFirstColumnRenderer ()
    {
      var builder = new BocColumnRendererArrayBuilder(
          CreateColumnCollection(
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Preferred},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Automatic},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Preferred}),
          _serviceLocator,
          _wcagHelperStub.Object);

      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(3));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.True);
      Assert.That(bocColumnRenderers[1].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[2].ShowIcon, Is.False);
    }

    [Test]
    public void GetColumnRenderers_ColumnDefinitionWithRowIcon_OnlyPreferred_EnablesIconOnFirstColumnRenderer ()
    {
      var builder = new BocColumnRendererArrayBuilder(
          CreateColumnCollection(
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Preferred},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Preferred},
              new StubValueColumnDefinition {RowIconMode = RowIconMode.Preferred}),
          _serviceLocator,
          _wcagHelperStub.Object);

      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers();

      Assert.That(bocColumnRenderers.Length, Is.EqualTo(3));
      Assert.That(bocColumnRenderers[0].ShowIcon, Is.True);
      Assert.That(bocColumnRenderers[1].ShowIcon, Is.False);
      Assert.That(bocColumnRenderers[2].ShowIcon, Is.False);
    }

    private static ReadOnlyCollection<BocColumnDefinition> CreateColumnCollection (params BocColumnDefinition[] values) => new(values);
  }
}
