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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Remotion.Web.Development.Analyzers
{
  /// <summary>
  /// An analyzer for <see cref="WebDiagnosticIDs.RMWEB0003_PossiblyInvalidTypeForWxeResourcePageStepConstructor"/>.
  /// </summary>
  /// <seealso cref="WebDiagnosticIDs.RMWEB0003_PossiblyInvalidTypeForWxeResourcePageStepConstructor"/>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class WxeResourcePageStepAnalyzer : DiagnosticAnalyzer
  {
    private record SymbolContext (
        INamedTypeSymbol WxeResourcePageStepSymbol,
        INamedTypeSymbol WxeResourceUserPageStepSymbol,
        INamedTypeSymbol SystemTypeSymbol);

    public static readonly DiagnosticDescriptor RMWEB0003DiagnosticDescriptor = new(
        WebDiagnosticIDs.RMWEB0003_PossiblyInvalidTypeForWxeResourcePageStepConstructor,
        "Passing a type from another assembly to the constructor of WxeResourcePageStep or WxeResourceUserControlStep can indicate a copy & paste mistake",
        "Type '{0}' is not defined in the same assembly as the containing type '{1}'. If this is intentional, you can suppress the warning.",
        "Usage",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor RMWEB0004DiagnosticDescriptor = new(
        WebDiagnosticIDs.RMWEB0004_NonTypeofValuePassedToWxeResourcePageStepCosntructur,
        "Passing non-typeof value to the constructor of WxeResourcePageStep or WxeResourceUserControlStep is not intended",
        "Only typeof(xxx) values should be passed to the constructors of WxeResourcePageStep and WxeResourceUserControlStep that take a Type parameter."
        + "If the non-typeof usage is intentional, pass the type's assembly instead (`new(type.Assembly);`)",
        "Usage",
        DiagnosticSeverity.Error,
        true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RMWEB0003DiagnosticDescriptor, RMWEB0004DiagnosticDescriptor);

    public override void Initialize (AnalysisContext context)
    {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);

      context.RegisterCompilationStartAction(
          compilationContext =>
          {
            var wxeResourcePageStepSymbol = compilationContext.Compilation.GetTypeByMetadataName("Remotion.Web.ExecutionEngine.WxeResourcePageStep");
            var wxeResourceUserPageStepSymbol = compilationContext.Compilation.GetTypeByMetadataName("Remotion.Web.ExecutionEngine.WxeResourceUserControlStep");
            if (wxeResourcePageStepSymbol == null || wxeResourceUserPageStepSymbol == null)
              return;

            var systemTypeSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Type")!;

            var symbolContext = new SymbolContext(wxeResourcePageStepSymbol, wxeResourceUserPageStepSymbol, systemTypeSymbol);

            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeObjectCreationExpression(ctx, symbolContext),
                ImmutableArray.Create(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression));
          });
    }

    private void AnalyzeObjectCreationExpression (SyntaxNodeAnalysisContext ctx, SymbolContext symbolContext)
    {
      var node = (BaseObjectCreationExpressionSyntax)ctx.Node;
      if (ctx.SemanticModel.GetOperation(node) is not IObjectCreationOperation operation)
        return;

      if (!SymbolEqualityComparer.Default.Equals(operation.Type, symbolContext.WxeResourcePageStepSymbol)
          && !SymbolEqualityComparer.Default.Equals(operation.Type, symbolContext.WxeResourceUserPageStepSymbol))
        return;

      if (operation.Arguments.IsEmpty)
        return;

      if (operation.Arguments[0].Value is not ITypeOfOperation typeOfOperation)
      {
        if (SymbolEqualityComparer.Default.Equals(operation.Arguments[0].Parameter?.Type, symbolContext.SystemTypeSymbol))
        {
          var diagnostic = Diagnostic.Create(
              RMWEB0004DiagnosticDescriptor,
              ctx.Node.GetLocation());
          ctx.ReportDiagnostic(diagnostic);
        }

        return;
      }

      var typeOfOperand = typeOfOperation.TypeOperand;
      if (typeOfOperand == null)
        return;

      var typeDeclaration = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
      if (typeDeclaration == null)
        return;

      var containingTypeInfo = ctx.SemanticModel.GetDeclaredSymbol(typeDeclaration);
      if (containingTypeInfo == null)
        return;

      var containingTypeAssembly = containingTypeInfo.ContainingAssembly;
      var typeOfAssembly = typeOfOperand.ContainingAssembly;
      if (!SymbolEqualityComparer.Default.Equals(containingTypeAssembly, typeOfAssembly))
      {
        var diagnostic = Diagnostic.Create(
            RMWEB0003DiagnosticDescriptor,
            ctx.Node.GetLocation(),
            typeOfOperand.ToDisplayString(),
            containingTypeInfo.ToDisplayString());
        ctx.ReportDiagnostic(diagnostic);
      }
    }
  }
}
