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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Mixins.Context;
using Remotion.Mixins.Validation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTools
{
  /// <summary>
  /// Provides functionality for pre-generating mixed types and saving them to disk to be later loaded via 
  /// <see cref="IPipeline.CodeManager"/>'s <see cref="ICodeManager.LoadFlushedCode"/> method.
  /// </summary>
  public class Mixer
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<Mixer>();

    public static Mixer Create (string assemblyName, string assemblyOutputDirectory, int degreeOfParallelism)
    {
      var builderFactory = new MixerPipelineFactory(assemblyName, degreeOfParallelism);

      // Use a custom TypeDiscoveryService with the LoadAllAssemblyLoaderFilter so that mixed types within system assemblies are also considered.
      var assemblyLoader = new FilteringAssemblyLoader(new LoadAllAssemblyLoaderFilter());
      var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(false, assemblyLoader);
      var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
      var typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);

      var finder = new MixedTypeFinder(typeDiscoveryService);

      return new Mixer(finder, builderFactory, assemblyOutputDirectory);
    }

    public event EventHandler<TypeEventArgs> TypeBeingProcessed = delegate { };
    public event EventHandler<ValidationErrorEventArgs> ValidationErrorOccurred = delegate { };
    public event EventHandler<ErrorEventArgs> ErrorOccurred = delegate { };

    private readonly List<Tuple<Type, Exception>> _errors = new List<Tuple<Type, Exception>>();
    private readonly HashSet<Type> _processedTypes = new HashSet<Type>();
    private readonly Dictionary<Type, Type> _finishedTypes = new Dictionary<Type, Type>();

    private IReadOnlyCollection<string>? _generatedFiles;

    public Mixer (IMixedTypeFinder mixedTypeFinder, IMixerPipelineFactory mixerPipelineFactory, string assemblyOutputDirectory)
    {
      ArgumentUtility.CheckNotNull("mixedTypeFinder", mixedTypeFinder);
      ArgumentUtility.CheckNotNull("mixerPipelineFactory", mixerPipelineFactory);
      ArgumentUtility.CheckNotNull("assemblyOutputDirectory", assemblyOutputDirectory);

      MixedTypeFinder = mixedTypeFinder;
      MixerPipelineFactory = mixerPipelineFactory;
      AssemblyOutputDirectory = assemblyOutputDirectory;
    }

    public IMixedTypeFinder MixedTypeFinder { get; private set; }
    public IMixerPipelineFactory MixerPipelineFactory { get; private set; }
    public string AssemblyOutputDirectory { get; private set; }

    public ReadOnlyCollection<Tuple<Type, Exception>> Errors
    {
      get { return _errors.AsReadOnly(); }
    }

    public IReadOnlyCollection<Type> ProcessedTypes
    {
      get { return _processedTypes.AsReadOnly(); }
    }

    public IReadOnlyDictionary<Type, Type> FinishedTypes
    {
      get { return new ReadOnlyDictionary<Type, Type>(_finishedTypes); }
    }

    public IReadOnlyCollection<string>? GeneratedFiles
    {
      // TODO RM-7818: Getter should return an empty array or throw if Execute was not called.
      get { return _generatedFiles?.AsReadOnly(); }
    }

    public void PrepareOutputDirectory ()
    {
      if (!Directory.Exists(AssemblyOutputDirectory))
      {
        s_logger.LogInformation("Preparing output directory '{0}'.", AssemblyOutputDirectory);
        Directory.CreateDirectory(AssemblyOutputDirectory);
      }

      CleanupIfExists(MixerPipelineFactory.GetModulePaths(AssemblyOutputDirectory));
    }

    // The MixinConfiguration is passed to Execute in order to be able to call PrepareOutputDirectory before analyzing the configuration (and potentially
    // locking old generated files).
    public void Execute (MixinConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull("configuration", configuration);

      using (StopwatchScope.CreateScope(s_logger, LogLevel.Information, "Time needed to mix and save all types: {elapsed}."))
      {
        _errors.Clear();
        _processedTypes.Clear();
        _finishedTypes.Clear();
        _generatedFiles = new string[0];

        s_logger.LogInformation("The base directory is '{0}'.", AppContext.BaseDirectory);

        var pipeline = MixerPipelineFactory.CreatePipeline(AssemblyOutputDirectory);

        var mixedTypes = MixedTypeFinder.FindMixedTypes(configuration).ToArray();

        s_logger.LogInformation("Generating types...");
        using (configuration.EnterScope())
        {
          foreach (var mixedType in mixedTypes)
            Generate(mixedType, pipeline);
        }

        s_logger.LogInformation("Saving assemblies...");
        Save(pipeline);
      }

      s_logger.LogInformation("Successfully generated concrete types for {0} target classes.", _finishedTypes.Count);
    }

    private void Generate (Type mixedType, IPipeline pipeline)
    {
      _processedTypes.Add(mixedType);

      try
      {
        TypeBeingProcessed(this, new TypeEventArgs(mixedType));

        Type concreteType = pipeline.ReflectionService.GetAssembledType(mixedType);
        _finishedTypes.Add(mixedType, concreteType);
      }
      catch (ValidationException validationException)
      {
        _errors.Add(new Tuple<Type, Exception>(mixedType, validationException));
        ValidationErrorOccurred(this, new ValidationErrorEventArgs(validationException));
      }
      catch (Exception ex)
      {
        _errors.Add(new Tuple<Type, Exception>(mixedType, ex));
        ErrorOccurred(this, new ErrorEventArgs(ex));
      }
    }

    private void Save (IPipeline pipeline)
    {
      _generatedFiles = pipeline.CodeManager.FlushCodeToDisk();

      foreach (var generatedFile in _generatedFiles)
        s_logger.LogInformation("Generated assembly file '{0}'.", generatedFile);
    }

    private void CleanupIfExists (string[] paths)
    {
      foreach (var path in paths)
      {
        if (File.Exists(path))
        {
          s_logger.LogInformation("Removing file '{0}'.", path);
          File.Delete(path);
        }
      }
    }
  }
}
