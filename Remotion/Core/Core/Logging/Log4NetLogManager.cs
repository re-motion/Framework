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
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Logging
{
  /// <summary>
  /// Implementation of <see cref="ILogManager"/> for <b>log4net</b>.
  /// </summary>
  [ImplementationFor (typeof(ILogManager), Lifetime = LifetimeKind.Singleton)]
  public class Log4NetLogManager : ILogManager
  {
    private readonly WrapperMap _wrapperMap = new WrapperMap (logger => new Log4NetLog (logger));

    /// <summary>
    /// Creates a new instance of the <see cref="Log4NetLog"/> type.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve. Must not be <see langword="null"/> or empty.</param>
    /// <returns>A <see cref="Log4NetLog"/> for the <paramref name="name"/> specified.</returns>
    public ILog GetLogger (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return WrapLogger (LoggerManager.GetLogger (Assembly.GetCallingAssembly(), name));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Log4NetLog"/> type.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve. Must not be <see langword="null"/>.</param>
    /// <returns>A <see cref="Log4NetLog"/> for the fully qualified name of the <paramref name="type"/> specified.</returns>
    public ILog GetLogger (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return WrapLogger (LoggerManager.GetLogger (Assembly.GetCallingAssembly(), type));
    }

    /// <summary>
    /// Initializes <b>log4net</b> by invoking <see cref="XmlConfigurator.Configure()"/>.
    /// </summary>
    public void Initialize ()
    {
      //TODO: Check if there is a sensible way for testing log4net startup.
      XmlConfigurator.Configure ();
    }

    public void InitializeConsole ()
    {
      var appender = CreateConsoleAppender ();
      BasicConfigurator.Configure (appender);
    }

    public void InitializeConsole (LogLevel defaultThreshold, params LogThreshold[] logThresholds)
    {
      ArgumentUtility.CheckNotNull ("logThresholds", logThresholds);

      var appender = CreateConsoleAppender();

      var repositoryAssembly = Assembly.GetCallingAssembly ();
      var loggerRepository = log4net.LogManager.GetRepository (repositoryAssembly);
      var hierarchy = loggerRepository as Hierarchy;
      if (hierarchy == null)
      {
        var message = string.Format (
            "Cannot set a default threshold for the logger repository of type '{1}' configured for assembly '{0}'. The repository does not derive "
            + "from the '{2}' class.", 
            repositoryAssembly.GetName().Name,
            loggerRepository.GetType(),
            typeof (Hierarchy));
        throw new InvalidOperationException (message);
      }

      hierarchy.Root.Level = Log4NetLog.Convert (defaultThreshold);
      hierarchy.Root.AddAppender (appender);
      hierarchy.Configured = true;

      foreach (var logThreshold in logThresholds)
      {
        var log4netLog = logThreshold.Logger as ILoggerWrapper;
        if (log4netLog == null)
        {
          throw new ArgumentException (
              "This LogManager only supports ILog implementations that also implement the log4net.Core.ILoggerWrapper interface.",
              "logThresholds");
        }

        var log4netLogger = log4netLog.Logger as Logger;
        if (log4netLogger == null)
        {
          var message = string.Format (
              "Log-specific thresholds can only be set for log4net loggers of type '{0}'. The specified logger '{1}' is of type '{2}'.",
              typeof (Logger),
              log4netLog.Logger.Name,
              log4netLog.Logger.GetType());
          throw new ArgumentException (message, "logThresholds");
        }

        log4netLogger.Level = Log4NetLog.Convert (logThreshold.Threshold);
      }
    }

    private ILog WrapLogger (ILogger logger)
    {
      return (Log4NetLog) _wrapperMap.GetWrapper (logger);
    }

    private ConsoleAppender CreateConsoleAppender ()
    {
      var appender = new ConsoleAppender ();
      appender.Layout = new PatternLayout ("%-5level: %message%newline");
      return appender;
    }
  }
}
