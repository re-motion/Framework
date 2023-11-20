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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocListRowWithValidationFailureComparerTest
  {
    private BocListRowWithValidationFailureComparer _comparer;
    private IBocListValidationFailureRepository _validationFailureRepository;

    private IBusinessObject _zeroErrorBusinessObject;
    private IBusinessObject _singleErrorBusinessObject;
    private IBusinessObject _multipleErrorBusinessObject;

    private BocListRow _zeroErrorRow;
    private BocListRow _singleErrorRow;
    private BocListRow _multipleErrorRow;

    [SetUp]
    public void Setup ()
    {
      _validationFailureRepository = new BocListValidationFailureRepository();
      _comparer = new BocListRowWithValidationFailureComparer(_validationFailureRepository);

      _singleErrorBusinessObject = new Mock<IBusinessObject>().Object;
      _zeroErrorBusinessObject = new Mock<IBusinessObject>().Object;
      _multipleErrorBusinessObject = new Mock<IBusinessObject>().Object;

      _zeroErrorRow = new BocListRow(0, _zeroErrorBusinessObject);
      _singleErrorRow = new BocListRow(1, _singleErrorBusinessObject);
      _multipleErrorRow = new BocListRow(2, _multipleErrorBusinessObject);

      _validationFailureRepository.AddValidationFailuresForDataRow(_zeroErrorBusinessObject, new BusinessObjectValidationFailure[0]);
      _validationFailureRepository.AddValidationFailuresForDataRow(_singleErrorBusinessObject, new[] { BusinessObjectValidationFailure.Create("Single error") });
      _validationFailureRepository.AddValidationFailuresForDataRow(
          _multipleErrorBusinessObject,
          new[]
          {
            BusinessObjectValidationFailure.Create("Error 1"),
            BusinessObjectValidationFailure.Create("Error 2")
          });
    }

    [Test]
    public void Compare_WithFirstObjectNull_ReturnsNegativeNumber ()
    {
      Assert.That(_comparer.Compare(null, _singleErrorRow), Is.EqualTo(1));
    }

    [Test]
    public void Compare_WithSecondObjectNull_ReturnsPositiveNumber ()
    {
      Assert.That(_comparer.Compare(_singleErrorRow, null), Is.EqualTo(-1));
    }

    [Test]
    public void Compare_WithBothObjectNull_ReturnsZero ()
    {
      Assert.That(_comparer.Compare(null, null), Is.EqualTo(0));
    }

    [Test]
    public void Compare_WithFirstObjectMoreErrors_ReturnsZero ()
    {
      Assert.That(_comparer.Compare(_multipleErrorRow, _singleErrorRow), Is.EqualTo(0));
    }

    [Test]
    public void Compare_WithSecondObjectMoreErrors_ReturnsZero ()
    {
      Assert.That(_comparer.Compare(_singleErrorRow, _multipleErrorRow), Is.EqualTo(0));
    }

    [Test]
    public void Compare_WithFirstObjectNoErrors_ReturnsPositiveNumber ()
    {
      Assert.That(_comparer.Compare(_zeroErrorRow, _singleErrorRow), Is.EqualTo(1));
    }

    [Test]
    public void Compare_WithSecondObjectNoErrors_ReturnsNegativeNumber ()
    {
      Assert.That(_comparer.Compare(_singleErrorRow, _zeroErrorRow), Is.EqualTo(-1));
    }
  }
}
