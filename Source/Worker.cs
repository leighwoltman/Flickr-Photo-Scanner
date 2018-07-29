using FlickrNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
    public string downloadStatus = "";
  }

  public enum State
  {
    PRE_INIT,
    INIT,
    NOT_AUTHENTICATED,
    TEST_AUTHENTICATION,
    START_AUTHENTICATION,
    UPLOADING_PICTURE,
    ATTEMPTING_NEW_ALBUM,
    SETTING_PICTURE_ALBUM,
    IDLE,
    DOWNLOADING,
    DOWNLOADNEXT,
  }

  class Worker
  {
    private volatile bool keepGoing;
    private FormMain myParent;
    private Queue<QueuedMessage> queue;
    private Flickr fFlickr;
    private State state = State.PRE_INIT;
    private Album currentImageAlbum = null;
    private AppSettings fAppSettings;
    private OAuthRequestToken requestToken;
    private UIStatus uiStatus = null;
    private List<Album> myAlbums;
    private string downloadFolder;
    private Queue<Album> photoIDsToDownload;
    private double totalPhotosToDownload;

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

                byte[] byteArray = Imaging.EncodeImage(img, System.Drawing.Imaging.ImageFormat.Jpeg, 100);
                string fileName = Path.Combine(Utils.TempFolder.GetPath(), "scanned.jpg");
                Imaging.SaveImageByteArrayToFile(fileName, byteArray);

                uiStatus.scannedImage = true;

                // send the image back for display
                myParent.EnqueueMessage(new QueuedMessage("SCANNED_IMAGE_RETURN", img));

                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
              }
              break;

            case "CUSTOM_WIDTH":
              {
                double width = (double)toProcess.payload;
                fAppSettings.CustomWidth = width;
              }
              break;

            case "CUSTOM_HEIGHT":
              {
                double height = (double)toProcess.payload;
                fAppSettings.CustomHeight = height;
              }
              break;
          }
        }

        switch(state)
        {
          case State.PRE_INIT:
            {
              if(toProcess != null)
              {
                if(toProcess.msg == "START")
                {
                  state = State.INIT;
                }
              }
            }
            break;

          case State.INIT:
            {
              // if there was a photo uploading, we want to remove any scanned image
              string starting_fileName = Path.Combine(Utils.TempFolder.GetPath(), "scanned.jpg");
              string uploading_fileName = Path.Combine(Utils.TempFolder.GetPath(), "uploading.jpg");
              string lastuploaded_fileName = Path.Combine(Utils.TempFolder.GetPath(), "lastuploaded.jpg");

              if (File.Exists(uploading_fileName))
              {
                if(File.Exists(starting_fileName))
                {
                  File.Delete(starting_fileName);
                  int count = 0;

                  while (File.Exists(starting_fileName) && count < 100000000)
                  {
                    count++;
                  }
                }

                File.Move(uploading_fileName, starting_fileName);
              }

              if (File.Exists(lastuploaded_fileName))
              {
                Image lastUploaded = Utils.Imaging.LoadImageFromFile(lastuploaded_fileName);
                myParent.EnqueueMessage(new QueuedMessage("LAST_UPLOADED", lastUploaded));
              }

              if (File.Exists(starting_fileName))
              {
                Image scanned = Utils.Imaging.LoadImageFromFile(starting_fileName);
                myParent.EnqueueMessage(new QueuedMessage("SCANNED_IMAGE_RETURN", scanned));
              }

              state = State.TEST_AUTHENTICATION;
            }
            break;

          case State.NOT_AUTHENTICATED:
            {
              // just wait until we get a message with an authentication token
              if(toProcess != null && toProcess.msg == "AUTHENTICATION_KEY")
              {
                if (toProcess.payload == null)
                {
                  state = State.START_AUTHENTICATION;
                }
                else
                {
                  var accessToken = fFlickr.OAuthGetAccessToken(requestToken, (string)toProcess.payload);
                  // we are authenticated now, let's test it
                  state = State.TEST_AUTHENTICATION;
                }
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
                myAlbums = list;
                if (list.Count > 0)
                {
                  myParent.EnqueueMessage(new QueuedMessage("ALBUM_LIST", list));
                }
                uiStatus.authenticated = true;
                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
                state = State.IDLE;
              }
              catch (Exception)
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
                currentImageAlbum = (Album)toProcess.payload;
                fAppSettings.SelectedAlbumName = currentImageAlbum.name;
                state = State.UPLOADING_PICTURE;
              }
              else if (toProcess != null && toProcess.msg == "DOWNLOAD_ALL")
              {
                uiStatus.uploading = true;
                uiStatus.downloadStatus = "Starting to retrieve list of photos";
                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
                downloadFolder = (string)toProcess.payload;
                state = State.DOWNLOADING;
              }
            }
            break;

          case State.UPLOADING_PICTURE:
            {
              try
              {
                string starting_fileName = Path.Combine(Utils.TempFolder.GetPath(), "scanned.jpg");
                string uploading_fileName = Path.Combine(Utils.TempFolder.GetPath(), "uploading.jpg");

                // need to try and delete any uploading file
                File.Delete(uploading_fileName);
                int count = 0;

                while (File.Exists(uploading_fileName) && count < 100000000)
                {
                  count++;
                }

                if (count >= 100000000)
                {

                }

                File.Move(starting_fileName, uploading_fileName);

                string photoId = fFlickr.UploadPicture(uploading_fileName, "", "", null, false, false, false);

                // now that the photo has been uploaded, we need to decide whether we should assign it to an album (photoset)
                // or create a new album
                bool albumAdd = false;

                if (String.IsNullOrEmpty(currentImageAlbum.id))
                {
                  albumAdd = true;
                  fFlickr.PhotosetsCreate(currentImageAlbum.name, photoId);
                }
                else
                {
                  fFlickr.PhotosetsAddPhoto(currentImageAlbum.id, photoId);
                }

                // if this was a success, then we rename the photo
                string lastuploaded_fileName = Path.Combine(Utils.TempFolder.GetPath(), "lastuploaded.jpg");

                // need to try and delete any uploading file
                File.Delete(lastuploaded_fileName);
                count = 0;

                while (File.Exists(lastuploaded_fileName))
                {
                  count++;
                }

                File.Move(uploading_fileName, lastuploaded_fileName);

                uiStatus.uploading = false;
                // set the authentication to false so we will upload the new photo

                if (albumAdd)
                {
                  uiStatus.authenticated = false;
                  state = State.TEST_AUTHENTICATION;
                }
                else
                {
                  state = State.IDLE;
                }

                myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
              }
              catch(Exception e)
              {
                // a generic error occured 
                myParent.EnqueueMessage(new QueuedMessage("ERROR", e.Message));
                state = State.INIT;
                uiStatus.authenticated = false;
                uiStatus.uploading = false;
              }
            }
            break;
          case State.DOWNLOADING:
            {
              photoIDsToDownload = new Queue<Album>();

              // let's work through all the pictures
              foreach(Album album in this.myAlbums)
              {
                PhotosetPhotoCollection photos = fFlickr.PhotosetsGetPhotos(album.id);
                for (int i = 0; i < photos.Count; i++ )
                {
                  photoIDsToDownload.Enqueue(new Album(album.name, photos[i].PhotoId));
                }
              }

              totalPhotosToDownload = photoIDsToDownload.Count;
              this.state = State.DOWNLOADNEXT;
            }
            break;

          case State.DOWNLOADNEXT:
            {
              if(photoIDsToDownload.Count > 0)
              {
                Album photoToDownload = photoIDsToDownload.Dequeue();

                string directory = Path.Combine(downloadFolder, photoToDownload.name);

                if(!Directory.Exists(directory))
                {
                  Directory.CreateDirectory(directory);
                }

                // now create the filename
                string fileName = Path.Combine(directory, photoToDownload.id + ".jpg");

                double temp = photoIDsToDownload.Count;
                temp = (totalPhotosToDownload - temp) / totalPhotosToDownload * 100;

                if ( File.Exists(fileName) && (new FileInfo(fileName).Length) > 2000 )
                {
                  uiStatus.downloadStatus = "File exists " + photoToDownload.id + " (" + temp.ToString("0.00") + "%)";
                }
                else
                {
                  uiStatus.downloadStatus = "Downloading photo " + photoToDownload.id + " (" + temp.ToString("0.00") + "%)";

                  try
                  {
                    // need to get the URL
                    SizeCollection size = fFlickr.PhotosGetSizes(photoToDownload.id);

                    string url = "";

                    foreach (FlickrNet.Size siz in size)
                    {
                      if (siz.Label == "Original")
                      {
                        url = siz.Source;
                        break;
                      }
                    }

                    using (var client = new WebClient())
                    {
                      client.DownloadFile(url, fileName);
                    }
                  }
                  catch(Exception)
                  {
                    // do nothing now, future maybe keep a counter on this file and fail it after a while
                  }
                }
              }
              else
              {
                uiStatus.uploading = false;
                uiStatus.downloadStatus = "Done";
                state = State.IDLE;
              }
              myParent.EnqueueMessage(new QueuedMessage("STATUS", uiStatus));
            }
            break;
        }

        Thread.Sleep(10);
      }
    }
  }
}
