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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Remotion.ObjectBinding.Validation;

namespace Remotion.ObjectBinding.UnitTests.Validation
{
  [TestFixture]
  public class CompoundBusinessObjectPropertyPathBasedValidationFailureMatcherTest
  {
    [Test]
    public void CompoundBusinessObjectPropertyPathBasedValidationFailureMatcherTest_AssignsCorrectPropertyPaths ()
    {
      var firstPropertyPathStub = new Mock<IBusinessObjectPropertyPath>();
      var secondPropertyPathStub = new Mock<IBusinessObjectPropertyPath>();

      var matcher = new CompoundBusinessObjectPropertyPathBasedValidationFailureMatcher(new [] {firstPropertyPathStub.Object, secondPropertyPathStub.Object});

      Assert.That(matcher.PropertyPaths, Is.EquivalentTo(new [] {firstPropertyPathStub.Object, secondPropertyPathStub.Object}));
    }

    [Test]
    public void GetMatchingValidationFailures_WithEmptyPropertyPaths_ReturnsEmpty ()
    {
      var rowObjectStub = new Mock<IBusinessObject>();
      var validationResultStub = new Mock<IBusinessObjectValidationResult>();

      var matcher = new CompoundBusinessObjectPropertyPathBasedValidationFailureMatcher(Array.Empty<IBusinessObjectPropertyPath>());
      var result = matcher.GetMatchingValidationFailures(rowObjectStub.Object, validationResultStub.Object);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetMatchingValidationFailures_ReturnsValidationFailures ()
    {
      var rowObjectStub = new Mock<IBusinessObject>();
      var validationResultStub = new Mock<IBusinessObjectValidationResult>();

      //first property
      var firstPropertyPathResultStub = new Mock<IBusinessObjectPropertyPathResult>();
      var firstResultObjectStub = new Mock<IBusinessObject>();
      firstPropertyPathResultStub
          .Setup(_ => _.ResultObject)
          .Returns(firstResultObjectStub.Object);

      var firstResultPropertyStub = new Mock<IBusinessObjectProperty>();
      firstPropertyPathResultStub
          .Setup(_ => _.ResultProperty)
          .Returns(firstResultPropertyStub.Object);
      var firstPropertyPathStub = new Mock<IBusinessObjectPropertyPath>();

      firstPropertyPathStub
          .Setup(
              _ => _.GetResult(
                  rowObjectStub.Object,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
          .Returns(firstPropertyPathResultStub.Object);

      //second property
      var secondPropertyPathResultStub = new Mock<IBusinessObjectPropertyPathResult>();
      var secondResultObjectStub = new Mock<IBusinessObject>();
      secondPropertyPathResultStub
          .Setup(_ => _.ResultObject)
          .Returns(secondResultObjectStub.Object);

      var secondResultPropertyStub = new Mock<IBusinessObjectProperty>();
      secondPropertyPathResultStub
          .Setup(_ => _.ResultProperty)
          .Returns(secondResultPropertyStub.Object);

      var secondPropertyPathStub = new Mock<IBusinessObjectPropertyPath>();
      secondPropertyPathStub
          .Setup(
              _ => _.GetResult(
                  rowObjectStub.Object,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
          .Returns(secondPropertyPathResultStub.Object);

      var validationErrors = new[]
                             {
                               BusinessObjectValidationFailure.Create("Oh no, something happened"),
                               BusinessObjectValidationFailure.Create("Another error?"),
                               BusinessObjectValidationFailure.Create("What are you doing?"),
                             };

      validationResultStub
          .Setup(_ => _.GetValidationFailures(firstResultObjectStub.Object, firstResultPropertyStub.Object, true))
          .Returns(new[] { validationErrors[0] });
      validationResultStub
          .Setup(_ => _.GetValidationFailures(secondResultObjectStub.Object, secondResultPropertyStub.Object, true))
          .Returns(new[] { validationErrors[1], validationErrors[2] });

      var matcher = new CompoundBusinessObjectPropertyPathBasedValidationFailureMatcher(new [] {firstPropertyPathStub.Object, secondPropertyPathStub.Object});
      var result = matcher.GetMatchingValidationFailures(rowObjectStub.Object, validationResultStub.Object);

      Assert.That(result, Is.EqualTo(validationErrors));
    }

    [Test]
    public void GetMatchingValidationFailures_FirstPathNoErrors_ReturnsValidationFailuresForSecondPath ()
    {
      var rowObjectStub = new Mock<IBusinessObject>();
      var validationResultStub = new Mock<IBusinessObjectValidationResult>();

      var firstPropertyPathResultStub = new Mock<IBusinessObjectPropertyPathResult>();
      var firstResultObjectStub = new Mock<IBusinessObject>();
      firstPropertyPathResultStub
          .Setup(_ => _.ResultObject)
          .Returns(firstResultObjectStub.Object);

      var firstResultPropertyStub = new Mock<IBusinessObjectProperty>();
      firstPropertyPathResultStub
          .Setup(_ => _.ResultProperty)
          .Returns(firstResultPropertyStub.Object);

      var firstPropertyPathStub = new Mock<IBusinessObjectPropertyPath>();
      firstPropertyPathStub
          .Setup(
              _ => _.GetResult(
                  rowObjectStub.Object,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
          .Returns(firstPropertyPathResultStub.Object);

      var secondPropertyPathResultStub = new Mock<IBusinessObjectPropertyPathResult>();
      var secondResultObjectStub = new Mock<IBusinessObject>();
      secondPropertyPathResultStub
          .Setup(_ => _.ResultObject)
          .Returns(secondResultObjectStub.Object);

      var secondResultPropertyStub = new Mock<IBusinessObjectProperty>();
      secondPropertyPathResultStub
          .Setup(_ => _.ResultProperty)
          .Returns(secondResultPropertyStub.Object);

      var secondPropertyPathStub = new Mock<IBusinessObjectPropertyPath>();
      secondPropertyPathStub
          .Setup(
              _ => _.GetResult(
                  rowObjectStub.Object,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
          .Returns(secondPropertyPathResultStub.Object);

      var validationErrors = new[]
                             {
                               BusinessObjectValidationFailure.Create("Oh no, something happened"),
                               BusinessObjectValidationFailure.Create("Another error?"),
                             };

      validationResultStub
          .Setup(_ => _.GetValidationFailures(firstResultObjectStub.Object, firstResultPropertyStub.Object, true))
          .Returns(Array.Empty<BusinessObjectValidationFailure>());
      validationResultStub
          .Setup(_ => _.GetValidationFailures(secondResultObjectStub.Object, secondResultPropertyStub.Object, true))
          .Returns(new[] { validationErrors[0], validationErrors[1] });
      var matcher = new CompoundBusinessObjectPropertyPathBasedValidationFailureMatcher(new [] {firstPropertyPathStub.Object, secondPropertyPathStub.Object});
      var result = matcher.GetMatchingValidationFailures(rowObjectStub.Object, validationResultStub.Object);

      Assert.That(result, Is.EqualTo(validationErrors));
    }
  }
}
