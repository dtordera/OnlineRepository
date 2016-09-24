<%@ Page Language="C#" Inherits="OnlineRepository.RaisingHighMaps" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>Raising Highmaps</title>
	<style type="text/css">
    	p{text-align:center;font-variants: small-caps; font-name=Lucida Console; color:white;}
    </style>
</head>
<body bgcolor="#00000F" >
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server" id="ScriptManager1">
	</asp:ScriptManager>
	<p>RAISING HIGHMAPS</p>
	<table align="center" >
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="tbEntrophy" Text="Entrophy (0-1 for controled high limits):" ForeColor="white" Font-size="small" /></td>
	<td><asp:TextBox id="tbEntrophy" runat="server" Text="1" Columns="6" Style="width:150pts;text-align: right" ToolTip="Entrophy (bigger, more disorded &amp; bumped)" /></td>
	</tr>
	</table>
	<asp:UpdatePanel runat="server" id="UpdatePanel1">
		<ContentTemplate>
			<p>
			<asp:Button id="btGenerate" runat="server" Text="Generate field" OnClick="btGenerate_Click" /><br/>
			<asp:Label runat="server" Text="Any image yet generated" id="InfoLabel" font-size="small" font-name="verdana" ></asp:Label><br/>
			<asp:Image runat="server" id="UHighMap" ImageAlign="Middle" Width="257" Height="257" BorderStyle="None" />
			<asp:Image runat="server" id="URender" ImageAlign="Middle" Width="1000" Height="550" BorderStyle="None" />
			</p>
		</ContentTemplate>
	</asp:UpdatePanel>
	</form>
</body>
</html>
