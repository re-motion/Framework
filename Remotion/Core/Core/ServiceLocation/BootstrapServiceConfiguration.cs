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
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    private static string? s_getLoggerFactoryFirstExceptionStackTrace;
    private static readonly string[] s_knownTestHosts = ["testhost.dll", "testhost.x86.dll", "ReSharperTestRunner.dll", "ReSharperTestRunner32.dll", "ReSharperTestRunner64.dll"];

    public static void SetLoggerFactory (ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      lock (s_loggerFactoryLock)
      {
        s_loggerFactory = loggerFactory;
      }
    }

    public static ILoggerFactory GetLoggerFactory ()
    {
      var loggerFactory = s_loggerFactory;
      if (loggerFactory != null)
        return loggerFactory;

      lock (s_loggerFactoryLock)
      {
        if (s_loggerFactory != null)
          return s_loggerFactory;

        if (IsEntryAssemblyPartOfAKnownTestingFramework(out string entryAssemblyLocation))
        {
        //   // When code is executed as part of a unit test suite, the test runner itself is normally not part of the test fixture's BIN directory.
        //   // Based on this assumption we can deduce that we are in a unit test scenario and substitute the NullLoggerFactory as a default initialization.
        //   s_loggerFactory = NullLoggerFactory.Instance;
        //
        //   Console.WriteLine("Logging was not configured programmatically and NullLogger was chosen as a default.");
        //   Console.WriteLine("Entry assembly location: " + entryAssemblyLocation);
        //
        //   return s_loggerFactory;
        }

        if (s_getLoggerFactoryFirstExceptionStackTrace == null)
        {
          try
          {
            s_getLoggerFactoryFirstExceptionStackTrace = new StackTrace().ToString();
          }
          catch
          {
            s_getLoggerFactoryFirstExceptionStackTrace = "No StackTrace is available.";
          }
        }

        throw new InvalidOperationException(
            """
            The BootstrapServiceConfiguration.SetLoggerFactory(...) method must be called before accessing the service configuration.

            Example based on log4net:
            1. Add reference to Remotion.Logging.Log4Net Nuget package.
            2. Add the following code in your startup code (the application's Main() method or a SetupFixture in the tests) before performing other operations.
                var loggerFactory = new LoggerFactory(new[] { new Log4NetLoggerProvider() });
                BootstrapServiceConfiguration.SetLoggerFactory(loggerFactory);

            Alternatively, you can supply a different logging framework or chose to have no logging at all by passing the NullLoggerFactory.Instance.


            """
            + "Entry assembly location: " + entryAssemblyLocation

            +
            """


            --- Begin of diagnostic stack trace for this exception's first occurance ---


            """
            + s_getLoggerFactoryFirstExceptionStackTrace
            +
            """

            --- End of diagnostic stack trace ---

            """);
      }
    }

    private static bool IsEntryAssemblyPartOfAKnownTestingFramework (out string entryAssemblyLocation)
    {
      // Unknown entry assembly means we cannot make an assumption about the execution.
      var entryAssembly = Assembly.GetEntryAssembly();
      if (entryAssembly == null)
      {
        entryAssemblyLocation = "<unknown>";
        return false;
      }

      entryAssemblyLocation = entryAssembly.Location;

      // Application is run via a known testhost
      if (Array.Find(s_knownTestHosts, knownTestHost => knownTestHost.Equals(entryAssembly.ManifestModule.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
        return true;

      // Unknown entry assembly location means we cannot make an assumption about the execution.
      var entryAssemblyDirectory = Directory.GetParent(entryAssembly.Location);
      if (entryAssemblyDirectory == null)
      {
        entryAssemblyLocation = "<unknown>";
        return false;
      }

      // Unknown Remotion.dll location means we cannot make an assumption about the execution.
      var currentAssemblyDirectory = Directory.GetParent(typeof(BootstrapServiceConfiguration).Assembly.Location);
      if (currentAssemblyDirectory == null)
        return false;

      // Application is deployed in a separate location from the Remotion.dll. This is known to only happen for testing frameworks. 
      if (currentAssemblyDirectory.FullName != entryAssemblyDirectory.FullName)
        return true;

      // Fallback, we assume it's a normal application run.
      return false;
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
