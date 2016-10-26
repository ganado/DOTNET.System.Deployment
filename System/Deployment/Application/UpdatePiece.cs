// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.UpdatePiece
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class UpdatePiece : ModalPiece
  {
    private Label lblHeader;
    private Label lblSubHeader;
    private PictureBox pictureDesktop;
    private Label lblApplication;
    private LinkLabel linkAppId;
    private Label lblFrom;
    private Label lblFromId;
    private GroupBox groupRule;
    private GroupBox groupDivider;
    private Button btnOk;
    private Button btnSkip;
    private TableLayoutPanel contentTableLayoutPanel;
    private TableLayoutPanel descriptionTableLayoutPanel;
    private TableLayoutPanel okSkipTableLayoutPanel;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private UserInterfaceInfo _info;

    public UpdatePiece(UserInterfaceForm parentForm, UserInterfaceInfo info, ManualResetEvent modalEvent)
    {
      this._info = info;
      this._modalEvent = modalEvent;
      this._modalResult = UserInterfaceModalResult.Cancel;
      this.SuspendLayout();
      this.InitializeComponent();
      this.InitializeContent();
      this.ResumeLayout(false);
      parentForm.SuspendLayout();
      parentForm.SwitchUserInterfacePiece((FormPiece) this);
      parentForm.Text = this._info.formTitle;
      parentForm.MinimizeBox = false;
      parentForm.MaximizeBox = false;
      parentForm.ControlBox = true;
      this.lblHeader.Font = new Font(this.lblHeader.Font, this.lblHeader.Font.Style | FontStyle.Bold);
      this.linkAppId.Font = new Font(this.linkAppId.Font, this.linkAppId.Font.Style | FontStyle.Bold);
      this.lblFromId.Font = new Font(this.lblFromId.Font, this.lblFromId.Font.Style | FontStyle.Bold);
      parentForm.ActiveControl = (Control) this.btnOk;
      parentForm.ResumeLayout(false);
      parentForm.PerformLayout();
      parentForm.Visible = true;
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UpdatePiece));
      this.descriptionTableLayoutPanel = new TableLayoutPanel();
      this.pictureDesktop = new PictureBox();
      this.lblSubHeader = new Label();
      this.lblHeader = new Label();
      this.lblApplication = new Label();
      this.linkAppId = new LinkLabel();
      this.lblFrom = new Label();
      this.lblFromId = new Label();
      this.groupRule = new GroupBox();
      this.groupDivider = new GroupBox();
      this.btnOk = new Button();
      this.btnSkip = new Button();
      this.contentTableLayoutPanel = new TableLayoutPanel();
      this.okSkipTableLayoutPanel = new TableLayoutPanel();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      this.descriptionTableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.pictureDesktop).BeginInit();
      this.contentTableLayoutPanel.SuspendLayout();
      this.okSkipTableLayoutPanel.SuspendLayout();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      componentResourceManager.ApplyResources((object) this.descriptionTableLayoutPanel, "descriptionTableLayoutPanel");
      this.descriptionTableLayoutPanel.BackColor = SystemColors.Window;
      this.descriptionTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400f));
      this.descriptionTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60f));
      this.descriptionTableLayoutPanel.Controls.Add((Control) this.pictureDesktop, 1, 0);
      this.descriptionTableLayoutPanel.Controls.Add((Control) this.lblSubHeader, 0, 1);
      this.descriptionTableLayoutPanel.Controls.Add((Control) this.lblHeader, 0, 0);
      this.descriptionTableLayoutPanel.Margin = new Padding(0);
      this.descriptionTableLayoutPanel.Name = "descriptionTableLayoutPanel";
      this.descriptionTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.descriptionTableLayoutPanel.RowStyles.Add(new RowStyle());
      componentResourceManager.ApplyResources((object) this.pictureDesktop, "pictureDesktop");
      this.pictureDesktop.Margin = new Padding(3, 0, 0, 0);
      this.pictureDesktop.Name = "pictureDesktop";
      this.descriptionTableLayoutPanel.SetRowSpan((Control) this.pictureDesktop, 2);
      this.pictureDesktop.TabStop = false;
      componentResourceManager.ApplyResources((object) this.lblSubHeader, "lblSubHeader");
      this.lblSubHeader.Margin = new Padding(29, 3, 3, 8);
      this.lblSubHeader.Name = "lblSubHeader";
      componentResourceManager.ApplyResources((object) this.lblHeader, "lblHeader");
      this.lblHeader.Margin = new Padding(10, 11, 3, 0);
      this.lblHeader.Name = "lblHeader";
      componentResourceManager.ApplyResources((object) this.lblApplication, "lblApplication");
      this.lblApplication.Margin = new Padding(0, 0, 3, 3);
      this.lblApplication.Name = "lblApplication";
      componentResourceManager.ApplyResources((object) this.linkAppId, "linkAppId");
      this.linkAppId.AutoEllipsis = true;
      this.linkAppId.Margin = new Padding(3, 0, 0, 3);
      this.linkAppId.Name = "linkAppId";
      this.linkAppId.TabStop = true;
      this.linkAppId.UseMnemonic = false;
      this.linkAppId.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkAppId_LinkClicked);
      componentResourceManager.ApplyResources((object) this.lblFrom, "lblFrom");
      this.lblFrom.Margin = new Padding(0, 3, 3, 0);
      this.lblFrom.Name = "lblFrom";
      componentResourceManager.ApplyResources((object) this.lblFromId, "lblFromId");
      this.lblFromId.AutoEllipsis = true;
      this.lblFromId.Margin = new Padding(3, 3, 0, 0);
      this.lblFromId.Name = "lblFromId";
      this.lblFromId.UseMnemonic = false;
      componentResourceManager.ApplyResources((object) this.groupRule, "groupRule");
      this.groupRule.Margin = new Padding(0, 0, 0, 3);
      this.groupRule.BackColor = SystemColors.ControlDark;
      this.groupRule.FlatStyle = FlatStyle.Flat;
      this.groupRule.Name = "groupRule";
      this.groupRule.TabStop = false;
      componentResourceManager.ApplyResources((object) this.groupDivider, "groupDivider");
      this.groupDivider.Margin = new Padding(0, 3, 0, 3);
      this.groupDivider.BackColor = SystemColors.ControlDark;
      this.groupDivider.FlatStyle = FlatStyle.Flat;
      this.groupDivider.Name = "groupDivider";
      this.groupDivider.TabStop = false;
      componentResourceManager.ApplyResources((object) this.btnOk, "btnOk");
      this.btnOk.Margin = new Padding(0, 0, 3, 0);
      this.btnOk.MinimumSize = new Size(75, 23);
      this.btnOk.Name = "btnOk";
      this.btnOk.Padding = new Padding(10, 0, 10, 0);
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      componentResourceManager.ApplyResources((object) this.btnSkip, "btnSkip");
      this.btnSkip.Margin = new Padding(3, 0, 0, 0);
      this.btnSkip.MinimumSize = new Size(75, 23);
      this.btnSkip.Name = "btnSkip";
      this.btnSkip.Padding = new Padding(10, 0, 10, 0);
      this.btnSkip.Click += new EventHandler(this.btnSkip_Click);
      componentResourceManager.ApplyResources((object) this.contentTableLayoutPanel, "contentTableLayoutPanel");
      this.contentTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.contentTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.contentTableLayoutPanel.Controls.Add((Control) this.lblApplication, 0, 0);
      this.contentTableLayoutPanel.Controls.Add((Control) this.lblFrom, 0, 1);
      this.contentTableLayoutPanel.Controls.Add((Control) this.linkAppId, 1, 0);
      this.contentTableLayoutPanel.Controls.Add((Control) this.lblFromId, 1, 1);
      this.contentTableLayoutPanel.Margin = new Padding(20, 15, 12, 18);
      this.contentTableLayoutPanel.Name = "contentTableLayoutPanel";
      this.contentTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.contentTableLayoutPanel.RowStyles.Add(new RowStyle());
      componentResourceManager.ApplyResources((object) this.okSkipTableLayoutPanel, "okSkipTableLayoutPanel");
      this.okSkipTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.okSkipTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.okSkipTableLayoutPanel.Controls.Add((Control) this.btnOk, 0, 0);
      this.okSkipTableLayoutPanel.Controls.Add((Control) this.btnSkip, 1, 0);
      this.okSkipTableLayoutPanel.Margin = new Padding(0, 7, 8, 6);
      this.okSkipTableLayoutPanel.Name = "okSkipTableLayoutPanel";
      this.okSkipTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.descriptionTableLayoutPanel, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.okSkipTableLayoutPanel, 0, 4);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.contentTableLayoutPanel, 0, 2);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.groupRule, 0, 1);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.groupDivider, 0, 3);
      this.overarchingTableLayoutPanel.Margin = new Padding(0);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      componentResourceManager.ApplyResources((object) this, "$this");
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.Name = "UpdatePiece";
      this.descriptionTableLayoutPanel.ResumeLayout(false);
      this.descriptionTableLayoutPanel.PerformLayout();
      ((ISupportInitialize) this.pictureDesktop).EndInit();
      this.contentTableLayoutPanel.ResumeLayout(false);
      this.contentTableLayoutPanel.PerformLayout();
      this.okSkipTableLayoutPanel.ResumeLayout(false);
      this.okSkipTableLayoutPanel.PerformLayout();
      this.overarchingTableLayoutPanel.ResumeLayout(false);
      this.overarchingTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void InitializeContent()
    {
      this.pictureDesktop.Image = Resources.GetImage("setup.bmp");
      this.lblSubHeader.Text = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_UpdateSubHeader"), new object[1]
      {
        (object) UserInterface.LimitDisplayTextLength(this._info.productName)
      });
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

    private void btnOk_Click(object sender, EventArgs e)
    {
      this._modalResult = UserInterfaceModalResult.Ok;
      this._modalEvent.Set();
      this.Enabled = false;
    }

    private void btnSkip_Click(object sender, EventArgs e)
    {
      this._modalResult = UserInterfaceModalResult.Skip;
      this._modalEvent.Set();
      this.Enabled = false;
    }
  }
}
