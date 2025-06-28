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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Collections;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public class CustomClassEmitter : IClassEmitter
  {
    private static readonly ConstructorInfo s_generatedMethodWrapperAttributeCtor =
        typeof(GeneratedMethodWrapperAttribute).GetConstructor(new[] { typeof(Type), typeof(string), typeof(string) })!;

    public static string FlattenTypeName (string fullName)
    {
      ArgumentNullException.ThrowIfNull(fullName);
      return fullName.Replace("+", "/");
    }

    private readonly AbstractTypeEmitter _innerEmitter;
    private readonly Dictionary<MethodInfo, IMethodEmitter> _publicMethodWrappers = new Dictionary<MethodInfo, IMethodEmitter>();
    private bool _hasBeenBuilt = false;
    private readonly List<CustomEventEmitter> _eventEmitters = new List<CustomEventEmitter>();

    public CustomClassEmitter (AbstractTypeEmitter innerEmitter)
    {
      ArgumentNullException.ThrowIfNull(innerEmitter);
      _innerEmitter = innerEmitter;
    }

    public CustomClassEmitter (ModuleScope scope, string name, Type baseType)
        : this(scope, name, baseType, Type.EmptyTypes, TypeAttributes.Class | TypeAttributes.Public, false)
    {
    }

    public CustomClassEmitter (ModuleScope scope, string name, Type baseType, Type[] interfaces, TypeAttributes flags, bool forceUnsigned)
        : this(
            new ClassEmitterSupportingOpenGenericBaseType(
                scope ?? throw new ArgumentNullException(nameof(scope)),
                ArgumentUtility.CheckNotNullOrEmpty(nameof(name), name),
                CheckBaseType(baseType),
                CheckInterfaces(interfaces),
                flags,
                forceUnsigned))
    {
    }

    private static Type CheckBaseType (Type baseType)
    {
      ArgumentNullException.ThrowIfNull(baseType);
      if (baseType.IsInterface)
        throw new ArgumentException("Base type must not be an interface (" + baseType.GetFullNameSafe() + ").", nameof(baseType));
      if (baseType.IsSealed)
        throw new ArgumentException("Base type must not be sealed (" + baseType.GetFullNameSafe() + ").", nameof(baseType));
      return baseType;
    }

    private static Type[] CheckInterfaces (Type[] interfaces)
    {
      ArgumentNullException.ThrowIfNull(interfaces);
      foreach (Type interfaceType in interfaces)
      {
        if (!interfaceType.IsInterface)
          throw new ArgumentException("Interface type must not be a class or value type (" + interfaceType.GetFullNameSafe() + ").", nameof(interfaces));
      }
      return interfaces;
    }

    internal AbstractTypeEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public Type? BaseType
    {
      get { return TypeBuilder.BaseType; }
    }

    public TypeBuilder TypeBuilder
    {
      get { return InnerEmitter.TypeBuilder; }
    }

    public bool HasBeenBuilt
    {
      get { return _hasBeenBuilt; }
    }

    public ConstructorEmitter CreateConstructor (ArgumentReference[] arguments)
    {
      ArgumentNullException.ThrowIfNull(arguments);

      return InnerEmitter.CreateConstructor(arguments);
    }

    public ConstructorEmitter CreateConstructor (Type[] arguments)
    {
      ArgumentNullException.ThrowIfNull(arguments);

      ArgumentReference[] argumentReferences = ArgumentsUtil.ConvertToArgumentReference(arguments);
      return CreateConstructor(argumentReferences);
    }

    public void CreateDefaultConstructor ()
    {
      InnerEmitter.CreateDefaultConstructor();
    }

    public ConstructorEmitter CreateTypeConstructor ()
    {
      return InnerEmitter.CreateTypeConstructor();
    }

    public FieldReference CreateField (string name, Type fieldType)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(fieldType);

      return InnerEmitter.CreateField(name, fieldType);
    }

    public FieldReference CreateField (string name, Type fieldType, FieldAttributes attributes)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(fieldType);

      return InnerEmitter.CreateField(name, fieldType, attributes);
    }

    public FieldReference CreateStaticField (string name, Type fieldType)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(fieldType);

      return InnerEmitter.CreateStaticField(name, fieldType);
    }

    public FieldReference CreateStaticField (string name, Type fieldType, FieldAttributes attributes)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(fieldType);

      return InnerEmitter.CreateStaticField(name, fieldType, attributes);
    }

    public IMethodEmitter CreateMethod (string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(attributes);
      ArgumentNullException.ThrowIfNull(returnType);
      ArgumentNullException.ThrowIfNull(parameterTypes);

      var method = new CustomMethodEmitter(this, name, attributes, returnType, parameterTypes);
      return method;
    }

    public IMethodEmitter CreateMethod (string name, MethodAttributes attributes, MethodInfo methodToUseAsATemplate)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(attributes);
      ArgumentNullException.ThrowIfNull(methodToUseAsATemplate);

      return new CustomMethodEmitter(this, name, attributes, methodToUseAsATemplate);
    }

    public CustomPropertyEmitter CreateProperty (string name, PropertyKind propertyKind, Type propertyType)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(propertyType);

      return new CustomPropertyEmitter(this, name, propertyKind, propertyType, Type.EmptyTypes, PropertyAttributes.None);
    }

    public CustomPropertyEmitter CreateProperty (
        string name, PropertyKind propertyKind, Type propertyType, Type[] indexParameters, PropertyAttributes attributes)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(propertyType);
      ArgumentNullException.ThrowIfNull(indexParameters);

      return new CustomPropertyEmitter(this, name, propertyKind, propertyType, indexParameters, attributes);
    }

    public CustomEventEmitter CreateEvent (string name, EventKind eventKind, Type eventType, EventAttributes attributes)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(eventType);

      return new CustomEventEmitter(this, name, eventKind, eventType, attributes);
    }

    public CustomEventEmitter CreateEvent (string name, EventKind eventKind, Type eventType)
    {
      ArgumentNullException.ThrowIfNull(name);
      ArgumentNullException.ThrowIfNull(eventType);

      return CreateEvent(name, eventKind, eventType, EventAttributes.None);
    }

    internal void RegisterEventEmitter (CustomEventEmitter emitter)
    {
      _eventEmitters.Add(emitter);
    }

    public IMethodEmitter CreateMethodOverride (MethodInfo baseMethod)
    {
      ArgumentNullException.ThrowIfNull(baseMethod);
      MethodAttributes oldVisibility = baseMethod.Attributes & MethodAttributes.MemberAccessMask;
      return CreateMethodOverrideOrInterfaceImplementation(baseMethod, true, MethodAttributes.ReuseSlot | oldVisibility);
    }

    /// <inheritdoc />
    public IMethodEmitter CreateFullNamedMethodOverride (MethodInfo baseMethod)
    {
      ArgumentNullException.ThrowIfNull(baseMethod);
      MethodAttributes oldVisibility = baseMethod.Attributes & MethodAttributes.MemberAccessMask;
      return CreateMethodOverrideOrInterfaceImplementation(baseMethod, false, MethodAttributes.ReuseSlot | MethodAttributes.Final | oldVisibility);
    }

    public IMethodEmitter CreateInterfaceMethodImplementation (MethodInfo interfaceMethod)
    {
      ArgumentNullException.ThrowIfNull(interfaceMethod);
      return CreateMethodOverrideOrInterfaceImplementation(
          interfaceMethod, false, MethodAttributes.NewSlot | MethodAttributes.Private | MethodAttributes.Final);
    }

    /// <inheritdoc />
    public IMethodEmitter CreatePublicInterfaceMethodImplementation (MethodInfo interfaceMethod)
    {
      ArgumentNullException.ThrowIfNull(interfaceMethod);
      return CreateMethodOverrideOrInterfaceImplementation(interfaceMethod, true, MethodAttributes.NewSlot | MethodAttributes.Public);
    }

    public IMethodEmitter CreateMethodOverrideOrInterfaceImplementation (
        MethodInfo baseOrInterfaceMethod,
        bool keepName,
        MethodAttributes visibilityFlags)
    {
      ArgumentNullException.ThrowIfNull(baseOrInterfaceMethod);

      MethodAttributes methodDefinitionAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | visibilityFlags;
      if (baseOrInterfaceMethod.IsSpecialName)
        methodDefinitionAttributes |= MethodAttributes.SpecialName;

      string methodName = GetMemberOverrideName(baseOrInterfaceMethod, keepName);

      IMethodEmitter methodDefinition = CreateMethod(methodName, methodDefinitionAttributes, baseOrInterfaceMethod);

      TypeBuilder.DefineMethodOverride(methodDefinition.MethodBuilder, baseOrInterfaceMethod);

      return methodDefinition;
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePropertyOverride (PropertyInfo baseProperty)
    {
      ArgumentNullException.ThrowIfNull(baseProperty);
      return CreatePropertyOverrideOrInterfaceImplementation(baseProperty, true);
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreateInterfacePropertyImplementation (PropertyInfo interfaceProperty)
    {
      ArgumentNullException.ThrowIfNull(interfaceProperty);
      return CreatePropertyOverrideOrInterfaceImplementation(interfaceProperty, false);
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePublicInterfacePropertyImplementation (PropertyInfo interfaceProperty)
    {
      ArgumentNullException.ThrowIfNull(interfaceProperty);
      return CreatePropertyOverrideOrInterfaceImplementation(interfaceProperty, true);
    }

    // does not create the property's methods
    private CustomPropertyEmitter CreatePropertyOverrideOrInterfaceImplementation (PropertyInfo baseOrInterfaceProperty, bool keepName)
    {
      ArgumentNullException.ThrowIfNull(baseOrInterfaceProperty);

      string propertyName = GetMemberOverrideName(baseOrInterfaceProperty, keepName);
      Type[] indexParameterTypes = Array.ConvertAll(baseOrInterfaceProperty.GetIndexParameters(), p => p.ParameterType);

      CustomPropertyEmitter newProperty = CreateProperty(
          propertyName,
          PropertyKind.Instance,
          baseOrInterfaceProperty.PropertyType,
          indexParameterTypes,
          PropertyAttributes.None);

      return newProperty;
    }

    // does not create the event's methods
    public CustomEventEmitter CreateEventOverride (EventInfo baseEvent)
    {
      ArgumentNullException.ThrowIfNull(baseEvent);
      return CreateEventOverrideOrInterfaceImplementation(baseEvent, true);
    }

    // does not create the event's methods
    public CustomEventEmitter CreateInterfaceEventImplementation (EventInfo interfaceEvent)
    {
      ArgumentNullException.ThrowIfNull(interfaceEvent);
      return CreateEventOverrideOrInterfaceImplementation(interfaceEvent, false);
    }

    // does not create the event's methods
    public CustomEventEmitter CreatePublicInterfaceEventImplementation (EventInfo interfaceEvent)
    {
      ArgumentNullException.ThrowIfNull(interfaceEvent);
      return CreateEventOverrideOrInterfaceImplementation(interfaceEvent, true);
    }

    /// <inheritdoc />
    public IClassEmitter CreateNestedClass (string typeName, Type? baseType, Type[] interfaces)
    {
      return new CustomClassEmitter(new NestedClassEmitter(InnerEmitter, typeName, baseType, interfaces));
    }

    /// <inheritdoc />
    public IClassEmitter CreateNestedClass (string typeName, Type? baseType, Type[] interfaces, TypeAttributes flags)
    {
      return new CustomClassEmitter(new NestedClassEmitter(InnerEmitter, typeName, flags, baseType, interfaces));
    }

    // does not create the event's methods
    private CustomEventEmitter CreateEventOverrideOrInterfaceImplementation (EventInfo baseOrInterfaceEvent, bool keepName)
    {
      ArgumentNullException.ThrowIfNull(baseOrInterfaceEvent);

      string eventName = GetMemberOverrideName(baseOrInterfaceEvent, keepName);
      CustomEventEmitter newEvent = CreateEvent(eventName, EventKind.Instance, baseOrInterfaceEvent.EventHandlerType!, EventAttributes.None);
      return newEvent;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentNullException.ThrowIfNull(customAttribute);
      TypeBuilder.SetCustomAttribute(customAttribute);
    }

    private static bool IsPublicOrProtected (MethodBase member)
    {
      return member.IsPublic || member.IsFamily || member.IsFamilyOrAssembly;
    }

    public void ReplicateBaseTypeConstructors (Action<ConstructorEmitter> preStatementsAdder, Action<ConstructorEmitter> postStatementsAdder)
    {
      ArgumentNullException.ThrowIfNull(preStatementsAdder);
      ArgumentNullException.ThrowIfNull(postStatementsAdder);

      ConstructorInfo[] constructors = BaseType!.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (ConstructorInfo constructor in constructors)
      {
        if (IsPublicOrProtected(constructor))
          ReplicateBaseTypeConstructor(constructor, preStatementsAdder, postStatementsAdder);
      }
    }

    private void ReplicateBaseTypeConstructor (ConstructorInfo constructor, Action<ConstructorEmitter> preStatementsAdder, Action<ConstructorEmitter> postStatementsAdder)
    {
      ArgumentNullException.ThrowIfNull(constructor);
      ArgumentNullException.ThrowIfNull(preStatementsAdder);
      ArgumentNullException.ThrowIfNull(postStatementsAdder);

      ArgumentReference[] arguments = ArgumentsUtil.ConvertToArgumentReference(constructor.GetParameters());
      ConstructorEmitter newConstructor = InnerEmitter.CreateConstructor(arguments);

      preStatementsAdder(newConstructor);
      Expression[] argumentExpressions = ArgumentsUtil.ConvertArgumentReferenceToExpression(arguments);
      newConstructor.CodeBuilder.AddStatement(new ConstructorInvocationStatement(constructor, argumentExpressions));
      postStatementsAdder(newConstructor);

      newConstructor.CodeBuilder.AddStatement(new ReturnStatement());
    }

    public MethodInfo GetPublicMethodWrapper (MethodInfo methodToBeWrapped)
    {
      ArgumentNullException.ThrowIfNull(methodToBeWrapped);

      // C# compiler 7.2 does not provide caching for delegate but during type generation there is already a significant amount of GC pressure so the delegate creation does not matter
      return _publicMethodWrappers.GetOrCreateValue(methodToBeWrapped, CreatePublicMethodWrapper).MethodBuilder;
    }

    private IMethodEmitter CreatePublicMethodWrapper (MethodInfo methodToBeWrapped)
    {
      const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;

      IMethodEmitter wrapper = CreateMethod("__wrap__" + methodToBeWrapped.Name, attributes, methodToBeWrapped);
      wrapper.ImplementByDelegating(new TypeReferenceWrapper(SelfReference.Self, TypeBuilder), methodToBeWrapped);
      var attributeBuilder = new CustomAttributeBuilder(
          s_generatedMethodWrapperAttributeCtor,
          new object?[] { methodToBeWrapped.DeclaringType, methodToBeWrapped.Name, methodToBeWrapped.ToString() });
      wrapper.AddCustomAttribute(attributeBuilder);

      return wrapper;
    }

    public Type BuildType ()
    {
      _hasBeenBuilt = true;
      foreach (CustomEventEmitter eventEmitter in _eventEmitters)
        eventEmitter.EnsureValid();

      return InnerEmitter.BuildType();
    }

    private string GetMemberOverrideName (MemberInfo baseOrInterfaceMember, bool keepSimpleName)
    {
      if (keepSimpleName)
        return baseOrInterfaceMember.Name;
      else
        return string.Format("{0}.{1}", baseOrInterfaceMember.DeclaringType!.GetFullNameChecked(), baseOrInterfaceMember.Name);
    }
  }
}
