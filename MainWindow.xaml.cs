using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json;

namespace Wpf___CurrencyConverterWithAPI
{
    public partial class MainWindow : Window
    {
        Root val = new Root();

        public class Root
        {
            public Rate rates { get; set; }
            public long timestamp;
            public string license;
        }

        public class Rate
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double NZD { get; set; }
            public double EUR { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            GetValue();
        }

        private async void GetValue()
        {
            val = await GetDataGetMethod<Root>("https://openexchangerates.org/api/latest.json?app_id=69cb235f2fe74f03baeec270066587cf"); //API Link
            BindCurrency();
        }

        public static async Task<Root> GetDataGetMethod<T>(string url)
        {
            var ss = new Root();
            try
            {
                //HttpClient class provides a base class for sending/receiving the HTTP requests/responses from a URL.
                using (var client = new HttpClient())
                {
                    //The timespan to wait before the request times out.
                    client.Timeout = TimeSpan.FromMinutes(1);

                    //HttpResponseMessage is a way of returning a message/data from your action.
                    HttpResponseMessage response = await client.GetAsync(url);

                    //Check API response status code ok
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //Serialize the HTTP content to a string as an asynchronous operation.
                        var ResponceString = await response.Content.ReadAsStringAsync();


                        //JsonConvert.DeserializeObject to deserialize Json to a C#
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString);
                        return ResponceObject;
                    }
                    return ss;
                }
            }
            catch
            {
                return ss;
            }
        }

        #region Bind Currency From and To Combobox
        private void BindCurrency()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Text");

            dt.Columns.Add("Rate");

            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("CZK", val.rates.CZK);

            cmbFromCurrency.ItemsSource = dt.DefaultView;

            cmbFromCurrency.DisplayMemberPath = "Text";

            cmbFromCurrency.SelectedValuePath = "Rate";

            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dt.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Rate";
            cmbToCurrency.SelectedIndex = 0;
        }
        #endregion

        #region Extra Events
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region Button Click Event

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                txtCurrency.Focus();
                return;
            }
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0 || cmbFromCurrency.Text == "--SELECT--")
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0 || cmbToCurrency.Text == "--SELECT--")
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                cmbToCurrency.Focus();
                return;
            }

            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amount textbox value set in ConvertedValue. double.parse is used to change Datatype String To Double. Textbox text have String and ConvertedValue is double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                //Calculation for currency converter is From currency value is multiplied(*) with amount textbox value and then that total is divided(/) with To currency value.                
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());

                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method is used to clear all control values
            ClearControls();
        }
        #endregion
    }
}

