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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

namespace Remotion.Development.CodeDom
{
/// <summary>
/// Base class for extended code providers.
/// </summary>
/// <remarks>
/// <para>
///   Extended code providers extend the functionality of .NET CodeDOM code providers. In order to generate
///   code for a specific language, a subclass of ExtendedCodeProvider must be implemented that extends the
///   functionality of CodeDOM code providers.
/// <para></para>
///   <b>Note to inheritors:</b>
/// <para></para>
///   Inheritors must at least call the base class constructor with a valid <c>CodeDomProvider</c> and 
///   implement <see cref="IsCaseSensitive"/> and <see cref="CreateUnaryOperatorExpression"/>.
/// <para></para>
///   If the specific language supports custom casting operators, override <see cref="SupportsCastingOperators"/> 
///   and <see cref="CreateCastingOperator"/>. 
/// <para></para>
///   If the specific language supports operator overriding, override <see cref="SupportsOperatorOverriding"/> 
///   and <see cref="CreateBinaryOperator"/>. 
/// </para>
/// </remarks>
public abstract class ExtendedCodeProvider
{
  /// <summary>
  /// This class is used to create a mapping table for <c>AppendMemberAttributeString</c>.
  /// <seealso cref="AppendMemberAttributeString"/>
  /// </summary>
  protected class MemberAttributeKeywordMapping
  {
    public MemberAttributeKeywordMapping (MemberAttributes attribute, MemberAttributes mask, string keyword)
    {
      Attribute = attribute;
      Mask = mask;
      Keyword = keyword;
    }
    public readonly MemberAttributes Attribute;
    public readonly MemberAttributes Mask;
    public readonly string Keyword;

    public bool IsSet (MemberAttributes concreteAttributes)
    {
      if (Mask != (MemberAttributes) 0)
        return (concreteAttributes & Mask) == Attribute;
      else
        return (concreteAttributes & Attribute) == Attribute;
    }
  }

  // fields

  public readonly CodeDomProvider Provider;
  public readonly ICodeGenerator Generator;

  // construction and disposal

  /// <summary>
  /// Creates an <c>ExtendedCodeProvider</c> using a <c>CodeDomProvider</c> and a matching generator.
  /// </summary>
	public ExtendedCodeProvider (CodeDomProvider provider, ICodeGenerator generator)
	{
    if (provider == null) throw new ArgumentNullException ("provider");
    if (generator == null) throw new ArgumentNullException ("generator");
    Provider = provider;
    Generator = generator;
	}

  /// <summary>
  /// Creates an <c>ExtendedCodeProvider</c> using a <c>CodeDomProvider</c>.
  /// </summary>
	public ExtendedCodeProvider (CodeDomProvider provider)
	{
    if (provider == null) throw new ArgumentNullException ("provider");
    Provider = provider;
    Generator = provider.CreateGenerator ();
	}

  // properties and methods

  /// <summary>
  /// Specifies whether the current provider supports casting operators.
  /// </summary>
  /// <value>The default implementation always returns <see langword="false" />.</value>
  public virtual bool SupportsCastingOperators
  {
    get { return false; }
  }

  /// <summary>
  /// For derived classes that support casting operators, this method creates implicit or explicit casting operators.
  /// </summary>
  /// <param name="fromType">Type that the method casts from.</param>
  /// <param name="toType">Type that the method casts to.</param>
  /// <param name="argumentName">The name of the cast operator's argument.</param>
  /// <param name="statements">Statements that perform the conversion, ending with a <c>CodeMethodReturnStatement</c>.</param>
  /// <param name="attributes">Method attributes that define access and scope. Must be <see langword="static"/>.</param>
  /// <param name="castOperatorKind"><c>Implicit</c> to create an implicit casting operator, <c>Explicit</c> otherwise.</param>
  /// <returns>A <c>CodeTypeMember</c> object that can be appended to a CodeDOM type object.</returns>
  /// <exception cref="NotSupportedException">The default implementation always throws this exception.</exception>
  public virtual CodeTypeMember CreateCastingOperator (
      string fromType, string toType, string argumentName, CodeStatementCollection statements, 
      MemberAttributes attributes, CodeCastOperatorKind castOperatorKind)
  {
    throw new NotSupportedException (this.GetType().FullName + " does not support casting operators.");
  }

  public virtual bool SupportsOperatorOverriding
  {
    get { return false; }
  }

  public virtual CodeTypeMember CreateBinaryOperator (
      string argumentTypeName, string firstArgumentName, string secondArgumentName, 
      CodeOverridableOperatorType operatorType, string returnTypeName,
      CodeStatementCollection statements, MemberAttributes attributes)
  {
    throw new NotSupportedException (this.GetType().FullName + " does not support operator overriding.");
  }

  /// <summary>
  /// Specifies whether the provider supports documentation comments.
  /// </summary>
  /// <value>The default implementation always returns false.</value>
  public virtual bool SupportsDocumentationComments
  {
    get { return false; }
  }

  /// <summary>
  /// Adds a documentation comment if the provider supports it, a normal comment otherwise.
  /// </summary>
  /// <param name="item">The source code item the comment is to be attached to.</param>
  /// <param name="elementName">The documentation comments element name.</param>
  /// <param name="elementArguments">Optional. A string containing the XML attributes for this element.</param>
  /// <param name="alternativeHeadline">Optional. A string that is written as a headline if the provider does not support documentation comments.</param>
  /// <param name="description">The text of the comment.</param>
  public virtual void AddDocumentationComment (
      CodeTypeMember item, string elementName, string elementArguments, string alternativeHeadline, string description)
  {
    if (description == null)
      description = string.Empty;

    StringBuilder sb = new StringBuilder();
    if (SupportsDocumentationComments)
    {
      sb.Append ("<");
      sb.Append (elementName);
      if (elementArguments != null && elementArguments.Length > 0)
      {
        sb.Append (" ");
        sb.Append (elementArguments);
      }
      sb.Append (">");
      sb.Append (description.Replace("&", "&amp;").Replace ("<", "&lt;").Replace(">", "&gt;"));
      sb.Append ("</");
      sb.Append (elementName);
      sb.Append (">");
      item.Comments.Add (new CodeCommentStatement (sb.ToString(), true));
    }
    else
    {
      if (alternativeHeadline != null && alternativeHeadline.Length > 0)
      {
        sb.Append (alternativeHeadline);
        sb.Append (":\n");                                        
      }
      sb.Append (description);
      item.Comments.Add (new CodeCommentStatement (sb.ToString(), false));
    }
  }

  /// <summary>
  /// Adds a <c>summary</c> documentation comment.
  /// </summary>
  public virtual void AddSummaryComment (CodeTypeMember item, string summary)
  {
    AddDocumentationComment (item, "summary", null, null, summary);
  }

  /// <summary>
  /// Adds a <c>remarks</c> documentation comment.
  /// </summary>
  public virtual void AddRemarksComment (CodeTypeMember item, string remarks)
  {
    AddDocumentationComment (item, "remarks", null, "Remarks", remarks);
  }

  /// <summary>
  /// Adds a <c>param</c> documentation comment.
  /// </summary>
  public virtual void AddParameterComment (CodeTypeMember item, string parameterName, string description)
  {
    AddDocumentationComment (item, "param", "name=\"" + parameterName + "\"", "Parameter " + parameterName, description);
  }

  public virtual void AddExceptionComment (CodeTypeMember item, Type exceptionType, string condition)
  {
    AddDocumentationComment (item, "exception", "cref=\"" + exceptionType.FullName + "\"", "Exception " + exceptionType, condition);
  }

  public virtual void AddValueComment (CodeTypeMember item, string description)
  {
    AddDocumentationComment (item, "value", null, "Value", description);
  }

  public virtual void AddReturnsComment (CodeTypeMember item, string description)
  {
    AddDocumentationComment (item, "returns", null, "Returns", description);
  }

  /// <summary>
  /// This method returns a valid identifier name for the CodeDOM provider.
  /// </summary>
  /// <param name="name">The identifier.</param>
  /// <returns>The parameter <c>name</c> itself.</returns>
  /// <remarks>Override this method only if the CodeDOM provider you are using does not correctly escape all identifiers, as is
  /// the case with the C# provider that does not escape the keyword <c>params</c>.
  /// </remarks>
  public virtual string GetValidName (string name)
  {
    return name;
  }

  /// <summary>
  /// If implemented by a derived class, modifies the specified <c>CompilerParameters</c> to create an XML documentation file.
  /// </summary>
  public virtual void AddOptionCreateXmlDocumentation (CompilerParameters parameters, string xmlFilename, bool missingXmlWarnings)
  {
  }

  /// <summary>
  /// Specifies whether the language is case sensitive.
  /// </summary>
  public abstract bool IsCaseSensitive { get; }

  /// <summary>
  /// Writes the language-specific keywords that correspond to the values of the <c>attribute</c> parameter.
  /// </summary>
  /// <remarks>
  /// This method aids in the implementation of <see cref="CreateCastingOperator"/>.
  /// </remarks>
  /// <param name="sb">The StringBuilder object to write to.</param>
  /// <param name="mappings">An array of <see cref="MemberAttributeKeywordMapping"/> mappings that define the language-specific keywords.</param>
  /// <param name="attributes">The concrete attributes that are to be written.</param>
  protected static void AppendMemberAttributeString (
      StringBuilder sb, MemberAttributeKeywordMapping[] mappings, MemberAttributes attributes)
  {
    foreach (MemberAttributeKeywordMapping mapping in mappings)
    {
      if (mapping.IsSet (attributes))
      {
        if (sb.Length > 0)
          sb.Append (" ");
        sb.Append (mapping.Keyword);
      }
    }
  }

  /// <summary>
  /// Creates a type that contains integer values.
  /// </summary>
  /// <param name="name">The name of the new type.</param>
  /// <returns>The default implementation returns a <c>CodeTypeDeclaration</c> object with its <c>IsEnum</c> property set to <see langword="true" />.</returns>
  public virtual CodeTypeDeclaration CreateEnumDeclaration (string name)
  {
    CodeTypeDeclaration enumDeclaration = new CodeTypeDeclaration (GetValidName (name));
    enumDeclaration.IsEnum = true;
    return enumDeclaration;
  }

  /// <summary>
  /// Creates a member for an enum type.
  /// </summary>
  /// <param name="enumDeclaration">A reference to the enum type this member is intended for.</param>
  /// <param name="numericValue">The numeric enumeration value.</param>
  /// <param name="name">The name of the member.</param>
  /// <returns>Returns a <c>CodeTypeMember</c> that can be added to the <c>Members</c> collection of an enum type created 
  /// using <see cref="CreateEnumDeclaration"/>. </returns>
  public virtual CodeTypeMember CreateEnumValue (CodeTypeReference enumDeclaration, int numericValue, string name)
  {
    CodeMemberField enumValueField = new CodeMemberField (enumDeclaration, name);
    enumValueField.InitExpression = new CodePrimitiveExpression (numericValue);
    enumValueField.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Const;
    return enumValueField;
  }

  public abstract CodeExpression CreateUnaryOperatorExpression (CodeUnaryOperatorType operatorType, CodeExpression expression);

  /// <summary>
  /// Use this method to create value types if you want to create constructors.
  /// </summary>
  /// <remarks>
  /// This method allows the VB provider to create a workaround for a CodeDOM bug. See <see cref="CreateStructConstructor"/>.
  /// </remarks>
  public virtual CodeTypeDeclaration CreateStructWithConstructors (string name)
  {
    CodeTypeDeclaration type = new CodeTypeDeclaration (name);
    type.IsStruct = true;
    return type;
  }

  /// <summary>
  /// Use this method to create constructors for value types.
  /// </summary>
  /// <remarks>
  /// This method allows the VB provider to create a workaround for a CodeDOM bug. See <see cref="CreateStructWithConstructors"/>.
  /// </remarks>
  public virtual CodeConstructor CreateStructConstructor ()
  {
    CodeConstructor ctor = new CodeConstructor ();
    return ctor;
  }
}

/// <summary>
/// Specifies one of the two kinds of casting operators.
/// </summary>
public enum CodeCastOperatorKind 
{ 
  Implicit, 
  Explicit 
}

public enum CodeUnaryOperatorType
{
  BooleanNot,
  Negate,
  Plus
}

public enum CodeOverridableOperatorType
{
  Equality,
  Inequality,
  LessThan,
  LessThanOrEqual,
  GreaterThan,
  GreaterThanOrEqual,
  BitwiseAnd,
  BooleanOr,
  Add,
  Subtract,
  Multiply,
  Divide,
  Modulus
}

}
