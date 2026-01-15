using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Grid
{
    public class GridViewModel<T>
    {
        public string Title { get; set; }
        public List<GridCellViewModel> HeaderCells { get; set; } = new List<GridCellViewModel>();
        public List<GridRowViewModel<T>> Rows { get; set; } = new List<GridRowViewModel<T>>();

        /// <summary>
        /// The ID of the Group or Establishment
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A useful tag. And why not.
        /// </summary>
        public string Tag { get; set; }

        public GridViewModel()
        {

        }

        public GridViewModel(string title)
        {
            Title = title;
        }

        public GridViewModel<T> AddHeaderCell(string text, string sortKey)
        {
            HeaderCells.Add(new GridCellViewModel(text, sortKey));
            return this;
        }

        public GridViewModel<T> AddHeaderCell(string text, bool condition, string sortKey = "", string sortType = "")
        {
            if(condition)
            {
                HeaderCells.Add(new GridCellViewModel(text, sortKey, sortType));
            }

            return this;
        }

        public GridRowViewModel<T> AddRow(T domainModel, bool editable, object sortValue = null)
        {
            var row = new GridRowViewModel<T>(this) { Model = domainModel, SortValue = sortValue, IsEditable = editable };
            Rows.Add(row);
            return row;
        }
    }
}
