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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions
{
  public class TestTransaction: ITransaction
  {
    [ThreadStatic]
    private static TestTransaction _current;

    public static TestTransaction Current
    {
      get { return _current; }
      set { _current = value; }
    }

    private bool _isRolledBack;
    private bool _isCommitted;
    private bool _isReleased;
    private bool _isReset;
    private bool _canCreateChild;
    private TestTransaction _child;
    private TestTransaction _parent;
    private bool _isChild;
    private bool _throwOnCommit;
    private bool _throwOnRollback;
    private bool _throwOnRelease;
    private bool _isReadOnly;
    private bool _hasUncommittedChanges;
    private readonly List<object> _registeredObjects = new List<object>();

    public event EventHandler Committed;
    public event EventHandler RolledBack;

    public TestTransaction ()
    {
    }

    public ITransactionScope EnterScope ()
    {
      return new TestTransactionScope(this);
    }

    public TTransaction To<TTransaction> ()
    {
      ArgumentUtility.CheckTypeIsAssignableFrom("TTransaction", typeof(TTransaction), typeof(TestTransaction));
      return (TTransaction)(object)this;
    }

    public void Rollback ()
    {
      if (Current != this)
        throw new InvalidOperationException("Current transaction is not this transaction.");
      if (ThrowOnRollback)
        throw new RollbackException();
      _isRolledBack = true;
      if (RolledBack != null)
        RolledBack(this, EventArgs.Empty);
    }

    public void Commit ()
    {
      if (Current != this)
        throw new InvalidOperationException("Current transaction is not this transaction.");
      if (ThrowOnCommit)
        throw new CommitException();
      _isCommitted = true;
      if (Committed != null)
        Committed(this, EventArgs.Empty);
    }

    public ITransaction CreateChild ()
    {
      _child = new TestTransaction();
      _child._isChild = true;
      _child._parent = this;
      return _child;
    }

    public bool IsChild
    {
      get { return _isChild; }
    }

    public bool HasUncommittedChanges
    {
      get { return _hasUncommittedChanges; }
      set { _hasUncommittedChanges = value; }
    }

    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    public bool CanCreateChild
    {
      get { return _canCreateChild; }
      set { _canCreateChild = value; }
    }

    public void Release ()
    {
      if (ThrowOnRelease)
        throw new ReleaseException();

      if (_child != null)
      {
        _child._parent = null;
        _child = null;
      }
      _isReleased = true;
    }

    public void Reset ()
    {
      Release();
      _isReset = true;
    }

    public void EnsureCompatibility (IEnumerable objects)
    {
      foreach (var obj in objects)
        _registeredObjects.Add(obj);
    }

    public ITransaction Parent
    {
      get { return _parent; }
    }

    public bool IsRolledBack
    {
      get { return _isRolledBack; }
    }

    public bool IsCommitted
    {
      get { return _isCommitted; }
    }

    public bool IsReleased
    {
      get { return _isReleased; }
    }

    public bool IsReset
    {
      get { return _isReset; }
    }

    public bool ThrowOnCommit
    {
      get { return _throwOnCommit; }
      set { _throwOnCommit = value; }
    }

    public bool ThrowOnRollback
    {
      get { return _throwOnRollback; }
      set { _throwOnRollback = value; }
    }

    public bool ThrowOnRelease
    {
      get { return _throwOnRelease; }
      set { _throwOnRelease = value; }
    }

    public List<object> RegisteredObjects
    {
      get { return _registeredObjects; }
    }
  }
}
