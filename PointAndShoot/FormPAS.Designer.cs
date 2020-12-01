namespace PointAndShoot
{
    partial class FormPAS
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
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.PickButton = new System.Windows.Forms.Button();
            this.CalibrateButton = new System.Windows.Forms.Button();
            this.OptimizeButton = new System.Windows.Forms.Button();
            this.AOCheckBox = new System.Windows.Forms.CheckBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LogTextBox
            // 
            this.LogTextBox.Location = new System.Drawing.Point(12, 59);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.Size = new System.Drawing.Size(366, 293);
            this.LogTextBox.TabIndex = 0;
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(12, 12);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 41);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Find PA";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // PickButton
            // 
            this.PickButton.Location = new System.Drawing.Point(107, 12);
            this.PickButton.Name = "PickButton";
            this.PickButton.Size = new System.Drawing.Size(75, 41);
            this.PickButton.TabIndex = 2;
            this.PickButton.Text = "Pick Star";
            this.PickButton.UseVisualStyleBackColor = true;
            this.PickButton.Click += new System.EventHandler(this.PickButton_Click);
            // 
            // CalibrateButton
            // 
            this.CalibrateButton.Location = new System.Drawing.Point(303, 12);
            this.CalibrateButton.Name = "CalibrateButton";
            this.CalibrateButton.Size = new System.Drawing.Size(75, 41);
            this.CalibrateButton.TabIndex = 3;
            this.CalibrateButton.Text = "Calibrate Guider";
            this.CalibrateButton.UseVisualStyleBackColor = true;
            this.CalibrateButton.Click += new System.EventHandler(this.CalibrateButton_Click);
            // 
            // OptimizeButton
            // 
            this.OptimizeButton.Location = new System.Drawing.Point(208, 12);
            this.OptimizeButton.Name = "OptimizeButton";
            this.OptimizeButton.Size = new System.Drawing.Size(75, 41);
            this.OptimizeButton.TabIndex = 4;
            this.OptimizeButton.Text = "Optimize Exposure";
            this.OptimizeButton.UseVisualStyleBackColor = true;
            this.OptimizeButton.Click += new System.EventHandler(this.OptimizeButton_Click);
            // 
            // AOCheckBox
            // 
            this.AOCheckBox.AutoSize = true;
            this.AOCheckBox.Location = new System.Drawing.Point(12, 371);
            this.AOCheckBox.Name = "AOCheckBox";
            this.AOCheckBox.Size = new System.Drawing.Size(41, 17);
            this.AOCheckBox.TabIndex = 5;
            this.AOCheckBox.Text = "AO";
            this.AOCheckBox.UseVisualStyleBackColor = true;
            this.AOCheckBox.CheckedChanged += new System.EventHandler(this.AOCheckBox_CheckedChanged);
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(303, 358);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 41);
            this.CloseButton.TabIndex = 6;
            this.CloseButton.Text = "Done";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // FormPAS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 407);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.AOCheckBox);
            this.Controls.Add(this.OptimizeButton);
            this.Controls.Add(this.CalibrateButton);
            this.Controls.Add(this.PickButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.LogTextBox);
            this.Name = "FormPAS";
            this.Text = "Point And Shoot";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button PickButton;
        private System.Windows.Forms.Button CalibrateButton;
        private System.Windows.Forms.Button OptimizeButton;
        private System.Windows.Forms.CheckBox AOCheckBox;
        private System.Windows.Forms.Button CloseButton;
    }
}