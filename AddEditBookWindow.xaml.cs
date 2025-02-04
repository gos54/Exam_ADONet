using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Shapes;

namespace WpfApp7
{
    public partial class AddEditBookWindow : Window
    {
        private DataRowView bookRow;
        private string conn;
        public AddEditBookWindow(string con)
        {
            InitializeComponent();
            conn = con;
        }

        public AddEditBookWindow(DataRowView selectedRow, string con) : this(con)
        {
            conn = con;
            bookRow = selectedRow;
            TitleTextBox.Text = bookRow.Row.Field<string>("Title");
            AuthorTextBox.Text = bookRow.Row.Field<string>("Author");
            PublisherTextBox.Text = bookRow.Row.Field<string>("Publisher");
            PagesTextBox.Text = bookRow.Row.Field<int>("Pages").ToString();
            GenreTextBox.Text = bookRow.Row.Field<string>("Genre");
            YearPublishedTextBox.Text = bookRow.Row.Field<int>("YearPublished").ToString();
            SellingPriceTextBox.Text = bookRow.Row.Field<decimal>("CostPrice").ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                SqlCommand command;

                if (bookRow == null)
                {
                    command = new SqlCommand("INSERT INTO Books (Title, Author, Publisher, Pages, Genre, YearPublished, CostPrice, IsSequel, IsONSale, SaleDiscount) VALUES (@Title, @Author, @Publisher, @Pages, @Genre, @YearPublished, @CostPrice, @IsSequel, @IsONSale, @SaleDiscount)", connection);
                }
                else
                {
                    command = new SqlCommand("UPDATE Books SET Title = @Title, Author = @Author, Publisher = @Publisher, Pages = @Pages, Genre = @Genre, YearPublished = @YearPublished, CostPrice = @CostPrice WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", bookRow.Row.Field<int>("Id"));
                }
                command.Parameters.AddWithValue("@Title", TitleTextBox.Text);
                command.Parameters.AddWithValue("@Author", AuthorTextBox.Text);
                command.Parameters.AddWithValue("@Publisher", PublisherTextBox.Text);
                command.Parameters.AddWithValue("@Pages", int.Parse(PagesTextBox.Text));
                command.Parameters.AddWithValue("@Genre", GenreTextBox.Text);
                command.Parameters.AddWithValue("@YearPublished", int.Parse(YearPublishedTextBox.Text));
                command.Parameters.AddWithValue("@CostPrice", decimal.Parse(SellingPriceTextBox.Text));
                command.Parameters.AddWithValue("@IsSequel", false);
                command.Parameters.AddWithValue("@IsOnSale", false);
                command.Parameters.AddWithValue("@SaleDiscount", 0);

                command.ExecuteNonQuery();
                this.Close();
            }
        }
    }
}
