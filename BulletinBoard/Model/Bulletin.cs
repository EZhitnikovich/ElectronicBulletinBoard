using System;
using System.Drawing.Imaging;

namespace BulletinBoard.Model
{
    [Serializable]
    public class Bulletin
    {
        public object Image;
        public string Content;

        public Bulletin(object image, string content)
        {
            Image = image;
            Content = content;
        }
    }
}