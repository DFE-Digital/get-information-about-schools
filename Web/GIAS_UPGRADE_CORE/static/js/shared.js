
// should contain common actions that could be reused by all pages

function getFileName(path) {
    return path.split('\\').pop().split('/').pop();
}

function getFolderName(path) {
  if (path.lastIndexOf('\\') >= 0) {
    return path.substring(0, path.lastIndexOf('\\'));
  }

  return path.substring(0, path.lastIndexOf('/'));
}

function getRuleCategory(ruleId) {
    return ruleId.split('.')[0];
}

function getPosition(instance) {
    let position = "";
    
    if (instance.location.line != undefined && instance.location.line >= 0)
    {
      position = `(${instance.location.line + 1}`;
      if (instance.location.column != undefined && instance.location.column >= 0)
      {
          position += `,${instance.location.column + 1}`;
      }

      position += ")";
    }

    return position;
}

String.prototype.escape = function() {
  var tagsToReplace = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;'
  };
  return this.replace(/[&<>]/g, function(tag) {
      return tagsToReplace[tag] || tag;
  });
};

// Toggle style for currently expanded td, (class does not work since we use without-border class and it overrides our left border)
function highlightExpandedRow(expander) {
  let td = expander.closest('td');
  if (td.attr('style') === undefined) {
      td.attr('style', 'border-left: 2px solid #0d6efd;');
  }
  else {
      td.removeAttr('style');
  }
}


const CHART_COLORS = {
  red: 'rgb(255, 99, 132)',
  orange: 'rgb(255, 159, 64)',
  yellow: 'rgb(255, 205, 86)',
  blue: 'rgb(54, 162, 235)',
  green: 'rgb(75, 192, 192)',
  purple: 'rgb(153, 102, 255)',
  grey: 'rgb(201, 203, 207)'
};

// data is a dictionary of key-value pairs: labels and values
function generateSeverityCard(data, chartContainer) {
    const chartLabels = Object.keys(data);
    const chartData = Object.values(data);

    new Chart(chartContainer, {
      type: 'doughnut',
      data: {
        labels: chartLabels,
        datasets: [
          {
            data: chartData,
            backgroundColor: Object.values(CHART_COLORS),
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'right',
            labels: {
              generateLabels: (chart) => {
                const datasets = chart.data.datasets;
                return datasets[0].data.map((data, i) => ({
                  text: `${getLocString(chart.data.labels[i]) ?? chart.data.labels[i]} (${data})`,
                  fillStyle: datasets[0].backgroundColor[i],
                  index: i
                }))
              }
            }
          }
        }
      },
    });

    var areaLabel = (strings["IssuesSeverityChart"] ?? "Issues severity chart: ") + getKeyValueStringFromObject(data);
    chartContainer.setAttribute('aria-label', areaLabel);
}

function getKeyValueStringFromObject(data) {
  var areaLabel = "";

  for(const key of Object.keys(data)) {
    const value = data[key];
    areaLabel += `${key} (${value}) `;
  }

  return areaLabel;
}

// data is a dictionary of key-value pairs: labels and values
function generateCategoryCard(data, chartContainer) {
    const chartLabels = [];

    for(const key of Object.keys(data)) {
      const value = data[key];
      chartLabels.push(`${key} (${value})`);
    }

    const chartData = Object.values(data);

    new Chart(chartContainer, {
        type: 'bar',
        data: {
          labels: chartLabels,
          datasets: [
            {
              data: chartData,
              backgroundColor: 'rgb(54, 162, 235)'
            },
          ],
        },
        options: {
          indexAxis: 'y',
          responsive: true,
          plugins: {
            legend: {
              display: false
            },
            tooltips: {
              enabled: true
            }
          }
        }
      });

    var areaLabel = (strings["IssuesCategoriesChart"] ?? "Issues categories chart: ") + getKeyValueStringFromObject(data);;
    chartContainer.setAttribute('aria-label', areaLabel);
}

function getIssueDetails(instance) {
  if (instance === undefined)
  {
      return "";
  }

  let rule = results.rules[instance.ruleId]
  let ruleDescription = rule.description;
  let issueDescription = instance.description;
  let incidentLabel = instance.location.label ?? rule.label;
  let issueSnippet = instance.location.snippet;
  let ruleSampleSnippet = rule.snippet;

  let html = '<div class="details-text-size">';

  if (incidentLabel !== undefined) {
      html += `<pre style="white-space: pre-wrap;font-family: inherit; font-weight: 600;">${incidentLabel}</pre>`;
  }

  if (ruleDescription !== undefined) {
      html += `<pre style="white-space: pre-wrap;font-family: inherit">${ruleDescription}</pre>`;
  }

  if (issueDescription !== undefined) {
      html += `<pre style="white-space: pre-wrap;font-family: inherit">${issueDescription}</pre>`;
  }

  if (issueSnippet !== undefined) {
      html += `<p><em loc-text="Source">Source:</em></p>`;
      html += `<pre class="bg-light"><code style="white-space: pre-wrap;">${issueSnippet.escape()}</code></pre>`;
  }

  if (ruleSampleSnippet !== undefined) {
      html += `<p><em>Potential change:</em></p>`;
      html += `<pre class="bg-light"><code style="white-space: pre-wrap;">${ruleSampleSnippet.escape()}</code></pre>`;
  }

  if (rule.links !== undefined) {
      html = addLinks(html, rule.links);
  }

  if (instance.location.links !== undefined) {
      html = addLinks(html, instance.location.links);
  }

  html += '</div>'
  return html;
}

function addLinks(html, links) {
    for (let i = 0; i < links.length; i++) {

        let link = links[i];
        if (link.url === undefined) {
            continue;
        }

        var title = adjustLinkTitle(link, i);

        html += `<a href="${link.url}" class="accessible-link-style">${title}</a><br/>`;
    }

    return html;
}

function adjustLinkTitle(link, position) {
    var title = link.title;

    if (title === undefined) {
        if (position == 0) {
            title = "More info";
        }
        else {
            title = link.url;
        }
    }

    return title;
}

function getIssuesCount(ruleInstances) {
    let projectRules = {};
    let count = 0;

    for (const instance of ruleInstances) {
        let projectRule = projectRules[instance.ruleId];

        if (projectRule === undefined) {
            projectRules[instance.ruleId] = {};
            count++;
        }
    }

    return count;
}

const stateMap = {
  "Active": 0,
  "Resolved": 1,
  "N/A": 2
};

function getState(ruleInstances) {
  let minsStateWeight = 2;
  let minState = getLocString("N/A") ?? "N/A";

  for (const instance of ruleInstances) {
    let stateWeight = stateMap[instance.state];

    if (stateWeight < minsStateWeight) {
        minsStateWeight = stateWeight;
        minState = instance.state;
    }
  }

  return getLocString(minState) ?? minState;
}

function getRelativePath(path, basePath) {
  if (path === undefined || basePath === undefined) {
    return path;
  }
  
  if (path.startsWith(basePath + '\\') || path.startsWith(basePath + '/')) {
    return path.substring(basePath.length + 1);
  }

  return getUpPath(path, basePath);
}

function getUpPath(path, basePath) {

  let upPath = "";

  while (basePath !== undefined) {
    const lastSeparator = basePath.lastIndexOf('\\');

    if (lastSeparator < 0) {
      return path;
    }

    basePath = basePath.substring(0, lastSeparator);
    upPath += "..\\";

    if (path.startsWith(basePath + '\\') || path.startsWith(basePath + '/')) {
      return upPath + path.substring(basePath.length + 1);
    }
  }

  return path;
}


