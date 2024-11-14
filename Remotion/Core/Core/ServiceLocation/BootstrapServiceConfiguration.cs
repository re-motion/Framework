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
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Allows users to register services before an actual container or <see cref="IServiceLocator"/> has been built.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class BootstrapServiceConfiguration : IBootstrapServiceConfiguration
  {
    private static readonly object s_loggerFactoryLock = new object();
    private static ILoggerFactory? s_loggerFactory;
    private static string? s_stackTraceForFirstCallToGetLoggerFactory;

    /// <summary>
    /// Sets the <see cref="ILoggerFactory"/> returned by <see cref="GetLoggerFactory"/>. This <see cref="ILoggerFactory"/> is later used to create the <see cref="ILogger"/> instances.
    /// </summary>
    /// <param name="loggerFactory">
    /// An instance implementing the <see cref="ILoggerFactory"/> interface. Use <see cref="NullLoggerFactory"/> to permanently disable logging. Must not be <see langword="null" />.
    /// </param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetLoggerFactory"/> is called after <see cref="GetLoggerFactory"/>.</exception>
    public static void SetLoggerFactory (ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      lock (s_loggerFactoryLock)
      {
        if (s_stackTraceForFirstCallToGetLoggerFactory == null)
        {
          s_loggerFactory = loggerFactory;
        }
        else
        {
          throw new InvalidOperationException(
              """
              The BootstrapServiceConfiguration.SetLoggerFactory(...) method must not be called after the configured value has been read via BootstrapServiceConfiguration.GetLoggerFactory().

              The first call to BootstrapServiceConfiguration.GetLoggerFactory() generated the following stack trace:

              --- Begin of diagnostic stack trace ---


              """
              + s_stackTraceForFirstCallToGetLoggerFactory
              +
              """
  
              --- End of diagnostic stack trace ---
              """);
        }
      }
    }

    /// <summary>
    /// Returns the <see cref="ILoggerFactory"/> supplied when calling <see cref="SetLoggerFactory"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no <see cref="ILoggerFactory"/> has been configured.</exception>
    public static ILoggerFactory GetLoggerFactory ()
    {
      lock (s_loggerFactoryLock)
      {
        StackTrace? stackTrace = null;
        if (s_stackTraceForFirstCallToGetLoggerFactory == null)
        {
          try
          {
            stackTrace = new StackTrace();
            s_stackTraceForFirstCallToGetLoggerFactory = stackTrace.ToString();
          }
          catch
          {
            s_stackTraceForFirstCallToGetLoggerFactory = "No StackTrace is available.";
          }
        }

        if (s_loggerFactory != null)
          return s_loggerFactory;

        var fallbackLoggerFactory = GetFallbackLoggerFactory(stackTrace);
        if (fallbackLoggerFactory != null)
        {
          s_loggerFactory = fallbackLoggerFactory;
          return s_loggerFactory;
        }

        throw new InvalidOperationException(
            """
            The BootstrapServiceConfiguration.SetLoggerFactory(...) method must be called before accessing the service configuration.

            Example based on log4net:
            1. Add reference to Remotion.Logging.Log4Net Nuget package.
            2. Add the following code in your startup code (the application's Main() method or a SetupFixture in the tests) before performing other operations.
                var loggerFactory = new LoggerFactory(new[] { new Log4NetLoggerProvider() });
                BootstrapServiceConfiguration.SetLoggerFactory(loggerFactory);

            You can supply a different logging framework or chose to have no logging at all by passing the NullLoggerFactory.Instance.

            """
            + "For testing purposes or if no logging is required in your application, you may instead apply the "
            + typeof(EnableNullLoggerFactoryAsFallbackInBootstrapServiceConfigurationAttribute).FullName + " "
            + "on your testing assembly or your application's main assembly. "
            + "This change will configure the infrastructure to use the NullLoggerFactory as the default for logging."
            + """


              --- Begin of diagnostic stack trace for this exception's first occurance ---


              """
            + s_stackTraceForFirstCallToGetLoggerFactory
            + """

              --- End of diagnostic stack trace ---
              """);
      }
    }

    /// <summary>
    /// Returns the <see cref="NullLoggerFactory"/> if any <see cref="Assembly"/> on the stack trace
    /// defines the <see cref="EnableNullLoggerFactoryAsFallbackInBootstrapServiceConfigurationAttribute"/>.
    /// </summary>
    private static ILoggerFactory? GetFallbackLoggerFactory (StackTrace? stackTrace)
    {
      if (stackTrace == null)
        return null;

      var assembliesInStackTrace = new HashSet<Assembly>();
      for (int i = 0; i < stackTrace.FrameCount; i++)
      {
        var assembly = stackTrace.GetFrame(i)?.GetMethod()?.DeclaringType?.Assembly;
        var isNewAssmebly = assembly != null && assembliesInStackTrace.Add(assembly);
        if (isNewAssmebly && assembly!.IsDefined(typeof(EnableNullLoggerFactoryAsFallbackInBootstrapServiceConfigurationAttribute)))
          return NullLoggerFactory.Instance;
      }

      return null;
    }

    private readonly object _lock = new object();
    private readonly List<ServiceConfigurationEntry> _registrations = new List<ServiceConfigurationEntry>();

    private DefaultServiceLocator _bootstrapServiceLocator = DefaultServiceLocator.Create();

    public BootstrapServiceConfiguration ()
    {

    }

    public IServiceLocator BootstrapServiceLocator
    {
      get { return _bootstrapServiceLocator; }
    }

    public ServiceConfigurationEntry[] Registrations
    {
      get
      {
        lock (_lock)
        {
          return _registrations.ToArray();
        }
      }
    }

    public void Register (ServiceConfigurationEntry entry)
    {
      ArgumentUtility.CheckNotNull("entry", entry);

      lock (_lock)
      {
        _bootstrapServiceLocator.Register(entry);
        _registrations.Add(entry);
      }
    }

    public void Register (Type serviceType, Type implementationType, LifetimeKind lifetime)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);
      ArgumentUtility.CheckNotNull("implementationType", implementationType);

      var entry = new ServiceConfigurationEntry(serviceType, new ServiceImplementationInfo(implementationType, lifetime));
      Register(entry);
    }

    public void Reset ()
    {
      lock (_lock)
      {
        _bootstrapServiceLocator = DefaultServiceLocator.Create();
        _registrations.Clear();
      }
    }
  }
}
