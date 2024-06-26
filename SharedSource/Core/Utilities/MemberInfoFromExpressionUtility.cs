// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Linq.Expressions;
using System.Reflection;
#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>
  /// Provides typed access to the reflection objects for members referenced in <see cref="Expression"/> instances.
  /// </summary>
  /// <remarks>
  /// Note that this utility just extracts the <see cref="MemberInfo"/> instance referenced in the expression.
  /// For methods and properties the returned object might be different from the one specified by the user.
  /// This is because the C# compiler always puts the base definition of overriding methods and properties into the expression.
  /// </remarks>
  static partial class MemberInfoFromExpressionUtility
  {
    public static MemberInfo GetMember<TMemberType> (Expression<Func<TMemberType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetMemberInfoFromExpression(expression.Body);
    }

    public static MemberInfo GetMember<TSourceObject, TMemberType> (Expression<Func<TSourceObject, TMemberType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetMemberInfoFromExpression(expression.Body);
    }

    public static FieldInfo GetField<TFieldType> (Expression<Func<TFieldType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetFieldInfoFromMemberExpression(expression.Body);
    }

    public static FieldInfo GetField<TSourceObject, TFieldType> (Expression<Func<TSourceObject, TFieldType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetFieldInfoFromMemberExpression(expression.Body);
    }

    public static ConstructorInfo GetConstructor<TType> (Expression<Func<TType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetConstructorInfoFromNewExpression(expression.Body);
    }

    public static MethodInfo GetMethod (Expression<Action> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetMethodInfoFromMethodCallExpression(expression.Body);
    }

    public static MethodInfo GetMethod<TReturnType> (Expression<Func<TReturnType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetMethodInfoFromMethodCallExpression(expression.Body);
    }

    public static MethodInfo GetMethod<TSourceObject> (Expression<Action<TSourceObject>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetMethodInfoFromMethodCallExpression(expression.Body);
    }

    public static MethodInfo GetMethod<TSourceObject, TReturnType> (Expression<Func<TSourceObject, TReturnType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetMethodInfoFromMethodCallExpression(expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition (Expression<Action> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetGenericMethodDefinition(expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition<TReturnType> (Expression<Func<TReturnType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetGenericMethodDefinition(expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition<TSourceObject> (Expression<Action<TSourceObject>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetGenericMethodDefinition(expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition<TSourceObject, TReturnType> (Expression<Func<TSourceObject, TReturnType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetGenericMethodDefinition(expression.Body);
    }

    public static PropertyInfo GetProperty<TPropertyType> (Expression<Func<TPropertyType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetPropertyInfoFromMemberExpression(expression.Body);
    }

    public static PropertyInfo GetProperty<TSourceObject, TPropertyType> (Expression<Func<TSourceObject, TPropertyType>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      return GetPropertyInfoFromMemberExpression(expression.Body);
    }

    private static MemberInfo GetMemberInfoFromExpression (Expression expression)
    {
      if (expression is MemberExpression)
        if (((MemberExpression)expression).Member is PropertyInfo)
          return GetPropertyInfoFromMemberExpression(expression);
        else
          return GetFieldInfoFromMemberExpression(expression);

      if (expression is MethodCallExpression)
        return GetMethodInfoFromMethodCallExpression(expression);
      if (expression is NewExpression)
        return GetConstructorInfoFromNewExpression(expression);

      throw new ArgumentException("Must be a MemberExpression, MethodCallExpression or NewExpression.", "expression");
    }

    private static T GetTypedMemberInfoFromMemberExpression<T> (Expression expression, string memberType)
        where T : MemberInfo
    {
      var memberExpression = expression as MemberExpression;
      if (memberExpression == null)
        throw new ArgumentException("Must be a MemberExpression.", "expression");

      var member = memberExpression.Member as T;
      if (member == null)
      {
        var message = string.Format("Must hold a {0} access expression.", memberType);
        throw new ArgumentException(message, "expression");
      }

      return member;
    }

    private static FieldInfo GetFieldInfoFromMemberExpression (Expression expression)
    {
      return GetTypedMemberInfoFromMemberExpression<FieldInfo>(expression, "field");
    }

    private static PropertyInfo GetPropertyInfoFromMemberExpression (Expression expression)
    {
      // For redeclared properties (overridden in C#) the MemberExpression contains the root definition.
      return GetTypedMemberInfoFromMemberExpression<PropertyInfo>(expression, "property");
    }

    private static ConstructorInfo GetConstructorInfoFromNewExpression (Expression expression)
    {
      var newExpression = expression as NewExpression;
      if (newExpression == null)
        throw new ArgumentException("Must be a NewExpression.", "expression");

      // TODO RM-7773: newExpression.Constructor possibly being null must be handled.
      return newExpression.Constructor!;
    }

    private static MethodInfo GetMethodInfoFromMethodCallExpression (Expression expression)
    {
      var methodCallExpression = expression as MethodCallExpression;
      if (methodCallExpression == null)
        throw new ArgumentException("Must be a MethodCallExpression.", "expression");

      // For virtual methods the MethodCallExpression containts the root definition.
      return methodCallExpression.Method;
    }

    private static MethodInfo GetGenericMethodDefinition (Expression expression)
    {
      var methodInfo = GetMethodInfoFromMethodCallExpression(expression);
      if (!methodInfo.IsGenericMethod)
        throw new ArgumentException("Must hold a generic method access expression.", "expression");

      return methodInfo.GetGenericMethodDefinition();
    }
  }
}
