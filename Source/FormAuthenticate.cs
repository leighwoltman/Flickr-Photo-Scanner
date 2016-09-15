using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;
using FlickrNet;

namespace PDFScanningApp
{
  public partial class FormAuthenticate : Form
  {
    private Flickr fFlickr;
    private OAuthRequestToken requestToken;
    private Timer timer;
    private ChromiumWebBrowser browser;

    public string responseCode;

    public FormAuthenticate(Flickr fFlickr)
    {
      InitializeComponent();

      this.fFlickr = fFlickr;
      // clear any old token data
      fFlickr.OAuthAccessToken = "";
      fFlickr.OAuthAccessTokenSecret = "";
      requestToken = fFlickr.OAuthGetRequestToken("oob");

      string url = fFlickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

      browser = new ChromiumWebBrowser(url);

      browser.Dock = DockStyle.Fill;
      this.Controls.Add(browser);

      timer = new Timer();
      timer.Interval = 500;
      timer.Tick += timer_Tick;
      timer.Start();
    }

    void timer_Tick(object sender, EventArgs e)
    {
      timer.Stop();
      try
      {
        var task = browser.GetBrowser().FocusedFrame.EvaluateScriptAsync("(function() { return document.querySelectorAll('#Main span')[0].innerHTML; })();", null);

        task.ContinueWith(t =>
        {
          if (!t.IsFaulted)
          {
            var response = t.Result;
            string msg = (string)response.Result;
            if (!String.IsNullOrEmpty(msg))
            {
              try
              {
                var accessToken = fFlickr.OAuthGetAccessToken(requestToken, msg);
                this.DialogResult = DialogResult.OK;
              }
              catch(Exception ex)
              {
                MessageBox.Show(ex.Message);
              }
            }
            else
            {
              timer.Start();
            }
          }
        }, TaskScheduler.FromCurrentSynchronizationContext());
      }
      catch(Exception)
      {
        // do nothing
      }

    }
  }
}
