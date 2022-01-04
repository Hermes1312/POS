using System.Windows.Forms;

namespace POS
{
    public class ErrorMsg
    {
        public static void Show(string error) => MessageBox.Show(error, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
