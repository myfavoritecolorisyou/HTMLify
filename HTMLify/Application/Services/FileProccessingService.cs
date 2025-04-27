using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTMLify.Domain.Services
{
    public class FileProccessingService
    {
        public void ConvertMhtToHtml(string mhtPath, string htmlPath)
        {
            var mhtMessage = MimeMessage.Load(mhtPath);
            var htmlBody = mhtMessage.HtmlBody ?? throw new Exception("HTML 내용을 찾을 수 없습니다.");

            var cidToBase64Map = new Dictionary<string, string>();
            var locationToBase64Map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part in mhtMessage.BodyParts)
            {
                if (part is MimePart mimePart)
                {
                    using var ms = new MemoryStream();
                    mimePart.Content.DecodeTo(ms);

                    string base64 = Convert.ToBase64String(ms.ToArray());
                    string mime = mimePart.ContentType.MimeType;

                    if (!string.IsNullOrEmpty(mimePart.ContentId))
                    {
                        cidToBase64Map[mimePart.ContentId.Trim('<', '>')] = $"data:{mime};base64,{base64}";
                    }

                    if (mimePart.ContentLocation is not null)
                    {
                        locationToBase64Map[mimePart.ContentId.ToString()] = $"data:{mime};base64,{base64}";
                    }
                }

                htmlBody = cidToBase64Map.Aggregate(htmlBody, (current, kvp) =>
                    current.Replace($"cid:{kvp.Key}", kvp.Value));

                htmlBody = locationToBase64Map.Aggregate(htmlBody, (current, kvp) =>
                    current.Replace(kvp.Key, kvp.Value));

                htmlBody = ReplaceFileSrcWithBase64(htmlBody);
            }
        }

        private string ReplaceFileSrcWithBase64(string html)
        {
            const string pattern = @"<img[^>]+src\s*=\s*[""']file:///([^""']+)[""'][^>]*>";
            return Regex.Replace(html, pattern, match =>
            {
                string fullPath = match.Groups[1].Value;
                if (!File.Exists(fullPath)) return match.Value;

                try
                {
                    byte[] imagesBytes = File.ReadAllBytes(fullPath);
                    string base64 = Convert.ToBase64String(imagesBytes);
                    string extension = Path.GetExtension(fullPath).ToLower();
                    string mimeType = extension switch
                    {
                        ".jpg" => "image/jpeg",
                        ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".bmp" => "image/bmp",
                        _ => "application/octet-stream"
                    };

                    return Regex.Replace(match.Value, @"src\s*=\s*[""'][^""']+[""']", $"src=\"data:{mimeType};base64,{base64}\"");
                }
                catch
                {
                    return match.Value;
                }
            });
        } // ReplaceFileSrcWithBase64();
    }
}
