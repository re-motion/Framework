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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Encapsulates the modifications that are applied to a mixin target type.
  /// </summary>
  public class TargetTypeGenerator
  {
    private static readonly ConstructorInfo s_mixinArrayInitializerCtor =
        MemberInfoFromExpressionUtility.GetConstructor(() => new MixinArrayInitializer(null!, Type.EmptyTypes));
    private static readonly MethodInfo s_createMixinArrayMethod =
        MemberInfoFromExpressionUtility.GetMethod((MixinArrayInitializer o) => o.CreateMixinArray(new object[0]));
    private static readonly MethodInfo s_checkMixinArrayMethod =
        MemberInfoFromExpressionUtility.GetMethod((MixinArrayInitializer o) => o.CheckMixinArray(new object[0]));

    private static readonly PropertyInfo s_classContextProperty = MemberInfoFromExpressionUtility.GetProperty((IMixinTarget o) => o.ClassContext);
    private static readonly PropertyInfo s_mixinProperty = MemberInfoFromExpressionUtility.GetProperty((IMixinTarget o) => o.Mixins);
    private static readonly PropertyInfo s_firstNextCallProperty = MemberInfoFromExpressionUtility.GetProperty((IMixinTarget o) => o.FirstNextCallProxy);

    private static readonly MethodInfo s_initializeMixinMethod =
        MemberInfoFromExpressionUtility.GetMethod((IInitializableMixin o) => o.Initialize(null!, null, false));

    private static readonly PropertyInfo s_currentMixedObjectInstantiationScopeProperty =
        MemberInfoFromExpressionUtility.GetProperty(() => MixedObjectInstantiationScope.Current);
    private static readonly PropertyInfo s_suppliedMixinInstancesProperty =
        MemberInfoFromExpressionUtility.GetProperty((MixedObjectInstantiationScope o) => o.SuppliedMixinInstances);

    private readonly MutableType _concreteTarget;
    private readonly IExpressionBuilder _expressionBuilder;
    private readonly IAttributeGenerator _attributeGenerator;
    private readonly INextCallProxyGenerator _nextCallProxyGenerator;

    private INextCallProxy? _nextCallProxy;
    private MutableFieldInfo? _extensionsFieldInfo;
    private MethodInfo? _initializationMethod;

    private Expression? _extensionsField;
    private Expression? _classContextField;
    private Expression? _mixinArrayInitializerField;
    private Expression? _extensionsInitializedField;
    private Expression? _firstField;

    public TargetTypeGenerator (
        MutableType concreteTarget,
        IExpressionBuilder expressionBuilder,
        IAttributeGenerator attributeGenerator,
        INextCallProxyGenerator nextCallProxyGenerator)
    {
      ArgumentUtility.CheckNotNull("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull("expressionBuilder", expressionBuilder);
      ArgumentUtility.CheckNotNull("attributeGenerator", attributeGenerator);
      ArgumentUtility.CheckNotNull("nextCallProxyGenerator", nextCallProxyGenerator);

      _concreteTarget = concreteTarget;
      _expressionBuilder = expressionBuilder;
      _attributeGenerator = attributeGenerator;
      _nextCallProxyGenerator = nextCallProxyGenerator;
    }

    public void AddInterfaces (IEnumerable<Type> interfacesToImplement)
    {
      ArgumentUtility.CheckNotNull("interfacesToImplement", interfacesToImplement);

      foreach (var ifc in interfacesToImplement)
        _concreteTarget.AddInterface(ifc);
    }

    [MemberNotNull(nameof(_extensionsFieldInfo))]
    [MemberNotNull(nameof(_extensionsField))]
    public void AddExtensionsField ()
    {
      _extensionsFieldInfo = _concreteTarget.AddField("__extensions", FieldAttributes.Private, typeof(object[]));
      new AttributeGenerator().AddDebuggerBrowsableAttribute(_extensionsFieldInfo, DebuggerBrowsableState.Never);

      _extensionsField = GetFieldExpression(_extensionsFieldInfo);
    }

    [MemberNotNull(nameof(_nextCallProxy))]
    public void AddNextCallProxy (TargetClassDefinition targetClassDefinition, IList<IMixinInfo> mixinInfos)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull("mixinInfos", mixinInfos);

      Assertion.IsNotNull(_extensionsFieldInfo, "AddExtensionsField must be called first.");

      _nextCallProxy = _nextCallProxyGenerator.Create(_concreteTarget, _extensionsFieldInfo, targetClassDefinition, mixinInfos);
    }

    [MemberNotNull( nameof(_extensionsInitializedField))]
    [MemberNotNull( nameof(_classContextField))]
    [MemberNotNull( nameof(_mixinArrayInitializerField))]
    [MemberNotNull( nameof(_firstField))]
    public void AddFields ()
    {
      Assertion.IsNotNull(_nextCallProxy, "AddNextCallProxy must be called first.");

#pragma warning disable SYSLIB0050
      var notSerialized = FieldAttributes.NotSerialized;
#pragma warning restore SYSLIB0050

      // Not serialized so that initialization is triggered again after deserialization. Default is false. Set immediately _before_ mixins are
      // serialized (to avoid reentrancy).
      _extensionsInitializedField = AddDebuggerInvisibleField(
          "__extensionsInitialized", typeof(bool), FieldAttributes.Private | notSerialized);

      var privateStatic = FieldAttributes.Private | FieldAttributes.Static;
      _classContextField = AddDebuggerInvisibleField("__classContext", typeof(ClassContext), privateStatic);
      _mixinArrayInitializerField = AddDebuggerInvisibleField("__mixinArrayInitializer", typeof(MixinArrayInitializer), privateStatic);
      _firstField = AddDebuggerInvisibleField("__first", _nextCallProxy.Type, FieldAttributes.Private | notSerialized);
    }

    public void AddTypeInitializations (ClassContext classContext, IEnumerable<Type> mixinTypes)
    {
      ArgumentUtility.CheckNotNull("classContext", classContext);
      ArgumentUtility.CheckNotNull("mixinTypes", mixinTypes);
      Assertion.IsNotNull(_classContextField, "AddFields must be called first.");
      Assertion.IsNotNull(_mixinArrayInitializerField, "AddFields must be called first.");

      _concreteTarget.AddTypeInitialization(
          Expression.Block(
              typeof(void),
              InitializeClassContextField(classContext),
              InitializeMixinArrayInitializerField(classContext.Type, mixinTypes)));
    }

    [MemberNotNull(nameof(_initializationMethod))]
    public void AddInitializations (List<Type> mixinTypes)
    {
      ArgumentUtility.CheckNotNull("mixinTypes", mixinTypes);
      Assertion.IsNotNull(_extensionsField, "AddExtensionsField must be called first.");
      Assertion.IsNotNull(_extensionsInitializedField, "AddFields must be called first.");
      Assertion.IsNotNull(_firstField, "AddFields must be called first.");
      Assertion.IsNotNull(_nextCallProxy, "AddNextCallProxy must be called first.");
      Assertion.IsNotNull(_mixinArrayInitializerField, "AddFields must be called first.");
      Assertion.IsNotNull(_extensionsInitializedField, "AddFields must be called first.");

      _initializationMethod = _concreteTarget.AddMethod(
          "__InitializeMixins",
          MethodAttributes.Private,
          parameters: new[] { new ParameterDeclaration(typeof(bool), "isDeserialization") },
          bodyProvider: ctx => ImplementMixinInitalizationMethod(ctx, mixinTypes));

      _concreteTarget.AddInitialization(
          ctx => Expression.Call(
              ctx.This,
              _initializationMethod,
              Expression.Equal(ctx.InitializationSemantics, Expression.Constant(InitializationSemantics.Deserialization))));
    }

    public void ImplementIMixinTarget (string targetClassName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("targetClassName", targetClassName);
      Assertion.IsNotNull(_initializationMethod, "AddInitializations must be called first.");
      Assertion.IsNotNull(_classContextField, "AddFields must be called first.");
      Assertion.IsNotNull(_extensionsField, "AddExtensionsField must be called first.");
      Assertion.IsNotNull(_firstField, "AddFields must be called first.");

      var noInitialization = Expression.Empty();
      var classContextDebuggerDisplay = "Class context for " + targetClassName;
      // Initialize this instance in case we're being called before the ctor has finished running.
      var initialization = _expressionBuilder.CreateInitialization(_concreteTarget, _initializationMethod);

      ImplementReadOnlyProperty(_classContextField, noInitialization, s_classContextProperty, "ClassContext", classContextDebuggerDisplay);
      ImplementReadOnlyProperty(_extensionsField, initialization, s_mixinProperty, "Mixins", "Count = {__extensions.Length}");
      ImplementReadOnlyProperty(_firstField, initialization, s_firstNextCallProperty, "FirstNextCallProxy", "Generated proxy");
    }

    public void ImplementIntroducedInterfaces (IEnumerable<InterfaceIntroductionDefinition> introducedInterfaces)
    {
      ArgumentUtility.CheckNotNull("introducedInterfaces", introducedInterfaces);
      Assertion.IsNotNull(_extensionsField, "AddExtensionsField must be called first.");
      Assertion.IsNotNull(_extensionsInitializedField, "AddFields must be called first.");
      Assertion.IsNotNull(_initializationMethod, "AddInitializations must be called first.");

      foreach (var introduction in introducedInterfaces)
      {
        var implementer = GetIntroducedInterfaceImplementer(introduction);

        foreach (var method in introduction.IntroducedMethods)
          ImplementIntroducedMethod(implementer, method.InterfaceMember, method.ImplementingMember, method.Visibility);
        foreach (var property in introduction.IntroducedProperties)
          ImplementIntroducedProperty(implementer, property);
        foreach (var @event in introduction.IntroducedEvents)
          ImplementIntroducedEvent(implementer, @event);
      }
    }

    public void ImplementRequiredDuckMethods (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);

      foreach (var faceRequirement in targetClassDefinition.RequiredTargetCallTypes)
      {
        if (faceRequirement.Type.IsInterface && !targetClassDefinition.ImplementedInterfaces.Contains(faceRequirement.Type)
            && !targetClassDefinition.ReceivedInterfaces.ContainsKey(faceRequirement.Type))
        {
          foreach (var requiredMethod in faceRequirement.Methods)
          {
            Assertion.IsTrue(
                requiredMethod.ImplementingMethod.DeclaringClass == targetClassDefinition,
                "Duck typing is only supported with members from the base type.");

            ImplementRequiredDuckMethod(requiredMethod);
          }
        }
      }
    }

    public void ImplementAttributes (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);

      ImplementAttributes(_concreteTarget, targetClassDefinition, targetClassDefinition);
    }

    public void AddMixedTypeAttribute (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);

      var classContext = targetClassDefinition.ConfigurationContext;
      var orderedMixinTypes = targetClassDefinition.Mixins.Select(m => m.Type);

      _attributeGenerator.AddConcreteMixedTypeAttribute(_concreteTarget, classContext, orderedMixinTypes);
    }

    public void AddDebuggerDisplayAttribute (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);

      if (!targetClassDefinition.ReceivedAttributes.ContainsKey(typeof(DebuggerDisplayAttribute))
          && !targetClassDefinition.CustomAttributes.ContainsKey(typeof(DebuggerDisplayAttribute)))
      {
        var debuggerDisplayString = "{ToString(),nq} (mixed)";
        _attributeGenerator.AddDebuggerDisplayAttribute(_concreteTarget, debuggerDisplayString, debuggerDisplayNameStringOrNull: null);
      }
    }

    public void ImplementOverrides (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);
      Assertion.IsNotNull(_extensionsField, "AddExtensionsField must be called first.");
      Assertion.IsNotNull(_firstField, "AddFields must be called first.");
      Assertion.IsNotNull(_nextCallProxy, "AddNextCallProxy must be called first.");
      Assertion.IsNotNull(_extensionsInitializedField, "AddFields must be called first.");
      Assertion.IsNotNull(_initializationMethod, "AddInitializations must be called first.");

      foreach (var member in targetClassDefinition.GetAllMembers())
      {
        if (member.Overrides.Count > 0)
        {
          var memberOverride = ImplementOverride(member);
          ImplementAttributes(memberOverride, member, targetClassDefinition);
        }
      }
    }

    public void ImplementOverridingMethods (TargetClassDefinition targetClassDefinition, IList<IMixinInfo> mixinInfos)
    {
      ArgumentUtility.CheckNotNull("mixinInfos", mixinInfos);

      var overriders = targetClassDefinition.GetAllMethods().Where(methodDefinition => methodDefinition.Base != null);
      foreach (var overrider in overriders)
      {
        var mixin = overrider.Base!.DeclaringClass as MixinDefinition;
        Assertion.IsNotNull(mixin, "We only support mixins as overriders of target class members.");
        var mixinInfo = mixinInfos[mixin.MixinIndex];
        var methodInOverrideInterface = mixinInfo.GetOverrideInterfaceMethod(overrider.Base.MethodInfo);

        // It's necessary to explicitly implement some members defined by the concrete mixins' override interfaces: implicit implementation doesn't
        // work if the overrider is non-public or generic. Because it's simpler, we just implement all the members explicitly.
        var methodToCall = overrider.MethodInfo;
        _concreteTarget.AddExplicitOverride(methodInOverrideInterface, ctx => (Expression)ctx.DelegateTo(ctx.This, methodToCall));
      }
    }

    private Expression AddDebuggerInvisibleField (string name, Type type, FieldAttributes attributes)
    {
      var field = _concreteTarget.AddField(name, attributes, type);
      _attributeGenerator.AddDebuggerBrowsableAttribute(field, DebuggerBrowsableState.Never);

      return GetFieldExpression(field);
    }

    private Expression GetFieldExpression (FieldInfo field)
    {
      var instance = field.IsStatic ? null : new ThisExpression(_concreteTarget);
      return Expression.Field(instance, field);
    }

    private BinaryExpression InitializeClassContextField (ClassContext classContext)
    {
      // __classContext = new ClassContext (...);

      return Expression.Assign(_classContextField, _expressionBuilder.CreateNewClassContext(classContext));
    }

    private BinaryExpression InitializeMixinArrayInitializerField (Type targetType, IEnumerable<Type> concreteMixinTypes)
    {
      // __mixinArrayInitializer = new MixinArrayInitializer (targetClassName, mixinTypes);

      return Expression.Assign(
          _mixinArrayInitializerField,
          Expression.New(s_mixinArrayInitializerCtor, Expression.Constant(targetType), Expression.ArrayConstant(concreteMixinTypes)));
    }

    private Expression ImplementMixinInitalizationMethod (MethodBodyCreationContext ctx, List<Type> mixinTypes)
    {
      // if (!__extensionsInitialized) {
      //   __extensionsInitialized = true;
      //   <set first call proxy>;
      //   if (isDeserialization)
      //     <check deserialzed mixin instances>;
      //   else
      //     <create mixin instances>;
      //   <initialize mixins>(isDeserialization);
      // }

      return Expression.IfThen(
          Expression.Not(_extensionsInitializedField),
          Expression.Block(
              Expression.Assign(_extensionsInitializedField, Expression.Constant(true)),
              ImplementSettingFirstNextCallProxy(ctx.This),
              Expression.IfThenElse(
                  ctx.Parameters[0],
                  ImplementCheckingDeserializedMixinInstances(),
                  ImplementCreatingMixinInstances()),
              ImplementInitializingMixins(ctx.This, mixinTypes, ctx.Parameters[0])));
    }

    private Expression ImplementSettingFirstNextCallProxy (ThisExpression @this)
    {
      // __first = <NewNextCallProxy (0)>;

      return Expression.Assign(_firstField, NewNextCallProxy(@this, depth: 0));
    }

    private Expression ImplementCheckingDeserializedMixinInstances ()
    {
      // __mixinArrayInitializer.CheckMixinArray (__extensions);

      return Expression.Call(_mixinArrayInitializerField, s_checkMixinArrayMethod, _extensionsField);
    }

    private Expression ImplementCreatingMixinInstances ()
    {
      // __extensions = __mixinArrayInitializer.CreateMixinArray (MixedObjectInstantiationScope.Current.SuppliedMixinInstances);

      return Expression.Assign(
          _extensionsField,
          Expression.Call(
              _mixinArrayInitializerField,
              s_createMixinArrayMethod,
              Expression.Property(Expression.Property(null, s_currentMixedObjectInstantiationScopeProperty), s_suppliedMixinInstancesProperty)));
    }

    private Expression ImplementInitializingMixins (ThisExpression @this, IList<Type> expectedMixinTypes, Expression isDeserialization)
    {
      var mixinInitExpressions = new List<Expression>();

      for (int i = 0; i < expectedMixinTypes.Count; i++)
      {
        if (typeof(IInitializableMixin).IsTypePipeAssignableFrom(expectedMixinTypes[i]))
        {
          // ((IInitializableMixin) __extensions[i]).Initialize (mixinTargetInstance, <NewNextCallProxy (i + 1)>, isDeserialization);

          var initExpression = Expression.Call(
              Expression.Convert(
                  Expression.ArrayAccess(_extensionsField, Expression.Constant(i)),
                  typeof(IInitializableMixin)),
              s_initializeMixinMethod,
              @this,
              NewNextCallProxy(@this, i + 1),
              isDeserialization);

          mixinInitExpressions.Add(initExpression);
        }
      }

      return Expression.BlockOrEmpty(mixinInitExpressions);
    }

    private Expression NewNextCallProxy (ThisExpression @this, int depth)
    {
      Assertion.DebugIsNotNull(_nextCallProxy, "_nextCallProxy != null");

      return _nextCallProxy.CallConstructor(@this, Expression.Constant(depth));
    }

    private void ImplementReadOnlyProperty (
        Expression backingField,
        Expression initialization,
        PropertyInfo interfaceProperty,
        string debuggerDisplayNameString,
        string debuggerDisplayString)
    {
      var name = MemberImplementationUtility.GetNameForExplicitImplementation(interfaceProperty);
      var getMethod = _concreteTarget.AddExplicitOverride(interfaceProperty.GetGetMethod(), ctx => Expression.Block(initialization, backingField));
      var property = _concreteTarget.AddProperty(name, PropertyAttributes.None, getMethod, setMethod: null);
      _attributeGenerator.AddDebuggerDisplayAttribute(property, debuggerDisplayString, debuggerDisplayNameString);
    }

    private Expression GetIntroducedInterfaceImplementer (InterfaceIntroductionDefinition introduction)
    {
      // ((InterfaceType) __extensions[implementerIndex])

      return Expression.Convert(
          Expression.ArrayAccess(_extensionsField, Expression.Constant(introduction.Implementer.MixinIndex)),
          introduction.InterfaceType);
    }

    private MutableMethodInfo ImplementIntroducedMethod (
        Expression implementer, MethodInfo interfaceMethod, MethodDefinition implementingMethod, MemberVisibility visibility)
    {
      Assertion.DebugIsNotNull(_initializationMethod, "_initializationMethod != null");

      var method = visibility == MemberVisibility.Public
                       ? _concreteTarget.GetOrAddImplementation(interfaceMethod)
                       : _concreteTarget.AddExplicitOverride(interfaceMethod, ctx => Expression.Default(ctx.ReturnType));

      method.SetBody(ctx => _expressionBuilder.CreateInitializingDelegation(ctx, _initializationMethod, implementer, interfaceMethod));

      _attributeGenerator.AddIntroducedMemberAttribute(method, interfaceMethod, implementingMethod);
      _attributeGenerator.ReplicateAttributes(implementingMethod, method);

      return method;
    }

    private void ImplementIntroducedProperty (Expression implementer, PropertyIntroductionDefinition introducedProperty)
    {
      var interfaceProperty = introducedProperty.InterfaceMember;
      var implementingProperty = introducedProperty.ImplementingMember;
      var visibility = introducedProperty.Visibility;

      MutableMethodInfo? getMethod = null, setMethod = null;
      if (introducedProperty.IntroducesGetMethod)
      {
        var implementedGetMethod = Assertion.IsNotNull(
            implementingProperty.GetMethod,
            $"Property {implementingProperty.DeclaringClass.FullName}.{implementingProperty.Name} has no getter");
        var interfaceGetMethod = Assertion.IsNotNull(
            interfaceProperty.GetGetMethod(),
            $"Property {interfaceProperty.DeclaringType!.GetFullNameSafe()}.{interfaceProperty.Name} has no getter");
        getMethod = ImplementIntroducedMethod(implementer, interfaceGetMethod, implementedGetMethod, visibility);
      }
      if (introducedProperty.IntroducesSetMethod)
      {
        var implementedSetMethod = Assertion.IsNotNull(
            implementingProperty.SetMethod,
            $"Property {implementingProperty.DeclaringClass.FullName}.{implementingProperty.Name} has no setter");
        var interfaceSetMethod = Assertion.IsNotNull(
            interfaceProperty.GetSetMethod(),
            $"Property {interfaceProperty.DeclaringType!.GetFullNameSafe()}.{interfaceProperty.Name} has no setter");
        setMethod = ImplementIntroducedMethod(implementer, interfaceSetMethod, implementedSetMethod, visibility);
      }

      var name = GetIntroducedMemberName(visibility, interfaceProperty);
      var property = _concreteTarget.AddProperty(name, PropertyAttributes.None, getMethod, setMethod);

      _attributeGenerator.AddIntroducedMemberAttribute(property, interfaceProperty, implementingProperty);
      _attributeGenerator.ReplicateAttributes(implementingProperty, property);
    }

    private void ImplementIntroducedEvent (Expression implementer, EventIntroductionDefinition introducedEvent)
    {
      var interfaceEvent = introducedEvent.InterfaceMember;
      var implementingEvent = introducedEvent.ImplementingMember;
      var visibility = introducedEvent.Visibility;

      var addMethod = ImplementIntroducedMethod(implementer, interfaceEvent.GetAddMethod()!, implementingEvent.AddMethod, visibility);
      var removeMethod = ImplementIntroducedMethod(implementer, interfaceEvent.GetRemoveMethod()!, implementingEvent.RemoveMethod, visibility);

      var name = GetIntroducedMemberName(visibility, interfaceEvent);
      var @event = _concreteTarget.AddEvent(name, EventAttributes.None, addMethod, removeMethod);

      _attributeGenerator.AddIntroducedMemberAttribute(@event, interfaceEvent, implementingEvent);
      _attributeGenerator.ReplicateAttributes(implementingEvent, @event);
    }

    private string GetIntroducedMemberName (MemberVisibility visibility, MemberInfo interfaceMember)
    {
      return visibility == MemberVisibility.Public
                 ? interfaceMember.Name
                 : MemberImplementationUtility.GetNameForExplicitImplementation(interfaceMember);
    }

    private void ImplementRequiredDuckMethod (RequiredMethodDefinition requiredMethod)
    {
      _concreteTarget.AddExplicitOverride(
          requiredMethod.InterfaceMethod,
          ctx => Expression.Call(ctx.This, requiredMethod.ImplementingMethod.MethodInfo, ctx.Parameters.Cast<Expression>()));
    }

    private void ImplementAttributes (
        IMutableMember member, IAttributeIntroductionTarget targetConfiguration, TargetClassDefinition targetClassDefinition)
    {
      foreach (var attribute in targetConfiguration.CustomAttributes)
      {
        if (_attributeGenerator.ShouldBeReplicated(attribute, targetConfiguration, targetClassDefinition))
          _attributeGenerator.AddAttribute(member, attribute.Data);
      }

      foreach (var introducedAttribute in targetConfiguration.ReceivedAttributes)
        _attributeGenerator.AddAttribute(member, introducedAttribute.Attribute.Data);
    }

    private IMutableMember ImplementOverride (MemberDefinitionBase member)
    {
      var memberAsMethodDefinition = member as MethodDefinition;
      if (memberAsMethodDefinition != null)
        return ImplementMethodOverride(memberAsMethodDefinition);

      var memberAsPropertyDefinition = member as PropertyDefinition;
      if (memberAsPropertyDefinition != null)
        return ImplementPropertyOverride(memberAsPropertyDefinition);

      var memberAsEventDefinition = member as EventDefinition;
      Assertion.IsNotNull(memberAsEventDefinition, "Only methods, properties, and events can be overridden.");
      return ImplementEventOverride(memberAsEventDefinition);
    }

    private MutableMethodInfo ImplementMethodOverride (MethodDefinition method)
    {
      Assertion.DebugIsNotNull(_nextCallProxy, "_nextCallProxy != null");
      Assertion.DebugIsNotNull(_initializationMethod, "_initializationMethod != null");
      Assertion.DebugIsNotNull(_firstField, "_firstField != null");

      var proxyMethod = _nextCallProxy.GetProxyMethodForOverriddenMethod(method);
      var methodOverride = _concreteTarget.GetOrAddOverride(method.MethodInfo);
      methodOverride.SetBody(ctx => _expressionBuilder.CreateInitializingDelegation(ctx, _initializationMethod, _firstField, proxyMethod));

      return methodOverride;
    }

    private IMutableMember ImplementPropertyOverride (PropertyDefinition property)
    {
      MutableMethodInfo? getMethodOverride = null, setMethodOverride = null;
      if (property.GetMethod != null && property.GetMethod.Overrides.Count > 0)
        getMethodOverride = ImplementMethodOverride(property.GetMethod);
      if (property.SetMethod != null && property.SetMethod.Overrides.Count > 0)
        setMethodOverride = ImplementMethodOverride(property.SetMethod);

      return _concreteTarget.AddProperty(property.Name, PropertyAttributes.None, getMethodOverride, setMethodOverride);
    }

    private IMutableMember ImplementEventOverride (EventDefinition @event)
    {
      MutableMethodInfo? addMethodOverride = null, removeMethodOverride = null;
      if (@event.AddMethod.Overrides.Count > 0)
        addMethodOverride = ImplementMethodOverride(@event.AddMethod);
      if (@event.RemoveMethod.Overrides.Count > 0)
        removeMethodOverride = ImplementMethodOverride(@event.RemoveMethod);

      return _concreteTarget.AddEvent(@event.Name, EventAttributes.None, addMethodOverride, removeMethodOverride);
    }
  }
}
