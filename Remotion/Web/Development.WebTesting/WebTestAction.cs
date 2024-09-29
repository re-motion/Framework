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
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Represents a web test action (e.g. a click on a specific button within a given <see cref="ControlObject"/>). Such an action may trigger partial
  /// and/or full page reloads as well as modal browser dialogs, which are dealt with according to the given <see cref="IWebTestActionOptions"/> object.
  /// </summary>
  public abstract class WebTestAction
  {
    /// <summary>
    /// Returns a unique ID for a <see cref="WebTestAction"/> (used for debug purposes).
    /// </summary>
    private static class WebTestActionSequenceNumberGenerator
    {
      private static int s_sequenceNumber = 0;

      public static int GetNextSequenceNumber ()
      {
        return Interlocked.Increment(ref s_sequenceNumber);
      }
    }

    private readonly ILogger _logger;
    private readonly ControlObject _control;
    private readonly ElementScope _scope;
    private int _actionID;

    protected WebTestAction ([NotNull] ControlObject control, [NotNull] ElementScope scope, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("control", control);
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("logger", logger);

      _logger = logger;
      _control = control;
      _scope = scope;
    }

    /// <summary>
    /// Template property for the action's name (e.g. "Click").
    /// </summary>
    protected abstract string ActionName { get; }

    /// <summary>
    /// Gets the <see cref="ILogger"/> used by the <see cref="WebTestAction"/> for diagnostic output.
    /// </summary>
    protected ILogger Logger
    {
      get { return _logger; }
    }

    /// <summary>
    /// Executes the action using the given <paramref name="options"/>. This method blocks until the action is completed (i.e. all triggered
    /// partial and/or full page reloads are finished).
    /// </summary>
    /// <param name="options">See <see cref="IWebTestActionOptions"/> for more information.</param>
    public void Execute ([NotNull] IWebTestActionOptions options)
    {
      ArgumentUtility.CheckNotNull("options", options);

      _actionID = WebTestActionSequenceNumberGenerator.GetNextSequenceNumber();
      var completionDetectionStrategy = options.CompletionDetectionStrategy ?? new NullCompletionDetectionStrategy();
      var modalDialogHandler = options.ModalDialogHandler;
      var pageObjectContext = _control.Context.PageObject.Context;

      OutputDebugMessage("Started.");

      OutputDebugMessage("Collecting state for completion detection...");
      var state = completionDetectionStrategy.PrepareWaitForCompletion(pageObjectContext, _logger);

      OutputDebugMessage(string.Format("Performing '{0}'...", ActionName));
      ExecuteInteraction(_scope);

      if (modalDialogHandler != null)
      {
        OutputDebugMessage("Handling modal dialog...");
        modalDialogHandler.HandleModalDialog(pageObjectContext);
      }

      OutputDebugMessage("Waiting for completion...");
      completionDetectionStrategy.WaitForCompletion(pageObjectContext, state, _logger);

      OutputDebugMessage("Finished.");
    }

    /// <summary>
    /// Template method in which derived implementations should execute the actual interaction with the given <paramref name="scope"/>.
    /// </summary>
    protected abstract void ExecuteInteraction ([NotNull] ElementScope scope);

    /// <summary>
    /// Outputs the given debug <paramref name="message"/>.
    /// </summary>
    protected void OutputDebugMessage ([NotNull] string message)
    {
      ArgumentUtility.CheckNotNullOrEmpty("message", message);

      _logger.LogDebug("Action {0}: {1}", _actionID, message);
    }
  }
}
