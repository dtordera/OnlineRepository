<%@ Page Language="C#" Inherits="OnlineRepository.Pixelation" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>Image pixelation</title>
	<style type="text/css">
    	p{text-align:center;font-variants: small-caps; font-name=Lucida Console; color:white;}
    </style>
</head>
<body id="PageBody" bgcolor="#380E00" runat="server">
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server" id="ScriptManager1">
	</asp:ScriptManager>
		<asp:UpdatePanel runat="server" id="UpdatePanel1">
			<ContentTemplate>
			<p>
				<asp:Timer runat="server" id="Timer" Interval="500" OnTick="Timer_Tick"></asp:Timer>
				<asp:Button id="btLoadImage" runat="server" Text="Load Image" OnClick="btLoadImage_Click" Width="150" /><br/>
				<asp:Button id="btPixelateIt" runat="server" Text="Pixelate it!" OnClick="btPixelateIt_Click" Width="150" /><br/>
				<asp:Button id="btFadeIn" runat="server" Text="Fade in" OnClick="btFadeIn_Click" Width="150" /><br/><br/>
				<asp:Label runat="server" Text="Any image yet generated" id="InfoLabel" font-size="small" font-name="verdana" ></asp:Label><br/>
				<asp:Image runat="server" id="UImage" ImageAlign="AbsMiddle" Width="400px" Height="300px" BorderStyle="Dotted" BorderWidth="1px" BorderColor="White" AlternateText="Image to pixelate" />
			</p>
			</ContentTemplate>
		</asp:UpdatePanel>
	</form>
</body>
</html>