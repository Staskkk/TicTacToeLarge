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
using Tic_tac_toe.Models;
using Tic_tac_toe.Services;

namespace Tic_tac_toe
{
    public partial class Cells : UserControl
    {
        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register("CellSize", typeof(int), typeof(Cells), new PropertyMetadata(32));

        public static readonly DependencyProperty FieldSizeProperty =
            DependencyProperty.Register("FieldSize", typeof(int), typeof(Cells), new PropertyMetadata(10));

        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(int), typeof(Cells), new PropertyMetadata(20));

        private TextBox[,] cells;

        public Cells()
        {
            this.InitializeComponent();
        }

        public event EventHandler<Move> PlayerMoved;

        public int CellSize
        {
            get { return (int)this.GetValue(CellSizeProperty); }
            set { this.SetValue(CellSizeProperty, value); }
        }

        public int FieldSize
        {
            get
            {
                return (int)GetValue(FieldSizeProperty);
            }

            set
            {
                this.SetFieldSize(value);
                this.SetValue(FieldSizeProperty, value);
            }
        }

        public int TextFontSize
        {
            get { return (int)GetValue(TextFontSizeProperty); }
            set { this.SetValue(TextFontSizeProperty, value); }
        }

        public string PlayerSymbol { get; set; } = "X";

        public string ComputerSymbol { get; set; } = "O";

        public RootState State
        {
            get
            {
                var state = new RootState(this.FieldSize);
                for (int i = 0; i < this.FieldSize; ++i)
                {
                    for (int j = 0; j < this.FieldSize; ++j)
                    {
                        if (this.cells[i, j].Text == this.PlayerSymbol)
                        {
                            state[i, j] = GameManager.PlayerMark;
                        }
                        else if (this.cells[i, j].Text == this.ComputerSymbol)
                        {
                            state[i, j] = GameManager.ComputerMark;
                        }
                        else
                        {
                            state[i, j] = GameManager.EmptyMark;
                        }
                    }
                }

                return state;
            }

            set
            {
                for (int i = 0; i < this.FieldSize; ++i)
                {
                    for (int j = 0; j < this.FieldSize; ++j)
                    {
                        switch (value[i, j])
                        {
                            case GameManager.PlayerMark:
                                this.cells[i, j].Text = this.PlayerSymbol;
                                break;
                            case GameManager.ComputerMark:
                                this.cells[i, j].Text = this.ComputerSymbol;
                                break;
                            default:
                                this.cells[i, j].Text = string.Empty;
                                break;
                        }
                    }
                }
            }
        }

        private void SetFieldSize(int fieldSize)
        {
            if (this.FieldSize == fieldSize && this.cells != null)
            {
                return;
            }

            if (this.cells != null)
            {
                for (int i = 0; i < this.FieldSize; ++i)
                {
                    for (int j = 0; j < this.FieldSize; ++j)
                    {
                        this.GridMain.Children.Remove(this.cells[i, j]);
                    }
                }
            }

            this.cells = new TextBox[fieldSize, fieldSize];
            int marginTop = 0;
            for (int i = 0; i < fieldSize; ++i)
            {
                int marginLeft = 0;
                for (int j = 0; j < fieldSize; ++j)
                {
                    this.cells[i, j] = new TextBox
                    {
                        FontSize = this.TextFontSize,
                        Width = this.CellSize,
                        Height = this.CellSize,
                        Margin = new Thickness
                        {
                            Top = marginTop,
                            Left = marginLeft
                        },
                        IsReadOnly = true,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        MaxLength = 2,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center
                    };
                    this.cells[i, j].GotFocus += this.N_GotFocus;
                    this.GridMain.Children.Add(this.cells[i, j]);
                    marginLeft += this.CellSize;
                }

                marginTop += this.CellSize;
            }
        }

        private void N_GotFocus(object sender, RoutedEventArgs e)
        {
            Move move = this.GetPlayerMove((TextBox)sender);
            this.PlayerMoved?.Invoke(this, move);
            this.Focus();
        }

        private Move GetPlayerMove(TextBox textBox)
        {
            Move move = new Move();
            for (int i = 0; i < this.FieldSize; ++i)
            {
                for (int j = 0; j < this.FieldSize; ++j)
                {
                    if (this.cells[i, j] == textBox)
                    {
                        move.I = i;
                        move.J = j;
                        return move;
                    }
                }
            }

            return move;
        }
    }
}
