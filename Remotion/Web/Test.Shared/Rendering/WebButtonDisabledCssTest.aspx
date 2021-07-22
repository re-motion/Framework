<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebButtonDisabledCssTest.aspx.cs" Inherits="Remotion.Web.Test.Rendering.WebButtonDisabledCssTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>

  <style>
    .testClass {
      margin: 1em;
      background-color: yellow;
    }

    .disabled {
      color: red;
    }

    .aspNetDisabled
    {
      font-style: italic;
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
    <div>
      derived button with css style set. <br />
      expected:
      <button type="submit" value="Enabled" class="testClass"><span class="buttonBody">Enabled</span></button>
      <br />

      actual:
      <remotion:WebButtonWithDisabledCssStyleSupport runat="server" Text="Enabled" CssClass="testClass" />
      <br />
      <br />

      derived button without style set. <br />
      expected:
      <button type="submit" value="Enabled" class="webButton"><span class="buttonBody">Enabled</span></button>
      <br />

      actual:
      <remotion:WebButtonWithDisabledCssStyleSupport ID="WebButtonWithDisabledCssStyleSupport5" runat="server" Text="Enabled" />
      <br />
      <br />
      
      derived button with css style and disabled. <br />
      expected:
      <button type="submit" value="Enabled" class="testClass aspNetDisabled" disabled="disabled" ><span class="buttonBody">Enabled</span></button>
      <br />

      actual:
      <remotion:WebButtonWithDisabledCssStyleSupport ID="WebButtonWithDisabledCssStyleSupport1" runat="server" Text="Enabled" CssClass="testClass" Enabled="False" />
      <br />
      <br />
      
      derived button without css style and disabled from inheritance. <br />
      expected:
      <button type="submit" value="Enabled" class="webButton disabled aspNetDisabled" disabled="disabled" ><span class="buttonBody">Enabled</span></button>
      <br />

      actual:
      <remotion:WebButtonWithDisabledCssStyleSupport runat="server" Text="Enabled" Enabled="False" />
      <br />
      <br />

    </div>
  </form>
</body>
</html>
