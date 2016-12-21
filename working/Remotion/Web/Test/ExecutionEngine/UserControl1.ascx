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
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="UserControl1.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.UserControl1" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<DIV style="WIDTH: 464px; POSITION: relative; HEIGHT: 128px" ms_positioning="GridLayout">&nbsp;
  <asp:TextBox id="TextBox1" style="Z-INDEX: 101; LEFT: 40px; POSITION: absolute; TOP: 32px" runat="server"></asp:TextBox>
  <asp:Button id="Stay" style="Z-INDEX: 102; LEFT: 32px; POSITION: absolute; TOP: 88px" runat="server"
    Text="Stay"></asp:Button>
  <asp:Button id="Sub" style="Z-INDEX: 103; LEFT: 104px; POSITION: absolute; TOP: 88px" runat="server"
    Text="Sub"></asp:Button>
  <asp:Button id="Next" style="Z-INDEX: 104; LEFT: 168px; POSITION: absolute; TOP: 88px" runat="server"
    Text="Next"></asp:Button>
  <asp:Label id="Label1" style="Z-INDEX: 105; LEFT: 280px; POSITION: absolute; TOP: 48px" runat="server"
    Width="112px">Label</asp:Label></DIV>
