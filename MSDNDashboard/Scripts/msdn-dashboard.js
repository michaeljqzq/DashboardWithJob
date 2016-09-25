AutoRefresh = function(){

};
Util = function () {

};
AutoRefresh.prototype.intervalId = -1;

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
    $('#switch-input').bootstrapSwitch();
    if ($('#switch-input') && $('#switch-input').bootstrapSwitch('state')) {
        AutoRefresh.prototype.turnOnAutoRefresh();
    }

    $('#switch-input').on('switchChange.bootstrapSwitch', function (e, data) {
        if (data) {
            AutoRefresh.prototype.turnOnAutoRefresh();
        } else {
            AutoRefresh.prototype.turnOffAutoRefresh();
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
                //TODO set a timer to refresh page
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
        $('#jobdetail').load(url + '?jobid=' + jobid, null, function () {
            
        });
    });
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

