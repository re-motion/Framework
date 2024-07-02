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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement
{
  /// <summary>
  /// Represents and tracks a <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  public class ClientTransactionHierarchy : IClientTransactionHierarchy
  {
    [NotNull]
    private readonly ClientTransaction _rootTransaction;

    [NotNull]
    private ClientTransaction _leafTransaction;

    [NotNull]
    private ClientTransaction _activeTransaction;

    public ClientTransactionHierarchy (ClientTransaction rootTransaction)
    {
      ArgumentUtility.CheckNotNull("rootTransaction", rootTransaction);
      _rootTransaction = rootTransaction;
      _leafTransaction = rootTransaction;
      _activeTransaction = rootTransaction;
    }

    public ClientTransaction RootTransaction
    {
      get { return _rootTransaction; }
    }

    public ClientTransaction LeafTransaction
    {
      get { return _leafTransaction; }
    }

    public ClientTransaction ActiveTransaction
    {
      get { return _activeTransaction; }
    }

    public void AppendLeafTransaction (ClientTransaction leafTransaction)
    {
      ArgumentUtility.CheckNotNull("leafTransaction", leafTransaction);

      if (leafTransaction.ParentTransaction != _leafTransaction)
        throw new ArgumentException("The new LeafTransaction must have the previous LeafTransaction as its parent.", "leafTransaction");

      _leafTransaction = leafTransaction;
      Assertion.IsTrue(_leafTransaction.RootTransaction == _rootTransaction);
    }

    public void RemoveLeafTransaction ()
    {
      if (_leafTransaction == _rootTransaction)
        throw new InvalidOperationException("Cannot remove the root transaction.");

      var parentTransaction = _leafTransaction.ParentTransaction;
      Assertion.IsNotNull(parentTransaction);

      _leafTransaction = parentTransaction;
    }

    public IDisposable ActivateTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);

      if (clientTransaction.RootTransaction != _rootTransaction)
        throw new ArgumentException("The activated transaction must be from this ClientTransactionHierarchy.", "clientTransaction");

      var previousActivatedTransaction = _activeTransaction;
      _activeTransaction = clientTransaction;

      return new ActivationScope(this, clientTransaction, previousActivatedTransaction);
    }

    private sealed class ActivationScope : IDisposable
    {
      private readonly ClientTransactionHierarchy _hierarchy;
      private readonly ClientTransaction _expectedActivatedTransaction;
      private readonly ClientTransaction _previousActivatedTransaction;

      public ActivationScope (
          ClientTransactionHierarchy hierarchy, ClientTransaction expectedActivatedTransaction, ClientTransaction previousActivatedTransaction)
      {
        ArgumentUtility.CheckNotNull("hierarchy", hierarchy);
        ArgumentUtility.CheckNotNull("expectedActivatedTransaction", expectedActivatedTransaction);
        ArgumentUtility.CheckNotNull("previousActivatedTransaction", previousActivatedTransaction);

        _hierarchy = hierarchy;
        _expectedActivatedTransaction = expectedActivatedTransaction;
        _previousActivatedTransaction = previousActivatedTransaction;
      }

      public void Dispose ()
      {
        if (_hierarchy._activeTransaction != _expectedActivatedTransaction)
          throw new InvalidOperationException("The scopes returned by ActivateTransaction must be disposed inside out.");

        _hierarchy._activeTransaction = _previousActivatedTransaction;
      }
    }
  }
}
