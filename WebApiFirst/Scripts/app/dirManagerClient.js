(function () {
	'use strict';

	angular
		.module('myapp')
		.factory('dirManagerClient', dirManagerClient);

	dirManagerClient.$inject = ['$resource'];

	function dirManagerClient($resource) {
		return $resource("api/getdirectories/:id",
				{ id: "@id" },
				{
					'query': { method: 'GET' },
					//'save': { method: 'POST', transformRequest: angular.identity, headers: { 'Content-Type': undefined } },
					//'remove': { method: 'DELETE', url: 'api/photo/:fileName', params: { name: '@fileName' } }
				});
	}
})();