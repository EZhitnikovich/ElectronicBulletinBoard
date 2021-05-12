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
    }
}