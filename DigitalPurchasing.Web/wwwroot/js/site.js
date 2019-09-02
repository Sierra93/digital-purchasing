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

$(document).ready(function() {
  // show delete modal
  $('button[data-delete-modal]').click(function () {
    var $this = $(this);
    var $deleteModal = $($this.data().deleteModal);
    $deleteModal.modal('show');
  });

  // confirm delete
  $('body').on('click', 'button[data-delete-id][data-delete-url][data-delete-redirect]', function () {
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

  $.fn.select2.defaults.set("theme", "bootstrap");

  $('.remote-select-uom').select2({
    ajax: {
      url: '/uom/select2',
      dataType: 'json'
    }
  });

  $('[data-toggle="tooltip"]').tooltip();
});

/**
 * Enable Bootstrap tooltips using Vue directive
 * @author Vitim.us
 * @see https://gist.github.com/victornpb/020d393f2f5b866437d13d49a4695b47
 * @example
 *   <button v-tooltip="foo">Hover me</button>
 *   <button v-tooltip.click="bar">Click me</button>
 *   <button v-tooltip.html="baz">Html</button>
 *   <button v-tooltip:top="foo">Top</button>
 *   <button v-tooltip:left="foo">Left</button>
 *   <button v-tooltip:right="foo">Right</button>
 *   <button v-tooltip:bottom="foo">Bottom</button>
 *   <button v-tooltip:auto="foo">Auto</button>
 *   <button v-tooltip:auto.html="clock" @click="clock = Date.now()">Updating</button>
 *   <button v-tooltip:auto.html.live="clock" @click="clock = Date.now()">Updating Live</button>
 */
Vue.directive('tooltip', {
  bind: function bsTooltipCreate(el, binding) {
    var trigger;
    if (binding.modifiers.focus || binding.modifiers.hover || binding.modifiers.click) {
      var t = [];
      if (binding.modifiers.focus) t.push('focus');
      if (binding.modifiers.hover) t.push('hover');
      if (binding.modifiers.click) t.push('click');
      trigger = t.join(' ');
    }
    $(el).tooltip({
      title: binding.value,
      placement: binding.arg,
      trigger: trigger,
      html: binding.modifiers.html
    });
  },
  update: function bsTooltipUpdate(el, binding) {
    var $el = $(el);
    $el.attr('title', binding.value).tooltip('fixTitle');

    var data = $el.data('bs.tooltip');
    if (binding.modifiers.live) { // update live without flickering (but it doesn't reposition)
      if (data.$tip) {
        if (data.options.html) data.$tip.find('.tooltip-inner').html(binding.value);
        else data.$tip.find('.tooltip-inner').text(binding.value);
      }
    } else {
      if (data.inState.hover || data.inState.focus || data.inState.click) $el.tooltip('show');
    }
  },
  unbind: function(el, binding) {
    $(el).tooltip('destroy');
  }
});
