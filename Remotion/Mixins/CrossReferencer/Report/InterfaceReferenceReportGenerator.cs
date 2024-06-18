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
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;
using IRemotionReflector = MixinXRef.Reflection.RemotionReflector.IRemotionReflector;

namespace MixinXRef.Report
{
  public class InterfaceReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;

    public InterfaceReferenceReportGenerator (
        InvolvedType involvedType, IIdentifierGenerator<Type> interfaceIdentifierGenerator, IRemotionReflector remotionReflector)
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _involvedType = involvedType;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _remotionReflector = remotionReflector;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "ImplementedInterfaces",
          from implementedInterface in GetAllInterfaces()
          where !_remotionReflector.IsInfrastructureType (implementedInterface)
          select GenerateInterfaceReference (implementedInterface)
          );
    }

    private XElement GenerateInterfaceReference (Type implementedInterface)
    {
      return new XElement ("ImplementedInterface", new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (implementedInterface)));
    }

    private HashSet<Type> GetAllInterfaces ()
    {
      var allInterfaces = new HashSet<Type>();

      foreach (var iface in _involvedType.Type.GetInterfaces())
        allInterfaces.Add (iface);

      if (_involvedType.IsTarget)
      {
        foreach (var composedInterface in _remotionReflector.GetComposedInterfaces (_involvedType.ClassContext))
          allInterfaces.Add (composedInterface);
      }

      return allInterfaces;
    }
  }
}