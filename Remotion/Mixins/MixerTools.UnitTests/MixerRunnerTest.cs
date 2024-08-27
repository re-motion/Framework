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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Compilation;

namespace Remotion.Mixins.MixerTools.UnitTests
{
  [TestFixture]
  public class MixerRunnerTest
  {
    private MixerParameters _parameters;

    [SetUp]
    public void SetUp ()
    {
      _parameters = new MixerParameters();
    }

    [Test]
    public void ParameterDefaults ()
    {
      Assert.That(_parameters.AssemblyOutputDirectory, Is.EqualTo(Environment.CurrentDirectory));
      Assert.That(_parameters.BaseDirectory, Is.EqualTo(Environment.CurrentDirectory));
      Assert.That(_parameters.ConfigFile, Is.EqualTo(""));
      Assert.That(_parameters.AssemblyName, Is.EqualTo("Remotion.Mixins.Persistent.{counter}"));
    }

#if NETFRAMEWORK
    [Test]
    public void CreateAppDomainSetup_Default ()
    {
      var setup = MixerRunner.CreateAppDomainSetup(_parameters);

      Assert.That(setup.ApplicationName, Is.EqualTo("Mixer"));
      Assert.That(setup.ApplicationBase, Is.EqualTo(_parameters.BaseDirectory));
    }
#endif

    [Test]
    public void CreateMixer_Default ()
    {
      var runner = new MixerRunner(_parameters);
      var mixer = CallCreateMixer(runner);

      Assert.That(((MixerPipelineFactory)mixer.MixerPipelineFactory).AssemblyName, Is.EqualTo(_parameters.AssemblyName));
      Assert.That(mixer.AssemblyOutputDirectory, Is.EqualTo(_parameters.AssemblyOutputDirectory));
    }

    [Test]
    [Ignore("TODO RM-7799: Create out-of-process test infrastructure to replace tests done with app domains")]
    public void RunDefault ()
    {
      _parameters.AssemblyOutputDirectory = "MixerRunnerTest";
      _parameters.BaseDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "MixerRunnerTest_Input");
      var assemblyPath = Path.Combine(_parameters.AssemblyOutputDirectory, "Remotion.Mixins.Persistent.1.dll");

      Assert.That(Directory.Exists(_parameters.AssemblyOutputDirectory), Is.False);
      Assert.That(File.Exists(assemblyPath), Is.False);

      Assert.That(Directory.Exists(_parameters.BaseDirectory), Is.False);

      try
      {
        Directory.CreateDirectory(_parameters.BaseDirectory);

        var compiler = new AssemblyCompiler(
            Path.Combine(TestContext.CurrentContext.TestDirectory, @"SampleAssembly"),
            Path.Combine(_parameters.BaseDirectory, "SampleAssembly.dll"),
            typeof(Mixin).Assembly.Location);
        compiler.Compile();

        var runner = new MixerRunner(_parameters);
        runner.Run();
        Assert.That(Directory.Exists(_parameters.AssemblyOutputDirectory), Is.True);
        Assert.That(File.Exists(assemblyPath), Is.True);
      }
      finally
      {
        if (Directory.Exists(_parameters.BaseDirectory))
          Directory.Delete(_parameters.BaseDirectory, true);
        if (Directory.Exists(_parameters.AssemblyOutputDirectory))
          Directory.Delete(_parameters.AssemblyOutputDirectory, true);
      }
    }


    private Mixer CallCreateMixer (MixerRunner runner)
    {
      return (Mixer)PrivateInvoke.InvokeNonPublicMethod(runner, "CreateMixer");
    }
  }
}
