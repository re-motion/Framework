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
using System.Linq;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UberProfIntegration.UnitTests
{
  [TestFixture]
  public class LinqToSqlExtensionFactoryTest : TestBase
  {
    private LinqToSqlExtensionFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new LinqToSqlExtensionFactory();
    }

    [Test]
    public void CreateClientTransactionExtensions_RootTransaction ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();

      var result = _factory.CreateClientTransactionExtensions (clientTransaction).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      var clientTransactionExtension = result.Single();
      Assert.That (clientTransactionExtension, Is.TypeOf<LinqToSqlExtension> ());
      Assert.That (((LinqToSqlExtension) clientTransactionExtension).ClientTransactionID, Is.EqualTo (clientTransaction.ID));
      Assert.That (((LinqToSqlExtension) clientTransactionExtension).AppenderProxy, Is.SameAs (AppenderProxy));
    }

    [Test]
    public void CreateClientTransactionExtensions_SubTransaction ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ().CreateSubTransaction();

      var result = _factory.CreateClientTransactionExtensions (clientTransaction);

      Assert.That (result, Is.Empty);
    }
  }
}