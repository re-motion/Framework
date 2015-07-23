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
using Remotion.Mixins;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObject]
  public class TypeWithAllDataTypes
  {
    public static TypeWithAllDataTypes Create ()
    {
      return ObjectFactory.Create<TypeWithAllDataTypes> (true, ParamList.Empty);
    }

    public static TypeWithAllDataTypes Create (string stringValue, int int32Value)
    {
      return ObjectFactory.Create<TypeWithAllDataTypes> (true, ParamList.Create (stringValue, int32Value));
    }

    private bool _boolean;
    private byte _byte;
    private DateTime _date;
    private DateTime _dateTime;
    private decimal _decimal;
    private double _double;
    private TestEnum _enum;
    private Guid _guid;
    private short _int16;
    private int _int32;
    private long _int64;
    private TypeWithString _businessObject;
    private float _single;
    private string _string;

    protected TypeWithAllDataTypes ()
    {
    }

    protected TypeWithAllDataTypes (string @string, int int32)
    {
      _string = @string;
      _int32 = int32;
    }

    public bool Boolean
    {
      get { return _boolean; }
      set { _boolean = value; }
    }

    public byte Byte
    {
      get { return _byte; }
      set { _byte = value; }
    }

    [DateProperty]
    public DateTime Date
    {
      get { return _date; }
      set { _date = value; }
    }

    public DateTime DateTime
    {
      get { return _dateTime; }
      set { _dateTime = value; }
    }

    public decimal Decimal
    {
      get { return _decimal; }
      set { _decimal = value; }
    }

    public double Double
    {
      get { return _double; }
      set { _double = value; }
    }

    public TestEnum Enum
    {
      get { return _enum; }
      set { _enum = value; }
    }

    public Guid Guid
    {
      get { return _guid; }
      set { _guid = value; }
    }

    public short Int16
    {
      get { return _int16; }
      set { _int16 = value; }
    }

    public int Int32
    {
      get { return _int32; }
      set { _int32 = value; }
    }

    public long Int64
    {
      get { return _int64; }
      set { _int64 = value; }
    }

    public TypeWithString BusinessObject
    {
      get { return _businessObject; }
      set { _businessObject = value; }
    }

    public float Single
    {
      get { return _single; }
      set { _single = value; }
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }
  }
}
