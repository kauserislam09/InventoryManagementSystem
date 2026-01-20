using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace InventoryManagementSystem
{
    public partial class ProductForm : Form
    {
        // 🔹 CONNECTION STRING (SAFE & CLEAN)
        string connectionString =
        @"Data Source=(LocalDB)\MSSQLLocalDB;
          AttachDbFilename=|DataDirectory|\InventoryDB.mdf;
          Integrated Security=True;";

        int selectedProductId = -1;

        public ProductForm()
        {
            InitializeComponent();
        }

        // FORM LOAD
        private void ProductForm_Load(object sender, EventArgs e)
        {
            dgvProducts.AutoGenerateColumns = true;
            LoadProducts();
        }

        // LOAD ALL PRODUCTS
        private void LoadProducts()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query =
                        "SELECT ProductId, ProductName, Price, Quantity FROM dbo.Products";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvProducts.AutoGenerateColumns = true;
                    dgvProducts.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // CLEAR INPUT FIELDS
        private void ClearFields()
        {
            txtProductName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            selectedProductId = -1;
        }

        // ADD PRODUCT
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtProductName.Text.Trim() == "" ||
                txtPrice.Text.Trim() == "" ||
                txtQuantity.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query =
                        "INSERT INTO Products (ProductName, Price, Quantity) " +
                        "VALUES (@name, @price, @qty)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtProductName.Text.Trim());
                    cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                    cmd.Parameters.AddWithValue("@qty", int.Parse(txtQuantity.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Product added successfully");

                    LoadProducts();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        // SELECT PRODUCT FROM GRID
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                selectedProductId = Convert.ToInt32(row.Cells[0].Value);
                txtProductName.Text = row.Cells[1].Value.ToString();
                txtPrice.Text = row.Cells[2].Value.ToString();
                txtQuantity.Text = row.Cells[3].Value.ToString();
            }
        }

        // UPDATE PRODUCT
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Please select a product first");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query =
                        "UPDATE dbo.Products SET ProductName=@name, Price=@price, Quantity=@qty " +
                        "WHERE ProductId=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtProductName.Text);
                    cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(txtPrice.Text));
                    cmd.Parameters.AddWithValue("@qty", Convert.ToInt32(txtQuantity.Text));
                    cmd.Parameters.AddWithValue("@id", selectedProductId);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Product updated successfully");

                    LoadProducts();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // DELETE PRODUCT
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Please select a product first");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this product?",
                "Confirm Delete",
                MessageBoxButtons.YesNo);

            if (confirm == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        string query =
                            "DELETE FROM dbo.Products WHERE ProductId=@id";

                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", selectedProductId);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Product deleted successfully");

                        LoadProducts();
                        ClearFields();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        // 🔹 SEARCH PRODUCT
        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query =
                        "SELECT ProductId, ProductName, Price, Quantity FROM dbo.Products " +
                        "WHERE ProductName LIKE @search";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue(
                        "@search", "%" + txtSearch.Text + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvProducts.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // 🔹 REFRESH
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadProducts();
            ClearFields();
        }
    }
}
