using System;
using System.Collections.Generic;

public class Calculate
{
    // 比例尺常量
    public const int SCALE_1M = 0;      // 1:100万
    public const int SCALE_500K = 1;    // 1:50万
    public const int SCALE_250K = 2;    // 1:25万
    public const int SCALE_100K = 3;    // 1:10万
    public const int SCALE_50K = 4;     // 1:5万
    public const int SCALE_25K = 5;     // 1:2.5万
    public const int SCALE_10K = 6;     // 1:1万
    
    // 经纬度格式转换：DD.MMSS -> 十进制度
    public double ConvertDMSToDegree(double dms)
    {
        int degrees = (int)dms;
        double minutes = (dms - degrees) * 100;
        int min = (int)minutes;
        double seconds = (minutes - min) * 100;
        
        return degrees + min / 60.0 + seconds / 3600.0;
    }
    
    // 十进制度 -> DD.MMSS
    public double ConvertDegreeToDMS(double degree)
    {
        int degrees = (int)degree;
        double minutesDecimal = (degree - degrees) * 60;
        int minutes = (int)minutesDecimal;
        double seconds = (minutesDecimal - minutes) * 60;
        
        // 四舍五入到整数秒
        seconds = Math.Round(seconds);
        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
            if (minutes == 60)
            {
                minutes = 0;
                degrees += 1;
            }
        }
        
        return degrees + minutes / 100.0 + seconds / 10000.0;
    }
    
    // 正向计算：根据经纬度计算图幅编号
    public void CalculateMapCode(DataCenter dataCenter)
    {
        // 转换为十进制度
        double lonDegree = ConvertDMSToDegree(dataCenter.Longitude);
        double latDegree = ConvertDMSToDegree(dataCenter.Latitude);
        
        // 根据选择的比例尺计算图幅编号
        switch (dataCenter.SelectedScale)
        {
            case SCALE_1M:
                Calculate1MMapCode(dataCenter, lonDegree, latDegree);
                break;
            case SCALE_500K:
                Calculate500KMapCode(dataCenter, lonDegree, latDegree);
                break;
            case SCALE_250K:
                Calculate250KMapCode(dataCenter, lonDegree, latDegree);
                break;
            case SCALE_100K:
                Calculate100KMapCode(dataCenter, lonDegree, latDegree);
                break;
            case SCALE_50K:
                Calculate50KMapCode(dataCenter, lonDegree, latDegree);
                break;
            case SCALE_25K:
                Calculate25KMapCode(dataCenter, lonDegree, latDegree);
                break;
            case SCALE_10K:
                Calculate10KMapCode(dataCenter, lonDegree, latDegree);
                break;
        }
        
        // 计算图廓点经纬度和相邻图幅
        CalculateMapBounds(dataCenter);
        CalculateAdjacentMaps(dataCenter);
        
        // 生成报告
        GenerateReport(dataCenter);
    }
    
    // 反向计算：根据图幅编号计算经纬度和相邻图幅
    public void CalculateFromMapCode(DataCenter dataCenter, bool isTraditional)
    {
        string code = isTraditional ? dataCenter.TraditionalCode : dataCenter.NewCode;
        
        // 根据编号长度判断比例尺
        if (isTraditional)
        {
            DetermineScaleFromTraditionalCode(dataCenter, code);
        }
        else
        {   
            DetermineScaleFromNewCode(dataCenter, code);
        }
        
        // 根据比例尺和编号计算中心点经纬度
        CalculateCenterFromCode(dataCenter, code, isTraditional);
        
        // 计算图廓点经纬度和相邻图幅
        CalculateMapBounds(dataCenter);
        CalculateAdjacentMaps(dataCenter);
        
        // 如果输入的是传统编号，计算新编号；反之亦然
        if (isTraditional)
        {
            ConvertTraditionalToNewCode(dataCenter);
        }
        else
        {
            ConvertNewToTraditionalCode(dataCenter);
        }
        
        // 生成报告
        GenerateReport(dataCenter);
    }
    
    // 1:100万图幅编号计算
    private void Calculate1MMapCode(DataCenter dataCenter, double lonDegree, double latDegree)
    {
        // 计算行号（纬度）
        int row = (int)((latDegree + 4) / 4);
        char rowChar = (char)('A' + row - 1);
        
        // 计算列号（经度）
        int col = (int)((lonDegree + 6) / 6);
        
        // 生成传统编号
        dataCenter.TraditionalCode = $"{rowChar}{col:D2}";
        
        // 生成新编号
        string hemisphere = lonDegree >= 0 ? "E" : "W";
        int lonAbs = (int)Math.Abs(lonDegree);
        dataCenter.NewCode = $"{rowChar}{lonAbs:D3}{hemisphere}";
    }
    
    // 1:50万图幅编号计算
    private void Calculate500KMapCode(DataCenter dataCenter, double lonDegree, double latDegree)
    {
        // 先计算1:100万图幅编号
        Calculate1MMapCode(dataCenter, lonDegree, latDegree);
        string baseCode = dataCenter.TraditionalCode;
        
        // 在1:100万图幅内细分
        int row = (int)((latDegree % 4) / 2);
        int col = (int)((lonDegree % 6) / 3);
        
        // 生成传统编号
        char subCode = (char)('1' + row * 2 + col);
        dataCenter.TraditionalCode = $"{baseCode}-{subCode}";
        
        // 生成新编号
        string baseNewCode = dataCenter.NewCode;
        dataCenter.NewCode = $"{baseNewCode}-{subCode}";
    }
    
    // 1:25万图幅编号计算
    private void Calculate250KMapCode(DataCenter dataCenter, double lonDegree, double latDegree)
    {
        // 先计算1:50万图幅编号
        Calculate500KMapCode(dataCenter, lonDegree, latDegree);
        string baseCode = dataCenter.TraditionalCode;
        
        // 在1:50万图幅内细分
        int row = (int)((latDegree % 2) / 1);
        int col = (int)((lonDegree % 3) / 1.5);
        
        // 生成传统编号
        char subCode = (char)('A' + row * 2 + col);
        dataCenter.TraditionalCode = $"{baseCode}{subCode}";
        
        // 生成新编号
        string baseNewCode = dataCenter.NewCode;
        dataCenter.NewCode = $"{baseNewCode}{subCode}";
    }
    
    // 1:10万图幅编号计算
    private void Calculate100KMapCode(DataCenter dataCenter, double lonDegree, double latDegree)
    {
        // 先计算1:100万图幅编号
        Calculate1MMapCode(dataCenter, lonDegree, latDegree);
        string baseCode = dataCenter.TraditionalCode;
        
        // 在1:100万图幅内细分
        int row = (int)((latDegree % 4) / (4.0 / 12));
        int col = (int)((lonDegree % 6) / (6.0 / 12));
        
        // 生成传统编号
        int subCode = row * 12 + col + 1;
        dataCenter.TraditionalCode = $"{baseCode}{subCode:D3}";
        
        // 生成新编号
        string baseNewCode = dataCenter.NewCode;
        dataCenter.NewCode = $"{baseNewCode}{subCode:D3}";
    }
    
    // 1:5万图幅编号计算
    private void Calculate50KMapCode(DataCenter dataCenter, double lonDegree, double latDegree)
    {
        // 先计算1:10万图幅编号
        Calculate100KMapCode(dataCenter, lonDegree, latDegree);
        string baseCode = dataCenter.TraditionalCode;
        
        // 在1:10万图幅内细分
        int row = (int)((latDegree % (4.0 / 12)) / (4.0 / 24));
        int col = (int)((lonDegree % (6.0 / 12)) / (6.0 / 24));
        
        // 生成传统编号
        char subCode = (char)('1' + row * 2 + col);
        dataCenter.TraditionalCode = $"{baseCode}{subCode}";
        
        // 生成新编号
        string baseNewCode = dataCenter.NewCode;
        dataCenter.NewCode = $"{baseNewCode}{subCode}";
    }
    
    // 1:2.5万图幅编号计算
    private void Calculate25KMapCode(DataCenter dataCenter, double lonDegree, double latDegree)
    {
        // 先计算1:5万图幅编号
        Calculate50KMapCode(dataCenter, lonDegree, latDegree);
        string baseCode = dataCenter.TraditionalCode;
        
        // 在1:5万图幅内细分
        int row = (int)((latDegree % (4.0 / 24)) / (4.0 / 48));
        int col = (int)((lonDegree % (6.0 / 24)) / (6.0 / 48));
        
        // 生成传统编号
        char subCode = (char)('A' + row * 2 + col);
        dataCenter.TraditionalCode = $"{baseCode}{subCode}";
        
        // 生成新编号
        string baseNewCode = dataCenter.NewCode;
        dataCenter.NewCode = $"{baseNewCode}{subCode}";
    }
    
    // 1:1万图幅编号计算
    private void Calculate10KMapCode(DataCenter dataCenter, double lonDegree, double latDegree)
    {
        // 先计算1:2.5万图幅编号
        Calculate25KMapCode(dataCenter, lonDegree, latDegree);
        string baseCode = dataCenter.TraditionalCode;
        
        // 在1:2.5万图幅内细分
        int row = (int)((latDegree % (4.0 / 48)) / (4.0 / 96));
        int col = (int)((lonDegree % (6.0 / 48)) / (6.0 / 96));
        
        // 生成传统编号
        int subCode = row * 2 + col + 1;
        dataCenter.TraditionalCode = $"{baseCode}({subCode})";
        
        // 生成新编号
        string baseNewCode = dataCenter.NewCode;
        dataCenter.NewCode = $"{baseNewCode}({subCode})";
    }
    
    // 根据传统编号判断比例尺
    private void DetermineScaleFromTraditionalCode(DataCenter dataCenter, string code)
    {
        if (code.Length == 3) // 例如：I49
        {
            dataCenter.SelectedScale = SCALE_1M;
        }
        else if (code.Contains("-") && code.Length == 5) // 例如：I49-1
        {
            dataCenter.SelectedScale = SCALE_500K;
        }
        else if (code.Length == 6 && char.IsLetter(code[code.Length - 1])) // 例如：I49-1A
        {
            dataCenter.SelectedScale = SCALE_250K;
        }
        else if (code.Length == 6 && char.IsDigit(code[code.Length - 1])) // 例如：I49001
        {
            dataCenter.SelectedScale = SCALE_100K;
        }
        else if (code.Length == 7 && char.IsDigit(code[code.Length - 3])) // 例如：I490011
        {
            dataCenter.SelectedScale = SCALE_50K;
        }
        else if (code.Length == 8 && char.IsLetter(code[code.Length - 1])) // 例如：I490011A
        {
            dataCenter.SelectedScale = SCALE_25K;
        }
        else if (code.Contains("(") && code.Contains(")")) // 例如：I490011A(1)
        {
            dataCenter.SelectedScale = SCALE_10K;
        }
    }
    
    // 根据新编号判断比例尺
    private void DetermineScaleFromNewCode(DataCenter dataCenter, string code)
    {
        // 新编号格式类似：I149E-1A(1)
        if (code.Contains("E") || code.Contains("W"))
        {
            if (!code.Contains("-") && !code.Contains("(")) // 例如：I149E
            {
                dataCenter.SelectedScale = SCALE_1M;
            }
            else if (code.Contains("-") && code.Length <= 8) // 例如：I149E-1
            {
                dataCenter.SelectedScale = SCALE_500K;
            }
            else if (code.Length == 9 && char.IsLetter(code[code.Length - 1])) // 例如：I149E-1A
            {
                dataCenter.SelectedScale = SCALE_250K;
            }
            else if (code.Contains("003") || code.Contains("006") || code.Contains("009")) // 例如：I149E003
            {
                dataCenter.SelectedScale = SCALE_100K;
            }
            else if (code.Length >= 10 && char.IsDigit(code[code.Length - 1]) && !code.Contains("(")) // 例如：I149E0031
            {
                dataCenter.SelectedScale = SCALE_50K;
            }
            else if (code.Length >= 11 && char.IsLetter(code[code.Length - 1]) && !code.Contains("(")) // 例如：I149E0031A
            {
                dataCenter.SelectedScale = SCALE_25K;
            }
            else if (code.Contains("(") && code.Contains(")")) // 例如：I149E0031A(1)
            {
                dataCenter.SelectedScale = SCALE_10K;
            }
        }
    }
    
    // 根据编号计算中心点经纬度
    private void CalculateCenterFromCode(DataCenter dataCenter, string code, bool isTraditional)
    {
        // 根据不同比例尺和编号格式计算中心点经纬度
        switch (dataCenter.SelectedScale)
        {
            case SCALE_1M:
                Calculate1MCenter(dataCenter, code, isTraditional);
                break;
            case SCALE_500K:
                Calculate500KCenter(dataCenter, code, isTraditional);
                break;
            case SCALE_250K:
                Calculate250KCenter(dataCenter, code, isTraditional);
                break;
            case SCALE_100K:
                Calculate100KCenter(dataCenter, code, isTraditional);
                break;
            case SCALE_50K:
                Calculate50KCenter(dataCenter, code, isTraditional);
                break;
            case SCALE_25K:
                Calculate25KCenter(dataCenter, code, isTraditional);
                break;
            case SCALE_10K:
                Calculate10KCenter(dataCenter, code, isTraditional);
                break;
        }
    }
    
    // 计算1:100万图幅中心点
    private void Calculate1MCenter(DataCenter dataCenter, string code, bool isTraditional)
    {
        if (isTraditional)
        {
            // 传统编号格式：I49
            char rowChar = code[0];
            int row = rowChar - 'A' + 1;
            int col = int.Parse(code.Substring(1));
            
            // 计算中心点经纬度（十进制度）
            double latDegree = (row * 4) - 2;
            double lonDegree = (col * 6) - 3;
            
            // 转换为DD.MMSS格式
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
        else
        {
            // 新编号格式：I149E
            char rowChar = code[0];
            int row = rowChar - 'A' + 1;
            
            // 提取经度
            int lonIndex = code.IndexOf('E');
            if (lonIndex == -1) lonIndex = code.IndexOf('W');
            
            int lonDegree = int.Parse(code.Substring(1, lonIndex - 1));
            char hemisphere = code[lonIndex];
            if (hemisphere == 'W') lonDegree = -lonDegree;
            
            // 计算中心点经纬度（十进制度）
            double latDegree = (row * 4) - 2;
            double lonDegreeFinal = lonDegree + 3; // 中心点偏移
            
            // 转换为DD.MMSS格式
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegreeFinal);
        }
    }
    
    // 计算1:50万图幅中心点
    private void Calculate500KCenter(DataCenter dataCenter, string code, bool isTraditional)
    {
        // 先计算1:100万图幅中心点
        string baseCode;
        if (isTraditional)
        {
            baseCode = code.Split('-')[0];
            Calculate1MCenter(dataCenter, baseCode, true);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - '1';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * 2) - 1;
            double lonOffset = (col * 3) - 1.5;
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
        else
        {
            // 新编号格式：I149E-1
            baseCode = code.Split('-')[0];
            Calculate1MCenter(dataCenter, baseCode, false);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - '1';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * 2) - 1;
            double lonOffset = (col * 3) - 1.5;
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
    }
    
    // 计算1:25万图幅中心点
    private void Calculate250KCenter(DataCenter dataCenter, string code, bool isTraditional)
    {
        // 先计算1:50万图幅中心点
        string baseCode;
        if (isTraditional)
        {
            // 传统编号格式：I49-1A
            baseCode = code.Substring(0, code.Length - 1);
            Calculate500KCenter(dataCenter, baseCode, true);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - 'A';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * 1) - 0.5;
            double lonOffset = (col * 1.5) - 0.75;
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
        else
        {
            // 新编号格式：I149E-1A
            baseCode = code.Substring(0, code.Length - 1);
            Calculate500KCenter(dataCenter, baseCode, false);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - 'A';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * 1) - 0.5;
            double lonOffset = (col * 1.5) - 0.75;
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
    }
    
    // 计算1:10万图幅中心点
    private void Calculate100KCenter(DataCenter dataCenter, string code, bool isTraditional)
    {
        if (isTraditional)
        {
            // 传统编号格式：I49001
            string baseCode = code.Substring(0, 3);
            Calculate1MCenter(dataCenter, baseCode, true);
            
            // 提取子图号
            int subCode = int.Parse(code.Substring(3));
            int row = (subCode - 1) / 12;
            int col = (subCode - 1) % 12;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 12)) - (4.0 / 24);
            double lonOffset = (col * (6.0 / 12)) - (6.0 / 24);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
        else
        {
            // 新编号格式：I149E003
            int eIndex = code.IndexOf('E');
            int wIndex = code.IndexOf('W');
            int hemisphereIndex = eIndex != -1 ? eIndex : wIndex;
            
            string baseCode = code.Substring(0, hemisphereIndex + 1);
            Calculate1MCenter(dataCenter, baseCode, false);
            
            // 提取子图号
            int subCode = int.Parse(code.Substring(hemisphereIndex + 1));
            int row = (subCode - 1) / 12;
            int col = (subCode - 1) % 12;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 12)) - (4.0 / 24);
            double lonOffset = (col * (6.0 / 12)) - (6.0 / 24);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
    }
    
    // 计算1:5万图幅中心点
    private void Calculate50KCenter(DataCenter dataCenter, string code, bool isTraditional)
    {
        if (isTraditional)
        {
            // 传统编号格式：I490011
            string baseCode = code.Substring(0, code.Length - 1);
            Calculate100KCenter(dataCenter, baseCode, true);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - '1';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 24)) - (4.0 / 48);
            double lonOffset = (col * (6.0 / 24)) - (6.0 / 48);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
        else
        {
            // 新编号格式：I149E0031
            int eIndex = code.IndexOf('E');
            int wIndex = code.IndexOf('W');
            int hemisphereIndex = eIndex != -1 ? eIndex : wIndex;
            
            string baseCode = code.Substring(0, code.Length - 1);
            Calculate100KCenter(dataCenter, baseCode, false);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - '1';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 24)) - (4.0 / 48);
            double lonOffset = (col * (6.0 / 24)) - (6.0 / 48);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
    }
    
    // 计算1:2.5万图幅中心点
    private void Calculate25KCenter(DataCenter dataCenter, string code, bool isTraditional)
    {
        if (isTraditional)
        {
            // 传统编号格式：I490011A
            string baseCode = code.Substring(0, code.Length - 1);
            Calculate50KCenter(dataCenter, baseCode, true);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - 'A';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 48)) - (4.0 / 96);
            double lonOffset = (col * (6.0 / 48)) - (6.0 / 96);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
        else
        {
            // 新编号格式：I149E0031A
            string baseCode = code.Substring(0, code.Length - 1);
            Calculate50KCenter(dataCenter, baseCode, false);
            
            // 提取子图号
            char subCode = code[code.Length - 1];
            int subIndex = subCode - 'A';
            int row = subIndex / 2;
            int col = subIndex % 2;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 48)) - (4.0 / 96);
            double lonOffset = (col * (6.0 / 48)) - (6.0 / 96);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
    }
    
    // 计算1:1万图幅中心点
    private void Calculate10KCenter(DataCenter dataCenter, string code, bool isTraditional)
    {
        if (isTraditional)
        {
            // 传统编号格式：I490011A(1)
            int startIndex = code.IndexOf('(');
            int endIndex = code.IndexOf(')');
            string baseCode = code.Substring(0, startIndex);
            Calculate25KCenter(dataCenter, baseCode, true);
            
            // 提取子图号
            int subCode = int.Parse(code.Substring(startIndex + 1, endIndex - startIndex - 1));
            int row = (subCode - 1) / 2;
            int col = (subCode - 1) % 2;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 96)) - (4.0 / 192);
            double lonOffset = (col * (6.0 / 96)) - (6.0 / 192);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
        else
        {
            // 新编号格式：I149E0031A(1)
            int startIndex = code.IndexOf('(');
            int endIndex = code.IndexOf(')');
            string baseCode = code.Substring(0, startIndex);
            Calculate25KCenter(dataCenter, baseCode, false);
            
            // 提取子图号
            int subCode = int.Parse(code.Substring(startIndex + 1, endIndex - startIndex - 1));
            int row = (subCode - 1) / 2;
            int col = (subCode - 1) % 2;
            
            // 计算偏移量
            double latOffset = (row * (4.0 / 96)) - (4.0 / 192);
            double lonOffset = (col * (6.0 / 96)) - (6.0 / 192);
            
            // 调整中心点经纬度
            double latDegree = ConvertDMSToDegree(dataCenter.Latitude) + latOffset;
            double lonDegree = ConvertDMSToDegree(dataCenter.Longitude) + lonOffset;
            
            dataCenter.Latitude = ConvertDegreeToDMS(latDegree);
            dataCenter.Longitude = ConvertDegreeToDMS(lonDegree);
        }
    }
    
    // 计算图廓点经纬度
    private void CalculateMapBounds(DataCenter dataCenter)
    {
        double latDegree = ConvertDMSToDegree(dataCenter.Latitude);
        double lonDegree = ConvertDMSToDegree(dataCenter.Longitude);
        
        // 根据不同比例尺计算图廓范围
        double latRange = 0;
        double lonRange = 0;
        
        switch (dataCenter.SelectedScale)
        {
            case SCALE_1M:
                latRange = 4.0 / 2;
                lonRange = 6.0 / 2;
                break;
            case SCALE_500K:
                latRange = 2.0 / 2;
                lonRange = 3.0 / 2;
                break;
            case SCALE_250K:
                latRange = 1.0 / 2;
                lonRange = 1.5 / 2;
                break;
            case SCALE_100K:
                latRange = (4.0 / 12) / 2;
                lonRange = (6.0 / 12) / 2;
                break;
            case SCALE_50K:
                latRange = (4.0 / 24) / 2;
                lonRange = (6.0 / 24) / 2;
                break;
            case SCALE_25K:
                latRange = (4.0 / 48) / 2;
                lonRange = (6.0 / 48) / 2;
                break;
            case SCALE_10K:
                latRange = (4.0 / 96) / 2;
                lonRange = (6.0 / 96) / 2;
                break;
        }
        
        // 计算四角坐标（十进制度）
        double northLat = latDegree + latRange;
        double southLat = latDegree - latRange;
        double eastLon = lonDegree + lonRange;
        double westLon = lonDegree - lonRange;
        
        // 转换为DD.MMSS格式
        dataCenter.NorthLatitude = ConvertDegreeToDMS(northLat);
        dataCenter.SouthLatitude = ConvertDegreeToDMS(southLat);
        dataCenter.EastLongitude = ConvertDegreeToDMS(eastLon);
        dataCenter.WestLongitude = ConvertDegreeToDMS(westLon);
    }
    
    // 计算相邻图幅编号
    private void CalculateAdjacentMaps(DataCenter dataCenter)
    {
        // 获取当前图幅的中心点经纬度
        double latDegree = ConvertDMSToDegree(dataCenter.Latitude);
        double lonDegree = ConvertDMSToDegree(dataCenter.Longitude);
        
        // 根据不同比例尺计算相邻图幅的偏移量
        double latOffset = 0;
        double lonOffset = 0;
        
        switch (dataCenter.SelectedScale)
        {
            case SCALE_1M:
                latOffset = 4.0;
                lonOffset = 6.0;
                break;
            case SCALE_500K:
                latOffset = 2.0;
                lonOffset = 3.0;
                break;
            case SCALE_250K:
                latOffset = 1.0;
                lonOffset = 1.5;
                break;
            case SCALE_100K:
                latOffset = 4.0 / 12;
                lonOffset = 6.0 / 12;
                break;
            case SCALE_50K:
                latOffset = 4.0 / 24;
                lonOffset = 6.0 / 24;
                break;
            case SCALE_25K:
                latOffset = 4.0 / 48;
                lonOffset = 6.0 / 48;
                break;
            case SCALE_10K:
                latOffset = 4.0 / 96;
                lonOffset = 6.0 / 96;
                break;
        }
        
        // 计算8个相邻图幅的中心点经纬度
        double nwLat = latDegree + latOffset;
        double nwLon = lonDegree - lonOffset;
        
        double nLat = latDegree + latOffset;
        double nLon = lonDegree;
        
        double neLat = latDegree + latOffset;
        double neLon = lonDegree + lonOffset;
        
        double wLat = latDegree;
        double wLon = lonDegree - lonOffset;
        
        double eLat = latDegree;
        double eLon = lonDegree + lonOffset;
        
        double swLat = latDegree - latOffset;
        double swLon = lonDegree - lonOffset;
        
        double sLat = latDegree - latOffset;
        double sLon = lonDegree;
        
        double seLat = latDegree - latOffset;
        double seLon = lonDegree + lonOffset;
        
        // 临时保存当前图幅编号
        string currentTraditional = dataCenter.TraditionalCode;
        string currentNew = dataCenter.NewCode;
        
        // 计算西北图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(nwLat);
        dataCenter.Longitude = ConvertDegreeToDMS(nwLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["NW"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["NW"] = dataCenter.NewCode;
        
        // 计算北图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(nLat);
        dataCenter.Longitude = ConvertDegreeToDMS(nLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["N"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["N"] = dataCenter.NewCode;
        
        // 计算东北图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(neLat);
        dataCenter.Longitude = ConvertDegreeToDMS(neLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["NE"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["NE"] = dataCenter.NewCode;
        
        // 计算西图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(wLat);
        dataCenter.Longitude = ConvertDegreeToDMS(wLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["W"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["W"] = dataCenter.NewCode;
        
        // 计算东图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(eLat);
        dataCenter.Longitude = ConvertDegreeToDMS(eLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["E"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["E"] = dataCenter.NewCode;
        
        // 计算西南图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(swLat);
        dataCenter.Longitude = ConvertDegreeToDMS(swLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["SW"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["SW"] = dataCenter.NewCode;
        
        // 计算南图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(sLat);
        dataCenter.Longitude = ConvertDegreeToDMS(sLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["S"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["S"] = dataCenter.NewCode;
        
        // 计算东南图幅编号
        dataCenter.Latitude = ConvertDegreeToDMS(seLat);
        dataCenter.Longitude = ConvertDegreeToDMS(seLon);
        CalculateMapCode(dataCenter);
        dataCenter.AdjacentMapsTraditional["SE"] = dataCenter.TraditionalCode;
        dataCenter.AdjacentMapsNew["SE"] = dataCenter.NewCode;
        
        // 恢复当前图幅编号
        dataCenter.TraditionalCode = currentTraditional;
        dataCenter.NewCode = currentNew;
    }
    
    // 传统编号转新编号
    private void ConvertTraditionalToNewCode(DataCenter dataCenter)
    {
        // 根据传统编号计算新编号
        // 这里简化处理，直接根据经纬度重新计算
        string currentTraditional = dataCenter.TraditionalCode;
        CalculateMapCode(dataCenter);
        string newCode = dataCenter.NewCode;
        dataCenter.TraditionalCode = currentTraditional;
    }
    
    // 新编号转传统编号
    private void ConvertNewToTraditionalCode(DataCenter dataCenter)
    {
        // 根据新编号计算传统编号
        // 这里简化处理，直接根据经纬度重新计算
        string currentNew = dataCenter.NewCode;
        CalculateMapCode(dataCenter);
        string traditionalCode = dataCenter.TraditionalCode;
        dataCenter.NewCode = currentNew;
    }
    
    // 生成计算报告
    private void GenerateReport(DataCenter dataCenter)
    {
        dataCenter.Report.Clear();
        
        // 添加基本信息
        dataCenter.AddReport($"比例尺: {dataCenter.Scales[dataCenter.SelectedScale]}");
        dataCenter.AddReport($"经度: {dataCenter.Longitude:F4} ({FormatDMS(dataCenter.Longitude)})");
        dataCenter.AddReport($"纬度: {dataCenter.Latitude:F4} ({FormatDMS(dataCenter.Latitude)})");
        dataCenter.AddReport($"传统编号: {dataCenter.TraditionalCode}");
        dataCenter.AddReport($"新编号: {dataCenter.NewCode}");
        
        // 添加图廓点信息
        dataCenter.AddReport("\n图廓点坐标:");
        dataCenter.AddReport($"北纬: {dataCenter.NorthLatitude:F4} ({FormatDMS(dataCenter.NorthLatitude)})");
        dataCenter.AddReport($"南纬: {dataCenter.SouthLatitude:F4} ({FormatDMS(dataCenter.SouthLatitude)})");
        dataCenter.AddReport($"东经: {dataCenter.EastLongitude:F4} ({FormatDMS(dataCenter.EastLongitude)})");
        dataCenter.AddReport($"西经: {dataCenter.WestLongitude:F4} ({FormatDMS(dataCenter.WestLongitude)})");
        
        // 添加相邻图幅信息
        dataCenter.AddReport("\n相邻图幅编号(传统编号):");
        dataCenter.AddReport($"西北: {dataCenter.AdjacentMapsTraditional["NW"]}");
        dataCenter.AddReport($"北: {dataCenter.AdjacentMapsTraditional["N"]}");
        dataCenter.AddReport($"东北: {dataCenter.AdjacentMapsTraditional["NE"]}");
        dataCenter.AddReport($"西: {dataCenter.AdjacentMapsTraditional["W"]}");
        dataCenter.AddReport($"东: {dataCenter.AdjacentMapsTraditional["E"]}");
        dataCenter.AddReport($"西南: {dataCenter.AdjacentMapsTraditional["SW"]}");
        dataCenter.AddReport($"南: {dataCenter.AdjacentMapsTraditional["S"]}");
        dataCenter.AddReport($"东南: {dataCenter.AdjacentMapsTraditional["SE"]}");
        
        dataCenter.AddReport("\n相邻图幅编号(新编号):");
        dataCenter.AddReport($"西北: {dataCenter.AdjacentMapsNew["NW"]}");
        dataCenter.AddReport($"北: {dataCenter.AdjacentMapsNew["N"]}");
        dataCenter.AddReport($"东北: {dataCenter.AdjacentMapsNew["NE"]}");
        dataCenter.AddReport($"西: {dataCenter.AdjacentMapsNew["W"]}");
        dataCenter.AddReport($"东: {dataCenter.AdjacentMapsNew["E"]}");
        dataCenter.AddReport($"西南: {dataCenter.AdjacentMapsNew["SW"]}");
        dataCenter.AddReport($"南: {dataCenter.AdjacentMapsNew["S"]}");
        dataCenter.AddReport($"东南: {dataCenter.AdjacentMapsNew["SE"]}");
    }
    
    // 格式化经纬度为度分秒格式
    private string FormatDMS(double dms)
    {
        int degrees = (int)dms;
        double minutes = (dms - degrees) * 100;
        int min = (int)minutes;
        double seconds = (minutes - min) * 100;
        
        return $"{degrees}°{min:D2}′{seconds:F0}″";
    }