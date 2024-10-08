﻿using HtmlAgilityPack;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Pet_Shop2.Helper
{
    public static class Utilities
    {
        public static int PAGE_SIZE=15;


        //Hàm check Email
        public static bool  IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        //Hàm chuyển đổi đơn vị
        public static string ToVND(this double dongia)
        {
            return dongia.ToString("#,##0") + " VNĐ";
        }

        public static string ToUSD(this double dongia)
        {
            double dongiaUSD = dongia / 25315;
            return dongiaUSD.ToString("C", new CultureInfo("en-US"))
                .Remove(0, 1) + " USD";
        }

        //Hàm tạo đường dẫn mới nếu không có
        public static void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists) {
                Directory.CreateDirectory(path);
            
            }
        }
        public static string ToTitleCase(string str)
        {
            string result = str;
            if(!string.IsNullOrEmpty(result))
            {
                var words = str.Split(' ');
                for(int index = 0;index< words.Length;index++)
                {
                    var s = words[index];
                    if(s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }    
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        public static string GetRandomKey(int length=5)
        {
            //Chuỗi mẫu (pattern)
            string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();
            StringBuilder sb= new StringBuilder();
            for(int i=0; i<length;i++)
            {
                sb.Append(pattern[rd.Next(0,pattern.Length)]);
            }    
            return sb.ToString();
        }

        public static string ToFriendlyUrl(this string message) {
            return message.Replace(" ", "-")
                .ToLower()
                .ToNonAccentVietnamese();
        }
        public static string ToNonAccentVietnamese(this string message) {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = message.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty)
                .Replace('\u0111', 'd')
                .Replace('\u0110', 'D');
        }

        public static string SEOUrl(this string url)
        {
            var result = url.ToLower().Trim();
            result = Regex.Replace(result, "áàạảãâấầậẩẫăắằặẳẵ", "a");
            result = Regex.Replace(result, "éèẹẻẽêếềệểễ", "e");
            result = Regex.Replace(result, "óòọỏõôốồộổỗơớờợởỡ", "o");
            result = Regex.Replace(result, "úùụủũưứừựửữ", "u");
            result = Regex.Replace(result, "íìịỉĩ", "i");
            result = Regex.Replace(result, "ýỳỵỷỹ", "y");
            result = Regex.Replace(result, "đ", "d");
            result = Regex.Replace(result, "[^a-z0-9-]", "");
            result = Regex.Replace(result, "(-)+", "-");
            return result;
        }
        public static List<string> GetTextFromHtml(string html)
        {
            List<string> textValues = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Duyệt qua tất cả các nút chứa trong tài liệu
            foreach (HtmlNode node in doc.DocumentNode.DescendantsAndSelf())
            {
                // Nếu nút là một nút văn bản (text node), lấy giá trị text của nó
                if (node.NodeType == HtmlNodeType.Text)
                {
                    string text = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        textValues.Add(text);
                    }
                }
            }

            return textValues;
        }
        public static async Task<string> UploadFile(IFormFile file,string sDirectory,string newname)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FolderImages", sDirectory);
                CreateIfMissing(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FolderImages", sDirectory, newname);
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    return "";
                }
                else
                {
                    using(var stream=new FileStream(pathFile,FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }    
            }
            catch
            {
                return "";
            }
        }
        
    }
}
