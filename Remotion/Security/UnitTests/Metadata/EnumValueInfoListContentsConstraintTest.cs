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
using NUnit.Framework.Constraints;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class EnumValueInfoListContentsConstraintTest
  {
    private EnumValueInfo _enumValueInfo1;
    private EnumValueInfo _enumValueInfo2;
    private List<EnumValueInfo> _list;

    public EnumValueInfoListContentsConstraintTest ()
    {
    }

    [SetUp]
    public void SetUp ()
    {
      _enumValueInfo1 = new EnumValueInfo("TypeName", "First", 1);
      _enumValueInfo2 = new EnumValueInfo("TypeName", "Second", 2);
      _list = new List<EnumValueInfo>();
      _list.Add(_enumValueInfo1);
      _list.Add(_enumValueInfo2);
    }

    [Test]
    public void AssertWithValidValue ()
    {
      var constraint = new EnumValueInfoListContentsConstraint("First");
      Assert.That(_list, constraint);
    }

    [Test]
    public void AssertWithListEmpty ()
    {
      var constraint = new EnumValueInfoListContentsConstraint("First");
      Assert.That(constraint.ApplyTo(new List<EnumValueInfo>()).IsSuccess, Is.False);
    }

    [Test]
    public void AssertWithInvalidValue ()
    {
      var constraint = new EnumValueInfoListContentsConstraint("Other");
      Assert.That(constraint.ApplyTo(_list).IsSuccess, Is.False);
    }

    [Test]
    public void GetMessage ()
    {
      var writerMock = new Mock<MessageWriter>();
      writerMock.Setup(mock => mock.Write("Expected: ExpectedName	 but was: First, Second")).Verifiable();

      var constraint = new EnumValueInfoListContentsConstraintResult(null, _list, "ExpectedName", false);
      constraint.WriteMessageTo(writerMock.Object);
      
      writerMock.Verify();
    }
    
  }
}
