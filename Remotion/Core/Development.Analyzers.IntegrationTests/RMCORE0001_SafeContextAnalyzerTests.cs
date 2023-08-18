using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Verifier = Remotion.Core.Development.Analyzers.IntegrationTests.CSharpAnalyzerVerifier<Core.Development.Analyzers.RMCORE0001_SafeContextAnalyzer>;
using Analyzer = Core.Development.Analyzers.RMCORE0001_SafeContextAnalyzer;

namespace Remotion.Core.Development.Analyzers.IntegrationTests
{
  [TestFixture]
  public class RMCORE0001_SafeContextAnalyzerTests
  {
    [Test]
    public async Task SafeContext_WithTaskContinueWith_ButNoSafeContext_NoDiagnostic ()
    {
      const string input = @"
using System.Threading.Tasks;
public class A
{
  public void Test ()
  {
    var t = Task.CompletedTask;
    t.ContinueWith((task => { }));
  }
}
";
      await Verifier.VerifyAnalyzerAsync(input, false);
    }

    [Test]
    public async Task SafeContext_WithTaskRun_Diagnostic ()
    {
      const string input = @"
using System.Threading.Tasks;
using Remotion.Context;
public class A
{
  public void Test ()
  {
    SafeContext.Instance.SetData(""a"", 3);
    Task.Run(() => SafeContext.Instance.GetData(""a""));

    Task.Run(() => GetData(""a""));
  }
  static void GetData(string s)
  {
    SafeContext.Instance.GetData(s);
  }
}
";
      var diagnosticFunc = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(9, 5, 9, 54)
          .WithMessage(
              "'System.Threading.Tasks.Task.Run' should not be used with SafeContext, use 'Remotion.Context.SafeContext.Task.Run' instead.");
      var diagnosticAction = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(11, 5, 11, 33)
          .WithMessage(
              "'System.Threading.Tasks.Task.Run' should not be used with SafeContext, use 'Remotion.Context.SafeContext.Task.Run' instead.");

      await Verifier.VerifyAnalyzerAsync(input, true, diagnosticFunc, diagnosticAction);
    }

    [Test]
    public async Task SafeContext_WithTaskContinueWith_Diagnostic ()
    {
      const string input = @"
using System.Threading.Tasks;
using Remotion.Context;
public class A
{
  public void Test ()
  {
    SafeContext.Instance.SetData(""a"", 3);

    var t = Task.CompletedTask;
    t.ContinueWith((task => { }));
  }
}
";
      var diagnostic = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(11, 5, 11, 34)
          .WithMessage(
              "'System.Threading.Tasks.Task.ContinueWith' should not be used with SafeContext, use 'Remotion.Context.SafeContext.Task.ContinueWith' instead.");

      await Verifier.VerifyAnalyzerAsync(input, true, diagnostic);
    }

    [Test]
    public async Task SafeContext_WithExecutionContextRun_Diagnostic ()
    {
      const string input = @"
using System.Threading;
using Remotion.Context;
public class A
{
  public void Test ()
  { 
    SafeContext.Instance.SetData(""a"", 3);
    var ec = ExecutionContext.Capture();

    ExecutionContext.Run(
        ec,
        _ => { SafeContext.Instance.SetData(""a"", 3); },
        null);
  }
}
";
      var diagnosticFunc = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(11, 5, 14, 14)
          .WithMessage(
              "'System.Threading.ExecutionContext.Run' should not be used with SafeContext, use 'Remotion.Context.SafeContext.ExecutionContext.Run' instead.");

      await Verifier.VerifyAnalyzerAsync(input, true, diagnosticFunc);
    }

    [Test]
    public async Task SafeContext_WithTasksParallel_Diagnostic ()
    {
      const string input = @"
using System;
using System.Threading.Tasks;
using System.Linq;
using Remotion.Context;
public class A
{
  public void Test ()
  { 
    SafeContext.Instance.SetData(""a"", 3);

    Parallel.For(0, 5, i => { i = (int) SafeContext.Instance.GetData(""a"");});

    Parallel.ForEach(Enumerable.Range(5, 10).ToList(), i => Console.Write(i));

    Parallel.ForEachAsync(
        Enumerable.Range(0, 5),
        new ParallelOptions(), 
        (i, token) => ValueTask.CompletedTask
    );

    Parallel.Invoke(() => Console.Write(""a""));
  }
}
";
      var diagnosticFor = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(12, 5, 12, 77)
          .WithMessage(
              "'System.Threading.Tasks.Parallel.For' should not be used with SafeContext by itself, use 'Remotion.Context.SafeContext.Parallel.OpenSafeContextBoundary()' to create a safe context boundary.");

      var diagnosticForEach = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(14, 5, 14, 78)
          .WithMessage(
              "'System.Threading.Tasks.Parallel.ForEach' should not be used with SafeContext by itself, use 'Remotion.Context.SafeContext.Parallel.OpenSafeContextBoundary()' to create a safe context boundary.");

      var diagnosticForEachAsync = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(16, 5, 20, 6)
          .WithMessage(
              "'System.Threading.Tasks.Parallel.ForEachAsync' should not be used with SafeContext by itself, use 'Remotion.Context.SafeContext.Parallel.OpenSafeContextBoundary()' to create a safe context boundary.");

      var diagnosticInvoke = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(22, 5, 22, 46)
          .WithMessage(
              "'System.Threading.Tasks.Parallel.Invoke' should not be used with SafeContext by itself, use 'Remotion.Context.SafeContext.Parallel.OpenSafeContextBoundary()' to create a safe context boundary.");

      await Verifier.VerifyAnalyzerAsync(input, true, diagnosticFor, diagnosticForEach, diagnosticForEachAsync, diagnosticInvoke);
    }

    [Test]
    public async Task SafeContext_NewThread_Diagnostic ()
    {
      const string input = @"
using System.Threading;
using Remotion.Context;
public class A
{
  public void Test ()
  { 
    SafeContext.Instance.SetData(""a"", 3);
    new Thread(() => SafeContext.Instance.GetData(""a""));
  }
}
";
      var diagnosticFunc = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(9, 5, 9, 56)
          .WithMessage(
              "'new System.Threading.Thread' should not be used with SafeContext, use 'Remotion.Context.SafeContext.Thread.New' instead.");

      await Verifier.VerifyAnalyzerAsync(input, true, diagnosticFunc);
    }

    [Test]
    public async Task SafeContext_NewThreadingTimer_Diagnostic ()
    {
      const string input = @"
using System.Threading;
using Remotion.Context;
public class A
{
  public void Test ()
  { 
    SafeContext.Instance.SetData(""a"", 3);
    var timer = new Timer(new TimerCallback(_ => { }));
  }
}
";
      var diagnosticFunc = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(9, 17, 9, 55)
          .WithMessage(
              "'new System.Threading.Timer' should not be used with SafeContext, use 'Remotion.Context.SafeContext.Threading.NewTimer' instead.");

      await Verifier.VerifyAnalyzerAsync(input, true, diagnosticFunc);
    }

    [Test]
    public async Task SafeContext_TimersTimerElapsed_Diagnostic ()
    {
      const string input = @"
using System.Timers;
using Remotion.Context;
using System.Threading.Tasks;
public class A
{
  public void Test ()
  { 
    SafeContext.Instance.SetData(""a"", 3);
    var timer = new Timer(1000);
    timer.Elapsed += async (sender, e) => { await Task.CompletedTask; };
  }
}
";
      var diagnosticFunc = Verifier.Diagnostic(Analyzer.AlternativeDescriptor)
          .WithSpan(11, 5, 11, 72)
          .WithMessage(
              "'System.Timers.Timer.add_Elapsed' should not be used with SafeContext, use 'Remotion.Context.SafeContext.Timers.AddElapsedEventHandler' instead.");

      await Verifier.VerifyAnalyzerAsync(input, true, diagnosticFunc);
    }
  }
}
