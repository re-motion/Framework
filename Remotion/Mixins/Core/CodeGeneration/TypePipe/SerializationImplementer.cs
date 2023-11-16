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
using System.Runtime.Serialization;
using Remotion.Reflection;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  public class SerializationImplementer
  {
    private static readonly MethodInfo s_getObjectDataMethod =
#pragma warning disable SYSLIB0050
        MemberInfoFromExpressionUtility.GetMethod((ISerializable o) => o.GetObjectData(null!, new StreamingContext()))!;
#pragma warning restore SYSLIB0050

    private static readonly ConstructorInfo s_invalidOperationExceptionConstructor =
        MemberInfoFromExpressionUtility.GetConstructor(() => new InvalidOperationException("message"));

    private static readonly SerializationEventRaiser s_serializationEventRaiser = new SerializationEventRaiser();

    private static bool IsPublicOrProtected (MethodBase method)
    {
      return method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;
    }

    public static void ImplementGetObjectDataByDelegation (
        MutableType mutableType, Func<MethodBodyContextBase, bool, Expression> delegatingExpressionFunc)
    {
      ArgumentUtility.CheckNotNull("mutableType", mutableType);
      ArgumentUtility.CheckNotNull("delegatingExpressionFunc", delegatingExpressionFunc);

      var baseIsISerializable = typeof(ISerializable).IsTypePipeAssignableFrom(mutableType.BaseType);
      var attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final;
      var md = MethodDeclaration.CreateEquivalent(s_getObjectDataMethod);

      mutableType.AddMethod(
          s_getObjectDataMethod.Name,
          attributes,
          md,
          ctx =>
          {
            var baseCall = baseIsISerializable ? ImplementBaseGetObjectDataCall(ctx) : Expression.Empty();
            var delegatingExpression = delegatingExpressionFunc(ctx, baseIsISerializable) ?? Expression.Empty();

            return Expression.Block(baseCall, delegatingExpression);
          });
    }

    private static Expression ImplementBaseGetObjectDataCall (MethodBodyContextBase ctx)
    {
      ConstructorInfo? baseConstructor = ctx.DeclaringType.BaseType!.GetConstructor(
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          CallingConventions.Any,
          new[] { typeof(SerializationInfo), typeof(StreamingContext) },
          null);
      if (baseConstructor == null || !IsPublicOrProtected(baseConstructor))
      {
        string message = string.Format(
            "No public or protected deserialization constructor in type {0} - serialization is not supported.",
            ctx.DeclaringType.BaseType.GetFullNameSafe());
        return Expression.Throw(Expression.New(s_invalidOperationExceptionConstructor, Expression.Constant(message)));
      }

      MethodInfo? baseGetObjectDataMethod =
          ctx.DeclaringType.BaseType.GetMethod("GetObjectData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      if (baseGetObjectDataMethod == null || !IsPublicOrProtected(baseGetObjectDataMethod))
      {
        string message = string.Format("No public or protected GetObjectData in type {0} - serialization is not supported.",
            ctx.DeclaringType.BaseType.GetFullNameSafe());
        return Expression.Throw(Expression.New(s_invalidOperationExceptionConstructor, Expression.Constant(message)));
      }

      return ctx.DelegateToBase(baseGetObjectDataMethod);
    }

    //public static ConstructorEmitter ImplementDeserializationConstructorByThrowing (IClassEmitter classEmitter)
    //{
    //  ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);

    //  ConstructorEmitter emitter = classEmitter.CreateConstructor (new[] { typeof (SerializationInfo), typeof (StreamingContext) });
    //  emitter.CodeBuilder.AddStatement (
    //      new ThrowStatement (
    //          typeof (NotImplementedException),
    //          "The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers."));
    //  return emitter;
    //}

    //public static ConstructorEmitter ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (IClassEmitter classEmitter)
    //{
    //  ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);

    //  var serializationConstructorSignature = new[] { typeof (SerializationInfo), typeof (StreamingContext) };
    //  ConstructorInfo baseConstructor = classEmitter.BaseType.GetConstructor (
    //      BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
    //      null,
    //      serializationConstructorSignature,
    //      null);
    //  if (baseConstructor == null)
    //    return ImplementDeserializationConstructorByThrowing (classEmitter);
    //  else
    //    return null;
    //}

    public static void RaiseOnDeserialization (object deserializedObject, object? sender)
    {
      ArgumentUtility.CheckNotNull("deserializedObject", deserializedObject);
      s_serializationEventRaiser.RaiseDeserializationEvent(deserializedObject, sender);
    }

    public static void RaiseOnDeserializing (object deserializedObject, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull("deserializedObject", deserializedObject);
      s_serializationEventRaiser.InvokeAttributedMethod(deserializedObject, typeof(OnDeserializingAttribute), context);
    }

    public static void RaiseOnDeserialized (object deserializedObject, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull("deserializedObject", deserializedObject);
      s_serializationEventRaiser.InvokeAttributedMethod(deserializedObject, typeof(OnDeserializedAttribute), context);
    }
  }
}
