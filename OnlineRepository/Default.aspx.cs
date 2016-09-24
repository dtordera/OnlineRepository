using System;
using System.Web;
using System.Web.UI;

namespace OnlineRepository
{
	
	public partial class Default : System.Web.UI.Page
	{
		public void Redirect_Click (object sender, EventArgs args)
		{
			Response.Redirect (((System.Web.UI.WebControls.Button)sender).ID+".aspx");
		}
	}
}

