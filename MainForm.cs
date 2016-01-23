﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Threading;


namespace WIATest
{
    public partial class MainForm : Form
    {
      private Settings settings;

      const int none_selected = 5000;
      int selected_index = none_selected;

        public MainForm(Settings settings)
        {
          this.settings = settings;
          InitializeComponent();

          moveLeftButton.Enabled = false;
          moveRightButton.Enabled = false;
          deleteButton.Enabled = false;
          rotateButton.Enabled = false;
          buttonLandscape.Enabled = false;
          saveButton.Enabled = false;
          checkSided2ButtonStatus();

          comboBoxResolution.Items.Add("200 dpi");
          comboBoxResolution.Items.Add("300 dpi");
          comboBoxResolution.SelectedIndex = 1;
        }

        int getScanResolution()
        {
          if(comboBoxResolution.SelectedIndex == 1)
          {
            return 300;
          }
          else
          {
            return 200;
          }
        }

        void picBox_Click(object sender, EventArgs e)
        {
          PictureBox pBox = (PictureBox)(sender);

          if (selected_index != imagePanel.Controls.IndexOf(pBox))
          {
            // remove the BorderStyle of the existing index
            if(selected_index != none_selected)
            {
              ((PictureBox)(imagePanel.Controls[selected_index])).BorderStyle = BorderStyle.None;
              ((PictureBox)(imagePanel.Controls[selected_index])).BackColor = imagePanel.BackColor;
            }

            // select this one
            selected_index = imagePanel.Controls.IndexOf(pBox);
            pBox.BorderStyle = BorderStyle.FixedSingle;
            pBox.BackColor = Color.DarkGray;

            if(selected_index != 0)
            {
              moveLeftButton.Enabled = true;
            }

            if(selected_index != (imagePanel.Controls.Count-1))
            {
              moveRightButton.Enabled = true;
            }

            deleteButton.Enabled = true;
            rotateButton.Enabled = true;
            buttonLandscape.Enabled = true;
            checkSided2ButtonStatus();
          }
          else
          {
            // remove the index
            pBox.BorderStyle = BorderStyle.None;
            pBox.BackColor = imagePanel.BackColor;
            selected_index = none_selected;

            moveLeftButton.Enabled = false;
            moveRightButton.Enabled = false;
            deleteButton.Enabled = false;
            rotateButton.Enabled = false;
            buttonLandscape.Enabled = false;
            checkSided2ButtonStatus();
          }
        }

        private void Scan(double width, double height, double resolution)
        {
          List<Image> images = WIAScanner.Scan(settings.CurrentScanner, width, height, resolution);

          if (images.Count != 0)
          {
            for(int i = 0; i < images.Count; i++)
            {
              // get a temporary path
              string fileName = Path.GetTempFileName();

              ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

              // Create an Encoder object based on the GUID 
              // for the Quality parameter category.
              System.Drawing.Imaging.Encoder myEncoder =
                  System.Drawing.Imaging.Encoder.Quality;

              // Create an EncoderParameters object. 
              // An EncoderParameters object has an array of EncoderParameter 
              // objects. In this case, there is only one 
              // EncoderParameter object in the array.
              EncoderParameters myEncoderParameters = new EncoderParameters(1);

              EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 95L);
              myEncoderParameters.Param[0] = myEncoderParameter;

              // use max as the filename
              images[i].Save(fileName, jgpEncoder, myEncoderParameters);

              ImageContainer myImage = new ImageContainer(fileName, true, height == 14 ? ScanPageSize.Legal : ScanPageSize.Letter);
              Control picBox = myImage.getPictureBox();
              picBox.Click += picBox_Click;
              imagePanel.Controls.Add(picBox);

              saveButton.Enabled = true;
              checkSided2ButtonStatus();
            }

            // close all these images
            while(images.Count > 0)
            {
              Image img = images[0];
              images.RemoveAt(0);
              img.Dispose();
            }
          }
          else
          {
            // an error occured in scanning the file
            MessageBox.Show("Last image now scanned correctly, rescan it. If this error keeps occuring close the program and restart it.");
          }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

          ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

          foreach (ImageCodecInfo codec in codecs)
          {
            if (codec.FormatID == format.Guid)
            {
              return codec;
            }
          }
          return null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
          Scan(8.48, 11, getScanResolution());

          button3.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          Scan(8.48, 14, getScanResolution());

          button1.Focus();
        }
      
        private void checkSided2ButtonStatus()
        {
          // we have to have at least 3 pages to make this worth while
          // only allow this button when no page is selected
          if (imagePanel.Controls.Count > 2 && selected_index == none_selected)
          {
            sided2Button.Enabled = true;
          }
          else
          {
            sided2Button.Enabled = false;
          }
        }

        private void moveLeftButton_Click(object sender, EventArgs e)
        {
          imagePanel.Controls.SetChildIndex(imagePanel.Controls[selected_index], selected_index-1);
          selected_index--;

          if(selected_index == 0)
          {
            moveLeftButton.Enabled = false;
          }
          // after moving left we can always move right
          moveRightButton.Enabled = true;
        }

        private void moveRightButton_Click(object sender, EventArgs e)
        {
          imagePanel.Controls.SetChildIndex(imagePanel.Controls[selected_index], selected_index + 1);
          selected_index++;

          // check if we are at the end
          if (selected_index == (imagePanel.Controls.Count - 1))
          {
            moveRightButton.Enabled = false;
          }
          // after moving right we can always move left
          moveLeftButton.Enabled = true;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
          if(DialogResult.OK == MessageBox.Show("Delete selected image?", "Delete?", MessageBoxButtons.OKCancel))
          {
            ImageContainer imgContainer = (ImageContainer)imagePanel.Controls[selected_index].Tag;
            
            imagePanel.Controls.RemoveAt(selected_index);
            selected_index = none_selected;

            moveLeftButton.Enabled = false;
            moveRightButton.Enabled = false;
            deleteButton.Enabled = false;
            rotateButton.Enabled = false;
            buttonLandscape.Enabled = false;
            checkSided2ButtonStatus();

            imgContainer.cleanUp();

            if(imagePanel.Controls.Count == 0)
            {
              saveButton.Enabled = false;
            }
          }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
          SaveFileDialog saveFileDialog1 = new SaveFileDialog();

          if (!Directory.Exists(settings.LastDirectory))
          {
            settings.LastDirectory = "M:\\";
          }

          saveFileDialog1.InitialDirectory = settings.LastDirectory;
          saveFileDialog1.Filter = "PDF files (*.pdf)|*.pdf";
          saveFileDialog1.FilterIndex = 1;

          if (saveFileDialog1.ShowDialog() == DialogResult.OK)
          {
            string fileName = saveFileDialog1.FileName;

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";

            for (int i = 0; i < imagePanel.Controls.Count; i++)
            {
              ImageContainer imgContainer = (ImageContainer)imagePanel.Controls[i].Tag;

              // Create an empty page
              PdfPage page = document.AddPage();
              if(imgContainer.getScanPageSize() == ScanPageSize.Legal)
              {
                page.Size = PageSize.Legal;
              }
              else
              {
                page.Size = PageSize.Letter;
              }

              // we need to swap the height and the width if this is a landscape image
              double aspect_ratio = ((double)imgContainer.getWidth()) / ((double)imgContainer.getHeight());

              if(imgContainer.isLandscape())
              {
                page.Orientation = PageOrientation.Landscape;
              }
              else
              {
                page.Orientation = PageOrientation.Portrait;
              }

              // Get an XGraphics object for drawing
              XGraphics gfx = XGraphics.FromPdfPage(page);
              // Create a font
              XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

              int draw_point_x = 0;
              int draw_point_y = 0;
              int draw_point_width = 0;
              int draw_point_height = 0;

              if(imgContainer.isLandscape())
              {
                // these are swapped
                draw_point_width = (int)page.Height;
                draw_point_height = (int)page.Width;

                if (aspect_ratio > ((double)draw_point_width / (double)draw_point_height))
                {
                  // means our image has the width as the maximum dimension
                  draw_point_height = (int)((double)draw_point_width / aspect_ratio);
                  draw_point_y = ((int)page.Height - draw_point_height) / 2;
                  draw_point_x = ((int)page.Width - draw_point_width) / 2;
                }
                else
                {
                  // means our image has the height as the maximum dimension
                  draw_point_width = (int)(aspect_ratio * (double)draw_point_height);
                  draw_point_x = ((int)page.Width - draw_point_width) / 2;
                  draw_point_y = ((int)page.Height - draw_point_height) / 2;
                }
              }
              else
              {
                draw_point_width = (int)page.Width;
                draw_point_height = (int)page.Height;

                if (aspect_ratio > ((double)draw_point_width / (double)draw_point_height))
                {
                  // means our image has the width as the maximum dimension
                  draw_point_height = (int)((double)draw_point_width / aspect_ratio);
                  draw_point_y = ((int)page.Height - draw_point_height) / 2;
                }
                else
                {
                  // means our image has the height as the maximum dimension
                  draw_point_width = (int)(aspect_ratio * (double)draw_point_height);
                  draw_point_x = ((int)page.Width - draw_point_width) / 2;
                }
              }
              

              XImage image = XImage.FromFile(imgContainer.getFileName());

              if (imgContainer.isRotated())
              {
                // rotate around the center of the page
                gfx.RotateAtTransform(-180, new XPoint(page.Width / 2, page.Height / 2));
              }

              if( imgContainer.isLandscape() )
              {
                // rotate around the center of the page
                gfx.RotateAtTransform(90, new XPoint(page.Width / 2, page.Height / 2));
              }              

              gfx.DrawImage(image, draw_point_x, draw_point_y, draw_point_width, draw_point_height);
              image.Dispose();
            }

            // Save the document...
            document.Save(fileName);

            // first copy the controls to another croup;
            int count = imagePanel.Controls.Count;

            Control[] ctrls = new Control[count];
            imagePanel.Controls.CopyTo(ctrls, 0);

            for (int i = 0; i < count; i++)
            {
              imagePanel.Controls.RemoveAt(0);
            }

            Thread.Sleep(5000);

            // remove all the files
            for (int i = 0; i < ctrls.Length; i++)
            {
              ImageContainer imgContainer = (ImageContainer)ctrls[i].Tag;
              imgContainer.cleanUp();
            }

            settings.LastDirectory = Path.GetDirectoryName(fileName);
            settings.SaveSettings();

            selected_index = none_selected;

            saveButton.Enabled = false;
            moveLeftButton.Enabled = false;
            moveRightButton.Enabled = false;
            deleteButton.Enabled = false;
            rotateButton.Enabled = false;
            buttonLandscape.Enabled = false;
            checkSided2ButtonStatus();
          }
        }

        private void imageLoad_Click(object sender, EventArgs e)
        {
          OpenFileDialog openFileDialog1 = new OpenFileDialog();

          // Set the file dialog to filter for graphics files. 
          openFileDialog1.Filter =
              "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF,*.PNG|" +
              "All files (*.*)|*.*";

          // Allow the user to select multiple images. 
          openFileDialog1.Multiselect = true;
          openFileDialog1.Title = "My Image Browser";

          if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
            // load the files
            for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
            {
              ImageContainer myImage = new ImageContainer(openFileDialog1.FileNames[i]);
              Control picBox = myImage.getPictureBox();
              picBox.Click += picBox_Click;
              imagePanel.Controls.Add(picBox);

              checkSided2ButtonStatus();
            }
          }

          if(imagePanel.Controls.Count > 0)
          {
            saveButton.Enabled = true;
          }

          button3.Focus();
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
          ImageContainer imgContainer = (ImageContainer)imagePanel.Controls[selected_index].Tag;
          imgContainer.rotate();
        }

        private void sided2Button_Click(object sender, EventArgs e)
        {
          
          int loc_of_next = 1;
          while(loc_of_next < imagePanel.Controls.Count - 1 )
          {
            // keep moving the last page into position
            imagePanel.Controls.SetChildIndex(imagePanel.Controls[imagePanel.Controls.Count-1], loc_of_next);
            loc_of_next += 2;
          }
        }

        private void buttonLandscape_Click(object sender, EventArgs e)
        {
          ImageContainer imgContainer = (ImageContainer)imagePanel.Controls[selected_index].Tag;
          imgContainer.makeLandscape();
        }
    }

}

