using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RunPdftopngs
{
    public class Pdftopngs
    {
        public void ExtractToPngs(
            string pdfFile,
            Action<string> pngPathCallback,
            string exePath = null,
            string exportPrefix = null,
            int? dpi = null,
            int? first = null,
            int? last = null,
            Action<string[]> lineCallback = null,
            string moreOptions = null
        )
        {
            exportPrefix = (exportPrefix == null)
                ? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + "_")
                : exportPrefix;
            var psi = new ProcessStartInfo(
                exePath ?? GetEmbeddedExePath(),
                string.Join(" "
                    , dpi.HasValue ? $"-r {dpi}" : ""
                    , first.HasValue ? $"-f {first}" : ""
                    , last.HasValue ? $"-l {last}" : ""
                    , moreOptions ?? ""
                    , $"\"{pdfFile}\""
                    , $"\"{exportPrefix}\""
                )
            )
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false,
            };
            var p = Process.Start(psi);
            p.Start();
            var reader = p.StandardOutput;
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                var cells = line.Split('\t');
                lineCallback?.Invoke(cells);
                if (cells[0] == "image" && cells.Length >= 3)
                {
                    pngPathCallback?.Invoke(cells[2]);
                }
            }
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                var message = p.StandardError.ReadToEnd();
                throw new PdftopngsException(p.ExitCode, message);
            }
        }

        public static string GetEmbeddedExePath() => Path.Combine(
            Path.GetDirectoryName(new Uri(typeof(Pdftopngs).Assembly.Location).LocalPath),
            "GPL",
            "pdftopngs.exe"
        );
    }
}
