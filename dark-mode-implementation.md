# Dark Mode Implementation for DevBetterWeb Member Area

This document explains the dark mode feature implementation for the DevBetterWeb application's member/admin area only.

## Overview

The dark mode feature allows authenticated users in the member/admin area to toggle between light and dark themes for better viewing experience, especially in low-light conditions. The preference is stored for the current browser session only.

**Important:** Dark mode is only available in the authenticated member/admin area, not on the public homepage or marketing pages.

## Scope

- **Enabled**: Member area (`/User/`, `/Admin/` pages) with `_MemberLayout.cshtml`
- **Disabled**: Public pages (`/`, marketing pages) with `_Layout.cshtml`
- **Design**: Integrates with SB Admin 2 theme used in the member area

## Files Modified/Created

### New Files:
1. **`wwwroot/css/dark-mode.css`** - Contains all dark mode CSS variables and styles
2. **`wwwroot/dark-mode-test.html`** - Test page for verifying dark mode functionality

### Modified Files:
1. **`Pages/_MemberLayout.cshtml`** - Added dark mode CSS link, anti-flicker inline script, and toggle button to topbar
2. **`wwwroot/js/site.js`** - Added member-area-specific dark mode toggle functionality

### Removed From:
1. **`Views/Shared/_Layout.cshtml`** - Dark mode toggle and CSS removed from public layout

## Anti-Flicker Implementation

One common issue with dark mode implementations is the "flicker" effect where the page briefly displays in light mode before switching to dark mode on page load. This is solved with an inline script in the `<head>` section that runs before CSS loads:

```html
<!-- Inline script to prevent dark mode flicker - must run before CSS loads -->
<script>
    (function() {
        const darkModeKey = 'devbetter_darkMode_member';
        const sessionPreference = sessionStorage.getItem(darkModeKey);
        
        if (sessionPreference === 'true') {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else if (sessionPreference === null) {
            if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                document.documentElement.setAttribute('data-theme', 'dark');
                sessionStorage.setItem(darkModeKey, 'true');
            }
        }
    })();
</script>
```

This approach:
1. **Executes immediately** before CSS loads
2. **Checks session storage** for user's preference
3. **Applies theme instantly** by setting the `data-theme` attribute
4. **Respects system preference** if no session preference exists
5. **Works with the main JavaScript** which handles subsequent toggle interactions

## How It Works

### CSS Variables Approach
The implementation uses CSS custom properties (variables) to define colors for both light and dark themes:

```css
:root {
    /* Light mode colors (default) */
    --bg-color: #ffffff;
    --text-color: #212529;
    /* ... more variables */
}

[data-theme="dark"] {
    /* Dark mode colors */
    --bg-color: #1a1a1a;
    --text-color: #e9ecef;
    /* ... more variables */
}
```

### JavaScript Toggle
The `DarkModeToggle` class handles:
- Loading the current theme preference from sessionStorage
- Toggling between light and dark modes
- Updating the toggle button appearance
- Storing the preference in sessionStorage for the current session

### Theme Application
The theme is applied by setting the `data-theme="dark"` attribute on the `<html>` element, which triggers the CSS variable overrides.

## Usage

### For Users
1. Look for the dark mode toggle button in the top navigation bar
2. Click the button to switch between light and dark modes
3. The button icon and text will update to reflect the current mode:
   - 🌙 Dark (when in light mode - click to go dark)
   - ☀️ Light (when in dark mode - click to go light)

### For Developers

#### Adding Dark Mode Support to New Components
When creating new CSS styles, use the CSS variables instead of hardcoded colors:

```css
/* Good - uses CSS variables */
.my-component {
    background-color: var(--card-bg);
    color: var(--text-color);
    border: 1px solid var(--border-color);
}

/* Bad - hardcoded colors */
.my-component {
    background-color: #ffffff;
    color: #000000;
    border: 1px solid #cccccc;
}
```

#### Available CSS Variables
- `--bg-color` - Main background color
- `--text-color` - Primary text color
- `--navbar-bg` - Navigation bar background
- `--navbar-text` - Navigation bar text
- `--card-bg` - Card/panel backgrounds
- `--border-color` - Border colors
- `--link-color` - Link colors
- `--link-hover-color` - Link hover colors
- `--footer-bg` - Footer background
- `--footer-text` - Footer text color
- `--btn-primary-bg` - Primary button background
- `--input-bg` - Form input backgrounds
- `--table-bg` - Table backgrounds
- `--table-striped-bg` - Striped table row backgrounds
- `--shadow` - Box shadow colors

## Testing

1. Open the test page: `/dark-mode-test.html`
2. Click the dark mode toggle button
3. Verify that all elements change appropriately:
   - Background colors
   - Text colors
   - Form elements
   - Tables
   - Cards
   - Navigation
   - Footer

## Future Enhancements

### Persistence Options
Currently, the dark mode preference is only stored for the current session. Future enhancements could include:

1. **localStorage** - Persist across browser sessions
```javascript
// Instead of sessionStorage
localStorage.setItem('darkMode', isDark.toString());
```

2. **User Profile** - Save preference to user account
3. **System Preference Detection** - Detect OS dark mode preference
```javascript
const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
```

### Additional Features
- Smooth transitions between themes
- Custom color schemes
- Per-page theme overrides
- Accessibility improvements

## Browser Support

This implementation uses:
- CSS Custom Properties (supported in all modern browsers)
- sessionStorage (widely supported)
- HTML5 data attributes (widely supported)

The feature degrades gracefully in older browsers by falling back to the default light theme.

## Performance Considerations

- CSS variables provide excellent performance as they're processed by the browser's CSS engine
- No JavaScript required for theme application once set
- Minimal bundle size impact
- Smooth transitions with CSS animations
