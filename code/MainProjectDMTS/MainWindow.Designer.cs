namespace MainProjectDMTS
{
    partial class MainWindow
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsmiWorkWithDB = new System.Windows.Forms.ToolStripMenuItem();
            this.решателиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNeuroNets = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDesisionTrees = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiWorkWithDB,
            this.решателиToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(592, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "MenuStrip";
            // 
            // tsmiWorkWithDB
            // 
            this.tsmiWorkWithDB.Name = "tsmiWorkWithDB";
            this.tsmiWorkWithDB.Size = new System.Drawing.Size(84, 20);
            this.tsmiWorkWithDB.Text = "Работа с БД";
            this.tsmiWorkWithDB.Click += new System.EventHandler(this.tsmiWorkWithDBClick);
            // 
            // решателиToolStripMenuItem
            // 
            this.решателиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNeuroNets,
            this.tsmiDesisionTrees});
            this.решателиToolStripMenuItem.Name = "решателиToolStripMenuItem";
            this.решателиToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.решателиToolStripMenuItem.Text = "Решатели";
            // 
            // tsmiNeuroNets
            // 
            this.tsmiNeuroNets.Name = "tsmiNeuroNets";
            this.tsmiNeuroNets.Size = new System.Drawing.Size(173, 22);
            this.tsmiNeuroNets.Text = "Нейронные сети";
            this.tsmiNeuroNets.Click += new System.EventHandler(this.tsmiNeuroNets_Click);
            // 
            // tsmiDesisionTrees
            // 
            this.tsmiDesisionTrees.Name = "tsmiDesisionTrees";
            this.tsmiDesisionTrees.Size = new System.Drawing.Size(173, 22);
            this.tsmiDesisionTrees.Text = "Деревья решений";
            this.tsmiDesisionTrees.Click += new System.EventHandler(this.tsmiDesisionTrees_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 332);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.Name = "MainWindow";
            this.Text = "Инструментальная система интеллектуального анализа данных";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmiWorkWithDB;
        private System.Windows.Forms.ToolStripMenuItem решателиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiNeuroNets;
        private System.Windows.Forms.ToolStripMenuItem tsmiDesisionTrees;
    }
}

