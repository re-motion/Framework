// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Data;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests;

[TestFixture]
public class DateTimeStorageTypeAttributeTest
{
  [Test]
  [TestCase(DateTimeStorageType.DateTime)]
  [TestCase(DateTimeStorageType.DateTime2)]
  public void Ctor (DateTimeStorageType storageType)
  {
    var attribute = new DateTimeStorageTypeAttribute(storageType);

    Assert.That(attribute.StorageType, Is.EqualTo(storageType));
  }
}
