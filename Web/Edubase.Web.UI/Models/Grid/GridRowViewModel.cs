using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Grid
{
    public class GridRowViewModel
    {
        public List<GridCellViewModel> Cells { get; set; } = new List<GridCellViewModel>();

        public GridViewModel Parent { get; private set; }

        public GridRowViewModel AddCell(object content)
        {
            var cell = new GridCellViewModel(content?.ToString());
            Cells.Add(cell);
            if (Cells.Count > Parent.HeaderCells.Count) throw new Exception("You cannot add more row cells than header cells");
            cell.Parent = Parent.HeaderCells[Cells.Count - 1];
            return this;
        }

        public GridRowViewModel AddCell(object content, bool condition)
        {
            if (condition) AddCell(content);
            return this;
        }

        public GridRowViewModel(GridViewModel parent)
        {
            Parent = parent;
        }
    }
}