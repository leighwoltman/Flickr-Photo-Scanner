﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Utils;


namespace Model
{
  public class PageFromImage : Page
  {
    private Image myImageFile;

    public PageFromImage(Image image)
    {
      this.myImageFile = image;
      this.fSize = PageSize.Letter;

      InitializeImage();
    }


    protected override Image CreateImage()
    {
      return myImageFile;
    }


    public override void CleanUp()
    {
      this.myImageFile.Dispose();
      this.myImageFile = null;

      base.CleanUp();
    }
  }
}