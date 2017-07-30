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
	<div class="BoxRight">
		<asp:Literal runat="server" ID="lblProvider"></asp:Literal>
	</div>
	<p class="Detail">
	SQL Server : <b><i><asp:Literal runat="server" ID="lblSQLServer"></asp:Literal></i></b><br />
	SQL Database : <b><i><asp:Literal runat="server" ID="lblSQLDB"></asp:Literal></i></b><br />
	SQL User : <b><i><asp:Literal runat="server" ID="lblSQLUser"></asp:Literal></i></b><br />
	SQL Status : <b><i><asp:Literal runat="server" ID="lblSQLStatus"></asp:Literal></i></b>
	</p>
	<p class="Detail">
	Choose a payment provider :&nbsp;
	<asp:DropDownList runat="server" id="lstProvider" AutoPostBack="true">
		<asp:ListItem Value="016" Text="PayU" Selected="True"></asp:ListItem>
		<asp:ListItem Value="203" Text="Ikajo"></asp:ListItem>
		<asp:ListItem Value="034" Text="Transact24"></asp:ListItem>
	</asp:DropDownList>
	<br /><br />
	<asp:Button  runat="server" ID="btnProcess1" CssClass="Button" onclick="btnProcess1_Click" Text="Get Tokens" />&nbsp;
	<asp:Button  runat="server" ID="btnProcess2" CssClass="Button" onclick="btnProcess2_Click" Text="Process Payments" />&nbsp;
	<asp:Button  runat="server" ID="btnTest1"    CssClass="Button" onclick="btnConfig_Click"   Text="Show Config" Visible="false" />
	<asp:Button  runat="server" ID="btnTest2"    CssClass="Button" onclick="btnSQL_Click"      Text="Test SQL ..." />
	<asp:TextBox runat="server" ID="txtTest" Width="560px"></asp:TextBox>
	</p>
	<hr />
	<asp:Literal runat="server" ID="lblTest"></asp:Literal>
	<asp:Label runat="server" ID="lblError" CssClass="Error"></asp:Label>
	<p class="Footer">
	&nbsp;Phone +230 404 8000&nbsp; | &nbsp;Email <a href="mailto:info@prosperian.mu">Info@prosperian.mu</a>
	<span style="float:right;margin-right:5px"><asp:Literal runat="server" ID="lblVersion"></asp:Literal></span>
	</p>
</form>
</body>
</html>
