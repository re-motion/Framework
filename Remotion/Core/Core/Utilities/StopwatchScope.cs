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
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides a simple way of timing a piece of code wrapped into a <c>using</c> block. At the end of the block or at a checkpoint, an action is 
  /// performed or a log message written.
  /// </summary>
  public class StopwatchScope : IDisposable
  {
    /// <summary>
    /// Defines an action to be called whenever a checkpoint is reached or the <see cref="StopwatchScope"/> is disposed.
    /// </summary>
    /// <param name="context">The context in which the action is invoked. This corresponds to the parameter given to 
    /// <see cref="StopwatchScope.Checkpoint"/>.</param>
    /// <param name="scope">The <see cref="StopwatchScope"/> triggering the action. Use <see cref="StopwatchScope.ElapsedTotal"/> and
    /// <see cref="StopwatchScope.ElapsedSinceLastCheckpoint"/> to retrieve the elapsed time.</param>
    public delegate void MeasurementAction (string context, StopwatchScope scope);

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time, executing the given
    /// <paramref name="action"/> when the scope is disposed or when a <see cref="Checkpoint"/> is reached.
    /// </summary>
    /// <param name="action">The <see cref="MeasurementAction"/> to receive the result. Takes the following arguments: 
    /// <c>string context, StopwatchScope scope</c>. Use <see cref="StopwatchScope.ElapsedTotal"/> and
    /// <see cref="StopwatchScope.ElapsedSinceLastCheckpoint"/> to retrieve the elapsed time. When the scope is disposed, the context parameter
    /// is set to the string "end".
    /// </param>
    /// <returns>
    /// A <see cref="StopwatchScope"/> that measures the time.
    /// </returns>
    public static StopwatchScope CreateScope (MeasurementAction action)
    {
      return new StopwatchScope(action, "end");
    }

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time, writing the result to the given 
    /// <paramref name="writer"/> when the scope is disposed or a <see cref="Checkpoint"/> is reached.
    /// </summary>
    /// <param name="writer">The writer to receive the result.</param>
    /// <param name="formatString">A string to format the result with. The string can contain the following placeholders:
    /// <list type="bullet">
    /// <item>
    /// <term>{context}</term>
    /// <description>Replaced with the context string passed to <see cref="Checkpoint"/>. At the end of the scope, this is the string "end".</description>
    /// </item>
    /// <item>
    /// <term>{elapsed}</term>
    /// <description>Replaced with the standard string representation of <see cref="ElapsedTotal"/>.</description>
    /// </item>
    /// <item>
    /// <term>{elapsed:ms}</term>
    /// <description>Replaced with <see cref="ElapsedTotal"/>, using <see cref="TimeSpan.TotalMilliseconds"/> to obtain the 
    ///   number of milliseconds elapsed since the last checkpoint.</description>
    /// </item>
    /// <item>
    /// <term>{elapsedCP}</term>
    /// <description>Replaced with the standard string representation of <see cref="ElapsedSinceLastCheckpoint"/>.</description>
    /// </item>
    /// <item>
    /// <term>{elapsedCP:ms}</term>
    /// <description>Replaced with <see cref="ElapsedSinceLastCheckpoint"/>, using <see cref="TimeSpan.TotalMilliseconds"/> to obtain the 
    ///   number of milliseconds elapsed since the last checkpoint.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>A <see cref="StopwatchScope"/> that measures the time and writes it to a <see cref="TextWriter"/>.</returns>
    public static StopwatchScope CreateScope (TextWriter writer, string formatString)
    {
      var actualFormatString = ReplacePlaceholders(formatString);
      return new StopwatchScope((context, scope) => writer.WriteLine(
          actualFormatString,
          context,
          scope.ElapsedTotal.ToString(),
          scope.ElapsedTotal.TotalMilliseconds.ToString(),
          scope.ElapsedSinceLastCheckpoint.ToString(),
          scope.ElapsedSinceLastCheckpoint.TotalMilliseconds.ToString()), "end");
    }

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time, writing the result to the given
    /// <paramref name="logger"/> when the scope is disposed or a <see cref="Checkpoint"/> is reached.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to receive the result.</param>
    /// <param name="logLevel">The log level to log the result with.</param>
    /// <param name="formatString">A string to format the result with. The string can contain the following placeholders:
    /// <list type="bullet">
    /// <item>
    /// <term>{context}</term>
    /// <description>Replaced with the context string passed to <see cref="Checkpoint"/>. At the end of the scope, this is the string "end".</description>
    /// </item>
    /// <item>
    /// <term>{elapsed}</term>
    /// <description>Replaced with the standard string representation of <see cref="ElapsedTotal"/>.</description>
    /// </item>
    /// <item>
    /// <term>{elapsed:ms}</term>
    /// <description>Replaced with <see cref="ElapsedTotal"/>, using <see cref="TimeSpan.TotalMilliseconds"/> to obtain the 
    ///   number of milliseconds elapsed since the last checkpoint.</description>
    /// </item>
    /// <item>
    /// <term>{elapsedCP}</term>
    /// <description>Replaced with the standard string representation of <see cref="ElapsedSinceLastCheckpoint"/>.</description>
    /// </item>
    /// <item>
    /// <term>{elapsedCP:ms}</term>
    /// <description>Replaced with <see cref="ElapsedSinceLastCheckpoint"/>, using <see cref="TimeSpan.TotalMilliseconds"/> to obtain the 
    ///   number of milliseconds elapsed since the last checkpoint.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>
    /// A <see cref="StopwatchScope"/> that measures the time in milliseconds.
    /// </returns>
    public static StopwatchScope CreateScope (ILogger logger, LogLevel logLevel, string formatString)
    {
      var actualFormatString = ReplacePlaceholders(formatString);
      return new StopwatchScope(Log, "end");

      void Log (string context, StopwatchScope scope)
      {
        if (logger.IsEnabled(logLevel))
        {
          var logMessage = string.Format(
              actualFormatString,
              context,
              scope.ElapsedTotal.ToString(),
              scope.ElapsedTotal.TotalMilliseconds.ToString(),
              scope.ElapsedSinceLastCheckpoint.ToString(),
              scope.ElapsedSinceLastCheckpoint.TotalMilliseconds.ToString());
          logger.Log(logLevel, logMessage);
        }
      }
    }

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time, writing the result to the <see cref="Console"/> when the scope is disposed or 
    /// a <see cref="Checkpoint"/> is reached.
    /// </summary>
    /// <param name="formatString">A string to format the result with. The string can contain the following placeholders:
    /// <list type="bullet">
    /// <item>
    /// <term>{context}</term>
    /// <description>Replaced with the context string passed to <see cref="Checkpoint"/>. At the end of the scope, this is the string "end".</description>
    /// </item>
    /// <item>
    /// <term>{elapsed}</term>
    /// <description>Replaced with the standard string representation of <see cref="ElapsedTotal"/>.</description>
    /// </item>
    /// <item>
    /// <term>{elapsed:ms}</term>
    /// <description>Replaced with <see cref="ElapsedTotal"/>, using <see cref="TimeSpan.TotalMilliseconds"/> to obtain the 
    ///   number of milliseconds elapsed since the last checkpoint.</description>
    /// </item>
    /// <item>
    /// <term>{elapsedCP}</term>
    /// <description>Replaced with the standard string representation of <see cref="ElapsedSinceLastCheckpoint"/>.</description>
    /// </item>
    /// <item>
    /// <term>{elapsedCP:ms}</term>
    /// <description>Replaced with <see cref="ElapsedSinceLastCheckpoint"/>, using <see cref="TimeSpan.TotalMilliseconds"/> to obtain the 
    ///   number of milliseconds elapsed since the last checkpoint.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>
    /// A <see cref="StopwatchScope"/> that measures the time in milliseconds.
    /// </returns>
    public static StopwatchScope CreateScope (string formatString)
    {
      var actualFormatString = ReplacePlaceholders(formatString);
      return new StopwatchScope((context, scope) => Console.WriteLine(
          actualFormatString,
          context,
          scope.ElapsedTotal.ToString(),
          scope.ElapsedTotal.TotalMilliseconds.ToString(),
          scope.ElapsedSinceLastCheckpoint.ToString(),
          scope.ElapsedSinceLastCheckpoint.TotalMilliseconds.ToString()), "end");
    }

    private static string ReplacePlaceholders (string formatString)
    {
      return formatString
          .Replace("{context}", "{0}")
          .Replace("{elapsed}", "{1}")
          .Replace("{elapsed:ms}", "{2}")
          .Replace("{elapsedCP}", "{3}")
          .Replace("{elapsedCP:ms}", "{4}");
    }

    private readonly Stopwatch _stopwatch;
    private readonly MeasurementAction _action;
    private readonly string _scopeEndString;

    private TimeSpan _lastCheckpointElapsed;
    private bool _disposed;

    private StopwatchScope (MeasurementAction action, string scopeEndString)
    {
      ArgumentUtility.CheckNotNull("action", action);

      _action = action;
      _scopeEndString = scopeEndString;
      _lastCheckpointElapsed = TimeSpan.Zero;
      _stopwatch = Stopwatch.StartNew();
      _disposed = false;
    }

    /// <summary>
    /// Stops measuring the time and invokes the time measurement action defined when creating the scope.
    /// </summary>
    public void Dispose ()
    {
      if (!_disposed)
      {
        _stopwatch.Stop();
        Checkpoint(_scopeEndString);
        _disposed = true;
      }
    }

    /// <summary>
    /// Gets the time elapsed since the construction of this <see cref="StopwatchScope"/> until now or the point of time where <see cref="Dispose"/> 
    /// was called, whichever occurs first.
    /// </summary>
    /// <value>The total elapsed time.</value>
    public TimeSpan ElapsedTotal
    {
      get { return _stopwatch.Elapsed; }
    }

    /// <summary>
    /// Gets the time elapsed since the last call to <see cref="Checkpoint"/> (or the construction of this <see cref="StopwatchScope"/> if no 
    /// checkpoint has been created) until now. If the scope has been disposed, this is <see cref="TimeSpan.Zero"/>.
    /// </summary>
    /// <value>The elapsed time since the last checkpoint.</value>
    public TimeSpan ElapsedSinceLastCheckpoint
    {
      get { return ElapsedTotal - _lastCheckpointElapsed; }
    }

    /// <summary>
    /// Triggers a checkpoint, invoking the action the time measurement action defined when creating the scope with the given <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The context information to pass to the action.</param>
    public void Checkpoint (string context)
    {
      if (_disposed)
        throw new ObjectDisposedException("StopwatchScope");

      _action(context, this);
      _lastCheckpointElapsed = ElapsedTotal;
    }

    /// <summary>
    /// Pauses the time measurement. Resume it with <see cref="Resume"/>. If the measurement is already paused, this method does nothing.
    /// </summary>
    public void Pause ()
    {
      if (_disposed)
        throw new ObjectDisposedException("StopwatchScope");

      _stopwatch.Stop();
    }

    /// <summary>
    /// Resumes the time measurement after it has been paused with <see cref="Pause"/>. If the measurement is not paused, this method does nothing.
    /// </summary>
    public void Resume ()
    {
      if (_disposed)
        throw new ObjectDisposedException("StopwatchScope");

      _stopwatch.Start();
    }
  }
}
