﻿using System;
using System.Collections.Generic;
using System.Text;


namespace Utils
{
  public class ResolutionDpi
  {
    private double fHorizontal;
    private double fVertical;


    public double Horizontal
    {
      get { return fHorizontal; }
    }


    public double Vertical
    {
      get { return fVertical; }
    }


    public ResolutionDpi(double horizontal, double vertical)
    {
      fHorizontal = horizontal;
      fVertical = vertical;
    }


    public ResolutionDpi Transform(bool sideways)
    {
      return sideways ? new ResolutionDpi(fVertical, fHorizontal) : this;
    }
  }
}