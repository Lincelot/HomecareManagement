var EmployerData;
var count = 0;
var showRoute = false;
$(document).ready(function () {
    //初始化
    //set Equipment User ComboBox
    $(function () {
        $("#Supervisor-Map-User").kendoComboBox({
            filter: "Contains",
            height: 400,
            dataSource: {
                transport: {
                    read: {
                        type: "POST",
                        dataType: 'json',
                        contentType: "application/json; charset=utf-8",
                        url: hostname + "/Supervisor/getAllSupervisorData"
                    }
                }
            },
            dataTextField: "showName",
            dataValueField: "account_uid",
            change: onChange
        });
        function onChange(e) {
            var uid = $("#Supervisor-Map-User").data("kendoComboBox").value();
            showProgressBar(true);
            //ajax
            $(function () {
                $.ajax({
                    type: "POST",
                    url: hostname + "/Supervisor/getEmployerDataForSupervisorID",
                    dataType: "json",
                    data: JSON.stringify({
                        uid: uid
                    }),
                    async: true,
                    contentType: 'application/json; charset=utf-8',
                    success: function (result) {
                        EmployerData = result;
                        count = 0;
                    },
                    error: function () {
                        showAlert("error", true, serverErrorMessage);
                    },
                    complete: function () {
                        if (EmployerData != null) {
                            directionsDisplay.setPanel(null);
                            directionsDisplay.setMap(null);
                            deleteMarkers();
                            for (var i = 0; i < EmployerData.length; i++) {
                                (function (i) {
                                    getLatLngForAddress(EmployerData[i].address, function (res) {
                                        var LatLng = res[0].geometry.location;
                                        if (EmployerData[count].online == 0) {
                                            addMarker(EmployerData[count].displayname,
                                                        "地址：" + EmployerData[count].address,
                                                        LatLng,
                                                        "purple");
                                        } else {
                                            addMarker(EmployerData[count].displayname,
                                                        "地址：" + EmployerData[count].address,
                                                        LatLng,
                                                        "green");
                                        }
                                        count++;
                                    });
                                })(i);
                            }
                        }
                        showProgressBar(false);
                        $("#right-panel").css("display", "none");
                    }
                });
            });
        };
    });
});

//Google Map

var map;
var directionsDisplay;
var directionsService;
var markers = [];
var infowindows = [];
var nowLocation;

//初始化地圖
function initMap() {
    directionsDisplay = new google.maps.DirectionsRenderer;
    directionsService = new google.maps.DirectionsService;
    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 14,
        center: {
            //弘光科技大學
            lat: 24.2172724,
            lng: 120.5828439
        }
    });
    // Try HTML5 geolocation.
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            //設定地圖中心點為當前位置
            map.setCenter(pos);
            //儲存當前位置
            nowLocation = pos;

            //將經緯度轉換為地址
            getAddressForLatLng(nowLocation, function (res) {
                //加入標記_目前位置
                addMarker("目前位置", res[0].formatted_address + "<br />(請開啟GPS以增進精準度)", nowLocation, null);
            });
        }, function () {
            var infoWindow = new google.maps.InfoWindow({ map: map });
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        var infoWindow = new google.maps.InfoWindow({ map: map });
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());
    }
    //不支援定位
    function handleLocationError(browserHasGeolocation, infoWindow, pos) {
        infoWindow.setPosition(pos);
        infoWindow.setContent(browserHasGeolocation ?
                              'Error: The Geolocation service failed.' :
                              'Error: Your browser doesn\'t support geolocation.');
    }
}

//顯示路線
function calculateAndDisplayRoute(start, end) {
    showRoute = true;
    directionsDisplay.setPanel(null);
    directionsDisplay.setMap(null);
    var start = start;
    var end = end;
    directionsService.route({
        origin: start,
        destination: end,
        travelMode: google.maps.TravelMode.DRIVING
    }, function (response, status) {
        if (status === google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
            //console.log(response.routes[0].legs[0].distance.text);
            directionsDisplay.setMap(map);
            directionsDisplay.setPanel(document.getElementById("right-panel"));
            $("#right-panel").css("display", "block");
        } else {
            window.alert("Directions request failed due to " + status);
        }
    });
}

// Adds a marker to the map and push to the array.
function addMarker(title, content, location, icon) {
    var img;
    switch (icon) {
        case "blue": {
            img = hostname + "/Images/marker/blue.png";
            break;
        }
        case "green": {
            img = hostname + "/Images/marker/green.png";
            break;
        }
        case "purple": {
            img = hostname + "/Images/marker/purple.png";
            break;
        }
        case "red": {
            img = hostname + "/Images/marker/red.png";
            break;
        }
        case "yellow": {
            img = hostname + "/Images/marker/yellow.png";
            break;
        }
        case null: {
            img = null;
            break;
        }
    }
    var marker = new google.maps.Marker({
        position: location,
        draggable: true,
        animation: google.maps.Animation.DROP,
        map: map,
        icon: img
    });
    //set Animation
    marker.setAnimation(google.maps.Animation.BOUNCE);
    //訊息視窗
    var contentString = "<h4>" + title + "</h4>" + content;
    var infowindow = new google.maps.InfoWindow({
        content: contentString
    });
    //set Click Listener for Animation
    marker.addListener('click', function () {
        // Marker Animations
        if (marker.getAnimation() !== null) {
            marker.setAnimation(null);
            if (icon != null) {
                hideInfowindows();
                setMarkersAnimationOnAll();
                calculateAndDisplayRoute(nowLocation, location);
            }
            //開啟訊息窗
            infowindow.open(map, marker);
        } else {
            marker.setAnimation(google.maps.Animation.BOUNCE);
            //關閉訊息窗
            infowindow.close();
        }
    });
    markers.push(marker);
    infowindows.push(infowindow);
    if (icon != null) {
        setFitBounds();
    }
}

// Sets the map on all markers in the array.
function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    setMapOnAll(null);
}

// Shows any markers currently in the array.
function showMarkers() {
    setMapOnAll(map);
}

// Deletes all markers in the array by removing references to them.
function deleteMarkers() {
    clearMarkers();
    markers = [];
    getAddressForLatLng(nowLocation, function (res) {
        //加入標記_目前位置
        addMarker("目前位置", res[0].formatted_address + "<br />(請開啟GPS以增進精準度)", nowLocation, null);
    });
}

// 隱藏所有訊息視窗
function hideInfowindows() {
    for (var i = 0; i < infowindows.length; i++) {
        infowindows[i].close();
    }

}

// 讓所有Marker動
function setMarkersAnimationOnAll() {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setAnimation(google.maps.Animation.BOUNCE);
    }
}

// 調整為最佳大小
function setFitBounds() {
    //  Create a new viewpoint bound
    var bounds = new google.maps.LatLngBounds();
    //  Go through each...
    for (var i = 0; i < markers.length; i++) {
        //  And increase the bounds to take this point
        bounds.extend(markers[i].position);
    }
    //  Fit these bounds to the map
    map.fitBounds(bounds);
}

// 將經緯度轉換為地址
getAddressForLatLng = function (LatLng, f) {
    var geocoder = new google.maps.Geocoder();
    // 傳入 latLng 資訊至 geocoder.geocode
    if (typeof LatLng != 'undefined' && LatLng != null) {
        geocoder.geocode({ latLng: LatLng, }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                f(results);
            }
        });
    }
    return -1;
}

// 將地址轉換為經緯度
getLatLngForAddress = function (address, f) {
    var geocoder = new google.maps.Geocoder();
    // 傳入 address 資訊至 geocoder.geocode
    if (typeof address != 'undefined' && address != null) {
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                f(results);
            }
        });
    }
    return -1;
}