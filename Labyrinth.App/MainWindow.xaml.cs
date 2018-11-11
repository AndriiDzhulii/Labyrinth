using Labyrinth.App.Core;
using System;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Labyrinth.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Representation _representation;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StartProccess();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }

        public void StartProccess()
        {
            var labyrinthModel = FileParser.ParseLabyrinthFile();

            _representation = new Representation(canvasLabyrinth);
            _representation.DrawLabyrinth(labyrinthModel);

            var labyrinthProccessor = new LabyrinthProcessor();
            labyrinthProccessor.StepChanged += LabyrinthProccessor_StepChanged;

            new Thread(() =>
            {
                var result = labyrinthProccessor.ProccessLabyrinth(labyrinthModel);

                rbResult.Dispatcher.BeginInvoke((Action)(() =>
                {
                    var strResults = string.Join(" ", result.Select(e => $"{{{e.X},{e.Y}}}"));
                    rbResult.Text = strResults;
                }));

            }).Start();
        }

        private void LabyrinthProccessor_StepChanged(object sender, StepChangeEventArgs e)
        {
            _representation.SetActiveStep(e.Point);
        }
    }
}
