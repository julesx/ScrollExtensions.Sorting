using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using PropertyChanged;
using WPFHelper;

namespace ScrollExtensions.Sorting
{
    [ImplementPropertyChanged]
    public partial class MainWindow : Window
    {
        public IObservableCollection<int> Items { get; set; }
        public SourceCache<int, int> ItemCache;
        public SortController<int> SortController;

        public string SortDirection { get; set; } = "Ascending";

        private RelayCommand _setSortDirection;
        public ICommand CmdSetSortDirection => _setSortDirection ?? (_setSortDirection = new RelayCommand(SetSortDirection));

        public IntSorter IntSorter;

        public bool ShouldBreak;

        public MainWindow()
        {
            InitializeComponent();

            ItemCache = new SourceCache<int, int>(x => x);
            IntSorter = new IntSorter();
            Items = new ObservableCollectionExtended<int>();
            SortController = new SortController<int>(IntSorter);

            for (var i = 0; i < 1000; i++)
            {
                ItemCache.AddOrUpdate(i);
            }

            DataContext = this;

            Connect();
        }

        private void SetSortDirection(object o)
        {
            ShouldBreak = true;
            SortDirection = (string)o;
            IntSorter.Inverter = SortDirection == "Ascending" ? 1 : -1;
            SortController.Resort();
        }

        public void Connect()
        {
            ItemCache
                .Connect()
                .Sort(SortController)
                .Bind(Items)
                .Subscribe();
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            if (ShouldBreak)
            {
                Debugger.Break();
            }
        }
    }

    public class IntSorter : IComparer<int>
    {
        public int Inverter = 1;

        public int Compare(int x, int y)
        {
            return x.CompareTo(y) * Inverter;
        }
    }
}
