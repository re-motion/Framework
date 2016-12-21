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
using System.Linq;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Initializes the mixin array held by generated concrete mixed types.
  /// </summary>
  public class MixinArrayInitializer
  {
    private readonly Type _targetType;
    private readonly Type[] _expectedMixinTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixinArrayInitializer"/> class.
    /// </summary>
    /// <param name="targetType">Target type of which the concrete mixed type was generated. This is mainly used for error messages.</param>
    /// <param name="expectedMixinTypes">The expected mixin types. For derived mixins, these contain the concrete mixed types.</param>
    public MixinArrayInitializer (Type targetType, Type[] expectedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("expectedMixinTypes", expectedMixinTypes);

      _targetType = targetType;
      _expectedMixinTypes = expectedMixinTypes;
    }

    public void CheckMixinArray (object[] mixins)
    {
      ArgumentUtility.CheckNotNull ("mixins", mixins);

      if (mixins.Length != _expectedMixinTypes.Length)
        throw CreateInvalidMixinArrayException(mixins);

      for (int i = 0; i < mixins.Length; ++i)
      {
        if (!_expectedMixinTypes[i].IsAssignableFrom (mixins[i].GetType()))
          throw CreateInvalidMixinArrayException (mixins);
      }
    }

    public object[] CreateMixinArray (object[] suppliedMixins)
    {
      var mixins = new object[_expectedMixinTypes.Length];

      FillInSuppliedMixins (mixins, suppliedMixins);

      for (int i = 0; i < mixins.Length; i++)
      {
        if (mixins[i] == null)
          mixins[i] = CreateMixin (_expectedMixinTypes[i]);
      }
      return mixins;
    }

    private void FillInSuppliedMixins (object[] targetArray, object[] suppliedMixins)
    {
      if (suppliedMixins == null)
        return;

      // Note: This has a complexity of O(n*m) where n is the number of suppliedMixins and m is the total number of mixins.
      // We assume that m and especially n are very small.
      foreach (var suppliedMixin in suppliedMixins)
      {
        int index = GetSuppliedMixinIndex (suppliedMixin);
        if (index == -1)
        {
          string message = string.Format (
              "The supplied mixin of type '{0}' is not valid for target type '{1}' in the current configuration.",
              suppliedMixin.GetType (),
              _targetType);
          throw new InvalidOperationException (message);
        }
        else
        {
          if (targetArray[index] != null)
          {
            var message = string.Format (
                "Two mixins were supplied that would match the expected mixin type '{0}' on target class '{1}'.",
                _expectedMixinTypes[index],
                _targetType);
            throw new InvalidOperationException (message);
          }

          targetArray[index] = suppliedMixin;
        }
      }
    }

    private int GetSuppliedMixinIndex (object suppliedMixin)
    {
      var suppliedMixinType = suppliedMixin.GetType ();

      for (int index = 0; index < _expectedMixinTypes.Length; ++index)
      {
        var expectedMixinType = _expectedMixinTypes[index];
        if (expectedMixinType.IsAssignableFrom (suppliedMixinType))
        {
          return index;
        }
        else if (expectedMixinType.BaseType.IsAssignableFrom (suppliedMixinType) && MixinTypeUtility.IsGeneratedByMixinEngine (expectedMixinType))
        {
          var message = string.Format (
              "A mixin was supplied that would match the expected mixin type '{0}' on target class '{1}'. However, a derived type must be "
              + "generated for that mixin type, so the supplied instance cannot be used.",
              expectedMixinType.BaseType,
              _targetType);
          throw new InvalidOperationException (message);
        }
      }

      return -1;
    }

    private object CreateMixin (Type mixinType)
    {
      if (mixinType.IsValueType)
      {
        return Activator.CreateInstance (mixinType); // there's always a public constructor for value types
      }
      else
      {
        try
        {
          return ObjectFactory.Create (mixinType, ParamList.Empty);
        }
        catch (MissingMethodException ex)
        {
          string message = string.Format (
              "Cannot instantiate mixin '{0}' applied to class '{1}', there is no visible default constructor.",
              mixinType,
              _targetType);
          throw new MissingMethodException (message, ex);
        }
      }
    }

    private InvalidOperationException CreateInvalidMixinArrayException (object[] mixins)
    {
      var expectedMixinTypes = string.Join (", ", (IEnumerable<Type>) _expectedMixinTypes);
      var givenMixinTypes = String.Join ((string) ", ", (IEnumerable<string>) mixins.Select (mixin => mixin.GetType ().ToString ()));
      var message = string.Format (
          "Invalid mixin instances supplied. Expected the following mixin types (in this order): ('{0}'). The given types were: ('{1}').",
          expectedMixinTypes,
          givenMixinTypes);
      return new InvalidOperationException (message);
    }
  }
}
