angular.module("umbraco.resources")
        .factory("Our.Umbraco.Nexu.Resource", function ($http) {
            return {
                getIncomingLinks: function (id) {
                    return $http.get(Umbraco.Sys.ServerVariables.Nexu.GetIncomingLinks + "?contentId=" + id);
                },
                getRebuildStatus : function() {
                    return $http.get(Umbraco.Sys.ServerVariables.Nexu.GetRebuildStatus);
                }
            };
        });