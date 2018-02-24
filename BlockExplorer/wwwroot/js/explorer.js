$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    function getNodeUrl() {
        return $("#nodeUrl").val();
    }

    function getBlocks() {
        var url = getNodeUrl() + '/api/blocks';
        $.get(url, function (rawData) {
            if (rawData.length > 0) {
                _.each(rawData, function (d) {
                    var timeAgoString = moment(d.dateCreated).fromNow();
                    d.dateCreated = timeAgoString;
                });

                var templateData = { target: rawData };
                var template = _.template($("#blck-tmpl").text());
                $("#blocksPlaceHolder").html(template(templateData));
            }
        });
    }

    function getBlocksList() {
        var url = getNodeUrl() + '/api/blocks';
        $.get(url, function (rawData) {
            if (rawData.length > 0) {
                _.each(rawData, function (d) {
                    var timeAgoString = moment(d.dateCreated).fromNow();
                    d.dateCreated = timeAgoString;
                    var blockHashLabel = d.blockHash.substring(0, 25) + '...';
                    d.blockHashLabel = blockHashLabel;
                });

                var templateData = { target: rawData };
                var template = _.template($("#blck-list-tmpl").text());
                $("#blocksListPlaceHolder").html(template(templateData));
            }
        });
    }

    function getTransactions() {
        var url = getNodeUrl() + '/api/transactions/pending';
        $.get(url, function (rawData) {
            if (rawData.length > 0) {
                _.each(rawData, function (d) {
                    var timeAgoString = moment(d.dateCreated).fromNow();
                    d.dateCreated = timeAgoString;
                    var fromLabel = d.from.substring(0, 10) + '...';
                    d.fromLabel = fromLabel;
                    var toLabel = d.to.substring(0, 10) + '...';
                    d.toLabel = toLabel;
                    var transactionHashLabel = d.transactionHashHex.substring(0, 10) + '...';
                    d.transactionHashLabel = transactionHashLabel;
                });

                var templateData = { target: rawData };
                var template = _.template($("#trns-tmpl").text());
                $("#transactionsPlaceHolder").html(template(templateData));
            }
        });
    }

    function getTransactionsList() {
        var url = getNodeUrl() + '/api/transactions/pending';
        $.get(url, function (rawData) {
            if (rawData.length > 0) {
                _.each(rawData, function (d) {
                    var timeAgoString = moment(d.dateCreated).fromNow();
                    d.dateCreated = timeAgoString;
                    var fromLabel = d.from.substring(0, 10) + '...';
                    d.fromLabel = fromLabel;
                    var toLabel = d.to.substring(0, 10) + '...';
                    d.toLabel = toLabel;
                    var transactionHashLabel = d.transactionHashHex.substring(0, 10) + '...';
                    d.transactionHashLabel = transactionHashLabel;
                });

                var templateData = { target: rawData };
                var template = _.template($("#trns-list-tmpl").text());
                $("#transactionsListPlaceHolder").html(template(templateData));
            }
        });
    }

    function getPeersNetwork() {
        var imagePath = "./images/pc-icon.png";
        var url = getNodeUrl() + '/api/peers/network';
        $.get(url, function (rawData) {
            if (rawData.nodes.length > 0) {
                _.each(rawData.nodes, function (node) {
                    node.image = imagePath;
                    node.shape = 'image';
                });
                var data = {
                    nodes: new vis.DataSet(rawData.nodes),
                    edges: new vis.DataSet(rawData.edges)
                };
                var container = document.getElementById("peersNetworkPlaceholder");
                var options = {};
                var network = new vis.Network(container, data, options);
            }
        });
        
        //var nodes = new vis.DataSet([
        //    { id: 1, label: 'Node 1', image: imagePath, shape: 'image' },
        //    { id: 2, label: 'Node 2', image: imagePath, shape: 'image' },
        //    { id: 3, label: 'Node 3', image: imagePath, shape: 'image' },
        //    { id: 4, label: 'Node 4', image: imagePath, shape: 'image' },
        //    { id: 5, label: 'Node 5', image: imagePath, shape: 'image' }
        //]);
        //var edges = new vis.DataSet([
        //    { from: 1, to: 3 },
        //    { from: 1, to: 2 },
        //    { from: 2, to: 4 },
        //    { from: 2, to: 5 }
        //]);

        //var container = document.getElementById("peersNetworkPlaceholder");
        //var data = {
        //    nodes: nodes,
        //    edges: edges
        //};
        //var options = {};
        //var network = new vis.Network(container, data, options);
    }

    $('#buttonHome').click(function () {
        showView("homeSection");
        $(this).parent().addClass("active");

        getBlocks();
        getTransactions();
    });

    $('#buttonBlocks').click(function () {
        showView("blocksSection");
        $(this).parent().addClass("active");

        getBlocksList();
    });

    $('#buttonTransactions').click(function () {
        showView("transactionsSection");
        $(this).parent().addClass("active");

        getTransactionsList();
    });

    $('#buttonAccounts').click(function () {
        showView("accountsSection");
        $(this).parent().addClass("active");
    });

    $('#buttonPeersNetwork').click(function () {
        showView("peersMapSection");
        $(this).parent().addClass("active");

        getPeersNetwork();
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