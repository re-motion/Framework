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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Resources;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  public class PropertyValueChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PropertyValueChecker ()
    {
    }

    // methods and properties

    public void Check (PropertyDefinition propertyDefinition, DataContainer containerWithExpectedValue, DataContainer actualContainer)
    {
      var expectedValue = containerWithExpectedValue.GetValue(propertyDefinition);
      var actualValue = actualContainer.GetValue(propertyDefinition);
      AreValuesEqual(
          expectedValue,
          actualValue,
          string.Format("Value, expected property name: '{0}'", propertyDefinition.PropertyName));

      if (expectedValue != null)
      {
        Assert.That(
            actualValue.GetType(),
            Is.EqualTo(expectedValue.GetType()),
            string.Format("Type of Value, expected property name: '{0}'", propertyDefinition.PropertyName));
      }

      var expectedOriginalValue = containerWithExpectedValue.GetValue(propertyDefinition, ValueAccess.Original);
      var actualOriginalValue = actualContainer.GetValue(propertyDefinition, ValueAccess.Original);
      AreValuesEqual(expectedOriginalValue, actualOriginalValue, string.Format("OriginalValue, expected property name: '{0}'", propertyDefinition.PropertyName));

      if (expectedOriginalValue != null)
      {
        Assert.That(
            actualOriginalValue.GetType(),
            Is.EqualTo(expectedOriginalValue.GetType()),
            string.Format("Type of OriginalValue, expected property name: '{0}'", propertyDefinition.PropertyName));
      }

      Assert.That(
          actualContainer.HasValueChanged(propertyDefinition),
          Is.EqualTo(containerWithExpectedValue.HasValueChanged(propertyDefinition)),
          string.Format("HasChanged, expected property name: '{0}'", propertyDefinition.PropertyName));

      Assert.That(
          actualContainer.HasValueBeenTouched(propertyDefinition),
          Is.EqualTo(containerWithExpectedValue.HasValueBeenTouched(propertyDefinition)),
          string.Format("HasBeenTouched, expected property name: '{0}'", propertyDefinition.PropertyName));
    }

    private void AreValuesEqual (object expected, object actual, string message)
    {
      if (expected == actual)
        return;

      if (expected == null || expected.GetType() != typeof(byte[]))
        Assert.That(actual, Is.EqualTo(expected), message);
      else
        ResourceManager.AreEqual((byte[])expected, (byte[])actual, message);
    }
  }
}
