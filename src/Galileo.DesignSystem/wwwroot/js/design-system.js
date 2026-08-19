(function () {
  'use strict';

  const openerByDialog = new WeakMap();

  function isDialog(element) {
    return element instanceof HTMLDialogElement && element.hasAttribute('data-gds-dialog');
  }

  function isDismissible(dialog) {
    return dialog.dataset.gdsDialogDismissible !== 'false';
  }

  function restoreFocus(dialog) {
    const opener = openerByDialog.get(dialog);

    if (opener && opener.isConnected && typeof opener.focus === 'function') {
      opener.focus({ preventScroll: true });
    }

    openerByDialog.delete(dialog);
  }

  function closeDialog(dialog) {
    if (dialog.open) {
      dialog.close();
    } else {
      restoreFocus(dialog);
    }
  }

  function openDialog(dialog, opener) {
    if (!dialog.open && typeof dialog.showModal === 'function') {
      openerByDialog.set(dialog, opener);
      dialog.showModal();
    }
  }

  function isTableRowInteractiveElement(element) {
    return element.closest(
      'a, button, input, select, textarea, summary, [data-gds-table-row-ignore]'
    ) !== null;
  }

  function openTableRowLink(row) {
    const link = row.querySelector('a[href]');

    if (link) {
      link.click();
    }
  }

  document.addEventListener('click', function (event) {
    if (!(event.target instanceof Element)) {
      return;
    }

    const openTrigger = event.target.closest('[data-gds-dialog-open]');
    if (openTrigger) {
      const dialogId = openTrigger.getAttribute('data-gds-dialog-open');
      const dialog = dialogId ? document.getElementById(dialogId) : null;

      if (isDialog(dialog)) {
        event.preventDefault();
        openDialog(dialog, openTrigger);
        return;
      }
    }

    const closeTrigger = event.target.closest('[data-gds-dialog-close]');
    if (closeTrigger) {
      const dialog = closeTrigger.closest('dialog[data-gds-dialog]');

      if (isDialog(dialog)) {
        event.preventDefault();
        closeDialog(dialog);
        return;
      }
    }

    const tableRow = event.target.closest('tr[data-gds-table-row-link]');
    if (tableRow && !isTableRowInteractiveElement(event.target)) {
      openTableRowLink(tableRow);
      return;
    }

    const dialog = event.target;
    if (isDialog(dialog) && dialog.open && event.target === dialog && isDismissible(dialog)) {
      closeDialog(dialog);
    }
  });

  document.addEventListener('change', function (event) {
    if (!(event.target instanceof HTMLSelectElement) ||
        !event.target.hasAttribute('data-gds-submit-on-change')) {
      return;
    }

    const form = event.target.form;
    if (form) {
      form.requestSubmit();
    }
  });

  document.addEventListener('cancel', function (event) {
    const dialog = event.target;

    if (!isDialog(dialog) || !dialog.open) {
      return;
    }

    if (!isDismissible(dialog)) {
      event.preventDefault();
      return;
    }

    event.preventDefault();
    closeDialog(dialog);
  }, true);

  document.addEventListener('close', function (event) {
    const dialog = event.target;

    if (isDialog(dialog)) {
      restoreFocus(dialog);
    }
  }, true);
})();
