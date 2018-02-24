$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    function getNodeUrl() {
        return $("#nodeUrl").val();
    }

    function getBlocks() {
        var url = getNodeUrl() + '/api/blocks';
        $.get(url, function (rawData) {
            _.each(rawData, function (d) {
                var timeAgoString = moment(d.dateCreated).fromNow();
                d.dateCreated = timeAgoString;
            });

            var templateData = { target: rawData };
            var template = _.template($("#blck-tmpl").text());
            $("#blocksPlaceHolder").html(template(templateData));
        });
    }

    $('#buttonHome').click(function () {
        showView("homeSection");
        $(this).parent().addClass("active");

        getBlocks();
    });

    $('#buttonBlocks').click(function () {
        showView("blocksSection");
        $(this).parent().addClass("active");
    });

    $('#buttonTransactions').click(function () {
        showView("transactionsSection");
        $(this).parent().addClass("active");
    });

    $('#buttonAccounts').click(function () {
        showView("accountsSection");
        $(this).parent().addClass("active");
    });

    $('#buttonPeersMap').click(function () {
        showView("peersMapSection");
        $(this).parent().addClass("active");
    });

    $(document).on({
        ajaxStart: function () { $("#loadingBox").show() },
        ajaxStop: function () { $("#loadingBox").hide() }
    });
    
    function showView(viewName) {
        $('li.nav-item').removeClass("active");
        $('section').hide();
        $('#' + viewName).show();
    }
    $('#buttonHome').click();
});