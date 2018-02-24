$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    $('#buttonHome').click(function () {
        showView("homeSection");
        $(this).parent().addClass("active");
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
    showView("homeSection");
});