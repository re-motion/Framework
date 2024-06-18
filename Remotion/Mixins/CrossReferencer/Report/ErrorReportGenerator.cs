﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Reflectors;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class ErrorReportGenerator : IXmlReportGenerator
  {
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflector _reflector;

    public ErrorReportGenerator (
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        IRemotionReflector reflector)
    {
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull ("reflector", reflector);

      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
      _reflector = reflector;
    }


    public XDocument GenerateXmlDocument ()
    {
      var compositeReportGenerator = CreateCompositeReportGenerator();

      var result = compositeReportGenerator.GenerateXml();
      result.Add (new XAttribute ("creation-time", DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss")));

      return new XDocument (result);
    }

    private CompositeReportGenerator CreateCompositeReportGenerator ()
    {
      var configurationErrorReport = new ConfigurationErrorReportGenerator (_configurationErrors);
      var validationErrorReport = new ValidationErrorReportGenerator (_validationErrors, _reflector);

      return new CompositeReportGenerator (configurationErrorReport, validationErrorReport);
    }
  }
}
