using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VejrudsigtenProgrammering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string by = By.Text;

            WeatherService service = new(by);
            await service.UpdateWeatherAsync();

            double temperatur = service.TodayTemperature;
            string beskrivelse = service.TodayDescription;

            // Vejret er regn og det er 8.4 grader

            string besked = $"Vejret er {beskrivelse} og det er {temperatur} grader";
            Vejrudsigten.Content = besked;
        }
    }
}
