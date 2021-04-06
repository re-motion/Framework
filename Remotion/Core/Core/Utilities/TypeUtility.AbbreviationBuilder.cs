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
using System.Text;

namespace Remotion.Utilities
{
  public static partial class TypeUtility
  {
    private class AbbreviationBuilder
    {
      public string BuildAbbreviatedTypeName (Type type, bool includeVersionAndCulture)
      {
        ArgumentUtility.DebugCheckNotNull ("type", type);

        // TODO RM-7763: properties should be checked for null. Consider passing the properties instead of the type/assembly into the next method to use only the null-checked values.
        var typeNameBuilder = new StringBuilder (type.FullName!.Length + 20 + (includeVersionAndCulture ? type.Assembly!.FullName!.Length : 0));
        BuildAbbreviatedTypeName (typeNameBuilder, type, includeVersionAndCulture, false);
        return typeNameBuilder.ToString();
      }

      private void BuildAbbreviatedTypeName (StringBuilder typeNameBuilder, Type type, bool includeVersionAndCulture, bool isTypeParameter)
      {
        string ns = type.Namespace ?? string.Empty;
        string asm = type.Assembly!.GetName().Name!;
        bool canAbbreviate = ns.StartsWith (asm);

        // put type paramters in [brackets] if they include commas, so the commas cannot be confused with type parameter separators
        bool needsBrackets = isTypeParameter && (includeVersionAndCulture || !canAbbreviate);
        if (needsBrackets)
          typeNameBuilder.Append ("[");

        if (canAbbreviate)
        {
          var nsLength = string.IsNullOrEmpty (ns) ? 0 : ns.Length + 1;
          var name = StripTypeParametersFromName (type.FullName!.Substring (nsLength));
          typeNameBuilder.Append (asm).Append ("::");

          if (ns.Length > asm.Length)
            typeNameBuilder.Append (ns.Substring (asm.Length + 1)).Append ('.').Append (name);
          else
            typeNameBuilder.Append (name);

          BuildAbbreviatedTypeParameters (typeNameBuilder, type, includeVersionAndCulture);
        }
        else
        {
          typeNameBuilder.Append (StripTypeParametersFromName (type.FullName!));
          BuildAbbreviatedTypeParameters (typeNameBuilder, type, includeVersionAndCulture);
          typeNameBuilder.Append (", ").Append (asm);
        }

        if (includeVersionAndCulture)
          typeNameBuilder.Append (type.Assembly!.FullName!.Substring (asm.Length));

        if (needsBrackets)
          typeNameBuilder.Append ("]");
      }

      private string StripTypeParametersFromName (string typeName)
      {
        var p = typeName.IndexOf ('[');
        return p < 0 ? typeName : typeName.Substring (0, p);
      }

      private void BuildAbbreviatedTypeParameters (StringBuilder typeNameBuilder, Type type, bool includeVersionAndCulture)
      {
        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
          Type[] typeParams = type.GetGenericArguments();
          if (typeParams.Length > 0)
          {
            typeNameBuilder.Append ("[");
            for (int i = 0; i < typeParams.Length; ++i)
            {
              if (i > 0)
                typeNameBuilder.Append (", ");

              Type typeParam = typeParams[i];
              BuildAbbreviatedTypeName (typeNameBuilder, typeParam, includeVersionAndCulture, true);
            }
            typeNameBuilder.Append ("]");
          }
        }
      }
    }
  }
}
