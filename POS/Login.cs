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
            InitializeComponent();

            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Login_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            var x = (Screen.PrimaryScreen.Bounds.Width / 2) - (Size.Width / 2);
            var y = (Screen.PrimaryScreen.Bounds.Height / 2) - (Size.Height / 2);
            Location = new Point(x, y);

            var th = new Thread(TryConnect);
            th.Start();
        }

        private void TryConnect()
        {
            Database.CreateConnection();

            var i = 0;
            string error = default;

            for (i = 0; i < 3; i++)
            {
                try
                {
                    loadingLabel.Invoke((MethodInvoker) delegate
                    {
                        loadingLabel.Text = $"Connecting to database ({i + 1}/3)...";
                    });

                    Database.Connection.Open();

                    loadingPanel.Invoke((MethodInvoker) delegate { loadingPanel.Visible = false; });

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
                var role = (Database.GetRole(loginTb.Text));

                switch (role)
                {
                    case 0:
                        new Main().Show();
                        break;
                    case 1:
                        new Cashier(Database.GetUserId(loginTb.Text).ToString()).Show();
                        break;
                }

                Hide();
            }

            submitButton.Enabled = true;
        }
    }
}