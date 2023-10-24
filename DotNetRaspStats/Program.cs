using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Sang.IoT.SSD1306;
using SkiaSharp;

internal class Program
{
    private static bool _quitRequested = false;
    private static void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += delegate
        {
            _quitRequested = true;
        };
        Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Cancel Key Pressed");
            e.Cancel = true;
            _quitRequested = true;
        };
        using (var oled = new SSD1306_128_64(1))
        {
            SKPaint paint = new()
            {
                Color = new SKColor(255, 255, 255),
                StrokeWidth = 1,
                Typeface = SKTypeface.FromFile("assets/RobotoRegular-3m4L.ttf"),
                TextSize = 13,
                Style = SKPaintStyle.Fill,
            };
            ScreenStack stack = new ScreenStack();

            var ip = Metrics.GetLocalIPAddress();
            while (!_quitRequested)
            {
                var metrics = Metrics.GetUnixMetrics();
                var cpuUsage = Metrics.GetCpuMetrics();
                var totalMemory = Metrics.SizeSuffix((long)metrics.Total, 1);
                var usedMemory = Metrics.SizeSuffix((long)metrics.Used, 1);
                stack.Add($"IP:{ip}");
                stack.Add($"CPU: {cpuUsage.CPU}%");
                stack.Add($"Mem: {usedMemory} / {totalMemory}");
                stack.Add($"Up: {cpuUsage.UpTime}");

                oled.Begin();
                oled.Clear();

                using SKBitmap bitmap = new SKBitmap(128, 64, true);

                using SKCanvas canvas = new(bitmap);

                stack.Draw(canvas, paint);

                oled.Image(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray());


                oled.Display();
                stack.Reset();
                Thread.Sleep(5000);
            }
            Console.WriteLine("Exiting");
            oled.Begin();
            oled.Clear();
            oled.Display();
        }


    }
}




