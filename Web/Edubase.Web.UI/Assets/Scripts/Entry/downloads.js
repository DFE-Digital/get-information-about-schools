import GiasOkCancel from "../GiasModules/GiasModals/GiasOkCancel";
import GiasTabs from '../GiasModules/GiasTabs';
import GiasDownloadFilters from '../GiasSearchFilters/GiasDownloadFilters';

$('#main-content').find('.gias-tabs-wrapper').giasTabs();

GiasDownloadFilters.init();
