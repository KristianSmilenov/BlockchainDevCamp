﻿$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

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
        var url = getNodeUrl() + '/api/transactions';
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

    function getTransactionsList(filter) {
        // TODO: Fix template to show full transaction hash or allow users to copy it

        var url = getNodeUrl() + '/api/transactions';
        if (filter && filter != '') {
            url += '?status=' + filter
        }

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
    }

    function getNodeInfo() {
        var url = getNodeUrl() + '/api/info';
        $.get(url, function (nodeData) {
            $("#nodeAbout").text("Running on: " + nodeData.about);
            $("#nodeName").text("Connected to: " + nodeData.nodeName);
            $("#blockchainDifficulty").text("Difficulty: " + nodeData.difficulty);
        });
    }

    function getNodeUrl() {
        return $("#nodeUrl").val();
    }

    $('#buttonSearch').click(function () {
        showView("searchResultsSection");
        $("#searchResultsPlaceHolder").text("No data found.");
        var searchText = $("#searchBox").val();
        if (searchText.length == 0)
            return;

        var transactionUrl = getNodeUrl() + '/api/transactions/' + searchText;
        $.get(transactionUrl, function (data) {
            setSearchResultData(data, 'transaction');
        }).fail(function () {
            tryGetWalletInfo(searchText);
        });
    });

    function tryGetWalletInfo(searchText) {
        var walletInfoUrl = getNodeUrl() + '/api/balance/' + searchText;
        $.get(walletInfoUrl, function (data) {
            setSearchResultData(data, 'wallet');
        }).fail(function () {
            tryGetBlockInfo(searchText);
        });
    }

    function tryGetBlockInfo(searchText) {
        if (!isNaN(searchText)) {
            var blockIndex = parseInt(searchText)
            var urlGetBlock = getNodeUrl() + '/api/blocks/' + blockIndex;
            $.get(urlGetBlock, function (data) {
                setSearchResultData(data, 'block');
            });
        }
    }

    function setSearchResultData(data, type) {
        //TODO: render template
        $("#searchResultsPlaceHolder").text(JSON.stringify(data, undefined, 2));
    }

    $('#buttonSetNodeUrl').click(function () {
        getNodeInfo();
        $('#buttonHome').click();
    });

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

    $('#buttonPending').click(function () {
        $("#transactionsListPlaceHolder").html('No transactions available.');
        getTransactionsList('pending');
    });

    $('#buttonConfirmed').click(function () {
        $("#transactionsListPlaceHolder").html('No transactions available.');
        getTransactionsList('confirmed');
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
    getNodeInfo();
});