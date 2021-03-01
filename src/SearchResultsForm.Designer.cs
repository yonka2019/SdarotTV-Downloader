
namespace SdarotTV_Downloader
{
    partial class SearchResultsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchResultsForm));
            this.Results_Panel = new Guna.UI2.WinForms.Guna2Panel();
            this.Title_Label = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.Loading_Label = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.Ok_Button = new Guna.UI2.WinForms.Guna2Button();
            this.Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // Results_Panel
            // 
            this.Results_Panel.BackColor = System.Drawing.Color.Transparent;
            this.Results_Panel.FillColor = System.Drawing.Color.White;
            this.Results_Panel.Location = new System.Drawing.Point(12, 115);
            this.Results_Panel.Name = "Results_Panel";
            this.Results_Panel.ShadowDecoration.Parent = this.Results_Panel;
            this.Results_Panel.Size = new System.Drawing.Size(772, 323);
            this.Results_Panel.TabIndex = 0;
            // 
            // Title_Label
            // 
            this.Title_Label.BackColor = System.Drawing.Color.Transparent;
            this.Title_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title_Label.Location = new System.Drawing.Point(32, 42);
            this.Title_Label.Name = "Title_Label";
            this.Title_Label.Size = new System.Drawing.Size(242, 39);
            this.Title_Label.TabIndex = 2;
            this.Title_Label.Text = "Choose a series:";
            // 
            // Loading_Label
            // 
            this.Loading_Label.BackColor = System.Drawing.Color.Transparent;
            this.Loading_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.Loading_Label.Location = new System.Drawing.Point(32, 87);
            this.Loading_Label.Name = "Loading_Label";
            this.Loading_Label.Size = new System.Drawing.Size(3, 2);
            this.Loading_Label.TabIndex = 11;
            this.Loading_Label.Text = null;
            // 
            // Ok_Button
            // 
            this.Ok_Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Ok_Button.BackgroundImage")));
            this.Ok_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Ok_Button.BorderRadius = 5;
            this.Ok_Button.BorderThickness = 1;
            this.Ok_Button.CheckedState.Parent = this.Ok_Button;
            this.Ok_Button.CustomImages.Parent = this.Ok_Button;
            this.Ok_Button.Enabled = false;
            this.Ok_Button.FillColor = System.Drawing.Color.Transparent;
            this.Ok_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ok_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Ok_Button.HoverState.Parent = this.Ok_Button;
            this.Ok_Button.Location = new System.Drawing.Point(582, 42);
            this.Ok_Button.Name = "Ok_Button";
            this.Ok_Button.ShadowDecoration.Parent = this.Ok_Button;
            this.Ok_Button.Size = new System.Drawing.Size(93, 38);
            this.Ok_Button.TabIndex = 13;
            this.Ok_Button.Click += new System.EventHandler(this.Ok_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Cancel_Button.BackgroundImage")));
            this.Cancel_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Cancel_Button.BorderRadius = 5;
            this.Cancel_Button.BorderThickness = 1;
            this.Cancel_Button.CheckedState.Parent = this.Cancel_Button;
            this.Cancel_Button.CustomImages.Parent = this.Cancel_Button;
            this.Cancel_Button.FillColor = System.Drawing.Color.Transparent;
            this.Cancel_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Cancel_Button.HoverState.Parent = this.Cancel_Button;
            this.Cancel_Button.Location = new System.Drawing.Point(681, 42);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.ShadowDecoration.Parent = this.Cancel_Button;
            this.Cancel_Button.Size = new System.Drawing.Size(93, 38);
            this.Cancel_Button.TabIndex = 14;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // SearchResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 450);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Ok_Button);
            this.Controls.Add(this.Loading_Label);
            this.Controls.Add(this.Title_Label);
            this.Controls.Add(this.Results_Panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SearchResultsForm";
            this.Text = "SearchResultsForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SearchResultsForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel Results_Panel;
        private Guna.UI2.WinForms.Guna2HtmlLabel Title_Label;
        private Guna.UI2.WinForms.Guna2HtmlLabel Loading_Label;
        private Guna.UI2.WinForms.Guna2Button Ok_Button;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
    }
}