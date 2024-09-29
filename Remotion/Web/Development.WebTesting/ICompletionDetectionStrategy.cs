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
using Microsoft.Extensions.Logging;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Completion detection ensures that an action (e.g. a click on a button) and all its effects (e.g. a postback, an AJAX request, etc.) have been
  /// completed and execution of the next action can begin safely.
  /// </summary>
  /// <remarks>
  /// Completion detection is an important part of race condition prevention in web tests. When beginning a new action, we must be sure that the
  /// previous action has completed and the DOM is not modified anymore. Most other frameworks simply wait until the query for the DOM element in
  /// question succeeds and immediately start the interaction. However, that is especially not safe for ASP.NET WebForms pages where postbacks often
  /// display the very same DOM elements again.
  /// </remarks>
  public interface ICompletionDetectionStrategy
  {
    // TODO RM-8106: Improve nullability of completion detection strategy API's

    /// <summary>
    /// Called immediately before the action is performed. This method should capture all state (e.g. a page sequence number) required for
    /// subsequently determining whether the action has completed.
    /// </summary>
    /// <param name="context">The reloading page's <see cref="PageObjectContext"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output.</param>
    /// <returns>A state object which is subsequently passed to <see cref="WaitForCompletion"/>. May be null.</returns>
    object? PrepareWaitForCompletion ([NotNull] PageObjectContext context, [NotNull] ILogger logger);

    /// <summary>
    /// Called immediately after the action has been performed. This method should block until the DOM fulfills certain characteristics (i.e. a page
    /// sequence number has been increased).
    /// </summary>
    /// <param name="context">The reloading page's <see cref="PageObjectContext"/>.</param>
    /// <param name="state">The state object obtained from <see cref="PrepareWaitForCompletion"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output.</param>
    void WaitForCompletion ([NotNull] PageObjectContext context, [CanBeNull] object? state, [NotNull] ILogger logger);
  }
}
