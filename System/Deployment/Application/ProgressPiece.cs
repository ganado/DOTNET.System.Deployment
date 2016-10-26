// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ProgressPiece
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class ProgressPiece : FormPiece, IDownloadNotification
  {
    private static long[] _bytesFormatRanges = new long[9]
    {
      1024L,
      10240L,
      102400L,
      1048576L,
      10485760L,
      104857600L,
      1073741824L,
      10737418240L,
      107374182400L
    };
    private static string[] _bytesFormatStrings = new string[10]
    {
      "UI_ProgressBytesInBytes",
      "UI_ProgressBytesIn1KB",
      "UI_ProgressBytesIn10KB",
      "UI_ProgressBytesIn100KB",
      "UI_ProgressBytesIn1MB",
      "UI_ProgressBytesIn10MB",
      "UI_ProgressBytesIn100MB",
      "UI_ProgressBytesIn1GB",
      "UI_ProgressBytesIn10GB",
      "UI_ProgressBytesIn100GB"
    };
    private Label lblHeader;
    private Label lblSubHeader;
    private PictureBox pictureDesktop;
    private PictureBox pictureAppIcon;
    private Label lblApplication;
    private LinkLabel linkAppId;
    private Label lblFrom;
    private Label lblFromId;
    private ProgressBar progress;
    private Label lblProgressText;
    private GroupBox groupRule;
    private GroupBox groupDivider;
    private Button btnCancel;
    private TableLayoutPanel topTextTableLayoutPanel;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private TableLayoutPanel contentTableLayoutPanel;
    private UserInterfaceInfo _info;
    private bool _userCancelling;
    private DownloadEventArgs _downloadData;
    private Bitmap _appIconBitmap;
    private bool _appIconShown;
    private UserInterfaceForm _parentForm;
    private MethodInvoker disableMethodInvoker;
    private MethodInvoker updateUIMethodInvoker;

    public ProgressPiece(UserInterfaceForm parentForm, UserInterfaceInfo info)
    {
      this._info = info;
      this.SuspendLayout();
      this.InitializeComponent();
      this.InitializeContent();
      this.ResumeLayout(false);
      parentForm.SuspendLayout();
      parentForm.SwitchUserInterfacePiece((FormPiece) this);
      parentForm.Text = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_ProgressTitle"), new object[2]
      {
        (object) 0,
        (object) this._info.formTitle
      });
      parentForm.MinimizeBox = true;
      parentForm.MaximizeBox = false;
      parentForm.ControlBox = true;
      this.lblHeader.Font = new Font(this.lblHeader.Font, this.lblHeader.Font.Style | FontStyle.Bold);
      this.linkAppId.Font = new Font(this.linkAppId.Font, this.linkAppId.Font.Style | FontStyle.Bold);
      this.lblFromId.Font = new Font(this.lblFromId.Font, this.lblFromId.Font.Style | FontStyle.Bold);
      parentForm.ActiveControl = (Control) this.btnCancel;
      parentForm.ResumeLayout(false);
      parentForm.PerformLayout();
      parentForm.Visible = true;
      this.updateUIMethodInvoker = new MethodInvoker(this.UpdateUI);
      this.disableMethodInvoker = new MethodInvoker(this.Disable);
      this._parentForm = parentForm;
    }

    public void DownloadModified(object sender, DownloadEventArgs e)
    {
      if (this._userCancelling)
      {
        ((FileDownloader) sender).Cancel();
      }
      else
      {
        this._downloadData = e;
        if (this._info.iconFilePath != null && this._appIconBitmap == null && (e.Cookie != null && File.Exists(this._info.iconFilePath)))
        {
          using (Icon associatedIcon = Icon.ExtractAssociatedIcon(this._info.iconFilePath))
            this._appIconBitmap = this.TryGet32x32Bitmap(associatedIcon);
        }
        this.BeginInvoke((Delegate) this.updateUIMethodInvoker);
      }
    }

    public void DownloadCompleted(object sender, DownloadEventArgs e)
    {
      this.BeginInvoke((Delegate) this.disableMethodInvoker);
    }

    public override bool OnClosing()
    {
      bool flag = base.OnClosing();
      if (!this.Enabled)
        return false;
      this._userCancelling = true;
      return flag;
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ProgressPiece));
      this.topTextTableLayoutPanel = new TableLayoutPanel();
      this.pictureDesktop = new PictureBox();
      this.lblSubHeader = new Label();
      this.lblHeader = new Label();
      this.pictureAppIcon = new PictureBox();
      this.lblApplication = new Label();
      this.linkAppId = new LinkLabel();
      this.lblFrom = new Label();
      this.lblFromId = new Label();
      this.progress = new ProgressBar();
      this.lblProgressText = new Label();
      this.groupRule = new GroupBox();
      this.groupDivider = new GroupBox();
      this.btnCancel = new Button();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      this.contentTableLayoutPanel = new TableLayoutPanel();
      this.topTextTableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.pictureDesktop).BeginInit();
      ((ISupportInitialize) this.pictureAppIcon).BeginInit();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this.contentTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      componentResourceManager.ApplyResources((object) this.topTextTableLayoutPanel, "topTextTableLayoutPanel");
      this.topTextTableLayoutPanel.BackColor = SystemColors.Window;
      this.topTextTableLayoutPanel.Controls.Add((Control) this.pictureDesktop, 1, 0);
      this.topTextTableLayoutPanel.Controls.Add((Control) this.lblSubHeader, 0, 1);
      this.topTextTableLayoutPanel.Controls.Add((Control) this.lblHeader, 0, 0);
      this.topTextTableLayoutPanel.MinimumSize = new Size(498, 61);
      this.topTextTableLayoutPanel.Name = "topTextTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this.pictureDesktop, "pictureDesktop");
      this.pictureDesktop.MinimumSize = new Size(61, 61);
      this.pictureDesktop.Name = "pictureDesktop";
      this.topTextTableLayoutPanel.SetRowSpan((Control) this.pictureDesktop, 2);
      this.pictureDesktop.TabStop = false;
      componentResourceManager.ApplyResources((object) this.lblSubHeader, "lblSubHeader");
      this.lblSubHeader.Name = "lblSubHeader";
      componentResourceManager.ApplyResources((object) this.lblHeader, "lblHeader");
      this.lblHeader.AutoEllipsis = true;
      this.lblHeader.Name = "lblHeader";
      this.lblHeader.UseMnemonic = false;
      componentResourceManager.ApplyResources((object) this.pictureAppIcon, "pictureAppIcon");
      this.pictureAppIcon.Name = "pictureAppIcon";
      this.pictureAppIcon.TabStop = false;
      componentResourceManager.ApplyResources((object) this.lblApplication, "lblApplication");
      this.lblApplication.Name = "lblApplication";
      componentResourceManager.ApplyResources((object) this.linkAppId, "linkAppId");
      this.linkAppId.AutoEllipsis = true;
      this.linkAppId.Name = "linkAppId";
      this.linkAppId.UseMnemonic = false;
      this.linkAppId.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkAppId_LinkClicked);
      componentResourceManager.ApplyResources((object) this.lblFrom, "lblFrom");
      this.lblFrom.Name = "lblFrom";
      componentResourceManager.ApplyResources((object) this.lblFromId, "lblFromId");
      this.lblFromId.AutoEllipsis = true;
      this.lblFromId.MinimumSize = new Size(384, 32);
      this.lblFromId.Name = "lblFromId";
      this.lblFromId.UseMnemonic = false;
      componentResourceManager.ApplyResources((object) this.progress, "progress");
      this.contentTableLayoutPanel.SetColumnSpan((Control) this.progress, 2);
      this.progress.Name = "progress";
      this.progress.TabStop = false;
      componentResourceManager.ApplyResources((object) this.lblProgressText, "lblProgressText");
      this.contentTableLayoutPanel.SetColumnSpan((Control) this.lblProgressText, 2);
      this.lblProgressText.Name = "lblProgressText";
      componentResourceManager.ApplyResources((object) this.groupRule, "groupRule");
      this.groupRule.BackColor = SystemColors.ControlDark;
      this.groupRule.FlatStyle = FlatStyle.Flat;
      this.groupRule.Name = "groupRule";
      this.groupRule.TabStop = false;
      componentResourceManager.ApplyResources((object) this.groupDivider, "groupDivider");
      this.groupDivider.BackColor = SystemColors.ControlDark;
      this.groupDivider.FlatStyle = FlatStyle.Flat;
      this.groupDivider.Name = "groupDivider";
      this.groupDivider.TabStop = false;
      componentResourceManager.ApplyResources((object) this.btnCancel, "btnCancel");
      this.btnCancel.MinimumSize = new Size(75, 23);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.contentTableLayoutPanel, 0, 2);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.topTextTableLayoutPanel, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.groupRule, 0, 1);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.btnCancel, 0, 4);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.groupDivider, 0, 3);
      this.overarchingTableLayoutPanel.MinimumSize = new Size(515, 240);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this.contentTableLayoutPanel, "contentTableLayoutPanel");
      this.contentTableLayoutPanel.Controls.Add((Control) this.pictureAppIcon, 0, 0);
      this.contentTableLayoutPanel.Controls.Add((Control) this.lblApplication, 1, 0);
      this.contentTableLayoutPanel.Controls.Add((Control) this.lblFrom, 1, 1);
      this.contentTableLayoutPanel.Controls.Add((Control) this.lblProgressText, 1, 3);
      this.contentTableLayoutPanel.Controls.Add((Control) this.linkAppId, 2, 0);
      this.contentTableLayoutPanel.Controls.Add((Control) this.progress, 1, 2);
      this.contentTableLayoutPanel.Controls.Add((Control) this.lblFromId, 2, 1);
      this.contentTableLayoutPanel.MinimumSize = new Size(466, 123);
      this.contentTableLayoutPanel.Name = "contentTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this, "$this");
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.MinimumSize = new Size(498, 240);
      this.Name = "ProgressPiece";
      this.topTextTableLayoutPanel.ResumeLayout(false);
      this.topTextTableLayoutPanel.PerformLayout();
      ((ISupportInitialize) this.pictureDesktop).EndInit();
      ((ISupportInitialize) this.pictureAppIcon).EndInit();
      this.overarchingTableLayoutPanel.ResumeLayout(false);
      this.overarchingTableLayoutPanel.PerformLayout();
      this.contentTableLayoutPanel.ResumeLayout(false);
      this.contentTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void InitializeContent()
    {
      this.pictureDesktop.Image = Resources.GetImage("setup.bmp");
      this.lblHeader.Text = this._info.formTitle;
      using (Icon icon = Resources.GetIcon("defaultappicon.ico"))
        this.pictureAppIcon.Image = (Image) this.TryGet32x32Bitmap(icon);
      this.linkAppId.Text = this._info.productName;
      this.linkAppId.Links.Clear();
      if (UserInterface.IsValidHttpUrl(this._info.supportUrl))
        this.linkAppId.Links.Add(0, this._info.productName.Length, (object) this._info.supportUrl);
      this.lblFromId.Text = this._info.sourceSite;
    }

    private void linkAppId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      this.linkAppId.LinkVisited = true;
      UserInterface.LaunchUrlInBrowser(e.Link.LinkData.ToString());
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this._userCancelling = true;
      this.Disable();
      this._parentForm.Visible = false;
    }

    private void Disable()
    {
      this.lblProgressText.Text = Resources.GetString("UI_ProgressDone");
      this.Enabled = false;
    }

    private Bitmap TryGet32x32Bitmap(Icon icon)
    {
      using (Icon icon1 = new Icon(icon, 32, 32))
      {
        Bitmap bitmap = icon1.ToBitmap();
        bitmap.MakeTransparent();
        return bitmap;
      }
    }

    private void UpdateUI()
    {
      if (this.IsDisposed)
        return;
      this.SuspendLayout();
      this.lblProgressText.Text = ProgressPiece.FormatProgressText(this._downloadData.BytesCompleted, this._downloadData.BytesTotal);
      this.progress.Minimum = 0;
      long bytesTotal = this._downloadData.BytesTotal;
      int num1;
      int num2;
      if (bytesTotal > (long) int.MaxValue)
      {
        num1 = (int) ((double) this._downloadData.BytesCompleted / (double) ((float) bytesTotal / (float) int.MaxValue));
        num2 = int.MaxValue;
      }
      else
      {
        num1 = (int) this._downloadData.BytesCompleted;
        num2 = (int) bytesTotal;
      }
      this.progress.Maximum = num2;
      this.progress.Value = num1;
      this.FindForm().Text = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_ProgressTitle"), new object[2]
      {
        (object) this._downloadData.Progress,
        (object) this._info.formTitle
      });
      if (!this._appIconShown && this._appIconBitmap != null)
      {
        this.pictureAppIcon.Image = (Image) this._appIconBitmap;
        this._appIconShown = true;
      }
      this.ResumeLayout(false);
    }

    private static string FormatProgressText(long completed, long total)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_ProgressText"), new object[2]
      {
        (object) ProgressPiece.FormatBytes(completed),
        (object) ProgressPiece.FormatBytes(total)
      });
    }

    private static string FormatBytes(long bytes)
    {
      int num = Array.BinarySearch<long>(ProgressPiece._bytesFormatRanges, bytes);
      int index = num >= 0 ? num + 1 : ~num;
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString(ProgressPiece._bytesFormatStrings[index]), new object[1]
      {
        (object) (float) (index == 0 ? (double) bytes : (double) bytes / (double) ProgressPiece._bytesFormatRanges[(index - 1) / 3 * 3])
      });
    }
  }
}
