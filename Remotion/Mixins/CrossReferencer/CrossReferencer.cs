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
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Remotion.Mixins.Context;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer
{
  public static class CrossReferencer
  {
    public static Assembly[] GetAssemblies ()
    {
      BootstrapServiceConfiguration.SetLoggerFactory(NullLoggerFactory.Instance);
      var assemblyResolver = AssemblyResolver.Create();
      AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.HandleAssemblyResolve;

      var typeDiscoveryService = ContextAwareTypeUtility.GetTypeDiscoveryService();

      var allTypes = typeDiscoveryService.GetTypes(null, true);
      return allTypes.Cast<Type>().Select(t => t.Assembly)
          .Distinct()
          .Where(a =>
          {
            var nonApplicationAssemblyAttributeType = typeof(NonApplicationAssemblyAttribute);
            var isAssemblyAbleToDefineNonApplicationAssemblyAttribute = a.GetReferencedAssemblies().Contains(nonApplicationAssemblyAttributeType.Assembly.GetName());
            // Note: This is a performance optimization: we only test for the attribute if the assembly has the required references.
            //       This performance optimization is likely overkill since we will load the assemblies anyway.
            return !isAssemblyAbleToDefineNonApplicationAssemblyAttribute || !a.IsDefined(nonApplicationAssemblyAttributeType, false);
          })
          .ToArray();
    }

    public static (XDocument ResultDocument, (Assembly, ReflectionTypeLoadException)[] FailedAssemblies) GetAssemblyInformation (Assembly[] assemblies, bool generateFullReport)
    {
      ArgumentUtility.CheckNotNull("assemblies", assemblies);

      BootstrapServiceConfiguration.SetLoggerFactory(NullLoggerFactory.Instance);

      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(assemblies);
      var configurationErrors = new ErrorAggregator<ConfigurationException>();
      var validationErrors = new ErrorAggregator<ValidationException>();

      var involvedTypeFinder = new InvolvedTypeFinder(mixinConfiguration, assemblies, configurationErrors, validationErrors);
      var involvedTypesResult = involvedTypeFinder.FindInvolvedTypes();

      var reportGenerator = GetReportGenerator(generateFullReport, involvedTypesResult.InvolvedTypes, configurationErrors, validationErrors);
      var outputDocument = reportGenerator.GenerateXmlDocument();

      return (outputDocument, involvedTypesResult.FailedAssemblies);
    }

    private static IXmlReportGenerator GetReportGenerator (
        bool generateFullReport,
        InvolvedType[] involvedTypes,
        ErrorAggregator<ConfigurationException> configurationErrors,
        ErrorAggregator<ValidationException> validationErrors)
    {
      return generateFullReport
          ? new ErrorReportGenerator(configurationErrors, validationErrors)
          : new FullReportGenerator(involvedTypes, configurationErrors, validationErrors, new OutputFormatter());
    }
  }
}
