new function () {
    var preLoadTime = 0;
    var preLoadCount = 0;
    var preLoadError = 0;
    var preLoadFinish = 0;
    var preLoadPercent = 0;
    var preLoadStart = 0;
    var preLoadTotal = 0;
    var preLoadLoaded = 0;
    var preLoadCLength = 0;
    var preLoadSampleLoaded = 0;
    var preLoadSampleCLength = 0;
    function preLoadUpdateUI() {
        var progressbar = document.getElementById("progressbar");
        if (progressbar) {
            var p = preLoadFinish / preLoadCount;
            if (preLoadTotal) {
                p = preLoadLoaded / preLoadTotal;
            }
            else if (preLoadSampleLoaded) {
                var ratio = preLoadSampleCLength / preLoadSampleLoaded;
                var p2 = Math.min(1, (preLoadLoaded * ratio / preLoadCLength) * (preLoadStart / preLoadCount));
                p = (p + p2) / 2;
            }
            preLoadPercent = Math.max(preLoadPercent, p);
            progressbar.innerHTML = "<span style='position:absolute;left:0;background-color:darkgreen;height:10px;border-radius:5px;width:" + (progressbar.offsetWidth * preLoadPercent) + "px'></span>";
        }
    }
    function preLoadResource(dllname) {
        preLoadCount++;
        var xh = new XMLHttpRequest();
        xh.open("GET", dllname, true);
        var loaded = 0;
        var total = 0;
        var clength = 0;
        xh.onprogress = function (e) {
            if (!e.loaded) return;
            if (loaded == 0) {
                preLoadStart++;
                clength = parseInt(xh.getResponseHeader("Content-Length"));
                total = e.total;
                preLoadCLength += clength;
                preLoadTotal += total;
            }
            preLoadLoaded += e.loaded - loaded;
            loaded = e.loaded;
            preLoadUpdateUI();
        }
        xh.onload = function () {
            if (loaded && clength) {
                preLoadSampleLoaded += loaded;
                preLoadSampleCLength += clength;
            }
            preLoadFinish++;
            if (xh.status != 200) preLoadError++;
            //console.log(preLoadFinish + "/" + preLoadCount, clength / loaded, dllname);
            if (preLoadFinish == preLoadCount) {
                var span = new Date().getTime() - preLoadTime;
                console.log("All Done In " + span + " ms , " + preLoadError + " errors");
            }
        }
        xh.send("");
    }
    function preLoadAll() {
        preLoadTime = new Date().getTime();
        var xh = new XMLHttpRequest();
        xh.open("GET", "_framework/blazor.boot.json", true);
        xh.onload = function () {
            var res = JSON.parse(xh.responseText);
            console.log(res);
            var arr = [];
            function moveFront(part) {
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].indexOf(part) != -1) {
                        arr.unshift(arr.splice(i, 1)[0]);
                        break;
                    }
                }
            }

            arr.push("_framework/blazor.webassembly.js");
            for (var p in res.resources.runtime)
                arr.push("_framework/" + p);
            for (var p in res.resources.assembly)
                arr.push("_framework/" + p);

            moveFront("System.Core.dll");
            moveFront("System.Data.dll");
            moveFront("System.dll");
            moveFront("System.Xml.dll");
            moveFront("mscorlib");
            moveFront("dotnet.wasm");
            arr.forEach(preLoadResource);
            //console.log(arr);
        }
        xh.send("");
    }
    preLoadAll();
}