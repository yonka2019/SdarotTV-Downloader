using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SdarotTV_Downloader
{
    public partial class SearchResultsForm : Form
    {
        public int resultIndex;
        private readonly CancellationTokenSource ts;
        private List<ResultPictureBox> pictures;

        public SearchResultsForm(ReadOnlyCollection<IWebElement> results)
        {
            InitializeComponent();
            ts = new CancellationTokenSource();
            CancellationToken ct = ts.Token;
            pictures = new List<ResultPictureBox>();
            resultIndex = 0;

            Task.Run(() =>
            {
                int i = 0;
                int total = results.Count;
                foreach (var result in results)
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                    ResultPictureBox picture = new ResultPictureBox(i)
                    {
                        Location = new Point(
                        (i % 6) * (Consts.IMAGE_WIDTH + 2 * Consts.IMAGE_MARGIN) + Consts.IMAGE_MARGIN,
                        (i / 6) * (Consts.IMAGE_HEIGHT + 2 * Consts.IMAGE_MARGIN) + Consts.IMAGE_MARGIN),
                        Image = Utils.ResizeImage(Utils.GetImage(result.FindElement(By.TagName("img")).GetAttribute("src")), Consts.IMAGE_WIDTH, Consts.IMAGE_HEIGHT)
                    };
                    picture.Click += Picture_Click;
                    pictures.Add(picture);
                    string loadingLabelText = "Loading... " + (100 * i / total).ToString() + "%";
                    try
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            Results_Panel.Controls.Add(picture);
                            Loading_Label.Text = loadingLabelText;
                        });
                    }
                    catch { }
                    i++;
                }
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        Results_Panel.AutoScroll = true;
                        Loading_Label.Text = "";
                    });
                }
                catch { }
            });
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            pictures[resultIndex].BorderStyle = BorderStyle.None;
            ResultPictureBox selectedPicture = sender as ResultPictureBox;
            resultIndex = selectedPicture.resultIndex;
            selectedPicture.BorderStyle = BorderStyle.Fixed3D;
            Ok_Button.Enabled = true;
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SearchResultsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ts.Cancel();
        }

        private void Ok_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
