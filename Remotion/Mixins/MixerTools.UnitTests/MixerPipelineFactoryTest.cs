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
using Remotion.Mixins.MixerTools.UnitTests.TestDomain;
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Mixins.MixerTools.UnitTests
{
  [TestFixture]
  public class MixerPipelineFactoryTest
  {
    [Test]
    public void CreatePipeline ()
    {
      var factory = new MixerPipelineFactory("Assembly_{counter}", 2);
      var pipeline = factory.CreatePipeline(@"c:\directory");

      CheckRemotionPipelineFactoryWasUsedForCreation(pipeline);

      var defaultPipeline = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().DefaultPipeline;
      Assert.That(pipeline.ParticipantConfigurationID, Is.EqualTo(defaultPipeline.ParticipantConfigurationID));
      Assert.That(pipeline.Participants, Is.EqualTo(defaultPipeline.Participants));

      Assert.That(pipeline.Settings.AssemblyNamePattern, Is.EqualTo("Assembly_{counter}"));
      Assert.That(pipeline.Settings.AssemblyDirectory, Is.EqualTo(@"c:\directory"));
      Assert.That(
          pipeline.Settings.EnableSerializationWithoutAssemblySaving,
          Is.EqualTo(defaultPipeline.Settings.EnableSerializationWithoutAssemblySaving));
#if FEATURE_STRONGNAMESIGNING      
      Assert.That(pipeline.Settings.ForceStrongNaming, Is.EqualTo(defaultPipeline.Settings.ForceStrongNaming));
      Assert.That(pipeline.Settings.KeyFilePath, Is.EqualTo(defaultPipeline.Settings.KeyFilePath));
#endif
      Assert.That(pipeline.Settings.DegreeOfParallelism, Is.EqualTo(2));
    }

    [Test]
    public void GetModulePaths_WithoutCounterPattern_WithoutGeneratedFiles_ReturnsEmptyList ()
    {
      var tempDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
      try
      {
        var factory = new MixerPipelineFactory("Assembly", 1);
        File.Create(Path.Combine(tempDirectory.FullName, "AssemblyOther.dll")).Close();

        Assert.That(factory.GetModulePaths(tempDirectory.FullName), Is.Empty);
      }
      finally
      {
        tempDirectory.Delete(true);
      }
    }

    [Test]
    public void GetModulePaths_WithoutCounterPattern_WithGeneratedFiles_ReturnsMatchingFiles ()
    {
      var tempDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
      try
      {
        var factory = new MixerPipelineFactory("Assembly", 1);

        var assembly = Path.Combine(tempDirectory.FullName, "Assembly.dll");
        File.Create(assembly).Close();

        File.Create(Path.Combine(tempDirectory.FullName, "AssemblyOther.dll")).Close();

        Assert.That(factory.GetModulePaths(tempDirectory.FullName), Is.EqualTo(new[] { assembly }));
      }
      finally
      {
        tempDirectory.Delete(true);
      }
    }

    [Test]
    public void GetModulePaths_WithCounterPattern_WithGeneratedFiles_ReturnsMatchingFiles ()
    {
      var tempDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
      try
      {
        var factory = new MixerPipelineFactory("Assembly.{counter}", 1);

        var assembly1 = Path.Combine(tempDirectory.FullName, "Assembly.1.dll");
        File.Create(assembly1).Close();

        var assembly2 = Path.Combine(tempDirectory.FullName, "Assembly.2.dll");
        File.Create(assembly2).Close();

        File.Create(Path.Combine(tempDirectory.FullName, "AssemblyOther.dll")).Close();

        Assert.That(factory.GetModulePaths(tempDirectory.FullName), Is.EqualTo(new[] { assembly1, assembly2 }));
      }
      finally
      {
        tempDirectory.Delete(true);
      }
    }

    private void CheckRemotionPipelineFactoryWasUsedForCreation (IPipeline pipeline)
    {
      var targetType = typeof(TargetClassForGlobalMix);
      var assembledType = pipeline.ReflectionService.GetAssembledType(targetType);
      var assembly = assembledType.Assembly;

      Assert.That(assembledType, Is.Not.EqualTo(targetType));

      // RemotionPipelineFactory will add the [NonApplicationAssemblyAttribute] to the generated assembly.
      Assert.That(assembly.IsDefined(typeof(NonApplicationAssemblyAttribute), inherit: false), Is.True);
    }
  }
}
