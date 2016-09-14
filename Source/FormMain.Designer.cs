namespace PDFScanningApp
{
  partial class FormMain
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
      if(disposing && (components != null))
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.PanelPreview = new System.Windows.Forms.Panel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.ButtonScanLetter = new System.Windows.Forms.Button();
      this.ButtonScanLegal = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.PanelPreview);
      this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel1);
      this.splitContainer1.Size = new System.Drawing.Size(976, 531);
      this.splitContainer1.SplitterDistance = 412;
      this.splitContainer1.TabIndex = 3;
      // 
      // PanelPreview
      // 
      this.PanelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
      this.PanelPreview.Location = new System.Drawing.Point(0, 0);
      this.PanelPreview.Name = "PanelPreview";
      this.PanelPreview.Size = new System.Drawing.Size(560, 463);
      this.PanelPreview.TabIndex = 4;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
      this.flowLayoutPanel1.Controls.Add(this.ButtonScanLetter);
      this.flowLayoutPanel1.Controls.Add(this.ButtonScanLegal);
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 463);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
      this.flowLayoutPanel1.Size = new System.Drawing.Size(560, 68);
      this.flowLayoutPanel1.TabIndex = 1;
      // 
      // ButtonScanLetter
      // 
      this.ButtonScanLetter.Location = new System.Drawing.Point(8, 8);
      this.ButtonScanLetter.Name = "ButtonScanLetter";
      this.ButtonScanLetter.Size = new System.Drawing.Size(131, 52);
      this.ButtonScanLetter.TabIndex = 28;
      this.ButtonScanLetter.Text = "Scan 8.5x11";
      this.ButtonScanLetter.UseVisualStyleBackColor = true;
      this.ButtonScanLetter.Click += new System.EventHandler(this.ButtonScanLetter_Click);
      // 
      // ButtonScanLegal
      // 
      this.ButtonScanLegal.Location = new System.Drawing.Point(145, 8);
      this.ButtonScanLegal.Name = "ButtonScanLegal";
      this.ButtonScanLegal.Size = new System.Drawing.Size(131, 52);
      this.ButtonScanLegal.TabIndex = 31;
      this.ButtonScanLegal.Text = "Scan 8.5x14";
      this.ButtonScanLegal.UseVisualStyleBackColor = true;
      this.ButtonScanLegal.Click += new System.EventHandler(this.ButtonScanLegal_Click);
      // 
      // FormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(976, 531);
      this.Controls.Add(this.splitContainer1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormMain";
      this.Text = "Form1";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.Load += new System.EventHandler(this.FormMain_Load);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Button ButtonScanLetter;
    private System.Windows.Forms.Button ButtonScanLegal;
    private System.Windows.Forms.Panel PanelPreview;
  }
}

