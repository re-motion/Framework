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
using System.Web;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  public class RendererTestBase
  {
    protected HttpContextBase HttpContext { get; private set; }
    protected HtmlHelper Html { get; private set; }

    protected IGlobalizationService GlobalizationService
    {
      get
      {
        return new CompoundGlobalizationService (
            new[] { new GlobalizationService (new ResourceManagerResolver (new ResourceAttributeBasedResourceManagerFactory())) });
      }
    }

    protected RendererTestBase ()
    {
    }

    protected virtual void Initialize ()
    {
      Html = new HtmlHelper();

      HttpContext = MockRepository.GenerateStub<HttpContextBase>();
    }
  }
}