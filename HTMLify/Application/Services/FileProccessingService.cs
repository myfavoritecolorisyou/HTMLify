using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTMLify.Application.Services
{
    public class FileProccessingService
    {
        public string ConvertMhtToHtml(string mhtPath)
        {
            var htmlTempFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HTMLify", "TempHtmlPreview");

            Directory.CreateDirectory(htmlTempFolder);

            var htmlFileName = Path.GetFileNameWithoutExtension(mhtPath) + ".html";
            var htmlFilePath = Path.Combine(htmlTempFolder, htmlFileName);

            EnsureOutputDirectoryExists(htmlFilePath);

            var mhtMessage = MimeMessage.Load(mhtPath);
            var htmlBody = mhtMessage.HtmlBody ?? throw new Exception("HTML 내용을 찾을 수 없습니다.");

            var base64Mappings = ExtractBase64Mappings(mhtMessage);

            htmlBody = ApplyBase64Mapping(htmlBody, base64Mappings);
            htmlBody = ReplaceFileSrcWithBase64(htmlBody);

            File.WriteAllText(htmlFilePath, htmlBody);

            return htmlFilePath;
        }

        public void ConvertMhtToHtml(string mhtPath, string htmlPath)
        {
            EnsureOutputDirectoryExists(htmlPath);

            var mhtMessage = MimeMessage.Load(mhtPath);
            var htmlBody = mhtMessage.HtmlBody ?? throw new Exception("HTML 내용을 찾을 수 없습니다.");

            var base64Mappings = ExtractBase64Mappings(mhtMessage);

            htmlBody = ApplyBase64Mapping(htmlBody, base64Mappings);
            htmlBody = ReplaceFileSrcWithBase64(htmlBody);

            File.WriteAllText(htmlPath, htmlBody);
        }

        private void EnsureOutputDirectoryExists(string htmlPath)
        {
            string? outputDir = Path.GetDirectoryName(htmlPath);
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
        }

        /// <summary>
        /// MHT message에서 Base64로 인코딩된 리소스 매핑을 추출합니다.
        /// </summary>
        /// <param name="mhtMessage">MHT message 객체</param>
        /// <returns>리소스 경로와 Base64 데이터의 매핑</returns>
        private Dictionary<string ,string> ExtractBase64Mappings(MimeMessage mhtMessage)
        {
            var base64Mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part in mhtMessage.BodyParts.OfType<MimePart>())
            {
                using var ms = new MemoryStream();
                part.Content.DecodeTo(ms);

                string base64 = Convert.ToBase64String(ms.ToArray());
                string mime = part.ContentType.MimeType;

                if (!string.IsNullOrEmpty(part.ContentId)) 
                    base64Mappings[$"cid:{part.ContentId.Trim('<', '>')}"] = $"data:{mime};base64,{base64}";

                if (part.ContentLocation is not null) 
                    base64Mappings[part.ContentLocation.ToString()] = $"data:{mime};base64,{base64}";
            }

            return base64Mappings;
        }

        private string ApplyBase64Mapping(string htmlBody, Dictionary<string, string> base64Mappings)
        {
            return base64Mappings.Aggregate(htmlBody, (current, kvp) => current.Replace(kvp.Key, kvp.Value));
        }

        /// <summary>
        /// HTML에서 파일 경로를 Base64 데이터로 변환합니다.
        /// </summary>
        /// <param name="html">HTML 문자열</param>
        /// <returns>Base64 데이터가 적용된 HTML 문자열</returns>
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
                    string mimeType = GetMimeType(Path.GetExtension(fullPath));

                    return Regex.Replace(match.Value, @"src\s*=\s*[""'][^""']+[""']", $"src=\"data:{mimeType};base64,{base64}\"");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"이미지 변환 실패: {ex.Message}");
                    return match.Value;
                }
            });
        } // ReplaceFileSrcWithBase64();

        private string GetMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
