namespace UIhub.Analyze
{
    //public class AnalyzeResponse
    //{
    //    public List<AnalyzedItem> Items { get; set; }
    //}
    public class AnalyzedItem //изображение
    {
        public string FileName { get; set; }
        public ImageInfo Image { get; set; }
        public List<UiElement> Elements { get; set; }
    }

    public class ImageInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class UiElement
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public double Confidence { get; set; }
        public BBox Bbox { get; set; }
        public Geometry Geometry { get; set; }
        public Visual Visual { get; set; }
        public OcrData Ocr { get; set; }
    }

    public class Geometry
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
    }

    public class Visual
    {
        public List<double> MeanColorRgb { get; set; }
        public List<double> BackgroundColorRgb { get; set; }
    }

    public class BBox
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public class AnalysisResult
    {
        public string AnalyzerName { get; set; } = string.Empty;
        public AnalysisMetric Metric { get; set; }
        public string Recomendation { get; set; } = string.Empty;
        public string StandardReference { get; set; } = string.Empty;
        public List<AnalysisItem> Items { get; set; } = new();
        public string Code { get; set; } = string.Empty;
    }
    public class AnalysisMetric
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Threshold { get; set; } = string.Empty;
        public bool IsOk { get; set; }
    }
    public class AnalysisItem // элемент
    {
        public List<int> ElementIds { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public string GroupLabel { get; set; } = string.Empty; 
    }
    public class OcrData
    {
        public string Text { get; set; } = string.Empty;
        public int FontSize { get; set; }
        public List<double> TextColorRgb { get; set; }
        public List<double> BackgroundColorRgb { get; set; }
    }
}
