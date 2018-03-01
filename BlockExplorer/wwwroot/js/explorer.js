$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }
    $('#buttonDisplayBalance').click(displayBalance);
    $('#viewAllBlocks').click(loadBlocksSection);
    $("#viewAllTransactions").click(loadTransactionsSection);

    let activeView = "";
    let itemsListPageSize = 10;
    $("#accountBalanceForm").validator();

    function displayBalance() {
        var validator = $("#accountBalanceForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            let address = $("#accountBalanceAddress").val();
            let nodeUrl = getNodeUrl();
            let balanceConfirmations = $("#accountBalanceConfirmations").val();

            var requestUrl = nodeUrl + '/balance/' + address + '/' + balanceConfirmations;
            $.get(requestUrl, function (addressData) {
                var confirmed = `${addressData.confirmedBalance.balance} coins confirmed with ${addressData.confirmedBalance.confirmations} confirmations`;
                var lastMined = `${addressData.lastMinedBalance.balance} coins last mined with ${addressData.lastMinedBalance.confirmations} confirmations`;
                var pending = `${addressData.pendingBalance.balance} coins pending ${addressData.pendingBalance.confirmations} confirmations`;

                $("#balanceConfirmed").text(confirmed);
                $("#balanceLastMined").text(lastMined);
                $("#balancePending").text(pending);
            });
        }
    }

    /*
     * BLOCKS
     */

    function getBlocks() {
        getBlocksListPage(0, 7, function (data) {
            formatBlockListData(data.items);
            renderTemplateData(data.items, "#blck-tmpl", "#blocksPlaceHolder");
        });
    }

    function getTotalPages(data) {
        var pages = data.total / itemsListPageSize;
        if (data.total % itemsListPageSize !== 0) {
            pages += 1;
        }
        return pages;
    }

    function initBlockListPagination(data) {
        $('#blocksListPagination').bootpag({
            total: getTotalPages(data)
        }).on("page", function (event, pageNumber) {
            var pager = $(this)
            getBlocksListPage(pageNumber - 1, itemsListPageSize, function (data) {
                formatBlockListData(data.items);
                renderTemplateData(data.items, "#blck-list-tmpl", "#blocksListPlaceHolder");
                pager.bootpag({ total: getTotalPages(data) });
            });
        });
    }

    function getBlocksList(pageNumber) {
        if (typeof pageNumber === 'undefined')
            pageNumber = 0;
        getBlocksListPage(pageNumber, itemsListPageSize, function (data) {
            formatBlockListData(data.items);
            renderTemplateData(data.items, "#blck-list-tmpl", "#blocksListPlaceHolder");
            initBlockListPagination(data);
        });
    }

    function getBlocksListPage(pageNumber, pageSize, callback) {
        var url = getNodeUrl() + '/blocks?pageNumber=' + pageNumber + '&pageSize=' + pageSize;
        $.get(url, function (data) {
            callback(data);
        });
    }

    function formatBlockListData(items) {
        _.each(items, function (d) {
            var timeAgoString = moment(d.dateCreated).fromNow();
            d.dateCreated = timeAgoString;
            var blockHashLabel = d.blockHash.substring(0, 25) + '...';
            d.blockHashLabel = blockHashLabel;
            var previousBlockHashLabel = d.previousBlockHash.substring(0, 25) + '...';
            d.previousBlockHashLabel = previousBlockHashLabel;
            var minedByLabel = d.minedBy.substring(0, 25) + '...';
            d.minedByLabel = minedByLabel;
        });
    }

    function renderTemplateData(items, itemTemplateSelector, placeholderSelector) {
        var templateData = { target: items };
        var template = _.template($(itemTemplateSelector).text());
        $(placeholderSelector).html(template(templateData));
    }

    /*
     * TRANSACTIONS
     */

    function getTransactionsListPage(pageNumber, pageSize, callback, filter) {
        var url = getNodeUrl() + '/transactions?pageNumber=' + pageNumber + '&pageSize=' + pageSize;
        if (filter && filter !== '') {
            url += '&status=' + filter
        }

        $.get(url, function (data) {
            callback(data);
        });
    }

    function initTransactionsListPagination(data) {
        $("#transactionsListPagination").bootpag({
            total: getTotalPages(data)
        }).on("page", function (event, pageNumber) {
            var pager = $(this)
            getTransactionsListPage(pageNumber - 1, itemsListPageSize, function (data) {
                formatTransactionListData(data.items);
                renderTemplateData(data.items, "#trns-list-tmpl", "#transactionsListPlaceHolder");
                pager.bootpag({ total: getTotalPages(data) });
            });
        });
    }

    function formatTransactionListData(items) {
        _.each(items, function (d) {
            var timeAgoString = moment(d.dateCreated || new Date()).fromNow();
            d.dateCreated = timeAgoString;
            var fromLabel = (d.from || "").substring(0, 10) + '...';
            d.fromLabel = fromLabel;
            var toLabel = (d.to || "").substring(0, 10) + '...';
            d.toLabel = toLabel;
            var transactionHashLabel = (d.transactionHashHex || "").substring(0, 10) + '...';
            d.transactionHashLabel = transactionHashLabel;
        });
    }

    function getUnconfirmedTransactions() {
        getTransactionsListPage(0, 5, function (data) {
            formatTransactionListData(data.items);
            renderTemplateData(data.items, "#unconf-trns-tmpl", "#unconfirmedTransactionsPlaceHolder");
        }, "pending");
    }

    function getTransactions() {
        getTransactionsListPage(0, 5, function (data) {
            formatTransactionListData(data.items);
            renderTemplateData(data.items, "#trns-tmpl", "#transactionsPlaceHolder");
        });
    }

    function getTransactionsList(filter) {
        // TODO: Fix template to show full transaction hash or allow users to copy it

        getTransactionsListPage(0, itemsListPageSize, function (data) {
            formatTransactionListData(data.items);
            renderTemplateData(data.items, "#trns-list-tmpl", "#transactionsListPlaceHolder");
            initTransactionsListPagination(data);
        });
    }

    /*
     * PEERS
     */
    function getPeersNetwork() {
        var imagePath = "./images/pc-icon.png";
        var url = getNodeUrl() + '/peers/network';
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
                var options = {
                    layout: {
                        randomSeed: 25//so the layour stays the same
                    }
                };
                var network = new vis.Network(container, data, options);
            }
        });
    }

    function getNodeInfo() {
        var url = getNodeUrl() + '/info';
        $.get(url, function (nodeData) {
            $("#nodeAbout").text("Running on: " + nodeData.about);
            $("#nodeName").text("Connected to: " + nodeData.nodeName);
            $("#blockchainDifficulty").text("Difficulty: " + nodeData.difficulty);
        });
    }

    function getNodeUrl() {
        return $("#blockchainNodeUrl").val() + '/api';
    }

    /*
     * SEARCH
     */

    $('#buttonSearch').click(loadSearchSection);
    function loadSearchSection() {
        showView("searchResultsSection");
        $("#searchResultsPlaceHolder").text("No data found.");
        var searchText = $("#searchBox").val();
        if (searchText.length === 0)
            return;

        var transactionUrl = getNodeUrl() + '/transactions/' + searchText;
        $.get(transactionUrl, function (data) {
            setSearchResultData(data, 'transaction');
        }).fail(function () {
            tryGetBlockByHash(searchText);
        });
    }

    function tryGetBlockByHash(searchText) {
        var urlGetBlock = getNodeUrl() + '/blocks/hash/' + searchText;
        $.get(urlGetBlock, function (data) {
            setSearchResultData(data, 'block');
        }).fail(function () {
            tryGetBlockByIndex(searchText);
        });
    }

    function tryGetBlockByIndex(searchText) {
        if (!isNaN(searchText)) {
            var blockIndex = parseInt(searchText)
            var urlGetBlock = getNodeUrl() + '/blocks/index/' + blockIndex;
            $.get(urlGetBlock, function (data) {
                setSearchResultData(data, 'block');
            });
        }
    }

    function setSearchResultData(data, type) {
        $("#searchResultsPlaceHolder").html(JSON.stringify(data, undefined, 2));
    }

    $("#blockchainNodeForm").validator();
    $('#buttonSetNodeUrl').click(function () {
        var validator = $("#blockchainNodeForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            $("#explorerConfiguration").collapse('hide');
        }
        getNodeInfo();
    });

    $('#buttonHome').click(loadHomeSection);
    function loadHomeSection() {
        showView("homeSection");
        $(this).parent().addClass("active");

        getBlocks();
        getTransactions();
        getUnconfirmedTransactions();
    }

    $('#buttonBlocks').click(loadBlocksSection);
    let isBlockSectionInitialized = false;
    function loadBlocksSection() {
        showView("blocksSection");
        $(this).parent().addClass("active");

        if (!isBlockSectionInitialized) {
            isBlockSectionInitialized = true;
            getBlocksList();
        }
    }

    $('#buttonTransactions').click(loadTransactionsSection);
    let isTranscationSectionInitialized = false;
    function loadTransactionsSection() {
        showView("transactionsSection");
        $(this).parent().addClass("active");
        if (!isTranscationSectionInitialized) {
            isTranscationSectionInitialized = true;
            getTransactionsList();
        }
    }
    
    $('#buttonAccounts').click(loadAccountsSection);
    function loadAccountsSection() {
        showView("accountsSection");
        $(this).parent().addClass("active");
    }

    $('#buttonPeersNetwork').click(loadPeersMapSection);
    function loadPeersMapSection() {
        showView("peersMapSection");
        $(this).parent().addClass("active");

        getPeersNetwork();
    }

    //$(document).on({
    //    ajaxStart: function () { $("#loadingBox").show() },
    //    ajaxStop: function () { $("#loadingBox").hide() }
    //});

    function showView(viewName) {
        activeView = viewName;
        $('li.nav-item').removeClass("active");
        $('section').hide();
        $('#' + viewName).show();
    }
    $('#buttonHome').click();
    getNodeInfo();

    setInterval(refreshData, 10000);
    function refreshData() {
        var refreshViewMap = {
            //'searchResultsSection': loadSearchSection,
            'homeSection': loadHomeSection,
            //'blocksSection': loadBlocksSection,
            //'transactionsSection': loadTransactionsSection,
            //'accountsSection': loadAccountsSection
        }
        //this will repeat every 10 seconds
        if (refreshViewMap[activeView]) {
            refreshViewMap[activeView]();
        }
    }
});