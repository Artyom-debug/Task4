document.querySelectorAll(".pass-eye").forEach(function (button) {
  button.addEventListener("click", function () {
    var wrap = button.closest(".pass-wrap");
    if (!wrap) return;

    var input = wrap.querySelector("input");
    if (!input) return;

    var show = input.getAttribute("type") === "password";
    input.setAttribute("type", show ? "text" : "password");

    var eyeOpen = button.querySelector(".eye-open");
    var eyeClose = button.querySelector(".eye-close");
    if (eyeOpen) eyeOpen.classList.toggle("d-none", show);
    if (eyeClose) eyeClose.classList.toggle("d-none", !show);
  });
});

(function () {
  var checks = document.querySelectorAll(".user-check");
  var bulkButtons = document.querySelectorAll(".bulk-action");
  if (!checks.length || !bulkButtons.length) return;

  function syncBulkButtons() {
    var anyChecked = Array.prototype.some.call(checks, function (c) { return c.checked; });
    bulkButtons.forEach(function (btn) {
      btn.disabled = !anyChecked;
    });
  }

  checks.forEach(function (c) {
    c.addEventListener("change", syncBulkButtons);
  });
  syncBulkButtons();
})();
