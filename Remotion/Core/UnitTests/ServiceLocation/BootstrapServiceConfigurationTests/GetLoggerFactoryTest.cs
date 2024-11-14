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
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Compilation;
using Remotion.Development.UnitTesting.IsolatedCodeRunner;
using Remotion.Logging;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.UnitTests.ServiceLocation.BootstrapServiceConfigurationTests
{
  [TestFixture]
  public class GetLoggerFactoryTest
  {
    private const string c_testAssemblySourceDirectoryRoot = @"ServiceLocation\BootstrapServiceConfigurationTests\TestAssemblies";

    private ILoggerFactory _backupLoggerFactory;
    private string _backupStacktraceForFirstCallToGetLoggerFactory;

    [SetUp]
    public void SetUp ()
    {
      _backupLoggerFactory = GetLoggerFactoryOnBootstrapServiceConfiguration();
      _backupStacktraceForFirstCallToGetLoggerFactory = GetStackTraceForFirstCallToGetLoggerFactoryOnBootstrapServiceConfiguration();

      SetLoggerFactoryOnBootstrapServiceConfiguration(null);
      SetStackTraceForFirstCallToGetLoggerFactoryOnBootstrapServiceConfiguration(null);    }

    [TearDown]
    public void TearDown ()
    {
      SetLoggerFactoryOnBootstrapServiceConfiguration(_backupLoggerFactory);
      SetStackTraceForFirstCallToGetLoggerFactoryOnBootstrapServiceConfiguration(_backupStacktraceForFirstCallToGetLoggerFactory);
    }

    [Test]
    public void GetLoggerFactory_AfterSetLoggerFactory_ReturnsLoggerFactory ()
    {
      var loggerFactoryStub = Mock.Of<ILoggerFactory>();
      BootstrapServiceConfiguration.SetLoggerFactory(loggerFactoryStub);

      Assert.That(BootstrapServiceConfiguration.GetLoggerFactory(), Is.SameAs(loggerFactoryStub));
    }

    [Test]
    public void GetLoggerFactory_AfterMultipleCallsToSetLoggerFactory_ReturnsLastConfiguredLoggerFactory ()
    {
      BootstrapServiceConfiguration.SetLoggerFactory(Mock.Of<ILoggerFactory>());

      var secondLoggerFactoryStub = Mock.Of<ILoggerFactory>();
      BootstrapServiceConfiguration.SetLoggerFactory(secondLoggerFactoryStub);

      Assert.That(BootstrapServiceConfiguration.GetLoggerFactory(), Is.SameAs(secondLoggerFactoryStub));
    }

    [Test]
    public void GetLoggerFactory_WithoutSetLoggerFactory_ThrowsInvalidOperationException ()
    {
      Assert.That(
          () => BootstrapServiceConfiguration.GetLoggerFactory(),
          Throws.InvalidOperationException
              .With.Message.StartsWith("The BootstrapServiceConfiguration.SetLoggerFactory(...) method must be called before accessing the service configuration.")
              .And.Message.Contains("Remotion.Logging." + nameof(EnableNullLoggerFactoryAsFallbackInBootstrapServiceConfigurationAttribute))
              .And.Message.Contains(
                  """
                  as the default for logging.

                  --- Begin of diagnostic stack trace for this exception's first occurance ---

                     at Remotion.ServiceLocation.BootstrapServiceConfiguration.GetLoggerFactory()
                     at Remotion.UnitTests.ServiceLocation.BootstrapServiceConfigurationTests.GetLoggerFactoryTest.
                  """)
              .And.Message.EndsWith(
                  """
                  )

                  --- End of diagnostic stack trace ---
                  """)
          );
    }

    [Test]
    public void SetLoggerFactory_AfterGetLoggerFactory_ThrowsInvalidOperationException ()
    {
      SetLoggerFactoryOnBootstrapServiceConfiguration(Mock.Of<ILoggerFactory>());
      Dev.Null = BootstrapServiceConfiguration.GetLoggerFactory();

      Assert.That(
          () => BootstrapServiceConfiguration.SetLoggerFactory(Mock.Of<ILoggerFactory>()),
          Throws.InvalidOperationException
              .With.Message.StartsWith(
                  """
                  The BootstrapServiceConfiguration.SetLoggerFactory(...) method must not be called after the configured value has been read via BootstrapServiceConfiguration.GetLoggerFactory().

                  The first call to BootstrapServiceConfiguration.GetLoggerFactory() generated the following stack trace:

                  --- Begin of diagnostic stack trace ---

                     at Remotion.ServiceLocation.BootstrapServiceConfiguration.GetLoggerFactory()
                     at Remotion.UnitTests.ServiceLocation.BootstrapServiceConfigurationTests.GetLoggerFactoryTest.SetLoggerFactory_AfterGetLoggerFactory_ThrowsInvalidOperationException()
                     at
                  """)
              .And.Message.EndsWith(
                  """
                  )

                  --- End of diagnostic stack trace ---
                  """
                  )
          );
    }

    [Test]
    public void GetLoggerFactory_WithDefaultLogger_ReturnsNullLoggerFactory ()
    {
      var isolatedCodeRunner = new IsolatedCodeRunner(TestMain);
      isolatedCodeRunner.Run();

      static void TestMain (string[] args)
      {
        var firstInMemoryAssembly = CompileTestAssemblyInMemory(
            "AssemblyWithDefaultLogger",
            typeof(BootstrapServiceConfiguration).Module.Name,
            typeof(NullLoggerFactory).Module.Name);

        Test(firstInMemoryAssembly);
      }

      static void Test (Assembly firstInMemoryAssembly)
      {
        var programType = firstInMemoryAssembly.GetTypes().Single(t => t.Name == "Program");
        var methodInfo = programType.GetMethod("Main");
        Assertion.IsNotNull(methodInfo);
        var result = methodInfo.Invoke(null, []);
        Assert.That(result, Is.TypeOf<NullLoggerFactory>());
      }
    }

    private static ILoggerFactory GetLoggerFactoryOnBootstrapServiceConfiguration ()
    {
      return (ILoggerFactory)PrivateInvoke.GetNonPublicStaticField(typeof(BootstrapServiceConfiguration), "s_loggerFactory");
    }

    private static void SetLoggerFactoryOnBootstrapServiceConfiguration (ILoggerFactory value)
    {
      PrivateInvoke.SetNonPublicStaticField(typeof(BootstrapServiceConfiguration), "s_loggerFactory", value);
    }

    private static string GetStackTraceForFirstCallToGetLoggerFactoryOnBootstrapServiceConfiguration ()
    {
      return (string)PrivateInvoke.GetNonPublicStaticField(typeof(BootstrapServiceConfiguration), "s_stackTraceForFirstCallToGetLoggerFactory");
    }

    private static void SetStackTraceForFirstCallToGetLoggerFactoryOnBootstrapServiceConfiguration (string value)
    {
      PrivateInvoke.SetNonPublicStaticField(typeof(BootstrapServiceConfiguration), "s_stackTraceForFirstCallToGetLoggerFactory", value);
    }

    private static Assembly CompileTestAssemblyInMemory (string assemblyName, params string[] referencedAssemblies)
    {
      AssemblyCompiler assemblyCompiler = AssemblyCompiler.CreateInMemoryAssemblyCompiler(c_testAssemblySourceDirectoryRoot + "\\" + assemblyName, referencedAssemblies);
      assemblyCompiler.Compile();
      return assemblyCompiler.CompiledAssembly;
    }
  }
}
