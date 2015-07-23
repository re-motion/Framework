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
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Development;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static readonly IPipelineRegistry s_pipelineRegistry = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>();

    private static IPipeline s_pipeline;
    private static bool s_skipDeletion;

    private static AssemblyTrackingCodeManager s_assemblyTrackingCodeManager;

    public static IPipelineRegistry PipelineRegistry
    {
      get { return s_pipelineRegistry; }
    }

    public static IPipeline Pipeline
    {
      get
      {
        if (s_pipeline == null)
          throw new InvalidOperationException ("SetUp must be executed first.");
        return s_pipeline;
      }
    }

    /// <summary>
    /// Signals that the <see cref="SetUpFixture"/> should not delete the files it generates. Call this ad-hoc in a test to keep the files and inspect
    /// them with Reflector or ildasm.
    /// </summary>
    public static void SkipDeletion ()
    {
      s_skipDeletion = true;
    }

    /// <summary>
    /// Adds an assembly to be verified and deleted at the end of the test runs.
    /// </summary>
    public static void AddSavedAssembly (string assemblyPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assemblyPath", assemblyPath);
      s_assemblyTrackingCodeManager.AddSavedAssembly (assemblyPath);
    }

    [SetUp]
    public void SetUp ()
    {
      var assemblyTrackingPipelineFactory = new AssemblyTrackingPipelineFactory();
      var settings = PipelineSettings.New().SetEnableSerializationWithoutAssemblySaving (true).Build();
      var participants = new IParticipant[] { new MixinParticipant() };

      s_pipeline = assemblyTrackingPipelineFactory.Create ("re-mix-tests", settings, participants);
      s_assemblyTrackingCodeManager = assemblyTrackingPipelineFactory.AssemblyTrackingCodeManager;
    }

    [TearDown]
    public void TearDown()
    {
#if !NO_PEVERIFY
      try
      {
        s_assemblyTrackingCodeManager.FlushCodeToDisk();
      }
      catch (Exception ex)
      {
        Assert.Fail ("Error when saving assemblies: {0}", ex);
      }

      s_assemblyTrackingCodeManager.PeVerifySavedAssemblies();
#endif

      if (!s_skipDeletion)
      {
        s_assemblyTrackingCodeManager.DeleteSavedAssemblies(); // Delete assemblies if everything went fine.
      }
      else
      {
        Console.WriteLine (
            "Assemblies saved to: " + Environment.NewLine
            + string.Join (Environment.NewLine, s_assemblyTrackingCodeManager.SavedAssemblies));
      }
    }
  }
}
