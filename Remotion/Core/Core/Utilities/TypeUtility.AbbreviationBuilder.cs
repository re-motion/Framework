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

        var typeNameBuilder = new StringBuilder (type.FullName.Length + 20 + (includeVersionAndCulture ? type.Assembly.FullName.Length : 0));
        BuildAbbreviatedTypeName (typeNameBuilder, type, includeVersionAndCulture, false);
        return typeNameBuilder.ToString();
      }

      private void BuildAbbreviatedTypeName (StringBuilder typeNameBuilder, Type type, bool includeVersionAndCulture, bool isTypeParameter)
      {
        string ns = type.Namespace ?? string.Empty;
        string asm = type.Assembly.GetName().Name;
        bool canAbbreviate = ns.StartsWith (asm);

        // put type paramters in [brackets] if they include commas, so the commas cannot be confused with type parameter separators
        bool needsBrackets = isTypeParameter && (includeVersionAndCulture || !canAbbreviate);
        if (needsBrackets)
          typeNameBuilder.Append ("[");

        if (canAbbreviate)
        {
          var nsLength = string.IsNullOrEmpty (ns) ? 0 : ns.Length + 1;
          var name = StripTypeParametersFromName (type.FullName.Substring (nsLength));
          typeNameBuilder.Append (asm).Append ("::");

          if (ns.Length > asm.Length)
            typeNameBuilder.Append (ns.Substring (asm.Length + 1)).Append ('.').Append (name);
          else
            typeNameBuilder.Append (name);

          BuildAbbreviatedTypeParameters (typeNameBuilder, type, includeVersionAndCulture);
        }
        else
        {
          typeNameBuilder.Append (StripTypeParametersFromName (type.FullName));
          BuildAbbreviatedTypeParameters (typeNameBuilder, type, includeVersionAndCulture);
          typeNameBuilder.Append (", ").Append (asm);
        }

        if (includeVersionAndCulture)
          typeNameBuilder.Append (type.Assembly.FullName.Substring (asm.Length));

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
