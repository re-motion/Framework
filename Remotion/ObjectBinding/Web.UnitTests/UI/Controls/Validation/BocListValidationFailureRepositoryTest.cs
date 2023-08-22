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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class BocListValidationFailureRepositoryTest
  {
    private Mock<IBusinessObject> _rowObjectStub;
    private Mock<BocColumnDefinition> _columnDefinitionStub;

    [SetUp]
    public void Setup ()
    {
      _rowObjectStub = new Mock<IBusinessObject>();
      _columnDefinitionStub = new Mock<BocColumnDefinition>();
    }

    [Test]
    public void AddValidationFailuresForBocList_ContainedInRepository ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailure(failures[0]),
                             };

      repository.AddValidationFailuresForBocList(failures);

      var result = repository.GetUnhandledValidationFailuresForBocList(false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void AddValidationFailuresForDataRow_ContainedInRepository ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailureForRow(failures[0], _rowObjectStub.Object),
                             };

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataRow(_rowObjectStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void AddValidationFailuresForDataCell_ContainedInRepository ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                            {
                              BocListValidationFailureWithLocationInformation.CreateFailureForCell(failures[0], _rowObjectStub.Object, _columnDefinitionStub.Object),
                            };

      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void ClearAllValidationFailures_NothingContainedInRepository ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      var listFailure = BusinessObjectValidationFailure.Create("List failure");

      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      repository.ClearAllValidationFailures();

      var result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataCell_ReturnsCellFailures ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var listFailure = BusinessObjectValidationFailure.Create("List failure");
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      var expectedFailures = new[]
                             {
                                 BocListValidationFailureWithLocationInformation.CreateFailureForCell(cellFailure, _rowObjectStub.Object, _columnDefinitionStub.Object)
                             };

      var result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, false);
      Assert.That(result, Is.EquivalentTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataCell_MarkAsHandled_ValidationFailureIsNotReturnedAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailureForCell(failures[0], _rowObjectStub.Object, _columnDefinitionStub.Object),
                             };

      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, true);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, false);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataCell_DontMarkAsHandled_ReturnsValidationFailureInSubsequentCalls ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailureForCell(failures[0], _rowObjectStub.Object, _columnDefinitionStub.Object),
                             };

      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRow_ReturnsRowFailures ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var listFailure = BusinessObjectValidationFailure.Create("List failure");
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      var expectedFailures = new[]
                             {
                                 BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object)
                             };

      var result = repository.GetUnhandledValidationFailuresForDataRow(_rowObjectStub.Object, false).ToArray();
      Assert.That(result, Is.EquivalentTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRow_MarkAsHandled_NotContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailureForRow(failures[0], _rowObjectStub.Object),
                             };

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataRow(_rowObjectStub.Object, true);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataRow(_rowObjectStub.Object, false);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRow_DontMarkAsHandled_ContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailureForRow(failures[0], _rowObjectStub.Object),
                             };

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataRow(_rowObjectStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataRow(_rowObjectStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRowAndContainingDataCells_ReturnsCellAndRowFailures ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var listFailure = BusinessObjectValidationFailure.Create("List failure");
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      var expectedFailures = new []
                             {
                                 BocListValidationFailureWithLocationInformation.CreateFailureForCell(cellFailure, _rowObjectStub.Object, _columnDefinitionStub.Object),
                                 BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object)
                             };

      var result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(_rowObjectStub.Object, false).ToArray();
      Assert.That(result, Is.EquivalentTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRowAndContainingDataCells_MarkAsHandled_NotContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailureForRow(failures[0], _rowObjectStub.Object),
                             };

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(_rowObjectStub.Object, true);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(_rowObjectStub.Object, false);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRowAndContainingDataCells_DontMarkAsHandled_ContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailureForRow(failures[0], _rowObjectStub.Object),
                             };

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, failures);

      var result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(_rowObjectStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(_rowObjectStub.Object, false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRowAndContainingDataCells_WithCellHandledSeparately_OnlyContainsUnhandledFailures ()
    {
      var repository = new BocListValidationFailureRepository();
      var markedColumnStub = new Mock<BocColumnDefinition>();

      var rowFailure = BusinessObjectValidationFailure.Create("A failure");
      var rowFailuresWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object);

      var unmarkedFailure = BusinessObjectValidationFailure.Create("Unmarked Failure");
      var unmarkedFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(unmarkedFailure, _rowObjectStub.Object, _columnDefinitionStub.Object);

      var markedFailure = BusinessObjectValidationFailure.Create("Marked Failure");
      var markedFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(markedFailure, _rowObjectStub.Object, markedColumnStub.Object);

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { unmarkedFailure });
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, markedColumnStub.Object, new[] { markedFailure });

      var result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(_rowObjectStub.Object, false);
      Assert.That(result, Is.EquivalentTo(new[] { rowFailuresWithPosition, unmarkedFailureWithPosition, markedFailureWithPosition }));

      result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, markedColumnStub.Object, true);
      Assert.That(result, Is.EqualTo(new[] { markedFailureWithPosition }));

      result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(_rowObjectStub.Object, false);
      Assert.That(result, Is.EquivalentTo(new[] { rowFailuresWithPosition, unmarkedFailureWithPosition }));
    }


    [Test]
    public void GetUnhandledValidationFailuresForDataRowsAndContainingDataCells_ReturnsCellAndRowFailures ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var listFailure = BusinessObjectValidationFailure.Create("List failure");
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      var expectedFailures = new []
                             {
                                 BocListValidationFailureWithLocationInformation.CreateFailureForCell(cellFailure, _rowObjectStub.Object, _columnDefinitionStub.Object),
                                 BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object)
                             };

      var result = repository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(false).ToArray();
      Assert.That(result, Is.EquivalentTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRowsAndContainingDataCells_MarkAsHandled_NotContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var expectedFailures = new[]
                             {
                                 BocListValidationFailureWithLocationInformation.CreateFailureForCell(cellFailure, _rowObjectStub.Object, _columnDefinitionStub.Object),
                                 BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object)
                             };

      var result = repository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(true);
      Assert.That(result, Is.EquivalentTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(false);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRowsAndContainingDataCells_DontMarkAsHandled_ContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var expectedFailures = new[]
                             {
                                 BocListValidationFailureWithLocationInformation.CreateFailureForCell(cellFailure, _rowObjectStub.Object, _columnDefinitionStub.Object),
                                 BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object)
                             };

      var result = repository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(false);
      Assert.That(result, Is.EquivalentTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(false);
      Assert.That(result, Is.EquivalentTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForDataRowsAndContainingDataCells_WithCellHandledSeparately_OnlyContainsUnhandledFailures ()
    {
      var repository = new BocListValidationFailureRepository();
      var markedColumnStub = new Mock<BocColumnDefinition>();

      var rowFailure = BusinessObjectValidationFailure.Create("A failure");
      var rowFailuresWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object);

      var unmarkedFailure = BusinessObjectValidationFailure.Create("Unmarked Failure");
      var unmarkedFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(unmarkedFailure, _rowObjectStub.Object, _columnDefinitionStub.Object);

      var markedFailure = BusinessObjectValidationFailure.Create("Marked Failure");
      var markedFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(markedFailure, _rowObjectStub.Object, markedColumnStub.Object);

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { unmarkedFailure });
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, markedColumnStub.Object, new[] { markedFailure });

      var result = repository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(false);
      Assert.That(result, Is.EquivalentTo(new[] { rowFailuresWithPosition, unmarkedFailureWithPosition, markedFailureWithPosition }));

      result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, markedColumnStub.Object, true);
      Assert.That(result, Is.EqualTo(new[] { markedFailureWithPosition }));

      result = repository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(false);
      Assert.That(result, Is.EquivalentTo(new[] { rowFailuresWithPosition, unmarkedFailureWithPosition }));
    }

    [Test]
    public void GetUnhandledValidationFailuresForBocList_ReturnsListFailures ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var listFailure = BusinessObjectValidationFailure.Create("List failure");
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      var expectedFailures = new[]
                             {
                                 BocListValidationFailureWithLocationInformation.CreateFailure(listFailure)
                             };

      var result = repository.GetUnhandledValidationFailuresForBocList(false);
      Assert.That(result, Is.EquivalentTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForBocList_MarkAsHandled_NotContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailure(failures[0]),
                             };

      repository.AddValidationFailuresForBocList(failures);

      var result = repository.GetUnhandledValidationFailuresForBocList(true);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForBocList(false);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailuresForBocList_DontMarkAsHandled_ContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailure(failures[0]),
                             };

      repository.AddValidationFailuresForBocList(failures);

      var result = repository.GetUnhandledValidationFailuresForBocList(false);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForBocList(false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells_ReturnsCellAndRowAndListFailures ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      var cellFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(cellFailure, _rowObjectStub.Object, _columnDefinitionStub.Object);

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      var rowFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForRow(rowFailure, _rowObjectStub.Object);

      var listFailure = BusinessObjectValidationFailure.Create("List failure");
      var listFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailure(listFailure);

      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      var result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Does.Contain(cellFailureWithPosition));
      Assert.That(result, Does.Contain(rowFailureWithPosition));
      Assert.That(result, Does.Contain(listFailureWithPosition));
    }

    [Test]
    public void GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells_MarkAsHandled_NotContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailure(failures[0]),
                             };

      repository.AddValidationFailuresForBocList(failures);

      var result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells_DontMarkAsHandled_ContainedInRepositoryAfterwards ()
    {
      var repository = new BocListValidationFailureRepository();

      var failures = new[]
                     {
                       BusinessObjectValidationFailure.Create("A failure")
                     };

      var expectedFailures = new[]
                             {
                               BocListValidationFailureWithLocationInformation.CreateFailure(failures[0]),
                             };

      repository.AddValidationFailuresForBocList(failures);

      var result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Is.EqualTo(expectedFailures));

      result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Is.EqualTo(expectedFailures));
    }

    [Test]
    public void GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells_WithCellAndRowHandledSeparately_OnlyContainsUnhandledFailures ()
    {
      var repository = new BocListValidationFailureRepository();

      var unmarkedRowFailure = BusinessObjectValidationFailure.Create("Unmarked row failure");
      var markedRowFailure = BusinessObjectValidationFailure.Create("Marked row failure");
      var unmarkedCellFailure = BusinessObjectValidationFailure.Create("Unmarked cell failure");
      var markedHiddenCellFailure = BusinessObjectValidationFailure.Create("Hidden marked cell failure");
      var markedCellFailure = BusinessObjectValidationFailure.Create("Marked cell failure");

      var markedColumnStub = new Mock<BocColumnDefinition>();
      var markedRowStub = new Mock<IBusinessObject>();

      var unmarkedRowFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForRow(unmarkedRowFailure, _rowObjectStub.Object);
      var markedRowFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForRow(markedRowFailure, markedRowStub.Object);
      var unmarkedCellFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(unmarkedCellFailure, _rowObjectStub.Object, _columnDefinitionStub.Object);
      var markedHiddenCellFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(markedHiddenCellFailure, markedRowStub.Object, markedColumnStub.Object);
      var markedCellFailureWithPosition = BocListValidationFailureWithLocationInformation.CreateFailureForCell(markedCellFailure, _rowObjectStub.Object, markedColumnStub.Object);

      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { unmarkedRowFailure });
      repository.AddValidationFailuresForDataRow(markedRowStub.Object, new[] { markedRowFailure });
      repository.AddValidationFailuresForDataCell(markedRowStub.Object, markedColumnStub.Object, new[] { markedHiddenCellFailure });

      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { unmarkedCellFailure });
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, markedColumnStub.Object, new[] { markedCellFailure });

      var result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Is.EquivalentTo(new[] {
                                                  unmarkedCellFailureWithPosition,
                                                  unmarkedRowFailureWithPosition,
                                                  markedRowFailureWithPosition,
                                                  markedCellFailureWithPosition,
                                                  markedHiddenCellFailureWithPosition
                                                }));

      result = repository.GetUnhandledValidationFailuresForDataCell(_rowObjectStub.Object, markedColumnStub.Object, true);
      Assert.That(result, Is.EqualTo(new[] { markedCellFailureWithPosition }));

      result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Is.EquivalentTo(new[] { unmarkedCellFailureWithPosition,
                                                  unmarkedRowFailureWithPosition,
                                                  markedRowFailureWithPosition,
                                                  markedHiddenCellFailureWithPosition }));

      result = repository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(markedRowStub.Object, true);
      Assert.That(result, Is.EquivalentTo(new[] { markedRowFailureWithPosition, markedHiddenCellFailureWithPosition }));

      result = repository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);
      Assert.That(result, Is.EquivalentTo(new[] { unmarkedCellFailureWithPosition, unmarkedRowFailureWithPosition }));
    }

    [Test]
    public void HasValidationFailuresForDataRow_WithoutFailures_ReturnsFalse ()
    {
      var repository = new BocListValidationFailureRepository();

      var result = repository.HasValidationFailuresForDataRow(_rowObjectStub.Object);

      Assert.That(result, Is.False);
    }

    [Test]
    public void HasValidationFailuresForDataRow_WithOnlyCellFailures_ReturnsTrue ()
    {
      var repository = new BocListValidationFailureRepository();

      var cellFailure = BusinessObjectValidationFailure.Create("Cell failure");
      repository.AddValidationFailuresForDataCell(_rowObjectStub.Object, _columnDefinitionStub.Object, new[] { cellFailure });

      var result = repository.HasValidationFailuresForDataRow(_rowObjectStub.Object);

      Assert.That(result, Is.True);
    }

    [Test]
    public void HasValidationFailuresForDataRow_WithOnlyRowFailures_ReturnsTrue ()
    {
      var repository = new BocListValidationFailureRepository();

      var rowFailure = BusinessObjectValidationFailure.Create("Row failure");
      repository.AddValidationFailuresForDataRow(_rowObjectStub.Object, new[] { rowFailure });

      var result = repository.HasValidationFailuresForDataRow(_rowObjectStub.Object);

      Assert.That(result, Is.True);
    }

    [Test]
    public void HasValidationFailuresForDataRow_WithOnlyListFailures_ReturnsFalse ()
    {
      var repository = new BocListValidationFailureRepository();

      var listFailure = BusinessObjectValidationFailure.Create("List failure");
      repository.AddValidationFailuresForBocList(new[] { listFailure });

      var result = repository.HasValidationFailuresForDataRow(_rowObjectStub.Object);

      Assert.That(result, Is.False);
    }
  }
}
