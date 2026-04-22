namespace UIhub.Analyze
{
    public class AnalyzeResponse
    {
        public List<AnalyzedItem> Items { get; set; }
    }
    public class AnalyzedItem
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
    }

    public class Geometry
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public int Area { get; set; }
        public double AspectRatio { get; set; }
    }

    public class Visual
    {
        public List<double> MeanColorRgb { get; set; }
        public double MeanBrightness { get; set; }
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
        public List<AnalysisItem> Items { get; set; } = new();
    }
    public class AnalysisItem
    {
        public List<int> ElementIds { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
