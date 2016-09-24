using System;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace OnlineRepository
{
	public partial class RaisingHighMaps : System.Web.UI.Page
	{
		private const int _MAXINDEX = 512;	  // Map will go from 0..MAX_INDEX (included). So number of cells 
											  // will be MAX_INDEX+1. Must be 2^n to ensure midpoint at each iteration.
					
		private const int _HIGHRANGE = 800;  // max high difference will be this value * Entrophy

		private const int _RENDERWIDTH = 1000;
		private const int _RENDERHEIGHT = 550;
	
		private Random rnd = new Random();	// generic random object
	
		public static Color[] Palette = new Color[_HIGHRANGE]; // palette 
	
		// gets a random int in range * entrophy
		public int r(int range,float S)
		{
			return (int)((rnd.NextDouble()*2 - 1)*range*S);
		}
	
		// main load event. Let's create palettes
		public void Page_Load(object sender, EventArgs args)
		{
			// First time page load, lets generate palettes  
			if (!Page.IsPostBack) 
				CreatePalettes ();
		}
	
		// here we generate the palette set
		private void CreatePalettes()
		{
			Color[] C = new Color[8];
			C[0] = Color.DarkBlue;
			C[1] = Color.DarkSlateBlue;
			C[2] = Color.DarkSeaGreen;
			C[3] = Color.DarkGreen;
			C[4] = Color.ForestGreen;
			C[5] = Color.Gray;
			C[6] = Color.AntiqueWhite;
			C[7] = Color.White;

			FillGradients(C);
		}
	
		// prepare work array
		private void Initialize(int[,] W)
		{
			// Let's fill in corners
			W[ 0,0] = rnd.Next() % _HIGHRANGE;
			W [_MAXINDEX, 0] = rnd.Next() % _HIGHRANGE;
			W [0, _MAXINDEX] = rnd.Next() % _HIGHRANGE;
			W [_MAXINDEX, _MAXINDEX] = rnd.Next() % _HIGHRANGE;
		}
	
		// main loop of diamond square algorithm, (S is entrophy)
		private void DoDiamondSquare(int[,] W, float S)
		{
			int hs, x, y;
			int A, B, C, D, M, n;
	
			// Lets iterate through side size until size is too small
			for (int it = _MAXINDEX; it > 1; it/=2) 
			{
				hs = it /2;
				
				//Midpoints
				for (y = hs; y < _MAXINDEX; y += it)
					for (x = hs; x < _MAXINDEX; x += it) 
					{
						// each square corner
						A = W [x - hs, y - hs];
						B = W [x - hs, y + hs];
						C = W [x + hs, y - hs];
						D = W [x + hs, y + hs];
	
						// center point will be average plus a random in-range value
						W [x, y] = ((A + B + C + D) / 4) + r (hs,S);
					}
	
				// Going through each diamond point				
				for (y = 0; y < _MAXINDEX+1; y += hs)
					for (x = y % it == 0? hs : 0; x < _MAXINDEX+1; x += it) // getting offset of x in function of y 
					{	  
						M = n = 0; // Sum and denominatior
	
						// this way we can calculate border points
						try { M += W [x + hs, y]; n++;} catch(Exception){}
						try { M += W [x - hs, y]; n++;} catch(Exception){}
						try { M += W [x, y + hs]; n++;} catch(Exception){}
						try { M += W [x, y - hs]; n++;} catch(Exception){}
	
						// lets average sum plus random value
						W [x, y] = M / n + r (hs,S)/2;
					}
			}
		}

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
	
		protected void FillGradients(Color[] C)
		{
			// counting the back color, 0 indexed, there will be C.Length + 1 color turns. 
			int m = _HIGHRANGE / (C.Length+1);
	
			Palette[0] = Color.FromArgb(0xff,0,0,5);
	
			// Lets point each choosen color at position and do gradient between them
			for (int i = 1; i < C.Length+1; i++) 
			{
				Palette [i * m] = C [i-1];
				Gradient ((i - 1) * m, i * m);
			}
		}
	
		// dumping work array to bitmap, using palette P. We mark under-zero and over-highrange values
		private void DumpToBitmap(Color[] P, int[,] W, Bitmap B)
		{
			int x, y;
			Color c;
	
			for (x = 0; x < _MAXINDEX + 1; x++)
				for (y = 0; y < _MAXINDEX + 1; y++) 
				{
					if (W [x, y] == 0)			c = Color.Red;
					else
					if (W[x,y] < 0)				c = Color.DarkRed;
					else
					if (W[x,y] > _HIGHRANGE-1)	c = Color.Yellow;
					else 						c = P[W[x,y]];
	
					B.SetPixel (x, y, c);
				}
		}
	
		// functional to send dump bitmap and send it to web
		private void DumpToBitmapAndSendToWeb(int[,] W)
		{
			// Create bitmap, and fill it 
			Bitmap B = new Bitmap (_MAXINDEX+1, _MAXINDEX+1,System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
			DumpToBitmap (Palette,W, B);
	
			using (MemoryStream m = new MemoryStream ()) 
			{
				B.Save (m, System.Drawing.Imaging.ImageFormat.Gif);
				UHighMap.ImageUrl = "data:image/Gif;base64," + Convert.ToBase64String (m.ToArray ());
			}
		}

		// raise map and draw result to a bitmap
		private void RaiseToBitmap(Color[] P,int[,] W, Bitmap B)
		{
			int x,y; // coordinates of work array
			int i,j; // projected coordinates
			Color c; // color to paint
		
			for (x=0;x<_MAXINDEX+1;x++)
				for (y=0;y<_MAXINDEX+1;y++)
				{
					try 
					{
						i = _RENDERWIDTH/2 - y + x;					// begin from the middle of the bitmap. 	
																	// We will offset negatively as we go down (y increases) so generates some isometric
																	// perspective
		
						j = 3*_RENDERHEIGHT/4 - 2*W[x,y]/3 + y/3;	// We inverse y from high map and apply a factor to it.
		
						if (i > 1 && i < _RENDERWIDTH - 1&& j > 0 && j < _RENDERHEIGHT) // range check
						{
							c = P[W[x,y]];	// gets color from high map
		
							if (W[x-1,y] > W[x,y]) // if left point is higher...
								c = Color.FromArgb(0xff,c.R/3, c.G/3, c.B/3); // ...current will be at it's shadow
		
							for(int k=j+20;k>j;k--) // lets draw it
								B.SetPixel(i,k,c);
		
							if (W[x,y] < 220 && x%4 == 0) // sea level 
								B.SetPixel(i,3*_RENDERHEIGHT/4 + y/3-2*220/3,P[W[x,y]]);
						}
					}
					catch(Exception){}
				}
		}

		// functional to raise highmap to bitmap & send it to web
		private void RaiseToBitmapAndSendToWeb(int [,]W)
		{
			// Create bitmap, and fill it 
			Bitmap B = new Bitmap (_RENDERWIDTH, _RENDERHEIGHT,System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			RaiseToBitmap(Palette,W, B);
	
			using (MemoryStream m = new MemoryStream ()) 
			{
				B.Save (m, System.Drawing.Imaging.ImageFormat.Jpeg);
				URender.ImageUrl = "data:image/Jpeg;base64," + Convert.ToBase64String (m.ToArray ());
			}
		}
	
		// an info search...
		private void SearchMinMax(int[,] W)
		{
			int  min=_HIGHRANGE	
				,max=-_HIGHRANGE;
	
			for (int y=0;y<_MAXINDEX+1;y++)
			for (int x=0;x<_MAXINDEX+1;x++)
			{
				min = W[x,y] < min ? W[x,y] : min;
				max = W[x,y] > max ? W[x,y] : max;
			}
	
			InfoLabel.Text = "min/max:" + min.ToString() + "/" + max.ToString();
		}
		
		// let's do a field...
		public void btGenerate_Click(object sender, EventArgs args)
		{
			Stopwatch sw = Stopwatch.StartNew();
	
			// Work array
			int[,] W = new int[_MAXINDEX+1,_MAXINDEX+1];
	
			// Create fractal
			Initialize(W);
			DoDiamondSquare (W,float.Parse(tbEntrophy.Text));
	
			// Dump result
			DumpToBitmapAndSendToWeb(W);
			SearchMinMax(W);

			// Render	
			RaiseToBitmapAndSendToWeb(W);
		
			// Finish
			sw.Stop ();
			InfoLabel.Text += " / total ellapsed time : " + sw.ElapsedMilliseconds.ToString() + "ms";
		}		
	}
}

