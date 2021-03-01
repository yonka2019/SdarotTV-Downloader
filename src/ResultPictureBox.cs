using System.Drawing;
using System.Windows.Forms;

namespace SdarotTV_Downloader
{
    class ResultPictureBox : Guna.UI2.WinForms.Guna2PictureBox
    {
        public int resultIndex;
        
        public ResultPictureBox(int resultIndex)
        {
            this.resultIndex = resultIndex;
            Size = new Size(Consts.IMAGE_WIDTH, Consts.IMAGE_HEIGHT);
            SizeMode = PictureBoxSizeMode.StretchImage;
            BorderRadius = 5;
        }
    }
}
