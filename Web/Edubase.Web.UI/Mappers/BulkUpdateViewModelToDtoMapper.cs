using System;
using System.IO;
using Edubase.Common.IO;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models;

namespace Edubase.Web.UI.Mappers
{
    public static class BulkUpdateViewModelToDtoMapper
    {
        public static BulkUpdateDto MapToDto(this BulkUpdateViewModel viewModel)
        {
            var fileName = FileHelper.GetTempFileName(Path.GetExtension(viewModel.BulkFile.FileName));

            return new BulkUpdateDto
            {
                BulkFileType = viewModel.BulkUpdateType.Value,
                FileName = fileName,
                OverrideCRProcess = viewModel.CanOverrideCRProcess && viewModel.OverrideCRProcess,
                EffectiveDate = viewModel.EffectiveDate.ToDateTime().HasValue ? viewModel.EffectiveDate.ToDateTime() : DateTime.Today
            };
        }
    }
}
