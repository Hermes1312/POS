using Guna.UI2.WinForms;
using POS.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static POS.Database;

namespace POS
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            var x = (Screen.PrimaryScreen.Bounds.Width / 2) - (Size.Width / 2);
            var y = (Screen.PrimaryScreen.Bounds.Height / 2) - (Size.Height / 2);
            Location = new Point(x, y);

            GridViewStyler();
        }

        private void GridViewStyler()
        {
            var gridViews = new List<Guna2DataGridView>();


            foreach (Control tabPage in MainMenu.TabPages)
            {
                foreach (Control tabPageControl in tabPage.Controls)
                {
                    gridViews.AddRange(tabPageControl.Controls.OfType<Guna2DataGridView>().ToList());
                    var groupBoxes = tabPageControl.Controls.OfType<Guna2GroupBox>().ToArray();

                    foreach (var gBoxControls in groupBoxes)
                        gridViews.AddRange(gBoxControls.Controls.OfType<Guna2DataGridView>().ToList());
                }
            }

            foreach (var gridView in gridViews)
            {
                gridView.BorderStyle = BorderStyle.FixedSingle;
                gridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                gridView.GridColor = ColorTranslator.FromHtml("#e4e6e8");
                gridView.DefaultCellStyle.Font = new Font(gridView.DefaultCellStyle.Font, FontStyle.Regular);
                gridView.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#292F36");
                gridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridView.AlternatingRowsDefaultCellStyle = gridView.RowsDefaultCellStyle;
            }
        }

        private void ReleaseBox(object sender, EventArgs e)
        {
            var gbox = (Guna2GroupBox) ((Control) sender).Parent;

            var tag = Convert.ToInt32(gbox.Tag);

            ((Control) sender).BackgroundImage = gbox.Size.Height == 40 ? Resources.dropup : Resources.dropdown;

            if (gbox.Size.Height != tag)
            {
                gbox.Size = new Size(gbox.Size.Width, tag);
                AdjustBoxesY(gbox.Parent, gbox, tag);
            }
            else
            {
                gbox.Size = new Size(gbox.Size.Width, 40);
                AdjustBoxesY(gbox.Parent, gbox, tag * (-1));
            }
        }

        private void AdjustBoxesY(Control tpage, Control ex, int offset)
        {
            var indx = 0;

            for (var i = 0; i < tpage.Controls.Count; i++)
            {
                if (tpage.Controls[i] != ex) continue;
                indx = i;
                break;
            }

            for (var i = 0; i < tpage.Controls.Count; i++)
            {
                if (i != indx && i < indx)
                {
                    tpage.Controls[i].Location =
                        new Point(tpage.Controls[i].Location.X, tpage.Controls[i].Location.Y + offset);
                }
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            var test = (Control) sender;
        }

        private void guna2DataGridView1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void opSupplierComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get supplier products
            opProductComboBox.Enabled = true;
            opProductComboBox.Items.AddRange(GetSupplierProducts((sender as Control).Text).ToArray());
            opProductComboBox.SelectedIndex = 0;
        }

        private void opProductComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            opQtyUpDown.Enabled = true;
            opPriceTB.Text = GetSupplierProductPrice((sender as Control).Text).ToString();
            opMarkUpPrice.Text = opPriceTB.Text;

            opTotalPriceTB.Text = (Convert.ToDecimal(opPriceTB.Text) * opQtyUpDown.Value).ToString();
            opTotalMarkupPrice.Text = (Convert.ToDecimal(opMarkUpPrice.Text) * opQtyUpDown.Value).ToString();

            opBarcodeTextBox.Text = GetSupplierProductBarcode(opSupplierComboBox.Text, opProductComboBox.Text);
        }

        private void opQtyUpDown_ValueChanged(object sender, EventArgs e)
        {
            opTotalPriceTB.Text = (Convert.ToDecimal(opPriceTB.Text) * opQtyUpDown.Value).ToString();
            opTotalMarkupPrice.Text = (Convert.ToDecimal(opMarkUpPrice.Text) * opQtyUpDown.Value).ToString();
        }

        private void opMarkUpPrice_TextChanged(object sender, EventArgs e)
        {
            if (opMarkUpPrice.Text.Length <= 0) return;

            try
            {
                opTotalMarkupPrice.Text = (Convert.ToDecimal(opMarkUpPrice.Text) * opQtyUpDown.Value).ToString();
            }

            catch (Exception)
            {
                MessageBox.Show("Only numbers are allowed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                opTotalMarkupPrice.Text = "1";
            }
        }

        private void opPlaceOrderButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SupplierProductAvailable(opSupplierComboBox.Text, opProductComboBox.Text, (int) opQtyUpDown.Value))
                {
                    if (PlaceOrder(opSupplierComboBox.Text, opProductComboBox.Text, opBarcodeTextBox.Text,
                            (int) opQtyUpDown.Value, Convert.ToDecimal(opTotalPriceTB.Text)))
                    {
                        MessageBox.Show("Order placed successfully!", "Action completed", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("There are not so many products in stock!", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                ErrorMsg.Show(ex.Message);
            }
        }

        private void MainMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    ManageProducts();
                    break;

                case 1:
                    ManageOrders();
                    break;

                case 2:
                    ManageSuppliers();
                    break;

                case 3:
                    ManageUsers();
                    break;

                case 4:
                    SalesReport();
                    break;

                case 5:
                    LogOut();
                    break;
            }
        }

        private void ManageProducts()
        {
            // Get All Products
            foreach (var prod in GetProducts())
                allProdsGrid.Rows.Add(prod);

            // Get suppliers
            opSupplierComboBox.Items.AddRange(GetSuppliersName().Distinct().ToArray());
        }

        private void ManageOrders()
        {
            OrdersGridView.Rows.Clear();
            ReceivedOrderGridView.Rows.Clear();

            foreach (var pendingOrder in GetPendingOrders())
                OrdersGridView.Rows.Add(pendingOrder);

            foreach (var receivedOrder in GetReceivedOrders())
                ReceivedOrderGridView.Rows.Add(receivedOrder);

            foreach (DataGridViewColumn col in OrdersGridView.Columns)
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewRow row in OrdersGridView.Rows)
            foreach (DataGridViewCell cell in row.Cells)
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void ManageSuppliers()
        {
            SCC = 0;
            editSuppliersGridView.Rows.Clear();
            AddNewSupplierGridView.Rows.Clear();

            foreach (var supplier in GetSuppliersName().Distinct().ToArray())
            {
                var suppInfo = GetSupplierInfo(supplier);

                editSuppliersGridView.Rows.Add(new[] {supplier, suppInfo[0], suppInfo[1]});
            }
        }

        private void ManageUsers()
        {
            regId.Text = GetNextUserId().ToString();
            allUsersGridView.Rows.Clear();

            foreach (var user in GetAllUsers())
            {
                user[4] = RoleIdToString(Convert.ToInt32(user[4]));
                allUsersGridView.Rows.Add(user);
            }
        }

        private void SalesReport()
        {
            foreach (var sale in GetSales(0))
                weekSalesReportGridView.Rows.Add(sale);

            foreach (var sale in GetSales(1))
                monthSalesReportGridView.Rows.Add(sale);

            foreach (var sale in GetSales(2))
                quarterSalesReportGridView.Rows.Add(sale);

            foreach (var sale in GetSales(3))
                yearSalesReportGridView.Rows.Add(sale);
        }

        private void LogOut()
        {
            new Login().Show();
            Close();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            var SelectedOrders = new List<string[]>();

            foreach (DataGridViewRow row in OrdersGridView.SelectedRows)
            {
                if (!string.IsNullOrEmpty((string) row.Cells[0].Value))
                {
                    var order = new string[7];

                    order[0] = (string) row.Cells[0].Value;
                    order[1] = (string) row.Cells[1].Value;
                    order[2] = (string) row.Cells[2].Value;
                    order[3] = (string) row.Cells[3].Value;
                    order[4] = (string) row.Cells[4].Value;
                    order[5] = (string) row.Cells[5].Value;
                    order[6] = (string) row.Cells[6].Value;

                    SelectedOrders.Add(order);
                }
            }

            try
            {
                if (ConfirmOrder(SelectedOrders))
                    MessageBox.Show("Delivery confirmed successfully!", "Action completed", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                else
                    MessageBox.Show("Something went wrong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            catch (Exception ex)
            {
                ErrorMsg.Show(ex.Message);
            }
        }

        private void AddSupplierButton_Click(object sender, EventArgs e)
        {
            var products = new List<string[]>();

            foreach (DataGridViewRow row in AddNewSupplierGridView.Rows)
            {
                if (!(string.IsNullOrEmpty((string) row.Cells[0].Value) ||
                      string.IsNullOrEmpty((string) row.Cells[1].Value) ||
                      string.IsNullOrEmpty((string) row.Cells[2].Value)))
                {
                    var product = new string[3];

                    product[0] = (string) row.Cells[0].Value;
                    product[1] = (string) row.Cells[1].Value;
                    product[2] = (string) row.Cells[2].Value;

                    products.Add(product);
                }
            }

            try
            {
                if (AddSupplier(guna2TextBox1.Text, guna2TextBox6.Text, guna2TextBox7.Text, products))
                    MessageBox.Show("Supplier has been added successfully!", "Action completed", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                else
                    MessageBox.Show("Something went wrong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            catch (Exception ex)
            {
                ErrorMsg.Show(ex.Message);
            }
        }

        private void guna2GroupBox8_Click(object sender, EventArgs e)
        {
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            guna2Button7.Enabled = false;

            foreach (DataGridViewRow row in editSuppliersProductsGridView.Rows)
            {
                var product = new List<string>();
                product.Add((string) row.Cells[0].Value);
                product.Add((string) row.Cells[1].Value);
                product.Add((string) row.Cells[2].Value);
                product.Add((string) row.Cells[3].Value);
                product.Add((string) row.Cells[4].Value);

                var supplier = new List<string>();
                supplier.Add((string) editSuppliersGridView.SelectedRows[0].Cells[0].Value);
                supplier.Add((string) editSuppliersGridView.SelectedRows[0].Cells[1].Value);
                supplier.Add((string) editSuppliersGridView.SelectedRows[0].Cells[2].Value);

                UpdateSupplier(supplier[0], supplier[1], supplier[2], product);
            }

            guna2Button7.Enabled = true;
        }

        private int SCC = 0; // selection change counter

        private void editSuppliersGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (editSuppliersGridView.SelectedRows.Count <= 0) return;
            if (string.IsNullOrEmpty((string) editSuppliersGridView.SelectedRows[0].Cells[0].Value)) return;

            if (SCC > 0)
            {
                var dr = MessageBox.Show("Do you want to save changes?", "Question", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                MessageBox.Show(dr == DialogResult.Yes ? "yes" : "no");
            }

            SCC++;

            editSuppliersProductsGridView.Rows.Clear();

            foreach (var product in GetSuppliersFullProducts((string) editSuppliersGridView.SelectedRows[0].Cells[0]
                         .Value))
                editSuppliersProductsGridView.Rows.Add(product);
        }

        private void editSuppliersProductsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private string RoleIdToString(int id)
        {
            switch (id)
            {
                case 0: return "Admin";
                case 1: return "Cashier";
            }

            return null;
        }

        private int RoleStringToId(string name)
        {
            switch (name)
            {
                case "Admin": return 0;
                case "Cashier": return 1;
            }

            return -1;
        }

        private void regAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (regPassword.Text == regRepeatPassword.Text)
                {
                    if (!InsertUser(regUsername.Text, regPassword.Text, regFname.Text, regSurname.Text,
                            regDOB.Value.ToString(), regRole.SelectedIndex))
                        MessageBox.Show("Something went wrong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    MessageBox.Show("Passwords are not the same!", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            catch (Exception)
            {
                // ignored
            }

            MessageBox.Show("User has been registered successfully!", "Action completed", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            regAddUser.Text = "";
            regDOB.Value = regDOB.MinDate;
            regFname.Text = "";
            regId.Text = GetNextUserId().ToString();
            regPassword.Text = "";
            regRepeatPassword.Text = "";
            regRole.SelectedIndex = 0;
            regSurname.Text = "";
            regUsername.Text = "";
        }

        private void allUsersGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
        }

        private int UCC = 0;

        private void allUsersGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {
            try
            {
                var user = new List<string>();
                var cells = allUsersGridView.SelectedRows[0].Cells;

                user.Add((string) cells[0].Value);
                user.Add((string) cells[1].Value);
                user.Add((string) cells[2].Value);
                user.Add((string) cells[3].Value);
                user.Add(RoleStringToId((string) cells[4].Value).ToString());

                UpdateUser(user);
            }

            catch (Exception ex)
            {
                ErrorMsg.Show(ex.Message);
            }

            MessageBox.Show("User data has been updated successfully!", "Action completed", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                AddProduct(AddProdName.Text, AddProdBarcode.Text, Convert.ToDecimal(AddProdOrgPrice.Text),
                    Convert.ToDecimal(AddProdMrkupPrice.Text), (int) AddProductNum.Value);
            }

            catch (Exception ex)
            {
                ErrorMsg.Show(ex.Message);
            }

            MessageBox.Show("Product added to database successfully!", "Action completed", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private bool menuState = false;

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (menuState)
            {
                MainMenu.TabButtonSize = new Size(250, MainMenu.TabButtonSize.Height);
                menuHideButton.BackgroundImage = Resources.lArrow;
                menuState = false;
            }

            else
            {
                MainMenu.TabButtonSize = new Size(100, MainMenu.TabButtonSize.Height);
                menuHideButton.BackgroundImage = Resources.rArrow;
                menuState = true;
            }
        }

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
        }
    }
}