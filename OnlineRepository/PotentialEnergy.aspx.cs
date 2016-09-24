using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Web.UI.WebControls;

namespace OnlineRepository
{
	public partial class PotentialEnergy : System.Web.UI.Page
	{
		protected float K; // Multiplicative factor
		protected int MAXQ; // Maximum value of a charge * 2

		// Charge definition
		protected class Charge
		{
			public int x;
			public int y;
			public float v;
		}

		// Potential of a specific point x,y derived from charges Q
		protected float V(int x, int y,Charge[] Q)
		{
			float v = 0;

			// So easy as run all charges and sum each q potential contribution
			foreach (Charge q in Q) 
			{
				// Vq(x,y) = q/r2
				try
				{
					v += K*q.v / ((q.x-x)*(q.x-x) + (q.y - y)*(q.y-y));
				}
				catch(Exception)
				{
					// when r2 = 0, so position of same charge	
				}
			}
			return (float)v;
		}

		public void btGenerate_Click (object sender, EventArgs args)
		{
			// Counter
			Stopwatch sw = Stopwatch.StartNew();
			Random r = new Random ();

			// get parameters
			int w = Int32.Parse (tbWidth.Text),
				h = Int32.Parse (tbHeight.Text);

			K = Int32.Parse (tbK.Text);
			MAXQ = Int32.Parse (tbMaxCharge.Text);

			// Create charges and fill them. Values must be negatives & positives
			Charge[] Q = new Charge[Int32.Parse (tbCharges.Text)];

			for (int i = 0; i < Q.Length; i++)
			{
				Q[i] = new Charge ();
				Q[i].x = r.Next () % w;
				Q[i].y = r.Next () % h;
				Q[i].v = (r.Next () % MAXQ) - MAXQ / 2;
			}

			// Create bitmap, and fill it 
			Bitmap B = new Bitmap (w, h,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int x, y;
			byte c; // using byte so this way got automatic modulus 0xff for color
		
			for (x = 0; x < w; x++)
				for (y = 0; y < h; y++) 
				{
					c = (byte)V (x, y, Q) ;
					B.SetPixel (x, y, Color.FromArgb (c,c,c));
				}
			
			sw.Stop ();
			InfoLabel.Text = "Bitmap generated on " + sw.ElapsedMilliseconds.ToString () + "ms";

			// Lets copy it in a stream and make page to load it encoded as base64
			sw.Start ();
			using (MemoryStream m = new MemoryStream ()) 
			{
				// Saving to .ImageFormat.Gif does a nice 90's effect :)
				B.Save (m, System.Drawing.Imaging.ImageFormat.Jpeg);
				UImage.ImageUrl = "data:image/png;base64," + Convert.ToBase64String (m.ToArray ());
			}
			sw.Stop ();
			InfoLabel.Text += " / total ellapsed time : " + sw.ElapsedMilliseconds.ToString() + "ms";
		}
	}
}

