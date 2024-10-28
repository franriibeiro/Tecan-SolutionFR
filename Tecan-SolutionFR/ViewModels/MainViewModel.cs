using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System;

namespace Tecan_SolutionFR.ViewModels
{
    /// <summary>
    /// ViewModel for managing the color generation and sorting operations in the application.
    /// Inherits from BaseViewModel to provide property change notifications.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {

        // Observable collection to store colors for each pixel.
        public ObservableCollection<SolidColorBrush> PixelColors { get; set; } = new ObservableCollection<SolidColorBrush>();

        // Service class to handle color generation and sorting
        private readonly ColorService _colorService;

        // Fields for grid dimensions
        private int _gridWidth;
        public int GridWidth
        {
            get => _gridWidth;
            set
            {
                _gridWidth = value;
                OnPropertyChanged(nameof(GridWidth));
            }
        }

        private int _gridHeight;
        public int GridHeight
        {
            get => _gridHeight;
            set
            {
                _gridHeight = value;
                OnPropertyChanged(nameof(GridHeight));
            }
        }

        // Command for generating random colors
        public ICommand GenerateColorsCommand { get; }

        // Command for sorting colors by hue.
        public ICommand SortColorsCommand { get; }

        public MainViewModel()
        {
            _colorService = new ColorService();
            GenerateColorsCommand = new RelayCommand(async () => await GenerateColors());
            SortColorsCommand = new RelayCommand(async () => await SortColors());
            PixelColors = new ObservableCollection<SolidColorBrush>();
        }

        // Generates a specified number of random colors asynchronously.
        public async Task GenerateColors()
        {
            int numPixels = 36100;

            var colors = await _colorService.GenerateColorsAsync(numPixels);
            PixelColors.Clear();

            // Calculate and set the GridWidth and GridHeight dynamically
            int gridSize = (int)Math.Ceiling(Math.Sqrt(numPixels)) * 2;

            GridWidth = gridSize;
            GridHeight = gridSize;

            foreach (var color in colors)
            {
                PixelColors.Add(new SolidColorBrush(color));
            }
        }

        // Sorts the current colors by hue asynchronously.
        public async Task SortColors()
        {
            var sortedColors = await _colorService.SortColorsByHueAsync(PixelColors.Select(b => b.Color).ToList());
            PixelColors.Clear();
            foreach (var color in sortedColors)
            {
                PixelColors.Add(new SolidColorBrush(color));
            }
        }
        public static double GetHue(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double hue = 0.0;

            if (max == min)
            {
                hue = 0.0;
            }
            else if (max == r)
            {
                hue = (60 * ((g - b) / (max - min)) + 360) % 360;
            }
            else if (max == g)
            {
                hue = (60 * ((b - r) / (max - min)) + 120) % 360;
            }
            else if (max == b)
            {
                hue = (60 * ((r - g) / (max - min)) + 240) % 360;
            }

            return hue;
        }


    }
}
