// Example: How to integrate with the dark mode system

// 1. Listen for dark mode changes
window.addEventListener('darkModeToggled', function(event) {
    const isDark = event.detail.isDark;
    console.log('Dark mode is now:', isDark ? 'enabled' : 'disabled');
    
    // Your custom logic here
    if (isDark) {
        // Do something when dark mode is enabled
        updateChartColors('dark');
    } else {
        // Do something when light mode is enabled
        updateChartColors('light');
    }
});

// 2. Check current theme programmatically
function checkCurrentTheme() {
    if (window.darkMode) {
        const currentTheme = window.darkMode.getCurrentTheme();
        console.log('Current theme:', currentTheme);
        return currentTheme;
    }
    return 'light'; // fallback
}

// 3. Example: Update chart colors based on theme
function updateChartColors(theme) {
    // Example for Chart.js or similar charting library
    const colors = theme === 'dark' ? {
        background: '#2d2d2d',
        text: '#e9ecef',
        grid: '#495057'
    } : {
        background: '#ffffff',
        text: '#212529',
        grid: '#dee2e6'
    };
    
    // Update your charts with new colors
    // chart.update();
}

// 4. Example: CSS-in-JS with theme awareness
function getThemedStyles() {
    const isDark = window.darkMode && window.darkMode.getCurrentTheme() === 'dark';
    
    return {
        backgroundColor: isDark ? '#2d2d2d' : '#ffffff',
        color: isDark ? '#e9ecef' : '#212529',
        borderColor: isDark ? '#495057' : '#dee2e6'
    };
}

// 5. Example: Dynamic CSS class application
function applyThemedClasses(element) {
    const isDark = window.darkMode && window.darkMode.getCurrentTheme() === 'dark';
    
    element.classList.toggle('dark-theme', isDark);
    element.classList.toggle('light-theme', !isDark);
}

// 6. Example: Initialize component based on current theme
document.addEventListener('DOMContentLoaded', function() {
    // Wait a bit for dark mode to initialize
    setTimeout(() => {
        const theme = checkCurrentTheme();
        initializeComponentWithTheme(theme);
    }, 100);
});

function initializeComponentWithTheme(theme) {
    console.log('Initializing component with theme:', theme);
    // Your initialization logic here
}
