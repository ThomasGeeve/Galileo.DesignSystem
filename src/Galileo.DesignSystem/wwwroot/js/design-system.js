(function () {
  'use strict';

  // Shared browser behavior for design-system components. Components opt in through data-gds-* attributes.

  // Dialog behavior
  const openerByDialog = new WeakMap();

  function isDialog(element) {
    return element instanceof HTMLDialogElement && element.hasAttribute('data-gds-dialog');
  }

  function isDismissible(dialog) {
    return dialog.dataset.gdsDialogDismissible !== 'false';
  }

  function restoreFocus(dialog) {
    // Return focus to the control that opened the dialog after it closes.
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
    // Use the native modal dialog API so focus and backdrop behavior remain browser-managed.
    if (!dialog.open && typeof dialog.showModal === 'function') {
      openerByDialog.set(dialog, opener);
      dialog.showModal();
    }
  }

  // Form behavior
  function findFormField(form, name) {
    if (!name) {
      return null;
    }

    for (const element of form.elements) {
      if (element instanceof HTMLElement && element.getAttribute('name') === name) {
        return element;
      }
    }

    return null;
  }

  function clearFormErrors(form) {
    form.querySelectorAll('[data-gds-form-error]').forEach(function (error) {
      error.textContent = '';
      error.hidden = true;
    });

    for (const element of form.elements) {
      if (element instanceof HTMLElement) {
        element.removeAttribute('aria-invalid');
      }
    }

    const summary = form.querySelector('[data-gds-form-error-summary]');
    if (summary instanceof HTMLElement) {
      summary.textContent = '';
      summary.hidden = true;
    }
  }

  function showFormSummary(form, message) {
    const summary = form.querySelector('[data-gds-form-error-summary]');

    if (summary instanceof HTMLElement) {
      summary.textContent = message;
      summary.hidden = false;
    }
  }

  function showFormErrors(form, errors) {
    let firstInvalidField = null;

    for (const [fieldName, rawMessages] of Object.entries(errors || {})) {
      const field = findFormField(form, fieldName);
      const fieldContainer = field?.closest('.gds-form-field');
      const error = fieldContainer?.querySelector('[data-gds-form-error]');
      const messages = Array.isArray(rawMessages) ? rawMessages : [rawMessages];
      const message = messages.filter(Boolean).join(' ');

      if (!message) {
        continue;
      }

      if (field) {
        field.setAttribute('aria-invalid', 'true');
        if (!firstInvalidField) {
          firstInvalidField = field;
        }
      }

      if (error instanceof HTMLElement) {
        error.textContent = message;
        error.hidden = false;

        if (field && error.id) {
          const describedBy = new Set((field.getAttribute('aria-describedby') || '').split(/\s+/).filter(Boolean));
          describedBy.add(error.id);
          field.setAttribute('aria-describedby', Array.from(describedBy).join(' '));
        }
      } else {
        showFormSummary(form, message);
      }
    }

    if (firstInvalidField instanceof HTMLElement) {
      firstInvalidField.focus({ preventScroll: true });
    }
  }

  function setFormBusy(form, busy) {
    const dialog = form.closest('dialog[data-gds-dialog]');

    if (busy) {
      form.setAttribute('aria-busy', 'true');
    } else {
      form.removeAttribute('aria-busy');
    }

    if (!isDialog(dialog)) {
      return;
    }

    dialog.querySelectorAll('button[type="submit"]').forEach(function (button) {
      if (busy) {
        button.dataset.gdsDialogDisabledBefore = button.disabled ? 'true' : 'false';
        button.disabled = true;
      } else {
        button.disabled = button.dataset.gdsDialogDisabledBefore === 'true';
        delete button.dataset.gdsDialogDisabledBefore;
      }
    });
  }

  // Dialog form behavior
  async function readJsonResponse(response) {
    const contentType = response.headers.get('content-type') || '';

    if (!contentType.includes('json')) {
      return null;
    }

    try {
      return await response.json();
    } catch {
      return null;
    }
  }

  async function submitDialogForm(form, dialog) {
    clearFormErrors(form);

    if (!form.checkValidity()) {
      form.reportValidity();
      return;
    }

    setFormBusy(form, true);

    try {
      const method = (form.method || 'get').toUpperCase();
      const formData = new FormData(form);
      const request = {
        method,
        headers: { Accept: 'application/json' },
        credentials: 'same-origin'
      };
      let url = form.action || window.location.href;

      if (method === 'GET') {
        const query = new URLSearchParams();
        for (const [key, value] of formData.entries()) {
          if (typeof value === 'string') {
            query.append(key, value);
          }
        }

        if (query.toString()) {
          url += `${url.includes('?') ? '&' : '?'}${query.toString()}`;
        }
      } else {
        request.body = formData;
      }

      const response = await fetch(url, request);
      const payload = await readJsonResponse(response);

      if (!response.ok) {
        if (payload?.errors && typeof payload.errors === 'object') {
          showFormErrors(form, payload.errors);
        } else {
          showFormSummary(
            form,
            payload?.message || form.dataset.gdsDialogErrorMessage || 'Er is iets misgegaan. Probeer het opnieuw.'
          );
        }
        return;
      }

      if (payload && Object.prototype.hasOwnProperty.call(payload, 'value')) {
        const value = payload.value == null ? '' : String(payload.value);
        const targetId = form.dataset.gdsDialogUpdateTarget;
        const target = targetId ? document.getElementById(targetId) : null;

        if (target) {
          target.textContent = value;
        }

        const field = findFormField(form, form.dataset.gdsDialogUpdateField);
        if (field instanceof HTMLInputElement || field instanceof HTMLTextAreaElement) {
          field.value = value;
        }
      }

      closeDialog(dialog);
    } catch {
      showFormSummary(
        form,
        form.dataset.gdsDialogErrorMessage || 'Er is iets misgegaan. Probeer het opnieuw.'
      );
    } finally {
      setFormBusy(form, false);
    }
  }

  // Table row behavior
  function isTableRowInteractiveElement(element) {
    return element.closest(
      'a, button, input, select, textarea, summary, [data-gds-table-row-ignore]'
    ) !== null;
  }

  function openTableRowLink(row) {
    // The first link provides the accessible destination; the row makes it easier to activate with a pointer.
    const link = row.querySelector('a[href]');

    if (link) {
      link.click();
    }
  }

  // Dialog and table click behavior
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

  // Dialog form submission
  document.addEventListener('submit', function (event) {
    if (!(event.target instanceof HTMLFormElement) ||
        !event.target.hasAttribute('data-gds-dialog-form')) {
      return;
    }

    const form = event.target;
    const dialog = form.closest('dialog[data-gds-dialog]');

    if (!isDialog(dialog)) {
      return;
    }

    event.preventDefault();
    submitDialogForm(form, dialog);
  });

  // Form auto-submit behavior for server-rendered filter forms.
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

  // Native dialog lifecycle behavior
  // Keep Escape behavior consistent for native dialogs, including non-dismissible dialogs.
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

  // Restore the opener focus after a native dialog closes by any supported route.
  document.addEventListener('close', function (event) {
    const dialog = event.target;

    if (isDialog(dialog)) {
      restoreFocus(dialog);
    }
  }, true);
})();
