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
    public partial class CustomerForm : Form
    {
        // Connection String
        string connectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;
              AttachDbFilename=""E:\8th Semester\( C# ) Object Oriented Programming 2\Final Term\MY Project\InventoryManagementSystem\InventoryDB.mdf"";
              Integrated Security=True;Connect Timeout=30";

        int selectedCustomerId = -1;

        public CustomerForm()
        {
            InitializeComponent();
        }

        // FORM LOAD
        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        // LOAD CUSTOMERS
        private void LoadCustomers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = "SELECT CustomerId, CustomerName, Address, Phone FROM Customers";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvCustomers.AutoGenerateColumns = true;
                    dgvCustomers.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // CLEAR INPUTS
        private void ClearFields()
        {
            txtCustomerName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            selectedCustomerId = -1;
        }

        // ADD CUSTOMER
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtCustomerName.Text == "" || txtAddress.Text == "" || txtPhone.Text == "")
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
                        "INSERT INTO Customers (CustomerName, Address, Phone) " +
                        "VALUES (@name, @address, @phone)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtCustomerName.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Customer added successfully");

                    LoadCustomers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // GRID ROW CLICK
        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];

                selectedCustomerId = Convert.ToInt32(row.Cells[0].Value);
                txtCustomerName.Text = row.Cells[1].Value.ToString();
                txtAddress.Text = row.Cells[2].Value.ToString();
                txtPhone.Text = row.Cells[3].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId == -1)
            {
                MessageBox.Show("Please select a customer first");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query =
                        "UPDATE Customers SET CustomerName=@name, Address=@address, Phone=@phone " +
                        "WHERE CustomerId=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtCustomerName.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@id", selectedCustomerId);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Customer updated successfully");

                    LoadCustomers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId == -1)
            {
                MessageBox.Show("Please select a customer first");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo);

            if (confirm == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        string query = "DELETE FROM Customers WHERE CustomerId=@id";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", selectedCustomerId);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Customer deleted successfully");

                        LoadCustomers();
                        ClearFields();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query =
                        "SELECT CustomerId, CustomerName, Address, Phone " +
                        "FROM Customers " +
                        "WHERE CustomerName LIKE @search OR Phone LIKE @search";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue(
                        "@search", "%" + txtSearch.Text + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvCustomers.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Refresh clicked");
            txtSearch.Clear();
            LoadCustomers();
        }
    }
}
