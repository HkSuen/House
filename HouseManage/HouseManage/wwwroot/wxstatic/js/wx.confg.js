var wxCon = function () {
    this._mui = mui;
    this._$ = $;
    this._wx = wx;
    this.Init();
};
wxCon.prototype = {
    constructor: wxCon,
    _config: {},
    ajax: function (url, data, callback, async, type, dataType) {
        async = (async == null || async == true) ? true : false;
        mui.showLoading();
        this._mui.ajax({
            type: type || "POST",
            url: url,
            data: data,
            async: async,
            dataType: dataType || "json",
            success: function (data) {
                mui.hideLoading();
                if (typeof (callback) == "function") {
                    if (data.code == 1000) {
                        callback(data.data || null);
                    } else {
                        if (data.msg != null && data.msg != undefined) {
                            this._mui.alert(data.msg);
                        }
                    }
                }
            }.bind(this)
        });
    },
    IsFunc: function (fun) {
        if (typeof (fun) != "function") {
            console.log("err:method paramter meed function type.");
            return false;
        };
        return true;
    },
    Init: function () {
        this.ajax("/WeChat/Home/SDKCON", { url: encodeURIComponent(location.href.split('#')[0]) }, function (data) {
            if (data != null) {
                this._config = data;
                this.registerConfig(data);
            }
        }.bind(this));
        return this;
    },
    registerConfig: function (config) {
        console.log("registerConfig");
        this._wx.config(config);
    },
    takePicture: function (callback) {
        this._wx.chooseImage({
            count: 3,
            //needResult: 3,
            sizeType: ['compressed'], // 可以指定是压缩图，默认二者都有  
            sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有  
            success: function (data) {
                if (data.localIds.length > 0) {
                    if (this.IsFunc(callback)) {
                        callback(data.localIds); //图片选择后方法
                    } else {
                        console.log("err:takePicture paramter type is function");
                    }
                } else {
                    console.log("err: not anything.");
                }
            }.bind(this),
            fail: function (res) {
                alert("take pictrue of fail" + JSON.stringify(res));
                //alterShowMessage("操作提示", JSON.stringify(res), "1", "确定", "", "", "");
            }
        });
    },
    previewImage: function (urls, cur) {
        this._wx.previewImage({
            current: cur || '', // 当前显示图片的http链接
            urls: urls // 需要预览的图片http链接列表
        });
    },
    getLocalImg: function (localId, callback) {
        this._wx.getLocalImgData({
            localId: localId, // 图片的localID
            success: function (res) {
                if (this.IsFunc(callback)) {
                    var localData = res.localData;
                    if (localData != "" && localData != undefined) {
                        if (localData.indexOf('data:image') != 0) {
                            //判断是否有这样的头部                                               
                            localData = 'data:image/jpeg;base64,' + localData
                        }
                        localData = localData.replace(/\r|\n/g, '').replace('data:image/jgp', 'data:image/jpeg')
                        callback(localData);
                    }
                }
            }.bind(this)
        });
    },
    taskLocalPicData: function (localcallback,callback) {
        this.takePicture(function (pic) {
            if (this.IsFunc(callback)) {
                callback(pic);
            }
            if (this.IsFunc(localcallback)) {
                this._$.each(pic, function (idx, url) {
                    this.getLocalImg(url, function (img) {
                        localcallback(idx,url,img);
                    })
                }.bind(this))
            }
        }.bind(this))
    }
}
//务必实例化
//此类依赖于 mui.js and jweixin.js and jquery.js
//实例化案例： var wcon = new wxCon();