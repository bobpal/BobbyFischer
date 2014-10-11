namespace BobbyFischer
{
    partial class Players
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
            this.computer = new System.Windows.Forms.Button();
            this.human = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // computer
            // 
            this.computer.Location = new System.Drawing.Point(12, 12);
            this.computer.Name = "computer";
            this.computer.Size = new System.Drawing.Size(164, 51);
            this.computer.TabIndex = 0;
            this.computer.Text = "1 Player vs. Computer";
            this.computer.UseVisualStyleBackColor = true;
            this.computer.Click += new System.EventHandler(this.computer_Click);
            // 
            // human
            // 
            this.human.Location = new System.Drawing.Point(12, 69);
            this.human.Name = "human";
            this.human.Size = new System.Drawing.Size(164, 51);
            this.human.TabIndex = 1;
            this.human.Text = "2 Player Duel";
            this.human.UseVisualStyleBackColor = true;
            this.human.Click += new System.EventHandler(this.human_Click);
            // 
            // Players
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 131);
            this.Controls.Add(this.human);
            this.Controls.Add(this.computer);
            this.Name = "Players";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Players";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button computer;
        private System.Windows.Forms.Button human;
    }
}