using System;
using System.Drawing.Imaging;

namespace BulletinBoard.Model
{
    public class Bulletin
    {
        public object Image { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; }

        public Bulletin(object image, string content, string title)
        {
            Image = image;
            Content = content;
            Title = title;
            Date = DateTime.Now;
        }

        public static bool operator ==(Bulletin b1, Bulletin b2)
        {
            return b1.Image == b2.Image && b1.Content == b2.Content && b1.Title == b2.Title && b1.Date == b2.Date;
        }

        public static bool operator !=(Bulletin b1, Bulletin b2)
        {
            return !(b1 == b2);
        }
    }
}