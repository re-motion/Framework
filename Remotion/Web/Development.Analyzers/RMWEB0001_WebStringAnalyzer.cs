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
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remotion.Web.Development.Analyzers
{
  /// <summary>
  /// An analyzer for <see cref="WebDiagnosticIDs.RMWEB0001_WrongWebStringUsage"/>.
  /// </summary>
  /// <seealso cref="WebDiagnosticIDs.RMWEB0001_WrongWebStringUsage"/>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class RMWEB0001_WebStringAnalyzer : DiagnosticAnalyzer
  {
    private record SymbolContext(
        INamedTypeSymbol WebStringTypeSymbol,
        INamedTypeSymbol PlainTextStringTypeSymbol,
        INamedTypeSymbol WebStringEnumerable,
        INamedTypeSymbol PlainTextStringEnumerable,
        IMethodSymbol StringBuilderAppendMethodSymbol,
        IMethodSymbol HtmlTextWriterWriteMethodSymbol,
        IReadOnlyCollection<IMethodSymbol> StringJoinMethodSymbols,
        IReadOnlyCollection<IMethodSymbol> StringFormatMethodSymbols,
        IReadOnlyCollection<IMethodSymbol> StringConcatMethodSymbols);

    public static readonly DiagnosticDescriptor DiagnosticDescriptor = new DiagnosticDescriptor(
        WebDiagnosticIDs.RMWEB0001_WrongWebStringUsage,
        "Wrong WebString or PlainTextString usage",
        "'{0}' should not be used with a '{1}' argument. {2}.",
        "Usage",
        DiagnosticSeverity.Error,
        true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptor);

    public override void Initialize (AnalysisContext context)
    {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);

      context.RegisterCompilationStartAction(
          compilationContext =>
          {
            var webStringTypeSymbol = compilationContext.Compilation.GetTypeByMetadataName("Remotion.Web.WebString");
            var plainTextStringTypeSymbol = compilationContext.Compilation.GetTypeByMetadataName("Remotion.Web.PlainTextString");
            var stringBuilderTypeSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Text.StringBuilder");
            var htmlTextWriterSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Web.UI.HtmlTextWriter");

            if (webStringTypeSymbol == null
                || plainTextStringTypeSymbol == null
                || stringBuilderTypeSymbol == null
                || htmlTextWriterSymbol == null)
            {
              return;
            }

            var objectSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Object");
            var stringSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.String");
            var iEnumerable = compilationContext.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")!;
            var webStringEnumerable = iEnumerable.Construct(webStringTypeSymbol);
            var plainTextStringEnumerable = iEnumerable.Construct(plainTextStringTypeSymbol);
            var stringBuilderAppendMethodSymbol = GetStringBuilderAppendObjectMethodSymbol(stringBuilderTypeSymbol, objectSymbol);
            var htmlTextWriterWriteMethodSymbol = GetHtmlTextWriterWriteObjectMethodSymbol(htmlTextWriterSymbol, objectSymbol);
            var stringJoinMethodSymbols = GetStringJoinMethodSymbols(stringSymbol!);
            var stringFormatMethodSymbols = GetStringFormatMethodSymbols(stringSymbol!);
            var stringConcatMethodSymbols = GetStringConcatMethodSymbols(stringSymbol!);

            var symbolContext = new SymbolContext(
                webStringTypeSymbol,
                plainTextStringTypeSymbol,
                webStringEnumerable,
                plainTextStringEnumerable,
                stringBuilderAppendMethodSymbol,
                htmlTextWriterWriteMethodSymbol,
                stringJoinMethodSymbols,
                stringFormatMethodSymbols,
                stringConcatMethodSymbols);

            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeAddExpression(ctx, symbolContext),
                ImmutableArray.Create(SyntaxKind.AddExpression));
            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeInterpolation(ctx, symbolContext),
                ImmutableArray.Create(SyntaxKind.Interpolation));
            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeInvocationExpression(ctx, symbolContext),
                ImmutableArray.Create(SyntaxKind.InvocationExpression));
          });
    }

    private void AnalyzeInterpolation (SyntaxNodeAnalysisContext ctx, SymbolContext symbolContext)
    {
      var node = (InterpolationSyntax)ctx.Node;
      var symbol = ctx.SemanticModel.GetTypeInfo(node.Expression).Type;

      if (SymbolEqualityComparer.Default.Equals(symbol, symbolContext.WebStringTypeSymbol)
          || SymbolEqualityComparer.Default.Equals(symbol, symbolContext.PlainTextStringTypeSymbol))
      {
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptor,
            node.Expression.GetLocation(),
            "$\"\"",
            symbol.ToString(),
            $"Encode the {symbol.ToString()} instance first.");
        ctx.ReportDiagnostic(diagnostic);
      }
    }

    private void AnalyzeAddExpression (SyntaxNodeAnalysisContext ctx, SymbolContext symbolContext)
    {
      var node = (BinaryExpressionSyntax)ctx.Node;
      var leftSymbol = ctx.SemanticModel.GetTypeInfo(node.Left).Type;
      var rightSymbol = ctx.SemanticModel.GetTypeInfo(node.Right).Type;

      if (SymbolEqualityComparer.Default.Equals(leftSymbol, symbolContext.WebStringTypeSymbol))
        ReportDiagnostic(ctx, node.Left.GetLocation(), symbolContext.WebStringTypeSymbol);
      if (SymbolEqualityComparer.Default.Equals(rightSymbol, symbolContext.WebStringTypeSymbol))
        ReportDiagnostic(ctx, node.Right.GetLocation(), symbolContext.WebStringTypeSymbol);
      if (SymbolEqualityComparer.Default.Equals(leftSymbol, symbolContext.PlainTextStringTypeSymbol))
        ReportDiagnostic(ctx, node.Left.GetLocation(), symbolContext.PlainTextStringTypeSymbol);
      if (SymbolEqualityComparer.Default.Equals(rightSymbol, symbolContext.PlainTextStringTypeSymbol))
        ReportDiagnostic(ctx, node.Right.GetLocation(), symbolContext.PlainTextStringTypeSymbol);

      static void ReportDiagnostic (SyntaxNodeAnalysisContext ctx, Location location, ITypeSymbol symbol)
      {
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptor,
            location,
            "+",
            symbol.ToString(),
            $"Encode the {symbol.ToString()} instance first.");
        ctx.ReportDiagnostic(diagnostic);
      }
    }

    private void AnalyzeInvocationExpression (SyntaxNodeAnalysisContext ctx, SymbolContext symbols)
    {
      var invocationSymbolInfo = ctx.SemanticModel.GetSymbolInfo(ctx.Node);

      if (invocationSymbolInfo.Symbol == null)
        return;

      var node = (InvocationExpressionSyntax)ctx.Node;
      var invocationOriginalDefinitionSymbol = invocationSymbolInfo.Symbol.OriginalDefinition;

      // Check if the invoked method is StringBuilder.Append(object?)
      if (SymbolEqualityComparer.Default.Equals(invocationOriginalDefinitionSymbol, symbols.StringBuilderAppendMethodSymbol))
      {
        var argumentType = GetFirstArgumentTypeSymbol(node, ctx.SemanticModel);
        // Check if the first argument is WebString or PlainTextString
        if (SymbolEqualityComparer.Default.Equals(argumentType, symbols.WebStringTypeSymbol)
            || SymbolEqualityComparer.Default.Equals(argumentType, symbols.PlainTextStringTypeSymbol))
        {
          var diagnostic = Diagnostic.Create(
              DiagnosticDescriptor,
              ctx.Node.GetLocation(),
              symbols.StringBuilderAppendMethodSymbol.ToString(),
              argumentType.ToString(),
              $"Call '.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks)' on the {argumentType.ToString()} instance instead.");
          ctx.ReportDiagnostic(diagnostic);
        }
      }
      // Check if the invoked method is HtmlTextWriter.Write(object?)
      else if (SymbolEqualityComparer.Default.Equals(invocationOriginalDefinitionSymbol, symbols.HtmlTextWriterWriteMethodSymbol))
      {
        var argumentType = GetFirstArgumentTypeSymbol(node, ctx.SemanticModel);
        // Check if the first argument is WebString or PlainTextString
        if (SymbolEqualityComparer.Default.Equals(argumentType, symbols.WebStringTypeSymbol)
            || SymbolEqualityComparer.Default.Equals(argumentType, symbols.PlainTextStringTypeSymbol))
        {
          var diagnostic = Diagnostic.Create(
              DiagnosticDescriptor,
              ctx.Node.GetLocation(),
              symbols.HtmlTextWriterWriteMethodSymbol.ToString(),
              argumentType.ToString(),
              $"Use '{argumentType.ToString()}.WriteTo(HtmlTextWriter)' instead.");
          ctx.ReportDiagnostic(diagnostic);
        }
      }
      // Check if the invoked method is a string.Join(...) overload
      else if (TryFindItem(
          symbols.StringJoinMethodSymbols,
          s => SymbolEqualityComparer.Default.Equals(invocationOriginalDefinitionSymbol, s),
          out var stringJoinMethodSymbol))
      {
        var argumentTypes = GetArgumentTypeSymbols(node, ctx.SemanticModel);
        // Check if one of the arguments is a WebString or PlainTextString or implicitly convertible to one of them.
        if (TryFindItem(
            argumentTypes,
            a => SymbolEqualityComparer.Default.Equals(a, symbols.WebStringTypeSymbol)
                 || SymbolEqualityComparer.Default.Equals(a, symbols.PlainTextStringTypeSymbol)
                 || ctx.Compilation.ClassifyConversion(a, symbols.WebStringEnumerable).IsImplicit
                 || ctx.Compilation.ClassifyConversion(a, symbols.PlainTextStringEnumerable).IsImplicit,
            out var result))
        {
          var diagnostic = Diagnostic.Create(
              DiagnosticDescriptor,
              ctx.Node.GetLocation(),
              stringJoinMethodSymbol.ToString(),
              result.ToString(),
              $"Encode the {result.ToString()} instances first.");
          ctx.ReportDiagnostic(diagnostic);
        }
      }
      // Check if the invoked method is a string.Format(...) overload
      else if (TryFindItem(
          symbols.StringFormatMethodSymbols,
          s => SymbolEqualityComparer.Default.Equals(invocationOriginalDefinitionSymbol, s),
          out var stringFormatMethodSymbol))
      {
        var argumentTypes = GetArgumentTypeSymbols(node, ctx.SemanticModel);
        // Check if one of the arguments is a WebString or PlainTextString.
        if (TryFindItem(
            argumentTypes,
            a => SymbolEqualityComparer.Default.Equals(a, symbols.WebStringTypeSymbol)
                 || SymbolEqualityComparer.Default.Equals(a, symbols.PlainTextStringTypeSymbol),
            out var result))
        {
          var diagnostic = Diagnostic.Create(
              DiagnosticDescriptor,
              ctx.Node.GetLocation(),
              stringFormatMethodSymbol.ToString(),
              result.ToString(),
              $"Encode the {result.ToString()} instances first.");
          ctx.ReportDiagnostic(diagnostic);
        }
      }
      // Check if the invoked method is a string.Concat(...) overload
      else if (TryFindItem(
          symbols.StringConcatMethodSymbols,
          s => SymbolEqualityComparer.Default.Equals(invocationOriginalDefinitionSymbol, s),
          out var stringFormatConcatSymbol))
      {
        var argumentTypes = GetArgumentTypeSymbols(node, ctx.SemanticModel);
        // Check if one of the arguments is a WebString or PlainTextString.
        if (TryFindItem(
            argumentTypes,
            a => SymbolEqualityComparer.Default.Equals(a, symbols.WebStringTypeSymbol)
                 || SymbolEqualityComparer.Default.Equals(a, symbols.PlainTextStringTypeSymbol)
                 || ctx.Compilation.ClassifyConversion(a, symbols.WebStringEnumerable).IsImplicit
                 || ctx.Compilation.ClassifyConversion(a, symbols.PlainTextStringEnumerable).IsImplicit,
            out var result))
        {
          var diagnostic = Diagnostic.Create(
              DiagnosticDescriptor,
              ctx.Node.GetLocation(),
              stringFormatConcatSymbol.ToString(),
              result.ToString(),
              $"Encode the {result.ToString()} instances first.");
          ctx.ReportDiagnostic(diagnostic);
        }
      }
    }

    private static bool TryFindItem<TSymbol> (IEnumerable<TSymbol> methods, Func<TSymbol, bool> predicate, out TSymbol match)
        where TSymbol : ISymbol
    {
      match = methods.FirstOrDefault(predicate)!;
      return match != null;
    }

    private static ITypeSymbol? GetFirstArgumentTypeSymbol (InvocationExpressionSyntax node, SemanticModel model)
    {
      var firstArgumentExpression = node.ArgumentList.Arguments.First().Expression;
      return model.GetTypeInfo(firstArgumentExpression).Type;
    }

    private static IReadOnlyCollection<ITypeSymbol> GetArgumentTypeSymbols (InvocationExpressionSyntax node, SemanticModel model)
    {
      return node.ArgumentList.Arguments.Select(s => model.GetTypeInfo(s.Expression).Type).OfType<ITypeSymbol>().ToList();
    }

    private static IReadOnlyCollection<IMethodSymbol> GetStringJoinMethodSymbols (INamedTypeSymbol stringSymbol)
    {
      return stringSymbol
          .GetMembers("Join")
          .Cast<IMethodSymbol>()
          .ToList();
    }

    private static IReadOnlyCollection<IMethodSymbol> GetStringFormatMethodSymbols (INamedTypeSymbol stringSymbol)
    {
      return stringSymbol
          .GetMembers("Format")
          .Cast<IMethodSymbol>()
          .ToList();
    }

    private IReadOnlyCollection<IMethodSymbol> GetStringConcatMethodSymbols (INamedTypeSymbol stringSymbol)
    {
      return stringSymbol
          .GetMembers("Concat")
          .Cast<IMethodSymbol>()
          .ToList();
    }

    private static IMethodSymbol GetHtmlTextWriterWriteObjectMethodSymbol (INamedTypeSymbol htmlTextWriterSymbol, INamedTypeSymbol? objectSymbol)
    {
      return htmlTextWriterSymbol
          .GetMembers("Write")
          .Cast<IMethodSymbol>()
          .Single(m => (m.Parameters.Length == 1) && SymbolEqualityComparer.Default.Equals(m.Parameters.Single().Type, objectSymbol));
    }

    private static IMethodSymbol GetStringBuilderAppendObjectMethodSymbol (INamedTypeSymbol stringBuilderTypeSymbol, INamedTypeSymbol? objectSymbol)
    {
      return stringBuilderTypeSymbol
          .GetMembers("Append")
          .Cast<IMethodSymbol>()
          .Single(m => (m.Parameters.Length == 1) && SymbolEqualityComparer.Default.Equals(m.Parameters.Single().Type, objectSymbol));
    }
  }
}
