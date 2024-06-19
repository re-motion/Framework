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
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Remotion.Mixins.XRef.Formatting;
using Remotion.Mixins.XRef.Report;

namespace Remotion.Mixins.XRef
{
  public static class XRef
  {
    private const string c_defaultXmlOutputFileName = "MixinXRef.xml";
    private const string c_defaultXmlOutputDirectory = "C:\\";

    public static void GetAssemblyInformation (
        Assembly? assemblyToCheck = null,
        bool generateFullReport = false,
        bool skipHtmlGeneration = true,
        string outputDirectory = c_defaultXmlOutputDirectory,
        string outputFileName = c_defaultXmlOutputFileName)
    {
      var reflector = new RemotionReflector(assemblyToCheck);

      if (!GetAssemblies(reflector, out var allAssemblies))
        return;

      var mixinConfiguration = reflector.BuildConfigurationFromAssemblies(allAssemblies!);
      var configurationErrors = new ErrorAggregator<Exception>();
      var validationErrors = new ErrorAggregator<Exception>();

      var involvedTypeFinder = new InvolvedTypeFinder(mixinConfiguration, allAssemblies, configurationErrors, validationErrors, reflector);
      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var reportGenerator = GetReportGenerator(generateFullReport, involvedTypes, configurationErrors, validationErrors, reflector);
      var outputDocument = reportGenerator.GenerateXmlDocument();

      var xmlFile = Path.Combine(outputDirectory, outputFileName);
      outputDocument.Save(xmlFile);

      var success = ProcessOutputDocument(xmlFile, outputDirectory, generateFullReport, skipHtmlGeneration);

      //return success;
    }

    private static bool GetAssemblies (RemotionReflector reflector, out Assembly[]? allAssemblies)
    {
      allAssemblies = null;
      var assemblyResolver = AssemblyResolver.Create();
      AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.HandleAssemblyResolve;

      var typeDiscoveryService = reflector.GetTypeDiscoveryService();

      ICollection allTypes;
      try
      {
        allTypes = typeDiscoveryService.GetTypes(null, true);
      }
      catch
      {
        return false;
      }

      allAssemblies = allTypes.Cast<Type>().Select(t => t.Assembly)
          .Distinct()
          .Where(a => !reflector.IsRelevantAssemblyForConfiguration(a) || !reflector.IsNonApplicationAssembly(a))
          .ToArray();

      return allAssemblies.Any();
    }

    private static IXmlReportGenerator GetReportGenerator (
        bool generateFullReport,
        InvolvedType[] involvedTypes,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        RemotionReflector reflector)
    {
      return generateFullReport
          ? new ErrorReportGenerator(configurationErrors, validationErrors, reflector)
          : new FullReportGenerator(involvedTypes, configurationErrors, validationErrors, reflector, new OutputFormatter());
    }

    private static bool ProcessOutputDocument (string xmlFile, string outputDirectory, bool generateFullReport, bool skipHtmlGeneration)
    {
      if (generateFullReport || skipHtmlGeneration)
      {
        return true;
      }

      var transformerExitCode = new XRefTransformer(xmlFile, outputDirectory).GenerateHtmlFromXml();
      if (transformerExitCode != 0)
      {
        return false;
      }

      // copy resources folder
      var xRefPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var directoryInfo = new DirectoryInfo(Path.Combine(xRefPath!, @"xml_utilities\resources"));
      var resourcesPath = Path.Combine(outputDirectory, "resources");
      directoryInfo.CopyTo(resourcesPath);

      return true;
    }
  }
}
