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
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class TargetReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _mixinType;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public TargetReferenceReportGenerator (InvolvedType mixinType, IIdentifierGenerator<Type> involvedTypeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull("mixinType", mixinType);
      ArgumentUtility.CheckNotNull("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);

      _mixinType = mixinType;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
    }

    public XElement? GenerateXml ()
    {
      if (!_mixinType.IsMixin)
        return null;

      return new XElement(
          "Targets",
          from targetType in _mixinType.TargetTypes.Keys
          select GenerateTargetElement(targetType.Type)
      );
    }

    private XElement GenerateTargetElement (Type targetType)
    {
      return new XElement(
          "Target",
          new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(targetType))
      );
    }
  }
}
