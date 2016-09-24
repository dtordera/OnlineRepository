using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineRepository
{
	public partial class PotentialEnergyColored : System.Web.UI.Page
	{
		protected static float K;  // Multiplicative factor
		protected static int MAXQ; // Maximum value of a charge * 2

		protected static byte[] Field; // byte array with calculated V values
		protected static int width, height; // general width & height

		protected static int _MAXCOLORS = 256; // maxcolors (remain in byte)
		protected static Color[] Palette = new Color[_MAXCOLORS]; // working palette

		protected static int _MAXPIVOTCOLORS = 5; // maxpivot colors (to degrade colors between)
		protected static Color[] PivotColors = new Color[_MAXPIVOTCOLORS]; // 

		protected Random r = new Random(); // general random object

		// Charge definition
		protected class Charge
		{
			public int x;
			public int y;
			public float v;
		}

		// main load event. Let's create a palette and a field
		public void Page_Load(object sender, EventArgs args)
		{
			// First time page load, lets generate a first field & a palette  
			if (!Page.IsPostBack) 
			{
				generateField ();
				btRandomPalette_Click (null, null);
			}
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

		// generates a new field 
		public void generateField()
		{
			// get parameters
			width = Int32.Parse (tbWidth.Text);
			height = Int32.Parse (tbHeight.Text);

			K = Int32.Parse (tbK.Text);
			MAXQ = Int32.Parse (tbMaxCharge.Text);

			// Create charges and fill them. Values must be negatives & positives
			Charge[] Q = new Charge[Int32.Parse (tbCharges.Text)];

			for (int i = 0; i < Q.Length; i++)
			{
				Q[i] = new Charge ();
				Q[i].x = r.Next () % width;
				Q[i].y = r.Next () % height;
				Q[i].v = (r.Next () % MAXQ) - MAXQ / 2;
			}

			// create array & fill it with rounded-to-byte values
			Field = new byte[width * height];

			int x, y;
			for (x = 0; x < width; x++)
				for (y = 0; y < height; y++)
					Field [y * width + x] = (byte)V (x, y, Q);
		}

		// Generates gradient between Palette indexed pivot colors
		protected void Gradient (int ao, int af)
		{
			int d = af - ao; // total distance between colors

			float 
			ro = Palette [ao].R, // Initial r,g,b values
			go = Palette [ao].G,
			bo = Palette [ao].B,
			dr = (Palette[af].R - ro)/d, // diferential of r,g,b
			dg = (Palette[af].G - go)/d, 
			db = (Palette[af].B - bo)/d;

			// lets fill each color in palette between range
			for (int i=0;i<d+1;i++)
				Palette [i + ao] = 
					Color.FromArgb (
						(byte)(ro + i*dr), 
						(byte)(go + i*dg), 
						(byte)(bo + i*db)
					);
		}

		// Fill gradients
		protected void FillGradients(Color[] C)
		{
			// search distance between pivot colors in palette 
			float m = Palette.Length / (C.Length);

			// let's tie palette, make it rounded
			Palette [0] = C [0];
			Palette [_MAXCOLORS-1] = C [0];

			// Lets point each choosen color at position and do gradient between them
			for (int i = 1; i < C.Length; i++) 
			{
				Palette [(int)(i * m)] = C [i];
				Gradient ((int)((i-1)* m),(int)(i * m));
			}

			// lets degrade last color index. This line is due float can be not exactly _MAXCOLORS-1
			Gradient ((int)((C.Length-1)*m), _MAXCOLORS - 1);
		}

		// dump field to a 8bpp indexed bitmap 
		protected void DumpToBitmap(Bitmap B, byte[] A)
		{
			Rectangle R = new Rectangle (0, 0, B.Width, B.Height);
			BitmapData b = B.LockBits(R,ImageLockMode.WriteOnly,System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

			IntPtr ptr = b.Scan0;
			Marshal.Copy (A, 0, ptr, B.Width * B.Height);
			B.UnlockBits (b);
		}
			
		// dump working palette to a bitmap palette
		private void DumpPalette(Bitmap B,Color[] C)
		{
			ColorPalette P = B.Palette;

			for (int i = 0; i < C.Length; i++)
				P.Entries [i] = C[i];

			B.Palette = P;
		}

		// generate a base64 string with the bitmap & palette and save it to image url
		public void SaveToUrl()
		{
			// Counter
			Stopwatch sw = Stopwatch.StartNew();

			// Create bitmap, and fill it 
			Bitmap B = new Bitmap (width, height,System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

			DumpToBitmap (B, Field);
			DumpPalette (B, Palette);

			sw.Stop ();
			InfoLabel.Text = "Bitmap generated on " + sw.ElapsedMilliseconds.ToString () + "ms";

			// Lets copy it in a stream and make page to load it encoded as base64
			sw.Start ();

			using (MemoryStream m = new MemoryStream ()) 
			{
				B.Save (m, System.Drawing.Imaging.ImageFormat.Jpeg);
				UImage.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String (m.ToArray ());

				InfoLabel.Text += "; size : " + Convert.ToBase64String (m.ToArray ()).Length + " bytes ";
			}

			B.Dispose();

			sw.Stop ();
			InfoLabel.Text += " / total ellapsed time : " + sw.ElapsedMilliseconds.ToString() + "ms";
		}

		// generate click will generate a new field and save it
		public void btGenerate_Click (object sender, EventArgs args)
		{
			generateField ();
			SaveToUrl ();
		}

		// random palette : generates some pivot colors and fill palette about
		public void btRandomPalette_Click (object sender, EventArgs args)
		{
			for (int i = 0; i < PivotColors.Length; i++) 
				PivotColors[i] = Color.FromArgb (r.Next () % 0xff, r.Next () % 0xff, r.Next () % 0xff);

			FillGradients (PivotColors);

			// invalidate page
			SaveToUrl ();
		}

		// timer event will cycle palette
		public void Timer_Tick (object sender, EventArgs args)
		{
			if (cbPaletteCycling.Checked) 
			{
				// no more than run palette
				Color swap = Palette [0];

				for (int i = 0; i < Palette.Length - 1; i++)
					Palette [i] = Palette [i + 1];

				Palette [_MAXCOLORS - 1] = swap;

				// let's invalidate page
				SaveToUrl ();
			}
		}
	}
}

