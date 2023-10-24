using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
public static class Metrics
{

    private static long totalMemoryInKb;


    public static UptimeMetrics GetCpuMetrics()
    {
        string output = "";
        var info = new ProcessStartInfo("uptime")
        {
            RedirectStandardOutput = true
        };
        using (var process = Process.Start(info))
        {
            output = process.StandardOutput.ReadToEnd();
            //Console.WriteLine(output);
        }

        var cpu = GetStringSection(output, ':', ',');
        var uptime = GetStringSection(output, "up", ",");

        var metrics = new UptimeMetrics();
        double percentage = double.Parse(cpu.Trim()) / Environment.ProcessorCount * 100;
        if (percentage > 100)
            percentage = 100;
        metrics.CPU = Math.Round(percentage, 0);
        metrics.UpTime = uptime;

        return metrics;
    }
    private static string GetStringSection(string source, string start, string end, bool LastIndexOf = false)
    {
        var sL = start.Length + 1;

        var pos = LastIndexOf ? source.LastIndexOf(start) + sL : source.IndexOf(start) + sL;
        var endPos = source.IndexOf(end, pos);

        return source.Substring(pos, endPos - pos);
    }
    private static string GetStringSection(string source, char start, char end)
    {
        var pos = source.LastIndexOf(start) + 1;
        var endPos = source.IndexOf(end, pos);
        return source.Substring(pos, endPos - pos);
    }
    public static MemoryMetrics GetUnixMetrics()
    {
        var output = "";

        var info = new ProcessStartInfo("free -m")
        {
            FileName = "/bin/bash",
            Arguments = "-c \"free -m\"",
            RedirectStandardOutput = true
        };

        using (var process = Process.Start(info))
        {
            output = process.StandardOutput.ReadToEnd();
        }

        var lines = output.Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics
        {
            Total = double.Parse(memory[1]) * 1024 * 1024,
            Used = double.Parse(memory[2]) * 1024 * 1024,
            Free = double.Parse(memory[3]) * 1024 * 1024
        };

        return metrics;
    }
    public static long GetTotalMemoryInBytes()
    {
        // only parse the file once
        if (totalMemoryInKb > 0)
        {
            return totalMemoryInKb;
        }

        string path = "/proc/meminfo";
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        using (var reader = new StreamReader(path))
        {
            string line = string.Empty;
            while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
            {
                if (line.Contains("MemTotal", StringComparison.OrdinalIgnoreCase))
                {
                    // e.g. MemTotal:       16370152 kB
                    var parts = line.Split(':');
                    var valuePart = parts[1].Trim();
                    parts = valuePart.Split(' ');
                    var numberString = parts[0].Trim();

                    var result = long.TryParse(numberString, out totalMemoryInKb);
                    return result ? (totalMemoryInKb * 1024) : throw new Exception($"Cannot parse 'MemTotal' value from the file {path}.");
                }
            }

            throw new Exception($"Cannot find the 'MemTotal' property from the file {path}.");
        }
    }

    private static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    public static string SizeSuffix(long value, int decimalPlaces = 1)
    {
        if (decimalPlaces < 0)
        { throw new ArgumentOutOfRangeException("decimalPlaces"); }
        if (value < 0)
        { return "-" + SizeSuffix(-value, decimalPlaces); }
        if (value == 0)
        { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}",
            adjustedSize,
            SizeSuffixes[mag]);
    }
    public static string GetLocalIPAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
public class MemoryMetrics
{
    public double Total;
    public double Used;
    public double Free;

    public double Percentage => Total / Used * 100;
}
public class UptimeMetrics
{
    public double CPU { get; set; }
    public string? UpTime { get; set; }
}
