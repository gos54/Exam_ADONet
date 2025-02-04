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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp7
{
    public partial class MainWindow : Window
    {
        private DataTable books_table;
        private string conn = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=SSPI;";
        private int user_id;

        public MainWindow()
        {
            InitializeComponent();
            search_choose.SelectedIndex = 0;
            user_id = 1;
            LoadBooks();
        }

        private void LoadBooks()
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Books", connection);
                books_table = new DataTable();
                adapter.Fill(books_table);
                BooksDataGrid.ItemsSource = books_table.DefaultView;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search_value = SearchBox.Text.ToLower();
            if (search_choose.SelectedIndex == 0)
            { try {
                    var rows = books_table.AsEnumerable().Where(row =>
                    row.Field<string>("Title").ToLower().Contains(search_value));
                BooksDataGrid.ItemsSource = rows.CopyToDataTable().DefaultView;
                    errors.Content = string.Empty;
                }
                catch { errors.Content = "Нет записей"; }
            }
            else if (search_choose.SelectedIndex == 1)
            { try {
                var rows = books_table.AsEnumerable().Where(row =>
                    row.Field<string>("Author").ToLower().Contains(search_value));
                BooksDataGrid.ItemsSource = rows.CopyToDataTable().DefaultView;
                    errors.Content = string.Empty;
                }
                catch { errors.Content = "Нет записей"; }
            }
            else { try {
                    var rows = books_table.AsEnumerable().Where(row =>
                        row.Field<string>("Genre").ToLower().Contains(search_value));
                    BooksDataGrid.ItemsSource = rows.CopyToDataTable().DefaultView;
                    errors.Content = string.Empty;
                }
                catch { errors.Content = "Нет записей"; }
            }
            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            AddEditBookWindow addEditBookWindow = new AddEditBookWindow(conn);
            addEditBookWindow.ShowDialog();
            LoadBooks(); 
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem != null)
            {
                DataRowView selected_row = BooksDataGrid.SelectedItem as DataRowView;
                AddEditBookWindow addEditBookWindow = new AddEditBookWindow(selected_row, conn);
                addEditBookWindow.ShowDialog();
                LoadBooks();
            }
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem != null)
            {
                DataRowView selected_row = BooksDataGrid.SelectedItem as DataRowView;
                int bookId = selected_row.Row.Field<int>("Id");
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM Books WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", bookId);
                    command.ExecuteNonQuery();
                }
                LoadBooks();
            }
        }

        private void BuyBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = BooksDataGrid.SelectedItem as DataRowView;
                int bookId = selectedRow.Row.Field<int>("Id");
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO Sales(UserId, BookId, SaleDate) VALUES(@UserId, @BookId, @SaleDate)", connection);
                    command.Parameters.AddWithValue("@SaleDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@BookId", bookId);
                    command.Parameters.AddWithValue("@UserId", user_id);
                    command.ExecuteNonQuery();
                    command = new SqlCommand("UPDATE Books SET IsSequel='False' WHERE ID=@BookId", connection);
                    command.Parameters.AddWithValue("@BookId", bookId);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("ВЫ успешно купили книгу");
            }
        }
        private void ReservationBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = BooksDataGrid.SelectedItem as DataRowView;
                int bookId = selectedRow.Row.Field<int>("Id");
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("UPDATE Books Set IsSequel='True' WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", bookId);
                    command.ExecuteNonQuery();
                }
                LoadBooks();
                MessageBox.Show("ВЫ успешно зарезервировали книгу");
            }
        }

        private void PopularTitlesButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Books WHERE Id IN (SELECT TOP 10 BookId FROM Sales Group BY BookId ORDER BY COUNT(BookId) DESC)", connection);
                books_table = new DataTable();
                adapter.Fill(books_table);
                BooksDataGrid.ItemsSource = books_table.DefaultView;
            }
        }

        private void PopularAvtorsButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Books WHERE Author IN (SELECT TOP 10 book.Author FROM Sales sale JOIN Books book ON sale.BookId = book.Id Group BY book.Author ORDER BY COUNT(book.Author) DESC)", connection);
                books_table = new DataTable();
                adapter.Fill(books_table);
                BooksDataGrid.ItemsSource = books_table.DefaultView;
            }
        }

        private void PopularGenreButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Books WHERE Genre IN (SELECT TOP 10 book.Genre FROM Sales sale JOIN Books book ON sale.BookId = book.Id Group BY book.Genre ORDER BY COUNT(book.Genre) DESC)", connection);
                books_table = new DataTable();
                adapter.Fill(books_table);
                BooksDataGrid.ItemsSource = books_table.DefaultView;
            }
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }

        private void ShowReservationButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Books WHERE IsSequel='True'", connection);
                books_table = new DataTable();
                adapter.Fill(books_table);
                BooksDataGrid.ItemsSource = books_table.DefaultView;
            }
        }
    }
}
