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
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Defines commonly used transaction modes that can be specified when a new <see cref="WxeFunction"/> is created.
  /// </summary>
  /// <typeparam name="TTransactionFactory">The type of the transaction factory to be used to create the transactions. This depends on the underlying
  /// persistence management framework. For re-store, specify <see cref="T:Remotion.Data.DomainObjects.ClientTransactionFactory"/>.</typeparam>
  public abstract class WxeTransactionMode<TTransactionFactory>
      where TTransactionFactory : ITransactionFactory, new ()
  {
    /// <summary>
    /// Indicates that a <see cref="WxeFunction"/> will not create any transactions when executed; instead, it will use the transaction of the
    /// <see cref="WxeStep.ParentFunction"/>, or no transaction if no parent exists.
    /// </summary>
    public static readonly ITransactionMode None = new NoneTransactionMode();

    /// <summary>
    /// Indicates that a <see cref="WxeFunction"/> will create a new root transaction when executed. The transaction must be explicitly committed,
    /// otherwise all changes made in its scope will be discarded.
    /// </summary>
    public static readonly ITransactionMode CreateRoot = new CreateRootTransactionMode(false, new TTransactionFactory ());

    /// <summary>
    /// Indicates that a <see cref="WxeFunction"/> will create a new root transaction when executed. The transaction can be explicitly committed, and
    /// it will be committed automatically when the function successfuly finishes execution. When the function finishes with an exception, the
    /// transaction will not be committed automatically.
    /// </summary>
    public static readonly ITransactionMode CreateRootWithAutoCommit = new CreateRootTransactionMode (true, new TTransactionFactory ());

    /// <summary>
    /// Indicates that a <see cref="WxeFunction"/> will create a new transaction when executed. If a <see cref="WxeStep.ParentFunction"/> exists which 
    /// has an associated transaction, the new transaction will be created as a child of that transaction. If no parent transaction exists, a root
    /// transaction will be created. The new transaction must be explicitly committed, otherwise all changes made in its scope will be discarded.
    /// </summary>
    public static readonly ITransactionMode CreateChildIfParent = new CreateChildIfParentTransactionMode(false, new TTransactionFactory());

    /// <summary>
    /// Indicates that a <see cref="WxeFunction"/> will create a new transaction when executed. If a <see cref="WxeStep.ParentFunction"/> exists which 
    /// has an associated transaction, the new transaction will be created as a child of that transaction. If no parent transaction exists, a root
    /// transaction will be created. The new transaction can be explicitly committed, and
    /// it will be committed automatically when the function successfuly finishes execution. When the function finishes with an exception, the
    /// transaction will not be committed automatically.
    /// </summary>
    public static readonly ITransactionMode CreateChildIfParentWithAutoCommit = new CreateChildIfParentTransactionMode (true, new TTransactionFactory());

    protected WxeTransactionMode ()
    {
    }

    internal abstract void OnlyAllowAbstractInheritance();
  }
}
