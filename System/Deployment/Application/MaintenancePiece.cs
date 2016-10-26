// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.MaintenancePiece
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class MaintenancePiece : ModalPiece
  {
    private Label lblHeader;
    private Label lblSubHeader;
    private PictureBox pictureDesktop;
    private PictureBox pictureRestore;
    private PictureBox pictureRemove;
    private RadioButton radioRestore;
    private RadioButton radioRemove;
    private GroupBox groupRule;
    private GroupBox groupDivider;
    private Button btnOk;
    private Button btnCancel;
    private Button btnHelp;
    private TableLayoutPanel okCancelHelpTableLayoutPanel;
    private TableLayoutPanel contentTableLayoutPanel;
    private TableLayoutPanel topTableLayoutPanel;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private UserInterfaceInfo _info;
    private MaintenanceInfo _maintenanceInfo;

    public MaintenancePiece(UserInterfaceForm parentForm, UserInterfaceInfo info, MaintenanceInfo maintenanceInfo, ManualResetEvent modalEvent)
    {
      this._modalResult = UserInterfaceModalResult.Cancel;
      this._info = info;
      this._maintenanceInfo = maintenanceInfo;
      this._modalEvent = modalEvent;
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
      parentForm.ActiveControl = (Control) this.btnCancel;
      parentForm.ResumeLayout(false);
      parentForm.PerformLayout();
      parentForm.Visible = true;
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MaintenancePiece));
      this.lblHeader = new Label();
      this.lblSubHeader = new Label();
      this.pictureRestore = new PictureBox();
      this.pictureRemove = new PictureBox();
      this.radioRestore = new RadioButton();
      this.radioRemove = new RadioButton();
      this.groupRule = new GroupBox();
      this.groupDivider = new GroupBox();
      this.btnOk = new Button();
      this.btnCancel = new Button();
      this.btnHelp = new Button();
      this.topTableLayoutPanel = new TableLayoutPanel();
      this.pictureDesktop = new PictureBox();
      this.okCancelHelpTableLayoutPanel = new TableLayoutPanel();
      this.contentTableLayoutPanel = new TableLayoutPanel();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      ((ISupportInitialize) this.pictureRestore).BeginInit();
      ((ISupportInitialize) this.pictureRemove).BeginInit();
      this.topTableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.pictureDesktop).BeginInit();
      this.okCancelHelpTableLayoutPanel.SuspendLayout();
      this.contentTableLayoutPanel.SuspendLayout();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      this.lblHeader.AutoEllipsis = true;
      componentResourceManager.ApplyResources((object) this.lblHeader, "lblHeader");
      this.lblHeader.Margin = new Padding(10, 11, 3, 0);
      this.lblHeader.Name = "lblHeader";
      this.lblHeader.UseMnemonic = false;
      componentResourceManager.ApplyResources((object) this.lblSubHeader, "lblSubHeader");
      this.lblSubHeader.Margin = new Padding(29, 3, 3, 8);
      this.lblSubHeader.Name = "lblSubHeader";
      componentResourceManager.ApplyResources((object) this.pictureRestore, "pictureRestore");
      this.pictureRestore.Margin = new Padding(0, 0, 3, 0);
      this.pictureRestore.Name = "pictureRestore";
      this.pictureRestore.TabStop = false;
      componentResourceManager.ApplyResources((object) this.pictureRemove, "pictureRemove");
      this.pictureRemove.Margin = new Padding(0, 0, 3, 0);
      this.pictureRemove.Name = "pictureRemove";
      this.pictureRemove.TabStop = false;
      componentResourceManager.ApplyResources((object) this.radioRestore, "radioRestore");
      this.radioRestore.Margin = new Padding(3, 0, 0, 0);
      this.radioRestore.Name = "radioRestore";
      this.radioRestore.CheckedChanged += new EventHandler(this.radioRestore_CheckedChanged);
      componentResourceManager.ApplyResources((object) this.radioRemove, "radioRemove");
      this.radioRemove.Margin = new Padding(3, 0, 0, 0);
      this.radioRemove.Name = "radioRemove";
      this.radioRemove.CheckedChanged += new EventHandler(this.radioRemove_CheckedChanged);
      componentResourceManager.ApplyResources((object) this.groupRule, "groupRule");
      this.groupRule.Margin = new Padding(0);
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
      this.btnOk.Margin = new Padding(0, 0, 4, 0);
      this.btnOk.MinimumSize = new Size(75, 23);
      this.btnOk.Name = "btnOk";
      this.btnOk.Padding = new Padding(10, 0, 10, 0);
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      componentResourceManager.ApplyResources((object) this.btnCancel, "btnCancel");
      this.btnCancel.Margin = new Padding(2, 0, 2, 0);
      this.btnCancel.MinimumSize = new Size(75, 23);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Padding = new Padding(10, 0, 10, 0);
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      componentResourceManager.ApplyResources((object) this.btnHelp, "btnHelp");
      this.btnHelp.Margin = new Padding(4, 0, 0, 0);
      this.btnHelp.MinimumSize = new Size(75, 23);
      this.btnHelp.Name = "btnHelp";
      this.btnHelp.Padding = new Padding(10, 0, 10, 0);
      this.btnHelp.Click += new EventHandler(this.btnHelp_Click);
      componentResourceManager.ApplyResources((object) this.topTableLayoutPanel, "topTableLayoutPanel");
      this.topTableLayoutPanel.BackColor = SystemColors.Window;
      this.topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 87.2f));
      this.topTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.8f));
      this.topTableLayoutPanel.Controls.Add((Control) this.pictureDesktop, 1, 0);
      this.topTableLayoutPanel.Controls.Add((Control) this.lblHeader, 0, 0);
      this.topTableLayoutPanel.Controls.Add((Control) this.lblSubHeader, 0, 1);
      this.topTableLayoutPanel.Margin = new Padding(0);
      this.topTableLayoutPanel.Name = "topTableLayoutPanel";
      this.topTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.topTableLayoutPanel.RowStyles.Add(new RowStyle());
      componentResourceManager.ApplyResources((object) this.pictureDesktop, "pictureDesktop");
      this.pictureDesktop.Margin = new Padding(3, 0, 0, 0);
      this.pictureDesktop.Name = "pictureDesktop";
      this.topTableLayoutPanel.SetRowSpan((Control) this.pictureDesktop, 2);
      this.pictureDesktop.TabStop = false;
      componentResourceManager.ApplyResources((object) this.okCancelHelpTableLayoutPanel, "okCancelHelpTableLayoutPanel");
      this.okCancelHelpTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.okCancelHelpTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.okCancelHelpTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.okCancelHelpTableLayoutPanel.Controls.Add((Control) this.btnOk, 0, 0);
      this.okCancelHelpTableLayoutPanel.Controls.Add((Control) this.btnCancel, 1, 0);
      this.okCancelHelpTableLayoutPanel.Controls.Add((Control) this.btnHelp, 2, 0);
      this.okCancelHelpTableLayoutPanel.Margin = new Padding(0, 9, 8, 8);
      this.okCancelHelpTableLayoutPanel.Name = "okCancelHelpTableLayoutPanel";
      this.okCancelHelpTableLayoutPanel.RowStyles.Add(new RowStyle());
      componentResourceManager.ApplyResources((object) this.contentTableLayoutPanel, "contentTableLayoutPanel");
      this.contentTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.contentTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.contentTableLayoutPanel.Controls.Add((Control) this.pictureRestore, 0, 0);
      this.contentTableLayoutPanel.Controls.Add((Control) this.pictureRemove, 0, 1);
      this.contentTableLayoutPanel.Controls.Add((Control) this.radioRemove, 1, 1);
      this.contentTableLayoutPanel.Controls.Add((Control) this.radioRestore, 1, 0);
      this.contentTableLayoutPanel.Margin = new Padding(20, 22, 12, 22);
      this.contentTableLayoutPanel.Name = "contentTableLayoutPanel";
      this.contentTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.contentTableLayoutPanel.RowStyles.Add(new RowStyle());
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.topTableLayoutPanel, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.okCancelHelpTableLayoutPanel, 0, 4);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.contentTableLayoutPanel, 0, 2);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.groupDivider, 0, 3);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.groupRule, 0, 1);
      this.overarchingTableLayoutPanel.Margin = new Padding(0);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      componentResourceManager.ApplyResources((object) this, "$this");
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.Name = "MaintenancePiece";
      ((ISupportInitialize) this.pictureRestore).EndInit();
      ((ISupportInitialize) this.pictureRemove).EndInit();
      this.topTableLayoutPanel.ResumeLayout(false);
      this.topTableLayoutPanel.PerformLayout();
      ((ISupportInitialize) this.pictureDesktop).EndInit();
      this.okCancelHelpTableLayoutPanel.ResumeLayout(false);
      this.okCancelHelpTableLayoutPanel.PerformLayout();
      this.contentTableLayoutPanel.ResumeLayout(false);
      this.contentTableLayoutPanel.PerformLayout();
      this.overarchingTableLayoutPanel.ResumeLayout(false);
      this.overarchingTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void InitializeContent()
    {
      this.pictureDesktop.Image = Resources.GetImage("setup.bmp");
      this.pictureRestore.Enabled = (uint) (this._maintenanceInfo.maintenanceFlags & MaintenanceFlags.RestorationPossible) > 0U;
      Bitmap image1 = (Bitmap) Resources.GetImage("restore.bmp");
      image1.MakeTransparent();
      this.pictureRestore.Image = (Image) image1;
      Bitmap image2 = (Bitmap) Resources.GetImage("remove.bmp");
      image2.MakeTransparent();
      this.pictureRemove.Image = (Image) image2;
      this.lblHeader.Text = this._info.productName;
      this.radioRestore.Checked = (uint) (this._maintenanceInfo.maintenanceFlags & MaintenanceFlags.RestorationPossible) > 0U;
      this.radioRestore.Enabled = (uint) (this._maintenanceInfo.maintenanceFlags & MaintenanceFlags.RestorationPossible) > 0U;
      this.radioRemove.Checked = (this._maintenanceInfo.maintenanceFlags & MaintenanceFlags.RestorationPossible) == MaintenanceFlags.ClearFlag;
      this.btnHelp.Enabled = UserInterface.IsValidHttpUrl(this._info.supportUrl);
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this._modalResult = UserInterfaceModalResult.Ok;
      this._modalEvent.Set();
      this.Enabled = false;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this._modalResult = UserInterfaceModalResult.Cancel;
      this._modalEvent.Set();
      this.Enabled = false;
    }

    private void btnHelp_Click(object sender, EventArgs e)
    {
      if (!UserInterface.IsValidHttpUrl(this._info.supportUrl))
        return;
      UserInterface.LaunchUrlInBrowser(this._info.supportUrl);
    }

    private void radioRestore_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioRestore.Checked)
        this._maintenanceInfo.maintenanceFlags |= MaintenanceFlags.RestoreSelected;
      else
        this._maintenanceInfo.maintenanceFlags &= ~MaintenanceFlags.RestoreSelected;
    }

    private void radioRemove_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioRemove.Checked)
        this._maintenanceInfo.maintenanceFlags |= MaintenanceFlags.RemoveSelected;
      else
        this._maintenanceInfo.maintenanceFlags &= ~MaintenanceFlags.RemoveSelected;
    }
  }
}
