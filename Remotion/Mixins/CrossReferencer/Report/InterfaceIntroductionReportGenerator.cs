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
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class InterfaceIntroductionReportGenerator : IReportGenerator
  {
    // UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition>
    private readonly ReflectedObject _interfaceIntroductionDefinitions;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public InterfaceIntroductionReportGenerator (
        ReflectedObject interfaceIntroductionDefinitions,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("interfaceIntroductionDefinitions", interfaceIntroductionDefinitions);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);

      _interfaceIntroductionDefinitions = interfaceIntroductionDefinitions;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "InterfaceIntroductions",
          from introducedInterface in _interfaceIntroductionDefinitions
          select GenerateInterfaceReferenceElement (introducedInterface.GetProperty ("InterfaceType").To<Type>()));
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
      return new XElement (
          "IntroducedInterface",
          new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (introducedInterface))
          //, GenerateMemberIntroductions
          );
    }
  }
}