
(function () {
    'use strict';

    const form = document.getElementById('loginForm');
    const emailInput = document.getElementById('email');
    const pwInput = document.getElementById('password');
    const emailError = document.getElementById('emailError');
    const passwordError = document.getElementById('passwordError');
    const togglePw = document.getElementById('togglePw');
    const eyeIcon = document.getElementById('eyeIcon');
    const submitBtn = document.getElementById('submitBtn');
    const btnLabel = document.getElementById('btnLabel');
    const btnSpinner = document.getElementById('btnSpinner');
    const formSuccess = document.getElementById('formSuccess');

    // Email validation function
    function isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
    }

    // Toggle password visibility
    togglePw.addEventListener('click', function () {
        const isPassword = pwInput.type === 'password';
        pwInput.type = isPassword ? 'text' : 'password';

        // Update eye icon
        if (!isPassword) {
            eyeIcon.innerHTML =
                `<path d="M2.5 12S6 5.5 12 5.5 21.5 12 21.5 12 18 18.5 12 18.5 2.5 12 2.5 12Z" stroke="currentColor" stroke-width="1.6"/><circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.6"/>`;
        } else {
            eyeIcon.innerHTML =
                `<path d="M2.5 12S6 5.5 12 5.5 21.5 12 21.5 12 18 18.5 12 18.5 2.5 12 2.5 12Z" stroke="currentColor" stroke-width="1.6"/><circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.6"/><line x1="3" y1="3" x2="21" y2="21" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/>`;
        }
    });

    // Clear errors on input
    emailInput.addEventListener('input', function () {
        this.classList.remove('input-error');
        emailError.classList.add('hidden');
    });

    pwInput.addEventListener('input', function () {
        this.classList.remove('input-error');
        passwordError.classList.add('hidden');
    });

    // Real-time validation on blur
    emailInput.addEventListener('blur', function () {
        const value = this.value.trim();
        if (value && !isValidEmail(value)) {
            this.classList.add('input-error');
            emailError.textContent = 'Please enter a valid email address';
            emailError.classList.remove('hidden');
        }
    });

    pwInput.addEventListener('blur', function () {
        const value = this.value.trim();
        if (value && value.length < 6) {
            this.classList.add('input-error');
            passwordError.textContent = 'Password must be at least 6 characters';
            passwordError.classList.remove('hidden');
        }
    });

    // Form submit validation
    form.addEventListener('submit', function (e) {
        let isValid = true;

        // Clear previous errors
        emailError.classList.add('hidden');
        passwordError.classList.add('hidden');
        emailInput.classList.remove('input-error');
        pwInput.classList.remove('input-error');

        // Validate email
        const email = emailInput.value.trim();
        if (!email) {
            emailInput.classList.add('input-error');
            emailError.textContent = 'Email is required';
            emailError.classList.remove('hidden');
            isValid = false;
        } else if (!isValidEmail(email)) {
            emailInput.classList.add('input-error');
            emailError.textContent = 'Please enter a valid email address';
            emailError.classList.remove('hidden');
            isValid = false;
        }

        // Validate password
        const password = pwInput.value.trim();
        if (!password) {
            pwInput.classList.add('input-error');
            passwordError.textContent = 'Password is required';
            passwordError.classList.remove('hidden');
            isValid = false;
        } else if (password.length < 6) {
            pwInput.classList.add('input-error');
            passwordError.textContent = 'Password must be at least 6 characters';
            passwordError.classList.remove('hidden');
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
            return;
        }

        // Show loading state
        submitBtn.disabled = true;
        btnLabel.textContent = 'Signing in…';
        btnSpinner.classList.remove('hidden');
    });

    // Reset button state if form submission fails (page reloads with errors)
    if (document.querySelector('.field-validation-error') || document.querySelector('.validation-summary-errors')) {
        submitBtn.disabled = false;
        btnLabel.textContent = 'Sign in to dashboard';
        btnSpinner.classList.add('hidden');
    }

    // Auto-submit on Enter key (already handled by form)

    // Handle successful form submission (if redirected with success)
    const alertMessage = document.querySelector('[role="alert"]');
    if (alertMessage && alertMessage.textContent.includes('Login successful')) {
        formSuccess.classList.remove('hidden');
        setTimeout(() => {
            window.location.href = '@Url.Action("Index", "Dashboard")';
        }, 1500);
    }

})();
