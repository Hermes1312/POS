using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace POS
{
    public partial class Login : Form
    {

        public Login()
        {
            MessageBox.Show("TO JEST\nTO SAMO");
            MessageBox.Show($"CO{Environment.NewLine}TU");

            InitializeComponent();

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Login_Load(object sender, EventArgs e)
        {
            decimal t = 2.50M;

            string tt = t.ToString().Replace(',', '.');

            string quey = $"SELECT * FROM x WHERE y={t}";

            StartPosition = FormStartPosition.CenterScreen;
            int x = (Screen.PrimaryScreen.Bounds.Width / 2) - (Size.Width / 2);
            int y = (Screen.PrimaryScreen.Bounds.Height / 2) - (Size.Height / 2);
            Location = new Point(x, y);

            Thread th = new Thread(TryConnect);
            th.Start();
        }

        private void TryConnect()
        {
            Database.CreateConnection();

            int i = 0; string error = default;

            for (i = 0; i < 3; i++)
            {
                try
                {
                    loadingLabel.Invoke((MethodInvoker)delegate { loadingLabel.Text = $"Connecting to database ({i + 1}/3)..."; });

                    Database.Connection.Open();

                    loadingPanel.Invoke((MethodInvoker)delegate { loadingPanel.Visible = false; });

                    break;
                }

                catch (Exception ex)
                {
                    error = ex.Message;
                }

                Thread.Sleep(2000);
            }

            if (!string.IsNullOrEmpty(error))
            {
                ErrorMsg.Show(error);
            }
        }

        private void SubmitLogin(object sender, EventArgs e)
        {
            Database.GetProducts();

            submitButton.Enabled = false;

            if (Database.ValidCredentials(loginTb.Text, passTb.Text))
            {
                int role = (Database.GetRole(loginTb.Text));

                if (role == 0)
                    new Main().Show();
                else if (role == 1)
                    new Cashier(Database.GetUserId(loginTb.Text).ToString()).Show();

                this.Hide();
            }

            submitButton.Enabled = true;
        }
    }
}
