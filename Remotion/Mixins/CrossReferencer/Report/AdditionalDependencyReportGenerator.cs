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
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class AdditionalDependencyReportGenerator : IReportGenerator
  {
    private readonly IReadOnlyCollection<Type> _explicitDependencies;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;

    public AdditionalDependencyReportGenerator (
        IReadOnlyCollection<Type> explicitDependencies,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull("explicitDependencies", explicitDependencies);
      ArgumentUtility.CheckNotNull("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("outputFormatter", outputFormatter);

      _explicitDependencies = explicitDependencies;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      return new XElement(
          "AdditionalDependencies",
          from explicitDependencyType in _explicitDependencies
          select new XElement(
              "AdditionalDependency",
              new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(explicitDependencyType)),
              new XAttribute("instance-name", _outputFormatter.GetShortFormattedTypeName(explicitDependencyType))
          )
      );
    }
  }
}
