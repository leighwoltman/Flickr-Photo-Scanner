using FlickrNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace PDFScanningApp
{
  public class QueuedMessage
  {
    public QueuedMessage(string msg, object payload)
    {
      this.msg = msg;
      this.payload = payload;
    }
    
    public string msg;
    public object payload;
  }

  public class Album
  {
    public Album(string name, string id)
    {
      this.name = name;
      this.id = id;
    }

    public string name;
    public string id;

    public override string ToString()
    {
      return name.ToString();
    }
  }

  public class UIStatus
  {
    public UIStatus(bool authenticated, bool uploading)
    {
      this.authenticated = authenticated;
      this.uploading = uploading;
    }

    public bool authenticated;
    public bool uploading;
    public bool uploadingImage;
    public bool scannedImage;
  }

  public enum State
  {
    INIT,
    NOT_AUTHENTICATED,
    TEST_AUTHENTICATION,
    START_AUTHENTICATION,
    ATTEMPTING_NEW_ALBUM,
    IDLE
  }

  class Worker
  {
    private volatile bool keepGoing;
    private FormMain myParent;
    private Queue<QueuedMessage> queue;
    private Flickr fFlickr;
    private State state = State.INIT;
    private AppSettings fAppSettings;
    private OAuthRequestToken requestToken;
    private UIStatus uiStatus = null;

    public Worker(FormMain parent, AppSettings fAppSettings)
    {
      queue = new Queue<QueuedMessage>();
      myParent = parent;
      this.fAppSettings = fAppSettings;
      keepGoing = true;

      uiStatus = new UIStatus(false, false);

      fFlickr = FlickrManager.GetInstance(fAppSettings);
    }

    public void EnqueueMessage(QueuedMessage msgToQueue)
    {
      lock(queue)
      {
        queue.Enqueue(msgToQueue);
      }
    }

    public void Stop()
    {
      keepGoing = false;
    }

    public void Run()
    {
      myParent.EnqueueMessage(new QueuedMessage("UIStatus", uiStatus));

      while(keepGoing)
      {
        QueuedMessage toProcess = null;

        lock(queue)
        {
          if(queue.Count > 0)
          {
            toProcess = queue.Dequeue();
          }
        }

        if(toProcess != null)
        {
          switch(toProcess.msg)
          {
            case "SCANNED_IMAGE":
              {
                // save out this scanned image
                Image img = (Image)toProcess.payload;

                byte[] byteArray = Imaging.EncodeImage(img, System.Drawing.Imaging.ImageFormat.Jpeg, 90);
                string fileName = Path.Combine(Utils.TempFolder.GetPath(), "scanned.jpg");
                Imaging.SaveImageByteArrayToFile(fileName, byteArray);

                uiStatus.scannedImage = true;

                // send the image back for display
                myParent.EnqueueMessage(new QueuedMessage("SCANNED_IMAGE_RETURN", img));

                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
              }
              break;
          }
        }

        switch(state)
        {
          case State.INIT:
            {
              state = State.TEST_AUTHENTICATION;
            }
            break;

          case State.NOT_AUTHENTICATED:
            {
              // just wait until we get a message with an authentication token
              if(toProcess != null && toProcess.msg == "AUTHENTICATION_KEY")
              {
                var accessToken = fFlickr.OAuthGetAccessToken(requestToken, (string)toProcess.payload);
                // we are authenticated now, let's test it
                state = State.TEST_AUTHENTICATION;
              }
            }
            break;

          case State.TEST_AUTHENTICATION:
            {
              // try to see if this is active
              try
              {
                PhotosetCollection col = fFlickr.PhotosetsGetList();
                FlickrManager.SaveAuthentication(fFlickr);
                List<Album> list = new List<Album>();
                foreach(Photoset set in col)
                {
                  list.Add(new Album(set.Title, set.PhotosetId));
                }
                if (list.Count > 0)
                {
                  myParent.EnqueueMessage(new QueuedMessage("ALBUM_LIST", list));
                }
                uiStatus.authenticated = true;
                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
                state = State.IDLE;
              }
              catch (Exception FlickrException)
              {
                // we aren't authenticated
                state = State.START_AUTHENTICATION;
              }
            }
            break;

          case State.START_AUTHENTICATION:
            {
              fFlickr.OAuthAccessToken = "";
              fFlickr.OAuthAccessTokenSecret = "";
              requestToken = fFlickr.OAuthGetRequestToken("oob");

              string url = fFlickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

              state = State.NOT_AUTHENTICATED;
              myParent.EnqueueMessage(new QueuedMessage("AUTHENTICATE", url));
            }
            break;

          case State.IDLE:
            {
              if (toProcess != null && toProcess.msg == "UPLOAD_SCANNED")
              {
                uiStatus.uploading = true;
                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));

                string fileName = Path.Combine(Utils.TempFolder.GetPath(), "scanned.jpg");
                string photoId = fFlickr.UploadPicture(fileName, "", "", null, false, false, false);
                uiStatus.uploading = false;
                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
              }
            }
            break;
        }

        Thread.Sleep(10);
      }
    }
  }
}
