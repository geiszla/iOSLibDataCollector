namespace iOSLibDataCollector
{
    partial class CollectionForm
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
            this.startButton = new System.Windows.Forms.Button();
            this.savePathBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressTextBox = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 98);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(318, 26);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Collect Data!";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // savePathBox
            // 
            this.savePathBox.Location = new System.Drawing.Point(12, 71);
            this.savePathBox.Name = "savePathBox";
            this.savePathBox.ReadOnly = true;
            this.savePathBox.Size = new System.Drawing.Size(237, 20);
            this.savePathBox.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(255, 69);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 34);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(318, 23);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 3;
            // 
            // progressTextBox
            // 
            this.progressTextBox.Location = new System.Drawing.Point(10, 9);
            this.progressTextBox.Name = "progressTextBox";
            this.progressTextBox.Size = new System.Drawing.Size(320, 22);
            this.progressTextBox.TabIndex = 4;
            this.progressTextBox.Text = "Press collect data button to begin!";
            this.progressTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CollectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 136);
            this.Controls.Add(this.progressTextBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.savePathBox);
            this.Controls.Add(this.startButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CollectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Collector";
            this.Shown += new System.EventHandler(this.CollectionForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.TextBox savePathBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressTextBox;
    }
}

