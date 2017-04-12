var head = "/otv";

//账户信息修改
function infoModify() {
    var email = $("#Email").val();
    var desc = $("#Desc").val();
    var user = $("#User").val();
    var pattern = /^[a-z0-9A-Z\._%+-]+@[a-z0-9A-Z\._%+-]+\.([a-zA-Z]){2,4}$/;
    if (!pattern.test(email)) {
        layer.msg("邮箱地址非法", {
            icon: 5,
            time: 3000
        });
        $("#Email").focus();
        return;
    }
    if (desc.length == 0) {
        layer.msg("简介不能为空", {
            icon: 5,
            time: 3000
        });
        $("#Desc").focus();
        return;
    }

    layer.msg('数据提交中...', { icon: 16 });
    $.ajax({
        url: head+"/Manager/Update",
        type: "POST",
        dataType: "json",
        data: { User: user, Email: email, Desc: desc },
        success: function (res) {
            layer.closeAll();
            if (res == "ok") {
                layer.msg('修改成功', {
                    icon: 1,
                    time: 3000
                });
            } else {
                layer.msg(res, {
                    icon: 2,
                    time: 3000
                });
            }
        },
        error: function (data) {
            layer.closeAll();
            layer.msg("服务器繁忙！", {
                icon: 5,
                time: 3000
            });
        }
    });
}

//账户密码修改
function resetPwd() {
    var oldPwd = $("#OldPwd").val();
    var newPwd = $("#NewPwd").val();
    var preNewPwd = $("#PreNewPwd").val();
    var user = $("#User").val();

    if (oldPwd.length == 0) {
        layer.msg("原密码为空", {
            icon: 5,
            time: 3000
        });
        $("#OldPwd").focus();
        return;
    }

    var pattern = /^[a-z0-9A-Z_-]{6,12}$/;
    if (!pattern.test(newPwd)) {
        layer.msg("密码必须是6-12位大小写字母、数字及下划线", {
            icon: 5,
            time: 3000
        });
        $("#NewPwd").focus();
        return;
    }

    if (newPwd.length == 0) {
        layer.msg("新密码为空", {
            icon: 5,
            time: 3000
        });
        $("#NewPwd").focus();
        return;
    }

    if (newPwd != preNewPwd) {
        layer.msg("两次密码输入不一致", {
            icon: 5,
            time: 3000
        });
        $("#NewPwd").focus();
        return;
    }

    layer.msg('数据提交中...', { icon: 16 });
    $.ajax({
        url: head+"/Manager/ResetPwd",
        type: "POST",
        dataType: "json",
        data: { User: user, OldPwd: oldPwd, NewPwd: newPwd, PreNewPwd: preNewPwd },
        success: function (res) {
            layer.closeAll();
            if (res == "ok") {
                layer.msg('密码修改成功', {
                    icon: 1,
                    time: 3000
                });
            } else {
                layer.msg(res, {
                    icon: 2,
                    time: 3000
                });
            }
        },
        error: function (data) {
            layer.closeAll();
            layer.msg("服务器繁忙！", {
                icon: 5,
                time: 3000
            });
        }
    });
}

//节目热度定时设置
function chnQuartz() {
    var h = $("#hour").val();
    var m = $("#minute").val();
    layer.msg('数据提交中...', { icon: 16 });
    $.ajax({
        url: head + "/Channel/HotChnResetQuartz",
        type: "post",
        data: { hour: h, minute: m },
        success: function (res) {
            layer.closeAll();
            if (res == "ok") {
                layer.msg('设置成功', {
                    icon: 1,
                    time: 3000
                }, function () {
                    $("#timer").html(h + ":" + m);
                });
            } else {
                layer.msg(res, {
                    icon: 2,
                    time: 3000
                });
            }
        },
        error: function (data) {
            layer.closeAll();
            layer.msg("服务器繁忙！", {
                icon: 5,
                time: 2000 //1秒关闭（如果不配置，默认是3秒）
            });
        }

    });
}

//获取节目热度列表
function getHotChns() {
    $("#chn_jqGrid").jqGrid({
        url: head + "/Channel/getChnList",
        mtype: "POST",
        datatype: "json",
        colModel: [
            { label: '编号', name: 'Cuid', width: 150, searchoptions: { sopt: ['eq', 'bw', 'cn', 'nc'] }, align: 'center' },
            { label: '节目信息', name: 'ChnInfo', width: 150, searchoptions: { sopt: ['eq', 'bw', 'cn', 'nc'] }, align: 'center' },
            { label: '热度值', name: 'HeatValue', width: 150, searchoptions: { sopt: ['le', 'ge'] }, align: 'center' },
            { label: '区域', name: 'Area', width: 150, searchoptions: { sopt: ['eq', 'bw'] }, align: 'center' },
        ],
        viewrecords: true,
        width: 1000,
        height: 550,
        rowNum: 20,
        rowList: [20, 30, 50],
        pager: "#chn_jqGridPager",
        jsonReader: {
            repeatitems: false
        },
        caption: "节目热度统计",
        rownumbers: true,
    }).navGrid("#chn_jqGridPager",
        { add: false, edit: false, del: false, search: true, refresh: true, view: true, position: "left", cloneToTop: true });
}

//获取弹幕数据列表
function getDanmuData() {
    $("#dm_jqGrid").jqGrid({
        url: head + "/Record/getDmList",
        mtype: "POST",
        datatype: "json",
        colModel: [
            { label: '机器编号', name: 'Uid', width: 150, searchoptions: { sopt: ['eq', 'bw', 'cn', 'nc'] }, align: 'center' },
            { label: '节目编号', name: 'Cuid', width: 150, searchoptions: { sopt: ['eq', 'bw', 'cn', 'nc'] }, align: 'center' },
            { label: '内容', name: 'Content', width: 150, searchoptions: { sopt: ['bw', 'cn', 'nc'] }, align: 'center' },
            { label: 'IP', name: 'IP', width: 150, search: false, sortable: false,align: 'center' },
            {
                label: '访问时间',
                name: 'Date',
                width: 150,
                sorttype: 'date',
                searchoptions: {
                    // dataInit is the client-side event that fires upon initializing the toolbar search field for a column
                    // use it to place a third party control to customize the toolbar
                    dataInit: function (element) {
                        $(element).datepicker({
                            id: 'orderDate_datePicker',
                            dateFormat: 'yy-mm-dd',
                            //minDate: new Date(2010, 0, 1),
                            maxDate: new Date(2020, 0, 0),
                            showOn: 'focus'
                        });
                    },
                    sopt: ['ge', 'le']
                },
                align: 'center'
            }
        ],
        viewrecords: true,
        width: 1000,
        height: 550,
        rowNum: 20,
        rowList: [20, 30, 50],
        pager: "#dm_jqGridPager",
        jsonReader: {
            repeatitems: false
        },
        caption: "弹幕记录",
        rownumbers: true,
    }).navGrid("#dm_jqGridPager",
        { add: false, edit: false, del: false, search: true, refresh: true, view: true, position: "left", cloneToTop: true });
}

//获取过滤数据列表
function createStateElement(value, editOptions) {
    
    var span = $("<span />");
    var checkbox;
    if (value.indexOf('checked="checked"')!=-1)
        checkbox= $("<input>", { type: "checkbox", name: "State", id: "State", checked: true });
    else
        checkbox = $("<input>", { type: "checkbox", name: "State", id: "State", checked: false });
    span.append(checkbox);

    return span;
}

function getStateValue(elem, oper, value) {
    //if (oper === "set") {
    //    var checkbox = $(elem).find("input:radio[value='" + value + "']");
    //    checkbox.prop("checked", true);
    //}

    //if (oper === "get") {
    //    return $(elem).find("input:checkbox:checked").checked;
    //}

    alert($(elem).find("input:checkbox").attr("checked") + "\n" + oper + "\n" + value);
    return $(elem).find("input:checkbox:checked").attr("checked");
}
function getFiltersData() {
    $("#filter_jqGrid").jqGrid({
        url: head + "/Filter/getFilteList",
        editurl: head + "/Filter/Manage",
        mtype: "POST",
        datatype: "json",
        colModel: [
            {
                label: '过滤词', name: 'Value', width: 150, searchoptions: { sopt: ['bw', 'cn', 'nc'] }, align: 'center',
                editable: true,
                formoptions: {
                    elmsuffix: "(必填字段)" // the suffix to show after that
                }, editrules: {
                    custom_func: function (value, column) {
                        if (value.length < 0)
                            return [false, "内容不能为空！"];
                        else if (value.length > 30) {
                            return [false, "内容长度不能大于30！"];
                        } else
                            return [true, ""];
                    },
                    custom: true,
                    required: true
                },
            },
            {
                label: '状态', name: 'State', width: 150,
                editable: true,
                edittype: 'checkbox',
                ///edittype: "custom",
                //editoptions: {
                //    custom_element: createStateElement,
                //    custom_value: getStateValue,
                //},
                formatter: function (v, x, r) {
                    if(v)
                        return "<input type='checkbox' checked='checked' disabled='disabled'/>";
                    else
                        return "<input type='checkbox' disabled='disabled'/>";
                },
                search: false, sortable: false, align: 'center',
            },
            {
                label: '创建日期',
                name: 'Date',
                width: 150,
                sorttype: 'date',
                editable: false,
                searchoptions: {
                    // dataInit is the client-side event that fires upon initializing the toolbar search field for a column
                    // use it to place a third party control to customize the toolbar
                    dataInit: function (element) {
                        $(element).datepicker({
                            id: 'orderDate_datePicker',
                            dateFormat: 'yy-mm-dd',
                            //minDate: new Date(2010, 0, 1),
                            maxDate: new Date(2020, 0, 0),
                            showOn: 'focus'
                        });
                    },
                    sopt: ['ge', 'le']
                },
                align: 'center'
            }
        ],
        viewrecords: true,
        width: 1000,
        height: 550,
        rowNum: 20,
        rowList: [20, 30, 50],
        pager: "#filter_jqGridPager",
        jsonReader: {
            repeatitems: false
        },
        caption: "过滤列表",
        rownumbers: true,
        multiselect: true,
    }).navGrid("#filter_jqGridPager",
        { add: true, edit: true, del: false, search: true, refresh: true, view: true, position: "left", cloneToTop: true},
            // options for the Edit Dialog
        {
            editCaption: "编辑过滤条目",
            top: "150",
            left: "150",
            jqModal: false,
            closeAfterEdit:true,
            reloadAfterSubmit: true,
            afterSubmit: function (response, postdata) {
                var msg = response.responseText;
                if (msg.indexOf("ok") != -1) {
                    layer.msg("编辑成功", {
                        icon: 1,
                        time: 1000 //1秒关闭（如果不配置，默认是3秒）
                    });
                    return [true, ""];
                } else {
                    return [false, msg];
                }
            },
            errorTextFormat: function (data) {
                return 'Error: ' + data.responseText
            }
        },
            // options for the Add Dialog
        {
            addCaption: "新增过滤条目",
            top : "150",  
            left : "150",  
            jqModal : false,   
            closeAfterAdd: true,
            reloadAfterSubmit: true,
            afterSubmit: function (response, postdata) {
                var msg = response.responseText;
                if (msg.indexOf("ok") != -1) {
                    layer.msg("新增成功", {
                        icon: 1,
                        time: 1000 //1秒关闭（如果不配置，默认是3秒）
                    });
                    return [true, ""];
                } else {
                    return [false, msg];
                }
            },
            errorTextFormat: function (data) {
                return 'Error: ' + data.responseText
            }
        },
        // options for the Delete Dailog
        {
            //delCaption: "删除过滤条目",
            //top: "200",
            //left: "250",
            //jqModal: false,
            //reloadAfterSubmit: true,
            //closeAfterDel:true,
            //afterSubmit: function (response, postdata) {
            //    var msg = response.statusText;
            //    if (msg.indexOf("ok") == -1) {
            //        layer.msg("删除成功", {
            //            icon: 1,
            //            time: 2000 //1秒关闭（如果不配置，默认是3秒）
            //        });
            //        return [true, ""];
            //    } else {
            //        return [false, msg];
            //    }
            //},
            //errorTextFormat: function (data) {
            //    return 'Error: ' + data.responseText
            //}
        }).navButtonAdd('#filter_jqGridPager', {
            caption: "", buttonicon: "ui-icon-trash", onClickButton: function () {
                var grid = $("#filter_jqGrid");
                //获取多行的id，是个Array  
                var selectedRowIds = grid.jqGrid("getGridParam", "selarrrow");
                //判断是否为空  
                if(selectedRowIds==""){  
                    layer.msg("请选择要删除的行", {
                        icon: 3,
                        time: 1000 //1秒关闭（如果不配置，默认是3秒）
                    });
                }  
                else {
                    var len = selectedRowIds.length;
                    var ids="";
                    for (var i = 0; i < len; i++) {
                        ids +=selectedRowIds[i]+",";
                    }
                    ids = ids.substr(0, ids.length - 1);
                    layer.confirm('是否删除所选行内容', {
                        btn: ['删除', '取消'], //按钮
                        shade: false, //不显示遮罩
                        icon: 3,
                        title: '删除'
                    }, function () {
                        layer.msg('数据提交中...', { icon: 16 });
                        $.ajax({
                            type: "post",
                            url: head + "/Filter/Manage",
                            data: { oper: 'del', id: ids },
                            dataType: "json",
                            success: function (res) {
                                layer.closeAll();
                                if (res == "ok") {
                                    layer.msg('删除条目成功', {
                                        icon: 1,
                                        time: 1000
                                    }, function () {
                                        jQuery("#filter_jqGrid").jqGrid('setGridParam', {
                                            page: 1
                                        }).trigger('reloadGrid')
                                    });
                                } else {
                                    layer.msg(res, {
                                        icon: 2,
                                        time: 3000
                                    });
                                }
                            },
                            error: function (data) {
                                layer.closeAll();
                                layer.msg("服务器繁忙！", {
                                    icon: 5,
                                    time: 3000
                                });
                            }
                        });
                    });
                }
            },
            position: "last"
        });
}

//获取预置数据
function getDmsData() {
    $("#dm_jqGrid").jqGrid({
        url: head + "/DanMu/getDmList",
        editurl: head + "/DanMu/Manage",
        mtype: "POST",
        datatype: "json",
        colModel: [
            {
                label: '内容', name: 'Value', width: 150, searchoptions: { sopt: ['bw', 'cn', 'nc'] }, align: 'center',
                editable: true,
                formoptions: {
                    elmsuffix: "(必填字段)" // the suffix to show after that
                }, editrules: {
                    custom_func: function (value, column) {
                        if (value.length < 0)
                            return [false, "内容不能为空！"];
                        else if (value.length > 100) {
                            return [false, "内容长度不能大于100！"];
                        }else
                            return [true, ""];
                    },
                    custom: true,
                    required: true
                },
            },
            {
                label: '状态', name: 'State', width: 150,
                editable: true,
                edittype: 'checkbox',
                formatter: function (v, x, r) {
                    if (v)
                        return "<input type='checkbox' checked='checked' disabled='disabled'/>";
                    else
                        return "<input type='checkbox' disabled='disabled'/>";
                },
                search: false, sortable: false, align: 'center',
                editoptions: { value: "1:0" },
            },
            {
                label: '创建日期',
                name: 'Date',
                width: 150,
                sorttype: 'date',
                editable: false,
                searchoptions: {
                    // dataInit is the client-side event that fires upon initializing the toolbar search field for a column
                    // use it to place a third party control to customize the toolbar
                    dataInit: function (element) {
                        $(element).datepicker({
                            id: 'orderDate_datePicker',
                            dateFormat: 'yy-mm-dd',
                            //minDate: new Date(2010, 0, 1),
                            maxDate: new Date(2020, 0, 0),
                            showOn: 'focus'
                        });
                    },
                    sopt: ['ge', 'le']
                },
                align: 'center'
            }
        ],
        viewrecords: true,
        width: 1000,
        height: 550,
        rowNum: 20,
        rowList: [20, 30, 50],
        pager: "#dm_jqGridPager",
        jsonReader: {
            repeatitems: false
        },
        caption: "弹幕预置评论列表",
        rownumbers: true,
        multiselect: true,
    }).navGrid("#dm_jqGridPager",
        { add: true, edit: true, del: false, search: true, refresh: true, view: true, position: "left", cloneToTop: true },
            // options for the Edit Dialog
        {
            editCaption: "编辑弹幕",
            top: "100",
            left: "150",
            jqModal: false,
            closeAfterEdit: true,
            reloadAfterSubmit: true,
            afterSubmit: function (response, postdata) {
                var msg = response.responseText;
                if (msg.indexOf("ok") != -1) {
                    layer.msg("编辑成功", {
                        icon: 1,
                        time: 1000 //1秒关闭（如果不配置，默认是3秒）
                    });
                    return [true, ""];
                } else {
                    return [false, msg];
                }
            },
            errorTextFormat: function (data) {
                return 'Error: ' + data.responseText
            }
        },
            // options for the Add Dialog
        {
            addCaption: "新增弹幕",
            top: "100",
            left: "150",
            jqModal: false,
            closeAfterAdd: true,
            reloadAfterSubmit: true,
            afterSubmit: function (response, postdata) {
                var msg = response.responseText;
                if (msg.indexOf("ok") != -1) {
                    layer.msg("新增成功", {
                        icon: 1,
                        time: 1000 //1秒关闭（如果不配置，默认是3秒）
                    });
                    return [true, ""];
                } else {
                    return [false, msg];
                }
            },
            errorTextFormat: function (data) {
                return 'Error: ' + data.responseText
            }
        },
        // options for the Delete Dailog
        {
            //delCaption: "删除过滤条目",
            //top: "200",
            //left: "250",
            //jqModal: false,
            //reloadAfterSubmit: true,
            //closeAfterDel:true,
            //afterSubmit: function (response, postdata) {
            //    var msg = response.statusText;
            //    if (msg.indexOf("ok") == -1) {
            //        layer.msg("删除成功", {
            //            icon: 1,
            //            time: 2000 //1秒关闭（如果不配置，默认是3秒）
            //        });
            //        return [true, ""];
            //    } else {
            //        return [false, msg];
            //    }
            //},
            //errorTextFormat: function (data) {
            //    return 'Error: ' + data.responseText
            //}
        }).navButtonAdd('#dm_jqGridPager', {
            caption: "", buttonicon: "ui-icon-trash", onClickButton: function () {
                var grid = $("#dm_jqGrid");
                //获取多行的id，是个Array  
                var selectedRowIds = grid.jqGrid("getGridParam", "selarrrow");
                //判断是否为空  
                if (selectedRowIds == "") {
                    layer.msg("请选择要删除的行", {
                        icon: 3,
                        time: 1000 //1秒关闭（如果不配置，默认是3秒）
                    });
                }
                else {
                    var len = selectedRowIds.length;
                    var ids = "";
                    for (var i = 0; i < len; i++) {
                        ids += selectedRowIds[i] + ",";
                    }
                    ids = ids.substr(0, ids.length - 1);
                    layer.confirm('是否删除所选行内容', {
                        btn: ['删除', '取消'], //按钮
                        shade: false, //不显示遮罩
                        icon: 3,
                        title: '删除'
                    }, function () {
                        layer.msg('数据提交中...', { icon: 16 });
                        $.ajax({
                            type: "post",
                            url: head + "/DanMu/Manage",
                            data: { oper: 'del', id: ids },
                            dataType: "json",
                            success: function (res) {
                                layer.closeAll();
                                if (res == "ok") {
                                    layer.msg('删除条目成功', {
                                        icon: 1,
                                        time: 1000
                                    }, function () {
                                        jQuery("#dm_jqGrid").jqGrid('setGridParam', {
                                            page: 1
                                        }).trigger('reloadGrid')
                                    });
                                } else {
                                    layer.msg(res, {
                                        icon: 2,
                                        time: 3000
                                    });
                                }
                            },
                            error: function (data) {
                                layer.closeAll();
                                layer.msg("服务器繁忙！", {
                                    icon: 5,
                                    time: 3000
                                });
                            }
                        });
                    });
                }
            },
            position: "last"
        });
}