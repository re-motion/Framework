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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/TestPage.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TestPage"%>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="CoreForms.Web.Extensions, Version=7.0.0.0, Culture=neutral, PublicKeyToken=4775604ccf360c09" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI.WebControls" Assembly="CoreForms.Web, Version=7.0.0.0, Culture=neutral, PublicKeyToken=4775604ccf360c09" %>

<html>
<head runat="server">
    <title>TestPage</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                  InnerResult: <asp:Label ID="InnerResult" runat="server" Text="0" />
                  <br />
                  <asp:Button ID="UpdateButton" runat="server" Text="Update This Panel" OnClick="UpdateButton_OnClick"  />
                    <asp:Textbox runat="server" Text="test1" AutoPostBack="True"/>
                    <asp:Textbox runat="server" Text="test2" AutoPostBack="True"/>
                </ContentTemplate>
            </asp:UpdatePanel>
            OuterResult: <asp:Label ID="OuterResult" runat="server" Text="0" />
        </div>
    </form>
</body>
</html>