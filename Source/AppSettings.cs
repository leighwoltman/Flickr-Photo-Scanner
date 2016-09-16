using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Defines;
using Utils;


namespace PDFScanningApp
{
  public class AppSettings
  {
    static private SettingsTable fTable = null;


    static public void Initialize(string filename)
    {
      fTable = new SettingsTable(filename);
    }


    public string CurrentScanner
    {
      get { return fTable.Get("CurrentScanner", ""); }
      set { fTable.Set("CurrentScanner", value); }
    }

    public string OAuthAccessToken
    {
      get { return fTable.Get("OAuthAccessToken", ""); }
      set { fTable.Set("OAuthAccessToken", value); }
    }

    public string OAuthAccessTokenSecret
    {
      get { return fTable.Get("OAuthAccessTokenSecret", ""); }
      set { fTable.Set("OAuthAccessTokenSecret", value); }
    }

    public string SelectedAlbumName
    {
      get { return fTable.Get("SelectedAlbumName", ""); }
      set { fTable.Set("SelectedAlbumName", value); }
    }

    public double CustomWidth
    {
      get { return fTable.GetDouble("CustomWidth", 1.0); }
      set { fTable.SetDouble("CustomWidth", value); }
    }

    public double CustomHeight
    {
      get { return fTable.GetDouble("CustomHeight", 1.0); }
      set { fTable.SetDouble("CustomHeight", value); }
    }
  }
}
