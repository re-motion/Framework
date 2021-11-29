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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Tools.Console.ConsoleApplication;

//. .SyntaxHelpers.Text;

namespace Remotion.Tools.UnitTests.Console.ConsoleApplicationTests
{
  [TestFixture]
  public class ConsoleApplicationTest
  {
    [Test]
    public void CommandLineSwitchShowUsageTest ()
    {
      var args = new[] { "/?" };

      var waitStub = new Mock<IWaiter>();

      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();
      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>(
          stringWriterError, stringWriterOut, 1000, waitStub.Object);

      consoleApplication.Main(args);

      var outResult = stringWriterOut.ToString();
      var errorResult = stringWriterError.ToString();

      Assert.That(outResult, Does.Contain("Application Usage:"));
      Assert.That(outResult, Does.Contain("[/stringArg:string_arg_sample] [/flagArg] [{/?}]"));
      Assert.That(outResult, Does.Contain("/stringArg  stringArg description."));
      Assert.That(outResult, Does.Contain("/flagArg    flagArg description."));
      Assert.That(outResult, Does.Contain("/?          Show usage"));
      Assert.That(outResult, Does.Contain(Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName)));

      Assert.That(errorResult, Is.EqualTo(""));
    }

    [Test]
    public void StringArgFlagArgTest ()
    {
      var args = new[] { "/stringArg:someText /flagArg+" };

      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();

      var waitMock = new Mock<IWaiter>();

      var consoleApplication =
          new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>(
              stringWriterError, stringWriterOut, 80, waitMock.Object
              );


      consoleApplication.Main(args);

      var errorResult = stringWriterError.ToString();

      Assert.That(errorResult, Is.EqualTo(""));
    }

    [Test]
    public void UnknownCommandLineSwitchTest ()
    {
      var args = new[] { "/UNKNOWN_ARGUMENT" };

      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();

      var waitMock = new Mock<IWaiter>();

      var consoleApplication =
          new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>(
              stringWriterError, stringWriterOut, 80, waitMock.Object
              );


      consoleApplication.Main(args);

      var errorResult = stringWriterError.ToString();
      Assert.That(errorResult, Does.Contain(@"An error occured: Argument /UNKNOWN_ARGUMENT: invalid argument name"));
    }

    [Test]
    public void WaitForKeypressTest ()
    {
      var args = new[] { "/wait+" };

      var waitMock = new Mock<IWaiter>();
      waitMock.Setup(mock => mock.Wait()).Verifiable();

      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>(
          TextWriter.Null, TextWriter.Null, 80, waitMock.Object);

      consoleApplication.Main(args);

      Assert.That(consoleApplication.Settings.WaitForKeypress, Is.True);
      waitMock.Verify();
    }

    [Test]
    public void BufferWidthTest ()
    {
      const int bufferWidth = 37;

      var waitMock = new Mock<IWaiter>();

      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>(
          TextWriter.Null, TextWriter.Null, bufferWidth, waitMock.Object);

      Assert.That(consoleApplication.BufferWidth, Is.EqualTo(bufferWidth));
    }

    [Test]
    public void DefaultCtorTest ()
    {
      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>();
      Assert.That(PrivateInvoke.GetNonPublicField(consoleApplication, "_errorWriter"), Is.EqualTo(System.Console.Error));
      Assert.That(PrivateInvoke.GetNonPublicField(consoleApplication, "_logWriter"), Is.EqualTo(System.Console.Out));
      Assert.That(consoleApplication.BufferWidth, Is.EqualTo(80));
      Assert.That(PrivateInvoke.GetNonPublicField(consoleApplication, "_waitAtEnd"), Is.TypeOf(typeof(ConsoleKeypressWaiter)));
    }

    [Test]
    public void RunApplicationTextWriterArgumentPassingTest ()
    {
      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();

      var waitStub = new Mock<IWaiter>();

      var applicationRunnerMock = new Mock<IApplicationRunner<ConsoleApplicationTestSettings>>();
      var consoleApplicationMock =
          new Mock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>>(
              stringWriterError, stringWriterOut, 80, waitStub.Object) { CallBase = true };

      consoleApplicationMock.Setup(x => x.CreateApplication()).Returns(applicationRunnerMock.Object).Verifiable();
      applicationRunnerMock.Setup(
          x => x.Run(
                   It.IsAny<ConsoleApplicationTestSettings>(),
                   stringWriterError,
                   stringWriterOut)).Verifiable();

      consoleApplicationMock.Object.Main(new string[0]);

      applicationRunnerMock.Verify();
      consoleApplicationMock.Verify();
    }

    [Test]
    public void RunApplicationExeptionTest ()
    {
      var stringWriterError = new StringWriter();
      var stringWriterOut = TextWriter.Null;

      var waitStub = new Mock<IWaiter>();

      var applicationRunnerMock = new Mock<IApplicationRunner<ConsoleApplicationTestSettings>>();
      var consoleApplicationMock =
          new Mock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>>(
              stringWriterError, stringWriterOut, 80, waitStub.Object) { CallBase = true };

      consoleApplicationMock.Setup(x => x.CreateApplication()).Returns(applicationRunnerMock.Object).Verifiable();
      applicationRunnerMock.Setup(
          x => x.Run(
                   It.IsAny<ConsoleApplicationTestSettings>(),
                   It.IsAny<TextWriter>(),
                   It.IsAny<TextWriter>())).Throws(new Exception("The valve just came loose...")).Verifiable();

      consoleApplicationMock.Object.Main(new string[0]);

      applicationRunnerMock.Verify();
      consoleApplicationMock.Verify();

      var result = stringWriterError.ToString();
      Assert.That(result, Does.StartWith("Execution aborted. Exception stack:\r\nSystem.Exception: The valve just came loose..."));
    }
  }
}