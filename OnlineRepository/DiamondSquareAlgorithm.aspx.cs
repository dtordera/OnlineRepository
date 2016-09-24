using System;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace OnlineRepository
{
	public partial class DiamondSquareAlgorithm : System.Web.UI.Page
	{
		private const int _MAXINDEX = 512;	  // Map will go from 0..MAX_INDEX (included). So number of cells 
											  // will be MAX_INDEX+1. Must be 2^n to ensure midpoint at each iteration.
					
		private const int _HIGHRANGE = 1000;  // max high difference will be this value * Entrophy
	
		private Random rnd = new Random();	// generic random object
	
		private const int _MAXPALETTES = 5;   // palette set number
		public static Color[][] Palette = new Color[_MAXPALETTES][]; // palette set
	
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
			int i;
	
			// Moduled gray
			Palette[0] = new Color[_HIGHRANGE];
			for (i=0;i<1000;i++)
				Palette[0][i] = Color.FromArgb(0xff,i%255,i%255,i%255);
			
			// Earth like
			Palette[1] = new Color[_HIGHRANGE];
			for (i=0;i<100;i++) // deep sea
				Palette[1][i] = Color.FromArgb(0x0f,0,0,rnd.Next()%40+5);
	
			for(i=100;i<300;i++) // sea
				Palette[1][i] = Color.FromArgb(0x0f,0,0,i/2-50);
			
			for(i=300;i<500;i++) // country-side
				Palette[1][i] = Color.FromArgb(0x0f,i/2-150,i-300,0);
	
			for(i=500;i<700;i++) // forest
				Palette[1][i] = Color.FromArgb(0x0f,0,355-i/2,0);
	
			for (i=700;i<950;i++) // mountain
				Palette[1][i] = Color.FromArgb(0x0f,i-700,i-700,i-700);
	
			for (i=950;i<1000;i++) // high mountain
				Palette[1][i] = Color.FromArgb(0xff,i-800,i-800,i-800);
	
			// Sky
			Palette[2] = new Color[_HIGHRANGE];
			for (i=0;i<1000;i++)
				Palette[2][i] = Color.FromArgb(0xff,(i/4)+5,(i/4)+5,155);
	
			// Volcano
			Palette[3] = new Color[_HIGHRANGE];
			for (i=0;i<1000;i++)
				Palette[3][i] = Color.Black;
	
			for (i=0;i<5;i++) // some lava rivers...
			{
				Palette[3][1 + 150*i] = Palette[3][13 + 150*i] = Color.DarkRed;
				Palette[3][2 + 150*i] = Palette[3][12 + 150*i] = Color.Red;
				Palette[3][3 + 150*i] = Palette[3][11 + 150*i] = Color.Red;
				Palette[3][4 + 150*i] = Palette[3][10 + 150*i] = Color.Orange;
				Palette[3][5 + 150*i] = Palette[3][9  + 150*i] = Color.Yellow;
				Palette[3][6 + 150*i] = Palette[3][8  + 150*i] = Color.LightYellow;
				Palette[3][7 + 150*i] = Color.White;
			}
	
			// Storm
			Palette[4] = new Color[_HIGHRANGE];
			for (i=0;i<1000;i++)
				Palette[4][i] = Color.FromArgb(0xff,30,30,(i/5)+50);
	
			for (i=0;i<4;i++) // some lightings...
			{
				Palette[4][1 + 150*i] = Palette[4][13 + 150*i] = Color.DeepSkyBlue;
				Palette[4][2 + 150*i] = Palette[4][12 + 150*i] = Color.SkyBlue;
				Palette[4][3 + 150*i] = Palette[4][11 + 150*i] = Color.Blue;
				Palette[4][4 + 150*i] = Palette[4][10 + 150*i] = Color.LightSteelBlue;
				Palette[4][5 + 150*i] = Palette[4][9  + 150*i] = Color.LightSteelBlue;
				Palette[4][6 + 150*i] = Palette[4][8  + 150*i] = Color.LightCyan;
				Palette[4][7 + 150*i] = Color.White;
			}
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
			DumpToBitmap (Palette[Int32.Parse(ddPalette.SelectedItem.Value)],W, B);
	
			using (MemoryStream m = new MemoryStream ()) 
			{
				B.Save (m, System.Drawing.Imaging.ImageFormat.Jpeg);
				UHighMap.ImageUrl = "data:image/Jpeg;base64," + Convert.ToBase64String (m.ToArray ());
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
		
			// Finish
			sw.Stop ();
			InfoLabel.Text += " / total ellapsed time : " + sw.ElapsedMilliseconds.ToString() + "ms";
		}		
	}
}

