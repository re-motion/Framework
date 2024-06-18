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

namespace Remotion.Mixins.CrossReferencer.Reflectors
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class ReflectorSupportAttribute : Attribute
  {
    public Version MinVersion { get; private set; }

    public string Component { get; private set; }

    public string[] RequiredAssemblies { get; private set; }

    public ReflectorSupportAttribute (string component, string minVersion, params string[] requiredAssemblies)
    {
      Component = component;
      RequiredAssemblies = requiredAssemblies;
      MinVersion = new Version(minVersion);
    }
  }
}
