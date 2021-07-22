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
<%@ Register TagPrefix="uc1" TagName="UserControl1" Src="UserControl1.ascx" %>
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Web.Test.ExecutionEngine.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>WebForm1</title>
    <remotion:htmlheadcontents id=HtmlHeadContents runat="server"/>
  </HEAD>
  <body MS_POSITIONING="GridLayout">
    <FORM id="Form1" method="post" runat="server">
      <asp:ScriptManager runat="server" EnablePartialRendering="False"/>
      <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <ContentTemplate>
      <asp:textbox id="TextBox1" style="Z-INDEX: 101; LEFT: 80px; POSITION: absolute; TOP: 48px" runat="server"></asp:textbox>
      <asp:Button id="ThrowText" style="Z-INDEX: 118; LEFT: 128px; POSITION: absolute; TOP: 200px"
        runat="server" Text='Throw ("test")'></asp:Button><asp:label id="Var2Label" style="Z-INDEX: 111; LEFT: 376px; POSITION: absolute; TOP: 56px"
        runat="server" Height="32px" Width="488px"></asp:label><asp:label id="Label5" style="Z-INDEX: 110; LEFT: 288px; POSITION: absolute; TOP: 56px" runat="server">Var2</asp:label><asp:label id="Var1Label" style="Z-INDEX: 104; LEFT: 376px; POSITION: absolute; TOP: 24px"
        runat="server" Height="32px" Width="488px"></asp:label><asp:button id="Stay" style="Z-INDEX: 102; LEFT: 40px; POSITION: absolute; TOP: 144px" runat="server"
        Text="Stay"></asp:button><asp:button id="Next" style="Z-INDEX: 103; LEFT: 216px; POSITION: absolute; TOP: 144px" runat="server"
        Text="Next"></asp:button>
      <DIV style="WIDTH: 16px; HEIGHT: 16.5em"></DIV>
      <asp:Calendar id="Calendar1" style="Z-INDEX: 119; LEFT: 912px; POSITION: absolute; TOP: 128px"
        runat="server"></asp:Calendar>
      <asp:TextBox id="SubNoReturnField" style="Z-INDEX: 117; LEFT: 904px; POSITION: absolute; TOP: 72px"
        runat="server" Width="248px" AutoPostBack="True">change text for SubNoReturn</asp:TextBox>
      <asp:Button id="SubNoReturnButton" style="Z-INDEX: 116; LEFT: 904px; POSITION: absolute; TOP: 32px"
        runat="server" Text="SubNoReturn"></asp:Button>
      <asp:Button id="SubExtButton" style="Z-INDEX: 115; LEFT: 136px; POSITION: absolute; TOP: 144px"
        runat="server" Text="SubExt"></asp:Button>
      <asp:Label id="RetValLabel" style="Z-INDEX: 114; LEFT: 376px; POSITION: absolute; TOP: 88px"
        runat="server" Width="488px"></asp:Label>
      <asp:Label id="Label1" style="Z-INDEX: 113; LEFT: 288px; POSITION: absolute; TOP: 88px" runat="server">RetVal</asp:Label>
      <uc1:UserControl1 id="UserControl11" runat="server"></uc1:UserControl1>
      <asp:Button id="Throw" style="Z-INDEX: 112; LEFT: 56px; POSITION: absolute; TOP: 200px" runat="server"
        Text="Throw"></asp:Button>
      <asp:label id="Label3" style="Z-INDEX: 109; LEFT: 280px; POSITION: absolute; TOP: 128px" runat="server">Stack</asp:label><asp:label id="Label2" style="Z-INDEX: 108; LEFT: 288px; POSITION: absolute; TOP: 24px" runat="server">Var1</asp:label><asp:label id="StackLabel" style="Z-INDEX: 107; LEFT: 376px; POSITION: absolute; TOP: 128px"
        runat="server" Height="168px" Width="480px"></asp:label><asp:button id="Sub" style="Z-INDEX: 106; LEFT: 96px; POSITION: absolute; TOP: 144px" runat="server"
        Text="Sub"></asp:button><asp:checkbox id="IsPostBackCheck" style="Z-INDEX: 105; LEFT: 88px; POSITION: absolute; TOP: 96px"
        runat="server" Text="IsPostBack" Enabled="False"></asp:checkbox>
        </ContentTemplate>
      </asp:UpdatePanel>
    </FORM>
  </body>
</HTML>
