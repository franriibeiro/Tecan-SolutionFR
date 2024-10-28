using System;
using System.Linq;
using System.Threading.Tasks;
using Tecan_SolutionFR.ViewModels;
using Xunit;

namespace Tecan_SolutionFR_UnitTests
{
    public class MainViewModelTests
    {

        private MainViewModel _viewModel;

        public MainViewModelTests()
        {
            _viewModel = new MainViewModel();
        }

        [Fact]
        public async Task GenerateColors_ShouldPopulatePixelColors()
        {
            await _viewModel.GenerateColors();

            Assert.Equal(36100, _viewModel.PixelColors.Count);
            Assert.True(_viewModel.PixelColors.Distinct().Count() > 1, "Colors should be randomly generated and unique");
        }

        [Fact]
        public async Task GenerateColors_ShouldSetGridDimensionsCorrectly()
        {
            await _viewModel.GenerateColors();

            // Calculate expected grid size for 36,100 pixels with 2x2 rectangles
            int expectedGridSize = (int)(System.Math.Ceiling(System.Math.Sqrt(36100)) * 2);

            Assert.Equal(expectedGridSize, _viewModel.GridWidth);
            Assert.Equal(expectedGridSize, _viewModel.GridHeight);
        }

        [Fact]
        public async Task SortColors_ShouldSortPixelColorsByHue()
        {
            await _viewModel.GenerateColors();
            var originalColors = _viewModel.PixelColors.Select(brush => brush.Color).ToList();

            await _viewModel.SortColors();

            var sortedColors = _viewModel.PixelColors.Select(brush => brush.Color).ToList();
            var huesSorted = sortedColors.Select(MainViewModel.GetHue).ToList();

            // Ensure list is sorted by hue
            for (int i = 1; i < huesSorted.Count; i++)
            {
                Assert.True(huesSorted[i - 1] <= huesSorted[i], "Colors should be sorted by hue");
            }

            Assert.Equal(originalColors.Count, sortedColors.Count);
        }

        [Fact]
        public async Task GridWidth_And_GridHeight_Should_ChangeOnGenerateColors()
        {
            _viewModel.GridWidth = 100;
            _viewModel.GridHeight = 100;

            // Check initial values
            Assert.NotEqual(380, _viewModel.GridWidth);
            Assert.NotEqual(380, _viewModel.GridHeight);

            // Trigger color generation to resize grid
            await _viewModel.GenerateColors();

            // Assert
            Assert.Equal(380, _viewModel.GridWidth);
            Assert.Equal(380, _viewModel.GridHeight);
        }

    }
}
