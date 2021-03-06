﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Scanning;
using Defines;
using Utils;
using Imports;


namespace Scanning
{
  partial class TwainDataSourceManager
  {
    private class DataSource : InterfaceDataSource, IMessageFilter
    {
      private enum StateType { Closed = 0, Idle, Active };

      private TwainDevice fTwain;
      private TwIdentity fIdent;
      private StateType fState;
      private DataSourceSettings fSettings;
      private DataSourceCapabilities fCapabilities;


      public DataSource(TwainDevice twain, TwIdentity identity)
      {
        fTwain = twain;
        fIdent = identity;
        fState = StateType.Closed;
        fSettings = null;
        fCapabilities = null;
      }


      public string Name
      {
        get { return "TWAIN I/F: " + fIdent.ProductName; }
      }

      
      private TwainCapability fCapabilitySupportedTypes;
      private TwainCapability fCapabilityPixelType;
      private TwainCapability fCapabilityPageSize;
      private TwainCapability fCapabilityResolutionX;
      private TwainCapability fCapabilityResolutionY;
      private TwainCapability fCapabilityFeederEnabled;
      private TwainCapability fCapabilityFeederLoaded;
      private TwainCapability fCapabilityBitDepthReduction;
      private TwainCapability fCapabilityThreshold;
      private TwainCapability fCapabilityBrightness;
      private TwainCapability fCapabilityContrast;


      public DataSourceCapabilities GetCapabilities()
      {
        if(fCapabilities == null)
        {
          if(this.Open())
          {
            fCapabilities = new DataSourceCapabilities();

            fCapabilities.ColorModes = GetAvailableValuesForColorMode();
            fCapabilities.PageTypes = GetAvailableValuesForPageType();
            fCapabilities.Resolutions = GetAvailableValuesForResolution();

            this.Close();
          }
        }

        return fCapabilities;
      }


      private List<ColorModeEnum> GetAvailableValuesForColorMode()
      {
        List<ColorModeEnum> result = new List<ColorModeEnum>();

        foreach(object item in fCapabilityPixelType.Items)
        {
          switch((TwCapPixelType)item)
          {
            case TwCapPixelType.BW: result.Add(ColorModeEnum.BW); break;
            case TwCapPixelType.RGB: result.Add(ColorModeEnum.RGB); break;
            case TwCapPixelType.Gray: result.Add(ColorModeEnum.Gray); break;
            default: break; // Unused capability
          }
        }

        return result;
      }


      private List<PageTypeEnum> GetAvailableValuesForPageType()
      {
        List<PageTypeEnum> result = new List<PageTypeEnum>();

        foreach(object item in fCapabilityPageSize.Items)
        {
          switch((TwCapPageType)item)
          {
            case TwCapPageType.UsLetter: result.Add(PageTypeEnum.Letter); break;
            case TwCapPageType.UsLegal: result.Add(PageTypeEnum.Legal); break;
            default: break; // Unused capability
          }
        }

        return result;
      }


      private List<int> GetAvailableValuesForResolution()
      {
        List<int> result = new List<int>();

        foreach(object item in fCapabilityResolutionX.Items)
        {
          result.Add((int)(float)item);
        }

        return result;
      }


      private bool IsOpen
      {
        get { return (bool)(fState > StateType.Closed); }
      }


      private bool Open()
      {
        if(fTwain.OpenDataSource(fIdent))
        {
          fState = StateType.Idle;

          fCapabilitySupportedTypes = new TwainCapability(fTwain, fIdent, TwCap.SupportedCapabilities);
          fCapabilityPixelType = new TwainCapability(fTwain, fIdent, TwCap.IPixelType);
          fCapabilityPageSize = new TwainCapability(fTwain, fIdent, TwCap.SupportedSizes);
          fCapabilityResolutionX = new TwainCapability(fTwain, fIdent, TwCap.XResolution);
          fCapabilityResolutionY = new TwainCapability(fTwain, fIdent, TwCap.YResolution);
          fCapabilityFeederEnabled = new TwainCapability(fTwain, fIdent, TwCap.FeederEnabled);
          fCapabilityFeederLoaded = new TwainCapability(fTwain, fIdent, TwCap.FeederLoaded);
          fCapabilityBitDepthReduction = new TwainCapability(fTwain, fIdent, TwCap.BitDepthReduction);
          fCapabilityThreshold = new TwainCapability(fTwain, fIdent, TwCap.Threshold);
          fCapabilityBrightness = new TwainCapability(fTwain, fIdent, TwCap.Brightness);
          fCapabilityContrast = new TwainCapability(fTwain, fIdent, TwCap.Contrast);
        }
        return IsOpen;
      }


      private void Close()
      {
        if(IsOpen)
        {
          fTwain.CloseDataSource(fIdent);
          fState = StateType.Closed;
        }
      }


      private bool IsActive()
      {
        return (bool)(fState == StateType.Active);
      }


      public bool Acquire(DataSourceSettings settings)
      {
        bool success = false;

        if(this.Open())
        {
          fSettings = settings;

          if(fSettings != null)
          {
            // Setup Pixel Type
            TwCapPixelType pixelType;

            switch(fSettings.ColorMode)
            {
              case ColorModeEnum.BW: pixelType = TwCapPixelType.BW; break;
              case ColorModeEnum.RGB: pixelType = TwCapPixelType.RGB; break;
              case ColorModeEnum.Gray: pixelType = TwCapPixelType.Gray; break;
              default: pixelType = TwCapPixelType.BW; break;
            }

            fCapabilityPixelType.CurrentValue = pixelType;

#if USE_PAGE_TYPE
            // Page Type Setting is not used anymore; Image Layout is defined instead
            TwCapPageType pageType;

            switch(fSettings.PageType)
            {
              case PageTypeEnum.Letter: pageType = TwCapPageType.UsLetter; break;
              case PageTypeEnum.Legal: pageType = TwCapPageType.UsLegal; break;
              default: pageType = TwCapPageType.UsLetter; break;
            }

            fCapabilityPageSize.CurrentValue = pageType;
#else
            // Setup Scan Area
            TwImageLayout ilayout = new TwImageLayout();

            if(fTwain.GetImageLayout(fIdent, ilayout))
            {
              BoundsInches scanArea = settings.ScanArea;

              ilayout.Frame.Left = (float)scanArea.X;
              ilayout.Frame.Top = (float)scanArea.Y;
              ilayout.Frame.Right = (float)(scanArea.X + scanArea.Width);
              ilayout.Frame.Bottom = (float)(scanArea.Y + scanArea.Height);
              fTwain.SetImageLayout(fIdent, ilayout);
            }
#endif

            // Setup Resolution
            float resolution = (float)fSettings.Resolution;

            fCapabilityResolutionX.CurrentValue = resolution;
            fCapabilityResolutionY.CurrentValue = resolution;

            // Enable/Disable Document Feeder
            if(fSettings.EnableFeeder)
            {
              fCapabilityFeederEnabled.CurrentValue = fCapabilityFeederLoaded.CurrentValue;
            }
            else
            {
              fCapabilityFeederEnabled.CurrentValue = false;
            }

            // Analog adjustments
            fCapabilityThreshold.ScaledValue = settings.Threshold;
            fCapabilityBrightness.ScaledValue = settings.Brightness;
            fCapabilityContrast.ScaledValue = settings.Contrast;
          }

          Application.AddMessageFilter(this);
          fState = StateType.Active; // This makes IsActive = true

          if(fTwain.StartDataSession(fIdent, settings.ShowSettingsUI, settings.ShowTransferUI))
          {
            success = true;
          }
          else
          {
            Stop(); // TODO: Is this OK?
          }
        }

        return success;
      }


      private void Stop()
      {
        if(IsActive())
        {
          fTwain.TransferEnd(fIdent);
          fTwain.DiscardPendingTransfers(fIdent);
          fTwain.FinishDataSession(fIdent);
          this.Close();
          Application.RemoveMessageFilter(this);
          Raise_OnScanningComplete();
        }
      }


      // IMessageFilter
      public bool PreFilterMessage(ref Message m)
      {
        bool handled = false;

        if(IsActive())
        {
          int pos = LibUser32.GetMessagePos();

          WINMSG message = new WINMSG();
          message.hwnd = m.HWnd;
          message.message = m.Msg;
          message.wParam = m.WParam;
          message.lParam = m.LParam;
          message.time = LibUser32.GetMessageTime();
          message.x = (short)pos;
          message.y = (short)(pos >> 16);

          TwMSG msgCode = 0;

          if(fTwain.ProcessEvent(fIdent, ref message, ref msgCode))
          {
            switch(msgCode)
            {
              case TwMSG.XFerReady:
                {
                  bool success = TransferPictures();

                  if((success == false) || (fSettings.ShowSettingsUI == false))
                  {
                    Stop(); // Need this only if UI is not enabled
                  }
                }
                break;

              case TwMSG.CloseDS:
              case TwMSG.CloseDSOK:
              case TwMSG.CloseDSReq:
                {
                  Stop(); // This happens only if UI is enabled
                }
                break;

              case TwMSG.DeviceEvent:
                break;

              default:
                break;
            }

            handled = true;
          }
        }

        return handled;
      }


      private bool TransferPictures()
      {
        bool success = IsActive();

        if(success)
        {
          TwPendingXfers pxfr = new TwPendingXfers();
          success = fTwain.GetNumberOfPendingTransfers(fIdent, pxfr);

          while(success && (pxfr.Count != 0))
          {
            TwImageInfo iinf = new TwImageInfo();
            success = fTwain.GetImageInfo(fIdent, iinf);

            IntPtr hBitmap = IntPtr.Zero;

            if(success)
            {
              success = fTwain.TransferImage(fIdent, ref hBitmap);
            }

            if(success)
            {
              Image image = null;
              IntPtr ptrBitmap = LibKernel32.GlobalLock(hBitmap);
              image = TwainUtils.DibToImage(ptrBitmap);
              LibKernel32.GlobalUnlock(hBitmap);
              Raise_OnNewPictureData(image, fSettings);
            }

            if(success)
            {
              success = fTwain.TransferEnd(fIdent, pxfr);
            }
          }
        }

        return success;
      }


      public event EventHandler OnNewPictureData;

      private void Raise_OnNewPictureData(Image image, DataSourceSettings theSettings)
      {
        if(OnNewPictureData != null)
        {
          NewPictureEventArgs args = new NewPictureEventArgs();
          args.TheImage = image;
          args.TheSettings = theSettings;
          OnNewPictureData(this, args);
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
}