using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using Tic_tac_toe.Models;
using Tic_tac_toe.Services;

namespace Tic_tac_toe
{
    public partial class MainWindow : Window
    {
        private GameManager gameManager = GameManager.Instance;

        public MainWindow()
        {
            this.InitializeComponent();
            Cells.PlayerMoved += this.Cells_PlayerMoved;
            this.gameManager.StateChanged += this.GameManager_StateChanged;
            this.gameManager.GameOver += this.GameManager_GameOver;
        }

        public static AutoResetEvent WaitEvent { get; set; } = new AutoResetEvent(false);

        private async void Cells_PlayerMoved(object sender, Move move)
        {
            if (!this.gameManager.GameStarted && !RadioPlayerFirst.IsChecked.Value)
            {
                return;
            }

            if (!this.gameManager.GameStarted)
            {
                await this.Start();
            }

            await Task.Run(() =>
            {
                this.gameManager.PlayerMakeMove(move);
            });
        }

        private void GameManager_StateChanged(object sender, RootState state)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Cells.State = state;
                TextBoxHeur.Text = state.DisplayHeurValue.ToString();
            }));
        }

        private void GameManager_GameOver(object sender, string message)
        {
            MessageBox.Show(message);
            this.Finish();
        }

        private async Task Start()
        {
            TextBoxDepth.IsEnabled = false;
            ButtonStart.IsEnabled = false;
            RadioPlayerFirst.IsEnabled = false;
            RadioComputerFirst.IsEnabled = false;
            RadioPlayerO.IsEnabled = false;
            RadioPlayerX.IsEnabled = false;
            ButtonStop.IsEnabled = true;

            Cells.PlayerSymbol = RadioPlayerX.IsChecked.Value ? "X" : "O";
            Cells.ComputerSymbol = RadioPlayerO.IsChecked.Value ? "X" : "O";

            int depth = Convert.ToInt32(TextBoxDepth.Text);
            bool isPlayerFirst = RadioPlayerFirst.IsChecked.Value;
            await Task.Run(() =>
            {
                this.gameManager.Start(depth, isPlayerFirst);
                this.gameManager.ComputerMakeMove();
            });
        }

        private void Finish()
        {
            TextBoxDepth.IsEnabled = true;
            RadioPlayerFirst.IsEnabled = true;
            RadioComputerFirst.IsEnabled = true;
            RadioPlayerO.IsEnabled = true;
            RadioPlayerX.IsEnabled = true;
            ButtonStop.IsEnabled = false;
            ButtonStart.IsEnabled = RadioPlayerFirst.IsChecked == true ? false : true;

            this.gameManager.Finish();
        }

        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            await this.Start();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            this.Finish();
        }

        private void RadioPlayerFirst_Checked(object sender, RoutedEventArgs e)
        {
            this.ButtonStart.IsEnabled = false;
        }

        private void RadioComputerFirst_Checked(object sender, RoutedEventArgs e)
        {
            this.ButtonStart.IsEnabled = true;
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            WaitEvent.Set();
        }
    }
}
