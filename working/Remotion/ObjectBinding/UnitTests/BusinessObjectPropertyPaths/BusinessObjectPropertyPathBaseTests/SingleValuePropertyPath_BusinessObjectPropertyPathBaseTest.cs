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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.BusinessObjectPropertyPathBaseTests
{
  [TestFixture]
  public class SingleValuePropertyPath_BusinessObjectPropertyPathBaseTest
  {
    private BusinessObjectPropertyPathTestHelper _testHelper;
    
    [SetUp]
    public void SetUp ()
    {
      _testHelper = new BusinessObjectPropertyPathTestHelper ();
    }

    [Test]
    public void GetValue ()
    {
      var path = new TestableBusinessObjectPropertyPathBase (_testHelper.Property);
      _testHelper.ReplayAll();

      var actual = path.GetResult (
          _testHelper.BusinessObjectWithIdentity,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      _testHelper.VerifyAll();

      Assert.That (actual, Is.InstanceOf<EvaluatedBusinessObjectPropertyPathResult>());
      Assert.That (actual.ResultObject, Is.SameAs (_testHelper.BusinessObjectWithIdentity));
      Assert.That (actual.ResultProperty, Is.SameAs (_testHelper.Property));
    }

    [Test]
    public void GetValue_LastPropertyIsReferenceProperty ()
    {
      var path = new TestableBusinessObjectPropertyPathBase (_testHelper.ReferenceProperty);
      _testHelper.ReplayAll();

      var actual = path.GetResult (
          _testHelper.BusinessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      _testHelper.VerifyAll();

      Assert.That (actual, Is.InstanceOf<EvaluatedBusinessObjectPropertyPathResult>());
      Assert.That (actual.ResultObject, Is.SameAs (_testHelper.BusinessObject));
      Assert.That (actual.ResultProperty, Is.SameAs (_testHelper.ReferenceProperty));
    }
  }
}
