// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.ComponentModel.Design;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Security.Metadata;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.ServiceLocation;
using Remotion.Tools.Console.CommandLine;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Metadata.Importer
{
  public class Program
  {
    public static int Main (string[] args)
    {
      CommandLineArguments arguments = GetArguments(args);
      if (arguments == null)
        return 1;

      Program program = new Program(arguments);
      return program.Run();
    }

    private static CommandLineArguments GetArguments (string[] args)
    {
      CommandLineClassParser parser = new CommandLineClassParser(typeof(CommandLineArguments));

      try
      {
        return (CommandLineArguments)parser.Parse(args);
      }
      catch (CommandLineArgumentException e)
      {
        Console.WriteLine(e.Message);
        WriteUsage(parser);

        return null;
      }
    }

    private static void WriteUsage (CommandLineClassParser parser)
    {
      Console.WriteLine("Usage:");

      string commandName = Environment.GetCommandLineArgs()[0];
      Console.WriteLine(parser.GetAsciiSynopsis(commandName, Console.BufferWidth));
    }

    private CommandLineArguments _arguments;

    private Program (CommandLineArguments arguments)
    {
      ArgumentUtility.CheckNotNull("arguments", arguments);
      _arguments = arguments;
    }

    public int Run ()
    {
      try
      {
        ServiceLocator.SetLocatorProvider(() => null);

        var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
        var rootAssemblyFinder = new FixedRootAssemblyFinder(new RootAssembly(typeof(BaseSecurityManagerObject).Assembly, true));
        var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));

        ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);
        MappingConfiguration.SetCurrent(
            new MappingConfiguration(
                new MappingReflector(
                    typeDiscoveryService,
                    new ClassIDProvider(),
                    new ReflectionBasedMemberInformationNameResolver(),
                    new PropertyMetadataReflector(),
                    new DomainModelConstraintProvider(),
                    new LegacyPropertyDefaultValueProvider(),
                    new SortExpressionDefinitionProvider(),
                    MappingReflector.CreateDomainObjectCreator()),
                new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage))));

        ClientTransaction transaction = ClientTransaction.CreateRootTransaction();

        if (_arguments.ImportMetadata)
          ImportMetadata(transaction);

        transaction.Commit();

        if (_arguments.ImportLocalization)
          ImportLocalization(transaction);

        transaction.Commit();

        return 0;
      }
      catch (Exception e)
      {
        HandleException(e);
        return 1;
      }
    }

    private void ImportMetadata (ClientTransaction transaction)
    {
      MetadataImporter importer = new MetadataImporter(transaction);
      WriteInfo("Importing metadata file '{0}'.", _arguments.MetadataFile);
      importer.Import(_arguments.MetadataFile);
    }

    private void ImportLocalization (ClientTransaction transaction)
    {
      CultureImporter importer = new CultureImporter(transaction);
      LocalizationFileNameStrategy localizationFileNameStrategy = new LocalizationFileNameStrategy();
      string[] localizationFileNames = localizationFileNameStrategy.GetLocalizationFileNames(_arguments.MetadataFile);

      foreach (string localizationFileName in localizationFileNames)
      {
        WriteInfo("Importing localization file '{0}'.", localizationFileName);
        importer.Import(localizationFileName);
      }

      if (localizationFileNames.Length == 0)
        WriteInfo("Localization files not found.");
    }

    private void HandleException (Exception exception)
    {
      if (_arguments.Verbose)
      {
        Console.Error.WriteLine("Execution aborted. Exception stack:");

        for (; exception != null; exception = exception.InnerException)
          Console.Error.WriteLine("{0}: {1}\n{2}", exception.GetType().GetFullNameSafe(), exception.Message, exception.StackTrace);
      }
      else
        Console.Error.WriteLine("Execution aborted: {0}", exception.Message);
    }

    private void WriteInfo (string text, params object[] args)
    {
      if (_arguments.Verbose)
        Console.WriteLine(text, args);
    }
  }
}
