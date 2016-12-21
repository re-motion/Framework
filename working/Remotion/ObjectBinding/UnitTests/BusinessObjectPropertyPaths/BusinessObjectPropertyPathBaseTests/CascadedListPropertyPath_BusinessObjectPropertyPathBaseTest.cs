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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.BusinessObjectPropertyPathBaseTests
{
  [TestFixture]
  public class CascadedListPropertyPath_BusinessObjectPropertyPathBaseTest
  {
    private BusinessObjectPropertyPathTestHelper _testHelper;
    private BusinessObjectPropertyPathBase _path;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new BusinessObjectPropertyPathTestHelper();
      _path = new TestableBusinessObjectPropertyPathBase (_testHelper.ReferenceListProperty, _testHelper.Property);
    }

    [Test]
    public void GetValue_ListProperty_ReturnsFirstItem ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferenceListPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (_testHelper.BusinessObjectWithIdentityList);
      }
      _testHelper.ReplayAll();

      var actual = _path.GetResult (
          _testHelper.BusinessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      _testHelper.VerifyAll();

      Assert.That (actual, Is.InstanceOf<EvaluatedBusinessObjectPropertyPathResult>());
      Assert.That (actual.ResultObject, Is.SameAs (_testHelper.BusinessObjectWithIdentity));
      Assert.That (actual.ResultProperty, Is.SameAs (_testHelper.Property));
    }

    [Test]
    public void GetValue_ListProperty_ThrowsInvalidOperationException ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferenceListPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (_testHelper.BusinessObjectWithIdentityList);
      }
      _testHelper.ReplayAll();

      Assert.That (
          () =>
          _path.GetResult (
              _testHelper.BusinessObject,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties),
          Throws.InvalidOperationException.With.Message
                .EqualTo ("Property #0 of property path 'Identifier' is not a single-value property."));
    }

    private void ExpectOnceOnReferenceListPropertyIsAccessible (bool returnValue)
    {
      _testHelper.ExpectOnceOnIsAccessible (
          _testHelper.BusinessObjectClass,
          _testHelper.BusinessObject,
          _testHelper.ReferenceListProperty, returnValue);
    }

    private void ExpectOnceOnBusinessObjectGetProperty (IBusinessObjectWithIdentity[] businessObjectsWithIdentity)
    {
      _testHelper.ExpectOnceOnGetProperty (_testHelper.BusinessObject, _testHelper.ReferenceListProperty, businessObjectsWithIdentity);
    }
  }
}