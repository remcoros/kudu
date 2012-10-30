function parseMSJSON(data, secure) {
    return Sys.Serialization.JavaScriptSerializer.deserialize(data, secure);
}

function wrapFunction(fn, superFn, base, baseConstructor) {
    return function () {
        var oldBase = this.base, oldBaseConstructor = this.baseConstructor, oldSuper = this._base;
        this.base = base;
        this.baseConstructor = baseConstructor;

        this._base = function () {
            return superFn.apply(this, Array.prototype.slice.call(arguments, 0));
        };

        try {
            return fn.apply(this, Array.prototype.slice.call(arguments, 0));
        }
        finally {
            this.base = oldBase;
            this.baseConstructor = oldBaseConstructor;
            this._base = oldSuper;
        }
    };
};

function extendMembers(target, source, base, baseConstructor) {
    var propName, prop;
    for (propName in source) {

        if (source.hasOwnProperty(propName)) {
            prop = source[propName];

            if (base && typeof prop === "function" && typeof base[propName] === "function") {
                target[propName] = wrapFunction(prop, base[propName], base, baseConstructor);
            }
            else {
                target[propName] = prop;
            }
        }
    }

    return target;
};

function delegate(instance, method, data) {
    return function () {
        if (typeof (data) === "undefined") {
            return method.apply(instance, arguments);
        }
        else {
            var args = Array.prototype.slice.apply(arguments, [0]);

            if (data instanceof Array) {
                args = args.concat(data);
            }
            else {
                args.push(data);
            }

            return method.apply(instance, args);
        }
    };
}

function msJSONFilter(data, type) {
    var parsedData = parseMSJSON(data, false);

    //TFS.Diag.assert(!$.isArray(parsedData), "Received JSON data from the server which is an array.  Sending JSON arrays from the server leaves us open to JSON Hijacking attacks.  Ensure that the server code is making use of the SecureJsonResult class.");

    // If the data object has a wrapped array property, then unwrap the array.
    // The SecureJsonResult on the server will wrap any arrays that are being sent
    // back to the client in an object to prevent JSON Hijacking attacks.  To make
    // it this transparent to the code consuming the results on the client, we
    // unwrap the array here.
    if (parsedData.hasOwnProperty("__wrappedArray")) {
        parsedData = parsedData.__wrappedArray;
    }

    return parsedData;
}

function getMSJSON(url, data, callback, errorCallback, ajaxOptions) {

        // Perform any pre work needed for the reuqest.
    jQuery.ajax($.extend({
        type: "GET",
        url: url,
        data: data,
        success: callback,
        error: errorCallback,
        dataType: "json",       
        //                    dataFilter: msJSONFilter,
        converters: { "text json": msJSONFilter },  // We convert in dataFilter
        traditional: true,
        timeout: 300000
    }, ajaxOptions));
}

Function.prototype.extend = function (staticMembers) {
    /// <summary>Adds static members to functions.</summary>
    /// <param name="staticMembers" type="Object">An object which properties ultimately be static members of the function.</param>
    return extendMembers(this, staticMembers, this.base, this.baseConstructor);
};

var defaultTfsContext;

function TfsContext(contextData) {

    this.baseUrl = contextData.baseUrl;
    this.token = contextData.token;
}


TfsContext.extend({
    _DEFAULT_CONTROLLER_NAME: "home",
    _DEFAULT_ACTION_NAME: "index",

    parseContext: function ($element) {

        // Getting the JSON string serialized by the server according to the current host
        var contextElement, json;

        contextElement = $element.find(".tfs-context");

        if (contextElement.length > 0) {
            json = contextElement.eq(0).html();

            if (json) {
                return new TfsContext(parseMSJSON(json, false));
            }
        }

        return null;
    },

    getDefault: function () {
        if (!defaultTfsContext) {
            var context = this.parseContext($(document));

            // Parsing the JSON and having it in the options
            defaultTfsContext = context;
        }

        return defaultTfsContext;
    }

});

TfsContext.prototype = {
    baseUrl: null,
    token: null
};

function WorkItemManager() {

}

WorkItemManager.prototype = {
    beginGetWorkItemData: function (id, callback, errorCallback, options) {
        var that = this,  e;

        

        getMSJSON("/wit/workitems", { id: id }, function (workItemPayload) {
            if ($.isFunction(callback)) {
                if (workItemPayload && workItemPayload.length === 1) {
                    callback.call(that, workItemPayload[0], errorCallback, options);
                }
                else {
                    callback.call(that, workItemPayload, errorCallback, options);
                }
            }
        }, errorCallback);
    },
    beginSetWebHookUrl: function(url, callback, errorCallback, options) {
        var that = this,  e;



        getMSJSON("/wit/SetWebHookUrl", { url: url }, function(workItemPayload) {
            if ($.isFunction(callback)) {
                if (workItemPayload && workItemPayload.length === 1) {
                    callback.call(that, workItemPayload[0], errorCallback, options);
                }
                else {
                    callback.call(that, workItemPayload, errorCallback, options);
                }
            }
        }, errorCallback);
    }
};

function SearchControl() {
    this.init();
}

SearchControl.prototype = {
    _element: null,
    $searchText: null,
    $searchButton: null,
    $searchInfo: null,
    _workItemManager: null,
    init: function() {
        var that = this, id, e;
        this._element = $('.search-container');
        this.$searchButton = this._element.find('#btnSearch');
        this.$searchButton.bind('click', delegate(this, this.onSearch));
        this.$searchText = this._element.find('.search-id');
        this.$searchInfo = this._element.find('.search-result');

        this._webHookUrlelement = $('.WebHookUrl-container');
        this.$webHookUrlButton = this._webHookUrlelement.find('#btnWebHookUrl');
        this.$webHookUrlButton.bind('click', delegate(this, this.onSetWebHookUrl));
        this.$webHookUrlText = this._webHookUrlelement.find('.WebHookUrl-id');
        this.$webHookUrlInfo = this._webHookUrlelement.find('.WebHookUrl-result');

        this._workItemManager = new WorkItemManager();
    },
    onSearch: function() {
        this.$searchInfo.text('searching...');
        this._workItemManager.beginGetWorkItemData(this.$searchText.val(), delegate(this, this.onSearchComplete), delegate(this, this.onSearchError));
    },
    onSearchComplete: function(data) {
        if (data.success === false) {
            this.$searchInfo.text('Error: ' + data.message);
        } else {
            this.$searchInfo.text('Title: ' + data.fields[1]);
        }
    },
    onSearchError: function(e) {
        this.$searchInfo.text('search failed');
    },
    onSetWebHookUrl: function() {
        this._workItemManager.beginSetWebHookUrl(this.$webHookUrlText.val(), delegate(this, this.onSetWebHookUrlComplete), delegate(this, this.onSetWebHookUrlError));
    },
    onSetWebHookUrlComplete: function(data) {
        alert("Success!");
//        if (data.success === false) {
//            this.$searchInfo.text('Error: ' + data.message);
//        } else {
//            this.$searchInfo.text('Title: ' + data.fields[1]);
//        }
    },
    onSetWebHookUrlError: function(e) {
        alert("Fail with " + e.message);
    }
};