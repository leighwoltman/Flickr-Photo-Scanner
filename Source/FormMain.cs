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
using System.Text.RegularExpressions;


namespace PDFScanningApp
{
  public partial class FormMain : Form
  {
    private AppSettings fAppSettings;
    private Scanner fScanner;
    private bool fClosing;
    
    private Queue<QueuedMessage> messageQueue;
    private Worker myWorker;
    private System.Windows.Forms.Timer myTimer;
    private List<Album> albumList;
    private string defaultAlbum;

    // Special UI
    private Cyotek.Windows.Forms.ImageBox PictureBoxPreview;


    public FormMain()
    {
      InitializeComponent();

      PauseControls();
      progressBarUploading.Visible = false;

      this.Text = AppInfo.GetApplicationName();

      albumList = null;

      // Create PictureBoxPreview from special component;
      this.PictureBoxPreview = new Cyotek.Windows.Forms.ImageBox();
      this.PictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
      this.PictureBoxPreview.TabStop = false;
      this.PictureBoxPreview.BorderStyle = BorderStyle.None;
      this.PictureBoxPreview.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
      this.PanelPreview.Controls.Add(this.PictureBoxPreview);

      fAppSettings = new AppSettings();

      defaultAlbum = fAppSettings.SelectedAlbumName;
      numericUpDownHeight.Value = (decimal)fAppSettings.CustomHeight;
      numericUpDownWidth.Value = (decimal)fAppSettings.CustomWidth;

      fScanner = new Scanner();

      fClosing = false;

      messageQueue = new Queue<QueuedMessage>();

      myWorker = new Worker(this, fAppSettings);
      Thread workerThread = new Thread(myWorker.Run);
      workerThread.Start();

      myTimer = new System.Windows.Forms.Timer();
      myTimer.Interval = 500;
      myTimer.Tick += myTimer_Tick;
      myTimer.Start();
    }


    void myTimer_Tick(object sender, EventArgs e)
    {
      myTimer.Stop();

      QueuedMessage msgToProcess = null;

      lock (messageQueue)
      {
        if(messageQueue.Count > 0)
        {
          msgToProcess = messageQueue.Dequeue();
        }
      }

      if(msgToProcess != null)
      {
        switch(msgToProcess.msg)
        {
          case "AUTHENTICATE":
            {
              FormAuthenticate formAuth = new FormAuthenticate((string)msgToProcess.payload);
              DialogResult result = formAuth.ShowDialog();
              if(result == DialogResult.OK)
              {
                myWorker.EnqueueMessage(new QueuedMessage("AUTHENTICATION_KEY", formAuth.responseCode));
              }
              else
              {
                myWorker.EnqueueMessage(new QueuedMessage("AUTHENTICATION_KEY", null));
              }
            }
            break;

          case "STATUS":
            {
              RefreshControls((UIStatus)msgToProcess.payload);
            }
            break;

          case "ALBUM_LIST":
            {
              // save the last album we had selected
              string selected_name = defaultAlbum;

              if ((comboBoxAlbum.Items.Count == 1) && (comboBoxAlbum.Items[0].ToString() == "No Albums"))
              {
                // we don't have any items
              }
              else
              {
                if(comboBoxAlbum.SelectedItem != null)
                {
                  selected_name = ((Album)comboBoxAlbum.SelectedItem).name;
                }
              }

              albumList = (List<Album>)msgToProcess.payload;

              // populate the dropdown
              comboBoxAlbum.Items.Clear();
              int toBeSelected = -1;
              int count = 0;
              foreach(Album a in albumList)
              {
                comboBoxAlbum.Items.Add(a);
                if(a.name == selected_name)
                {
                  toBeSelected = count;
                }
                count++;
              }
              if(toBeSelected != -1)
              {
                comboBoxAlbum.SelectedIndex = toBeSelected;
              }
            }
            break;

          case "SCANNED_IMAGE_RETURN":
            {
              Image img = (Image)msgToProcess.payload;
              PictureBoxPreview.Image = img;
              PictureBoxPreview.ZoomToFit();
            }
            break;

          case "LAST_UPLOADED":
            {
              Image img = (Image)msgToProcess.payload;
              pictureBoxLastUploaded.Image = img;
              pictureBoxLastUploaded.SizeMode = PictureBoxSizeMode.Zoom;
            }
            break;

          case "ERROR":
            {
              string message = (string)msgToProcess.payload;
              MessageBox.Show("An error occured, the program will try to recover, but consider shutting it down and restarting. The error message was: " + message, "Error");
            }
            break;
        }
      }

      myTimer.Start();
    }


    public void EnqueueMessage(QueuedMessage msgToQueue)
    {
      lock (messageQueue)
      {
        messageQueue.Enqueue(msgToQueue);
      }
    }


    private void FormMain_Load(object sender, EventArgs e)
    {
      RefreshControls(null);

      fScanner.Open(fScanner_OpenCallback);
    }


    private void fScanner_OpenCallback(bool success)
    {
      bool found = false;

      if (success)
      {
        List<string> scanners = fScanner.GetDataSourceNames();

        foreach (string scanner in scanners)
        {
          if (scanner.Contains("WIA") && scanner.Contains("EPSON") && scanner.Contains("Perfection V500"))
          {
            found = true;
          }
        }
      }

      if(!success || !found)
      {
        MessageBox.Show("The scanner does not seem to be connected or powered up. Check connections and maybe unplug and replug them. Then try the program again.", "Scanner Not Found");
        Application.Exit();
      }
      else
      {
        myWorker.EnqueueMessage(new QueuedMessage("START", null));
      }
    }


    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (fClosing == false)
      {
        myWorker.Stop();
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
      RefreshControls(null);
    }


    void RefreshControls(UIStatus status)
    {
      if(status != null)
      {
        buttonUpload.Enabled = (status.authenticated && !status.uploading && status.scannedImage);
        comboBoxAlbum.Enabled = (status.authenticated && (albumList != null));
        textBoxNewAlbum.Enabled = (status.authenticated && !status.uploading);
        buttonCreateAlbum.Enabled = (status.authenticated && !status.uploading);
        buttonScan4x6.Enabled = (status.authenticated);
        buttonScan3x5.Enabled = (status.authenticated);
        buttonScanCustom.Enabled = (status.authenticated);
        numericUpDownHeight.Enabled = (status.authenticated);
        numericUpDownWidth.Enabled = (status.authenticated);

        progressBarUploading.Visible = (status.uploading);
      }

      if(albumList == null)
      {
        if( (comboBoxAlbum.Items.Count == 1) && (comboBoxAlbum.Items[0].ToString() == "No Albums"))
        {
          // then we are good
        }
        else
        {
          comboBoxAlbum.Items.Clear();
          comboBoxAlbum.Items.Add("No Albums");
          comboBoxAlbum.SelectedIndex = 0;
          comboBoxAlbum.Enabled = false;
        }
      }
    }


    void PauseControls()
    {
      buttonUpload.Enabled = false;
      comboBoxAlbum.Enabled = false;
      textBoxNewAlbum.Enabled = false;
      buttonCreateAlbum.Enabled = false;
      buttonScan4x6.Enabled = false;
      buttonScan3x5.Enabled = false;
      buttonScanCustom.Enabled = false;
      numericUpDownHeight.Enabled = false;
      numericUpDownWidth.Enabled = false;
    }


    private void Scan(SizeInches size)
    {
      List<string> scanners = fScanner.GetDataSourceNames();

      string selectedScanner = "";

      foreach(string scanner in scanners)
      {
        if( scanner.Contains("WIA") && scanner.Contains("EPSON") && scanner.Contains("Perfection V500") )
        {
          selectedScanner = scanner;
        }
      }

      if (selectedScanner == "")
      {
        MessageBox.Show("Scanner not plugged in");
      }
      else
      {
        bool skip = false;

        if(PictureBoxPreview.Image != null)
        {
          DialogResult result = MessageBox.Show("Scan next photo without uploading last?", "Remove Last", MessageBoxButtons.YesNo);

          if(result != DialogResult.Yes)
          {
            skip = true;
          }
        }

        if (!skip)
        {
          PauseControls();

          fScanner.SetActiveDataSource(selectedScanner);

          ScanSettings settings = new ScanSettings();

          settings.ScanArea = new BoundsInches(0, 0, size);
          settings.Brightness = 0.5;
          settings.ColorMode = ColorModeEnum.RGB;
          settings.Contrast = 0.5;
          settings.EnableFeeder = false;
          settings.Resolution = 600;
          settings.ShowSettingsUI = false;
          settings.ShowTransferUI = true;

          fScanner.Acquire(settings, fScanner_AcquireCallback);
        }
      }
    }


    private void fScanner_AcquireCallback(Image theImage)
    {
      if (theImage != null)
      {
        myWorker.EnqueueMessage(new QueuedMessage("SCANNED_IMAGE", theImage));
      }
      else
      {
        Utils.Dialogs.ShowError("Scanner failed to start");
      }
    }

    private void buttonCreateAlbum_Click(object sender, EventArgs e)
    {
      Regex rgx = new Regex("[^a-zA-Z0-9 -]");
      textBoxNewAlbum.Text = rgx.Replace(textBoxNewAlbum.Text.Trim(), "");

      if( String.IsNullOrEmpty(textBoxNewAlbum.Text) )
      {
        MessageBox.Show("Album name must have only the characters, numbers or spaces", "Album Name Error");
      }
      else
      {
        bool found = false;
        // if we have no albums
        if ((comboBoxAlbum.Items.Count == 1) && (comboBoxAlbum.Items[0].ToString() == "No Albums"))
        {
          // then we have no albums, this will be the first
          comboBoxAlbum.Items.Clear();
          comboBoxAlbum.Enabled = true;
        }
        else
        {
          foreach (Album a in comboBoxAlbum.Items)
          {
            if (a.name == textBoxNewAlbum.Text)
            {
              found = true;
            }
          }
        }

        if (found)
        {
          MessageBox.Show("Album name must be unique", "Album Name Error");
        }
        else
        {
          comboBoxAlbum.Items.Add(new Album(textBoxNewAlbum.Text, ""));
          comboBoxAlbum.SelectedIndex = comboBoxAlbum.Items.Count - 1;
        }
      }
    }

    private void buttonScan4x6_Click(object sender, EventArgs e)
    {
      Scan(new SizeInches(5.79, 3.92));
    }

    private void buttonScan3x5_Click(object sender, EventArgs e)
    {
      Scan(new SizeInches(4.92, 3.44));
    }

    private void buttonScanCustom_Click(object sender, EventArgs e)
    {
      double width = (double)numericUpDownWidth.Value;
      double height = (double)numericUpDownHeight.Value;
      myWorker.EnqueueMessage(new QueuedMessage("CUSTOM_WIDTH", width));
      myWorker.EnqueueMessage(new QueuedMessage("CUSTOM_HEIGHT", height));
      Scan(new SizeInches(width, height));
    }

    private void buttonUpload_Click(object sender, EventArgs e)
    {
      if (comboBoxAlbum.SelectedItem != null && !String.IsNullOrEmpty( ((Album)comboBoxAlbum.SelectedItem).name ) ) 
      {
        // tell the worker to upload
        myWorker.EnqueueMessage(new QueuedMessage("UPLOAD_SCANNED", comboBoxAlbum.SelectedItem));

        pictureBoxLastUploaded.Image = PictureBoxPreview.Image;
        pictureBoxLastUploaded.SizeMode = PictureBoxSizeMode.Zoom;
        PictureBoxPreview.Image = null;
      }
      else
      {
        MessageBox.Show("An album must be selected to upload into.", "Select Album First");
      }
    }
  }
}
