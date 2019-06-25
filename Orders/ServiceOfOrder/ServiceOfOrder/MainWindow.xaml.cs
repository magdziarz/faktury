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
using System.Globalization;
using Microsoft.Win32;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ServiceOfOrder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> fileList = new List<string>();
        List<Order> ordersList = new List<Order>();
        string error;

        public MainWindow()
        {
            InitializeComponent();
           
          
         
        }

       

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            error = null;
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Multiselect = true;
            filedialog.Filter = "Orders (*.xml;*.json;*.csv)|*.XML;*.JSON;*.CSV";
            Nullable<bool> dialogOK = filedialog.ShowDialog();

         //   ordersList = null;

            if (dialogOK == true)
            {
                    foreach (string file in filedialog.FileNames)
                {
                  
                    if (file.EndsWith(".xml"))
                         {  OpenXml(file); }
                    else if (file.EndsWith(".csv"))
                         {  OpenCsv(file); }
                    else if (file.EndsWith(".json"))
                         {  OpenJson(file); }
                 }
                
            }
            if (error!=null)
            { MessageBox.Show(error);} 
            DataGridTabela.ItemsSource = null;
            DataGridTabela.ItemsSource = ordersList;

            //add list of client
            try
            {
                foreach (Order element in ordersList)
                {
                    if (BoxIDClient.Items.IndexOf(element.ClientId) == -1)
                    { BoxIDClient.Items.Add(element.ClientId); }
                }
            }
            catch { }
           
           


        }
        private void OpenXml(string path)
        {
            try
            {
                XPathDocument sr = new XPathDocument(path);
                XPathNavigator oXPathNavigator = sr.CreateNavigator();
                XPathNodeIterator oOrderNodesIterator = oXPathNavigator.Select("/requests/request");
                
                foreach (XPathNavigator currentOrder in oOrderNodesIterator)
                {
                    Order oneOrder = new Order(
                                    currentOrder.SelectSingleNode("clientId").Value,
                                    Convert.ToInt64(currentOrder.SelectSingleNode("requestId").Value),
                                    currentOrder.SelectSingleNode("name").Value,
                                    Convert.ToInt32(currentOrder.SelectSingleNode("quantity").Value),
                                    Convert.ToDouble(currentOrder.SelectSingleNode("price").Value, new CultureInfo("en-US")));

                    AddtoList(oneOrder,  path);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Nie można odczytać pliku. Błąd: {0}", e.Message);
            }
            
      }

        private void OpenCsv(string path)
        {           
            try
            {
               using (StreamReader sr = new StreamReader(path))
                {
                    if(sr.Peek() >= 0) sr.ReadLine();
                    while (sr.Peek() >= 0)
                    {

                        string[] currentOrder = sr.ReadLine().Split(',');
                        Order oneOrder = new Order(
                                     currentOrder[0],
                                     Convert.ToInt64(currentOrder[1]),
                                     currentOrder[2],
                                     Convert.ToInt32(currentOrder[3]),
                                     Convert.ToDouble(currentOrder[4], new CultureInfo("en-US")));

                        AddtoList(oneOrder,  path);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Nie można odczytać pliku. Błąd: {0}", e.Message);
            }
        }
       
       private void OpenJson(string path)
        {
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string data = r.ReadToEnd();

                    JObject oObject = JObject.Parse(data);
                    var currentOrder = oObject["requests"];
                    for (int i = 0; i < currentOrder.Count(); i++)
                    {
                        //var a1 = oObject["requests"][i]["clientId"];
                        //var a2 = oObject["requests"][i]["requestId"];
                        //var a3 = oObject["requests"][i]["name"];
                        //var a4 = oObject["requests"][i]["quantity"];
                        //var a5 = oObject["requests"][i]["price"];

                        Order oneOrder = new Order(
                            Convert.ToString(oObject["requests"][i]["clientId"]),
                            Convert.ToInt64(oObject["requests"][i]["requestId"]),
                            Convert.ToString(oObject["requests"][i]["name"]),
                            Convert.ToInt32(oObject["requests"][i]["quantity"]),
                            Convert.ToDouble(oObject["requests"][i]["price"]));


                        AddtoList(oneOrder,path);
                    }



                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Nie można odczytać pliku. Błąd: {0}", e.Message);
              
            }

        }

        private void AddtoList(Order oneorder, string path)
        {
            if (oneorder.IfCorrect)
            {
                ordersList.Add(oneorder);
            }
            else
            {
                error += "W pliku: "+Environment.NewLine + path+Environment.NewLine +"wystąpił błąd w  zamówieniu: " + Environment.NewLine+"klieent Id: " + oneorder.ClientId+ ", RequestId: " + oneorder.RequestId + ", Name: " + oneorder.Name + ", Quantity: " + oneorder.Quantity +", Price: "+ oneorder.Price;
            }
        }
        private void CreateReport(object sender, RoutedEventArgs e)
        {
            


          
            if (RBa.IsChecked == true)
            {
                var oList = from oElement in ordersList
                            select new { oElement.ClientId,oElement.RequestId};

                TextBox.Text = "Ilość zamówień: " + oList.Distinct().Count();
                DataGridTabela.ItemsSource = null;
                DataGridTabela.ItemsSource = oList.Distinct();

            }
            if (RBb.IsChecked == true)
            {
                string IDClient = BoxIDClient.Text;
                if (IDClient == "")
                {
                    MessageBox.Show("wybierz ID klienta");
                }
                else
                {
                    var oList = from oElement in ordersList
                                where oElement.ClientId==IDClient
                                select new { oElement.ClientId, oElement.RequestId };

                    TextBox.Text = "Ilość zamówień dla wybranego klienta: " + oList.Distinct().Count();
                    DataGridTabela.ItemsSource = null;
                    DataGridTabela.ItemsSource = oList.Distinct();
                }

            }
            if (RBc.IsChecked == true)
            {
                double suma = 0;
                foreach (Order element in ordersList)
                {
                    suma += element.Price * element.Quantity;
                }
                DataGridTabela.ItemsSource = null;
                TextBox.Text = "Kwota wszystkich zamówień: "+ suma.ToString("0.00");
            }
            if (RBd.IsChecked == true)
            {
                string IDClient = BoxIDClient.Text;
                if (IDClient == "")
                {
                    MessageBox.Show("wybierz ID klienta");
                }
                else
                {
                    double suma = 0;
                    foreach (Order element in ordersList)
                    {
                        if (element.ClientId==IDClient)
                        suma += element.Price * element.Quantity;
                    }
                    DataGridTabela.ItemsSource = null;
                    TextBox.Text = "Kwota wszystkich zamówień dla wybranego klienta: " + suma.ToString("0.00");
                }


             
            }
            if (RBe.IsChecked == true)
            {
                DataGridTabela.ItemsSource = null;
                DataGridTabela.ItemsSource = ordersList;
                TextBox.Text = "Lista wszystkich zamówień";
            }
            if (RBf.IsChecked == true)
            {
                string IDClient = BoxIDClient.Text;
                if (IDClient == "")
                {
                    MessageBox.Show("wybierz ID klienta");
                }
                else
                {
                    var oList = from oElement in ordersList
                                where oElement.ClientId==IDClient
                                select oElement;
                    DataGridTabela.ItemsSource = null;
                    DataGridTabela.ItemsSource = oList;
                    TextBox.Text = "Lista wszystkich zamówień danego klienta";
                }
            }
            if (RBg.IsChecked == true)
            {
                double value = ordersList.Sum(oOrder => (oOrder.Price*oOrder.Quantity));


                var oList = from oElement in ordersList
                            select new { oElement.ClientId, oElement.RequestId };
                value/=oList.Distinct().Count();





                TextBox.Text = "średnia wartość zamówienia "+value.ToString("0.00");
                DataGridTabela.ItemsSource = null;
                DataGridTabela.ItemsSource = oList.Distinct();
            }
            if (RBh.IsChecked == true)
            {
                string IDClient = BoxIDClient.Text;
                if (IDClient == "")
                {
                    MessageBox.Show("wybierz ID klienta");
                }
                else
                {
                    var value = (from oElement in ordersList
                                  where oElement.ClientId == IDClient
                                  select oElement).Sum(p=>p.Price*p.Quantity);

                    var oList = from oElement in ordersList
                                where oElement.ClientId==IDClient
                                select new { oElement.ClientId, oElement.RequestId };

                    value /= oList.Distinct().Count();





                    TextBox.Text = "średnia wartość zamówienia " + value.ToString("0.00");
                    DataGridTabela.ItemsSource = null;
                    DataGridTabela.ItemsSource = oList.Distinct();
                }

            }
            if (RBi.IsChecked == true)
            {
               

                var oList =
                            from p in ordersList
                            group p by p.Name into g
                            select new { Name = g.Key, Count = g.Count() };

                DataGridTabela.ItemsSource = null;
                DataGridTabela.ItemsSource = oList;
                TextBox.Text = "Ilość zamówień pogrupowanych po nazwie";
            }
            if (RBj.IsChecked == true)
            {
                string IDClient = BoxIDClient.Text;
                if (IDClient == "")
                {
                    MessageBox.Show("wybierz ID klienta");
                }
                else
                {
                    var oList =
                           from p in ordersList
                           where p.ClientId==IDClient
                           group p by p.Name into g
                           select new { Name = g.Key, Count = g.Count() };

                    DataGridTabela.ItemsSource = null;
                    DataGridTabela.ItemsSource = oList;
                    TextBox.Text = "Ilość zamówień pogrupowanych po nazwie dla danego klienta";
                }
            }
            if (RBk.IsChecked == true)
            {
                double min;
                double max;
                try { min = Convert.ToDouble(TBmin.Text);} 
                catch { min = 0; }
                try { max = Convert.ToDouble(TBmax.Text); }
                catch { max = (from p in ordersList select p.Price).Max(); }
               
                var oList =
                           from p in ordersList
                          where (p.Price>=min && p. Price<=max)
                           select p;

                DataGridTabela.ItemsSource = null;
                DataGridTabela.ItemsSource = oList;
                TextBox.Text = "Zamówienia w  przedziale cenowym: "+ min.ToString("0.00")+" - "+max.ToString("0.00");
            }

        }
    }
}
