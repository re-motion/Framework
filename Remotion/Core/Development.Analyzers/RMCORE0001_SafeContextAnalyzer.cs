using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Core.Development.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class RMCORE0001_SafeContextAnalyzer : DiagnosticAnalyzer
  {
    public static readonly DiagnosticDescriptor AlternativeDescriptor = new DiagnosticDescriptor(
        "RMCORE0001",
        "Use SafeContext instead of typical API",
        "'{0}' should not be used with SafeContext, use 'Remotion.Context.SafeContext.{1}' instead.",
        "Usage",
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor WarningDescriptor = new DiagnosticDescriptor(
        "RMCORE0001",
        "Use typical API with SafeContext",
        "'{0}' should not be used with SafeContext by itself, use '{1}' to create a safe context boundary.",
        "Usage",
        DiagnosticSeverity.Warning,
        true);

    public override void Initialize (AnalysisContext context)
    {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
      context.RegisterCompilationStartAction(
          compilationContext =>
          {
            var safeContextTypeSymbol = compilationContext.Compilation.GetTypeByMetadataName("Remotion.Context.SafeContext");
            if (safeContextTypeSymbol == null)
              return;

            var tasksTaskSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
            var parallelSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Parallel");
            var threadSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Threading.Thread");
            var threadingTimerSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Threading.Timer");
            var executionSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Threading.ExecutionContext");
            var timingTimerSymbol = compilationContext.Compilation.GetTypeByMetadataName("System.Timers.Timer");

            var invocationExpressionMethodsAndActions = new List<(IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>)>();
            AddMethodSymbolWithDiagnosticToList(invocationExpressionMethodsAndActions, tasksTaskSymbol, "Run", TaskRunDiagnostic);
            AddMethodSymbolWithDiagnosticToList(invocationExpressionMethodsAndActions, tasksTaskSymbol, "ContinueWith", TaskContinueWithDiagnostic);
            AddMethodSymbolWithDiagnosticToList(invocationExpressionMethodsAndActions, executionSymbol, "Run", ExecutionRunDiagnostic);
            AddMethodSymbolWithDiagnosticToList(invocationExpressionMethodsAndActions, parallelSymbol, "For", ParallelDiagnostic);
            AddMethodSymbolWithDiagnosticToList(invocationExpressionMethodsAndActions, parallelSymbol, "ForEach", ParallelDiagnostic);
            AddMethodSymbolWithDiagnosticToList(invocationExpressionMethodsAndActions, parallelSymbol, "ForEachAsync", ParallelDiagnostic);
            AddMethodSymbolWithDiagnosticToList(invocationExpressionMethodsAndActions, parallelSymbol, "Invoke", ParallelDiagnostic);

            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeInvocationExpression(ctx, invocationExpressionMethodsAndActions),
                SyntaxKind.InvocationExpression);

            var objectCreationMethodsAndActions = new List<(IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>)>();
            AddCtorSymbolWithDiagnosticToList(objectCreationMethodsAndActions, threadSymbol, NewThreadDiagnostic);
            AddCtorSymbolWithDiagnosticToList(objectCreationMethodsAndActions, threadingTimerSymbol, NewTimerDiagnostic);

            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeObjectCreationExpression(ctx, objectCreationMethodsAndActions),
                SyntaxKind.ObjectCreationExpression);

            var addMethodsWithActions = new List<(IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>)>();
            AddAddMethodSymbolWithDiagnosticToList(addMethodsWithActions, timingTimerSymbol, "Elapsed", AddElapsedTimerDiagnostic);

            compilationContext.RegisterSyntaxNodeAction(ctx => AnalyzeAddAssignmentExpression(ctx, addMethodsWithActions),
                SyntaxKind.AddAssignmentExpression);
          });
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get { return ImmutableArray.Create(AlternativeDescriptor, WarningDescriptor); }
    }

    private void AnalyzeAddAssignmentExpression (
        SyntaxNodeAnalysisContext ctx,
        IReadOnlyCollection<(IMethodSymbol eventSymbol, Action<SyntaxNodeAnalysisContext, ISymbol> CreateDiagnostic)> eventSymbolAndDiagnostics)
    {
      var memberAccessSymbolInfo = ctx.SemanticModel.GetSymbolInfo(ctx.Node);

      if (memberAccessSymbolInfo.Symbol == null)
        return;

      var memberAccessOriginalDefinitionSymbol = memberAccessSymbolInfo.Symbol.OriginalDefinition;

      var match = eventSymbolAndDiagnostics.FirstOrDefault(m => SymbolEqualityComparer.Default.Equals(m.eventSymbol, memberAccessOriginalDefinitionSymbol));
      if (match != default)
        match.CreateDiagnostic.Invoke(ctx, match.eventSymbol);
    }

    private static void AnalyzeInvocationExpression (
        SyntaxNodeAnalysisContext ctx,
        IReadOnlyCollection<(IMethodSymbol methodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol> CreateDiagnostic)> methodSymbols)
    {
      var invocationSymbolInfo = ctx.SemanticModel.GetSymbolInfo(ctx.Node);

      if (invocationSymbolInfo.Symbol == null)
        return;

      var invocationOriginalDefinitionSymbol = invocationSymbolInfo.Symbol.OriginalDefinition;

      var match = methodSymbols.FirstOrDefault(m => SymbolEqualityComparer.Default.Equals(m.methodSymbol, invocationOriginalDefinitionSymbol));
      if (match != default)
        match.CreateDiagnostic.Invoke(ctx, match.methodSymbol);
    }

    private static void AnalyzeObjectCreationExpression (
        SyntaxNodeAnalysisContext ctx,
        IReadOnlyCollection<(IMethodSymbol methodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol> CreateDiagnostic)> ctorMethodsWithDiagnostics)
    {
      var objectCreationInfo = ctx.SemanticModel.GetSymbolInfo(ctx.Node);

      if (objectCreationInfo.Symbol == null)
        return;

      var objectCreationInfoOriginalDefinitionSymbol = (IMethodSymbol)objectCreationInfo.Symbol.OriginalDefinition;

      var match = ctorMethodsWithDiagnostics.FirstOrDefault(m => SymbolEqualityComparer.Default.Equals(m.Item1, objectCreationInfoOriginalDefinitionSymbol));
      if (match != default)
        match.CreateDiagnostic.Invoke(ctx, match.methodSymbol);
    }

    private static IEnumerable<IMethodSymbol> GetMethodSymbols (INamedTypeSymbol? typeSymbol, string name)
    {
      if (typeSymbol == null)
        return Enumerable.Empty<IMethodSymbol>();

      return typeSymbol
          .GetMembers(name)
          .Cast<IMethodSymbol>()
          .ToList();
    }

    private static IEnumerable<IMethodSymbol> GetCtorSymbols (INamedTypeSymbol? typeSymbol)
    {
      if (typeSymbol == null)
        return Enumerable.Empty<IMethodSymbol>();

      return typeSymbol.Constructors.ToList();
    }

    private static IEnumerable<IMethodSymbol> GetAddEventMethodSymbol (INamedTypeSymbol? typeSymbol, string name)
    {
      if (typeSymbol == null)
        return Enumerable.Empty<IMethodSymbol>();


      return typeSymbol
          .GetMembers(name)
          .Cast<IEventSymbol>()
          .Where(s => s.AddMethod != null)
          .Select(s => s.AddMethod!);
    }

    private static void AddMethodSymbolWithDiagnosticToList (
        List<(IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>)> listOfMethodsAndActions,
        INamedTypeSymbol? parentSymbol,
        string methodName,
        Action<SyntaxNodeAnalysisContext, ISymbol> diagnostic)
    {
      var childMethod = GetMethodSymbols(parentSymbol, methodName);
      AddSymbolsWithAction(listOfMethodsAndActions, childMethod, diagnostic);
    }

    private static void AddCtorSymbolWithDiagnosticToList (
        List<(IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>)> listOfMethodsAndActions,
        INamedTypeSymbol? parentSymbol,
        Action<SyntaxNodeAnalysisContext, ISymbol> diagnostic)
    {
      var childMethod = GetCtorSymbols(parentSymbol);
      AddSymbolsWithAction(listOfMethodsAndActions, childMethod, diagnostic);
    }

    private static void AddAddMethodSymbolWithDiagnosticToList (
        List<(IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>)> listOfMethodsAndActions,
        INamedTypeSymbol? parentSymbol,
        string methodName,
        Action<SyntaxNodeAnalysisContext, ISymbol> diagnostic)
    {
      var childMethod = GetAddEventMethodSymbol(parentSymbol, methodName);
      AddSymbolsWithAction(listOfMethodsAndActions, childMethod, diagnostic);
    }

    private static void AddSymbolsWithAction (
        List<(IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>)> listWithBoth,
        IEnumerable<IMethodSymbol> symbols,
        Action<SyntaxNodeAnalysisContext, ISymbol> action)
    {
      listWithBoth.AddRange(symbols.Select(method => ((IMethodSymbol, Action<SyntaxNodeAnalysisContext, ISymbol>))(method, action)));
    }

    private static void ExecutionRunDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol)
    {
      CreateAlternativeDiagnostic(ctx, symbol, "ExecutionContext.Run");
    }

    private static void TaskRunDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol)
    {
      CreateAlternativeDiagnostic(ctx, symbol, "Task.Run");
    }

    private static void TaskContinueWithDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol)
    {
      CreateAlternativeDiagnostic(ctx, symbol, "Task.ContinueWith");
    }

    private static void ParallelDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol)
    {
      CreateWarningDiagnostic(ctx, symbol, "Remotion.Context.SafeContext.Parallel.OpenSafeContextBoundary()");
    }

    private static void NewThreadDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol)
    {
      CreateCtorDiagnostic(ctx, symbol, "Thread.New");
    }

    private static void NewTimerDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol)
    {
      CreateCtorDiagnostic(ctx, symbol, "Threading.NewTimer");
    }

    private static void AddElapsedTimerDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol)
    {
      CreateAlternativeDiagnostic(ctx, symbol, "Timers.AddElapsedEventHandler");
    }

    private static void CreateAlternativeDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol definitionSymbol, string solutionDescription)
    {
      var nameSpace = definitionSymbol.ContainingNamespace.ToString();
      var type = definitionSymbol.ContainingType.Name;
      var diagnostic = Diagnostic.Create(
          AlternativeDescriptor,
          ctx.Node.GetLocation(),
          $"{nameSpace}.{type}.{definitionSymbol.Name}",
          solutionDescription);
      ctx.ReportDiagnostic(diagnostic);
    }

    private static void CreateCtorDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol, string alternative)
    {
      var nameSpace = symbol.ContainingNamespace.ToString();
      var type = symbol.ContainingType.Name;
      var diagnostic = Diagnostic.Create(
          AlternativeDescriptor,
          ctx.Node.GetLocation(),
          $"new {nameSpace}.{type}",
          alternative);
      ctx.ReportDiagnostic(diagnostic);
    }

    private static void CreateWarningDiagnostic (SyntaxNodeAnalysisContext ctx, ISymbol symbol, string alternative)
    {
      var nameSpace = symbol.ContainingNamespace.ToString();
      var type = symbol.ContainingType.Name;
      var diagnostic = Diagnostic.Create(
          WarningDescriptor,
          ctx.Node.GetLocation(),
          $"{nameSpace}.{type}.{symbol.Name}",
          alternative);
      ctx.ReportDiagnostic(diagnostic);
    }
  }
}
