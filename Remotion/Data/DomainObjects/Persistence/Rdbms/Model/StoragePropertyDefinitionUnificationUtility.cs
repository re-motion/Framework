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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Provides utility functionality for implementing <see cref="IRdbmsStoragePropertyDefinition.UnifyWithEquivalentProperties"/>.
  /// </summary>
  public static class StoragePropertyDefinitionUnificationUtility
  {
    public static T CheckAndConvertEquivalentProperty<T> (
        T expected, object actual, string paramName, params Func<T, Tuple<string, object>>[] checkedPropertyGetters)
    {
      T other;
      try
      {
        other = (T) actual;
      }
      catch (InvalidCastException)
      {
        throw CreateExceptionForNonEquivalentProperties ("type", expected.GetType ().Name, actual.GetType ().Name, paramName);
      }

      foreach (var checkedPropertyGetter in checkedPropertyGetters)
        CheckEqual (checkedPropertyGetter (expected).Item1, checkedPropertyGetter (expected).Item2, checkedPropertyGetter (other).Item2, paramName);

      return other;
    }

    private static void CheckEqual<T> (string checkedItem, T expected, T actual, string paramName)
    {
      if (!Equals (actual, expected))
        throw CreateExceptionForNonEquivalentProperties (checkedItem, expected.ToString (), actual.ToString (), paramName);
    }

    private static ArgumentException CreateExceptionForNonEquivalentProperties (string mismatchingItem, string expected, string actual, string paramName)
    {
      var message =
          String.Format (
              "Only equivalent properties can be combined, but this property has {0} '{1}', and the given property has {0} '{2}'.",
              mismatchingItem,
              expected,
              actual);
      return new ArgumentException (message, paramName);
    }
  }
}