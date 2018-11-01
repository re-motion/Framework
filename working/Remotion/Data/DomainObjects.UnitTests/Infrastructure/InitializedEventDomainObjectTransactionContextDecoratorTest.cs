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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class InitializedEventDomainObjectTransactionContextDecoratorTest
  {
    [Test]
    public void AllowedMembers ()
    {
      CheckAllowed (ctx => ctx.ClientTransaction, ClientTransaction.CreateRootTransaction());
      CheckAllowed (ctx => ctx.IsInvalid, true);
    }

    [Test]
    public void ForbiddenMembers ()
    {
      CheckForbidden (ctx => Dev.Null = ctx.State);
      CheckForbidden (ctx => Dev.Null = ctx.Timestamp);
      CheckForbidden (ctx => ctx.RegisterForCommit ());
      CheckForbidden (ctx => ctx.EnsureDataAvailable ());
      CheckForbidden (ctx => ctx.TryEnsureDataAvailable ());
    }

    private void CheckAllowed<T> (Func<IDomainObjectTransactionContext, T> func, T mockResult)
    {
      var contextMock = MockRepository.GenerateMock<IDomainObjectTransactionContext> ();
      contextMock.Expect (mock => func (mock)).Return (mockResult);
      contextMock.Replay ();

      var result = func (new InitializedEventDomainObjectTransactionContextDecorator (contextMock));

      contextMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (result));
    }

    private void CheckForbidden (Action<IDomainObjectTransactionContext> action)
    {
      var contextMock = MockRepository.GenerateMock<IDomainObjectTransactionContext> ();

      Assert.That (
          () => action (new InitializedEventDomainObjectTransactionContextDecorator (contextMock)), 
          Throws.InvalidOperationException.With.Message.EqualTo (
              "While the OnReferenceInitializing event is executing, this member cannot be used."));

      contextMock.AssertWasNotCalled (action);
    }
  }
}