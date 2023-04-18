namespace REVIT_IMPORT
{
    partial class Revit_Import
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Revit_Import));
            this.btn_Create = new System.Windows.Forms.Button();
            this.cbo_ModelDirection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_clearInput = new System.Windows.Forms.Button();
            this.tb_data = new System.Windows.Forms.TextBox();
            this.btn_paste = new System.Windows.Forms.Button();
            this.cbo_middleBeam = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbo_specialBeam = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(208, 171);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(121, 66);
            this.btn_Create.TabIndex = 0;
            this.btn_Create.Text = "Build";
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // cbo_ModelDirection
            // 
            this.cbo_ModelDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ModelDirection.FormattingEnabled = true;
            this.cbo_ModelDirection.Items.AddRange(new object[] {
            "Vertical",
            "Horizontal"});
            this.cbo_ModelDirection.Location = new System.Drawing.Point(12, 163);
            this.cbo_ModelDirection.Name = "cbo_ModelDirection";
            this.cbo_ModelDirection.Size = new System.Drawing.Size(99, 21);
            this.cbo_ModelDirection.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Model Direction";
            // 
            // btn_clearInput
            // 
            this.btn_clearInput.Location = new System.Drawing.Point(314, 81);
            this.btn_clearInput.Name = "btn_clearInput";
            this.btn_clearInput.Size = new System.Drawing.Size(73, 47);
            this.btn_clearInput.TabIndex = 8;
            this.btn_clearInput.Text = "Clear Input";
            this.btn_clearInput.UseVisualStyleBackColor = true;
            this.btn_clearInput.Click += new System.EventHandler(this.btn_clearInput_Click);
            // 
            // tb_data
            // 
            this.tb_data.Location = new System.Drawing.Point(12, 12);
            this.tb_data.Multiline = true;
            this.tb_data.Name = "tb_data";
            this.tb_data.ReadOnly = true;
            this.tb_data.Size = new System.Drawing.Size(281, 116);
            this.tb_data.TabIndex = 9;
            // 
            // btn_paste
            // 
            this.btn_paste.Location = new System.Drawing.Point(314, 12);
            this.btn_paste.Name = "btn_paste";
            this.btn_paste.Size = new System.Drawing.Size(73, 49);
            this.btn_paste.TabIndex = 10;
            this.btn_paste.Text = "Paste";
            this.btn_paste.UseVisualStyleBackColor = true;
            this.btn_paste.Click += new System.EventHandler(this.btn_paste_Click);
            // 
            // cbo_middleBeam
            // 
            this.cbo_middleBeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_middleBeam.FormattingEnabled = true;
            this.cbo_middleBeam.Location = new System.Drawing.Point(12, 211);
            this.cbo_middleBeam.Name = "cbo_middleBeam";
            this.cbo_middleBeam.Size = new System.Drawing.Size(99, 21);
            this.cbo_middleBeam.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 195);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Middle Beam ( if exist )";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 249);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Special Beam ( if exist )";
            // 
            // cbo_specialBeam
            // 
            this.cbo_specialBeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_specialBeam.FormattingEnabled = true;
            this.cbo_specialBeam.Location = new System.Drawing.Point(12, 265);
            this.cbo_specialBeam.Name = "cbo_specialBeam";
            this.cbo_specialBeam.Size = new System.Drawing.Size(99, 21);
            this.cbo_specialBeam.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(208, 265);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 48);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Revit_Import
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 342);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbo_specialBeam);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbo_middleBeam);
            this.Controls.Add(this.btn_paste);
            this.Controls.Add(this.tb_data);
            this.Controls.Add(this.btn_clearInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbo_ModelDirection);
            this.Controls.Add(this.btn_Create);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Revit_Import";
            this.Text = "Revit Import v1.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.ComboBox cbo_ModelDirection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_clearInput;
        private System.Windows.Forms.TextBox tb_data;
        private System.Windows.Forms.Button btn_paste;
        private System.Windows.Forms.ComboBox cbo_middleBeam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbo_specialBeam;
        private System.Windows.Forms.Button button1;
    }
}

