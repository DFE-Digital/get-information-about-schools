import GiasOkCancel from "../GiasModules/GiasModals/GiasOkCancel";
import GiasTabs from '../GiasModules/GiasTabs';
import GiasDownloadFilters from '../GiasSearchFilters/GiasDownloadFilters';

$('#main-content').find('.gias-tabs-wrapper').giasTabs();

$('#select-all').on('click', (e)=> {
	e.preventDefault();
	check(true);
});

$('#clear-all').on('click', (e)=> {
	e.preventDefault();
	check(false);
});

$('#downloadSelected').on('click', (e)=> {
	var anyChecked = false;
    $('input:checkbox').each(function(){
	  if (this.checked) {
	    anyChecked = true;	
	  }
    });
	
    if (anyChecked == false) {
	  e.preventDefault();
      $('#downloadSelected').okCancel({
        ok: function(){
  	      this.closeModal();
        },
        cancel: null,
        title: 'No files selected',
        content: 'Please select at least one file to download.',
		immediate: true
      });
    }
});

function check(source) {
  $('input:checkbox').prop('checked',source);
}

GiasDownloadFilters.init();
