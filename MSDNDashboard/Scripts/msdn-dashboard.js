AutoRefresh = {
    intervalId: -1,
    turnOnAutoRefresh: function () {
        intervalId = setInterval(AutoRefresh.doAutoRefresh, 5000);
    },
    turnOffAutoRefresh: function () {
        if (intervalId != -1) {
            clearInterval(intervalId);
            intervalId = -1;
        }
    },
    doAutoRefresh: function () {
        var jobid = $('#jobid').val();
        var url = $('#ajaxurl').val();
        if (!jobid || !url) {
            return;
        }
        $('#current-state-partial').load(url + '?jobid=' + jobid);
    }
};

$(document).ready(function() {
    $('#switch-input').bootstrapSwitch();
    AutoRefresh.turnOnAutoRefresh();
    $('#switch-input').change(function () {
        if ($(this).is(':checked')) {
            AutoRefresh.turnOnAutoRefresh();
        } else {
            AutoRefresh.turnOffAutoRefresh();
        }
    });
});
