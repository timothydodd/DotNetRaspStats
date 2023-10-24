using Sang.IoT.SSD1306;
using SkiaSharp;

using (var oled = new SSD1306_128_64(1))
{

    oled.Begin();
    oled.Clear();

    using (var bitmap = new SKBitmap(128, 64, true))
    {
        SKCanvas canvas = new(bitmap);
        SKPaint paint = new()
        {
            Color = new SKColor(255, 255, 255),
            StrokeWidth = 1, //画笔宽度
            Typeface = SKTypeface.FromFile("assets/RobotoRegular-3m4L.ttf"),
            TextSize = 13,  //字体大小
            Style = SKPaintStyle.Fill,
        };
        canvas.DrawText("test: this ", 0, 13, paint);
        paint.TextSize = 30;
        canvas.DrawText("Another Time Test ", 0, 50, paint);
        oled.Image(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray());
    }

    oled.Display();
}
