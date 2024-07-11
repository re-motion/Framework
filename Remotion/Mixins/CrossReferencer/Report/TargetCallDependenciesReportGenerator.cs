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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class TargetCallDependenciesReportGenerator : IReportGenerator
  {
    private readonly MixinDefinition _mixinDefinition;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;

    public TargetCallDependenciesReportGenerator (
        MixinDefinition mixinDefinition,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IOutputFormatter outputFormatter)
    {
      _mixinDefinition = mixinDefinition;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      var element = new XElement("TargetCallDependencies");

      foreach (var targetCallDependencyDefinition in _mixinDefinition.TargetCallDependencies)
        element.Add(CreateDependencyElement(targetCallDependencyDefinition));

      return element;
    }

    private XElement CreateDependencyElement (TargetCallDependencyDefinition targetCallDependencyDefinition)
    {
      var targetClassDefinition = _mixinDefinition.TargetClass;
      var requiredType = targetCallDependencyDefinition.RequiredType.Type;
      var element = new XElement(
          "Dependency",
          new XAttribute(
              "assembly-ref",
              _assemblyIdentifierGenerator.GetIdentifier(requiredType.Assembly)),
          new XAttribute("metadataToken", requiredType.MetadataToken),
          new XAttribute("namespace", requiredType.Namespace ?? ""),
          new XAttribute("name", _outputFormatter.GetShortFormattedTypeName(requiredType)),
          new XAttribute("is-interface", requiredType.IsInterface));
      if (requiredType.IsInterface)
      {
        var implementedByTarget = targetClassDefinition.ImplementedInterfaces.Any(i => i == requiredType);
        var addedByMixin = targetClassDefinition.ReceivedInterfaces.Any(i => i.InterfaceType == requiredType);
        var implementedDynamically = !implementedByTarget && !addedByMixin;

        element.Add(new XAttribute("is-implemented-by-target", implementedByTarget));
        element.Add(new XAttribute("is-added-by-mixin", addedByMixin));
        element.Add(new XAttribute("is-implemented-dynamically", implementedDynamically));
      }

      return element;
    }
  }
}
