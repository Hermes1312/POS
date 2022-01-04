using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace POS
{
    public partial class Cashier : Form
    {
        public Cashier(string id)
        {
            InitializeComponent();
            cashierID.Text = "Cashier ID: " + id;
            dateTime.Text = "Date: " + DateTime.Now;
            CreateTransaction();
        }

        private void Cashier_Load(object sender, EventArgs e)
        {
            ticketGrid.BorderStyle = BorderStyle.FixedSingle;
            ticketGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            ticketGrid.GridColor = ColorTranslator.FromHtml("#e4e6e8");
            ticketGrid.DefaultCellStyle.Font = new Font(ticketGrid.DefaultCellStyle.Font, FontStyle.Regular);
            ticketGrid.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#292F36");
            ticketGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ticketGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ticketGrid.AlternatingRowsDefaultCellStyle = ticketGrid.RowsDefaultCellStyle;
        }

        private void dateTimer_Tick(object sender, EventArgs e)
        {
            dateTime.Text = "Date: " + DateTime.Now;
        }

        private void addProdButton_Click(object sender, EventArgs e)
        {
            if (Database.ProductAvailable(barcodeTB.Text, Convert.ToInt32(QtyUpDown.Value)))
            {
                List<string> product = Database.GetProductByBarcode(barcodeTB.Text);
                ticketGrid.Rows.Add(new[] { product[0], product[1], product[2], QtyUpDown.Value.ToString(), (Convert.ToDecimal(product[2]) * QtyUpDown.Value).ToString() });

                totalPriceTB.Text = TotalPrice().ToString();
            }

            else
            {
                MessageBox.Show("There are not so many products in stock!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private decimal TotalPrice()
        {
            List<decimal> totals = new List<decimal>();
            decimal total = 0;

            foreach (DataGridViewRow row in ticketGrid.Rows)
                totals.Add(Convert.ToDecimal(row.Cells[4].Value));

            foreach (decimal tot in totals)
                total += tot;

            return total;
        }

        private void newTransButton_Click(object sender, EventArgs e)
        {
            CreateTransaction();
        }

        private void CreateTransaction()
        {
            ticketGrid.Rows.Clear();
            barcodeTB.Text = "";
            cashGivenTB.Text = "";
            restTB.Text = "";
            totalPriceTB.Text = "";
            QtyUpDown.Value = 1;
            transID.Text = "Transaction ID: " + Database.GetNextTransactionId().ToString();
        }

        private void chargeButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in ticketGrid.Rows)
                {
                    if (!string.IsNullOrEmpty((string)row.Cells[0].Value))
                        Database.AddSale((string)row.Cells[0].Value, (string)row.Cells[1].Value, Convert.ToInt32(row.Cells[3].Value), Convert.ToDecimal(row.Cells[2].Value), (Convert.ToDecimal(row.Cells[3].Value) * Convert.ToDecimal(row.Cells[2].Value)));
                }
            }

            catch (Exception ex)
            {
                ErrorMsg.Show(ex.Message);
            }
        }

        private void chashGivenTB_TextChanged(object sender, EventArgs e)
        {

            try
            {
                decimal totalPrice = Convert.ToDecimal(totalPriceTB.Text);
                decimal cashGiven = Convert.ToDecimal(cashGivenTB.Text);

                restTB.Text = (cashGiven - totalPrice).ToString();
            }

            catch (Exception)
            {
                restTB.Text = "N/A";
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            new Login().Show();
            Close();
        }
    }
}
