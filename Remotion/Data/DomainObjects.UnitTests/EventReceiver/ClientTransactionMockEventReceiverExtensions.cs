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
using Moq.Language.Flow;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public static class IClientTransactionMockEventReceiverExtensions
  {
    public static ISetup<IClientTransactionMockEventReceiver> SetupLoaded (
        this MockWrapper<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        params DomainObject[] domainObjects)
    {
      return fluent.Setup(mock => mock.Loaded(sender, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))));
    }

    public static ISetup<IClientTransactionMockEventReceiver> SetupRollingBack (
        this MockWrapper<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        params DomainObject[] domainObjects)
    {
      return fluent.Setup(mock => mock.RollingBack(sender, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))));
    }

    public static void VerifyRollingBack (
        this Mock<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        DomainObject[] domainObjects,
        Times times)
    {
      fluent.Verify(mock => mock.RollingBack(sender, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))), times);
    }

    public static ISetup<IClientTransactionMockEventReceiver> SetupRolledBack (
        this MockWrapper<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        params DomainObject[] domainObjects)
    {
      return fluent.Setup(mock => mock.RolledBack(sender, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))));
    }

    public static void VerifyRolledBack (
        this Mock<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        DomainObject[] domainObjects,
        Times times)
    {
      fluent.Verify(mock => mock.RolledBack(sender, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))), times);
    }

    public static ISetup<IClientTransactionMockEventReceiver> SetupCommitting (
        this MockWrapper<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        params DomainObject[] domainObjects)
    {
      return fluent.Setup(mock => mock.Committing(sender, It.Is<ClientTransactionCommittingEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))));
    }

    public static void VerifyCommitting (
        this Mock<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        DomainObject[] domainObjects,
        Times times)
    {
      fluent.Verify(mock => mock.Committing(sender, It.Is<ClientTransactionCommittingEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))), times);
    }

    public static ISetup<IClientTransactionMockEventReceiver> SetupCommitted (
        this MockWrapper<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        params DomainObject[] domainObjects)
    {
      return fluent.Setup(mock => mock.Committed(sender, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))));
    }

    public static void VerifyCommitted (
        this Mock<IClientTransactionMockEventReceiver> fluent,
        ClientTransaction sender,
        DomainObject[] domainObjects,
        Times times)
    {
      fluent.Verify(mock => mock.Committed(sender, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SetEquals(domainObjects))), times);
    }
  }
}
