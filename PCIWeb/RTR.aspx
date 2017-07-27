<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RTR.aspx.cs" Inherits="PCIWeb.RTR" %>

<!DOCTYPE html>

<html>
<head>
<title>Prosperian Capital</title>
<link rel="stylesheet" href="Payment.css" type="text/css" />
</head>
<body>
<a href="http://prosperian.mu" target="P"><img src="LogoProsperian.png" title="Prosperian Capital" /></a>
<form runat="server" id="frmRTR">
	<p class="Header">
	Prosperian Capital : RTR Payments
	</p>
	<p class="Detail">
	Choose a payment provider :&nbsp;
	<asp:DropDownList runat="server" id="lstProvider">
		<asp:ListItem Value="016" Text="PayU" Selected="True"></asp:ListItem>
		<asp:ListItem Value="203" Text="Ikajo"></asp:ListItem>
		<asp:ListItem Value="034" Text="Transact24"></asp:ListItem>
	</asp:DropDownList>
	<br /><br />
	<asp:Button runat="server" ID="btnProcess2" Text="Payments (Test SQL)" CssClass="Button" onclick="btnProcess2_Click" />&nbsp;
	<asp:Button runat="server" ID="btnProcess3" Text="Payments (S/Proc SQL)" CssClass="Button" onclick="btnProcess3_Click" />&nbsp;
	<asp:Button runat="server" ID="btnTest" Text="Show Config" CssClass="Button" onclick="btnConfig_Click" />
    <p>
    <asp:Literal runat="server" ID="lblConfig"></asp:Literal>
    <asp:Label runat="server" ID="lblError" CssClass="Error"></asp:Label>
	</p>
	<hr />
	<p class="Footer">
	&nbsp;Phone +230 404 8000&nbsp; | &nbsp;Email <a href="mailto:info@prosperian.mu">Info@prosperian.mu</a>
	<span style="float:right;margin-right:3px"><asp:Literal runat="server" ID="lblVersion"></asp:Literal></span>
	</p>
</form>
</body>
</html>
