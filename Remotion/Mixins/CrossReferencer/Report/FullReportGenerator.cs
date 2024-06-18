// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Utility;
using IRemotionReflector = MixinXRef.Reflection.RemotionReflector.IRemotionReflector;

namespace MixinXRef.Report
{
  public class FullReportGenerator : IXmlReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;
    private string _creationTime;

    public FullReportGenerator (
        InvolvedType[] involvedTypes,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        IRemotionReflector remotionReflector,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("_involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
      _remotionReflector = remotionReflector;
      _outputFormatter = outputFormatter;
    }


    public string CreationTime
    {
      get { return _creationTime; }
    }

    public XDocument GenerateXmlDocument ()
    {
      var compositeReportGenerator = CreateCompositeReportGenerator();

      var result = compositeReportGenerator.GenerateXml();

      _creationTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
      result.Add (new XAttribute ("creation-time", _creationTime));

      return new XDocument (result);
    }

    private CompositeReportGenerator CreateCompositeReportGenerator ()
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
      var readOnlyassemblyIdentifierGenerator = assemblyIdentifierGenerator.GetReadonlyIdentiferGenerator ("none");
      var readonlyInvolvedTypeIdentiferGenerator =
          new IdentifierPopulator<Type> (_involvedTypes.Select (it => it.Type)).GetReadonlyIdentifierGenerator ("none");
      var memberIdentifierGenerator = new IdentifierGenerator<MemberInfo> ();
      var interfaceIdentiferGenerator = new IdentifierGenerator<Type>();
      var attributeIdentiferGenerator = new IdentifierGenerator<Type>();

      var involvedReport = new InvolvedTypeReportGenerator (
          _involvedTypes,
          assemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          memberIdentifierGenerator,
          interfaceIdentiferGenerator,
          attributeIdentiferGenerator,
          _remotionReflector,
          _outputFormatter);
      var interfaceReport = new InterfaceReportGenerator (
          _involvedTypes,
          assemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          memberIdentifierGenerator,
          interfaceIdentiferGenerator,
          _remotionReflector,
          _outputFormatter);
      var attributeReport = new AttributeReportGenerator (
          _involvedTypes,
          assemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          attributeIdentiferGenerator,
          _remotionReflector,
          _outputFormatter);
      var assemblyReport = new AssemblyReportGenerator (_involvedTypes, readOnlyassemblyIdentifierGenerator, readonlyInvolvedTypeIdentiferGenerator);

      var configurationErrorReport = new ConfigurationErrorReportGenerator (_configurationErrors);
      var validationErrorReport = new ValidationErrorReportGenerator (_validationErrors, _remotionReflector);

      return new CompositeReportGenerator (
          involvedReport,
          interfaceReport,
          attributeReport,
          assemblyReport,
          configurationErrorReport,
          validationErrorReport);
    }
  }
}