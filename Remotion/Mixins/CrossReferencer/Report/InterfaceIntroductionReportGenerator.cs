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
  public class InterfaceIntroductionReportGenerator : IReportGenerator
  {
    // UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition>
    private readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> _interfaceIntroductionDefinitions;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public InterfaceIntroductionReportGenerator (
        UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> interfaceIntroductionDefinitions,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull("interfaceIntroductionDefinitions", interfaceIntroductionDefinitions);
      ArgumentUtility.CheckNotNull("interfaceIdentifierGenerator", interfaceIdentifierGenerator);

      _interfaceIntroductionDefinitions = interfaceIntroductionDefinitions;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement(
          "InterfaceIntroductions",
          from introducedInterface in _interfaceIntroductionDefinitions
          select GenerateInterfaceReferenceElement(introducedInterface.InterfaceType));
    }

    private XElement GenerateInterfaceReferenceElement (Type introducedInterface)
    {
      /*
      MixinDefinition ab;
      ab.InterfaceIntroductions[0].IntroducedMethods[0].Visibility;
      ab.InterfaceIntroductions[0].IntroducedMethods[0].Name;
      ab.InterfaceIntroductions[0].IntroducedProperties;
      ab.InterfaceIntroductions[0].IntroducedEvents
      */
      return new XElement(
          "IntroducedInterface",
          new XAttribute("ref", _interfaceIdentifierGenerator.GetIdentifier(introducedInterface))
          //, GenerateMemberIntroductions
      );
    }
  }
}
