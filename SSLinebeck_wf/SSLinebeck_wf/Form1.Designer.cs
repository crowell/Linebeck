namespace SSLinebeck_wf
{
    partial class Form1
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
            this.tbNowPlaying = new System.Windows.Forms.TextBox();
            this.rtbQueue = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // tbNowPlaying
            // 
            this.tbNowPlaying.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNowPlaying.Location = new System.Drawing.Point(13, 24);
            this.tbNowPlaying.Name = "tbNowPlaying";
            this.tbNowPlaying.ReadOnly = true;
            this.tbNowPlaying.Size = new System.Drawing.Size(259, 20);
            this.tbNowPlaying.TabIndex = 0;
            // 
            // rtbQueue
            // 
            this.rtbQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbQueue.Location = new System.Drawing.Point(12, 63);
            this.rtbQueue.Name = "rtbQueue";
            this.rtbQueue.ReadOnly = true;
            this.rtbQueue.Size = new System.Drawing.Size(260, 187);
            this.rtbQueue.TabIndex = 1;
            this.rtbQueue.Text = "";
            this.rtbQueue.TextChanged += new System.EventHandler(this.rtbQueue_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.rtbQueue);
            this.Controls.Add(this.tbNowPlaying);
            this.Name = "Form1";
            this.Text = "SSLinebeck";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbNowPlaying;
        private System.Windows.Forms.RichTextBox rtbQueue;
    }
}

