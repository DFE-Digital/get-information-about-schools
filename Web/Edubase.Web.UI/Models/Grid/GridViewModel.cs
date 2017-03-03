using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Grid
{
    public class GridViewModel
    {
        public string Title { get; set; }
        public List<GridCellViewModel> HeaderCells { get; set; } = new List<GridCellViewModel>();
        public List<GridRowViewModel> Rows { get; set; } = new List<GridRowViewModel>();

        /// <summary>
        /// The ID of the Group or Establishment
        /// </summary>
        public int Id { get; set; }

        public GridViewModel()
        {

        }

        public GridViewModel(string title)
        {
            Title = title;
        }

        public GridViewModel AddHeaderCell(string text)
        {
            HeaderCells.Add(new GridCellViewModel(text));
            return this;
        }

        public GridViewModel AddHeaderCell(string text, bool condition)
        {
            if(condition) HeaderCells.Add(new GridCellViewModel(text));
            return this;
        }

        public GridRowViewModel AddRow(string keyOrId = null)
        {
            var row = new GridRowViewModel(this) { KeyOrId = keyOrId };
            Rows.Add(row);
            return row;
        }


    }
}