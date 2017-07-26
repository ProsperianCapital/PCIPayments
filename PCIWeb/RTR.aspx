<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RTR.aspx.cs" Inherits="PCIWeb.RTR" %>

<!DOCTYPE html>

<html>
<head>
<title>Prosperian Capital</title>
<link rel="stylesheet" href="Payment.css" type="text/css" />
</head>
<body>
<a href="http://prosperian.mu" target="P"><img src="Images/LogoProsperian.png" title="Prosperian Capital" /></a>
<form runat="server" id="frmRTR">
	<p class="Header">
	Prosperian Capital : RTR Payments
	</p>
	<p class="Detail">
	Choose a payment provider :&nbsp;
	<asp:DropDownList runat="server" id="lstProvider">
		<asp:ListItem Value="012" Text="PayU" Selected="True"></asp:ListItem>
		<asp:ListItem Value="203" Text="Ikajo"></asp:ListItem>
		<asp:ListItem Value="034" Text="Transact24"></asp:ListItem>
	</asp:DropDownList>
	<br /><br />
	<asp:Button runat="server" ID="btnProcess" Text="Process Payment(s)" CssClass="Button" onclick="btnProcess_Click" />
	</p>
	<hr />
	<p class="Footer">
	&nbsp;Phone +230 404 8000&nbsp; | &nbsp;Email <a href="mailto:info@prosperian.mu">Info@prosperian.mu</a>
	</p>
</form>
</body>
</html>
