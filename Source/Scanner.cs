﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TwainInterface;
using WiaInterface;
using Scanning;
using Utils;


namespace PDFScanningApp
{
  public delegate void NewPageEventHandler(object sender, NewPageEventArgs e);


  public class NewPageEventArgs : EventArgs
  {
    public Page ThePage;
  }

  
  class Scanner
  {
    private InterfaceDataSourceManager fTwain;
    private InterfaceDataSourceManager fWia;
    private List<InterfaceDataSource> fDataSources;
    private InterfaceDataSource fActiveDataSource;
    private List<ColorModeEnum> fAvailableValuesForColorMode;
    private List<PageTypeEnum> fAvailableValuesForPageType;
    private List<int> fAvailableValuesForResolution;


    public Scanner()
    {
      fTwain = new TwainDataSourceManager(UtilDialogs.MainWindow.Handle);
      fWia = new WiaDataSourceManager();
      fDataSources = null;
      fActiveDataSource = null;
      fAvailableValuesForColorMode = null;
      fAvailableValuesForPageType = null;
      fAvailableValuesForResolution = null;
    }


    public bool IsOpen
    {
      get { return (bool)(fDataSources != null); }
    }


    public bool Open()
    {
      if(IsOpen == false)
      {
        fDataSources = new List<InterfaceDataSource>();

        if(fTwain.Open())
        {
          fDataSources.AddRange(fTwain.GetDataSources());
        }
        if(fWia.Open())
        {
          fDataSources.AddRange(fWia.GetDataSources());
        }
      }
      return IsOpen;
    }


    public void Close()
    {
      if(IsOpen)
      {
        fTwain.Close();
        fWia.Close();
        fDataSources = null;
      }
    }


    public List<string> GetDataSourceNames()
    {
      List<string> result = new List<string>();

      if(IsOpen)
      {
        foreach(InterfaceDataSource ds in fDataSources)
        {
          result.Add(ds.Name);
        }
      }

      return result;
    }

    
    public string GetActiveDataSourceName()
    {
      string result = null;

      if(fActiveDataSource != null)
      {
        result = fActiveDataSource.Name;
      }

      return result;
    }


    public bool SelectActiveDataSource(string name)
    {
      bool success = false;

      InterfaceDataSource ds = GetDataSourceByName(name);

      if(ds != null)
      {
        if(ds.Open())
        {
          fAvailableValuesForColorMode = ds.GetAvailableValuesForColorMode();
          fAvailableValuesForPageType = ds.GetAvailableValuesForPageType();
          fAvailableValuesForResolution = ds.GetAvailableValuesForResolution();
          ds.Close();

          fActiveDataSource = ds;
          success = true;
        }
      }

      return success;
    }


    private InterfaceDataSource GetDataSourceByName(string name)
    {
      InterfaceDataSource result = null;

      if(IsOpen)
      {
        foreach(InterfaceDataSource ds in fDataSources)
        {
          if(ds.Name == name)
          {
            result = ds;
          }
        }
      }

      return result;
    }


    public List<ColorModeEnum> GetAvailableValuesForColorMode()
    {
      return fAvailableValuesForColorMode;
    }


    public List<PageTypeEnum> GetAvailableValuesForPageType()
    {
      return fAvailableValuesForPageType;
    }


    public List<int> GetAvailableValuesForResolution()
    {
      return fAvailableValuesForResolution;
    }


    public bool Acquire(ScanSettings settings, bool showSettingsUI, bool showTransferUI)
    {
      bool result = false;

      if(fActiveDataSource != null)
      {
        if(fActiveDataSource.Open())
        {
          fActiveDataSource.OnNewPictureData += fActiveDataSource_OnNewPictureData;
          fActiveDataSource.OnScanningComplete += fActiveDataSource_OnScanningComplete;

          result = fActiveDataSource.Acquire(settings, showSettingsUI, showTransferUI);

          if(result == false)
          {
            fActiveDataSource.Close();
          }
        }
      }

      return result;
    }


    private void fActiveDataSource_OnNewPictureData(object sender, NewPictureEventArgs args)
    {
      // get a temporary path
      string fileName = Path.GetTempFileName();

      UtilImaging.SaveImageAsJpeg(args.TheImage, fileName, 75L);

      Page myPage = new Page(fileName, true, args.TheSettings.PageType);

      Raise_OnNewPage(myPage);
    }


    private void fActiveDataSource_OnScanningComplete(object sender, EventArgs e)
    {
      fActiveDataSource.Close();
      Raise_OnScanningComplete();
    }


    public event NewPageEventHandler OnNewPage;

    private void Raise_OnNewPage(Page page)
    {
      if(OnNewPage != null)
      {
        NewPageEventArgs args = new NewPageEventArgs();
        args.ThePage = page;
        OnNewPage(this, args);
      }
    }


    public event EventHandler OnScanningComplete;

    private void Raise_OnScanningComplete()
    {
      if(OnScanningComplete != null)
      {
        OnScanningComplete(this, EventArgs.Empty);
      }
    }
  }
}