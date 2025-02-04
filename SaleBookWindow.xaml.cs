using System;
using System.Data.SqlClient;
using System.Windows;

namespace WpfApp7
{
    public partial class SaleBookWindow : Window
    {
        private string conn;
        public SaleBookWindow(string con)
        {
            InitializeComponent();
            choose.SelectedIndex = 0;
            conn = con;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                SqlCommand command;
                if (choose.SelectedIndex == 0)
                {
                    command = new SqlCommand("UPDATE Books SET CostPrice = CostPrice - (CostPrice / 100 * @SaleDiscount), SaleDiscount = @SaleDiscount WHERE Genre = @Genre", connection);
                    command.Parameters.AddWithValue("@SaleDiscount", percent.Text);
                    command.Parameters.AddWithValue("@Genre", genre.Text);
                }
                else
                {
                    command = new SqlCommand("UPDATE Books SET CostPrice = CostPrice + (CostPrice / 100 * @SaleDiscount), SaleDiscount = @SaleDiscount WHERE Genre = @Genre", connection);
                    command.Parameters.AddWithValue("@SaleDiscount", percent.Text);
                    command.Parameters.AddWithValue("@Genre", genre.Text);
                }
                command.ExecuteNonQuery();
                this.Close();
            }
        }
    }
}
