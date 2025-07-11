// Standard JS functions

// https://stackoverflow.com/a/31027662/13729
function epic(c) {
    c.onerror = '';
    c.src = '/devbetter_icon_200x200.jpg';
};

// Dark Mode Toggle Functionality
class DarkModeToggle {
    constructor() {
        this.storageKey = 'devbetter_darkMode';
        this.init();
    }

    init() {
        // Initialize dark mode state
        this.loadDarkModePreference();
        this.setupToggleButton();
        this.updateToggleButton();
        this.setupSystemPreferenceListener();
    }

    loadDarkModePreference() {
        // Check session storage first
        const sessionPreference = sessionStorage.getItem(this.storageKey);
        if (sessionPreference !== null) {
            const isDark = sessionPreference === 'true';
            this.setDarkMode(isDark);
            return;
        }

        // Fall back to system preference if no session preference
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            this.setDarkMode(true);
        } else {
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

// Initialize dark mode when the DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Create global instance for other scripts to access
    window.darkModeToggle = new DarkModeToggle();
    
    // Also make it available as a shorter reference
    window.darkMode = window.darkModeToggle;
});