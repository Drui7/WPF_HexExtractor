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
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace HexExtractor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int n = 5;
        Label[] l = new Label[n];
        TextBox[] t = new TextBox[n];
        public MainWindow()
        {
            InitializeComponent();
        }
        void atg(UIElement element, int col, int row)
        {
            grid.Children.Add(element);
            Grid.SetColumn(element, col);
            Grid.SetRow(element, row);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            for (int i = 0; i < n; i++)
            {
                l[i] = new Label()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }; atg(l[i], 0, i);
            }
            for (int i = 0; i < n; i++)
            {
                t[i] = new TextBox()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                }; atg(t[i], 1, i);
              if (i > 1) t[i].AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(text_digit));
            }
            l[0].Content = "Имя исходного файла";
            l[1].Content = "Имя файла(ов)";
            l[2].Content = "Адрес начала";
            l[3].Content = "Количество байт";
            l[4].Content = "Количество блоков подряд";
            t[0].Text = "1.txt";
            t[1].Text = "file_name.type";
            t[2].Text = "0";
            t[3].Text = "2";
            t[4].Text = "1";        
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int size = int.Parse(t[3].Text);
            byte[] b = new byte[size];
            int pos = int.Parse(t[2].Text);
            int num = int.Parse(t[4].Text);
            string name_ou = t[1].Text;
            string name_in = t[0].Text;
            bar.Value = 0;
            bar.Maximum = num;
            button.IsEnabled = false;
            DoEvents();
            if (File.Exists(name_in))
            {
                for (int j = 0; j < num; j++)
                {
                    Stream stream = File.OpenRead(name_in);
                    if (pos -1 + (j+1) * size < stream.Length)
                        {               
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            reader.BaseStream.Position = pos + j * size;
                            for (int i = 0; i < size; i++)
                                b[i] = reader.ReadByte();
                            reader.Close();
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open("доля" + j + name_ou, FileMode.OpenOrCreate)))
                        {
                            writer.BaseStream.Position = 0;
                            writer.Write(b);
                            writer.Close();
                        }
                        bar.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));                 
                    } else
                    {
                        bar.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0)); 
                    }
                    bar.Value++;
                    stream.Close();
                    DoEvents();
                }         
            }
            button.IsEnabled = true;
        }
        private void text_digit(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1)) e.Handled = true;
        }
        public static void DoEvents()//Реализация DoEvents в WPF
        {
            if(Application.Current != null) Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate{}));
        }

    }
}
