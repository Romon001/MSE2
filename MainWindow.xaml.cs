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
using Xceed.Wpf.Toolkit;
using System.Collections.ObjectModel;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;
using Microsoft.SolverFoundation.Services;



namespace MSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Data1 = new ObservableCollection<Axes> { new Axes { XValue = 0, YValue = 0 } };
            Data2 = new ObservableCollection<KeyValuePair<double, double>> { };
            Data3 = new ObservableCollection<KeyValuePair<double, double>> { };
            Data4 = new ObservableCollection<KeyValuePair<double, double>> { };
            InitializeComponent();
            dataGrid1.ItemsSource = Data1;
            textBlock1.Text = "Коэффициенты полинома 1й степени";
            textBlock2.Text = "a =  ";
            textBlock3.Text = "b = ";
            textBlock4.Text = "Коэффициенты полинома 2й степени";
            textBlock5.Text = "a = ";
            textBlock6.Text = "b = ";
            textBlock7.Text = "c = ";

        }
        private ObservableCollection<Axes> _data1;
        public ObservableCollection<Axes> Data1 { get { return _data1; } set { _data1 = value; } }
        
        private ObservableCollection<KeyValuePair<double, double>> _data2;
        public ObservableCollection<KeyValuePair<double, double>> Data2 { get { return _data2; } set { _data2 = value;}}
        
        private ObservableCollection<KeyValuePair<double, double>> _data3;
        public ObservableCollection<KeyValuePair<double, double>> Data3 { get { return _data3; } set { _data3 = value; } }
        
        private ObservableCollection<KeyValuePair<double, double>> _data4;
        public ObservableCollection<KeyValuePair<double, double>> Data4 { get { return _data4; } set { _data4 = value; } }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Расстановка исходных точек
            Data2.Clear();
            foreach (Axes a in Data1)
            {
                Data2.Add(new KeyValuePair<double, double>(a.XValue, a.YValue));
            }
            lineSeries1.DataContext = Data2;

            //Линейная функция
            var solver = SolverContext.GetContext();
            solver.ClearModel();
            var model = solver.CreateModel();
            Decision a0 = new Decision(Domain.Real, "a0");
            Decision a1 = new Decision(Domain.Real, "a1");
            model.AddDecision(a0);
            model.AddDecision(a1);
            Term generalError = 0; //функция которую минимизируем
            foreach(Axes a in Data1)
            {
                Term error = Model.Power(a0 + a1 * a.XValue - a.YValue,2);
                generalError += error;
            }
            model.AddGoal("ComplicatedGoal", GoalKind.Minimize, generalError);
            var solution = solver.Solve();
            var solutions = solution.Decisions.ToList();
            var linearB = Math.Round(solutions[0].ToDouble(),3);
            var linearA = Math.Round(solutions[1].ToDouble(), 3);

            
            solver.ClearModel();
            //Квадратичная функция
            var model2 = solver.CreateModel();
            Decision b0 = new Decision(Domain.Real, "b0");
            Decision b1 = new Decision(Domain.Real, "b1");
            Decision b2 = new Decision(Domain.Real, "b2");
            model2.AddDecision(b0);
            model2.AddDecision(b1);
            model2.AddDecision(b2);
            generalError = 0; //функция которую минимизируем
            foreach (Axes a in Data1)
            {
                Term error = Model.Power(b0 + b1 * a.XValue + b2 * a.XValue*a.XValue - a.YValue, 2);
                generalError += error;
            }
            model2.AddGoal("ComplicatedGoal", GoalKind.Minimize, generalError);
            solution = solver.Solve();
            solutions = solution.Decisions.ToList();
            var squaredC = Math.Round(solutions[0].ToDouble(), 3);
            var squaredB = Math.Round(solutions[1].ToDouble(), 3);
            var squaredA = Math.Round(solutions[2].ToDouble(), 3);

            textBlock1.Text = "Коэффициенты полинома 1й степени";
            textBlock2.Text = $"a = {linearA} ";
            textBlock3.Text = $"b = {linearB}";
            textBlock4.Text = "Коэффициенты полинома 2й степени";
            textBlock5.Text = $"a = {squaredA}";
            textBlock6.Text = $"b = {squaredB}";
            textBlock7.Text = $"c = {squaredC}";

            // Отрисовка полиномов
            double minX = Data1[0].XValue;
            double maxX = Data1[0].XValue;
            foreach (Axes a in Data1)
            {
                if (a.XValue < minX)
                {
                    minX = a.XValue;
                }
                if (a.XValue > maxX)
                {
                    maxX = a.XValue;
                }
            }
            Data3.Clear();
            for (double i = minX-2; i <= maxX+2; i += 0.1)
            {
                Data3.Add(new KeyValuePair<double, double>(i, linearA* i+ linearB));
            }
            lineSeries2.DataContext = Data3;
            
            Data4.Clear();
            for (double i = minX-2; i <= maxX+2; i += 0.1)
            {
                Data4.Add(new KeyValuePair<double, double>(i, squaredA*i*i + squaredB *i+ squaredC));
            }
            lineSeries3.DataContext = Data4;
        }
    }

}
