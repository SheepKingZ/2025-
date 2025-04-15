using System;
using System.IO;
using System.Windows.Forms;

public class FileHelper
{
    // 读取文件
    public bool ReadFile(DataCenter dataCenter, string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("文件不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            // 清空数据
            dataCenter.ClearData();
            
            // 读取文件内容
            string[] lines = File.ReadAllLines(filePath);
            
            if (lines.Length < 2)
            {
                MessageBox.Show("文件格式不正确！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            // 解析经纬度
            string[] coordinates = lines[0].Split(',');
            if (coordinates.Length >= 2)
            {
                if (double.TryParse(coordinates[0], out double longitude))
                {
                    dataCenter.Longitude = longitude;
                }
                
                if (double.TryParse(coordinates[1], out double latitude))
                {
                    dataCenter.Latitude = latitude;
                }
            }
            
            // 解析比例尺
            if (lines.Length > 1 && int.TryParse(lines[1], out int scale))
            {
                dataCenter.SelectedScale = scale;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"读取文件时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
    
    // 保存计算报告
    public bool SaveFile(DataCenter dataCenter, string filePath)
    {
        try
        {
            // 创建报告内容
            string report = "地形图图幅编号计算报告\r\n";
            report += "======================\r\n\r\n";
            
            // 添加基本信息
            report += $"比例尺: {dataCenter.Scales[dataCenter.SelectedScale]}\r\n";
            report += $"经度: {dataCenter.Longitude:F4}\r\n";
            report += $"纬度: {dataCenter.Latitude:F4}\r\n";
            report += $"传统编号: {dataCenter.TraditionalCode}\r\n";
            report += $"新编号: {dataCenter.NewCode}\r\n\r\n";
            
            // 添加图廓点信息
            report += "图廓点坐标:\r\n";
            report += $"北纬: {dataCenter.NorthLatitude:F4}\r\n";
            report += $"南纬: {dataCenter.SouthLatitude:F4}\r\n";
            report += $"东经: {dataCenter.EastLongitude:F4}\r\n";
            report += $"西经: {dataCenter.WestLongitude:F4}\r\n\r\n";
            
            // 添加相邻图幅信息
            report += "相邻图幅编号(传统编号):\r\n";
            report += $"西北: {dataCenter.AdjacentMapsTraditional["NW"]}\r\n";
            report += $"北: {dataCenter.AdjacentMapsTraditional["N"]}\r\n";
            report += $"东北: {dataCenter.AdjacentMapsTraditional["NE"]}\r\n";
            report += $"西: {dataCenter.AdjacentMapsTraditional["W"]}\r\n";
            report += $"东: {dataCenter.AdjacentMapsTraditional["E"]}\r\n";
            report += $"西南: {dataCenter.AdjacentMapsTraditional["SW"]}\r\n";
            report += $"南: {dataCenter.AdjacentMapsTraditional["S"]}\r\n";
            report += $"东南: {dataCenter.AdjacentMapsTraditional["SE"]}\r\n\r\n";
            
            report += "相邻图幅编号(新编号):\r\n";
            report += $"西北: {dataCenter.AdjacentMapsNew["NW"]}\r\n";
            report += $"北: {dataCenter.AdjacentMapsNew["N"]}\r\n";
            report += $"东北: {dataCenter.AdjacentMapsNew["NE"]}\r\n";
            report += $"西: {dataCenter.AdjacentMapsNew["W"]}\r\n";
            report += $"东: {dataCenter.AdjacentMapsNew["E"]}\r\n";
            report += $"西南: {dataCenter.AdjacentMapsNew["SW"]}\r\n";
            report += $"南: {dataCenter.AdjacentMapsNew["S"]}\r\n";
            report += $"东南: {dataCenter.AdjacentMapsNew["SE"]}\r\n";
            
            // 写入文件
            File.WriteAllText(filePath, report);
            
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存文件时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}