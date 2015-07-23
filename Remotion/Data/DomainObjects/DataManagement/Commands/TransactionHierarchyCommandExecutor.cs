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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Provides functionality for executing <see cref="IDataManagementCommand"/> instances on a whole <see cref="ClientTransaction"/> hierarchy.
  /// All commands are executed as a single, combined command - ordered from the leaf transaction to the root transaction -, i.e., first all the
  /// <see cref="IDataManagementCommand.Begin"/> logic is executed over the hierarchy, then all the <see cref="IDataManagementCommand.Perform"/>
  /// logic (the transactions are made writeable for this), then all the <see cref="IDataManagementCommand.End"/> logic.
  /// </summary>
  public class TransactionHierarchyCommandExecutor
  {
    private readonly Func<ClientTransaction, IDataManagementCommand> _commandFactory;

    public TransactionHierarchyCommandExecutor (Func<ClientTransaction, IDataManagementCommand> commandFactory)
    {
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);
      _commandFactory = commandFactory;
    }

    public bool TryExecuteCommandForTransactionHierarchy (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      var combinedCommand = CreateCombinedCommand (clientTransaction);
      if (!combinedCommand.CanExecute ())
        return false;

      combinedCommand.NotifyAndPerform();
      return true;
    }

    public void ExecuteCommandForTransactionHierarchy (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      var combinedCommand = CreateCombinedCommand (clientTransaction);
      combinedCommand.NotifyAndPerform();
    }

    private CompositeCommand CreateCombinedCommand (ClientTransaction clientTransaction)
    {
      var allCommands = from tx in clientTransaction.LeafTransaction.CreateSequence (tx => tx.ParentTransaction)
                        let command = _commandFactory (tx)
                        select new UnlockingCommandDecorator (command, tx);
      return new CompositeCommand (allCommands.Cast<IDataManagementCommand>());
    }

    private class UnlockingCommandDecorator : DataManagementCommandDecoratorBase
    {
      private readonly ClientTransaction _transactionToBeUnlocked;

      public UnlockingCommandDecorator (IDataManagementCommand decoratedCommand, ClientTransaction transactionToBeUnlocked)
          : base(decoratedCommand)
      {
        _transactionToBeUnlocked = transactionToBeUnlocked;
      }

      public override void Perform ()
      {
        using (_transactionToBeUnlocked.HierarchyManager.UnlockIfRequired())
        {
          base.Perform();
        }
      }

      protected override IDataManagementCommand Decorate (IDataManagementCommand decoratedCommand)
      {
        ArgumentUtility.CheckNotNull ("decoratedCommand", decoratedCommand);
        return new UnlockingCommandDecorator (decoratedCommand, _transactionToBeUnlocked);
      }
    }
  }
}