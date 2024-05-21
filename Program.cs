using AppTiles;
using Raylib_cs;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.IO;
using System.Numerics;
using System.Data;

class Program
{
    static readonly int sw = 1920;
    static readonly int sh = 1080;

    public static void Main(string[] args)
    {
        Raylib.InitWindow(sw, sh, "Hello World");

        TileRenderer trender = new(sw, sh);

        XmlDocument layout = new();
        layout.Load(".\\layout.xml");
        XmlNodeList tiles = layout.SelectNodes("//layout/*");
        if (tiles == null || tiles.Count == 0) { 
        Console.WriteLine("no tiles :(");
        System.Environment.Exit(1);
        }


        Vector2 mousePos = new(0, 0);


        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Raylib.DrawText("Tiles", sw / 16, sh / 16, 40, Color.White);

            mousePos = Raylib.GetMousePosition();

            foreach (XmlNode node in tiles)
            {
                if (node.Attributes["file"] == null) { continue; }

                XmlDocument tiledoc = new();

                tiledoc.Load(".\\" + node.Attributes["file"].Value + ".xml");
                XmlNodeList? views = tiledoc.GetElementsByTagName("tile").Item(0).ChildNodes;

                if (views == null) { continue; }

                XmlNode? view = null;

                foreach (XmlNode loop_view in views)
                {
                    if (loop_view.Attributes["width"] == null || loop_view.Attributes["height"] == null) { continue; }

                    if (loop_view.Attributes["width"].Value == node.Attributes["width"].Value && loop_view.Attributes["height"].Value == node.Attributes["height"].Value)
                    {
                        view = loop_view;
                    }
                }


                if (view == null)
                {
                    Console.WriteLine("no view :(");
                    System.Environment.Exit(1);
                }

                trender.Tick(tiledoc.GetElementsByTagName("tile").Item(0), view.Attributes["width"].Value + "x" + view.Attributes["height"].Value, 50,150, mousePos);
                trender.Render(view, 50, 150);
            }
     
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}