namespace BobbyFischer
{
    partial class NewGame
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
            this.onePlayer = new System.Windows.Forms.RadioButton();
            this.twoPlayer = new System.Windows.Forms.RadioButton();
            this.difficultyPanel = new System.Windows.Forms.Panel();
            this.hard = new System.Windows.Forms.RadioButton();
            this.medium = new System.Windows.Forms.RadioButton();
            this.easy = new System.Windows.Forms.RadioButton();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.logo = new System.Windows.Forms.PictureBox();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.light = new System.Windows.Forms.RadioButton();
            this.dark = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.difficultyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            this.colorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // onePlayer
            // 
            this.onePlayer.AutoSize = true;
            this.onePlayer.Checked = true;
            this.onePlayer.Location = new System.Drawing.Point(15, 30);
            this.onePlayer.Name = "onePlayer";
            this.onePlayer.Size = new System.Drawing.Size(127, 17);
            this.onePlayer.TabIndex = 0;
            this.onePlayer.TabStop = true;
            this.onePlayer.Text = "1 Player vs. computer";
            this.onePlayer.UseVisualStyleBackColor = true;
            this.onePlayer.CheckedChanged += new System.EventHandler(this.onePlayer_CheckedChanged);
            // 
            // twoPlayer
            // 
            this.twoPlayer.AutoSize = true;
            this.twoPlayer.Location = new System.Drawing.Point(15, 60);
            this.twoPlayer.Name = "twoPlayer";
            this.twoPlayer.Size = new System.Drawing.Size(88, 17);
            this.twoPlayer.TabIndex = 1;
            this.twoPlayer.Text = "2 Player Duel";
            this.twoPlayer.UseVisualStyleBackColor = true;
            this.twoPlayer.CheckedChanged += new System.EventHandler(this.twoPlayer_CheckedChanged);
            // 
            // difficultyPanel
            // 
            this.difficultyPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.difficultyPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.difficultyPanel.Controls.Add(this.hard);
            this.difficultyPanel.Controls.Add(this.medium);
            this.difficultyPanel.Controls.Add(this.easy);
            this.difficultyPanel.Location = new System.Drawing.Point(15, 110);
            this.difficultyPanel.Name = "difficultyPanel";
            this.difficultyPanel.Size = new System.Drawing.Size(127, 110);
            this.difficultyPanel.TabIndex = 4;
            // 
            // hard
            // 
            this.hard.AutoSize = true;
            this.hard.Location = new System.Drawing.Point(10, 75);
            this.hard.Name = "hard";
            this.hard.Size = new System.Drawing.Size(48, 17);
            this.hard.TabIndex = 2;
            this.hard.Text = "Hard";
            this.hard.UseVisualStyleBackColor = true;
            // 
            // medium
            // 
            this.medium.AutoSize = true;
            this.medium.Location = new System.Drawing.Point(10, 45);
            this.medium.Name = "medium";
            this.medium.Size = new System.Drawing.Size(62, 17);
            this.medium.TabIndex = 1;
            this.medium.Text = "Medium";
            this.medium.UseVisualStyleBackColor = true;
            // 
            // easy
            // 
            this.easy.AutoSize = true;
            this.easy.Checked = true;
            this.easy.Location = new System.Drawing.Point(10, 15);
            this.easy.Name = "easy";
            this.easy.Size = new System.Drawing.Size(48, 17);
            this.easy.TabIndex = 0;
            this.easy.TabStop = true;
            this.easy.Text = "Easy";
            this.easy.UseVisualStyleBackColor = true;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(75, 245);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 2;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(175, 245);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // logo
            // 
            this.logo.Image = global::BobbyFischer.Properties.Resources.logo;
            this.logo.Location = new System.Drawing.Point(213, 22);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(78, 78);
            this.logo.TabIndex = 2;
            this.logo.TabStop = false;
            // 
            // colorPanel
            // 
            this.colorPanel.Controls.Add(this.dark);
            this.colorPanel.Controls.Add(this.light);
            this.colorPanel.Location = new System.Drawing.Point(175, 110);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(116, 94);
            this.colorPanel.TabIndex = 5;
            // 
            // light
            // 
            this.light.AutoSize = true;
            this.light.Checked = true;
            this.light.Location = new System.Drawing.Point(10, 15);
            this.light.Name = "light";
            this.light.Size = new System.Drawing.Size(81, 17);
            this.light.TabIndex = 0;
            this.light.TabStop = true;
            this.light.Text = "Play as light";
            this.light.UseVisualStyleBackColor = true;
            // 
            // dark
            // 
            this.dark.AutoSize = true;
            this.dark.Location = new System.Drawing.Point(10, 45);
            this.dark.Name = "dark";
            this.dark.Size = new System.Drawing.Size(83, 17);
            this.dark.TabIndex = 1;
            this.dark.Text = "Play as dark";
            this.dark.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(15, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 2);
            this.label1.TabIndex = 3;
            // 
            // NewGame
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(319, 296);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.difficultyPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logo);
            this.Controls.Add(this.twoPlayer);
            this.Controls.Add(this.onePlayer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewGame";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Game";
            this.difficultyPanel.ResumeLayout(false);
            this.difficultyPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            this.colorPanel.ResumeLayout(false);
            this.colorPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton onePlayer;
        private System.Windows.Forms.RadioButton twoPlayer;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Panel difficultyPanel;
        private System.Windows.Forms.RadioButton hard;
        private System.Windows.Forms.RadioButton medium;
        private System.Windows.Forms.RadioButton easy;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Panel colorPanel;
        private System.Windows.Forms.RadioButton dark;
        private System.Windows.Forms.RadioButton light;
        private System.Windows.Forms.Label label1;
    }
}