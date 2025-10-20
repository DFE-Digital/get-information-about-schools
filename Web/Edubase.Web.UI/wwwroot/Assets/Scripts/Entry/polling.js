import GiasPolling from '../GiasModules/GiasPolling';

(function () {
  let options;
  const loc = window.location.toString();

  /**
   * generate the URLs for the ajax requests
   * that use restful urls containing the action guid
   * and we can't just hijack the current pages url params
   *
   * @param urlPortion the url without the guid
   * @param splitText text where the url will be split from to obtain the guid
   * @returns {string} url including the guid
   */
  function restfulUrl(urlPortion, splitText) {
    let splitLengthModifier = 10;
    if (splitText) {
      splitLengthModifier = splitText.length;
      splitText = splitText.toLowerCase();

    } else {
      splitText = '/download/';

    }
    const start = loc.toLowerCase().indexOf(splitText) + splitLengthModifier;
    const end = loc.indexOf('?') > -1 ? loc.indexOf('?') : loc.length;
    const id = loc.slice(start, end)

    return urlPortion + id;

  }

  const pollingConfigs = {
    establishments: {
      pollingUrl: '/Establishments/Search/EstablishmentDownloadAjax'
    },
    groups: {
      pollingUrl: '/Groups/Search/GroupDownloadAjax'
    },
    governors: {
      pollingUrl: '/Governors/Search/GovernorsDownloadAjax'
    },
    changeHistoryBulk: {
      pollingUrl: restfulUrl('/changeHistory/DownloadAjax/')
    },
    changeHistory: {
      pollingUrl: '/changeHistory/DownloadAjax/'
    },
    indiSchools: {
      pollingUrl: restfulUrl('/independent-schools/downloadAjax/')
    },
    bulkFree: {
      pollingUrl: restfulUrl('/Establishments/bulk-create-free-schools-ajax/', '/bulk-create-free-schools/')
    },
    bulkEstabUpdate: {
      // ooh double guids
      pollingUrl: loc.replace('result', 'resultAjax')
    },
    bulkAsscociate: {
      pollingUrl: restfulUrl('/Establishments/bulk-associate-estabs-to-groups-ajax/', '/bulk-associate-estabs-to-groups/')
    },
    downloads: {
      pollingUrl: restfulUrl('/Downloads/GenerateAjax/', '/Generated/')
    }
  }


  if (loc.indexOf('/Establishments/Search/') > -1) {
    options = pollingConfigs.establishments;

  } else if (loc.indexOf('/Groups/Search/') > -1) {
    options = pollingConfigs.groups;

  } else if (loc.indexOf('/Governors/Search/') > -1) {
    options = pollingConfigs.governors;

  } else if (loc.indexOf('/ChangeHistory/Download/') > -1) {
    options = pollingConfigs.changeHistoryBulk;

  } else if (loc.indexOf('/ChangeHistory/Search/Download/') > -1) {
    options = pollingConfigs.changeHistoryBulk;

  } else if (loc.indexOf('/independent-schools/download/') > -1) {
    options = pollingConfigs.indiSchools;

  } else if (loc.indexOf('/bulk-create-free-schools')> -1) {
    options = pollingConfigs.bulkFree;

  } else if (loc.indexOf('/Establishments/BulkUpdate/result/') > -1) {
    options = pollingConfigs.bulkEstabUpdate;

  } else if (loc.indexOf('/Establishments/bulk-associate-estabs-to-groups/') > -1) {
    options = pollingConfigs.bulkAsscociate;

  } else if (loc.indexOf('/Downloads/Generated/') > -1) {
    options = pollingConfigs.downloads;

  }




  if (typeof options !== 'undefined') {
    new GiasPolling(options);
  } else {
    console.log('polling options are undefined');
  }

}());
