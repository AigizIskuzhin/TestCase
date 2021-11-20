using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TestCase.Models;

namespace TestCase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OpenFileDialog OpenFileDialog = new ();
        public Map Map { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            SetupOpenFileDialog();
            Viewer.SizeChanged += ViewerOnSizeChanged;
        }

        private void ViewerOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Map is null) return;

            Map.Width = Viewer.ActualWidth;
            Map.Height = Viewer.ActualHeight;
            RenderMap(Map);
        }

        private void SetupOpenFileDialog()
        {
            OpenFileDialog.DefaultExt = ".txt";
            OpenFileDialog.Filter = "Текстовый файл (*.txt)|*.txt";
        }

        private void SetOutputResult(string text) => ListBox.Items.Add(text);

        private void OnClickSelectInputFile(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog.ShowDialog() == false)
            {
                SetOutputResult("Файл не выбран");
                return;
            }

            string path = OpenFileDialog.FileName;
            if (!File.Exists(path))
            {
                SetOutputResult("Файл не существует");
                return;
            }

            var file = new FileInfo(path);
            if (file.Length == 0)
            {
                SetOutputResult("Файл пустой");
                return;
            }
            // фиксация времени выполнения функции
            WatchFunction(() => { TryParseInput(path);}, nameof(TryParseInput));
            

            if (Map == null)
            {
                SetOutputResult("Не удалось загрузить входные данные");
            }

            if (!Map.Items.Any())
            {
                SetOutputResult("Файл с входными данными пуст");
                return;
            }
            
            // фиксация времени выполнения функции
            // проверка принадлежности к квадрату
            WatchFunction(() => { ParsePoints(Map);}, nameof(ParsePoints));
            
            // фиксация времени выполнения функции
            // отрисовка готовой карты
            WatchFunction(() => { RenderMap(Map);}, nameof(RenderMap));

            if (Canvas.Children.Count != 0)
            {
                SetOutputResult("Успешный рендеринг карты");
                if (ShowMarkedPointsBtn.Visibility == Visibility.Hidden)
                    ShowMarkedPointsBtn.Visibility = Visibility.Visible;
            }

        }

        private void WatchFunction(Action action, string target)
        {
            var timer = new Stopwatch();
            timer.Start();

            action.Invoke();
            
            timer.Stop();
            SetOutputResult(target+" выполнено за "+timer.Elapsed.ToString("g"));
        }

        private bool ParsePoints(Map map)
        {
            // Получаем последнее вхождение Квадрата, если таковы имеются
            var square = map.Items.OfType<SquareItem>().LastOrDefault();
            if (square == null)
            {
                SetOutputResult("Не удалось найти квадрат во входных данных");
                return false;
            }
            // получаем все загруженные отрезки
            var lines = map.Items.OfType<LineItem>();

            square.CheckIntersectionsWithLines(lines);
            return true;
        }

        private void TryParseInput(string path)
        {
            var lines = File.ReadAllLines(path);

            Map = new()
            {
                Width = Viewer.ActualWidth,
                Height = Viewer.ActualHeight
            };

            var linesLength = lines.Length;
            for (int i = 0; i < linesLength; i++)
            {
                // убираем пробелы, пытаемся сформировать входные данные по делителю
                // Ключ = Координаты x,y|x,y
                var data = lines[i].Replace(" ", "").Split('=');

                // пропускаем строку, если формат данных не соблюден
                if (data.Length != 2)
                {
                    SetOutputResult("Неверный формат данных КЛЮЧ=x,y|x,y. Строка: "+(i+1));
                    continue;
                }
                
                MapItem currentItem;
                // читаем ключ и создаем соответствующий элемент, иначе пропускаем строку
                switch (data[0])
                {
                    case "s":
                        currentItem = new SquareItem();
                        break;
                    case "l":
                        currentItem = new LineItem();
                        break;
                    default:
                        SetOutputResult("Неверный формат ключа, строка ["+(i+1)+"] пропущена");
                        continue;
                }

                var pointsStroke = data[1];

                // получаем координаты, заранее очистив неверные делители |x,y|x,y| => x,y|x,y
                var points = pointsStroke.Trim('|').Split('|');
                if (points.Length == 0)
                {
                    SetOutputResult("Неверный формат координат, строка ["+(i+1)+"] пропущена");
                    continue;
                }

                var len = points.Length;
                Cord lastPoint = null;
                for (int j = 0; j < len; j++)
                {
                    var point = points[j].Split(',');

                    // Если ячейка, хранящая координату не соответствует формату x,y, то пропускаем
                    if (point.Length != 2)
                    {
                        SetOutputResult("Неверный формат ячейки координат, строка ["+i+++"], ячейка ["+j+++"] пропущена");
                        continue;
                    }
                    
                    // Проверка на числовые значения
                    if (!float.TryParse(point[0], out var x) || !float.TryParse(point[1], out var y))
                    {
                        SetOutputResult("Обнаружены символы в ячейке координат, строка ["+(i+1)+"], ячейка ["+(j+1)+"] пропущена");
                        continue;
                    }
                    
                    // если первая итерация, добавляем начальные экстремумы с первой координаты
                    if (j == 0 && i == 0)
                    {
                        Map.Xmax = x;
                        Map.Xmin = x;
                        Map.Ymax = y;
                        Map.Ymin = y;
                    }
                    // иначе начинаем вычислять со следующими координатами
                    else
                    {
                        if (lastPoint != null && lastPoint.X.Equals(x) && lastPoint.Y.Equals(y))
                        {
                            SetOutputResult("Обнаружен дупликат в ячейке координат, строка [" + (i + 1) + "], ячейка [" + (j + 1) + "] пропущена");
                            continue;
                        }
                        lastPoint = new Cord(x, y);
                        Map.CalculateExtremes(x, y);
                    }

                    currentItem.Points.Add( new Cord(x, y));
                }

                // строим отрезки по точкам
                currentItem.BuildSegments();
                Map.Items.Add(currentItem);
            }
        }

        private void RenderMap(Map map)
        {
            Canvas.Children.Clear();
            var scaledRatioDouble = Map.GetScaledRatioDouble();

            #region Попытки работы с масштабом
            //Canvas.Width = (Math.Abs(Map.Xmin)+Map.Xmax) * sclaredRatioDouble;
            //Canvas.Height = (Math.Abs(Map.Ymin)+Map.Ymax) * sclaredRatioDouble;
            //Canvas.Margin = new Thickness((Math.Abs(Map.Xmin)) * sclaredRatioDouble, (Math.Abs(Map.Ymin)+Map.Ymax) * sclaredRatioDouble, Math.Abs(Map.Xmax) * sclaredRatioDouble, 0);
            #endregion

            // получаем последнее вхождение квадрата, если таковы имеются
            var square = map.Items.OfType<SquareItem>().LastOrDefault();
            List<Segment> segments;
            int len = 0;
            
            if (square != null)
            {
                segments = square.Segments;
                len = segments.Count;
                for (int i = 0; i < len; i++)
                {
                    var segment = segments[i];
                    var line = DrawLine(segment, scaledRatioDouble, Brushes.Red);
                    Canvas.Children.Add(line);
                }
            }

            // перебираем линии
            foreach (var mapItem in map.Items.OfType<LineItem>())
            {
                segments = mapItem.Segments;
                len = segments.Count;
                for (int i = 0; i < len; i++)
                {
                    var segment = segments[i];
                    
                    var color = segment.IsMarked ? Brushes.DeepSkyBlue : Brushes.Black;

                    var line = DrawLine(segment, scaledRatioDouble, color);
                    Canvas.Children.Add(line);
                }
            }
        }

        private Line DrawLine(Segment segment, double scaledRatioDouble, SolidColorBrush color, int thickness = 1)=>new Line
        {
            StrokeThickness = thickness,
            Stroke = color,
            X1 = segment.Start.X * scaledRatioDouble,
            Y1 = -(segment.Start.Y * scaledRatioDouble),
            X2 = segment.End.X * scaledRatioDouble,
            Y2 = -(segment.End.Y * scaledRatioDouble)
        };

        private void OnClickOpenMarkedPointsList(object sender, RoutedEventArgs e)
        {
            string result = "";
            var array = Map.Items.OfType<LineItem>().ToArray();
            var len = array.Length;
            for (int i = 0; i < len; i++)
            {
                var line = array[i];
                var lenS = line.Segments.Count;
                for (int j = 0; j < lenS; j++)
                {
                    var segment = line.Segments[j];
                    if (segment.IsMarked)
                    {
                        result +=
                            $"Линия [{i + 1}], отрезок [{j + 1}], координаты [{segment.Start.X},{segment.Start.Y}]->[{segment.End.X},{segment.End.Y}]\n";
                    }
                }
            }

            MessageBox.Show(result);
        }
    }
}
