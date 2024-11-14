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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class CommitValidationClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    [Test]
    public void DefaultKey ()
    {
      Assert.That(CommitValidationClientTransactionExtension.DefaultKey, Is.EqualTo(typeof(CommitValidationClientTransactionExtension).FullName));
    }

    [Test]
    public void Key ()
    {
      var extension = new CommitValidationClientTransactionExtension(new Mock<IPersistableDataValidator>().Object);
      Assert.That(extension.Key, Is.EqualTo(CommitValidationClientTransactionExtension.DefaultKey));
    }

    [Test]
    public void CommitValidate ()
    {
      var data1 = PersistableDataObjectMother.Create();
      var data2 = PersistableDataObjectMother.Create();

      var transaction = ClientTransaction.CreateRootTransaction();

      var validatorMock = new Mock<IPersistableDataValidator>(MockBehavior.Strict);
      var extension = new CommitValidationClientTransactionExtension(validatorMock.Object);

      validatorMock.Setup(mock => mock.Validate(transaction, data1)).Verifiable();
      validatorMock.Setup(mock => mock.Validate(transaction, data2)).Verifiable();

      extension.CommitValidate(transaction, Array.AsReadOnly(new[] { data1, data2 }));

      validatorMock.Verify();
    }
  }
}
