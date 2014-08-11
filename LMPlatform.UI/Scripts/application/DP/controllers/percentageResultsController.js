﻿angular
    .module('dpApp.ctrl.percentageResults', ['ngTable', 'ngResource'])
    .controller('percentageResultsCtrl', [
        '$scope',
        '$modal',
        'ngTableParams',
        '$resource',
        '$location',
        function ($scope, $modal, ngTableParams, $resource, $location) {

            $scope.setTitle("Результаты процентовки");

            var percentageResults = $resource('api/PercentageResult');
            var studentMarks = $resource('api/StudentMark');

            $scope.percentages = [];

            $scope.getPercentageResult = function (student, percentageGraphId) {

                var results = student.PercentageResults;

                for (var i = 0; i < results.length; i++) {
                    if (results[i].PercentageGraphId == percentageGraphId) {
                        return results[i];
                    }
                }
                return {
                    StudentId: student.Id,
                    PercentageGraphId: percentageGraphId
                };
            };


            $scope.savePercentageResult = function (percentageResult, newValue) {

                if (newValue && (isNaN(newValue, 10) || newValue < 1 || newValue > 10)) return "Введите число от 1 до 10!";

                percentageResult.Mark = newValue;

                percentageResults.save(percentageResult)
                    .$promise.then(function (data, status, headers, config) {
                        $scope.tableParams.reload();
                        alertify.success('Оценка успешно сохранена.');
                    }, $scope.handleError);
                return false;
            };


            $scope.saveStudentMark = function (assignedDiplomProjectId, newValue) {

                if (newValue && (isNaN(newValue, 10) || newValue < 1 || newValue > 10)) return "Введите число от 1 до 10!";


                studentMarks.save([assignedDiplomProjectId, newValue])
                    .$promise.then(function (data, status, headers, config) {
                        $scope.tableParams.reload();
                        alertify.success('Оценка успешно сохранена.');
                    }, $scope.handleError);
                return false;
            };


            $scope.tableParams = new ngTableParams(
                angular.extend({
                    page: 1,
                    count: 10
                }, $location.search()), {
                    total: 0,
                    getData: function ($defer, params) {
                        $location.search(params.url());
                        percentageResults.get(params.url(),
                            function (data) {
                                $defer.resolve(data.Students.Items);
                                params.total(data.Students.Total);
                                $scope.percentages = data.PercentageGraphs;
                            });
                    }
                });
        }]);