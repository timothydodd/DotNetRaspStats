using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Sang.IoT.SSD1306;
using SkiaSharp;

internal class Program
{

    private static void Main(string[] args)
    {
        bool quitRequested = false;
        var source = new CancellationTokenSource();
        AppDomain.CurrentDomain.ProcessExit += delegate
        {
            quitRequested = true;
            source.Cancel();
        };
        Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Cancel Key Pressed");
            e.Cancel = true;
            quitRequested = true;
            source.Cancel();
        };
        Graph g = new Graph(17);
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
            var token = source.Token;

            while (!quitRequested)
            {
                var metrics = Metrics.GetUnixMetrics();
                var cpuUsage = Metrics.GetCpuMetrics();
                var totalMemory = Metrics.SizeSuffix((long)metrics.Total, 1);
                var usedMemory = Metrics.SizeSuffix((long)metrics.Used, 1);
                g.Update((byte)cpuUsage.CPU);
                stack.Add($"IP:{ip}");
                stack.Add($"CPU: {cpuUsage.CPU}%");
                stack.AddGraph(g);
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
                if (token.IsCancellationRequested)
                    break;
                token.WaitHandle.WaitOne(5000);

            }
            Console.WriteLine("Exiting");
            oled.Begin();
            oled.Clear();
            oled.Display();
        }


    }
}




