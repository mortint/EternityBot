
namespace Eternity.Forms.Dialogs
{
    partial class TokenForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TokenForm));
            this.textBox_Token = new FlatUI.FlatTextBox();
            this.label_statusToken = new FlatUI.FlatLabel();
            this.flatLabel20 = new FlatUI.FlatLabel();
            this.link_GetToken = new System.Windows.Forms.LinkLabel();
            this.button_Apply = new Moonbyte.MaterialFramework.Controls.FlatButton();
            this.SuspendLayout();
            // 
            // textBox_Token
            // 
            this.textBox_Token.BackColor = System.Drawing.Color.Transparent;
            this.textBox_Token.FocusOnHover = false;
            this.textBox_Token.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_Token.Location = new System.Drawing.Point(11, 92);
            this.textBox_Token.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Token.MaxLength = 32767;
            this.textBox_Token.Multiline = true;
            this.textBox_Token.Name = "textBox_Token";
            this.textBox_Token.ReadOnly = false;
            this.textBox_Token.Size = new System.Drawing.Size(339, 24);
            this.textBox_Token.TabIndex = 98;
            this.textBox_Token.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.textBox_Token.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.textBox_Token.UseSystemPasswordChar = false;
            // 
            // label_statusToken
            // 
            this.label_statusToken.AutoSize = true;
            this.label_statusToken.BackColor = System.Drawing.Color.Transparent;
            this.label_statusToken.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.label_statusToken.ForeColor = System.Drawing.Color.Black;
            this.label_statusToken.Location = new System.Drawing.Point(138, 75);
            this.label_statusToken.Name = "label_statusToken";
            this.label_statusToken.Size = new System.Drawing.Size(93, 13);
            this.label_statusToken.TabIndex = 97;
            this.label_statusToken.Text = "Изменить токен";
            // 
            // flatLabel20
            // 
            this.flatLabel20.AutoSize = true;
            this.flatLabel20.BackColor = System.Drawing.Color.Transparent;
            this.flatLabel20.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.flatLabel20.ForeColor = System.Drawing.Color.Black;
            this.flatLabel20.Location = new System.Drawing.Point(13, 147);
            this.flatLabel20.Name = "flatLabel20";
            this.flatLabel20.Size = new System.Drawing.Size(342, 85);
            this.flatLabel20.TabIndex = 100;
            this.flatLabel20.Text = resources.GetString("flatLabel20.Text");
            this.flatLabel20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // link_GetToken
            // 
            this.link_GetToken.AutoSize = true;
            this.link_GetToken.Location = new System.Drawing.Point(153, 183);
            this.link_GetToken.Name = "link_GetToken";
            this.link_GetToken.Size = new System.Drawing.Size(82, 13);
            this.link_GetToken.TabIndex = 101;
            this.link_GetToken.TabStop = true;
            this.link_GetToken.Text = "vkhost.github.io";
            this.link_GetToken.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_GetToken_LinkClicked);
            // 
            // button_Apply
            // 
            this.button_Apply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.button_Apply.BackgroundColorClicked = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.button_Apply.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.button_Apply.BorderWidth = 0;
            this.button_Apply.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.button_Apply.Location = new System.Drawing.Point(11, 121);
            this.button_Apply.MouseOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Opacity = 0;
            this.button_Apply.OpacityColor = System.Drawing.Color.White;
            this.button_Apply.Size = new System.Drawing.Size(339, 23);
            this.button_Apply.TabIndex = 141;
            this.button_Apply.text = "Применить";
            this.button_Apply.TextColor = System.Drawing.Color.White;
            this.button_Apply.WaveColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
            // 
            // TokenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(364, 243);
            this.Controls.Add(this.button_Apply);
            this.Controls.Add(this.link_GetToken);
            this.Controls.Add(this.flatLabel20);
            this.Controls.Add(this.textBox_Token);
            this.Controls.Add(this.label_statusToken);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TokenForm";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Eternity :: Изменение токена";
            this.Shown += new System.EventHandler(this.TokenForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private FlatUI.FlatTextBox textBox_Token;
        private FlatUI.FlatLabel label_statusToken;
        private FlatUI.FlatLabel flatLabel20;
        private System.Windows.Forms.LinkLabel link_GetToken;
        private Moonbyte.MaterialFramework.Controls.FlatButton button_Apply;
    }
}