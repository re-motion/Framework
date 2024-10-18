// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building;

[TestFixture]
public class DateTimeDefaultStorageProviderTest
{
  [Test]
  public void StorageTypeName ()
  {
    var provider = new DateTimeDefaultStorageTypeProvider();

    Assert.That(provider.StorageTypeName, Is.EqualTo("datetime"));
  }

  [Test]
  public void DbType ()
  {
    var provider = new DateTimeDefaultStorageTypeProvider();

    Assert.That(provider.DbType, Is.EqualTo(System.Data.DbType.DateTime));
  }
}
