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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.HierarchyManagement
{
  [TestFixture]
  public class ReadOnlyClientTransactionListenerTest
  {
    private ReadOnlyClientTransactionListener _listener;
    private TestableClientTransaction _clientTransaction;

    private readonly string[] _neverThrowingMethodNames =
        {
            "TransactionInitialize",
            "TransactionDiscard",
            "SubTransactionInitialize",
            "SubTransactionCreated",
            "ObjectsLoading",
            "ObjectsLoaded",
            "ObjectsNotFound",
            "ObjectsUnloading",
            "ObjectsUnloaded",
            "ObjectDeleted",
            "PropertyValueReading",
            "PropertyValueRead",
            "PropertyValueChanged",
            "RelationReading",
            "RelationRead",
            "RelationRead",
            "RelationChanged",
            "FilterQueryResult",
            "FilterCustomQueryResult",
            "TransactionCommitValidate",
            "TransactionCommitted",
            "TransactionRolledBack",
            "RelationEndPointMapRegistering",
            "RelationEndPointMapUnregistering",
            "RelationEndPointBecomingIncomplete",
            "DataContainerMapRegistering",
            "DataContainerMapUnregistering"
        };

    private MethodInfo[] _allMethods;
    private MethodInfo[] _neverThrowingMethods;
    private MethodInfo[] _throwingMethods;

    [SetUp]
    public void SetUp ()
    {
      _listener = new ReadOnlyClientTransactionListener();
      _clientTransaction = new TestableClientTransaction();

      _allMethods = typeof(ReadOnlyClientTransactionListener).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      Assert.That(_allMethods, Has.Length.EqualTo(38));

      _neverThrowingMethods = _allMethods.Where(n => _neverThrowingMethodNames.Contains(n.Name)).ToArray();
      Assert.That(_neverThrowingMethods, Has.Length.EqualTo(_neverThrowingMethodNames.Length));

      _throwingMethods = _allMethods.Where(n => !_neverThrowingMethodNames.Contains(n.Name)).ToArray();
      Assert.That(_throwingMethods, Has.Length.EqualTo(_allMethods.Length - _neverThrowingMethods.Length));
    }

    [Test]
    public void ClientTransactionReadOnly_ThrowingMethods ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_clientTransaction, false);

      foreach (var method in _throwingMethods)
      {
        object[] arguments = Array.ConvertAll(method.GetParameters(), p => GetDefaultValue(p.ParameterType));

        ExpectException(method, arguments);
      }
    }

    [Test]
    public void ClientTransactionReadOnly_NotThrowingMethods ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_clientTransaction, true);

      foreach (var method in _neverThrowingMethods)
      {
        var concreteMethod = GetCallableMethod(method);
        object[] arguments = Array.ConvertAll(concreteMethod.GetParameters(), p => GetDefaultValue(p.ParameterType));

        ExpectNoException(concreteMethod, arguments);
      }
    }

    [Test]
    public void ClientTransactionWriteable_NoMethodThrows ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_clientTransaction, true);

      foreach (var method in _allMethods)
      {
        var concreteMethod = GetCallableMethod(method);
        object[] arguments = Array.ConvertAll(concreteMethod.GetParameters(), p => GetDefaultValue(p.ParameterType));

        ExpectNoException(concreteMethod, arguments);
      }
    }

    private void ExpectException (MethodInfo method, object[] arguments)
    {
      string message = string.Format(
            "The operation cannot be executed because the ClientTransaction is read-only, probably because it has an open subtransaction. "
            + "Offending transaction modification: {0}.",
            method.Name);

      Assert.That(
        () => method.Invoke(_listener, arguments),
        Throws.TargetInvocationException.With.InnerException.TypeOf<ClientTransactionReadOnlyException>().And.InnerException.Message.EqualTo(message),
        $"Expected exception to be thrown by method '{method.Name}'.");
    }

    private void ExpectNoException (MethodInfo method, object[] arguments)
    {
      Assert.That(
        () => method.Invoke(_listener, arguments),
        Throws.Nothing,
        $"Expected no exception to be thrown by method '{method.Name}'.");
    }

    private object GetDefaultValue (Type t)
    {
      if (t.IsValueType)
        return Activator.CreateInstance(t);
      else if (t == typeof(ClientTransaction))
        return _clientTransaction;
      else
        return null;
    }

    private MethodInfo GetCallableMethod (MethodInfo method)
    {
      return method.Name == "FilterQueryResult" || method.Name == "FilterCustomQueryResult"
                 ? method.MakeGenericMethod(typeof(Order))
                 : method;
    }
  }
}
