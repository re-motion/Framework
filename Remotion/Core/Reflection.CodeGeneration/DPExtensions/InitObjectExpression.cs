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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class InitObjectExpression : Expression
  {
    private readonly Reference _objectToBeInitialized;
    private readonly Type _type;

    public InitObjectExpression (Reference objectToBeInitialized, Type type)
    {
      ArgumentUtility.CheckNotNull ("objectToBeInitialized", objectToBeInitialized);
      ArgumentUtility.CheckNotNull ("type", type);

      _objectToBeInitialized = objectToBeInitialized;
      _type = type;
    }

    public InitObjectExpression (IMethodEmitter method, Type type)
        : this (ArgumentUtility.CheckNotNull ("method", method).DeclareLocal (type), type)
    {
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _objectToBeInitialized.LoadAddressOfReference (gen);
      gen.Emit (OpCodes.Initobj, _type);
      _objectToBeInitialized.LoadReference (gen);
    }
  }
}
