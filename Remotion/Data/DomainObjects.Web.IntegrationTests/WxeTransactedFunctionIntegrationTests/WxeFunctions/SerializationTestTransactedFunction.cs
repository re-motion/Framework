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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions
{
  [Serializable]
  public class SerializationTestTransactedFunction: WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SerializationTestTransactedFunction ()
        : base (WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit)
    {
    }

    public bool FirstStepExecuted;
    public bool SecondStepExecuted;
    public ClientTransaction TransactionBeforeSerialization;

    public byte[] SerializedSelf;

    // methods and properties

    private void Step1()
    {
      Assert.That (FirstStepExecuted, Is.False);
      FirstStepExecuted = true;

      TransactionBeforeSerialization = ClientTransactionScope.CurrentTransaction;
    }

    private void Step2 ()
    {
      Assert.That (FirstStepExecuted, Is.True);
      Assert.That (SecondStepExecuted, Is.False);

      SerializedSelf = Serializer.Serialize (this); // freeze at this point of time

      SecondStepExecuted = true;
      Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (TransactionBeforeSerialization));
    }
  }
}
