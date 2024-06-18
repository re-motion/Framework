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
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class TargetCallDependenciesReportGenerator : IReportGenerator
  {
    private readonly ReflectedObject _mixinDefinition;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;

    public TargetCallDependenciesReportGenerator (ReflectedObject mixinDefinition, IIdentifierGenerator<Assembly> assemblyIdentifierGenerator, IRemotionReflector remotionReflector, IOutputFormatter outputFormatter)
    {
      _mixinDefinition = mixinDefinition;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _remotionReflector = remotionReflector;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      var element = new XElement ("TargetCallDependencies");

      foreach (var targetCallDependencyDefinition in _remotionReflector.GetTargetCallDependencies (_mixinDefinition))
        element.Add (CreateDependencyElement (targetCallDependencyDefinition));

      return element;
    }

    private XElement CreateDependencyElement (ReflectedObject targetCallDependencyDefinition)
    {
      var targetClassDefinition = _mixinDefinition.GetProperty ("TargetClass");
      var requiredType = targetCallDependencyDefinition.GetProperty ("RequiredType").GetProperty ("Type").To<Type> ();
      var element = new XElement("Dependency",
                                 new XAttribute("assembly-ref",
                                                _assemblyIdentifierGenerator.GetIdentifier(requiredType.Assembly)),
                                 new XAttribute("metadataToken", requiredType.MetadataToken),
                                 new XAttribute("namespace", requiredType.Namespace),
                                 new XAttribute("name", _outputFormatter.GetShortFormattedTypeName(requiredType)),
                                 new XAttribute("is-interface", requiredType.IsInterface));
      if (requiredType.IsInterface)
      {
        var implementedByTarget = targetClassDefinition.GetProperty ("ImplementedInterfaces").Any (i => i.To<Type> () == requiredType);
        var addedByMixin = targetClassDefinition.GetProperty ("ReceivedInterfaces").Any (i => i.GetProperty ("InterfaceType").To<Type> () == requiredType);
        var implementedDynamically = !implementedByTarget && !addedByMixin;

        element.Add (new XAttribute ("is-implemented-by-target", implementedByTarget));
        element.Add (new XAttribute ("is-added-by-mixin", addedByMixin));
        element.Add (new XAttribute ("is-implemented-dynamically", implementedDynamically));
      }

      return element;
    }
  }
}