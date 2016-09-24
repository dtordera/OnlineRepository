using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Runtime.InteropServices;

namespace OnlineRepository
{
public partial class FireEffect : System.Web.UI.Page
{
	private const int _WIDTH = 100;   // 'heat' array size 
	private const int _HEIGHT = 300; // 

	private const int _FIREBASELENGTH = 5; // base line length 
	private const int _MAXCOLORS = 255; // Maximum palette colors

	private static 	Bitmap BShow = new Bitmap(	_WIDTH,
												_HEIGHT,
												 System.Drawing.Imaging.PixelFormat.Format8bppIndexed); // projection bitmap

	private static byte[] Back = new byte[_WIDTH*_HEIGHT];
	private static byte[] Fore = new byte[_WIDTH*_HEIGHT];

	private static Random r = new Random (); // Working random object
	private static Color[] Palette = new Color[_MAXCOLORS];

	public void Page_Load(object sender, EventArgs args)
	{
		// First time page load, lets generate a palette & put fire on base 
		if (!Page.IsPostBack) 
		{
			btRandomPalette_Click (null, null);
			FireBase ();
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
		int m = _MAXCOLORS / (C.Length+1);

		// first color (last one, 0, 'coldest' color) will be back page color
		Palette[0] = ColorTranslator.FromHtml(PageBody.Attributes["bgcolor"]);

		// Lets point each choosen color at position and do gradient between them
		for (int i = 1; i < C.Length+1; i++) 
		{
			Palette [i * m] = C [i-1];
			Gradient ((i - 1) * m, i * m);
		}
	}

	public void btRandomPalette_Click (object sender, EventArgs args)
	{
		Color[] C = new Color[6];
			
		for (int i = 0; i < C.Length; i++) 
			C[i] = Color.FromArgb (r.Next () % 0xff, r.Next () % 0xff, r.Next () % 0xff);

		FillGradients (C);
		DumpPalette ();
	}

	public void btDefaultPalette_Click (object sender, EventArgs args)
	{
		Color[] C = new Color[6];

		C [0] = Color.Navy;
		C [1] = Color.OrangeRed;
		C [2] = Color.Orange;
		C [3] = Color.Yellow;
		C [4] = Color.White;
		C [5] = Color.White;

		FillGradients (C);
		DumpPalette ();
	}

	private void DumpPalette()
	{
		ColorPalette P = BShow.Palette;

		for (int i = 0; i < _MAXCOLORS; i++)
			P.Entries [i] = Palette[i];

		BShow.Palette = P;
	}

	protected void Burn()
	{
		int x, y;
		int c;

		for (y = 1; y < _HEIGHT - 1; y++) 
			for (x = 1; x < _WIDTH - 1; x++) 
			{
				// Get all surrounding color indexs and do mean...
				c = (Back [x - 1 + (y - 1)*_WIDTH] + Back [x + (y - 1)*_WIDTH] + Back [x + 1 + (y - 1)*_WIDTH] +
					Back [x - 1 + y*_WIDTH]  +  /*   (x,y)    */     Back [x + 1 + y*_WIDTH]     + 
					Back [x - 1 + (y + 1)*_WIDTH] + Back [x + (y + 1)*_WIDTH] + Back [x + 1 + (y + 1)*_WIDTH])
					>> 3;

				// ...and we put it in the upper pixel. And then, fire grows...
				Fore [x + (y - 1)*_WIDTH] = (byte)c;
			}
	}
		
	protected void FireBase()
	{
		byte c = (byte)(_MAXCOLORS-1);
		int x, y;

		// Lets do, at image base, some random color lines
		for (y = _HEIGHT - 3; y < _HEIGHT; y++)
			for (x = 0; x < _WIDTH; x ++) 
			{
				if (x % _FIREBASELENGTH == 0)
					c = (byte)(Back [x + y*_WIDTH] + r.Next () % _MAXCOLORS);

				Back [x + y*_WIDTH] = (byte)(c % _MAXCOLORS - 1);
			}
	}

	protected void DumpToBitmap(Bitmap BShow)
	{
		Rectangle R = new Rectangle (0, 0, _WIDTH, _HEIGHT);
		BitmapData b = BShow.LockBits(R,ImageLockMode.WriteOnly,System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
	
		IntPtr ptr = b.Scan0;
		Marshal.Copy (Fore, 0, ptr, _WIDTH * _HEIGHT);
		BShow.UnlockBits (b);

		// When Fore array its all projected to bitmap, lets copy it as well to back, 
		// as source for next Burn 
		Array.Copy(Fore,Back,_WIDTH * _HEIGHT);
	}

	protected void Timer_Tick(object sender, EventArgs e)
	{
		// Counter
		Stopwatch sw = Stopwatch.StartNew();

		// Burn step 
		Burn ();
		sw.Stop ();

		// Dump it to bitmap
		DumpToBitmap (BShow);

		// Put fuel
		FireBase ();
		InfoLabel.Text = "Bitmap generated on " + sw.ElapsedMilliseconds.ToString () + "ms";

		// Dump to a string
		sw.Start ();
		using (MemoryStream m = new MemoryStream ()) 
		{
			BShow.Save (m, System.Drawing.Imaging.ImageFormat.Jpeg);
			UImage.ImageUrl = "data:image/Jpeg;base64," + Convert.ToBase64String (m.ToArray ());
		}
		sw.Stop ();
		InfoLabel.Text += " / total ellapsed time : " + sw.ElapsedMilliseconds.ToString() + "ms";
	}
}
}

