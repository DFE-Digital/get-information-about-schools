using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Grid
{
    public class GridCellViewModel
    {
        public string Text { get; set; }
        public GridCellViewModel Parent { get; set; }
        public GridCellViewModel(string text)
        {
            Text = text;
        }
    }
}