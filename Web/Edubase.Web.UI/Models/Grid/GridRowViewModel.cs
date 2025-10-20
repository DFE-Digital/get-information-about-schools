using System;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models.Grid
{
    public class GridRowViewModel<T>
    {
        public List<GridCellViewModel> Cells { get; set; } = new List<GridCellViewModel>();

        public GridViewModel<T> Parent { get; private set; }
        
        public T Model { get; set; }

        public object SortValue { get; set; }

        public GridRowViewModel<T> AddCell(object content)
        {
            var cell = new GridCellViewModel(content?.ToString());
            Cells.Add(cell);
            if (Cells.Count > Parent.HeaderCells.Count)
            {
                throw new Exception("You cannot add more row cells than header cells");
            }

            cell.Parent = Parent.HeaderCells[Cells.Count - 1];
            return this;
        }

        public GridRowViewModel<T> AddCell(object content, bool condition)
        {
            if (condition)
            {
                AddCell(content);
            }

            return this;
        }

        public GridRowViewModel(GridViewModel<T> parent)
        {
            Parent = parent;
        }
    }
}
