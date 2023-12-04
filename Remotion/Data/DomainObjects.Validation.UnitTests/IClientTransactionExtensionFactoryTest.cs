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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class IClientTransactionExtensionFactoryTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance ()
    {
      var factory = _serviceLocator.GetInstance<IClientTransactionExtensionFactory>();

      Assert.That(factory, Is.TypeOf<CompoundClientTransactionExtensionFactory>());
      var clientTransactionExtensionFactories = ((CompoundClientTransactionExtensionFactory)factory).ClientTransactionExtensionFactories;
      var factoryTypes = clientTransactionExtensionFactories.Select(f => f.GetType()).ToList();
      Assert.That(factoryTypes, Has.Member(typeof(ValidationClientTransactionExtensionFactory)));
      Assert.That(factoryTypes, Has.Member(typeof(CommitValidationClientTransactionExtensionFactory)));
      Assert.That(
          factoryTypes.IndexOf(typeof(ValidationClientTransactionExtensionFactory)),
          Is.LessThan(factoryTypes.IndexOf(typeof(CommitValidationClientTransactionExtensionFactory))));
    }
  }
}
