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
      this.buttonUpload = new System.Windows.Forms.Button();
      this.progressBarUploading = new System.Windows.Forms.ProgressBar();
      this.pictureBoxLastUploaded = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBoxAlbum = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.textBoxNewAlbum = new System.Windows.Forms.TextBox();
      this.PanelPreview = new System.Windows.Forms.Panel();
      this.buttonScan4x6 = new System.Windows.Forms.Button();
      this.buttonScan3x5 = new System.Windows.Forms.Button();
      this.buttonScanCustom = new System.Windows.Forms.Button();
      this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
      this.buttonCreateAlbum = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLastUploaded)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonUpload
      // 
      this.buttonUpload.Location = new System.Drawing.Point(202, 48);
      this.buttonUpload.Name = "buttonUpload";
      this.buttonUpload.Size = new System.Drawing.Size(50, 79);
      this.buttonUpload.TabIndex = 0;
      this.buttonUpload.Text = "Upload <<";
      this.buttonUpload.UseVisualStyleBackColor = true;
      this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
      // 
      // progressBarUploading
      // 
      this.progressBarUploading.ForeColor = System.Drawing.SystemColors.GrayText;
      this.progressBarUploading.Location = new System.Drawing.Point(13, 143);
      this.progressBarUploading.MarqueeAnimationSpeed = 10;
      this.progressBarUploading.Name = "progressBarUploading";
      this.progressBarUploading.Size = new System.Drawing.Size(169, 8);
      this.progressBarUploading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.progressBarUploading.TabIndex = 1;
      // 
      // pictureBoxLastUploaded
      // 
      this.pictureBoxLastUploaded.Location = new System.Drawing.Point(13, 48);
      this.pictureBoxLastUploaded.Name = "pictureBoxLastUploaded";
      this.pictureBoxLastUploaded.Size = new System.Drawing.Size(169, 95);
      this.pictureBoxLastUploaded.TabIndex = 2;
      this.pictureBoxLastUploaded.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 27);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(76, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Last Uploaded";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 201);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(81, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Album Selected";
      // 
      // comboBoxAlbum
      // 
      this.comboBoxAlbum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxAlbum.FormattingEnabled = true;
      this.comboBoxAlbum.Location = new System.Drawing.Point(15, 221);
      this.comboBoxAlbum.Name = "comboBoxAlbum";
      this.comboBoxAlbum.Size = new System.Drawing.Size(167, 21);
      this.comboBoxAlbum.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 282);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(91, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Make New Album";
      // 
      // textBoxNewAlbum
      // 
      this.textBoxNewAlbum.Location = new System.Drawing.Point(15, 299);
      this.textBoxNewAlbum.Name = "textBoxNewAlbum";
      this.textBoxNewAlbum.Size = new System.Drawing.Size(167, 20);
      this.textBoxNewAlbum.TabIndex = 7;
      // 
      // PanelPreview
      // 
      this.PanelPreview.Location = new System.Drawing.Point(269, 12);
      this.PanelPreview.Name = "PanelPreview";
      this.PanelPreview.Size = new System.Drawing.Size(551, 345);
      this.PanelPreview.TabIndex = 8;
      // 
      // buttonScan4x6
      // 
      this.buttonScan4x6.Location = new System.Drawing.Point(269, 363);
      this.buttonScan4x6.Name = "buttonScan4x6";
      this.buttonScan4x6.Size = new System.Drawing.Size(113, 75);
      this.buttonScan4x6.TabIndex = 9;
      this.buttonScan4x6.Text = "Scan 4x6";
      this.buttonScan4x6.UseVisualStyleBackColor = true;
      this.buttonScan4x6.Click += new System.EventHandler(this.buttonScan4x6_Click);
      // 
      // buttonScan3x5
      // 
      this.buttonScan3x5.Location = new System.Drawing.Point(446, 363);
      this.buttonScan3x5.Name = "buttonScan3x5";
      this.buttonScan3x5.Size = new System.Drawing.Size(113, 75);
      this.buttonScan3x5.TabIndex = 10;
      this.buttonScan3x5.Text = "Scan 3x5";
      this.buttonScan3x5.UseVisualStyleBackColor = true;
      this.buttonScan3x5.Click += new System.EventHandler(this.buttonScan3x5_Click);
      // 
      // buttonScanCustom
      // 
      this.buttonScanCustom.Location = new System.Drawing.Point(630, 363);
      this.buttonScanCustom.Name = "buttonScanCustom";
      this.buttonScanCustom.Size = new System.Drawing.Size(113, 75);
      this.buttonScanCustom.TabIndex = 11;
      this.buttonScanCustom.Text = "Scan Custom";
      this.buttonScanCustom.UseVisualStyleBackColor = true;
      this.buttonScanCustom.Click += new System.EventHandler(this.buttonScanCustom_Click);
      // 
      // numericUpDownWidth
      // 
      this.numericUpDownWidth.DecimalPlaces = 1;
      this.numericUpDownWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numericUpDownWidth.Location = new System.Drawing.Point(750, 379);
      this.numericUpDownWidth.Maximum = new decimal(new int[] {
            85,
            0,
            0,
            65536});
      this.numericUpDownWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownWidth.Name = "numericUpDownWidth";
      this.numericUpDownWidth.Size = new System.Drawing.Size(69, 20);
      this.numericUpDownWidth.TabIndex = 12;
      this.numericUpDownWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(747, 365);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(38, 13);
      this.label4.TabIndex = 13;
      this.label4.Text = "Width:";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(747, 402);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(41, 13);
      this.label5.TabIndex = 15;
      this.label5.Text = "Height:";
      // 
      // numericUpDownHeight
      // 
      this.numericUpDownHeight.DecimalPlaces = 1;
      this.numericUpDownHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numericUpDownHeight.Location = new System.Drawing.Point(750, 417);
      this.numericUpDownHeight.Maximum = new decimal(new int[] {
            115,
            0,
            0,
            65536});
      this.numericUpDownHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownHeight.Name = "numericUpDownHeight";
      this.numericUpDownHeight.Size = new System.Drawing.Size(69, 20);
      this.numericUpDownHeight.TabIndex = 14;
      this.numericUpDownHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // buttonCreateAlbum
      // 
      this.buttonCreateAlbum.Location = new System.Drawing.Point(92, 325);
      this.buttonCreateAlbum.Name = "buttonCreateAlbum";
      this.buttonCreateAlbum.Size = new System.Drawing.Size(90, 23);
      this.buttonCreateAlbum.TabIndex = 16;
      this.buttonCreateAlbum.Text = "Create Album";
      this.buttonCreateAlbum.UseVisualStyleBackColor = true;
      this.buttonCreateAlbum.Click += new System.EventHandler(this.buttonCreateAlbum_Click);
      // 
      // label6
      // 
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(15, 350);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(167, 65);
      this.label6.TabIndex = 17;
      this.label6.Text = "Note: Albums only get created on Flickr when you scan the first photo into it.";
      // 
      // FormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(832, 451);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.buttonCreateAlbum);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.numericUpDownHeight);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.numericUpDownWidth);
      this.Controls.Add(this.buttonScanCustom);
      this.Controls.Add(this.buttonScan3x5);
      this.Controls.Add(this.buttonScan4x6);
      this.Controls.Add(this.PanelPreview);
      this.Controls.Add(this.textBoxNewAlbum);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.comboBoxAlbum);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.pictureBoxLastUploaded);
      this.Controls.Add(this.progressBarUploading);
      this.Controls.Add(this.buttonUpload);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "FormMain";
      this.Text = "Form1";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.Load += new System.EventHandler(this.FormMain_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLastUploaded)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonUpload;
    private System.Windows.Forms.ProgressBar progressBarUploading;
    private System.Windows.Forms.PictureBox pictureBoxLastUploaded;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox comboBoxAlbum;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBoxNewAlbum;
    private System.Windows.Forms.Panel PanelPreview;
    private System.Windows.Forms.Button buttonScan4x6;
    private System.Windows.Forms.Button buttonScan3x5;
    private System.Windows.Forms.Button buttonScanCustom;
    private System.Windows.Forms.NumericUpDown numericUpDownWidth;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown numericUpDownHeight;
    private System.Windows.Forms.Button buttonCreateAlbum;
    private System.Windows.Forms.Label label6;

  }
}

