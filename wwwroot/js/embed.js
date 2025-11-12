$(function () {

    // 1 - Get DOM object for the div that's the report container.
    var reportContainer = document.getElementById("embed-container");

    // 2 - Get the report embedding data from the view model.
    var reportId = window.viewModel.reportId;
    var embedUrl = window.viewModel.embedUrl;
    var token = window.viewModel.token

    // 3 - Embed the report by using the Power BI JavaScript API.
    var models = window['powerbi-client'].models;

    var config = {
      type: 'report',
      id: reportId,
      embedUrl: embedUrl,
      accessToken: token,
      permissions: models.Permissions.All,
      tokenType: models.TokenType.Embed,
      viewMode: models.ViewMode.View,
      settings: {
        panes: {
          filters: { expanded: false, visible: true },
          pageNavigation: { visible: false }
        }
      }
    };

    // Embed the report and display it within the div container.
    var report = powerbi.embed(reportContainer, config);

    // 4 - Add logic to resize the embed container on a window resize event.
    var heightBuffer = 12;
    var newHeight = $(window).height() - ($("header").height() + heightBuffer);
    $("#embed-container").height(newHeight);
    $(window).resize(function () {
      var newHeight = $(window).height() - ($("header").height() + heightBuffer);
      $("#embed-container").height(newHeight);
    });

  });