namespace DesisionTrees
{
    partial class TreeNameInputForm
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
            this.btnConfirm = new System.Windows.Forms.Button();
            this.txtbTreeName = new System.Windows.Forms.TextBox();
            this.lblTreeName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnConfirm
            // 
            this.btnConfirm.Enabled = false;
            this.btnConfirm.Location = new System.Drawing.Point(213, 23);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "OK";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // txtbTreeName
            // 
            this.txtbTreeName.Location = new System.Drawing.Point(12, 38);
            this.txtbTreeName.Name = "txtbTreeName";
            this.txtbTreeName.Size = new System.Drawing.Size(190, 20);
            this.txtbTreeName.TabIndex = 1;
            this.txtbTreeName.TextChanged += new System.EventHandler(this.txtbTreeName_TextChanged);
            // 
            // lblTreeName
            // 
            this.lblTreeName.AutoSize = true;
            this.lblTreeName.Location = new System.Drawing.Point(13, 13);
            this.lblTreeName.Name = "lblTreeName";
            this.lblTreeName.Size = new System.Drawing.Size(111, 13);
            this.lblTreeName.TabIndex = 2;
            this.lblTreeName.Text = "Введите имя дерева";
            // 
            // TreeNameInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 70);
            this.ControlBox = false;
            this.Controls.Add(this.lblTreeName);
            this.Controls.Add(this.txtbTreeName);
            this.Controls.Add(this.btnConfirm);
            this.Name = "TreeNameInputForm";
            this.Text = "TreeNameInputForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.TextBox txtbTreeName;
        private System.Windows.Forms.Label lblTreeName;
    }
}