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
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Remotion.UnitTests.Xml
{
  [XmlType (ElementName, Namespace = SchemaUri)]
  public class SampleClass
  {
    public const string ElementName = "sampleClass";
    public const string SchemaUri = "http://www.re-motion.org/core/unitTests";
  
    public static XmlReader GetSchemaReader ()
    {
      return new XmlTextReader (Assembly.GetExecutingAssembly ().GetManifestResourceStream (typeof (SampleClass), "SampleClass.xsd"));
    }

    private int _value;

    public SampleClass()
    {
    }

    [XmlElement ("value")]
    public int Value
    {
      get { return _value; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("Value", value, "Only positive integer values are allowed.");
         _value = value;
      }
    }
  }
}
