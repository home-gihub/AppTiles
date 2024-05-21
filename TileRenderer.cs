using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Raylib_cs;

namespace AppTiles
{
	internal class TileRenderer(int sw_arg, int sh_arg)
	{
		int sw = sw_arg;
		int sh = sh_arg;
		int tile_unit_w = sw_arg / 7;
		int tile_unit_h = sh_arg /5;


		public void Render(XmlNode view, int origin_x, int origin_y)
		{
			Draw(view, origin_x, origin_y);
		}

		public void Tick(XmlNode tile, string view_type, int origin_x, int origin_y, Vector2 mousePos)
		{
            int[] clip = CalcTileBounds(origin_x, origin_y, view_type);

            string? url = tile.Attributes["url"].Value;

            // ik lots of nesting

			if (Raylib.IsMouseButtonPressed(MouseButton.Left))
			{
                Console.WriteLine("press");
                Console.WriteLine(origin_x);
                Console.WriteLine(mousePos.X);
                Console.WriteLine(clip[0]);
                Console.WriteLine(origin_y);
                Console.WriteLine(mousePos.Y);
                Console.WriteLine(clip[1]);


                if (origin_x <= mousePos.X && mousePos.X <= clip[0])
				{
                    if (origin_y <= mousePos.Y && mousePos.Y <= clip[1])
                    {
                        Console.WriteLine("click");

                        if (url != null)
                        {
                            Console.WriteLine("url is not null :)");

                            try
                            {
                                Process.Start(url);
                            }
                            catch
                            {
                                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    url = url.Replace("&", "^&");
                                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                                }
                                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                {
                                    Process.Start("xdg-open", url);
                                }
                                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                {
                                    Process.Start("open", url);
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
			}
        }

		private void Draw(XmlNode view, int origin_x, int origin_y)
		{
			Color view_bg;

			if ((view.Attributes["color_r"] == null || view.Attributes["color_g"] == null || view.Attributes["color_b"] == null))
			{
				view_bg = new(255, 0, 0, 255);
			}
			else
			{
				view_bg = new Color(Convert.ToInt32(view.Attributes["color_r"].Value), Convert.ToInt32(view.Attributes["color_g"].Value), Convert.ToInt32(view.Attributes["color_b"].Value), 255);
			}

			int[] clip = CalcTileBounds(origin_x, origin_y, view.Attributes["width"].Value + "x" + view.Attributes["height"].Value);

            switch (view.Attributes["width"].Value + "x" + view.Attributes["height"].Value)
            {
                case "1x1":
					Raylib.DrawRectangle(origin_x, origin_y, tile_unit_w, tile_unit_h, view_bg);
                    break;
                case "2x1":
                    Raylib.DrawRectangle(origin_x, origin_y, tile_unit_w * 2, tile_unit_h, view_bg);
                    break;
                case "2x2":
                    Raylib.DrawRectangle(origin_x, origin_y, tile_unit_w * 2, tile_unit_h * 2, view_bg);
                    break;
            }
            // draw stuff in the tile

			foreach (XmlNode node in view.ChildNodes)
			{
				ParseAndDrawNode(node, origin_x, origin_y, clip[0], clip[1]);
			}
		}

		private void ParseAndDrawNode(XmlNode node, int origin_x, int origin_y, int clip_x, int clip_y)
        {

            // hack

			if (origin_x + Convert.ToInt32(node.Attributes["x"].Value) < origin_x || origin_x + Convert.ToInt32(node.Attributes["x"].Value) > clip_x) {
				node.Attributes["x"].Value = "0";
			}

			if ((origin_y + Convert.ToInt32(node.Attributes["y"].Value)) < origin_y || origin_y + Convert.ToInt32(node.Attributes["y"].Value) > clip_y)
			{
				node.Attributes["y"].Value = "0";
			}

            // code for drawing different things

			switch (node.Name)
			{
				case "text":
					Color node_col = new Color(Convert.ToInt32(node.Attributes["color_r"].Value), Convert.ToInt32(node.Attributes["color_g"].Value), Convert.ToInt32(node.Attributes["color_b"].Value), 255);
					Raylib.DrawText(node.InnerText, origin_x + Convert.ToInt32(node.Attributes["x"].Value), origin_y + Convert.ToInt32(node.Attributes["y"].Value), Convert.ToInt32(node.Attributes["size"].Value), node_col);
					break;

			}
		}

		private int[] CalcTileBounds(int origin_x, int origin_y, string tile_type)
		{
            // calculate the bottom left point of the tile

            int clip_X = 0;
            int clip_Y = 0;

            switch (tile_type)
            {
                case "1x1":
                    clip_X = origin_x + tile_unit_w;
                    clip_Y = origin_y + tile_unit_h;
                    break;
                case "2x1":
                    clip_X = origin_x + tile_unit_w * 2;
                    clip_Y = origin_y + tile_unit_h;
                    break;
                case "2x2":
                    clip_X = origin_x + tile_unit_w * 2;
                    clip_Y = origin_y + tile_unit_h * 2;
                    break;
            }

            return [clip_X, clip_Y];
		}
	}
}
