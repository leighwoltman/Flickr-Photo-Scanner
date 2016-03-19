﻿using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Utils;


namespace Model
{
  abstract public class Page
  {
    private Image fSourceThumbnail;
    private Image fThumbnail;
    private int fImageHeightPixels;
    private int fImageWidthPixels;
    private int fImageVerticalResolutionDpi;
    private int fImageHorizontalResolutionDpi;
    private int fOrientation;
    private int fMirror;
    private PageSize fSize;


    public Page()
    {
      fSourceThumbnail = null;
      fThumbnail = null;
      fImageHeightPixels = 0;
      fImageWidthPixels = 0;
      fImageVerticalResolutionDpi = 0;
      fImageHorizontalResolutionDpi = 0;
      fOrientation = 0;
      fMirror = 0;
      fSize = null;
    }


    protected void InitializeImage(int verticalDpi, int horizontalDpi)
    {
      using(Image myImage = CreateImage())
      {
        fImageHeightPixels = myImage.Size.Height;
        fImageWidthPixels = myImage.Size.Width;
        fImageVerticalResolutionDpi = verticalDpi;
        fImageHorizontalResolutionDpi = horizontalDpi;
        fSourceThumbnail = UtilImaging.CreateThumbnail(myImage, 200);
      }
      RefreshThumbnail();
    }


    abstract protected Image CreateImage();


    public Image GetImage()
    {
      Image result = CreateImage();
      TransformImage(result);
      return result;
    }


    public Image Thumbnail
    {
      get { return fThumbnail; }
    }


    private void RefreshThumbnail()
    {
      fThumbnail = (Image)fSourceThumbnail.Clone();
      TransformImage(fThumbnail);
    }


    virtual public void CleanUp()
    {
      fThumbnail.Dispose();
    }


    readonly RotateFlipType[] rf_table =
    { 
      RotateFlipType.RotateNoneFlipNone,
      RotateFlipType.Rotate90FlipNone,
      RotateFlipType.Rotate180FlipNone,
      RotateFlipType.Rotate270FlipNone,
      RotateFlipType.RotateNoneFlipX,
      RotateFlipType.Rotate90FlipY,
      RotateFlipType.Rotate180FlipX,
      RotateFlipType.Rotate270FlipY,
    };


    protected void TransformImage(Image image)
    {
      int index;

      switch(fMirror)
      {
        case 0: index = fOrientation; break;
        case 1: index = fOrientation + 4; break;
        case 2: index = (fOrientation + 2) % 4 + 4; break;
        case 3: index = (fOrientation + 2) % 4; break;
        default: index = 0; break;
      }
      
      image.RotateFlip(rf_table[index]);
    }


    private bool IsFlipped
    {
      get { return (bool)((fOrientation & 1) == 0); }
    }


    public int ImageHeightPixels
    {
      get { return IsFlipped ? fImageHeightPixels : fImageWidthPixels; }
    }


    public int ImageWidthPixels
    {
      get { return IsFlipped ? fImageWidthPixels : fImageHeightPixels; }
    }


    public int ImageVerticalResolutionDpi
    {
      get { return IsFlipped ? fImageVerticalResolutionDpi : fImageHorizontalResolutionDpi; }
    }


    public int ImageHorizontalResolutionDpi
    {
      get { return IsFlipped ? fImageHorizontalResolutionDpi : fImageVerticalResolutionDpi; }
    }


    public bool ImageResolutionIsDefined
    {
      get { return (bool)((fImageVerticalResolutionDpi != 0) && (fImageHorizontalResolutionDpi != 0)); }
    }


    public void RotateClockwise()
    {
      fOrientation = (fOrientation + 1) % 4;
      RefreshThumbnail();
    }


    public void RotateCounterClockwise()
    {
      fOrientation = (fOrientation + 3) % 4;
      RefreshThumbnail();
    }


    public void MirrorHorizontally()
    {
      if(IsFlipped)
      {
        fMirror ^= 1;
      }
      else
      {
        fMirror ^= 2;
      }
      RefreshThumbnail();
    }


    public void MirrorVertically()
    {
      if(IsFlipped)
      {
        fMirror ^= 2;
      }
      else
      {
        fMirror ^= 1;
      }
      RefreshThumbnail();
    }


    public int Orientation
    {
      get { return fOrientation * 90; }
    }


    public bool IsMirrored
    {
      get { return (bool)(fMirror != 0); }
    }


    public PageSize Size
    {
      get { return fSize; }
      protected set { fSize = value; }
    }


    public void Landscape()
    {
      if(fSize != null)
      {
        double temp = fSize.Width;
        fSize.Width = fSize.Height;
        fSize.Height = temp;
      }
    }


    public bool IsLandscape
    {
      get 
      { 
        return (fSize != null) && (fSize.Width > fSize.Height); 
      }
    }


    public Rectangle GetPageRectangle()
    {
      double hRes;
      double vRes;

      if(ImageResolutionIsDefined)
      {
        hRes = ImageHorizontalResolutionDpi;
        vRes = ImageVerticalResolutionDpi;
      }
      else
      {
        double image_aspect_ratio = ImageWidthPixels / (double)ImageHeightPixels;
        double page_aspect_ratio = fSize.Width / fSize.Height;
        double imageWidth;
        double imageHeight;

        if(image_aspect_ratio > page_aspect_ratio)
        {
          // means our image has the width as the maximum dimension
          imageWidth = fSize.Width;
          imageHeight = fSize.Width / image_aspect_ratio;
        }
        else
        {
          // means our image has the height as the maximum dimension
          imageWidth = fSize.Height * image_aspect_ratio;
          imageHeight = fSize.Height;
        }

        hRes = ImageWidthPixels / imageWidth;
        vRes = ImageHeightPixels / imageHeight;
      }


      int pagePixelWidth = (int)(fSize.Width * hRes);
      int pagePixelHeight = (int)(fSize.Height * vRes);

      return new Rectangle(0, 0, pagePixelWidth, pagePixelHeight);
    }


    public Rectangle GetImageRectangle()
    {
      Rectangle pageRect = GetPageRectangle();

      Rectangle imageArea = new Rectangle();

      imageArea.Width = ImageWidthPixels;
      imageArea.Height = ImageHeightPixels;
      imageArea.X = (pageRect.Width - imageArea.Width) / 2;
      imageArea.Y = (pageRect.Height - imageArea.Height) / 2;

      return imageArea;
    }


    virtual public void AddPdfPage(PdfDocument pdfDocument)
    {
      // Create an empty page
      PdfPage pdfPage = pdfDocument.AddPage();

      if (this.Size == PageSize.Legal)
      {
        pdfPage.Size = PdfSharp.PageSize.Legal;
      }
      else
      {
        pdfPage.Size = PdfSharp.PageSize.Letter;
      }

      // we need to swap the height and the width if page is a landscape image
      double aspect_ratio = ((double)this.ImageWidthPixels) / ((double)this.ImageHeightPixels);

      if (this.IsLandscape)
      {
        pdfPage.Orientation = PageOrientation.Landscape;
      }
      else
      {
        pdfPage.Orientation = PageOrientation.Portrait;
      }

      // Get an XGraphics object for drawing
      XGraphics gfx = XGraphics.FromPdfPage(pdfPage);
      // Create a font
      XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

      int draw_point_x = 0;
      int draw_point_y = 0;
      int draw_point_width = 0;
      int draw_point_height = 0;

      if (this.IsLandscape)
      {
        // these are swapped
        draw_point_width = (int)pdfPage.Height;
        draw_point_height = (int)pdfPage.Width;

        if (aspect_ratio > ((double)draw_point_width / (double)draw_point_height))
        {
          // means our image has the width as the maximum dimension
          draw_point_height = (int)((double)draw_point_width / aspect_ratio);
          draw_point_y = ((int)pdfPage.Height - draw_point_height) / 2;
          draw_point_x = ((int)pdfPage.Width - draw_point_width) / 2;
        }
        else
        {
          // means our image has the height as the maximum dimension
          draw_point_width = (int)(aspect_ratio * (double)draw_point_height);
          draw_point_x = ((int)pdfPage.Width - draw_point_width) / 2;
          draw_point_y = ((int)pdfPage.Height - draw_point_height) / 2;
        }
      }
      else
      {
        draw_point_width = (int)pdfPage.Width;
        draw_point_height = (int)pdfPage.Height;

        if (aspect_ratio > ((double)draw_point_width / (double)draw_point_height))
        {
          // means our image has the width as the maximum dimension
          draw_point_height = (int)((double)draw_point_width / aspect_ratio);
          draw_point_y = ((int)pdfPage.Height - draw_point_height) / 2;
        }
        else
        {
          // means our image has the height as the maximum dimension
          draw_point_width = (int)(aspect_ratio * (double)draw_point_height);
          draw_point_x = ((int)pdfPage.Width - draw_point_width) / 2;
        }
      }


      XImage image = XImage.FromGdiPlusImage(this.GetImage());

      //if(page.IsRotated())
      //{
      //  // rotate around the center of the pdfPage
      //  gfx.RotateAtTransform(-180, new XPoint(pdfPage.Width / 2, pdfPage.Height / 2));
      //}

      //if(page.IsLandscape())
      //{
      //  // rotate around the center of the pdfPage
      //  gfx.RotateAtTransform(90, new XPoint(pdfPage.Width / 2, pdfPage.Height / 2));
      //}

      gfx.DrawImage(image, draw_point_x, draw_point_y, draw_point_width, draw_point_height);
      image.Dispose();
    }
  }
}
