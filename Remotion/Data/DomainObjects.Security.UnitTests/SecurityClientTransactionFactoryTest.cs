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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTransactionFactoryTest : TestBase
  {
    private TestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new TestHelper();
    }

    [Test]
    public void CreateRootTransaction ()
    {
      ITransactionFactory factory = new SecurityClientTransactionFactory();

      _testHelper.SetupSecurityIoCConfiguration();
      ITransaction transaction;
      try
      {
        transaction = factory.CreateRootTransaction();
      }
      finally
      {
        _testHelper.TearDownSecurityIoCConfiguration();
      }

      var clientTransaction = transaction.To<ClientTransaction>();
      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy(clientTransaction);
      Assert.That(persistenceStrategy, Is.InstanceOf(typeof(RootPersistenceStrategy)));
      Assert.That(
          clientTransaction.Extensions,
          Has.Some.InstanceOf(typeof(SecurityClientTransactionExtension))
              .With.Property("Key").EqualTo(typeof(SecurityClientTransactionExtension).FullName));
    }
  }
}
