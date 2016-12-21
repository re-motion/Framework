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
using Remotion.ExtensibleEnums;
using Remotion.Globalization;

public class TestEnum : ExtensibleEnum<TestEnum>
{
  public TestEnum (string declarationSpace, string valueName)
      : base (declarationSpace, valueName)
  {
  }
}

public static class TestEnumExtensions
{
  [MultiLingualName ("The Name en", "en")]
  [MultiLingualName ("The Name en-US", "en-US")]
  public static TestEnum ValueWithEnAndEnUS (ExtensibleEnumDefinition<TestEnum> definition)
  {
    throw new NotImplementedException();
  }

  [MultiLingualName ("The Name invariant", "")]
  [MultiLingualName ("The Name en", "en")]
  public static TestEnum ValueWithInvariantAndEn (ExtensibleEnumDefinition<TestEnum> definition)
  {
    throw new NotImplementedException();
  }
}
