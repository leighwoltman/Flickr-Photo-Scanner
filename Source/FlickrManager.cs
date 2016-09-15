using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlickrNet;


namespace PDFScanningApp
{
  public class FlickrManager
  {
    public const string ApiKey = "dfdf3da7c488a7670be352ddf8ac90e3";
    public const string SharedSecret = "723df33d320cc06e";
    static private AppSettings fAppSettings;

    public static Flickr GetInstance(AppSettings appSettings)
    {
      fAppSettings = appSettings;

      var f = new Flickr(ApiKey, SharedSecret);
      f.OAuthAccessToken = fAppSettings.OAuthAccessToken;
      f.OAuthAccessTokenSecret = fAppSettings.OAuthAccessTokenSecret;
      return f;
    }

    public static void SaveAuthentication(Flickr flickr)
    {
      fAppSettings.OAuthAccessToken = flickr.OAuthAccessToken;
      fAppSettings.OAuthAccessTokenSecret = flickr.OAuthAccessTokenSecret;
    }
  }
}
