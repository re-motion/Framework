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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class UniqueConstraintDefinitionTest
  {
    private UniqueConstraintDefinition _constraint;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;

    [SetUp]
    public void SetUp ()
    {
      _column1 = ColumnDefinitionObjectMother.CreateColumn("COL1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn("COL2");
      _constraint = new UniqueConstraintDefinition("Test", true, new[] {  _column1, _column2 });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_constraint.ConstraintName, Is.EqualTo("Test"));
      Assert.That(_constraint.IsClustered, Is.True);
      Assert.That(_constraint.Columns, Is.EqualTo(new[] { _column1, _column2 }));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = new Mock<ITableConstraintDefinitionVisitor>(MockBehavior.Strict);

      visitorMock.Setup(mock => mock.VisitUniqueConstraintDefinition(_constraint)).Verifiable();

      _constraint.Accept(visitorMock.Object);

      visitorMock.Verify();
    }
  }
}
