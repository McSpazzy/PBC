using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PBCLib;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace PBC2PNG
{
    public class Program
    {
        public static async Task<int> Main(params string[] args)
        {
            var rootCommand = new RootCommand(description: "Converts an image file from one format to another.");

            rootCommand.AddArgument(new Argument<string>("input", "input file."));
            rootCommand.AddOption(new Option(new[] { "--grid", "-g" }, "Draw grid."));
            rootCommand.AddOption(new Option<int>(new[] { "--scale", "-s" }, "Scale."));
            rootCommand.AddOption(new Option(new[] { "--type", "-t" }, "Draw tile type identifiers."));
            rootCommand.AddOption(new Option<string>(new[] { "--output", "-o" }, "Output Path."));
            rootCommand.AddOption(new Option<string>(new[] { "--colormap", "-c" }, "User specified colors in csv format. TODO"));

            rootCommand.Handler = CommandHandler.Create<string, bool, bool, string, string, int>(Run);
            return await rootCommand.InvokeAsync(args);
        }

        public static void Run(string input, bool grid, bool type, string output, string colormap, int scale = 1)
        {
            try
            {
                if (!File.Exists(input))
                {
                    Console.WriteLine($"Error: '{input}' does not exist.");
                    return;
                }

                if (type && scale < 16)
                {
                    Console.WriteLine("Warning: Type not recommended for scale less than 16");
                }

                if (!string.IsNullOrEmpty(colormap))
                {
                    Console.WriteLine("Warning: Not yet implemented.");
                }

                var pbc = new PBC(input);
                var bm = PBCDraw.Draw(pbc, scale, grid, type);
                if (string.IsNullOrEmpty(output))
                {
                    output = input.Replace("pbc", "png");
                }

                Console.WriteLine($"Writing File: {output}");
                bm.Save(output, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public class PBCDraw
    {
        public static Dictionary<uint, Color> Colors = new();

        static PBCDraw()
        {
            BuildColor();
        }

        public static Bitmap Draw(PBC pbc, int scale, bool grid, bool type)
        {
            var bm = new Bitmap(pbc.Header.Width * scale * 2, pbc.Header.Height * scale * 2);
            var gr = Graphics.FromImage(bm);

            var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
            var f = new Font(FontFamily.GenericMonospace, (int)(scale / 2), FontStyle.Regular, GraphicsUnit.Pixel);

            for (var h = 0; h < pbc.Header.Height; h++)
            {
                for (var w = 0; w < pbc.Header.Width; w++)
                {
                    var tile = pbc.Tiles[h, w];

                    var tileBitmap = new Bitmap(scale * 2, scale * 2);
                    var tileGraphic = Graphics.FromImage(tileBitmap);

                    for (var y = 0; y < 2; y++)
                    {
                        for (var x = 0; x < 2; x++)
                        {
                            var v = (uint)tile.Type[y, x];
                            var c = GetColor(v);
                            tileGraphic.FillRectangle(new SolidBrush(c), new Rectangle(y * scale, x * scale, scale, scale));
                            if (type)
                            {
                                tileGraphic.DrawString($"{v}", f, new SolidBrush(ContrastColor(c)), new Rectangle(y * scale, x * scale, scale, scale), sf);
                            }
                        }
                    }

                    gr.DrawImage(tileBitmap, w * scale * 2, h * scale * 2);

                    if (grid)
                    {
                        gr.DrawRectangle(new Pen(Color.Black), new Rectangle(w * scale * 2, h * scale * 2, scale * 2, scale * 2));
                    }
                }
            }

            return bm;
        }

        private static Color ContrastColor(Color color)
        {
            var luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            var d = luminance > 0.5 ? 0 : 255;
            return Color.FromArgb(d, d, d);
        }

        public static Color GetColor(uint i)
        {
            if (Colors.ContainsKey(i))
            {
                return Colors[i];
            }

            return Color.FromArgb(255, 128, 0, 128);
        }

        public static void BuildColor()
        {
            Colors.Add(0, Color.FromArgb(255, 70, 120, 64));
            Colors.Add(1, Color.FromArgb(255, 128, 215, 195));
            Colors.Add(3, Color.FromArgb(255, 192, 192, 192));
            Colors.Add(4, Color.FromArgb(255, 240, 230, 170));
            Colors.Add(5, Color.FromArgb(255, 128, 215, 195));
            Colors.Add(6, Color.FromArgb(255, 255, 128, 128));
            Colors.Add(7, Color.FromArgb(255, 0, 0, 0));
            Colors.Add(8, Color.FromArgb(255, 32, 32, 32));
            Colors.Add(9, Color.FromArgb(255, 255, 0, 0));
            Colors.Add(10, Color.FromArgb(255, 48, 48, 48));
            Colors.Add(12, Color.FromArgb(255, 128, 215, 195));
            Colors.Add(15, Color.FromArgb(255, 128, 215, 195));
            Colors.Add(22, Color.FromArgb(255, 192, 255, 98));
            Colors.Add(23, Color.FromArgb(255, 192, 155, 98));
            Colors.Add(28, Color.FromArgb(255, 255, 0, 0));
            Colors.Add(29, Color.FromArgb(255, 232, 222, 162));
            Colors.Add(41, Color.FromArgb(255, 118, 122, 132));
            Colors.Add(42, Color.FromArgb(255, 128, 133, 147));
            Colors.Add(44, Color.FromArgb(255, 62, 112, 56));
            Colors.Add(45, Color.FromArgb(255, 118, 122, 132));
            Colors.Add(46, Color.FromArgb(255, 120, 207, 187));
            Colors.Add(47, Color.FromArgb(255, 128, 128, 0));
            Colors.Add(49, Color.FromArgb(255, 190, 98, 98));
            Colors.Add(51, Color.FromArgb(255, 32, 152, 32));
        }
    }
}
