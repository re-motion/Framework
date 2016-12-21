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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class CustomAttributeExpression : Expression
  {
    private static readonly MethodInfo s_getCustomAttributesMethod =
        typeof (ICustomAttributeProvider).GetMethod ("GetCustomAttributes", new Type[] { typeof (Type), typeof (bool) }, null);

    private readonly TypeReference _attributeOwner;
    private readonly Type _attributeType;
    private readonly int _index;
    private readonly bool _inherited;
    private readonly Expression _getAttributeExpression;

    public CustomAttributeExpression (TypeReference attributeOwner, Type attributeType, int index, bool inherited)
    {
      ArgumentUtility.CheckNotNull ("attributeOwner", attributeOwner);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      ArgumentUtility.CheckTypeIsAssignableFrom ("attributeOwner", attributeOwner.Type, typeof (ICustomAttributeProvider));

      _attributeOwner = attributeOwner;
      _attributeType = attributeType;
      _index = index;
      _inherited = inherited;

      Expression getAttributesExpression = new ConvertExpression (
          _attributeType.MakeArrayType (),
          new VirtualMethodInvocationExpression (
              _attributeOwner,
              s_getCustomAttributesMethod,
              new TypeTokenExpression (_attributeType),
              new ConstReference (_inherited).ToExpression ()));
      _getAttributeExpression =
          new LoadCalculatedArrayElementExpression (getAttributesExpression, new ConstReference (_index).ToExpression (), _attributeType);
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _getAttributeExpression.Emit (member, gen);
    }
  }
}
