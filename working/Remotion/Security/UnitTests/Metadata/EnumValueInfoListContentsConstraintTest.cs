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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Security.Metadata;
using Rhino.Mocks;

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
      _enumValueInfo1 = new EnumValueInfo ("TypeName", "First", 1);
      _enumValueInfo2 = new EnumValueInfo ("TypeName", "Second", 2);
      _list = new List<EnumValueInfo>();
      _list.Add (_enumValueInfo1);
      _list.Add (_enumValueInfo2);
    }

    [Test]
    public void AssertWithValidValue ()
    {
      var constraint = new EnumValueInfoListContentsConstraint ("First");
      Assert.That (_list, constraint);
    }

    [Test]
    public void AssertWithListEmpty ()
    {
      var constraint = new EnumValueInfoListContentsConstraint ("First");
      Assert.That (constraint.Matches (new List<EnumValueInfo> ()), Is.False);
    }

    [Test]
    public void AssertWithInvalidValue ()
    {
      var constraint = new EnumValueInfoListContentsConstraint ("Other");
      Assert.That (constraint.Matches(_list), Is.False);
    }

    [Test]
    public void GetMessage ()
    {
      var writerMock = MockRepository.GenerateMock<TextMessageWriter>();
      writerMock.Expect (mock => mock.Write ("Expected: ExpectedName	 but was: First, Second"));
      writerMock.Replay();

      var constraint = new EnumValueInfoListContentsConstraint ("ExpectedName");
      PrivateInvoke.SetNonPublicField (constraint, "actual", _list);
      constraint.WriteMessageTo (writerMock);
      
      writerMock.VerifyAllExpectations();
    }
    
  }
}
