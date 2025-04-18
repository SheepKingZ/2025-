using System;
using System.Windows.Forms;

public partial class Form1 : Form
{
    private System.ComponentModel.IContainer components = null;
    
    // UI控件
    private Label lblTitle;
    private GroupBox gbCalculateType;
    private RadioButton rbForward;
    private RadioButton rbBackward;
    private GroupBox gbInput;
    private Label lblLongitude;
    private TextBox txtLongitude;
    private Label lblLatitude;
    private TextBox txtLatitude;
    private Label lblScale;
    private ComboBox cmbScale;
    private GroupBox gbCodeType;
    private RadioButton rbTraditional;
    private RadioButton rbNew;
    private GroupBox gbResult;
    private Label lblTraditionalCode;
    private TextBox txtTraditionalCode;
    private Label lblNewCode;
    private TextBox txtNewCode;
    private GroupBox gbReport;
    private TextBox txtReport;
    private Button btnCalculate;
    private Button btnImport;
    private Button btnExport;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
    
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.Text = "地形图图幅编号计算";
        this.Size = new System.Drawing.Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = true;
        
        // 标题
        this.lblTitle = new Label();
        this.lblTitle.Text = "地形图图幅编号计算";
        this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        this.lblTitle.Dock = DockStyle.Top;
        this.lblTitle.Height = 50;
        
        // 计算类型分组
        this.gbCalculateType = new GroupBox();
        this.gbCalculateType.Text = "计算类型";
        this.gbCalculateType.Location = new System.Drawing.Point(20, 60);
        this.gbCalculateType.Size = new System.Drawing.Size(200, 80);
        
        this.rbForward = new RadioButton();
        this.rbForward.Text = "正向计算（经纬度→编号）";
        this.rbForward.Location = new System.Drawing.Point(20, 20);
        this.rbForward.Size = new System.Drawing.Size(180, 20);
        this.rbForward.Checked = true;
        this.rbForward.CheckedChanged += new EventHandler(this.rbForward_CheckedChanged);
        
        this.rbBackward = new RadioButton();
        this.rbBackward.Text = "反向计算（编号→经纬度）";
        this.rbBackward.Location = new System.Drawing.Point(20, 50);
        this.rbBackward.Size = new System.Drawing.Size(180, 20);
        this.rbBackward.CheckedChanged += new EventHandler(this.rbBackward_CheckedChanged);
        
        this.gbCalculateType.Controls.Add(this.rbForward);
        this.gbCalculateType.Controls.Add(this.rbBackward);
        
        // 编号类型分组
        this.gbCodeType = new GroupBox();
        this.gbCodeType.Text = "编号类型";
        this.gbCodeType.Location = new System.Drawing.Point(240, 60);
        this.gbCodeType.Size = new System.Drawing.Size(200, 80);
        
        this.rbTraditional = new RadioButton();
        this.rbTraditional.Text = "传统编号";
        this.rbTraditional.Location = new System.Drawing.Point(20, 20);
        this.rbTraditional.Size = new System.Drawing.Size(180, 20);
        this.rbTraditional.Checked = true;
        this.rbTraditional.Enabled = false;
        this.rbTraditional.CheckedChanged += new EventHandler(this.rbTraditional_CheckedChanged);
        
        this.rbNew = new RadioButton();
        this.rbNew.Text = "新编号";
        this.rbNew.Location = new System.Drawing.Point(20, 50);
        this.rbNew.Size = new System.Drawing.Size(180, 20);
        this.rbNew.Enabled = false;
        this.rbNew.CheckedChanged += new EventHandler(this.rbNew_CheckedChanged);
        
        this.gbCodeType.Controls.Add(this.rbTraditional);
        this.gbCodeType.Controls.Add(this.rbNew);
        
        // 比例尺选择
        this.lblScale = new Label();
        this.lblScale.Text = "比例尺:";
        this.lblScale.Location = new System.Drawing.Point(460, 80);
        this.lblScale.Size = new System.Drawing.Size(60, 20);
        
        this.cmbScale = new ComboBox();
        this.cmbScale.Location = new System.Drawing.Point(520, 80);
        this.cmbScale.Size = new System.Drawing.Size(120, 20);
        this.cmbScale.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbScale.SelectedIndexChanged += new EventHandler(this.cmbScale_SelectedIndexChanged);
        
        // 输入分组
        this.gbInput = new GroupBox();
        this.gbInput.Text = "输入";
        this.gbInput.Location = new System.Drawing.Point(20, 150);
        this.gbInput.Size = new System.Drawing.Size(360, 120);
        
        this.lblLongitude = new Label();
        this.lblLongitude.Text = "经度 (DD.MMSS):";
        this.lblLongitude.Location = new System.Drawing.Point(20, 30);
        this.lblLongitude.Size = new System.Drawing.Size(120, 20);
        
        this.txtLongitude = new TextBox();
        this.txtLongitude.Location = new System.Drawing.Point(140, 30);
        this.txtLongitude.Size = new System.Drawing.Size(200, 20);
        
        this.lblLatitude = new Label();
        this.lblLatitude.Text = "纬度 (DD.MMSS):";
        this.lblLatitude.Location = new System.Drawing.Point(20, 60);
        this.lblLatitude.Size = new System.Drawing.Size(120, 20);
        
        this.txtLatitude = new TextBox();
        this.txtLatitude.Location = new System.Drawing.Point(140, 60);
        this.txtLatitude.Size = new System.Drawing.Size(200, 20);
        
        this.lblTraditionalCode = new Label();
        this.lblTraditionalCode.Text = "传统编号:";
        this.lblTraditionalCode.Location = new System.Drawing.Point(20, 90);
        this.lblTraditionalCode.Size = new System.Drawing.Size(120, 20);
        
        this.txtTraditionalCode = new TextBox();
        this.txtTraditionalCode.Location = new System.Drawing.Point(140, 90);
        this.txtTraditionalCode.Size = new System.Drawing.Size(200, 20);
        this.txtTraditionalCode.Enabled = false;
        
        this.gbInput.Controls.Add(this.lblLongitude);
        this.gbInput.Controls.Add(this.txtLongitude);
        this.gbInput.Controls.Add(this.lblLatitude);
        this.gbInput.Controls.Add(this.txtLatitude);
        this.gbInput.Controls.Add(this.lblTraditionalCode);
        this.gbInput.Controls.Add(this.txtTraditionalCode);
        
        // 结果分组
        this.gbResult = new GroupBox();
        this.gbResult.Text = "结果";
        this.gbResult.Location = new System.Drawing.Point(400, 150);
        this.gbResult.Size = new System.Drawing.Size(360, 120);
        
        this.lblNewCode = new Label();
        this.lblNewCode.Text = "新编号:";
        this.lblNewCode.Location = new System.Drawing.Point(20, 30);
        this.lblNewCode.Size = new System.Drawing.Size(120, 20);
        
        this.txtNewCode = new TextBox();
        this.txtNewCode.Location = new System.Drawing.Point(140, 30);
        this.txtNewCode.Size = new System.Drawing.Size(200, 20);
        this.txtNewCode.Enabled = false;
        
        this.gbResult.Controls.Add(this.lblNewCode);
        this.gbResult.Controls.Add(this.txtNewCode);
        
        // 报告分组
        this.gbReport = new GroupBox();
        this.gbReport.Text = "计算报告";
        this.gbReport.Location = new System.Drawing.Point(20, 280);
        this.gbReport.Size = new System.Drawing.Size(740, 230);
        
        this.txtReport = new TextBox();
        this.txtReport.Multiline = true;
        this.txtReport.ScrollBars = ScrollBars.Vertical;
        this.txtReport.ReadOnly = true;
        this.txtReport.Location = new System.Drawing.Point(20, 20);
        this.txtReport.Size = new System.Drawing.Size(700, 190);
        
        this.gbReport.Controls.Add(this.txtReport);
        
        // 按钮
        this.btnCalculate = new Button();
        this.btnCalculate.Text = "计算";
        this.btnCalculate.Location = new System.Drawing.Point(240, 520);
        this.btnCalculate.Size = new System.Drawing.Size(100, 30);
        this.btnCalculate.Click += new EventHandler(this.btnCalculate_Click);
        
        this.btnImport = new Button();
        this.btnImport.Text = "导入数据";
        this.btnImport.Location = new System.Drawing.Point(350, 520);
        this.btnImport.Size = new System.Drawing.Size(100, 30);
        this.btnImport.Click += new EventHandler(this.btnImport_Click);
        
        this.btnExport = new Button();
        this.btnExport.Text = "导出报告";
        this.btnExport.Location = new System.Drawing.Point(460, 520);
        this.btnExport.Size = new System.Drawing.Size(100, 30);
        this.btnExport.Click += new EventHandler(this.btnExport_Click);
        
        // 状态栏
        this.statusStrip = new StatusStrip();
        this.lblStatus = new ToolStripStatusLabel();
        this.lblStatus.Text = "状态: 就绪";
        this.statusStrip.Items.Add(this.lblStatus);
        
        // 添加控件到窗体
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.gbCalculateType);
        this.Controls.Add(this.gbCodeType);
        this.Controls.Add(this.lblScale);
        this.Controls.Add(this.cmbScale);
        this.Controls.Add(this.gbInput);
        this.Controls.Add(this.gbResult);
        this.Controls.Add(this.gbReport);
        this.Controls.Add(this.btnCalculate);
        this.Controls.Add(this.btnImport);
        this.Controls.Add(this.btnExport);
        this.Controls.Add(this.statusStrip);
    }
}