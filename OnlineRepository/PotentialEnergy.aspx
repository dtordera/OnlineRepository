<%@ Page Language="C#" Inherits="OnlineRepository.PotentialEnergy" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>PotentialEnergy</title>
	<style type="text/css">
    	p{text-align:center;font-variants: small-caps; font-name=Lucida Console; color:white;}
    </style>
</head>
<body bgcolor="#00000F" >
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server" id="ScriptManager1">
	</asp:ScriptManager>
	<p>Please enter values for width, height, and number of charges</p>
	<table align="center" >
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="tbWidth" Text="WIDTH" ForeColor="white" Font-size="small" /></td>
	<td><asp:TextBox id="tbWidth" runat="server" Text="1000" Columns="6" Style="width:150pts;text-align: right" ToolTip="width" /></td>
	</tr>
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="tbHeight" Text="HEIGHT" ForeColor="white" Font-size="small" /></td>
	<td><asp:TextBox id="tbHeight" runat="server" Text="300" Columns="6" Style="width:120pts;text-align: right" ToolTip="height" /></td>
	</tr>
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="tbCharges" Text="CHARGES" ForeColor="white" Font-size="small" /></td>
	<td><asp:TextBox id="tbCharges"  runat="server" Text="5" Columns="6" Style="width:120pts;text-align: right" ToolTip="charges" /></td>
	</tr>
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="tbK" Text="KONSTANT" ForeColor="white" Font-size="small" /></td>
	<td><asp:TextBox id="tbK"  runat="server" Text="3000" Columns="6" Style="width:120pts;text-align: right" ToolTip="charges" /></td>
	</tr>
	<tr>
	<td><asp:Label runat="server" AssociatedControlID="tbMaxCharge" Text="MAXIMUM CHARGE" ForeColor="white" Font-size="small" /></td>
	<td><asp:TextBox id="tbMaxCharge"  runat="server" Text="3000" Columns="6" Style="width:120pts;text-align: right" ToolTip="charges" /></td>
	</tr>
	</table>
	<asp:UpdatePanel runat="server" id="UpdatePanel1">
		<ContentTemplate>
			<p>
			<asp:Button id="btGenerate" runat="server" Text="Generate field" OnClick="btGenerate_Click" /><br/><br/>
			<asp:Label runat="server" Text="Any image yet generated" id="InfoLabel" font-size="small" font-name="verdana" ></asp:Label><br/>
			<asp:Image runat="server" id="UImage" ImageAlign="AbsMiddle"/>
			</p>
		</ContentTemplate>
	</asp:UpdatePanel>
	</form>
</body>
</html>

