namespace KinectLogin
{
    partial class SetupLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupLogin));
            this.numGesturesLabel = new System.Windows.Forms.Label();
            this.numGestures = new System.Windows.Forms.TextBox();
            this.setupGestures = new System.Windows.Forms.Button();
            this.setupVocalCommand = new System.Windows.Forms.Button();
            this.setupFacialRecognition = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // numGesturesLabel
            // 
            this.numGesturesLabel.AutoSize = true;
            this.numGesturesLabel.Location = new System.Drawing.Point(34, 25);
            this.numGesturesLabel.Name = "numGesturesLabel";
            this.numGesturesLabel.Size = new System.Drawing.Size(145, 13);
            this.numGesturesLabel.TabIndex = 0;
            this.numGesturesLabel.Text = "Number of Security Gestures:";
            // 
            // numGestures
            // 
            this.numGestures.Location = new System.Drawing.Point(185, 22);
            this.numGestures.Name = "numGestures";
            this.numGestures.Size = new System.Drawing.Size(33, 20);
            this.numGestures.TabIndex = 1;
            this.numGestures.Text = "3";
            this.numGestures.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // setupGestures
            // 
            this.setupGestures.Location = new System.Drawing.Point(37, 51);
            this.setupGestures.Name = "setupGestures";
            this.setupGestures.Size = new System.Drawing.Size(181, 23);
            this.setupGestures.TabIndex = 2;
            this.setupGestures.Text = "Setup Security Gestures";
            this.setupGestures.UseVisualStyleBackColor = true;
            this.setupGestures.Click += new System.EventHandler(this.setupGestures_Click);
            // 
            // setupVocalCommand
            // 
            this.setupVocalCommand.Location = new System.Drawing.Point(37, 80);
            this.setupVocalCommand.Name = "setupVocalCommand";
            this.setupVocalCommand.Size = new System.Drawing.Size(181, 23);
            this.setupVocalCommand.TabIndex = 3;
            this.setupVocalCommand.Text = "Setup Voice Command";
            this.setupVocalCommand.UseVisualStyleBackColor = true;
            this.setupVocalCommand.Click += new System.EventHandler(this.setupVocalCommand_Click);
            // 
            // setupFacialRecognition
            // 
            this.setupFacialRecognition.Location = new System.Drawing.Point(37, 110);
            this.setupFacialRecognition.Name = "setupFacialRecognition";
            this.setupFacialRecognition.Size = new System.Drawing.Size(181, 23);
            this.setupFacialRecognition.TabIndex = 4;
            this.setupFacialRecognition.Text = "Setup Facial Recognition";
            this.setupFacialRecognition.UseVisualStyleBackColor = true;
            this.setupFacialRecognition.Click += new System.EventHandler(this.setupFacialRecognition_Click);
            // 
            // SetupLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 160);
            this.Controls.Add(this.setupFacialRecognition);
            this.Controls.Add(this.setupVocalCommand);
            this.Controls.Add(this.setupGestures);
            this.Controls.Add(this.numGestures);
            this.Controls.Add(this.numGesturesLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupLogin";
            this.Text = "Setup Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label numGesturesLabel;
        private System.Windows.Forms.TextBox numGestures;
        private System.Windows.Forms.Button setupGestures;
        private System.Windows.Forms.Button setupVocalCommand;
        private System.Windows.Forms.Button setupFacialRecognition;
    }
}

