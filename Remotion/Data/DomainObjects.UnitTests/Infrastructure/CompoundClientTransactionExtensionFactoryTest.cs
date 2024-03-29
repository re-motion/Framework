﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class CompoundClientTransactionExtensionFactoryTest
  {
    [Test]
    public void Initialize ()
    {
      var innerFactories = new[]
                           {
                               new Mock<IClientTransactionExtensionFactory>().Object,
                               new Mock<IClientTransactionExtensionFactory>().Object
                           };
      var compoundFactory = new CompoundClientTransactionExtensionFactory(innerFactories);

      Assert.That(compoundFactory.ClientTransactionExtensionFactories, Is.Not.SameAs(innerFactories));
      Assert.That(compoundFactory.ClientTransactionExtensionFactories, Is.EqualTo(innerFactories));
    }

    [Test]
    public void CreateClientTransactionExtensions ()
    {
      var firstExtensions =  new[]
                           {
                               new Mock<IClientTransactionExtension>().Object,
                               new Mock<IClientTransactionExtension>().Object
                           };
      var secondExtensions =  new[]
                           {
                               new Mock<IClientTransactionExtension>().Object,
                               new Mock<IClientTransactionExtension>().Object
                           };
      var innerFactories = new[]
                           {
                               new Mock<IClientTransactionExtensionFactory>(),
                               new Mock<IClientTransactionExtensionFactory>()
                           };

      var clientTransaction = ClientTransaction.CreateRootTransaction();
      innerFactories[0].Setup(_ => _.CreateClientTransactionExtensions(clientTransaction)).Returns(firstExtensions);
      innerFactories[1].Setup(_ => _.CreateClientTransactionExtensions(clientTransaction)).Returns(secondExtensions);

      var compoundFactory = new CompoundClientTransactionExtensionFactory(innerFactories.Select(f => f.Object));

      var extensions = compoundFactory.CreateClientTransactionExtensions(clientTransaction);

      Assert.That(extensions, Is.EqualTo(firstExtensions.Concat(secondExtensions)));
    }
  }
}
