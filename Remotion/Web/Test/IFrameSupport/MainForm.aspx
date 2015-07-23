<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainForm.aspx.cs" Inherits="Remotion.Web.Test.IFrameSupport.MainForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%">
<head runat="server">
  <title></title>
  <script type="text/javascript">
    function Refresh() {
      window.__doPostBack("RefreshButton", "");
    }
  </script>
</head>
<body style="height: 95%">
  <form id="form1" runat="server" style="height: 100%">
    <input type="hidden" name="MainScriptManager_HiddenField1" value=";;AjaxControlToolkit, Version=3.5.40412.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e:de-AT:1547e793-5b7e-48fe-8490-03a375b13a33:de1feab2:f9cec9bc:a67c2700:f2c8e708:8613aea7:3202a5a2:ab09e3fe:87104b7c:be6fb298"/>
    <asp:ScriptManager runat="server" ID="ScriptManager" EnablePartialRendering="True" />
    <div style="float: left; width: 30%; height: 100%; border-right: 1px solid black">
      left<br />
      <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
          <asp:Button runat="server" ID="RefreshButton" />
          <br/>
          <%= DateTime.Now.ToLongTimeString() %>
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
    <div style="margin-left: 31%; height: 100%;">
      <iframe src="FrameContent.wxe" style="height: 100%; width: 100%"></iframe>
    </div>
  </form>
</body>
</html>
