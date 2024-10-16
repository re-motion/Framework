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
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class AttributeIntroductionReportGenerator : IReportGenerator
  {
    // MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _attributeIntroductionDefinitions
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _attributeIntroductionDefinitions;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeIntroductionReportGenerator (
        MultiDefinitionCollection<Type, AttributeIntroductionDefinition> attributeIntroductionDefinitions,
        IIdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull("attributeIntroductionDefinitions", attributeIntroductionDefinitions);
      ArgumentUtility.CheckNotNull("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _attributeIntroductionDefinitions = attributeIntroductionDefinitions;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement(
          "AttributeIntroductions",
          from introducedAttribute in _attributeIntroductionDefinitions
          where !CrossReferencerReflectionUtility.IsInfrastructureType(introducedAttribute.AttributeType)
          select GenerateAttributeReferanceElement(introducedAttribute.AttributeType));
    }

    private XElement GenerateAttributeReferanceElement (Type introducedAttribute)
    {
      return new XElement(
          "IntroducedAttribute",
          new XAttribute("ref", _attributeIdentifierGenerator.GetIdentifier(introducedAttribute))
      );
    }
  }
}
