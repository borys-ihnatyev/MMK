using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MMK.HotMark.ViewModel.PianoKeyBoard;

namespace MMK.HotMark.View
{
    public partial class PianoKeyBoardControl
    {
        private static readonly double GoldenRatio = (Math.Sqrt(5) - 1)/2;
        public PianoKeyBoardControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.NewValue as PianoKeyBoardViewModel;
            if(viewModel == null) return;            
            
            CreateLayout(viewModel);
        }

        private void CreateLayout(PianoKeyBoardViewModel viewModel)
        {
            Layout.Children.Clear();
            viewModel.LoadData();

            var left = 0.0;

            viewModel
                .Keys
                .Select(pianoKeyViewModel => CreatePianoKeyView(pianoKeyViewModel, ref left))
                .ForEach(view => Layout.Children.Add(view));

            MaxWidth = Width = left;
            MaxHeight = ActualHeight;
        }

        private PianoKeyControl CreatePianoKeyView(PianoKeyViewModel viewModel, ref double left)
        {
            var view = new PianoKeyControl {DataContext = viewModel};

            if (viewModel.IsSharpness())
            {
                view.Width = view.Width * GoldenRatio;
                view.Height = ActualHeight * GoldenRatio;

                view.Margin = new Thickness(left - view.Width/2, 0, 0, 0);
                view.SetValue(Panel.ZIndexProperty, 10);
            }
            else
            {
                view.Height = ActualHeight;
                view.Margin = new Thickness(left, 0, 0, 0);
                
                left += view.Width;
            }

            return view;
        }
    }
}