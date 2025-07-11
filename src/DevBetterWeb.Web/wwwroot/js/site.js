// Standard JS functions

// https://stackoverflow.com/a/31027662/13729
function epic(c) {
    c.onerror = '';
    c.src = '/devbetter_icon_200x200.jpg';
};

// Dark Mode Toggle Functionality (Member Area Only)
class DarkModeToggle {
    constructor() {
        this.storageKey = 'devbetter_darkMode_member';
        this.init();
    }

    init() {
        // Only initialize if we're in the member area
        if (!this.isMemberArea()) {
            return;
        }
        
        // Initialize dark mode state
        this.loadDarkModePreference();
        this.setupToggleButton();
        this.updateToggleButton();
        this.setupSystemPreferenceListener();
    }

    isMemberArea() {
        // Check if we're in the member area by looking for the dark mode toggle button
        // or by checking the URL path
        return document.getElementById('darkModeToggle') !== null || 
               window.location.pathname.startsWith('/User/') ||
               window.location.pathname.startsWith('/Admin/') ||
               document.getElementById('wrapper') !== null; // SB Admin wrapper
    }

    loadDarkModePreference() {
        // The inline script in the member layout has already applied the theme
        // We just need to ensure the session storage is consistent
        const sessionPreference = sessionStorage.getItem(this.storageKey);
        const currentTheme = document.documentElement.getAttribute('data-theme');
        
        if (sessionPreference !== null) {
            // Session preference exists - ensure it matches the current theme
            const isDark = sessionPreference === 'true';
            if ((isDark && currentTheme !== 'dark') || (!isDark && currentTheme === 'dark')) {
                this.setDarkMode(isDark);
            }
            return;
        }

        // No session preference - check if system preference was applied by inline script
        if (currentTheme === 'dark') {
            // Dark theme was applied by inline script based on system preference
            sessionStorage.setItem(this.storageKey, 'true');
        } else {
            // No preference - apply light mode as default
            this.setDarkMode(false);
        }
    }

    setDarkMode(isDark) {
        const html = document.documentElement;
        if (isDark) {
            html.setAttribute('data-theme', 'dark');
        } else {
            html.removeAttribute('data-theme');
        }
        
        // Store in session storage for current session
        sessionStorage.setItem(this.storageKey, isDark.toString());
        
        // Dispatch custom event for other components that might need to know
        window.dispatchEvent(new CustomEvent('darkModeToggled', { 
            detail: { isDark: isDark } 
        }));
    }

    toggleDarkMode() {
        const html = document.documentElement;
        const isDark = html.getAttribute('data-theme') === 'dark';
        this.setDarkMode(!isDark);
        this.updateToggleButton();
    }

    setupToggleButton() {
        const toggleButton = document.getElementById('darkModeToggle');
        if (toggleButton) {
            toggleButton.addEventListener('click', (e) => {
                e.preventDefault();
                this.toggleDarkMode();
            });
        }
    }

    updateToggleButton() {
        const toggleButton = document.getElementById('darkModeToggle');
        if (toggleButton) {
            const html = document.documentElement;
            const isDark = html.getAttribute('data-theme') === 'dark';
            
            if (isDark) {
                toggleButton.innerHTML = '<i class="fas fa-sun"></i> Light';
                toggleButton.title = 'Switch to light mode';
                toggleButton.setAttribute('aria-label', 'Switch to light mode');
            } else {
                toggleButton.innerHTML = '<i class="fas fa-moon"></i> Dark';
                toggleButton.title = 'Switch to dark mode';
                toggleButton.setAttribute('aria-label', 'Switch to dark mode');
            }
        }
    }

    setupSystemPreferenceListener() {
        // Listen for system preference changes (only if user hasn't manually set a preference)
        if (window.matchMedia) {
            const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
            mediaQuery.addEventListener('change', (e) => {
                // Only auto-switch if user hasn't manually set a preference this session
                const hasManualPreference = sessionStorage.getItem(this.storageKey) !== null;
                if (!hasManualPreference) {
                    this.setDarkMode(e.matches);
                    this.updateToggleButton();
                }
            });
        }
    }

    // Public method to get current theme
    getCurrentTheme() {
        const html = document.documentElement;
        return html.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
    }
}

// Initialize dark mode when the DOM is loaded (Member area only)
document.addEventListener('DOMContentLoaded', function() {
    // Only create dark mode instance if we're in the member area
    const darkModeToggle = new DarkModeToggle();
    
    // Only make it globally available if it was actually initialized
    if (darkModeToggle.isMemberArea()) {
        window.darkModeToggle = darkModeToggle;
        window.darkMode = darkModeToggle;
    }
});