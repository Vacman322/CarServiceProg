using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace CarServiceProg.EF
{
    public partial class Client
    {
        public Nullable<DateTime> LastVisit
        {
            get
            {
                var lastVisit = ClientService.OrderBy(r => r.StartTime).LastOrDefault();
                if (lastVisit == null)
                    return null;
                else
                    return lastVisit.StartTime;
            }
        }
        public int VisitsCount { get => ClientService.Count; }
        public List<TextBlock> TagsView
        {
            get
            {
                var result = new List<TextBlock>();
                foreach (var tag in Tag)
                {
                    var color = (Color)ColorConverter.ConvertFromString("#" + tag.Color);
                    var tb = new TextBlock()
                    {
                        Text = tag.Title,
                        Padding = new System.Windows.Thickness(5),
                        Background = new SolidColorBrush(color),
                        Foreground = (color.R + color.G + color.B) / 3 < 127 ? Brushes.White : Brushes.Black
                    };
                    result.Add(tb);
                }
                return result;
            }
        }

        public string ImgPath { get => new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName +"\\"+ PhotoPath; }

    }
}
