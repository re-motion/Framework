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
  public class BusinessObjectPropertyPathValidationFailureMatcherTest
  {
    [Test]
    public void BusinessObjectPropertyPathValidationFailureMatcher_AssignsCorrectPropertyPath ()
    {
      var propertyPathStub = new Mock<IBusinessObjectPropertyPath>();

      var matcher = new BusinessObjectPropertyPathValidationFailureMatcher(propertyPathStub.Object);

      Assert.That(matcher.PropertyPath, Is.EqualTo(propertyPathStub.Object));
    }

    [Test]
    public void GetMatchingValidationFailures_WithEmptyBusinessObjectFromResult_ReturnsEmptyArray ()
    {
      var validationResultStub = new Mock<IBusinessObjectValidationResult>();
      var rowObjectStub = new Mock<IBusinessObject>();

      var propertyPathStub = new Mock<IBusinessObjectPropertyPath>();
      propertyPathStub
          .Setup(
              _ => _.GetResult(
                  rowObjectStub.Object,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
          .Returns(new NullBusinessObjectPropertyPathResult())
          .Verifiable();

      var matcher = new BusinessObjectPropertyPathValidationFailureMatcher(propertyPathStub.Object);
      var result = matcher.GetMatchingValidationFailures(rowObjectStub.Object, validationResultStub.Object);

      Assert.That(result, Is.Empty);
      propertyPathStub.Verify();
    }

    [Test]
    public void GetMatchingValidationFailures_ReturnsCollectionFromGetValidationFailures ()
    {
      var rowObjectStub = new Mock<IBusinessObject>();

      var propertyPathResultStub = new Mock<IBusinessObjectPropertyPathResult>();
      var resultObjectStub = new Mock<IBusinessObject>();
      propertyPathResultStub
          .Setup(_ => _.ResultObject)
          .Returns(resultObjectStub.Object);

      var resultPropertyStub = new Mock<IBusinessObjectProperty>();
      propertyPathResultStub
          .Setup(_ => _.ResultProperty)
          .Returns(resultPropertyStub.Object);

      var propertyPathStub = new Mock<IBusinessObjectPropertyPath>();
      propertyPathStub
          .Setup(
              _ => _.GetResult(
                  rowObjectStub.Object,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
          .Returns(propertyPathResultStub.Object);

      var validationResultStub = new Mock<IBusinessObjectValidationResult>();
      var validationFailures = new[] { BusinessObjectValidationFailure.Create("Error"), BusinessObjectValidationFailure.Create("Another one") };
      validationResultStub
          .Setup(_ => _.GetValidationFailures(resultObjectStub.Object, resultPropertyStub.Object, true))
          .Returns(validationFailures);

      var matcher = new BusinessObjectPropertyPathValidationFailureMatcher(propertyPathStub.Object);
      var result = matcher.GetMatchingValidationFailures(rowObjectStub.Object, validationResultStub.Object);

      Assert.That(result, Is.EqualTo(validationFailures));
    }
  }
}
