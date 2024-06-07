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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  public static class MetadataObjectAssert
  {
    public static void AreEqual (EnumValueDefinition expected, EnumValueDefinition actual)
    {
      AreEqual(expected, actual, string.Empty);
    }

    public static void AreEqual (EnumValueDefinition expected, EnumValueDefinition actual, string message)
    {
      Assert.That(actual.MetadataItemID, Is.EqualTo(expected.MetadataItemID), message);
      Assert.That(actual.Name, Is.EqualTo(expected.Name), message);
      Assert.That(actual.Index, Is.EqualTo(expected.Index), message);
      Assert.That(actual.Value, Is.EqualTo(expected.Value), message);
    }

    public static void AreEqual (AccessTypeDefinition expected, AccessTypeDefinition actual)
    {
      AreEqual(expected, actual, string.Empty);
    }

    public static void AreEqual (AccessTypeDefinition expected, AccessTypeDefinition actual, string message)
    {
      AreEqual((EnumValueDefinition)expected, (EnumValueDefinition)actual, message);
    }


    public static void AreEqual (StateDefinition expected, StateDefinition actual)
    {
      AreEqual(expected, actual, string.Empty);
    }

    public static void AreEqual (StateDefinition expected, StateDefinition actual, string message)
    {
      Assert.That(actual.Name, Is.EqualTo(expected.Name), message);
      Assert.That(actual.Index, Is.EqualTo(expected.Index), message);
      Assert.That(actual.Value, Is.EqualTo(expected.Value), message);
    }

    public static void AreEqual (StatePropertyDefinition expected, StatePropertyDefinition actual, string message)
    {
      Assert.That(actual.MetadataItemID, Is.EqualTo(expected.MetadataItemID), message);
      Assert.That(actual.Name, Is.EqualTo(expected.Name), message);
      Assert.That(actual.Index, Is.EqualTo(expected.Index), message);

      Assert.That(actual.DefinedStates.Count, Is.EqualTo(expected.DefinedStates.Count), message);

      for (int i = 0; i < expected.DefinedStates.Count; i++)
        AreEqual(expected.DefinedStates[i], actual.DefinedStates[i], message);
    }

    public static void AreEqual (SecurableClassDefinition expected, ClientTransaction expectedObjectTransaction, SecurableClassDefinition actual)
    {
      AreEqual(expected, expectedObjectTransaction, actual, string.Empty, null);
    }

    public static void AreEqual (SecurableClassDefinition expected, ClientTransaction expectedObjectTransaction, SecurableClassDefinition actual, string message)
    {
      AreEqual(expected, expectedObjectTransaction, actual, message, null);
    }

    public static void AreEqual (
        SecurableClassDefinition expected,
        ClientTransaction expectedObjectTransaction,
        SecurableClassDefinition actual,
        string message,
        params object[] args)
    {
      string expectedName;
      int expectedIndex;
      using (expectedObjectTransaction.EnterNonDiscardingScope())
      {
        expectedName = expected.Name;
        expectedIndex = expected.Index;
      }

      Assert.That(actual.Name, Is.EqualTo(expectedName), () => string.Format(message, args));
      Assert.That(actual.Index, Is.EqualTo(expectedIndex), message);
    }
  }
}
