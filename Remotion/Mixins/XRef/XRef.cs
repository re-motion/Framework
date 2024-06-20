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
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.CrossReferencer
{
  public static class XRef
  {
    private const string c_defaultXmlOutputFileName = "MixinXRef.xml";
    private const string c_defaultXmlOutputDirectory = "C:\\";

    public static XDocument? GetAssemblyInformation (
        Assembly? assemblyToCheck = null,
        bool generateFullReport = false,
        bool skipHtmlGeneration = true,
        string outputDirectory = c_defaultXmlOutputDirectory,
        string outputFileName = c_defaultXmlOutputFileName)
    {
      var reflector = new RemotionReflector(assemblyToCheck);

      if (!GetAssemblies(reflector, out var allAssemblies))
        return null;

      var mixinConfiguration = reflector.BuildConfigurationFromAssemblies(allAssemblies!);
      var configurationErrors = new ErrorAggregator<ConfigurationException>();
      var validationErrors = new ErrorAggregator<ValidationException>();

      var involvedTypeFinder = new InvolvedTypeFinder(mixinConfiguration, allAssemblies, configurationErrors, validationErrors, reflector);
      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var reportGenerator = GetReportGenerator(generateFullReport, involvedTypes, configurationErrors, validationErrors, reflector);
      var outputDocument = reportGenerator.GenerateXmlDocument();

      var xmlFile = Path.Combine(outputDirectory, outputFileName);
      outputDocument.Save(xmlFile);

      return outputDocument;
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
        ErrorAggregator<ConfigurationException> configurationErrors,
        ErrorAggregator<ValidationException> validationErrors,
        RemotionReflector reflector)
    {
      return generateFullReport
          ? new ErrorReportGenerator(configurationErrors, validationErrors, reflector)
          : new FullReportGenerator(involvedTypes, configurationErrors, validationErrors, reflector, new OutputFormatter());
    }
  }
}
