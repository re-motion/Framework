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
using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation.PredefinedTransformations;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Denotes a property or method to be treated like a cast to the member's return type when used in a database LINQ query. To apply the attribute 
  /// to a property, attribute the property's get accessor.
  /// </summary>
  /// <remarks>
  /// If the member is an instance member, that instance is cast to the member's return type. If it is a static member, the method must have exactly
  /// one argument, which is then cast to the member's return type. The latter makes the attribute suitable for use with extension methods (see 
  /// example).
  /// </remarks>
  /// <example>
  /// This attribute is especially useful to provide easy, discoverable access to the members added by a mixin to a class. If the class uses the mixin,
  /// the class itself can provide a property that casts the object to the mixin's interface:
  /// <code>
  /// [DBTable]
  /// [Uses (typeof (MyMixin))] // MyMixin adds persistent properties and exposes them via IMyMixin 
  /// public class MyDomainObject : DomainObject
  /// {
  ///   [StorageClassNone]
  ///   public IMyMixin MixinMembers
  ///   {
  ///     [LinqCastMethod] get { return (IMyMixin) this; }
  ///   }
  /// }
  /// </code>
  /// 
  /// If the class is extended by the mixin, the mixin can add an extension method:
  /// <code>
  /// public static class MyDomainObjectExtensions
  /// {
  ///   [LinqCastMethod]
  ///   public static IMyMixin GetMixinMembers (this MyDomainObject that)
  ///   {
  ///     return (IMyMixin) that;
  ///   }
  /// }
  /// </code>
  /// The <see cref="LinqCastMethodAttribute"/> will ensure that those properties or methods can also be used from within a LINQ database query.
  /// </example>
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class LinqCastMethodAttribute : Attribute, AttributeEvaluatingExpressionTransformer.IMethodCallExpressionTransformerAttribute
  {
    /// <summary>
    /// Implements the transformation that allows the SQL generator to regard a property or method as a cast.
    /// </summary>
    public class MethodCallTransformer : IExpressionTransformer<MethodCallExpression>
    {
      public ExpressionType[] SupportedExpressionTypes
      {
        get { return new[] { ExpressionType.Call }; }
      }

      public Expression Transform (MethodCallExpression methodCallExpression)
      {
        ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

        if (methodCallExpression.Method.IsStatic)
        {
          if (methodCallExpression.Arguments.Count != 1)
          {
            var message = string.Format ("Static LinqCastMethods must have exactly one argument. Expression: '{0}'", methodCallExpression);
            throw new NotSupportedException (message);
          }
          return Expression.Convert (methodCallExpression.Arguments[0], methodCallExpression.Type);
        }
        else
        {
          if (methodCallExpression.Arguments.Count != 0)
          {
            var message = string.Format ("Non-static LinqCastMethods must have no arguments. Expression: '{0}'", methodCallExpression);
            throw new NotSupportedException (message);
          }
          return Expression.Convert (methodCallExpression.Object, methodCallExpression.Type);
        }
      }
    }

    public IExpressionTransformer<MethodCallExpression> GetExpressionTransformer (MethodCallExpression expression)
    {
      return new MethodCallTransformer();
    }
  }
}