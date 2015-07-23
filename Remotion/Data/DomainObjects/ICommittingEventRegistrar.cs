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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides an interface allowing event handlers to register objects for additional events to be raised before a <see cref="ClientTransaction.Commit"/> 
  /// operation is performed.
  /// </summary>
  public interface ICommittingEventRegistrar
  {
    /// <summary>
    /// Causes the whole committing event chain to be executed one additional time for the given <paramref name="domainObjects"/>.
    /// </summary>
    /// <param name="domainObjects">The <see cref="DomainObject"/> instances for which the event chain should be reexecuted. Each object must be part 
    /// of the commit set, i.e., its state must be <see cref="StateType.New"/>, <see cref="StateType.Changed"/>, or <see cref="StateType.Deleted"/>.
    /// Call <see cref="DomainObjectExtensions.RegisterForCommit"/> to add an <see cref="StateType.Unchanged"/> object to the commit set.</param>
    /// <exception cref="ArgumentException">One of the given objects is not part of the commit set.</exception>
    /// <remarks>
    /// <para>
    /// The committing event chain consists of <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitting"/>, 
    /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.Committing"/>, 
    /// <see cref="ClientTransaction"/>.<see cref="ClientTransaction.Committing"/>, and
    /// <see cref="DomainObject"/>.<see cref="DomainObject.Committing"/>.
    /// Usually, every event handler in the committing event chain receives each object in the commit set exactly once (with the order in which
    /// the <see cref="DomainObject"/>.<see cref="DomainObject.Committing"/> event is raised being undefined). 
    /// For example, when a set of objects (a, b) is in the commit set, the events are raised as follows:
    /// <list type="number">
    /// <item><description>
    /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitting"/> and 
    /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.Committing"/> for (a, b)</description></item>
    /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.Committing"/> for (a, b)</description></item>
    /// <item><description>a.<see cref="DomainObject.Committing"/>, b.<see cref="DomainObject.Committing"/> (order undefined)</description></item>
    /// </list>
    /// If any event handler adds an object c to the commit set (e.g., by changing or creating it), the whole chain starts again, but only for c.
    /// </para>
    /// <para>
    /// If a handler changes one of the objects already in the commit set (i.e., a or b), the chain just proceeds and is not reexecuted. This means 
    /// that it is possible that the state of an object changes after the committing event handlers for it have executed. The purpose of 
    /// <see cref="RegisterForAdditionalCommittingEvents"/> is to force the chain to run again for a given set of objects, allowing the handlers to 
    /// operate on a more recent state than before.
    /// </para>
    /// <para>
    /// If <see cref="RegisterForAdditionalCommittingEvents"/> is called for multiple objects or multiple times (for the same or different objects) 
    /// during one event chain run, or if an event handler adds an object to the commit set, there will be just one additional run of the event chain, 
    /// combining all registered and additional objects. The order in which objects are registered or added to the commit set does not influence the 
    /// order of the <see cref="DomainObject"/>.<see cref="DomainObject.Committing"/> events (which is still undefined):
    /// <list type="number">
    /// <item><description><see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitting"/> and 
    /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.Committing"/> for registered/additional objects from last run</description></item>
    /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.Committing"/> for registered/additional objects from last run</description></item>
    /// <item><description><see cref="DomainObject"/>.<see cref="DomainObject.Committing"/> for registered/additional objects from last run (order undefined)</description></item>
    /// </list>
    /// If during that additional run objects are rescheduled or added, another run will be performed, etc.
    /// </para>
    /// <para>
    /// Calling <see cref="RegisterForAdditionalCommittingEvents"/> for an object added to the commit set in the current event chain run does not 
    /// change the number of additional event chain runs (the object added to the commit set gets handled in an additional run anyway).
    /// </para>
    /// </remarks>
    void RegisterForAdditionalCommittingEvents (params DomainObject[] domainObjects);
  }
}