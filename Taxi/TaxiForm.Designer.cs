namespace Taxi
{
    partial class Taxi
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Taxi));
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.mainArea = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainArea)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTimer
            // 
            this.MainTimer.Interval = 1;
            // 
            // mainArea
            // 
            this.mainArea.Cursor = System.Windows.Forms.Cursors.Cross;
            this.mainArea.Location = new System.Drawing.Point(0, 0);
            this.mainArea.Name = "mainArea";
            this.mainArea.Size = new System.Drawing.Size(892, 640);
            this.mainArea.TabIndex = 3;
            this.mainArea.TabStop = false;
            this.mainArea.Click += new System.EventHandler(this.mainArea_Click);
            // 
            // Taxi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 641);
            this.Controls.Add(this.mainArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Taxi";
            this.Text = "Taxi";
            ((System.ComponentModel.ISupportInitialize)(this.mainArea)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.PictureBox mainArea;
    }
}

