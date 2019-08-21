var cssTable = {
  tableClass: 'table table-striped table-bordered',
  loadingClass: 'loading',
  ascendingIcon: 'glyphicon glyphicon-chevron-up',
  descendingIcon: 'glyphicon glyphicon-chevron-down',
  handleIcon: 'glyphicon glyphicon-menu-hamburger',
  pagination: {
    infoClass: 'pull-left',
    wrapperClass: 'vuetable-pagination pull-right',
    activeClass: 'btn-primary',
    disabledClass: 'disabled',
    pageClass: 'btn btn-border',
    linkClass: 'btn btn-border',
    icons: {
      first: '',
      prev: '',
      next: '',
      last: ''
    }
  }
};

var uivLocales = {
  ru: {
    uiv: {
      datePicker: {
        clear: 'Очистить',
        today: 'Сегодня',
        month: 'Месяц',
        month1: 'Январь',
        month2: 'Февраль',
        month3: 'Март',
        month4: 'Апрель',
        month5: 'Май',
        month6: 'Июнь',
        month7: 'Июль',
        month8: 'Август',
        month9: 'Сентябрь',
        month10: 'Октябрь',
        month11: 'Ноябрь',
        month12: 'Декабрь',
        year: 'Год',
        week1: 'Пн',
        week2: 'Вт',
        week3: 'Ср',
        week4: 'Чт',
        week5: 'Пт',
        week6: 'Сб',
        week7: 'Вс'
      },
      timePicker: {
        am: 'дня',
        pm: 'вечера'
      },
      modal: {
        cancel: 'Отмена',
        ok: 'OK'
      }
    }
  }
};

function isNumber(n) {
  return !isNaN(+n) && isFinite(n);
}

$(document).on('change', ':file', function () {
  $(this).parents('form').submit();
});

//numeral.locale('ru');

// show delete modal
$('button[data-delete-modal]').click(function () {
  var $this = $(this);
  var $deleteModal = $($this.data().deleteModal);
  $deleteModal.modal('show');
});

// confirm delete
$('body').on('click', 'button[data-delete-id][data-delete-url][data-delete-redirect]', function() {
  var $this = $(this);
  var postUrl = $this.data().deleteUrl;
  var postData = {
    id: $this.data().deleteId
  };
  var redirect = $this.data().deleteRedirect;
  axios.post(postUrl, postData).then(function (res) {
    window.location.href = redirect;
  }, function (res) {
    var $modal = $this.parents('.modal');
    $modal.on('hidden.bs.modal', function (e) {
      $modal.off();
      alert('Ошибка удаления');
    });
    $modal.modal('hide');
  });
});
