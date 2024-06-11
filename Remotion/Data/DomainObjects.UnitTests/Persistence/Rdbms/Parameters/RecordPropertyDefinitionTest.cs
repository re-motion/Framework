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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class RecordPropertyDefinitionTest
{
  [Test]
  public void ScalarAsValue_Initialize ()
  {
    var storagePropertyDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateGuidStorageProperty("Test");

    var propertyDefinition = RecordPropertyDefinition.ScalarAsValue(storagePropertyDefinition);

    Assert.That(propertyDefinition.PropertyName, Is.EqualTo("Self"));
    Assert.That(propertyDefinition.StoragePropertyDefinition, Is.SameAs(storagePropertyDefinition));
  }

  [Test]
  [TestCase("Text")]
  [TestCase(42)]
  [TestCase(new byte[] { 1, 1, 2, 3, 5, 8, 13, 21 })]
  [TestCase(new char[] { 'c', 'h', 'a', 'r', 's' })]
  [CLSCompliant(false)]
  public void Scalar_GetValue_ReturnsInput (object item)
  {
    var storagePropertyDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateGuidStorageProperty("Test");

    var propertyDefinition = RecordPropertyDefinition.ScalarAsValue(storagePropertyDefinition);

    var result = propertyDefinition.GetValue(item);
    Assert.That(result, Is.EqualTo(item));
  }
}
