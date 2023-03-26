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
            this.btn_importBeamData = new System.Windows.Forms.Button();
            this.btn_importSlabData = new System.Windows.Forms.Button();
            this.btn_importWallData = new System.Windows.Forms.Button();
            this.lb_beam = new System.Windows.Forms.Label();
            this.lb_slab = new System.Windows.Forms.Label();
            this.lb_wall = new System.Windows.Forms.Label();
            this.btn_clearInput = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(288, 147);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(99, 47);
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
            this.cbo_ModelDirection.Location = new System.Drawing.Point(31, 161);
            this.cbo_ModelDirection.Name = "cbo_ModelDirection";
            this.cbo_ModelDirection.Size = new System.Drawing.Size(99, 21);
            this.cbo_ModelDirection.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Model Direction";
            // 
            // btn_importBeamData
            // 
            this.btn_importBeamData.Location = new System.Drawing.Point(31, 32);
            this.btn_importBeamData.Name = "btn_importBeamData";
            this.btn_importBeamData.Size = new System.Drawing.Size(99, 29);
            this.btn_importBeamData.TabIndex = 4;
            this.btn_importBeamData.Text = "Import beam data";
            this.btn_importBeamData.UseVisualStyleBackColor = true;
            this.btn_importBeamData.Click += new System.EventHandler(this.btn_importBeamData_Click);
            // 
            // btn_importSlabData
            // 
            this.btn_importSlabData.Location = new System.Drawing.Point(161, 32);
            this.btn_importSlabData.Name = "btn_importSlabData";
            this.btn_importSlabData.Size = new System.Drawing.Size(99, 29);
            this.btn_importSlabData.TabIndex = 4;
            this.btn_importSlabData.Text = "Import slab data";
            this.btn_importSlabData.UseVisualStyleBackColor = true;
            this.btn_importSlabData.Click += new System.EventHandler(this.btn_importSlabData_Click);
            // 
            // btn_importWallData
            // 
            this.btn_importWallData.Location = new System.Drawing.Point(288, 32);
            this.btn_importWallData.Name = "btn_importWallData";
            this.btn_importWallData.Size = new System.Drawing.Size(99, 29);
            this.btn_importWallData.TabIndex = 4;
            this.btn_importWallData.Text = "Import wall data";
            this.btn_importWallData.UseVisualStyleBackColor = true;
            this.btn_importWallData.Click += new System.EventHandler(this.btn_importWallData_Click);
            // 
            // lb_beam
            // 
            this.lb_beam.AutoSize = true;
            this.lb_beam.Location = new System.Drawing.Point(31, 68);
            this.lb_beam.Name = "lb_beam";
            this.lb_beam.Size = new System.Drawing.Size(31, 13);
            this.lb_beam.TabIndex = 5;
            this.lb_beam.Text = "none";
            // 
            // lb_slab
            // 
            this.lb_slab.AutoSize = true;
            this.lb_slab.Location = new System.Drawing.Point(158, 68);
            this.lb_slab.Name = "lb_slab";
            this.lb_slab.Size = new System.Drawing.Size(31, 13);
            this.lb_slab.TabIndex = 6;
            this.lb_slab.Text = "none";
            // 
            // lb_wall
            // 
            this.lb_wall.AutoSize = true;
            this.lb_wall.Location = new System.Drawing.Point(285, 68);
            this.lb_wall.Name = "lb_wall";
            this.lb_wall.Size = new System.Drawing.Size(31, 13);
            this.lb_wall.TabIndex = 7;
            this.lb_wall.Text = "none";
            this.lb_wall.Click += new System.EventHandler(this.label2_Click);
            // 
            // btn_clearInput
            // 
            this.btn_clearInput.Location = new System.Drawing.Point(161, 147);
            this.btn_clearInput.Name = "btn_clearInput";
            this.btn_clearInput.Size = new System.Drawing.Size(99, 47);
            this.btn_clearInput.TabIndex = 8;
            this.btn_clearInput.Text = "Clear Input";
            this.btn_clearInput.UseVisualStyleBackColor = true;
            this.btn_clearInput.Click += new System.EventHandler(this.btn_clearInput_Click);
            // 
            // Revit_Import
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 287);
            this.Controls.Add(this.btn_clearInput);
            this.Controls.Add(this.lb_wall);
            this.Controls.Add(this.lb_slab);
            this.Controls.Add(this.lb_beam);
            this.Controls.Add(this.btn_importWallData);
            this.Controls.Add(this.btn_importSlabData);
            this.Controls.Add(this.btn_importBeamData);
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
        private System.Windows.Forms.Button btn_importBeamData;
        private System.Windows.Forms.Button btn_importSlabData;
        private System.Windows.Forms.Button btn_importWallData;
        private System.Windows.Forms.Label lb_beam;
        private System.Windows.Forms.Label lb_slab;
        private System.Windows.Forms.Label lb_wall;
        private System.Windows.Forms.Button btn_clearInput;
    }
}

