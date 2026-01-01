using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Model
{

    public enum PossibleColors
    {
        None, Red, Blue, Green, Yellow, Purple, Orange, Gray
    }
    public class FileReliquary
    {
        public bool IsFavourite {  get; set; }
        public PossibleColors ColorDecoration { get; set; } = PossibleColors.None;

        public List<string> Tags { get; set; } = new List<string>();
        public string Notes { get; set; } = string.Empty;
    }
}
