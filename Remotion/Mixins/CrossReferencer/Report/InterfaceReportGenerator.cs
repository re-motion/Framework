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
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class InterfaceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<MemberInfo> _memberIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;

    public InterfaceReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<MemberInfo> memberIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("memberIdentifierGenerator", memberIdentifierGenerator);
      ArgumentUtility.CheckNotNull("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _memberIdentifierGenerator = memberIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _outputFormatter = outputFormatter;
    }


    public XElement GenerateXml ()
    {
      var allInterfaces = GetAllInterfaces();
      var composedInterfaces = GetComposedInterfaces();

      return new XElement(
          "Interfaces",
          from usedInterface in allInterfaces.Keys
          where !CrossReferencerReflectionUtility.IsInfrastructureType(usedInterface)
          select GenerateInterfaceElement(usedInterface, allInterfaces, composedInterfaces.Contains(usedInterface))
      );
    }

    public HashSet<Type> GetComposedInterfaces ()
    {
      var allComposedInterfaces = new HashSet<Type>();

      foreach (var involvedType in _involvedTypes)
      {
        if (!involvedType.IsTarget) continue;

        foreach (var composedInterface in involvedType.ClassContext.ComposedInterfaces)
        {
          allComposedInterfaces.Add(composedInterface);
        }
      }

      return allComposedInterfaces;
    }


    private Dictionary<Type, List<Type>> GetAllInterfaces ()
    {
      var allInterfaces = new Dictionary<Type, List<Type>>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var usedInterface in involvedType.Type.GetInterfaces())
        {
          if (!allInterfaces.ContainsKey(usedInterface))
            allInterfaces.Add(usedInterface, new List<Type>());

          allInterfaces[usedInterface].Add(involvedType.Type);
        }

        if (involvedType.IsTarget)
        {
          foreach (var composedInterface in involvedType.ClassContext.ComposedInterfaces)
          {
            if (!allInterfaces.ContainsKey(composedInterface))
              allInterfaces.Add(composedInterface, new List<Type>());

            allInterfaces[composedInterface].Add(involvedType.Type);
          }
        }
      }

      return allInterfaces;
    }

    private XElement GenerateInterfaceElement (Type usedInterface, Dictionary<Type, List<Type>> allInterfaces, bool isComposedInterface)
    {
      return new XElement(
          "Interface",
          new XAttribute("id", _interfaceIdentifierGenerator.GetIdentifier(usedInterface)),
          new XAttribute("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier(usedInterface.Assembly)),
          new XAttribute("namespace", usedInterface.Namespace ?? ""),
          new XAttribute("name", _outputFormatter.GetShortFormattedTypeName(usedInterface)),
          new XAttribute("is-composed-interface", isComposedInterface),
          new MemberReportGenerator(usedInterface, new InvolvedType(usedInterface), _involvedTypeIdentifierGenerator, _memberIdentifierGenerator, _outputFormatter).GenerateXml(),
          new XElement(
              "ImplementedBy",
              from implementingType in allInterfaces[usedInterface]
              select
                  new XElement(
                      "InvolvedType-Reference",
                      new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(implementingType)))
          )
      );
    }
  }
}
