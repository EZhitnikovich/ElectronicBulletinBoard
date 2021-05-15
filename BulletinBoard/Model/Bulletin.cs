using System;
using System.Drawing.Imaging;
using System.Globalization;

namespace BulletinBoard.Model
{
    public class Bulletin
    {
        public string ImagePath { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }

        public Bulletin(string imagePath, string content, string title)
        {
            ImagePath = imagePath;
            Content = content;
            Title = title;
            Date = DateTime.Now;
        }

        public Bulletin()
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj.GetType() != this.GetType()) return false;
            var bul = obj as Bulletin;
            return ImagePath == bul.ImagePath && Content == bul.Content && Title == bul.Title && Date == bul.Date;
        }

        protected bool Equals(Bulletin other)
        {
            return ImagePath == other.ImagePath && Content == other.Content && Title == other.Title && Date.Equals(other.Date);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ImagePath, Content, Title, Date);
        }
    }
}