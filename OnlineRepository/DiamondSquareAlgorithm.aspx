<%@ Page Language="C#" Inherits="OnlineRepository.DiamondSquareAlgorithm" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>DiamondSquareAlgorithm</title>
	<style type="text/css">
    	p{text-align:center;font-variants: small-caps; font-name=Lucida Console; color:white;}
    </style>
</head>
<body bgcolor="#00000F" >
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server" id="ScriptManager1">
	</asp:ScriptManager>
	<p>DIAMONDSQUARE ALGORITHM</p>
	<table align="center" >
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="tbEntrophy" Text="Entrophy (0-1 for controled high limits):" ForeColor="white" Font-size="small" /></td>
	<td><asp:TextBox id="tbEntrophy" runat="server" Text="1" Columns="6" Style="width:150pts;text-align: right" ToolTip="Entrophy (bigger, more disorded &amp; bumped)" /></td>
	</tr>
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="ddPalette" Text="Applied palette" ForeColor="white" Font-size="small" /></td>
	<td><asp:DropDownList id="ddPalette" runat="server" >
		<asp:ListItem Text="Gray module" Value="0" Selected="true"></asp:ListItem>
		<asp:ListItem Text="Earth" Value="1" Selected="false"></asp:ListItem>
		<asp:ListItem Text="Sky" Value="2" Selected="false"></asp:ListItem>		
		<asp:ListItem Text="Volcano" Value="3" Selected="false"></asp:ListItem>
		<asp:ListItem Text="Storm" Value="4" Selected="false"></asp:ListItem>
		</asp:DropDownList> </td>
	</tr>
	</table>
	<asp:UpdatePanel runat="server" id="UpdatePanel1">
		<ContentTemplate>
			<p>
			<asp:Button id="btGenerate" runat="server" Text="Generate field" OnClick="btGenerate_Click" /><br/>
			<asp:Label runat="server" Text="Any image yet generated" id="InfoLabel" font-size="small" font-name="verdana" ></asp:Label><br/>
			<asp:Image runat="server" id="UHighMap" ImageAlign="AbsMiddle" Width="513" Height="513" BorderStyle="None" />
			</p>
		</ContentTemplate>
	</asp:UpdatePanel>
	</form>
</body>
</html>
