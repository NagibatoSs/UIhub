using SkiaSharp;

namespace UIhub.Analyze
{
    public class ImageDrawingService
    {
        public string DrawViolations(
            string originalImagePath,
            List<(int x, int y, int w, int h)> boxes,
            string outputPath)
        {
            using var original = SKBitmap.Decode(originalImagePath);
            using var surface = SKSurface.Create(new SKImageInfo(original.Width, original.Height));
            var canvas = surface.Canvas;

            // Рисуем оригинальное изображение
            canvas.DrawBitmap(original, 0, 0);

            // Рисуем рамки
            using var paint = new SKPaint
            {
                Color = SKColors.Red,
                StrokeWidth = 3,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            foreach (var (x, y, w, h) in boxes)
            {
                canvas.DrawRect(x, y, w, h, paint);
            }

            // Сохраняем
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(outputPath);
            data.SaveTo(stream);

            return outputPath;
        }
    }
}
