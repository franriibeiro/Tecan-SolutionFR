using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using System.Collections.Concurrent;

public class ColorService
{
    // Thread-local rand instance for parallel operations
    private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

    // Generates a list of random colors.
    public async Task<List<Color>> GenerateColorsAsync(int numPixels)
    {
        return await Task.Run(() =>
        {
            var colorList = new Color[numPixels];

            // Parallel processing for color generation
            Parallel.For(0, numPixels, i =>
            {
                var random = _random.Value;
                byte r = (byte)random.Next(256);
                byte g = (byte)random.Next(256);
                byte b = (byte)random.Next(256);
                colorList[i] = Color.FromRgb(r, g, b);
            });

            return colorList.ToList();
        });
    }

    //Sorts a list of colors by their hue asynchronously.
    public async Task<List<Color>> SortColorsByHueAsync(IEnumerable<Color> colors)
    {
        // Calculate hue values in parallel and insert them in a list
        var colorHues = await Task.Run(() =>
        {
            return colors
                .AsParallel() // Parallelize hu calculation
                .Select(color => new { Color = color, Hue = GetHue(color) })
                .ToList();
        });

        // Sort colors based on hue values
        return colorHues
            .OrderBy(colorHue => colorHue.Hue)
            .Select(colorHue => colorHue.Color)
            .ToList();
    }



    private static double GetHue(Color color)
    {
        // Normalize the RGB values to the range [0, 1]
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        // Determine max and min values of r, g, and b
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));

        double hue = 0.0;

        // Check if the maximum and minimum values are the same
        // If they are, the color is achromatic (gray), and hue is 0
        if (max == min)
        {
            hue = 0.0;
        }
        else if (max == r) // If red is the maximum value
        {
            // Calculate hue based on the formula for red
            hue = (60 * ((g - b) / (max - min)) + 360) % 360;
        }
        else if (max == g) // If green is the maximum value
        {
            // Calculate hue based on the formula for green
            hue = (60 * ((b - r) / (max - min)) + 120) % 360;
        }
        else if (max == b) // If blue is the maximum value
        {
            // Calculate hue based on the formula for blue
            hue = (60 * ((r - g) / (max - min)) + 240) % 360;
        }

        return hue;
    }
}
