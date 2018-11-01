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
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;
using Rhino.Mocks;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyLoading
{
  [TestFixture]
  [Serializable]
  public class FilteringAssemblyLoaderTest
  {
    private MockRepository _mockRepository;
    private IAssemblyLoaderFilter _filterMock;
    private FilteringAssemblyLoader _loader;

    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _filterMock = _mockRepository.StrictMock<IAssemblyLoaderFilter>();
      _loader = new FilteringAssemblyLoader (_filterMock);

      _memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (_memoryAppender);
      Assert.That (LogManager.GetLogger (typeof (FilteringAssemblyLoader)).IsDebugEnabled, Is.True);
    }

    [TearDown]
    public void TearDown ()
    {
      _memoryAppender.Clear ();
      LogManager.ResetConfiguration ();

      Assert.That (LogManager.GetLogger (typeof (FilteringAssemblyLoader)).IsDebugEnabled, Is.False); 
    }

    [Test]
    public void TryLoadAssembly ()
    {
      SetupFilterTrue();

      Assembly referenceAssembly = typeof (FilteringAssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.SameAs (referenceAssembly));
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderTrue_IncludeTrue ()
    {
      Assembly referenceAssembly = typeof (FilteringAssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;

      Expect.Call (_filterMock.ShouldConsiderAssembly (null))
          .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
          .Return (true);
      Expect.Call (_filterMock.ShouldIncludeAssembly (null))
          .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
          .Return (true);

      _mockRepository.ReplayAll();
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.SameAs (referenceAssembly));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderTrue_IncludeFalse ()
    {
      Assembly referenceAssembly = typeof (FilteringAssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;

      Expect.Call (_filterMock.ShouldConsiderAssembly (null))
          .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
          .Return (true);
      Expect.Call (_filterMock.ShouldIncludeAssembly (null))
          .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
          .Return (false);

      _mockRepository.ReplayAll();
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.Null);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderFalse ()
    {
      Assembly referenceAssembly = typeof (FilteringAssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;

      Expect.Call (_filterMock.ShouldConsiderAssembly (null))
          .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
          .Return (false);

      _mockRepository.ReplayAll();
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.Null);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void TryLoadAssembly_WithBadImageFormatException ()
    {
      SetupFilterTrue();

      const string path = "Invalid.dll";
      using (File.CreateText (path))
      {
        // no contents
      }

      try
      {
        Assembly loadedAssembly = _loader.TryLoadAssembly (path);
        Assert.That (loadedAssembly, Is.Null);
        
        CheckLog (
            "INFO : The file 'Invalid.dll' triggered a BadImageFormatException and will be ignored. Possible causes for this are:" + Environment.NewLine
            + "- The file is not a .NET assembly." + Environment.NewLine
            + "- The file was built for a newer version of .NET." + Environment.NewLine
            + "- The file was compiled for a different platform (x86, x64, etc.) than the platform this process is running on." + Environment.NewLine 
            + "- The file is damaged.");
      }
      finally
      {
        FileUtility.DeleteAndWaitForCompletion (path);
      }
    }

    // Assembly.Load will lock a file when it throws a FileLoadException, making it impossible to restore the previous state
    // for naive tests. We therefore run the actual test in another process using Process.Start; that way, the locked file
    // will be unlocked when the process exits and we can delete it after the test has run.
    [Test]
    public void TryLoadAssembly_WithFileLoadException ()
    {
      string program = Compile (
          "Reflection\\TypeDiscovery\\TestAssemblies\\FileLoadExceptionConsoleApplication",
          "FileLoadExceptionConsoleApplication.exe",
          true,
          null);
      string programConfig = program + ".config";
      File.Copy ("Reflection\\TypeDiscovery\\TestAssemblies\\FileLoadExceptionConsoleApplication\\app.config", programConfig);
      string delaySignAssembly = Compile (
          "Reflection\\TypeDiscovery\\TestAssemblies\\DelaySignAssembly",
          "DelaySignAssembly.dll",
          false,
          "/delaysign+ /keyfile:Reflection\\TypeDiscovery\\TestAssemblies\\DelaySignAssembly\\PublicKey.snk");

      try
      {
        var startInfo = new ProcessStartInfo (program);
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.Arguments = delaySignAssembly + " false";

        Process process = Process.Start (startInfo);
        // ReSharper disable PossibleNullReferenceException
        string output = process.StandardOutput.ReadToEnd ();
        // ReSharper restore PossibleNullReferenceException
        process.WaitForExit ();
        Assert.That (process.ExitCode, Is.EqualTo (0), output);

        CheckLogRegEx (
          output,
            @"WARN : The assembly 'DelaySignAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=.*' \(loaded in the context of "
            + @"'DelaySignAssembly.dll'\) triggered a FileLoadException and will be ignored - maybe the assembly is DelaySigned, but signing has not "
            + "been completed?");
      }
      finally
      {
        FileUtility.DeleteAndWaitForCompletion (program);
        FileUtility.DeleteAndWaitForCompletion (programConfig);
        FileUtility.DeleteAndWaitForCompletion (delaySignAssembly);
      }
    }

    [Test]
    public void TryLoadAssembly_WithFileLoadException_AndShadowCopying ()
    {
      string program = Compile (
          "Reflection\\TypeDiscovery\\TestAssemblies\\FileLoadExceptionConsoleApplication", "FileLoadExceptionConsoleApplication.exe", true, null);
      string delaySignAssembly = Compile ("Reflection\\TypeDiscovery\\TestAssemblies\\DelaySignAssembly", "DelaySignAssembly.dll", false, "/delaysign+ /keyfile:Reflection\\TypeDiscovery\\TestAssemblies\\DelaySignAssembly\\PublicKey.snk");

      try
      {
        var startInfo = new ProcessStartInfo (program);
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.Arguments = delaySignAssembly + " true";

        Process process = Process.Start (startInfo);
        // ReSharper disable PossibleNullReferenceException
        string output = process.StandardOutput.ReadToEnd ();
        // ReSharper restore PossibleNullReferenceException
        process.WaitForExit ();
        Assert.That (process.ExitCode, Is.EqualTo (0), output);
      }
      finally
      {
        FileUtility.DeleteAndWaitForCompletion (program);
        FileUtility.DeleteAndWaitForCompletion (delaySignAssembly);
      }
    }

    [Test]
    [ExpectedException (typeof (AssemblyLoaderException))]
    public void TryLoadAssembly_WithExceptionInShouldConsiderAssembly ()
    {
      var name = typeof (FilteringAssemblyLoaderTest).Assembly.GetName ();

      _filterMock.Expect (mock => mock.ShouldConsiderAssembly (name)).Throw (new Exception ("Fatal error"));

      _filterMock.Replay ();

      _loader.TryLoadAssembly (name, "my context");
    }

    [Test]
    [ExpectedException (typeof (AssemblyLoaderException))]
    public void TryLoadAssembly_WithExceptionInShouldIncludeAssembly ()
    {
      var name = typeof (FilteringAssemblyLoaderTest).Assembly.GetName();

      _filterMock.Expect (mock => mock.ShouldConsiderAssembly (name)).Return (true);
      _filterMock.Expect (mock => mock.ShouldIncludeAssembly (typeof (FilteringAssemblyLoaderTest).Assembly)).Throw (new Exception ("Fatal error"));

      _filterMock.Replay ();

      _loader.TryLoadAssembly (name, "my context");
    }

    [Test]
    public void PerformGuardedLoadOperation_WithNoException ()
    {
      var result = _loader.PerformGuardedLoadOperation ("x", "z", () => "y");
      Assert.That (result, Is.EqualTo ("y"));
    }

    [Test]
    public void PerformGuardedLoadOperation_WithBadImageFormatException ()
    {
      var result = _loader.PerformGuardedLoadOperation<string> ("x", "z", () => { throw new BadImageFormatException ("xy"); });
      Assert.That (result, Is.Null);
    }

    [Test]
    public void PerformGuardedLoadOperation_WithFileLoadException ()
    {
      var result = _loader.PerformGuardedLoadOperation<string> ("x", "z", () => { throw new FileLoadException ("xy"); });
      Assert.That (result, Is.Null);
    }

    [Test]
    public void PerformGuardedLoadOperation_WithFileNotFoundException ()
    {
      var fileNotFoundException = new FileNotFoundException ("xy");
      try
      {
        _loader.PerformGuardedLoadOperation<string> ("x", "z", () => { throw fileNotFoundException; });
        Assert.Fail ("Expected exception.");
      }
      catch (AssemblyLoaderException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo (
                "The assembly 'x' (loaded in the context of 'z') triggered a FileNotFoundException - maybe the assembly does not exist or a referenced "
                + "assembly is missing?\r\nFileNotFoundException message: xy"));
        Assert.That (ex.InnerException, Is.SameAs (fileNotFoundException));
      }
    }

    [Test]
    public void PerformGuardedLoadOperation_WithFileNotFoundException_WorkaroundForSystemIdentityModelSelectors ()
    {
      var fileNotFoundException = new FileNotFoundException ("xy");
      Assert.That (
          () => _loader.PerformGuardedLoadOperation<string> (
              "System.IdentityModel.Selectors, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
              "z",
              () => { throw fileNotFoundException; }),
          Throws.Nothing);
    }

    [Test]
    public void PerformGuardedLoadOperation_WithUnexpectedException ()
    {
      var unexpected = new IndexOutOfRangeException ("xy");
      try
      {
        _loader.PerformGuardedLoadOperation<string> ("x", "z", () => { throw unexpected; });
        Assert.Fail ("Expected exception.");
      }
      catch (AssemblyLoaderException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo (
                "The assembly 'x' (loaded in the context of 'z') triggered an unexpected exception of type System.IndexOutOfRangeException.\r\n"
                + "Unexpected exception message: xy"));
        Assert.That (ex.InnerException, Is.SameAs (unexpected));
      }
    }

    [Test]
    public void PerformGuardedLoadOperation_NoLoadContext ()
    {
      var unexpected = new IndexOutOfRangeException ("xy");
      try
      {
        _loader.PerformGuardedLoadOperation<string> ("x", null, () => { throw unexpected; });
        Assert.Fail ("Expected exception.");
      }
      catch (AssemblyLoaderException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("The assembly 'x' triggered an unexpected exception of type System.IndexOutOfRangeException.\r\n"
                                             + "Unexpected exception message: xy"));
        Assert.That (ex.InnerException, Is.SameAs (unexpected));
      }
    }

    private void SetupFilterTrue ()
    {
      SetupResult.For (_filterMock.ShouldConsiderAssembly (null)).IgnoreArguments().Return (true);
      SetupResult.For (_filterMock.ShouldIncludeAssembly (null)).IgnoreArguments().Return (true);

      _mockRepository.ReplayAll();
    }


    private string Compile (string sourceDirectory, string outputAssemblyName, bool generateExecutable, string compilerOptions)
    {
      var compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssemblyName,
          typeof (FilteringAssemblyLoader).Assembly.Location,
          typeof (Remotion.Logging.LogManager).Assembly.Location);

      compiler.CompilerParameters.GenerateExecutable = generateExecutable;
      compiler.CompilerParameters.CompilerOptions = compilerOptions;
      
      compiler.Compile();
      return compiler.OutputAssemblyPath;
    }

    private void CheckLog (string expectedLogMessage)
    {
      var loggingEvents = _memoryAppender.GetEvents ();
      Assert.That (loggingEvents, Is.Not.Empty);
      
      var fullLog = loggingEvents
          .Select (e => e.Level + " : " + e.RenderedMessage)
          .Aggregate ((text, message) => text + Environment.NewLine + message);
      CheckLog(fullLog, expectedLogMessage);
    }

    private void CheckLog (string fullLog, string expectedLogMessage)
    {
      Assert.That (fullLog, Is.StringContaining(expectedLogMessage));
    }

    private void CheckLogRegEx (string fullLog, string expectedLogRegEx)
    {
      Assert.That (fullLog, Is.StringMatching (expectedLogRegEx));
    }
  }
}
