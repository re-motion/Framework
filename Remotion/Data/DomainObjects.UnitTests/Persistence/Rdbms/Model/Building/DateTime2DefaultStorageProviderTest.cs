// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building;

[TestFixture]
public class DateTime2DefaultStorageProviderTest
{
  [Test]
  public void StorageTypeName ()
  {
    var provider = new DateTime2DefaultStorageTypeProvider();

    Assert.That(provider.StorageTypeName, Is.EqualTo("datetime2"));
  }

  [Test]
  public void DbType ()
  {
    var provider = new DateTime2DefaultStorageTypeProvider();

    Assert.That(provider.DbType, Is.EqualTo(System.Data.DbType.DateTime2));
  }
}
