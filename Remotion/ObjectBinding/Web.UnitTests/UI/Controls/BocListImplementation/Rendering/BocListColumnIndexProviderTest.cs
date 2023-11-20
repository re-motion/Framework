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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListColumnIndexProviderTest
  {
    [Test]
    public void Initialize_CreatesColumnLookupLazily ()
    {
      // Explicit strict mock to ensure it is not used in the constructor
      var columnRendererMock = new Mock<IReadOnlyList<BocColumnRenderer>>(MockBehavior.Strict);

      Assert.That(() => new BocListColumnIndexProvider(columnRendererMock.Object), Throws.Nothing);
    }

    [Test]
    public void GetColumnIndex ()
    {
      var stubColumnDefinition1 = new StubColumnDefinition();
      var bocColumnRenderer1 = CreateBocColumnRenderer(stubColumnDefinition1, 10, -10);

      var stubColumnDefinition2 = new StubColumnDefinition();
      var bocColumnRenderer2 = CreateBocColumnRenderer(stubColumnDefinition2, 42, -42);

      var columnIndexProvider = new BocListColumnIndexProvider(new[] { bocColumnRenderer1, bocColumnRenderer2 });

      Assert.That(columnIndexProvider.GetColumnIndex(stubColumnDefinition2), Is.EqualTo(42));
    }

    [Test]
    public void GetColumnIndex_ColumnDefinitionNotFound_ThrowsException ()
    {
      var bocColumnRenderer = CreateBocColumnRenderer(new StubColumnDefinition(), 42, -42);
      var columnIndexProvider = new BocListColumnIndexProvider(new[] { bocColumnRenderer });

      var nonExistentColumnDefinition = new StubColumnDefinition();
      nonExistentColumnDefinition.ItemID = "testID";
      Assert.That(
          () => columnIndexProvider.GetColumnIndex(nonExistentColumnDefinition),
          Throws.InvalidOperationException.With.Message.EqualTo("Could not find a BocColumnRenderer for the BocColumnDefinition with item ID 'testID'."));
    }

    [Test]
    public void GetVisibleColumnIndex ()
    {
      var stubColumnDefinition1 = new StubColumnDefinition();
      var bocColumnRenderer1 = CreateBocColumnRenderer(stubColumnDefinition1, 10, -10);

      var stubColumnDefinition2 = new StubColumnDefinition();
      var bocColumnRenderer2 = CreateBocColumnRenderer(stubColumnDefinition2, 42, -42);

      var columnIndexProvider = new BocListColumnIndexProvider(new[] { bocColumnRenderer1, bocColumnRenderer2 });

      Assert.That(columnIndexProvider.GetVisibleColumnIndex(stubColumnDefinition2), Is.EqualTo(-42));
    }

    [Test]
    public void GetVisibleColumnIndex_ColumnDefinitionNotFound_ThrowsException ()
    {
      var bocColumnRenderer = CreateBocColumnRenderer(new StubColumnDefinition(), 42, -42);
      var columnIndexProvider = new BocListColumnIndexProvider(new[] { bocColumnRenderer });

      var nonExistentColumnDefinition = new StubColumnDefinition();
      nonExistentColumnDefinition.ItemID = "testID";
      Assert.That(
          () => columnIndexProvider.GetVisibleColumnIndex(nonExistentColumnDefinition),
          Throws.InvalidOperationException.With.Message.EqualTo("Could not find a BocColumnRenderer for the BocColumnDefinition with item ID 'testID'."));
    }

    private BocColumnRenderer CreateBocColumnRenderer (BocColumnDefinition columnDefinition, int columnIndex, int visibleColumnIndex)
    {
      return new BocColumnRenderer(
          Mock.Of<IBocColumnRenderer>(),
          columnDefinition,
          columnIndex,
          visibleColumnIndex,
          false,
          false,
          SortingDirection.None,
          -1);
    }
  }
}
