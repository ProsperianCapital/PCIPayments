<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RTR.aspx.cs" Inherits="PCIWeb.RTR" %>

<!DOCTYPE html>

<html>
<head>
<title>Prosperian Capital</title>
<link rel="stylesheet" href="Payment.css" type="text/css" />
</head>
<body>
<script type="text/javascript">
function Busy(show,msg)
{
	try
	{
		var h = document.getElementById('divBusy');
		if ( show > 0 )
		{
			h.style.visibility = "visible";
			h.style.display    = "";
			h = document.getElementById('msgBusy');
			if ( msg != null )
				h.innerHTML = msg;
		}
		else
		{
			h.style.visibility = "hidden";
			h.style.display    = "none";
		}
	}
	catch (x)
	{ }
}
</script>
<a href="http://prosperian.mu" target="P"><img src="LogoProsperian.png" title="Prosperian Capital" /></a>
<form runat="server" id="frmRTR">
	<p class="Header">
	Prosperian Capital : RTR Payments
	</p>
	<table class="BoxRight">
	<tr>
		<td>SQL Server</td><td> : <b><i><asp:Literal runat="server" ID="lblSQLServer"></asp:Literal></i></b></td></tr>
	<tr>
		<td>SQL Database</td><td> : <b><i><asp:Literal runat="server" ID="lblSQLDB"></asp:Literal></i></b></td></tr>
	<tr>
		<td>SQL User</td><td> : <b><i><asp:Literal runat="server" ID="lblSQLUser"></asp:Literal></i></b></td></tr>
	<tr>
		<td>SQL Status</td><td> : <b><i><asp:Literal runat="server" ID="lblSQLStatus"></asp:Literal></i></td></tr>
	<tr>
		<td colspan="2"><hr /></td></tr>
	<tr>
		<td>System Mode</td><td class="Red"> : <b><i><asp:Literal runat="server" ID="lblSMode"></asp:Literal></i></b></td></tr>
	<tr>
		<td>System Status</td><td class="Red"> : <b><i><asp:Literal runat="server" ID="lblSStatus"></asp:Literal></i></b></td></tr>
	</table>
	<table class="Detail">
	<tr>
		<td>Payment Provider</td>
		<td> :
			<asp:DropDownList runat="server" id="lstProvider" AutoPostBack="true">
				<asp:ListItem Value="016" Text="PayU" Selected="True"></asp:ListItem>
				<asp:ListItem Value="203" Text="Ikajo"></asp:ListItem>
				<asp:ListItem Value="006" Text="Transact24"></asp:ListItem>
				<asp:ListItem Value="567" Text="MyGate"></asp:ListItem>
			</asp:DropDownList></td></tr>
	<tr>
		<td>Bureau Code</td>
		<td> : <asp:Literal runat="server" ID="lblBureauCode"></asp:Literal></td></tr>
	<tr>
		<td>Payment URL</td>
		<td> : <asp:Literal runat="server" ID="lblBureauURL"></asp:Literal></td></tr>
	<tr>
		<td>Prosperian Account/Key</td>
		<td> : <asp:Literal runat="server" ID="lblMerchantKey"></asp:Literal></td></tr>
	<tr>
		<td>Prosperian User ID</td>
		<td> : <asp:Literal runat="server" ID="lblMerchantUser"></asp:Literal></td></tr>
	<tr>
		<td>Cards waiting to be tokenized</td>
		<td> : <asp:Literal runat="server" ID="lblCards"></asp:Literal></td></tr>
	<tr>
		<td>Payments waiting to be processed</td>
		<td> : <asp:Literal runat="server" ID="lblPayments"></asp:Literal></td></tr>
	<tr>
		<td>Status</td>
		<td> : <asp:Literal runat="server" ID="lblBureauStatus"></asp:Literal></td></tr>
	</table>
	<p class="ButtonRow">
	<asp:Button  runat="server" ID="btnProcess1" CssClass="Button" OnClientClick="JavaScript:Busy(1,'Retrieving cards ... please be patient')" onclick="btnProcess1_Click" Text="Get Tokens" />&nbsp;
	<asp:Button  runat="server" ID="btnProcess2" CssClass="Button" OnClientClick="JavaScript:Busy(1,'Retrieving payments ... please be patient')" onclick="btnProcess2_Click" Text="Process Payments" />&nbsp;
	&nbsp;&nbsp;
	<asp:Button  runat="server" ID="btnConfig"   CssClass="Button" OnClientClick="JavaScript:Busy(1)" onclick="btnConfig_Click" Text="Config ..." />
	<asp:Button  runat="server" ID="btnInfo"     CssClass="Button" OnClientClick="JavaScript:Busy(1)" onclick="btnInfo_Click"   Text="Info Log ..." />
	<asp:Button  runat="server" ID="btnError"    CssClass="Button" OnClientClick="JavaScript:Busy(1)" onclick="btnError_Click"  Text="Error Log ..." />
	<asp:Button  runat="server" ID="btnSQL"      CssClass="Button" OnClientClick="JavaScript:Busy(1)" onclick="btnSQL_Click"    Text="Test SQL ..." />
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
<div id="divBusy" style="left:10px;top:20px;position:fixed;border:1px solid #000000;padding:5px;background-color:aquamarine">
	<table style="background-color:aquamarine">
		<tr>
			<td><img src="Busy.gif" title="Busy ..." /></td>
			<td id="msgBusy" style="font-size:18px;font-style:italic;color:red;font-weight:bold">Busy ... please wait</td></tr>
	</table>
</div>
<script type="text/javascript">
Busy(0);
</script>
</body>
</html>
