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
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class FullReportGenerator : IXmlReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly ErrorAggregator<ConfigurationException> _configurationErrors;
    private readonly ErrorAggregator<ValidationException> _validationErrors;
    private readonly IOutputFormatter _outputFormatter;
    private string _creationTime;

    public FullReportGenerator (
        InvolvedType[] involvedTypes,
        ErrorAggregator<ConfigurationException> configurationErrors,
        ErrorAggregator<ValidationException> validationErrors,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
      _outputFormatter = outputFormatter;
      _creationTime = string.Empty;
    }

    public XDocument GenerateXmlDocument ()
    {
      var compositeReportGenerator = CreateCompositeReportGenerator();

      var result = compositeReportGenerator.GenerateXml();

      _creationTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      result.Add(new XAttribute("creation-time", _creationTime));

      return new XDocument(result);
    }

    private CompositeReportGenerator CreateCompositeReportGenerator ()
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
      var readOnlyassemblyIdentifierGenerator = assemblyIdentifierGenerator.GetReadonlyIdentiferGenerator("none");
      var readonlyInvolvedTypeIdentiferGenerator =
          new IdentifierPopulator<Type>(_involvedTypes.Select(it => it.Type)).GetReadonlyIdentifierGenerator("none");
      var memberIdentifierGenerator = new IdentifierGenerator<MemberInfo>();
      var interfaceIdentiferGenerator = new IdentifierGenerator<Type>();
      var attributeIdentiferGenerator = new IdentifierGenerator<Type>();

      var involvedReport = new InvolvedTypeReportGenerator(
          _involvedTypes,
          assemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          memberIdentifierGenerator,
          interfaceIdentiferGenerator,
          attributeIdentiferGenerator,
          _outputFormatter);
      var interfaceReport = new InterfaceReportGenerator(
          _involvedTypes,
          assemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          memberIdentifierGenerator,
          interfaceIdentiferGenerator,
          _outputFormatter);
      var attributeReport = new AttributeReportGenerator(
          _involvedTypes,
          assemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          attributeIdentiferGenerator,
          _outputFormatter);
      var assemblyReport = new AssemblyReportGenerator(_involvedTypes, readOnlyassemblyIdentifierGenerator, readonlyInvolvedTypeIdentiferGenerator);

      var configurationErrorReport = new ConfigurationErrorReportGenerator(_configurationErrors);
      var validationErrorReport = new ValidationErrorReportGenerator(_validationErrors);

      return new CompositeReportGenerator(
          involvedReport,
          interfaceReport,
          attributeReport,
          assemblyReport,
          configurationErrorReport,
          validationErrorReport);
    }
  }
}
