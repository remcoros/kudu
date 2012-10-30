(function () {
    if (!window.tfsOAuth) {

        window.tfsOAuth = {
            login: function () {
                var clientId = "41e13c5e-7b7e-47d5-8212-bc512cfd4650";

                var width = 640, height = 480;
                var popupWindowFeatures = [
                    "width=" + width,
                    "height=" + height,
                    "top=" + window.screenTop + (document.documentElement.clientHeight - height) / 2 - 30, //NON-IE: + window.screenY + (window.outerHeight - height) / 2,
                    "left=" + window.screenLeft + (document.documentElement.clientWidth - width) / 2, //NON-IE: + window.screenX + (window.outerWidth - width) / 2,
                    "status=no",
                    "resizable=yes",
                    "toolbar=no",
                    "menubar=no",
                    "scrollbars=yes"];
                // TODO: replace with GIT-HUB
                var popup = window.open("https://cscro01.madhurig.tfsallin.net/_oauth/Authorize?client_id=" + clientId + "&response_type=code", "tfsOAuth", popupWindowFeatures.join());
            },

            logout: function () {

            }
        };
    }
})();