<%@ Page Language="C#" Inherits="OnlineRepository.FireEffect" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>Fire</title>
	<style type="text/css">
    	p{text-align:center;font-variants: small-caps; font-name=Lucida Console; color:white;}
    </style>
</head>
<body id="PageBody" bgcolor="#1A0900" runat="server">
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server" id="ScriptManager1">
	</asp:ScriptManager>
		<asp:UpdatePanel runat="server" id="UpdatePanel1">
			<ContentTemplate>
			<p>
				<asp:Button id="btRandomPalette" runat="server" Text="Random palette" OnClick="btRandomPalette_Click" Width="150" /><br/>
				<asp:Button id="btDefaultPalette" runat="server" Text="Default palette" OnClick="btDefaultPalette_Click" Width="150" /><br/><br/>
				<asp:Timer runat="server" id="Timer" Interval="500" OnTick="Timer_Tick"></asp:Timer>
			    <asp:Label runat="server" Text="Any image yet generated" id="InfoLabel" font-size="small" font-name="verdana" ></asp:Label><br/>
				<asp:Image runat="server" id="UImage" ImageAlign="AbsMiddle"/>
			</p>
			</ContentTemplate>
		</asp:UpdatePanel>
	</form>
</body>
</html>
