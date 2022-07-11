﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace RetroDevStudio.Documents
{
  public partial class Help : BaseDocument
  {
    private int         ZoomFactor = 100;



    public Help( StudioCore Core )
    {
      this.Core = Core;
      HideOnClose = true;

      InitializeComponent();

      webBrowser.ScriptErrorsSuppressed = true;

      try
      {
#if DEBUG
        string    helpDocPath = @"..\..\..\..\Doc\main.html";
#else
        string    helpDocPath = @"Doc\main.html";
#endif
        string    fullPath = System.IO.Path.Combine( System.IO.Path.GetDirectoryName( Application.ExecutablePath ), helpDocPath );

        Core.AddToOutput( "Help Path: " + fullPath );
        webBrowser.Navigate( fullPath );
      }
      catch ( Exception ex )
      {
        Debug.Log( "Got exception: " + ex.ToString() );
        Core.AddToOutput( "Help exception: " + ex.ToString() );
      }
      webBrowser.CanGoBackChanged += new EventHandler( webBrowser_CanGoBackChanged );
      webBrowser.CanGoForwardChanged += new EventHandler( webBrowser_CanGoForwardChanged );
      toolStripBtnForward.Enabled = webBrowser.CanGoForward;
      toolStripBtnBack.Enabled = webBrowser.CanGoBack;
    }



    void webBrowser_CanGoForwardChanged( object sender, EventArgs e )
    {
      toolStripBtnForward.Enabled = webBrowser.CanGoForward;
    }



    void webBrowser_CanGoBackChanged( object sender, EventArgs e )
    {
      toolStripBtnBack.Enabled = webBrowser.CanGoBack;
    }



    private void toolStripBtnBack_Click( object sender, EventArgs e )
    {
      webBrowser.GoBack();
    }



    private void toolStripBtnHome_Click( object sender, EventArgs e )
    {
#if DEBUG
      webBrowser.Navigate( System.IO.Path.Combine( System.AppDomain.CurrentDomain.BaseDirectory, "../../../../Doc/main.html" ) );
#else
      webBrowser.Navigate( System.IO.Path.Combine( System.AppDomain.CurrentDomain.BaseDirectory, "Doc/main.html" ) );
#endif
    }



    public void NavigateTo( string URL )
    {
#if DEBUG
      webBrowser.Navigate( System.IO.Path.Combine( System.AppDomain.CurrentDomain.BaseDirectory, "../../../../Doc/" + URL ) );
#else
      webBrowser.Navigate( System.IO.Path.Combine( System.AppDomain.CurrentDomain.BaseDirectory, "Doc/" + URL ) );
#endif
    }



    private void toolStripBtnForward_Click( object sender, EventArgs e )
    {
      webBrowser.GoForward();
    }



    private void toolStripBtnZoomIn_Click( object sender, EventArgs e )
    {
      if ( ZoomFactor < 1000 )
      {
        ZoomFactor += 10;
      }
      webBrowser.Zoom( ZoomFactor ); 
    }



    private void toolStripBtnZoomOut_Click( object sender, EventArgs e )
    {
      
      if ( ZoomFactor > 10 )
      {
        ZoomFactor -= 10;
      }
      webBrowser.Zoom( ZoomFactor );
    }



    private void toolStripBtnZoomReset_Click( object sender, EventArgs e )
    {
      ZoomFactor = 100;
      webBrowser.Zoom( ZoomFactor );
    }
  }
}
