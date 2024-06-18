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
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class AdditionalDependencyReportGenerator : IReportGenerator
  {
    private readonly ReflectedObject _explicitDependencies;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;

    public AdditionalDependencyReportGenerator (
        ReflectedObject explicitDependencies,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _explicitDependencies = explicitDependencies;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "AdditionalDependencies",
          from explicitDependencyType in _explicitDependencies
          select new XElement (
              "AdditionalDependency",
              new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (explicitDependencyType.To<Type>())),
              new XAttribute ("instance-name", _outputFormatter.GetShortFormattedTypeName (explicitDependencyType.To<Type>()))
              )
          );
    }
  }
}