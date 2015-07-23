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

namespace Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class SubTransactionInitializeTest : TestBase
  {
    private TestHelper _testHelper;
    private IClientTransactionExtension _extension;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new TestHelper ();
      _extension = new SecurityClientTransactionExtension ();
    }

    [Test]
    public void SubTransactionInitialize_AddsExtensionToSubTransaction ()
    {
      var subTransaction = ClientTransaction.CreateRootTransaction();
      _testHelper.AddExtension (_extension);
      
      _extension.SubTransactionInitialize (_testHelper.Transaction, subTransaction);
      
      Assert.That (subTransaction.Extensions[_extension.Key], Is.SameAs (_extension));
    }

    [Test]
    public void SubTransactionInitialize_IgnoresExtensionIfAlreadyExists ()
    {
      var subTransaction = ClientTransaction.CreateRootTransaction();
      _testHelper.AddExtension (_extension);

      var existingExtension = new SecurityClientTransactionExtension();
      subTransaction.Extensions.Add (existingExtension);

      _extension.SubTransactionInitialize (_testHelper.Transaction, subTransaction);
      
      Assert.That (subTransaction.Extensions[_extension.Key], Is.SameAs (existingExtension));
    }
  }
}
