using System;
using System.Collections.Generic;

public class DataCenter
{
    // 存储经纬度信息
    public double Longitude { get; set; } // 经度，格式为DD.MMSS
    public double Latitude { get; set; }  // 纬度，格式为DD.MMSS
    
    // 存储图幅编号信息
    public string TraditionalCode { get; set; } // 传统编号
    public string NewCode { get; set; }        // 新编号
    
    // 当前选择的比例尺
    public int SelectedScale { get; set; }
    
    // 比例尺列表
    public Dictionary<int, string> Scales { get; set; }
    
    // 图廓点经纬度
    public double NorthLatitude { get; set; }  // 北纬
    public double SouthLatitude { get; set; }  // 南纬
    public double EastLongitude { get; set; }  // 东经
    public double WestLongitude { get; set; }  // 西经
    
    // 相邻图幅编号
    public Dictionary<string, string> AdjacentMapsTraditional { get; set; } // 传统编号
    public Dictionary<string, string> AdjacentMapsNew { get; set; }        // 新编号
    
    // 计算报告
    public List<string> Report { get; set; }
    
    public DataCenter()
    {
        // 初始化比例尺字典
        Scales = new Dictionary<int, string>
        {
            { 0, "1:100万" },
            { 1, "1:50万" },
            { 2, "1:25万" },
            { 3, "1:10万" },
            { 4, "1:5万" },
            { 5, "1:2.5万" },
            { 6, "1:1万" }
        };
        
        // 初始化相邻图幅字典
        AdjacentMapsTraditional = new Dictionary<string, string>
        {
            { "NW", "" }, // 西北
            { "N", "" },  // 北
            { "NE", "" }, // 东北
            { "W", "" },  // 西
            { "E", "" },  // 东
            { "SW", "" }, // 西南
            { "S", "" },  // 南
            { "SE", "" }  // 东南
        };
        
        AdjacentMapsNew = new Dictionary<string, string>
        {
            { "NW", "" }, // 西北
            { "N", "" },  // 北
            { "NE", "" }, // 东北
            { "W", "" },  // 西
            { "E", "" },  // 东
            { "SW", "" }, // 西南
            { "S", "" },  // 南
            { "SE", "" }  // 东南
        };
        
        // 初始化报告列表
        Report = new List<string>();
    }
    
    // 清空数据
    public void ClearData()
    {
        Longitude = 0;
        Latitude = 0;
        TraditionalCode = "";
        NewCode = "";
        NorthLatitude = 0;
        SouthLatitude = 0;
        EastLongitude = 0;
        WestLongitude = 0;
        
        foreach (var key in AdjacentMapsTraditional.Keys)
        {
            AdjacentMapsTraditional[key] = "";
            AdjacentMapsNew[key] = "";
        }
        
        Report.Clear();
    }
    
    // 添加报告内容
    public void AddReport(string content)
    {
        Report.Add(content);
    }
}