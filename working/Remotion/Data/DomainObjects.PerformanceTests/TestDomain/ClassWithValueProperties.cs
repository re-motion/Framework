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
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [Serializable]
  public abstract class ClassWithValueProperties : SimpleDomainObject<ClassWithValueProperties>
  {
    public abstract int IntProperty1 { get; set; }
    public abstract int IntProperty2 { get; set; }
    public abstract int IntProperty3 { get; set; }
    public abstract int IntProperty4 { get; set; }
    public abstract int IntProperty5 { get; set; }
    public abstract int IntProperty6 { get; set; }
    public abstract int IntProperty7 { get; set; }
    public abstract int IntProperty8 { get; set; }
    public abstract int IntProperty9 { get; set; }
    public abstract int IntProperty10 { get; set; }

    public abstract DateTime DateTimeProperty1 { get; set; }
    public abstract DateTime DateTimeProperty2 { get; set; }
    public abstract DateTime DateTimeProperty3 { get; set; }
    public abstract DateTime DateTimeProperty4 { get; set; }
    public abstract DateTime DateTimeProperty5 { get; set; }
    public abstract DateTime DateTimeProperty6 { get; set; }
    public abstract DateTime DateTimeProperty7 { get; set; }
    public abstract DateTime DateTimeProperty8 { get; set; }
    public abstract DateTime DateTimeProperty9 { get; set; }
    public abstract DateTime DateTimeProperty10 { get; set; }

    public abstract string StringProperty1 { get; set; }
    public abstract string StringProperty2 { get; set; }
    public abstract string StringProperty3 { get; set; }
    public abstract string StringProperty4 { get; set; }
    public abstract string StringProperty5 { get; set; }
    public abstract string StringProperty6 { get; set; }
    public abstract string StringProperty7 { get; set; }
    public abstract string StringProperty8 { get; set; }
    public abstract string StringProperty9 { get; set; }
    public abstract string StringProperty10 { get; set; }

    public abstract bool BoolProperty1 { get; set; }
    public abstract bool BoolProperty2 { get; set; }
    public abstract bool BoolProperty3 { get; set; }
    public abstract bool BoolProperty4 { get; set; }
    public abstract bool BoolProperty5 { get; set; }
    public abstract bool BoolProperty6 { get; set; }
    public abstract bool BoolProperty7 { get; set; }
    public abstract bool BoolProperty8 { get; set; }
    public abstract bool BoolProperty9 { get; set; }
    public abstract bool BoolProperty10 { get; set; }
  }
}
