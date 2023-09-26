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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  public partial class DefaultServiceLocator : IServiceConfigurationRegistry
  {
    private class Registration
    {
      public readonly Func<object>? SingleFactory;
      public readonly Func<object>? CompoundFactory;
      public readonly IReadOnlyCollection<Func<object>> MultipleFactories;

      public Registration (
          Func<object>? singleFactory,
          Func<object>? compoundFactory,
          IReadOnlyCollection<Func<object>> multipleFactories)
      {
        SingleFactory = singleFactory;
        CompoundFactory = compoundFactory;
        MultipleFactories = multipleFactories;
      }
    }

    private static readonly MethodInfo s_resolveIndirectDependencyMethod =
        MemberInfoFromExpressionUtility.GetMethod((DefaultServiceLocator sl) => sl.ResolveIndirectDependency<object>(null))
        .GetGenericMethodDefinition();
    private static readonly MethodInfo s_resolveIndirectCollectionDependencyMethod =
        MemberInfoFromExpressionUtility.GetMethod((DefaultServiceLocator sl) => sl.ResolveIndirectCollectionDependency<object>(null, false))
        .GetGenericMethodDefinition();

    private readonly ConcurrentDictionary<Type, Registration> _dataStore = new ConcurrentDictionary<Type, Registration>();
    private readonly Func<Type, Registration> _createRegistrationFromTypeFunc;

    /// <inheritdoc/>
    public void Register (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      ArgumentUtility.CheckNotNull("serviceConfigurationEntry", serviceConfigurationEntry);

      var registration = CreateRegistration(serviceConfigurationEntry);
      if (!_dataStore.TryAdd(serviceConfigurationEntry.ServiceType, registration))
      {
        var message = string.Format(
            "Register cannot be called twice or after GetInstance for service type: '{0}'.",
            serviceConfigurationEntry.ServiceType);
        throw new InvalidOperationException(message);
      }
    }

    private Registration GetOrCreateRegistrationWithActivationException (Type serviceType)
    {
      try
      {
        return _dataStore.GetOrAdd(serviceType, _createRegistrationFromTypeFunc);
      }
      catch (Exception ex)
      {
        var message = string.Format("Error resolving service Type '{0}': {1}", serviceType, ex.Message);
        throw new ActivationException(message, ex);
      }
    }

    private Registration CreateRegistrationFromType (Type serviceType)
    {
      var serviceConfigurationEntry = _serviceConfigurationDiscoveryService.GetDefaultConfiguration(serviceType);
      return CreateRegistration(serviceConfigurationEntry);
    }

    private Registration CreateRegistration (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      var isCompound = serviceConfigurationEntry.ImplementationInfos.Any(i => i.RegistrationType == RegistrationType.Compound);
      Func<Func<object>, object> noDecorators = f => f();

      var decoratorChain = CreateDecoratorChain(
          serviceConfigurationEntry.ServiceType,
          serviceConfigurationEntry.ImplementationInfos.Where(i => i.RegistrationType == RegistrationType.Decorator));

      var singleFactory = serviceConfigurationEntry.ImplementationInfos
          .Where(i => i.RegistrationType == RegistrationType.Single)
          .Select(i => CreateInstanceFactory(serviceConfigurationEntry.ServiceType, i, decoratorChain))
          .SingleOrDefault(
              () => new InvalidOperationException(
                  string.Format(
                      "Cannot register multiple implementations with registration type '{0}' for service type '{1}'.",
                      RegistrationType.Single,
                      serviceConfigurationEntry.ServiceType)));

      var compoundFactory = serviceConfigurationEntry.ImplementationInfos
          .Where(i => i.RegistrationType == RegistrationType.Compound)
          .Select(i => CreateInstanceFactory(serviceConfigurationEntry.ServiceType, i, decoratorChain))
          .SingleOrDefault(
              () => new InvalidOperationException(
                  string.Format(
                      "Cannot register multiple implementations with registration type '{0}' for service type '{1}'.",
                      RegistrationType.Compound,
                      serviceConfigurationEntry.ServiceType)));

      var multipleFactories = serviceConfigurationEntry.ImplementationInfos
          .Where(i => i.RegistrationType == RegistrationType.Multiple)
          .Select(i => CreateInstanceFactory(serviceConfigurationEntry.ServiceType, i, isCompound ? noDecorators : decoratorChain))
          .ToArray();

      if (singleFactory != null && multipleFactories.Any())
      {
        throw new InvalidOperationException(
            string.Format(
                "Service type '{0}': Registrations of type 'Single' and 'Multiple' are mutually exclusive.",
                serviceConfigurationEntry.ServiceType));
      }

      if (singleFactory != null && compoundFactory != null)
      {
        throw new InvalidOperationException(
            string.Format(
                "Service type '{0}': Registrations of type 'Single' and 'Compound' are mutually exclusive.",
                serviceConfigurationEntry.ServiceType));
      }

      return new Registration(singleFactory, compoundFactory, multipleFactories);
    }

    private Func<Func<object>, object> CreateDecoratorChain (Type serviceType, IEnumerable<ServiceImplementationInfo> decorators)
    {
      Func<Func<object>, object> activator = (innerActivator) => innerActivator();

      //TODO RM-5506: Refactor to simple expression tree without closures etc
      // Note: for singleton-registrations, using a code-generation-free invocation might be benefical for performance.
      // arg => new DecoratorType3 (
      //            new DecoratorType2 (
      //                new DecoratorType1 ((ServiceType) arg)))

      foreach (var decoratorImplementationInfo in decorators)
      {
        var ctorInfo = GetSingleConstructor(decoratorImplementationInfo, serviceType);

        var decoratedObjectArgumentExpression = Expression.Parameter(typeof(object));
        var serviceLocator = Expression.Constant(this);
        var parameterInfos = ctorInfo.GetParameters();
        var ctorArgExpressions = parameterInfos.Select(
            x => x.ParameterType == serviceType
                ? Expression.Convert(decoratedObjectArgumentExpression, serviceType)
                : GetIndirectResolutionCall(serviceLocator, x, false));

        // arg => new DecoratorType (<this.GetInstance(),> (ServiceType) arg <,this.GetInstance()>)
        var activationExpression = Expression.Lambda<Func<object, object>>(
            Expression.New(ctorInfo, ctorArgExpressions),
            decoratedObjectArgumentExpression);
        var compiledExpression = activationExpression.Compile();

        var previousActivator = activator;

        activator = (innerActivator) => compiledExpression(previousActivator(innerActivator));
      }
      return activator;
    }

    private Func<object> CreateInstanceFactory (
        Type serviceType,
        ServiceImplementationInfo serviceImplementationInfo,
        Func<Func<object>, object> decoratorChain)
    {
      Func<object> instanceFactory;
      if (serviceImplementationInfo.Factory == null)
      {
        var isCompound = serviceImplementationInfo.RegistrationType == RegistrationType.Compound;
        var expectedParameterType = isCompound ? typeof(IEnumerable<>).MakeGenericType(serviceType) : null;
        var ctorInfo = GetSingleConstructor(serviceImplementationInfo, expectedParameterType);
        instanceFactory = CreateInstanceFactoryFromConstructorInfo(ctorInfo, isCompound);
      }
      else
      {
        instanceFactory = serviceImplementationInfo.Factory;
      }

      Func<object> decoratedFactory = () => decoratorChain(instanceFactory);

      switch (serviceImplementationInfo.Lifetime)
      {
        case LifetimeKind.Singleton:
          var factoryContainer = new Lazy<object>(decoratedFactory, LazyThreadSafetyMode.ExecutionAndPublication);
          return () => factoryContainer.Value;
        default:
          return decoratedFactory;
      }
    }

    private Func<object> CreateInstanceFactoryFromConstructorInfo (ConstructorInfo ctorInfo, bool isCompoundResolution)
    {
      var serviceLocator = Expression.Constant(this);

      var parameterInfos = ctorInfo.GetParameters();
      var ctorArgExpressions = parameterInfos.Select(x => GetIndirectResolutionCall(serviceLocator, x, isCompoundResolution));

      var factoryLambda = Expression.Lambda<Func<object>>(Expression.New(ctorInfo, ctorArgExpressions));
      return factoryLambda.Compile();
    }

    private ConstructorInfo GetSingleConstructor (ServiceImplementationInfo serviceImplementationInfo, Type? expectedParameterType)
    {
      var argumentTypesDoNotMatchMessage = string.Format(
          " The public constructor must at least accept an argument of type '{0}'.",
          expectedParameterType);

      var exceptionMessage = string.Format(
          "Type '{0}' cannot be instantiated. The type must have exactly one public constructor.{1}",
          serviceImplementationInfo.ImplementationType,
          expectedParameterType == null ? "" : argumentTypesDoNotMatchMessage);

      var constructors = serviceImplementationInfo.ImplementationType.GetConstructors();
      if (constructors.Length != 1)
        throw new InvalidOperationException(exceptionMessage);

      var constructor = constructors.First();
      if (expectedParameterType != null && constructor.GetParameters().Count(p => p.ParameterType == expectedParameterType) != 1)
        throw new InvalidOperationException(exceptionMessage);

      return constructor;
    }

    private Expression GetIndirectResolutionCall (Expression serviceLocator, ParameterInfo parameterInfo, bool isCompoundResolution)
    {
      var context = string.Format("constructor parameter '{0}' of type '{1}'", parameterInfo.Name, parameterInfo.Member.DeclaringType);

      MethodInfo resolutionMethod;
      Expression[] arguments;
      if (parameterInfo.ParameterType.IsGenericType && parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
      {
        var elementType = parameterInfo.ParameterType.GetGenericArguments().Single();
        resolutionMethod = s_resolveIndirectCollectionDependencyMethod.MakeGenericMethod(elementType);
        arguments = new Expression[] { Expression.Constant(context), Expression.Constant(isCompoundResolution) };
      }
      else
      {
        resolutionMethod = s_resolveIndirectDependencyMethod.MakeGenericMethod(parameterInfo.ParameterType);
        arguments = new Expression[] { Expression.Constant(context) };
      }

      return Expression.Call(serviceLocator, resolutionMethod, arguments);
    }

    private T ResolveIndirectDependency<T> (string? context)
    {
      try
      {
        return GetInstance<T>();
      }
      catch (ActivationException ex)
      {
        var message = string.Format("Error resolving indirect dependency of {0}: {1}", context, ex.Message);
        throw new ActivationException(message, ex);
      }
    }

    private IEnumerable<T> ResolveIndirectCollectionDependency<T> (string? context, bool isCompoundResolution)
    {
      IEnumerable<object> enumerable;
      try
      {
        enumerable = GetAllInstances(typeof(T), isCompoundResolution);
      }
      catch (ActivationException ex)
      {
        var message = string.Format("Error resolving indirect collection dependency of {0}: {1}", context, ex.Message);
        throw new ActivationException(message, ex);
      }

      // To keep the lazy sequence semantics of GetAllInstances, and still be able to catch the ActivationException, we need to manually iterate
      // the input sequence from within a try/catch block and yield return from outside the try/catch block. (Yield return is not allowed to stand
      // within a try/catch block.)
      using (var enumerator = enumerable.GetEnumerator())
      {
        while (true)
        {
          T current;
          try
          {
            if (!enumerator.MoveNext())
              yield break;
            current = (T)enumerator.Current;
          }
          catch (ActivationException ex)
          {
            var message = string.Format("Error resolving indirect collection dependency of {0}: {1}", context, ex.Message);
            throw new ActivationException(message, ex);
          }
          yield return current;
        }
      }
    }
  }
}
