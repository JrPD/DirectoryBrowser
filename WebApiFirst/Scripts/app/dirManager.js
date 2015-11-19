
(function() {
	'use strict';

	function fillInFields($scope, response) {
		// list of items
		$scope.directories = response;
		$scope.filesSmall = response[0].FilesSmall;
		$scope.filesMedium = response[0].FilesMedium;
		$scope.filesHeight = response[0].FilesHeight;
	}

	function browse(dirs, $scope, $http) {

		var uri = "/api/directories/";

		// getting data from server
		$http({
					method: 'GET',
					url: uri,
					params: { dir: JSON.stringify(dirs) }
				}
			)
			.success(function (response) {
				fillInFields($scope, response);
			})
			.error(function(response) {
				alert("ERROR");
			});
	}

	// controller function
	function dirCtrl($scope, $http) {
		$scope.directories = [];
		$scope.filesSmall = 0;
		$scope.filesMedium = 0;
		$scope.filesHeight = 0;
		$scope.currentDir = "";

		var uri = "/api/directories";

		// getting list of dirs and files
		$http.get(uri)
			.success(function (response) {

				$scope.currentDir = response[0].Path;
				fillInFields($scope, response);
			})
			.error(function(response) {
				alert(response);
			});

		function click(a) {
			// split path to post data to server
			var path = a.x.Path.split("\\");

			//if (a.x.Path.indexOf(".") !== -1) {
			//	alert(a.x.Path.split("\\").pop() + " IS A FILE!");
			//}
			//else {
				// load subcategories
				browse(path, $scope, $http);
				$scope.currentDir = a.x.Path;
			//}
		}

		$scope.click = click;
	}

	angular
		.module('myapp', [])
		.controller('dirCtrl', dirCtrl);

		dirCtrl.$inject = ['$scope', '$http'];
})();