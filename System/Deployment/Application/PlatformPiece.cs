// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.PlatformPiece
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class PlatformPiece : ModalPiece
  {
    private Label lblMessage;
    private PictureBox pictureIcon;
    private LinkLabel linkSupport;
    private Button btnOk;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private string _errorMessage;
    private Uri _supportUrl;

    public PlatformPiece(UserInterfaceForm parentForm, string platformDetectionErrorMsg, Uri supportUrl, ManualResetEvent modalEvent)
    {
      this._errorMessage = platformDetectionErrorMsg;
      this._supportUrl = supportUrl;
      this._modalResult = UserInterfaceModalResult.Ok;
      this._modalEvent = modalEvent;
      this.SuspendLayout();
      this.InitializeComponent();
      this.InitializeContent();
      this.ResumeLayout(false);
      parentForm.SuspendLayout();
      parentForm.SwitchUserInterfacePiece((FormPiece) this);
      parentForm.Text = Resources.GetString("UI_PlatformDetectionFailedTitle");
      parentForm.MinimizeBox = false;
      parentForm.MaximizeBox = false;
      parentForm.ControlBox = true;
      parentForm.ActiveControl = (Control) this.btnOk;
      parentForm.ResumeLayout(false);
      parentForm.PerformLayout();
      parentForm.Visible = true;
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (PlatformPiece));
      this.lblMessage = new Label();
      this.pictureIcon = new PictureBox();
      this.btnOk = new Button();
      this.linkSupport = new LinkLabel();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      ((ISupportInitialize) this.pictureIcon).BeginInit();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      componentResourceManager.ApplyResources((object) this.lblMessage, "lblMessage");
      this.lblMessage.Name = "lblMessage";
      componentResourceManager.ApplyResources((object) this.pictureIcon, "pictureIcon");
      this.pictureIcon.Name = "pictureIcon";
      this.pictureIcon.TabStop = false;
      componentResourceManager.ApplyResources((object) this.btnOk, "btnOk");
      this.overarchingTableLayoutPanel.SetColumnSpan((Control) this.btnOk, 2);
      this.btnOk.MinimumSize = new Size(75, 23);
      this.btnOk.Name = "btnOk";
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      componentResourceManager.ApplyResources((object) this.linkSupport, "linkSupport");
      this.linkSupport.Name = "linkSupport";
      this.linkSupport.TabStop = true;
      this.linkSupport.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkSupport_LinkClicked);
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.pictureIcon, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.btnOk, 0, 2);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.linkSupport, 1, 1);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.lblMessage, 1, 0);
      this.overarchingTableLayoutPanel.MinimumSize = new Size(349, 88);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this, "$this");
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.MinimumSize = new Size(373, 112);
      this.Name = "PlatformPiece";
      ((ISupportInitialize) this.pictureIcon).EndInit();
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
      this.linkSupport.Links.Clear();
      if (this._supportUrl == (Uri) null)
      {
        this.linkSupport.Text = Resources.GetString("UI_PlatformContactAdmin");
      }
      else
      {
        string str1 = Resources.GetString("UI_PlatformClickHere");
        string str2 = Resources.GetString("UI_PlatformClickHereHere");
        int start = str1.LastIndexOf(str2, StringComparison.Ordinal);
        this.linkSupport.Text = str1;
        this.linkSupport.Links.Add(start, str2.Length, (object) this._supportUrl.AbsoluteUri);
      }
      this.lblMessage.Text = this._errorMessage;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this._modalResult = UserInterfaceModalResult.Ok;
      this._modalEvent.Set();
      this.Enabled = false;
    }

    private void linkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      this.linkSupport.Links[this.linkSupport.Links.IndexOf(e.Link)].Visited = true;
      if (!(this._supportUrl != (Uri) null) || !UserInterface.IsValidHttpUrl(this._supportUrl.AbsoluteUri))
        return;
      UserInterface.LaunchUrlInBrowser(e.Link.LinkData.ToString());
    }
  }
}
