// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ErrorPiece
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class ErrorPiece : ModalPiece
  {
    private Label lblMessage;
    private PictureBox pictureIcon;
    private Button btnOk;
    private Button btnSupport;
    private TableLayoutPanel okDetailsTableLayoutPanel;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private LinkLabel errorLink;
    private string _errorMessage;
    private string _logFileLocation;
    private string _linkUrl;
    private string _linkUrlMessage;

    public ErrorPiece(UserInterfaceForm parentForm, string errorTitle, string errorMessage, string logFileLocation, string linkUrl, string linkUrlMessage, ManualResetEvent modalEvent)
    {
      this._errorMessage = errorMessage;
      this._logFileLocation = logFileLocation;
      this._linkUrl = linkUrl;
      this._linkUrlMessage = linkUrlMessage;
      this._modalResult = UserInterfaceModalResult.Ok;
      this._modalEvent = modalEvent;
      this.SuspendLayout();
      this.InitializeComponent();
      this.InitializeContent();
      this.ResumeLayout(false);
      parentForm.SuspendLayout();
      parentForm.SwitchUserInterfacePiece((FormPiece) this);
      parentForm.Text = errorTitle;
      parentForm.MinimizeBox = false;
      parentForm.MaximizeBox = false;
      parentForm.ControlBox = true;
      parentForm.ActiveControl = (Control) this.btnOk;
      parentForm.ResumeLayout(false);
      parentForm.PerformLayout();
      parentForm.Visible = true;
      if (Form.ActiveForm == parentForm)
        return;
      parentForm.Activate();
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ErrorPiece));
      this.lblMessage = new Label();
      this.pictureIcon = new PictureBox();
      this.btnOk = new Button();
      this.btnSupport = new Button();
      this.okDetailsTableLayoutPanel = new TableLayoutPanel();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      this.errorLink = new LinkLabel();
      ((ISupportInitialize) this.pictureIcon).BeginInit();
      this.okDetailsTableLayoutPanel.SuspendLayout();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      componentResourceManager.ApplyResources((object) this.lblMessage, "lblMessage");
      this.lblMessage.Name = "lblMessage";
      componentResourceManager.ApplyResources((object) this.pictureIcon, "pictureIcon");
      this.pictureIcon.Name = "pictureIcon";
      this.pictureIcon.TabStop = false;
      componentResourceManager.ApplyResources((object) this.btnOk, "btnOk");
      this.btnOk.MinimumSize = new Size(78, 28);
      this.btnOk.Name = "btnOk";
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      componentResourceManager.ApplyResources((object) this.btnSupport, "btnSupport");
      this.btnSupport.MinimumSize = new Size(78, 28);
      this.btnSupport.Name = "btnSupport";
      this.btnSupport.Click += new EventHandler(this.btnSupport_Click);
      componentResourceManager.ApplyResources((object) this.okDetailsTableLayoutPanel, "okDetailsTableLayoutPanel");
      this.overarchingTableLayoutPanel.SetColumnSpan((Control) this.okDetailsTableLayoutPanel, 2);
      this.okDetailsTableLayoutPanel.Controls.Add((Control) this.btnOk, 0, 0);
      this.okDetailsTableLayoutPanel.Controls.Add((Control) this.btnSupport, 1, 0);
      this.okDetailsTableLayoutPanel.Name = "okDetailsTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.pictureIcon, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.okDetailsTableLayoutPanel, 0, 2);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.lblMessage, 1, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.errorLink, 1, 1);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this.errorLink, "errorLink");
      this.errorLink.MinimumSize = new Size(280, 32);
      this.errorLink.Name = "errorLink";
      this.errorLink.LinkClicked += new LinkLabelLinkClickedEventHandler(this.errorLink_LinkClicked);
      componentResourceManager.ApplyResources((object) this, "$this");
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.Name = "ErrorPiece";
      ((ISupportInitialize) this.pictureIcon).EndInit();
      this.okDetailsTableLayoutPanel.ResumeLayout(false);
      this.okDetailsTableLayoutPanel.PerformLayout();
      this.overarchingTableLayoutPanel.ResumeLayout(false);
      this.overarchingTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void InitializeContent()
    {
      Bitmap image = (Bitmap) Resources.GetImage("information.bmp");
      image.MakeTransparent();
      this.pictureIcon.Image = (Image) image;
      this.lblMessage.Text = this._errorMessage;
      if (this._linkUrl != null && this._linkUrlMessage != null)
      {
        string str = Resources.GetString("UI_ErrorClickHereHere");
        this.errorLink.Text = this._linkUrlMessage;
        this.errorLink.Links.Add(this._linkUrlMessage.LastIndexOf(str, StringComparison.Ordinal), str.Length, (object) this._linkUrl);
      }
      else
      {
        this.errorLink.Text = string.Empty;
        this.errorLink.Links.Clear();
      }
      if (this._logFileLocation != null)
        return;
      this.btnSupport.Enabled = false;
    }

    private void errorLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      this.errorLink.Links[this.errorLink.Links.IndexOf(e.Link)].Visited = true;
      if (this._linkUrl == null || !UserInterface.IsValidHttpUrl(this._linkUrl))
        return;
      UserInterface.LaunchUrlInBrowser(e.Link.LinkData.ToString());
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this._modalResult = UserInterfaceModalResult.Ok;
      this._modalEvent.Set();
      this.Enabled = false;
    }

    private void btnSupport_Click(object sender, EventArgs e)
    {
      try
      {
        Process.Start("notepad.exe", this._logFileLocation);
      }
      catch (Win32Exception ex)
      {
      }
    }
  }
}
