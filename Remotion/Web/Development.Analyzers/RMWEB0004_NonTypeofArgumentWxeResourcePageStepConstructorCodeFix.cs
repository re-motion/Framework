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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Remotion.Web.Development.Analyzers;

/// <summary>
/// A code fix for <see cref="WebDiagnosticIDs.RMWEB0004_NonTypeofValuePassedToWxeResourcePageStepCosntructur"/>.
/// </summary>
/// <seealso cref="WebDiagnosticIDs.RMWEB0004_NonTypeofValuePassedToWxeResourcePageStepCosntructur"/>
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class RMWEB0004_NonTypeofArgumentWxeResourcePageStepConstructorCodeFix : CodeFixProvider
{
  /// <inheritdoc />
  public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(WebDiagnosticIDs.RMWEB0004_NonTypeofValuePassedToWxeResourcePageStepCosntructur);

  /// <inheritdoc />
  public override Task RegisterCodeFixesAsync (CodeFixContext context)
  {
    context.RegisterCodeFix(
        CodeAction.Create(
            "Use Type.Assembly instead",
            cancellationToken => ChangeToTypeAssemblyParameter(context, cancellationToken),
            WebDiagnosticIDs.RMWEB0004_NonTypeofValuePassedToWxeResourcePageStepCosntructur),
        context.Diagnostics.First());

    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public override FixAllProvider? GetFixAllProvider ()
  {
    return WellKnownFixAllProviders.BatchFixer;
  }

  private async Task<Document> ChangeToTypeAssemblyParameter (CodeFixContext context, CancellationToken cancellationToken)
  {
    var location = context.Diagnostics.First().Location;
    var syntaxRoot = await context.Document.GetSyntaxRootAsync(cancellationToken);

    if (syntaxRoot is null)
      return context.Document;

    var syntaxNode = (ObjectCreationExpressionSyntax)syntaxRoot.FindNode(location.SourceSpan);

    var argumentSyntax = syntaxNode.ArgumentList!.Arguments[0];

    var newArgument = argumentSyntax.WithExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, argumentSyntax.Expression, IdentifierName("Assembly")));
    var newSeparatedList = SeparatedList(new[] { newArgument }.Union(syntaxNode.ArgumentList!.Arguments.Skip(1)));
    var newArgumentList = syntaxNode.ArgumentList.WithArguments(newSeparatedList);

    var newRoot = syntaxRoot.ReplaceNode(syntaxNode.ArgumentList, newArgumentList);
    return context.Document.WithSyntaxRoot(newRoot);
  }
}
