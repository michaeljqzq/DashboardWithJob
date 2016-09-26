AutoRefresh = function(){

};
Util = function () {

};
AutoRefresh.prototype.intervalId = -1;

AutoRefresh.prototype.lastStatus = null;

AutoRefresh.prototype.turnOnAutoRefresh = function () {
    if (AutoRefresh.prototype.intervalId === -1) {
        AutoRefresh.prototype.intervalId = setInterval(AutoRefresh.prototype.doAutoRefresh, 5000);
    }
}

AutoRefresh.prototype.turnOffAutoRefresh = function () {
    if (AutoRefresh.prototype.intervalId !== -1) {
        clearInterval(AutoRefresh.prototype.intervalId);
        AutoRefresh.prototype.intervalId = -1;
    }
}

AutoRefresh.prototype.doAutoRefresh = function () {
    var jobid = $('#jobid').val();
    var url = $('#ajaxurl').val();
    if (!jobid || !url) {
        return;
    }
    $('#current-state-partial').load(url + '?jobid=' + jobid, null, function () {
        AutoRefresh.prototype.initialPage();
    });
}

AutoRefresh.prototype.initialPage = function () {
    $.fn.bootstrapSwitch.defaults.onColor = 'warning';
    $('#switch-input').bootstrapSwitch();
    if ($('#switch-input').length !== 0 && $('#switch-input').bootstrapSwitch('state')) {
        AutoRefresh.prototype.turnOnAutoRefresh();
    } else {
        AutoRefresh.prototype.turnOffAutoRefresh();
    }

    $('#switch-input').on('switchChange.bootstrapSwitch', function (e, data) {
        if (data) {
            AutoRefresh.prototype.turnOnAutoRefresh();
            $('#progressbar').addClass('active');
        } else {
            AutoRefresh.prototype.turnOffAutoRefresh();
            $('#progressbar').removeClass('active');
        }
    });

    $('#triggerjob').click(function () {
        var url = $('#jobtriggerurl').val();
        $.ajax(url, {
            dataType: "json",
            method: "POST"
        })
        .done(function(data){
            if (data.success === 'true') {
                $('#next-schedule-job').html(data.timestamp);
                Util.prototype.createAlert('Success : ' + data.message, 'alert alert-success fade in');
                // set a timer to refresh page
                $('#jobid').val(data.jobid);
                setTimeout(function () {
                    AutoRefresh.prototype.doAutoRefresh();
                }, 8000);
            } else {
                Util.prototype.createAlert('Fail : ' + data.message, 'alert alert-danger fade in');
            }
        })
        .fail(function(jqxhr,textmsg){
            Util.prototype.createAlert('Fail : Unknown error.', 'alert alert-danger fade in');
        });
    });

    $('#joblist a').click(function () {
        var url = $('#jobdetailurl').val();
        var jobid = $(this).attr('jobid');
        $('#jobdetail').load(url + '?jobid=' + jobid);
    });

    $('body').delegate('#jobdetailrefresh','click',function() {
        var url = $('#jobdetailurl').val();
        var jobid = $(this).attr('jobid');
        $('#jobdetail').load(url + '?jobid=' + jobid);
    });

    var status = $('#jobstatus').val();
    if (status === '1') {
        $('#progressbar').show();
    } else {
        $('#progressbar').hide();
    }

    if ((AutoRefresh.prototype.lastStatus === '1' && status === '0') || 
        (AutoRefresh.prototype.lastStatus === '0' && status === '1')) {
        $('#history-partial').load($('#jobhistoryurl').val());
    }
    AutoRefresh.prototype.lastStatus = status;
}

Util.prototype.createAlert = function (message, className) {
    var element = $('<div/>').attr('id', 'notification').addClass(className);
    var inner = $('<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><div>'+message+'</div>');
    inner.appendTo(element);
    $('#main-c-indicator').before(element);
}

$(document).ready(function () {
    AutoRefresh.prototype.initialPage();
});

