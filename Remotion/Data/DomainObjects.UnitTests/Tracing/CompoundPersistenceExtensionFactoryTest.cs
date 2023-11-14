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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class CompoundPersistenceExtensionFactoryTest
  {
    [Test]
    public void Initialize ()
    {
      var innerFactories = new[]
                           {
                               new Mock<IPersistenceExtensionFactory>().Object,
                               new Mock<IPersistenceExtensionFactory>().Object
                           };
      var compoundFactory = new CompoundPersistenceExtensionFactory(innerFactories);

      Assert.That(compoundFactory.PersistenceExtensionFactories, Is.Not.SameAs(innerFactories));
      Assert.That(compoundFactory.PersistenceExtensionFactories, Is.EqualTo(innerFactories));
    }

    [Test]
    public void CreateClientTransactionExtensions ()
    {
      var firstExtensions =  new[]
                           {
                               new Mock<IPersistenceExtension>().Object,
                               new Mock<IPersistenceExtension>().Object
                           };
      var secondExtensions =  new[]
                           {
                               new Mock<IPersistenceExtension>().Object,
                               new Mock<IPersistenceExtension>().Object
                           };
      var innerFactories = new[]
                           {
                               new Mock<IPersistenceExtensionFactory>(),
                               new Mock<IPersistenceExtensionFactory>()
                           };

      var clientTransactionID = Guid.NewGuid();
      innerFactories[0].Setup(_ => _.CreatePersistenceExtensions(clientTransactionID)).Returns(firstExtensions);
      innerFactories[1].Setup(_ => _.CreatePersistenceExtensions(clientTransactionID)).Returns(secondExtensions);

      var compoundFactory = new CompoundPersistenceExtensionFactory(innerFactories.Select(f => f.Object));

      var extensions = compoundFactory.CreatePersistenceExtensions(clientTransactionID);

      Assert.That(extensions, Is.EqualTo(firstExtensions.Concat(secondExtensions)));
    }
  }
}
