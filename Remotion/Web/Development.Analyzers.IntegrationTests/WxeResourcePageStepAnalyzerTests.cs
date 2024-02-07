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
using System.Threading.Tasks;
using NUnit.Framework;
using Verifier = Remotion.Web.Development.Analyzers.IntegrationTests.CSharpAnalyzerVerifier<Remotion.Web.Development.Analyzers.WxeResourcePageStepAnalyzer>;

namespace Remotion.Web.Development.Analyzers.IntegrationTests
{
  [TestFixture]
  public class WxeResourcePageStepAnalyzerTests
  {
    [Test]
    public async Task TypeFromOtherAssembly_Invalid ()
    {
      const string input = @"using System.Text;
using Remotion.Web.ExecutionEngine;
public class A
{
  public void Test()
  {
    new WxeResourcePageStep(typeof(int), ""Test.aspx"");
    new WxeResourceUserControlStep(typeof(float), ""Test.aspx"");
  }
  
  public WxeResourcePageStep Step => new(typeof(string), ""Test.aspx"");

  public WxeResourceUserControlStep UserStep => new(typeof(double), ""Test.aspx"");
}";

      var diagnostic1 = Verifier.Diagnostic(WxeResourcePageStepAnalyzer.RMWEB0003DiagnosticDescriptor)
          .WithSpan(7, 5, 7, 54)
          .WithMessage("Type 'int' is not defined in the same assembly as the containing type 'A'. If this is intentional, you can suppress the warning.");
      var diagnostic2 = Verifier.Diagnostic(WxeResourcePageStepAnalyzer.RMWEB0003DiagnosticDescriptor)
          .WithSpan(8, 5, 8, 63)
          .WithMessage("Type 'float' is not defined in the same assembly as the containing type 'A'. If this is intentional, you can suppress the warning.");
      var diagnostic3 = Verifier.Diagnostic(WxeResourcePageStepAnalyzer.RMWEB0003DiagnosticDescriptor)
          .WithSpan(11, 38, 11, 70)
          .WithMessage("Type 'string' is not defined in the same assembly as the containing type 'A'. If this is intentional, you can suppress the warning.");
      var diagnostic4 = Verifier.Diagnostic(WxeResourcePageStepAnalyzer.RMWEB0003DiagnosticDescriptor)
          .WithSpan(13, 49, 13, 81)
          .WithMessage("Type 'double' is not defined in the same assembly as the containing type 'A'. If this is intentional, you can suppress the warning.");

      await Verifier.VerifyAnalyzerAsync(input, diagnostic1, diagnostic2, diagnostic3, diagnostic4);
    }

    [Test]
    public async Task TypeFromSameAssembly_Valid ()
    {
      const string input = @"using System.Text;
using Remotion.Web.ExecutionEngine;
public class A
{
  public void Test()
  {
    new WxeResourcePageStep(typeof(A), ""Test.aspx"");
    new WxeResourceUserControlStep(typeof(A), ""Test.aspx"");
  }
  
  public WxeResourcePageStep Step => new(typeof(A), ""Test.aspx"");
  
  public WxeResourceUserControlStep UserStep => new(typeof(A), ""Test.aspx"");
}";

      await Verifier.VerifyAnalyzerAsync(input);
    }

    [Test]
    public async Task NonTypeofArgument_Invalid ()
    {
      const string input = @"using System.Text;
using Remotion.Web.ExecutionEngine;
public class A
{
  public void Test()
  {
    var type = typeof(int);
    new WxeResourcePageStep(type, ""Test.aspx"");
  }
  
  public WxeResourcePageStep Step
  {
    get
    {
      var type = typeof(string);
      return new(type, ""Test.aspx"");
    }
  }
}";

      var diagnostic1 = Verifier.Diagnostic(WxeResourcePageStepAnalyzer.RMWEB0004DiagnosticDescriptor)
          .WithSpan(8, 5, 8, 47);
      var diagnostic2 = Verifier.Diagnostic(WxeResourcePageStepAnalyzer.RMWEB0004DiagnosticDescriptor)
          .WithSpan(16, 14, 16, 36);

      await Verifier.VerifyAnalyzerAsync(input, diagnostic1, diagnostic2);
    }

    [Test]
    public async Task AssemblyArgument_Valid ()
    {
      const string input = @"using System.Text;
using Remotion.Web.ExecutionEngine;
public class A
{
  public void Test()
  {
    var type = typeof(int);
    new WxeResourcePageStep(type.Assembly, ""Test.aspx"");
  }
  
  
  public WxeResourcePageStep Step
  {
    get
    {
      var type = typeof(string);
      return new(type.Assembly, ""Test.aspx"");
    }
  }
}";

      await Verifier.VerifyAnalyzerAsync(input);
    }
  }
}
