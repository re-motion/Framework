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
using Remotion.Data.DomainObjects.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class CommitValidationClientTransactionExtensionFactoryTest
  {
    [Test]
    public void CreateClientTransactionExtensions_InRootTransaction_CreatesExtension ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var persistableDataValidatorStub = MockRepository.GenerateStub<IPersistableDataValidator>();
      var factory = new CommitValidationClientTransactionExtensionFactory (persistableDataValidatorStub);

      var result = factory.CreateClientTransactionExtensions (clientTransaction).ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      var clientTransactionExtension = result.First();
      Assert.That (clientTransactionExtension, Is.TypeOf<CommitValidationClientTransactionExtension>());
      Assert.That (((CommitValidationClientTransactionExtension) clientTransactionExtension).Validator, Is.SameAs(persistableDataValidatorStub));
    }

    [Test]
    public void CreateClientTransactionExtensions_InSubTransaction_DoesNotCreateExtension ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var subTransaction = clientTransaction.CreateSubTransaction();
      var factory = new CommitValidationClientTransactionExtensionFactory (MockRepository.GenerateStub<IPersistableDataValidator>());

      var result = factory.CreateClientTransactionExtensions (subTransaction).ToArray();

      Assert.That (result, Is.Empty);
    }
  }
}