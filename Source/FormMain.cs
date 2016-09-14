using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Model;
using Defines;
using Utils;


namespace PDFScanningApp
{
  public partial class FormMain : Form
  {
    private AppSettings fAppSettings;
    private Scanner fScanner;
    private bool fClosing;
    private bool fDeleting;

    // Special UI
    private Cyotek.Windows.Forms.ImageBox PictureBoxPreview;


    public FormMain()
    {
      InitializeComponent();
      this.Text = AppInfo.GetApplicationName();

      // Create PictureBoxPreview from special component;
      this.PictureBoxPreview = new Cyotek.Windows.Forms.ImageBox();
      this.PictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
      this.PictureBoxPreview.TabStop = false;
      this.PictureBoxPreview.BorderStyle = BorderStyle.None;
      this.PictureBoxPreview.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
      this.PanelPreview.Controls.Add(this.PictureBoxPreview);

      fAppSettings = new AppSettings();

      fScanner = new Scanner();

      fClosing = false;
      fDeleting = false;
    }


    private void FormMain_Load(object sender, EventArgs e)
    {
      RefreshControls();

      fScanner.Open(fScanner_OpenCallback);
    }


    private void fScanner_OpenCallback(bool success)
    {
      if (success)
      {
        foreach (string item in fScanner.GetDataSourceNames())
        {
          //ComboBoxScanners.Items.Add(item);

          if (item == fAppSettings.CurrentScanner)
          {
            //ComboBoxScanners.SelectedItem = item;
          }
        }

        RefreshScanner();
      }
    }


    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (fClosing == false)
      {
        fClosing = true;
        e.Cancel = true;
        fScanner.Close(fScanner_CloseCallback);
      }
    }


    private void fScanner_CloseCallback()
    {
      if (fClosing)
      {
        Close();
      }
    }


    private void ComboBoxScanners_SelectedIndexChanged(object sender, EventArgs e)
    {
      RefreshScanner();
    }


    private void RefreshScanner()
    {
      string selectedScanner = "";// (string)ComboBoxScanners.SelectedItem;
      fScanner.SetActiveDataSource(selectedScanner);
      RefreshControls();
    }


    void RefreshControls()
    {

    }

    private void Scan(PageTypeEnum pageType)
    {
      ScanSettings settings = new ScanSettings();

      SizeInches size;

      switch (pageType)
      {
        case PageTypeEnum.Letter:
          {
            size = SizeInches.Letter;
          }
          break;
        case PageTypeEnum.Legal:
          {
            size = SizeInches.Legal;
          }
          break;
        default:
        case PageTypeEnum.Custom:
          {
            size = new SizeInches(fAppSettings.ScannerCustomPageSize.Width, fAppSettings.ScannerCustomPageSize.Height);
          }
          break;
      }

      settings.ScanArea = new BoundsInches(0, 0, size);
      settings.EnableFeeder = fAppSettings.ScannerEnableFeeder;
      settings.ColorMode = fAppSettings.ScannerColorMode;
      settings.Resolution = fAppSettings.ScannerResolution;
      settings.Threshold = fAppSettings.ScannerThreshold;
      settings.Brightness = fAppSettings.ScannerBrightness;
      settings.Contrast = fAppSettings.ScannerContrast;
      settings.ShowSettingsUI = fAppSettings.ScannerUseNativeUI;
      settings.ShowTransferUI = true;

      fScanner.Acquire(settings, fScanner_AcquireCallback);
    }


    private void fScanner_AcquireCallback(Image theImage)
    {
      if (theImage != null)
      {
        // do something with the image
        // TODO
      }
      else
      {
        Utils.Dialogs.ShowError("Scanner failed to start");
      }
    }


    private void ButtonScanLetter_Click(object sender, EventArgs e)
    {
      Scan(PageTypeEnum.Letter);
      RefreshControls();
    }


    private void ButtonScanLegal_Click(object sender, EventArgs e)
    {
      Scan(PageTypeEnum.Legal);
      RefreshControls();
    }
  }
}
