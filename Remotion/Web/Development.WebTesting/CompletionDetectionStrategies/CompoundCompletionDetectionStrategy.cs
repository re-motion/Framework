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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionStrategies
{
  /// <summary>
  /// Allows to use more than one <see cref="ICompletionDetectionStrategy"/>, blocks until all of them signal completion.
  /// </summary>
  public class CompoundCompletionDetectionStrategy : ICompletionDetectionStrategy
  {
    private readonly ICompletionDetectionStrategy[] _strategies;

    public CompoundCompletionDetectionStrategy ([NotNull] params ICompletionDetectionStrategy[] strategies)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("strategies", strategies);

      _strategies = strategies;
    }

    /// <inheritdoc/>
    public object PrepareWaitForCompletion (PageObjectContext context)
    {
      var states = new List<object>();

      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var strategy in _strategies)
      {
        var state = strategy.PrepareWaitForCompletion (context);
        states.Add (state);
      }

      return states;
    }

    /// <inheritdoc/>
    public void WaitForCompletion (PageObjectContext context, object state)
    {
      var states = (List<object>) state;
      Assertion.IsNotNull (states, "The state should never be null - developer error.");

      var stragiesWithState = _strategies.Zip (states, (s, ss) => new { Strategy = s, State = ss });
      foreach (var strategyWithState in stragiesWithState)
        strategyWithState.Strategy.WaitForCompletion (context, strategyWithState.State);
    }
  }
}