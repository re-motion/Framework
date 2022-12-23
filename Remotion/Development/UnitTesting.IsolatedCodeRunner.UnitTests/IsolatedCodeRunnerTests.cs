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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace Remotion.Development.UnitTesting.IsolatedCodeRunner.UnitTests
{
  [TestFixture]
  public class IsolatedCodeRunnerTests
  {
    private static bool _shouldStayFalse;

    [Test]
    public void Constructor_WithNonStaticMethod_Throws ()
    {
      Assert.That(
          () => new IsolatedCodeRunner(_ => { }),
          Throws.ArgumentException.With.Message.EqualTo("The specified test method must be static."));
    }

    [Test]
    public void Run_ExecutesCodeInNewProcess ()
    {
      var isolatedCodeRunner = new IsolatedCodeRunner(TestAction);
      isolatedCodeRunner.Run();

      Assert.That(isolatedCodeRunner.Output.Trim(), Is.EqualTo("ran"));
      Assert.That(_shouldStayFalse, Is.False);

      static void TestAction (string[] args)
      {
        Console.WriteLine("ran");
        _shouldStayFalse = true;
      }
    }

    [Test]
    public void Run_ExceptionIsRethrownInThisProcess ()
    {
      var isolatedCodeRunner = new IsolatedCodeRunner(TestAction);
      Assert.That(
          () => isolatedCodeRunner.Run(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("This is bad."));

      static void TestAction (string[] args)
      {
        Console.WriteLine("HEHEA");
        throw new InvalidOperationException("This is bad.");
      }
    }

    [Test]
    public void Run_ArgumentsArePassedToNewProcess ()
    {
      var isolatedCodeRunner = new IsolatedCodeRunner(TestAction);
      isolatedCodeRunner.Run("a", "b", "c");

      static void TestAction (string[] args)
      {
        Assert.That(args, Is.EqualTo(new[] { "a", "b", "c" }));
      }
    }

    [Test]
    public void Run_CustomConfigFile ()
    {
      Assert.That(ConfigurationManager.AppSettings["customValue"], Is.Null);

      var customConfigFilePath = Path.Combine(
          Path.GetDirectoryName(typeof(IsolatedCodeRunnerTests).Assembly.Location)!,
          "CustomConfigFile.config");
      Assert.That(File.Exists(customConfigFilePath), Is.True);

      var isolatedCodeRunner = new IsolatedCodeRunner(TestAction);
      isolatedCodeRunner.ConfigFile = customConfigFilePath;
      isolatedCodeRunner.Run();

      static void TestAction (string[] args)
      {
        Assert.That(ConfigurationManager.AppSettings["customValue"], Is.EqualTo("123"));
      }
    }

    [Test]
    public void Run_DoubleQuotesAreEscaped ()
    {
      var isolatedCodeRunner = new IsolatedCodeRunner(TestAction);
      isolatedCodeRunner.Run(@"a """, "b");

      static void TestAction (string[] args)
      {
        Assert.That(args, Is.EqualTo(new[] { @"a """, "b" }));
      }
    }

    [Test]
    [Category("LongRunning")]
    public void Run_KillsProcessAfterTimeout ()
    {
      var isolatedCodeRunner = new IsolatedCodeRunner(TestAction, TimeSpan.FromSeconds(3));

      var sw = Stopwatch.StartNew();
      Assert.That(
          () => isolatedCodeRunner.Run(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Isolated code runner execution failed to stop within the specified timeout."));
      sw.Stop();

      Assert.That(sw.Elapsed.Seconds, Is.LessThanOrEqualTo(3));

      static void TestAction (string[] args)
      {
        Thread.Sleep(5_000);
      }
    }
  }
}
