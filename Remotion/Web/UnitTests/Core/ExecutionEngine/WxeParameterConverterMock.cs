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
using Remotion.Collections;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{

/// <summary> Exposes non-public members of the <see cref="WxeParameterConverter"/> type. </summary>
public class WxeParameterConverterMock: WxeParameterConverter
{
  public WxeParameterConverterMock (WxeParameterDeclaration parameter)
    : base (parameter)
  {
  }

  public new string ConvertVarRefToString (WxeVariableReference varRef, NameObjectCollection callerVariables)
  {
    return base.ConvertVarRefToString (varRef, callerVariables);
  }

  public new string ConvertObjectToString (object value)
  {
    return base.ConvertObjectToString (value);
  }

  public new void CheckForRequiredOutParameter()
  {
    base.CheckForRequiredOutParameter();
  }

  public new object TryConvertObjectToString (object value)
  {
    return base.TryConvertObjectToString (value);
  }
}

}
