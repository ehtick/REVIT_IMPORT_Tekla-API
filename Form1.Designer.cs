namespace REVIT_IMPORT
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
            this.btn_Create = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cboModuleArea = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(12, 30);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(132, 53);
            this.btn_Create.TabIndex = 0;
            this.btn_Create.Text = "Create";
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(75, 174);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 77);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cboModuleArea
            // 
            this.cboModuleArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModuleArea.FormattingEnabled = true;
            this.cboModuleArea.Items.AddRange(new object[] {
            "TopArea",
            "MiddleArea",
            "BotArea"});
            this.cboModuleArea.Location = new System.Drawing.Point(247, 47);
            this.cboModuleArea.Name = "cboModuleArea";
            this.cboModuleArea.Size = new System.Drawing.Size(121, 21);
            this.cboModuleArea.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(244, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Model Area";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 287);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboModuleArea);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Create);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cboModuleArea;
        private System.Windows.Forms.Label label1;
    }
}

