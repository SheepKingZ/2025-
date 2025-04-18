using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

public partial class Form1 : Form
{
    // 全局变量
    DataCenter dataCenter = new DataCenter();
    string reportContent = "";
    bool isProcessing = false;
    
    public Form1()
    {
        InitializeComponent();
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        // 初始化比例尺下拉框
        foreach (var scale in dataCenter.Scales)
        {
            cmbScale.Items.Add(scale.Value);
        }
        cmbScale.SelectedIndex = 4; // 默认选择1:5万
        dataCenter.SelectedScale = 4;
        
        // 初始化计算方式
        rbForward.Checked = true;
        
        // 初始化状态
        UpdateStatus("就绪");
    }
    
    private void UpdateStatus(string status)
    {
        lblStatus.Text = $"状态: {status}";
    }
    
    private void btnCalculate_Click(object sender, EventArgs e)
    {
        if (isProcessing)
        {
            MessageBox.Show("正在处理中，请稍候...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        isProcessing = true;
        UpdateStatus("计算中...");
        
        try
        {
            // 获取比例尺
            dataCenter.SelectedScale = cmbScale.SelectedIndex;
            
            // 正向计算：根据经纬度计算图幅编号
            if (rbForward.Checked)
            {
                // 获取经纬度
                if (!double.TryParse(txtLongitude.Text, out double longitude) ||
                    !double.TryParse(txtLatitude.Text, out double latitude))
                {
                    MessageBox.Show("请输入有效的经纬度！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isProcessing = false;
                    UpdateStatus("就绪");
                    return;
                }
                
                dataCenter.Longitude = longitude;
                dataCenter.Latitude = latitude;
                
                // 计算图幅编号
                Calculate calculator = new Calculate();
                calculator.CalculateMapCode(dataCenter);
                
                // 显示结果
                txtTraditionalCode.Text = dataCenter.TraditionalCode;
                txtNewCode.Text = dataCenter.NewCode;
                
                // 更新报告
                UpdateReport();
            }
            // 反向计算：根据图幅编号计算经纬度
            else
            {
                Calculate calculator = new Calculate();
                
                // 根据传统编号计算
                if (rbTraditional.Checked)
                {
                    if (string.IsNullOrEmpty(txtTraditionalCode.Text))
                    {
                        MessageBox.Show("请输入传统编号！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isProcessing = false;
                        UpdateStatus("就绪");
                        return;
                    }
                    
                    dataCenter.TraditionalCode = txtTraditionalCode.Text;
                    calculator.CalculateFromMapCode(dataCenter, true);
                    
                    // 显示结果
                    txtLongitude.Text = dataCenter.Longitude.ToString("F4");
                    txtLatitude.Text = dataCenter.Latitude.ToString("F4");
                    txtNewCode.Text = dataCenter.NewCode;
                }
                // 根据新编号计算
                else
                {
                    if (string.IsNullOrEmpty(txtNewCode.Text))
                    {
                        MessageBox.Show("请输入新编号！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isProcessing = false;
                        UpdateStatus("就绪");
                        return;
                    }
                    
                    dataCenter.NewCode = txtNewCode.Text;
                    calculator.CalculateFromMapCode(dataCenter, false);
                    
                    // 显示结果
                    txtLongitude.Text = dataCenter.Longitude.ToString("F4");
                    txtLatitude.Text = dataCenter.Latitude.ToString("F4");
                    txtTraditionalCode.Text = dataCenter.TraditionalCode;
                }
                
                // 更新报告
                UpdateReport();
            }
            
            UpdateStatus("计算完成");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"计算过程中发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateStatus("计算出错");
        }
        finally
        {
            isProcessing = false;
        }
    }
    
    private void UpdateReport()
    {
        // 清空报告内容
        txtReport.Clear();
        reportContent = "";
        
        // 添加报告内容
        foreach (var line in dataCenter.Report)
        {
            txtReport.AppendText(line + Environment.NewLine);
            reportContent += line + Environment.NewLine;
        }
    }
    
    private void btnImport_Click(object sender, EventArgs e)
    {
        if (isProcessing)
        {
            MessageBox.Show("正在处理中，请稍候...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        // 打开文件对话框
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
        openFileDialog.Title = "选择数据文件";
        
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            isProcessing = true;
            UpdateStatus("导入中...");
            
            try
            {
                // 读取文件
                FileHelper fileHelper = new FileHelper();
                if (fileHelper.ReadFile(dataCenter, openFileDialog.FileName))
                {
                    // 更新界面
                    txtLongitude.Text = dataCenter.Longitude.ToString("F4");
                    txtLatitude.Text = dataCenter.Latitude.ToString("F4");
                    cmbScale.SelectedIndex = dataCenter.SelectedScale;
                    
                    // 计算图幅编号
                    Calculate calculator = new Calculate();
                    calculator.CalculateMapCode(dataCenter);
                    
                    // 显示结果
                    txtTraditionalCode.Text = dataCenter.TraditionalCode;
                    txtNewCode.Text = dataCenter.NewCode;
                    
                    // 更新报告
                    UpdateReport();
                    
                    UpdateStatus("导入完成");
                    MessageBox.Show("数据导入成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UpdateStatus("导入失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入过程中发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("导入出错");
            }
            finally
            {
                isProcessing = false;
            }
        }
    }
    
    private void btnExport_Click(object sender, EventArgs e)
    {
        if (isProcessing)
        {
            MessageBox.Show("正在处理中，请稍候...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        if (string.IsNullOrEmpty(reportContent))
        {
            MessageBox.Show("没有可导出的报告内容！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        // 保存文件对话框
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
        saveFileDialog.Title = "保存计算报告";
        saveFileDialog.FileName = $"地形图图幅编号计算报告_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            isProcessing = true;
            UpdateStatus("导出中...");
            
            try
            {
                // 保存文件
                FileHelper fileHelper = new FileHelper();
                if (fileHelper.SaveFile(dataCenter, saveFileDialog.FileName))
                {
                    UpdateStatus("导出完成");
                    MessageBox.Show("报告导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UpdateStatus("导出失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出过程中发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("导出出错");
            }
            finally
            {
                isProcessing = false;
            }
        }
    }
    
    private void rbForward_CheckedChanged(object sender, EventArgs e)
    {
        // 正向计算：启用经纬度输入，禁用编号输入
        txtLongitude.Enabled = rbForward.Checked;
        txtLatitude.Enabled = rbForward.Checked;
        txtTraditionalCode.Enabled = !rbForward.Checked;
        txtNewCode.Enabled = !rbForward.Checked;
        
        // 启用/禁用编号类型选择
        rbTraditional.Enabled = !rbForward.Checked;
        rbNew.Enabled = !rbForward.Checked;
        
        if (rbForward.Checked)
        {
            // 清空编号输入框
            txtTraditionalCode.Clear();
            txtNewCode.Clear();
        }
        else
        {
            // 根据编号类型启用对应的输入框
            txtTraditionalCode.Enabled = rbTraditional.Checked;
            txtNewCode.Enabled = rbNew.Checked;
        }
    }
    
    private void rbBackward_CheckedChanged(object sender, EventArgs e)
    {
        // 反向计算：禁用经纬度输入，启用编号输入
        txtLongitude.Enabled = !rbBackward.Checked;
        txtLatitude.Enabled = !rbBackward.Checked;
        
        // 启用/禁用编号类型选择
        rbTraditional.Enabled = rbBackward.Checked;
        rbNew.Enabled = rbBackward.Checked;
        
        if (rbBackward.Checked)
        {
            // 根据编号类型启用对应的输入框
            txtTraditionalCode.Enabled = rbTraditional.Checked;
            txtNewCode.Enabled = rbNew.Checked;
            
            // 清空经纬度输入框
            txtLongitude.Clear();
            txtLatitude.Clear();
        }
        else
        {
            // 清空编号输入框
            txtTraditionalCode.Clear();
            txtNewCode.Clear();
        }
    }
    
    private void rbTraditional_CheckedChanged(object sender, EventArgs e)
    {
        // 根据编号类型启用对应的输入框
        if (rbBackward.Checked)
        {
            txtTraditionalCode.Enabled = rbTraditional.Checked;
            txtNewCode.Enabled = !rbTraditional.Checked;
            
            if (rbTraditional.Checked)
            {
                txtNewCode.Clear();
            }
        }
    }
    
    private void rbNew_CheckedChanged(object sender, EventArgs e)
    {
        // 根据编号类型启用对应的输入框
        if (rbBackward.Checked)
        {
            txtTraditionalCode.Enabled = !rbNew.Checked;
            txtNewCode.Enabled = rbNew.Checked;
            
            if (rbNew.Checked)
            {
                txtTraditionalCode.Clear();
            }
        }
    }
    
    private void cmbScale_SelectedIndexChanged(object sender, EventArgs e)
    {
        // 更新选择的比例尺
        dataCenter.SelectedScale = cmbScale.SelectedIndex;
    }
}