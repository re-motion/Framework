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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static IPipeline s_pipeline;
    private ServiceLocatorScope s_serviceLocatorScope;

#if FEATURE_ASSEMBLYBUILDER_SAVE
    private static bool s_skipDeletion;
    private static Remotion.TypePipe.Development.AssemblyTrackingCodeManager s_assemblyTrackingCodeManager;
#endif


    public static IPipeline Pipeline
    {
      get
      {
        if (s_pipeline == null)
          throw new InvalidOperationException("SetUp must be executed first.");
        return s_pipeline;
      }
    }

    /// <summary>
    /// Signals that the <see cref="SetUpFixture"/> should not delete the files it generates. Call this ad-hoc in a test to keep the files and inspect
    /// them with Reflector or ildasm.
    /// </summary>
    public static void SkipDeletion ()
    {
#if FEATURE_ASSEMBLYBUILDER_SAVE
      s_skipDeletion = true;
#endif
    }

    /// <summary>
    /// Adds an assembly to be verified and deleted at the end of the test runs.
    /// </summary>
    public static void AddSavedAssembly (string assemblyPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("assemblyPath", assemblyPath);
#if FEATURE_ASSEMBLYBUILDER_SAVE
      s_assemblyTrackingCodeManager.AddSavedAssembly(assemblyPath);
#endif
    }

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      // Force-initialize ObjectFactory to ensure it does not default onto the test pipeline.
      Dev.Null = ObjectFactory.Create<object>();
      // Force-initialize TestFactory to ensure it does not default onto the test pipeline.
      Dev.Null = TypeFactory.GetConcreteType(typeof(object));

      var settings = PipelineSettings.New().SetEnableSerializationWithoutAssemblySaving(true).SetAssemblyDirectory(TestContext.CurrentContext.TestDirectory).Build();
      var participants = new IParticipant[]
                         {
                           new MixinParticipant(
                               SafeServiceLocator.Current.GetInstance<IConfigurationProvider>(),
                               SafeServiceLocator.Current.GetInstance<IMixinTypeProvider>(),
                               SafeServiceLocator.Current.GetInstance<ITargetTypeModifier>(),
                               SafeServiceLocator.Current.GetInstance<IConcreteTypeMetadataImporter>())
                         };
      var pipelineName = "re-mix-tests";
#if FEATURE_ASSEMBLYBUILDER_SAVE
      var assemblyTrackingPipelineFactory = new Remotion.TypePipe.Development.AssemblyTrackingPipelineFactory();

      s_pipeline = assemblyTrackingPipelineFactory.Create(pipelineName, settings, participants);
      s_assemblyTrackingCodeManager = assemblyTrackingPipelineFactory.AssemblyTrackingCodeManager;
#else
      var defaultPipelineFactory = new DefaultPipelineFactory();
      s_pipeline = defaultPipelineFactory.Create(pipelineName, settings, participants);
#endif
      var pipelineRegistry = new DefaultPipelineRegistry(s_pipeline);
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IPipelineRegistry>(() => pipelineRegistry);
      s_serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown ()
    {
#if FEATURE_ASSEMBLYBUILDER_SAVE
      try
      {
        s_assemblyTrackingCodeManager.FlushCodeToDisk();
      }
      catch (Exception ex)
      {
        Assert.Fail($"Error when saving assemblies: {ex}");
      }

      //TODO RM-9285: Re-Introduce assembly verification

      if (!s_skipDeletion)
      {
        s_assemblyTrackingCodeManager.DeleteSavedAssemblies(); // Delete assemblies if everything went fine.
      }
      else
      {
        Console.WriteLine(
            "Assemblies saved to: " + Environment.NewLine
            + string.Join(Environment.NewLine, s_assemblyTrackingCodeManager.SavedAssemblies));
      }
#endif
      s_serviceLocatorScope.Dispose();
    }
  }
}
