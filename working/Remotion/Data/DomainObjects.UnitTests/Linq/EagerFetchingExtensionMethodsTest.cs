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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class EagerFetchingExtensionMethodsTest : StandardMappingTest
  {
    [Test]
    public void FetchOne ()
    {
      var source = QueryFactory.CreateLinqQuery<Order> ();
      Expression<Func<Order, Customer>> relatedObjectSelector = o => o.Customer;
      
      var expression = (MethodCallExpression) source.FetchOne (relatedObjectSelector).Expression;
      
      Assert.That (expression.Arguments.Count, Is.EqualTo (2));
      Assert.That (expression.Arguments[0], Is.SameAs (source.Expression));
      Assert.That (((UnaryExpression) expression.Arguments[1]).Operand, Is.SameAs (relatedObjectSelector));
      Assert.That (expression.Method,
                   Is.EqualTo (typeof (EagerFetchingExtensionMethods).GetMethod ("FetchOne").MakeGenericMethod (typeof (Order), typeof (Customer))));
    }

    [Test]
    public void FetchMany ()
    {
      var source = QueryFactory.CreateLinqQuery<Order>();
      Expression<Func<Order, IEnumerable<OrderItem>>> relatedObjectSelector = o => o.OrderItems;

      var expression = (MethodCallExpression) source.FetchMany (relatedObjectSelector).Expression;

      Assert.That (expression.Arguments.Count, Is.EqualTo (2));
      Assert.That (expression.Arguments[0], Is.SameAs (source.Expression));
      Assert.That (((UnaryExpression) expression.Arguments[1]).Operand, Is.SameAs (relatedObjectSelector));
      Assert.That (expression.Method,
                   Is.EqualTo (typeof (EagerFetchingExtensionMethods).GetMethod ("FetchMany").MakeGenericMethod (typeof (Order), typeof (OrderItem))));
    }

    [Test]
    public void ThenFetchOne ()
    {
      var source = QueryFactory.CreateLinqQuery<OrderTicket> ().FetchOne (ot => ot.Order);
      Expression<Func<Order, Customer>> relatedObjectSelector = o => o.Customer;

      var expression = (MethodCallExpression) source.ThenFetchOne (relatedObjectSelector).Expression;

      Assert.That (expression.Arguments.Count, Is.EqualTo (2));
      Assert.That (expression.Arguments[0], Is.SameAs (source.Expression));
      Assert.That (((UnaryExpression) expression.Arguments[1]).Operand, Is.SameAs (relatedObjectSelector));
      Assert.That (expression.Method,
                   Is.EqualTo (typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchOne").MakeGenericMethod (typeof (OrderTicket), typeof (Order), typeof (Customer))));
    }

    [Test]
    public void ThenFetchMany ()
    {
      var source = QueryFactory.CreateLinqQuery<OrderTicket> ().FetchOne (ot => ot.Order);
      Expression<Func<Order, IEnumerable<OrderItem>>> relatedObjectSelector = o => o.OrderItems;

      var expression = (MethodCallExpression) source.ThenFetchMany (relatedObjectSelector).Expression;

      Assert.That (expression.Arguments.Count, Is.EqualTo (2));
      Assert.That (expression.Arguments[0], Is.SameAs (source.Expression));
      Assert.That (((UnaryExpression) expression.Arguments[1]).Operand, Is.SameAs (relatedObjectSelector));
      Assert.That (expression.Method,
                   Is.EqualTo (typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchMany").MakeGenericMethod (typeof (OrderTicket), typeof (Order), typeof (OrderItem))));
    }
  }
}
