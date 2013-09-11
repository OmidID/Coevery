﻿'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'CreateOneToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', '$http','$parse',
            function ($scope, logger, $detour, $stateParams, $http, $parse) {

                $scope.recordDeleteBehavior = 'CascadingDelete';
                
                $scope.$watch('required', function (newValue) {
                    if (newValue && $scope.recordDeleteBehavior == 'NoAction') {
                        $scope.recordDeleteBehavior = 'CascadingDelete';
                    }
                });

                var validator = $("#onetomany-form").validate({
                    errorClass: "inputError"
                });

                $scope.save = function () {
                    $("input.primary-entity").prop('disabled', false);
                    var form = $('#onetomany-form');                  
                    if (!validator.form()) {
                        return null;
                    }
                    var promise = $http({
                        url: form.attr('action'),
                        method: form.attr('method'),
                        data: form.serialize(),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).then(function (response) {
                        return response;
                        logger.success('success');
                        $("input.primary-entity").prop('disabled', true);
                    }, function (result) {
                        logger.error('Failed:\n' + result.responseText);
                        $("input.primary-entity").prop('disabled', true);
                    });
                    return promise;
                };

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };


                $scope.saveAndView = function () {
                    var promise = $scope.save();
                    promise.then(function (response) {
                        var getter = $parse('relationId');
                        var relationId = getter(response.data);
                        if (relationId)
                            $detour.transitionTo('EditOneToMany', { EntityName: $stateParams.EntityName, RelationId: relationId });
                    });
                };
                
                
                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise && promise.then(function () {
                        $scope.exit();
                    });
                };
            }]
    ]);
});