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
using FlickrNet;


namespace PDFScanningApp
{
  public partial class FormMain : Form
  {
    private AppSettings fAppSettings;
    private Scanner fScanner;
    private bool fClosing;
    private bool fDeleting;
    private Flickr fFlickr;

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

      fFlickr = FlickrManager.GetInstance(fAppSettings);

      // try to see if this is active
      try
      {
        PhotosetCollection col = fFlickr.PhotosetsGetList();
        MessageBox.Show(col.Count.ToString());
      }
      catch(Exception FlickrException)
      {
        MessageBox.Show(FlickrException.Message);
      }
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

      SizeInches size = SizeInches.Letter;

      settings.ScanArea = new BoundsInches(0, 0, size);

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

    private void buttonAuthenticate_Click(object sender, EventArgs e)
    {
      FormAuthenticate formAuth = new FormAuthenticate(fFlickr);
      DialogResult result = formAuth.ShowDialog();
      if(result == DialogResult.OK)
      {
        // let's store this OAuthToken
        FlickrManager.SaveAuthentication(fFlickr);

        // try to get a list of photos
        PhotosetCollection col = fFlickr.PhotosetsGetList();
        MessageBox.Show(col.Count.ToString());
      }
      else
      {

      }
    }
  }
}
