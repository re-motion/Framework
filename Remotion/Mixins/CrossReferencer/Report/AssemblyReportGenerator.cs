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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class AssemblyReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public AssemblyReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var assembliesElement = new XElement ("Assemblies");

      var involvedTypes = _involvedTypes.GroupBy(i => i.Type.Assembly);
      foreach (var involvedTypesByAssembly in involvedTypes)
        assembliesElement.Add(GenerateAssemblyElement(involvedTypesByAssembly.Key, involvedTypesByAssembly));

      var additionalAssemblies = _assemblyIdentifierGenerator.Elements.Except(_involvedTypes.Select(t => t.Type.Assembly));
      foreach (var assemblies in additionalAssemblies)
        assembliesElement.Add(GenerateAssemblyElement(assemblies, Enumerable.Empty<InvolvedType>()));

      return assembliesElement;
    }

    private XElement GenerateAssemblyElement (Assembly assembly, IEnumerable<InvolvedType> involvedTypesForAssembly)
    {
      return new XElement (
        "Assembly",
        new XAttribute ("id", _assemblyIdentifierGenerator.GetIdentifier (assembly)),
        new XAttribute ("name", assembly.GetName ().Name),
        new XAttribute ("version", assembly.GetName ().Version),
        new XAttribute ("location", GetShortAssemblyLocation (assembly)),
        new XAttribute ("culture", assembly.GetName ().CultureInfo),
        new XAttribute ("publicKeyToken", Convert.ToBase64String (assembly.GetName ().GetPublicKeyToken ())),
        from involvedType in involvedTypesForAssembly
        select
          new XElement (
          "InvolvedType-Reference",
          new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (involvedType.Type))
          )
        );
    }

    public string GetShortAssemblyLocation (Assembly assembly)
    {
      return assembly.GlobalAssemblyCache ? assembly.Location : "./" + Path.GetFileName (assembly.Location);
    }
  }
}