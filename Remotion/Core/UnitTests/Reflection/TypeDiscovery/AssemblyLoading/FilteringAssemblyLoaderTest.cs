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
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using Moq;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyLoading
{
  [TestFixture]
  public class FilteringAssemblyLoaderTest
  {
    private Mock<IAssemblyLoaderFilter> _filterMock;
    private FilteringAssemblyLoader _loader;

    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _filterMock = new Mock<IAssemblyLoaderFilter>(MockBehavior.Strict);
      _loader = new FilteringAssemblyLoader(_filterMock.Object);

      _memoryAppender = new MemoryAppender();
      BasicConfigurator.Configure(_memoryAppender);
      Assert.That(LogManager.GetLogger(typeof(FilteringAssemblyLoader)).IsDebugEnabled, Is.True);
    }

    [TearDown]
    public void TearDown ()
    {
      _memoryAppender.Clear();
      LogManager.ResetConfiguration();

      Assert.That(LogManager.GetLogger(typeof(FilteringAssemblyLoader)).IsDebugEnabled, Is.False);
    }

    [Test]
    public void TryLoadAssembly ()
    {
      SetupFilterTrue();

      Assembly referenceAssembly = typeof(FilteringAssemblyLoaderTest).Assembly;
      string path = referenceAssembly.Location;
      Assembly loadedAssembly = _loader.TryLoadAssembly(path);
      Assert.That(loadedAssembly, Is.SameAs(referenceAssembly));
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderTrue_IncludeTrue ()
    {
      Assembly referenceAssembly = typeof(FilteringAssemblyLoaderTest).Assembly;
      string path = referenceAssembly.Location;

      _filterMock
          .Setup(_ => _.ShouldConsiderAssembly(It.Is<AssemblyName>(_ => _ != null && object.Equals(_.FullName, referenceAssembly.FullName))))
          .Returns(true)
          .Verifiable();
      _filterMock
          .Setup(_ => _.ShouldIncludeAssembly(It.Is<Assembly>(_ => _ != null && object.Equals(_.FullName, referenceAssembly.FullName))))
          .Returns(true)
          .Verifiable();

      Assembly loadedAssembly = _loader.TryLoadAssembly(path);
      Assert.That(loadedAssembly, Is.SameAs(referenceAssembly));
      _filterMock.Verify();
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderTrue_IncludeFalse ()
    {
      Assembly referenceAssembly = typeof(FilteringAssemblyLoaderTest).Assembly;
      string path = referenceAssembly.Location;

      _filterMock
          .Setup(_ => _.ShouldConsiderAssembly(It.Is<AssemblyName>(_ => _ != null && object.Equals(_.FullName, referenceAssembly.FullName))))
          .Returns(true)
          .Verifiable();
      _filterMock
          .Setup(_ => _.ShouldIncludeAssembly(It.Is<Assembly>(_ => _ != null && object.Equals(_.FullName, referenceAssembly.FullName))))
          .Returns(false)
          .Verifiable();

      Assembly loadedAssembly = _loader.TryLoadAssembly(path);
      Assert.That(loadedAssembly, Is.Null);
      _filterMock.Verify();
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderFalse ()
    {
      Assembly referenceAssembly = typeof(FilteringAssemblyLoaderTest).Assembly;
      string path = referenceAssembly.Location;

      _filterMock
          .Setup(_ => _.ShouldConsiderAssembly(It.Is<AssemblyName>(_ => _ != null && object.Equals(_.FullName, referenceAssembly.FullName))))
          .Returns(false)
          .Verifiable();

      Assembly loadedAssembly = _loader.TryLoadAssembly(path);
      Assert.That(loadedAssembly, Is.Null);
      _filterMock.Verify();
    }

    [Test]
    public void TryLoadAssembly_WithBadImageFormatException ()
    {
      SetupFilterTrue();

      const string path = "Invalid.dll";
      using (File.CreateText(path))
      {
        // no contents
      }

      try
      {
        Assembly loadedAssembly = _loader.TryLoadAssembly(path);
        Assert.That(loadedAssembly, Is.Null);

        CheckLog(
            "INFO : The file 'Invalid.dll' triggered a BadImageFormatException and will be ignored. Possible causes for this are:" + Environment.NewLine
            + "- The file is not a .NET assembly." + Environment.NewLine
            + "- The file was built for a newer version of .NET." + Environment.NewLine
            + "- The file was compiled for a different platform (x86, x64, etc.) than the platform this process is running on." + Environment.NewLine
            + "- The file is damaged.");
      }
      finally
      {
        FileUtility.DeleteAndWaitForCompletion(path);
      }
    }

    [Test]
    public void TryLoadAssembly_WithExceptionInShouldConsiderAssembly ()
    {
      var name = typeof(FilteringAssemblyLoaderTest).Assembly.GetName();

      _filterMock.Setup(mock => mock.ShouldConsiderAssembly(name)).Throws(new Exception("Fatal error")).Verifiable();

      Assert.That(
          () => _loader.TryLoadAssembly(name, "my context"),
          Throws.InstanceOf<AssemblyLoaderException>());
    }

    [Test]
    public void TryLoadAssembly_WithExceptionInShouldIncludeAssembly ()
    {
      var name = typeof(FilteringAssemblyLoaderTest).Assembly.GetName();

      _filterMock.Setup(mock => mock.ShouldConsiderAssembly(name)).Returns(true).Verifiable();
      _filterMock.Setup(mock => mock.ShouldIncludeAssembly(typeof(FilteringAssemblyLoaderTest).Assembly)).Throws(new Exception("Fatal error")).Verifiable();

      Assert.That(
          () => _loader.TryLoadAssembly(name, "my context"),
          Throws.InstanceOf<AssemblyLoaderException>());
    }

    [Test]
    public void PerformGuardedLoadOperation_WithNoException ()
    {
      var result = _loader.PerformGuardedLoadOperation("x", "z", () => "y");
      Assert.That(result, Is.EqualTo("y"));
    }

    [Test]
    public void PerformGuardedLoadOperation_WithBadImageFormatException ()
    {
      var result = _loader.PerformGuardedLoadOperation<string>("x", "z", () => { throw new BadImageFormatException("xy"); });
      Assert.That(result, Is.Null);
    }

    [Test]
    public void PerformGuardedLoadOperation_WithFileLoadException ()
    {
      var result = _loader.PerformGuardedLoadOperation<string>("x", "z", () => { throw new FileLoadException("xy"); });
      Assert.That(result, Is.Null);
    }

    [Test]
    public void PerformGuardedLoadOperation_WithFileNotFoundException ()
    {
      var fileNotFoundException = new FileNotFoundException("xy");
      try
      {
        _loader.PerformGuardedLoadOperation<string>("x", "z", () => { throw fileNotFoundException; });
        Assert.Fail("Expected exception.");
      }
      catch (AssemblyLoaderException ex)
      {
        Assert.That(
            ex.Message,
            Is.EqualTo(
                "The assembly 'x' (loaded in the context of 'z') triggered a FileNotFoundException - maybe the assembly does not exist or a referenced "
                + "assembly is missing?\r\nFileNotFoundException message: xy"));
        Assert.That(ex.InnerException, Is.SameAs(fileNotFoundException));
      }
    }

    [Test]
    public void PerformGuardedLoadOperation_WithFileNotFoundException_WorkaroundForSystemIdentityModelSelectors ()
    {
      var fileNotFoundException = new FileNotFoundException("xy");
      Assert.That(
          () => _loader.PerformGuardedLoadOperation<string>(
              "System.IdentityModel.Selectors, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
              "z",
              () => { throw fileNotFoundException; }),
          Throws.Nothing);
    }

    [Test]
    public void PerformGuardedLoadOperation_WithUnexpectedException ()
    {
      var unexpected = new IndexOutOfRangeException("xy");
      try
      {
        _loader.PerformGuardedLoadOperation<string>("x", "z", () => { throw unexpected; });
        Assert.Fail("Expected exception.");
      }
      catch (AssemblyLoaderException ex)
      {
        Assert.That(
            ex.Message,
            Is.EqualTo(
                "The assembly 'x' (loaded in the context of 'z') triggered an unexpected exception of type System.IndexOutOfRangeException.\r\n"
                + "Unexpected exception message: xy"));
        Assert.That(ex.InnerException, Is.SameAs(unexpected));
      }
    }

    [Test]
    public void PerformGuardedLoadOperation_NoLoadContext ()
    {
      var unexpected = new IndexOutOfRangeException("xy");
      try
      {
        _loader.PerformGuardedLoadOperation<string>("x", null, () => { throw unexpected; });
        Assert.Fail("Expected exception.");
      }
      catch (AssemblyLoaderException ex)
      {
        Assert.That(ex.Message, Is.EqualTo("The assembly 'x' triggered an unexpected exception of type System.IndexOutOfRangeException.\r\n"
                                             + "Unexpected exception message: xy"));
        Assert.That(ex.InnerException, Is.SameAs(unexpected));
      }
    }

    private void SetupFilterTrue ()
    {
      _filterMock.Setup(_ => _.ShouldConsiderAssembly(It.IsAny<AssemblyName>())).Returns(true).Verifiable();
      _filterMock.Setup(_ => _.ShouldIncludeAssembly(It.IsAny<Assembly>())).Returns(true).Verifiable();
    }

    private void CheckLog (string expectedLogMessage)
    {
      var loggingEvents = _memoryAppender.GetEvents();
      Assert.That(loggingEvents, Is.Not.Empty);

      var fullLog = loggingEvents
          .Select(e => e.Level + " : " + e.RenderedMessage)
          .Aggregate((text, message) => text + Environment.NewLine + message);
      CheckLog(fullLog, expectedLogMessage);
    }

    private void CheckLog (string fullLog, string expectedLogMessage)
    {
      Assert.That(fullLog, Does.Contain(expectedLogMessage));
    }
  }
}
