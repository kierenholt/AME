
using AME_base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AME_addin.View
{

    //cell data 
    //TCS/deadline/attempt - show best pdfscore
    //deadline/question/responses - show best decision

    /// <summary>
    /// Interaction logic for Book.xaml
    /// </summary>
    public partial class Book : UserControl
    {
        GridView gridView;
        IGridViewModelBase gvmBase; 

        public Book()
        {
            InitializeComponent();

            listView.DataContextChanged += listView_DataContextChanged;
            listView.Loaded += listView_Loaded;
            //TVM.outlookTeacher.TCSs. += new DependencyPropertyChangedEventHandler(onTCSSelected);
        }

        void listView_Loaded(object sender, RoutedEventArgs e) { onTCSSelected(); }
        void listView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { onTCSSelected(); }

        //public void onTCSSelected(object sender, DependencyPropertyChangedEventArgs coll)
        public void onTCSSelected()
        {
            //remove old event
            if (listView != null && gvmBase != null)
                listView.SelectionChanged -= listView_SelectionChanged;
            
            gridView = new GridView();
            this.listView.View = gridView;

            //GridViewModelSuper gvmSuper = (this.listView.DataContext as GridViewModelSuper);
            IGridViewModelBase gvmSingle = (this.listView.DataContext as IGridViewModelBase);
            gvmBase = gvmSingle; //gvmSuper as GridViewModelBase ?? gvmSingle as GridViewModelBase;

            //SET ITEMS SOURCE
            listView.ItemsSource = gvmBase.readOnlyRows;
            listView.SelectionChanged += listView_SelectionChanged;

            //FIRST COLUMN - Seat NAMES
                //BUTTON
                FrameworkElementFactory firstColumnButtonFactory = new FrameworkElementFactory(typeof(Button));
                firstColumnButtonFactory.SetValue
                (
                    Button.CommandProperty,
                    new Binding(String.Format("cellClick")) //binds to cellclick in rowviewmodel
                );
                firstColumnButtonFactory.SetValue
                (
                    Button.StyleProperty,
                    (Style)Resources["columnButton"] //in xaml
                );
                //TEXT RUN
                FrameworkElementFactory firstColumnRunFactory = new FrameworkElementFactory(typeof(Run));
                firstColumnRunFactory.SetValue
                (
                    Run.TextProperty,
                    new Binding("value") //binds to value in rowviewmodel
                    {
                        Mode =  BindingMode.OneWay 
                    }
                );
                firstColumnButtonFactory.AppendChild(firstColumnRunFactory);
                
                //FIRST COLUMN
                gridView.Columns.Add
                (
                    new GridViewColumn()
                    {
                        //DATACONTEXT IS ROWVIEWMODEL
                        Header = gvmBase.cornerText,
                        CellTemplate = new DataTemplate()
                        {
                            VisualTree = firstColumnButtonFactory
                        }
                    }
                );

            //COLUMNS AND HEADERS
            //if (gvmSuper != null)
            //{
            //    int i = 0;
            //    foreach (var group in gvmSuper.headers)
            //        gridView.Columns.Add(createSuperColumn(group, ref i, gvmSuper));
            //}
            if (gvmSingle != null)
            {
                int colIndex = 0;
                foreach (var vm in gvmSingle.readOnlyColumns)
                {
                    gridView.Columns.Add(createColumn(vm, colIndex));
                    colIndex++;
                    listView.ItemContainerStyle = (Style)Resources["listViewItem"]; //set row height
                }
            }
        }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
                gvmBase.deselectRow(listView.Items.IndexOf(item));

            foreach (var item in e.AddedItems)
                gvmBase.selectRow(listView.Items.IndexOf(item));
        }

        //CALLED FOR EACH SUPERHEADER
        //private GridViewColumn createSuperColumn(SuperHeaderViewModel headerVM, ref int i, GridViewModelSuper gvm)
        //{
        //    throw new NotImplementedException();

        //    //SuperHeaderVM - key = superheader, group = subheaders
        //    Grid superGrid = new Grid();
        //    superGrid.RowDefinitions.Add(new RowDefinition());
        //    superGrid.RowDefinitions.Add(new RowDefinition());

        //    //ROW 0 - SUPERHEADER TEMPLATE
        //    TextBlock tb = new TextBlock()
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Center,
        //        DataContext = headerVM.Key
        //    };
        //    tb.SetBinding(TextBlock.TextProperty, new Binding(gvm.superHeaderBindingPath));   //binding to super column viewmodel
        //    superGrid.Children.Add(tb);
        //    Grid.SetRow(tb, 0);

        //    //COLUMNGROUP - used in header and celltemplate
        //    //GridViewColumnCollection columnGroup = createColumnGroup(headerVM.VMgroup, ref i, gvm);

        //    //ROW 1 - SUPERHEADER
        //    GridViewHeaderRowPresenter gvrp = new GridViewHeaderRowPresenter()
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Stretch,
        //        //Columns = columnGroup //vms
        //    };
        //    superGrid.Children.Add(gvrp);
        //    Grid.SetRow(gvrp, 1);

        //    //COLUMN TEMPLATE
        //    FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
        //    FrameworkElementFactory gvrpFactory = new FrameworkElementFactory(typeof(GridViewRowPresenter));
        //    //gvrpFactory.SetValue
        //    //(
        //    //    GridViewRowPresenter.ColumnsProperty,
        //    //    columnGroup //vms
        //    //);
        //    gridFactory.AppendChild(gvrpFactory);
        //    DataTemplate dt = new DataTemplate()
        //    {
        //        VisualTree = gridFactory
        //    };

        //    //COlUMN ALTOGETHER
        //    return new GridViewColumn()
        //    {
        //        Header = superGrid,
        //        CellTemplate = dt
        //    };
        //}

        //MULTIPLE 90 DEGREE COLUMN GROUP
        //private GridViewColumnCollection createColumnGroup(IEnumerable<IColumnViewModel<Entity>> columnViewModels, 
        //    ref int i, 
        //    IGridViewModelBase<Entity,Entity,Entity> gvm)
        //{
        //    GridViewColumnCollection gvcColl = new GridViewColumnCollection();
        //    foreach (var columnVM in columnViewModels)
        //    {
        //        gvcColl.Add(createColumn(columnVM, i, gvm.readOnlyCellSettings[columnVM.readOnlyColumnSettings.index]));
        //        i++;
        //    }
        //    return gvcColl;
        //}

        //SINGLE 90 DEGREE COLUMN
        //key viewmodel and column index
        private GridViewColumn createColumn(IColumnViewModel columnVM, int columnIndex)
        {
            //WITH CELL BINDINGS THAT DEPEND ON COLUMN INDEX, AN INDIVIDUAL CELL TEMPLATE IS NEEDED FOR EACH ROW
            //SO NEW CELL TEMPLATE IS GENERATED FOR EACH COLUMN VIA FRAMEWORKELEMENTFACTORY

            //HEADER
            Button headerButton = new Button()
            {
                DataContext = columnVM,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand
            };
            headerButton.SetBinding
            (
                Button.CommandProperty,
                new Binding("cellClick") //binds to cellClick in columnviewmodel
            );
            StackPanel headerSP = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };

            headerButton.Content = headerSP;

            //THIS TEXT BLOCK CONTAINS THE LISTBOX DESCRIPTION NAME E.G. IMAGE
            //TextBlock propertyTB = new TextBlock
            //{
            //    Style = (Style)Resources["rotatedText"],
            //    Text = " : " + cellBinding.listboxDescriptionName
            //};
            //headerSP.Children.Add(propertyTB);
            
            TextBlock headerTB = new TextBlock()
            {
                Style = (Style)Resources["rotatedText"]
            };
            headerSP.Children.Add(headerTB);

            ////HEADER CHECK BOX
            //CheckBox headerCB = new CheckBox()
            //{
            //};
            //headerCB.SetBinding
            //(
            //    CheckBox.VisibilityProperty,
            //    new Binding("top.candidates[AttemptAggregateHeaderViewModel]")
            //    {
            //        Converter = (IValueConverter)Resources["objectToVis"]
            //    }
            //);
            //headerCB.SetBinding
            //(
            //    CheckBox.IsCheckedProperty,
            //    new Binding("candidateHasThis")
            //);
            //headerSP.Children.Add(headerCB);

            headerTB.SetBinding //binding to headerviewmodel key 
            (
                TextBlock.TextProperty,
                new Binding("value")
                {
                    Mode = BindingMode.OneWay
                }
            );

            //TEXTBOX CELL TEMPLATE
            //FrameworkElementFactory tbFactory;

            //tbFactory = new FrameworkElementFactory(typeof(TextBlock));
            //if (!String.IsNullOrEmpty(cellBinding.colorBindingPath)) //background colour
            //{
            //    tbFactory.SetValue
            //        (
            //            TextBlock.BackgroundProperty,
            //            new Binding(String.Format("[{0}].currentItem.{1}", i.ToString(), cellBinding.colorBindingPath))
            //        );
            //}

            //HYPERLINK DOES NOT WRAP AROUND IMAGE. USING BUTTON INSTEAD.
            //BINDING TO ROWVIEWMODEL INDEX (CELLVIEWMODEL)
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.SetValue
            (
                Button.CommandProperty,
                new Binding(String.Format("observableGrouping[{0}].cellClick", columnIndex.ToString()))
            );
            buttonFactory.SetValue
            (
                Button.StyleProperty,
                (Style)Resources["smallButton"]
            );
            if (columnVM.settings.isImage)
            {
                //BINDING TO IMAGE 
                FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
                imageFactory.SetBinding
                (
                    Image.SourceProperty,
                    new Binding(String.Format("observableGrouping[{0}].image", columnIndex.ToString())) //binds to image property of cellviewmodel
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay
                    }
                );
                buttonFactory.AppendChild(imageFactory);
            }
            else
            {
                //BINDING TO STRING
                FrameworkElementFactory runFactory = new FrameworkElementFactory(typeof(Run));
                runFactory.SetBinding
                (
                    Run.TextProperty, //String
                    new Binding(String.Format("observableGrouping[{0}].value", columnIndex.ToString()))
                    {
                        Mode = BindingMode.OneWay
                    }
                );
                runFactory.SetBinding
                (
                    Run.ForegroundProperty, //System.Windows.Media.Brush
                    new Binding(String.Format("observableGrouping[{0}].color", columnIndex.ToString()))
                    {
                        Mode = BindingMode.OneWay
                    }
                );
                buttonFactory.AppendChild(runFactory);
            }
            GridViewColumn gvc = new GridViewColumn()
            {
                Header = headerButton,
                CellTemplate = new DataTemplate()
                {
                    VisualTree = buttonFactory
                }
            };
            //DATACONTEXT IS GridViewModelBase!!
            //this binding sometimes throws error : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=readOnlyCellSettings[0].colWidth; DataItem='GridViewModelBase`3' (HashCode=38149452); target element is 'GridViewColumn' (HashCode=17398479); target property is 'Width' (type 'Double')
            BindingOperations.SetBinding(
                gvc,
                GridViewColumn.WidthProperty,
                new Binding(string.Format("readOnlySettings[{0}].colWidth", gvmBase.readOnlySettings.IndexOf(columnVM.settings).ToString()))
                {
                    Mode = BindingMode.OneWay
                });
            return gvc;
        }

    }
}
