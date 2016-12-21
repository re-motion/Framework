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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Tools.Console.ConsoleApplication;
using Rhino.Mocks;

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

      var waitStub = MockRepository.GenerateMock<IWaiter>();

      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();
      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
          stringWriterError, stringWriterOut, 1000, waitStub);

      consoleApplication.Main (args);

      var outResult = stringWriterOut.ToString();
      var errorResult = stringWriterError.ToString();

      Assert.That (outResult, Is.StringContaining("Application Usage:"));
      Assert.That (outResult, Is.StringContaining ("[/stringArg:string_arg_sample] [/flagArg] [{/?}]"));
      Assert.That (outResult, Is.StringContaining ("/stringArg  stringArg description."));
      Assert.That (outResult, Is.StringContaining ("/flagArg    flagArg description."));
      Assert.That (outResult, Is.StringContaining ("/?          Show usage"));
      Assert.That (outResult, Is.StringContaining (Path.GetFileName (Process.GetCurrentProcess ().MainModule.FileName)));

      Assert.That (errorResult, Is.EqualTo (""));
    }

    [Test]
    public void StringArgFlagArgTest ()
    {
      var args = new[] { "/stringArg:someText /flagArg+" };

      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();

      var waitMock = MockRepository.GenerateMock<IWaiter>();

      var consoleApplication =
          new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
              stringWriterError, stringWriterOut, 80, waitMock
              );


      consoleApplication.Main (args);

      var errorResult = stringWriterError.ToString();

      Assert.That (errorResult, Is.EqualTo (""));
    }

    [Test]
    public void UnknownCommandLineSwitchTest ()
    {
      var args = new[] { "/UNKNOWN_ARGUMENT" };

      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();

      var waitMock = MockRepository.GenerateMock<IWaiter>();

      var consoleApplication =
          new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
              stringWriterError, stringWriterOut, 80, waitMock
              );


      consoleApplication.Main (args);

      var errorResult = stringWriterError.ToString();
      Assert.That (errorResult, Is.StringContaining(@"An error occured: Argument /UNKNOWN_ARGUMENT: invalid argument name"));
    }

    [Test]
    public void WaitForKeypressTest ()
    {
      var args = new[] { "/wait+" };

      var waitMock = MockRepository.GenerateMock<IWaiter>();
      waitMock.Expect (mock => mock.Wait());
      waitMock.Replay();

      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
          TextWriter.Null, TextWriter.Null, 80, waitMock);

      consoleApplication.Main (args);

      Assert.That (consoleApplication.Settings.WaitForKeypress, Is.True);
      waitMock.VerifyAllExpectations();
    }

    [Test]
    public void BufferWidthTest ()
    {
      const int bufferWidth = 37;

      var waitMock = MockRepository.GenerateStub<IWaiter>();

      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
          TextWriter.Null, TextWriter.Null, bufferWidth, waitMock);

      Assert.That (consoleApplication.BufferWidth, Is.EqualTo (bufferWidth));
    }

    [Test]
    public void DefaultCtorTest ()
    {
      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>();
      Assert.That (PrivateInvoke.GetNonPublicField (consoleApplication, "_errorWriter"), Is.EqualTo (System.Console.Error));
      Assert.That (PrivateInvoke.GetNonPublicField (consoleApplication, "_logWriter"), Is.EqualTo (System.Console.Out));
      Assert.That (consoleApplication.BufferWidth, Is.EqualTo (80));
      Assert.That (PrivateInvoke.GetNonPublicField (consoleApplication, "_waitAtEnd"), Is.TypeOf (typeof (ConsoleKeypressWaiter)));
    }

    [Test]
    public void RunApplicationTextWriterArgumentPassingTest ()
    {
      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter();

      var waitStub = MockRepository.GenerateStub<IWaiter>();

      var mocks = new MockRepository();
      var applicationRunnerMock = mocks.DynamicMock<IApplicationRunner<ConsoleApplicationTestSettings>>();
      var consoleApplicationMock =
          mocks.PartialMock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>> (
              stringWriterError, stringWriterOut, 80, waitStub);


      consoleApplicationMock.Expect (x => x.CreateApplication()).Return (applicationRunnerMock);
      applicationRunnerMock.Expect (
          x => x.Run (
                   Arg<ConsoleApplicationTestSettings>.Is.Anything,
                   Arg<TextWriter>.Is.Equal (stringWriterError),
                   Arg<TextWriter>.Is.Equal (stringWriterOut)));

      consoleApplicationMock.Replay();
      applicationRunnerMock.Replay();

      consoleApplicationMock.Main (new string[0]);

      applicationRunnerMock.VerifyAllExpectations();
      consoleApplicationMock.VerifyAllExpectations();
    }

    [Test]
    public void RunApplicationExeptionTest ()
    {
      var stringWriterError = new StringWriter();
      var stringWriterOut = TextWriter.Null;

      var waitStub = MockRepository.GenerateStub<IWaiter>();

      var mocks = new MockRepository();
      var applicationRunnerMock = mocks.DynamicMock<IApplicationRunner<ConsoleApplicationTestSettings>>();
      var consoleApplicationMock =
          mocks.PartialMock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>> (
              stringWriterError, stringWriterOut, 80, waitStub);

      consoleApplicationMock.Expect (x => x.CreateApplication()).Return (applicationRunnerMock);
      applicationRunnerMock.Expect (
          x => x.Run (
                   Arg<ConsoleApplicationTestSettings>.Is.Anything,
                   Arg<TextWriter>.Is.Anything,
                   Arg<TextWriter>.Is.Anything)).Throw (new Exception ("The valve just came loose..."));

      consoleApplicationMock.Replay();
      applicationRunnerMock.Replay();

      consoleApplicationMock.Main (new string[0]);

      applicationRunnerMock.VerifyAllExpectations();
      consoleApplicationMock.VerifyAllExpectations();

      var result = stringWriterError.ToString();
      Assert.That (result, Is.StringStarting("Execution aborted. Exception stack:\r\nSystem.Exception: The valve just came loose..."));
    }
  }
}