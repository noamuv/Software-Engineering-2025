// API Base URL for all admin endpoints
const API_BASE_URL = '/api/admin';

/**
 * Creates a new user account
 * Called when create user form is submitted
 * Validates input, sends POST request to API, displays result
 */
document.getElementById('createUserForm')?.addEventListener('submit', async function(e) {
    e.preventDefault();
    
    // Get form values
    const userData = {
        User_Type_ID: parseInt(document.getElementById('userType').value),
        Title: document.getElementById('title').value || null,
        First_Name: document.getElementById('firstName').value,
        Last_Name: document.getElementById('lastName').value,
        Email: document.getElementById('email').value,
        Password: document.getElementById('password').value
    };
    
    // Validate required fields
    if (!userData.User_Type_ID || !userData.First_Name || !userData.Last_Name || !userData.Email || !userData.Password) {
        showError('Please fill in all required fields');
        return;
    }
    
    // Disable submit button to prevent double submission
    const submitBtn = document.getElementById('submitBtn');
    submitBtn.disabled = true;
    submitBtn.textContent = 'Creating...';
    
    try {
        // Call API endpoint
        const response = await fetch(`${API_BASE_URL}/create-user`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        });
        
        const data = await response.json();
        
        if (response.ok) {
            // Success - user created
            showSuccess(`User created successfully! User ID: ${data.user_ID}`);
            
            // Reset form
            document.getElementById('createUserForm').reset();
            
            // Scroll to top to see success message
            window.scrollTo({ top: 0, behavior: 'smooth' });
        } else {
            // Error from server (e.g., duplicate email)
            showError(data.message || 'Failed to create user');
        }
        
    } catch (error) {
        // Network error or server down
        console.error('Error:', error);
        showError('Network error. Please check your connection and try again.');
    } finally {
        // Re-enable submit button
        submitBtn.disabled = false;
        submitBtn.textContent = 'Create User';
    }
});

/**
 * Loads all users and displays in table
 * Called when manage users page loads
 */
async function loadUsers() {
    const loadingDiv = document.getElementById('loadingUsers');
    const tableBody = document.getElementById('usersTableBody');
    const noUsersDiv = document.getElementById('noUsers');
    
    // Exit if not on manage users page
    if (!tableBody) return;
    
    // Show loading spinner
    if (loadingDiv) loadingDiv.style.display = 'block';
    
    try {
        // Fetch users from API
        const response = await fetch(`${API_BASE_URL}/users`);
        const users = await response.json();
        
        // Hide loading spinner
        if (loadingDiv) loadingDiv.style.display = 'none';
        
        if (users.length === 0) {
            // No users found - show message
            if (noUsersDiv) noUsersDiv.style.display = 'block';
            tableBody.innerHTML = '';
        } else {
            // Users found - display in table
            if (noUsersDiv) noUsersDiv.style.display = 'none';
            displayUsers(users);
        }
        
    } catch (error) {
        // Error loading users
        console.error('Error loading users:', error);
        if (loadingDiv) loadingDiv.style.display = 'none';
        showError('Failed to load users. Please refresh the page.');
    }
}

/**
 * Displays users in the table
 * @param {Array} users - Array of user objects from API
 */
function displayUsers(users) {
    const tableBody = document.getElementById('usersTableBody');
    
    // Generate table rows
    tableBody.innerHTML = users.map(user => `
        <tr>
            <td>${user.user_ID}</td>
            <td>${user.title || ''} ${user.first_Name} ${user.last_Name}</td>
            <td>${user.email}</td>
            <td>${user.user_Type_Name || 'Unknown'}</td>
            <td>
                <span class="badge-${user.status.toLowerCase()}">
                    ${user.status}
                </span>
            </td>
            <td>
                ${user.status === 'Active' 
                    ? `<button class="btn-danger-custom btn-sm" onclick="disableUser(${user.user_ID})">Disable</button>`
                    : `<button class="btn-success-custom btn-sm" onclick="enableUser(${user.user_ID})">Enable</button>`
                }
            </td>
        </tr>
    `).join('');
}

/**
 * Disables a user account
 * @param {number} userId - ID of user to disable
 */
async function disableUser(userId) {
    // Confirmation dialog
    if (!confirm('Are you sure you want to disable this user account?')) {
        return;
    }
    
    try {
        // Call disable API endpoint
        const response = await fetch(`${API_BASE_URL}/disable-user/${userId}`, {
            method: 'PUT'
        });
        
        const data = await response.json();
        
        if (response.ok) {
            // Success
            showSuccess('User disabled successfully');
            loadUsers(); // Reload table to show updated status
        } else {
            // Error
            showError(data.message || 'Failed to disable user');
        }
        
    } catch (error) {
        console.error('Error:', error);
        showError('Network error. Please try again.');
    }
}

/**
 * Enables a user account
 * @param {number} userId - ID of user to enable
 */
async function enableUser(userId) {
    // Confirmation dialog
    if (!confirm('Are you sure you want to enable this user account?')) {
        return;
    }
    
    try {
        // Call enable API endpoint
        const response = await fetch(`${API_BASE_URL}/enable-user/${userId}`, {
            method: 'PUT'
        });
        
        const data = await response.json();
        
        if (response.ok) {
            // Success
            showSuccess('User enabled successfully');
            loadUsers(); // Reload table to show updated status
        } else {
            // Error
            showError(data.message || 'Failed to enable user');
        }
        
    } catch (error) {
        console.error('Error:', error);
        showError('Network error. Please try again.');
    }
}

/**
 * Shows success message alert
 * @param {string} message - Success message to display
 */
function showSuccess(message) {
    const alert = document.getElementById('successAlert');
    const messageSpan = document.getElementById('successMessage');
    
    if (alert && messageSpan) {
        messageSpan.textContent = message;
        alert.style.display = 'block';
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            alert.style.display = 'none';
        }, 5000);
    }
}

/**
 * Shows error message alert
 * @param {string} message - Error message to display
 */
function showError(message) {
    const alert = document.getElementById('errorAlert');
    const messageSpan = document.getElementById('errorMessage');
    
    if (alert && messageSpan) {
        messageSpan.textContent = message;
        alert.style.display = 'block';
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            alert.style.display = 'none';
        }, 5000);
    }
}

/**
 * Loads dashboard statistics
 * Called when admin dashboard loads
 * Fetches user counts by type and status for dashboard display
 */
async function loadDashboardStats() {
    // Check if we're on the dashboard page
    if (!document.getElementById('totalUsers')) return;
    
    try {
        // Fetch statistics from API
        const response = await fetch(`${API_BASE_URL}/statistics`);
        const stats = await response.json();
        
        if (response.ok) {
            // Update all stat cards with data
            document.getElementById('totalUsers').textContent = stats.totalUsers;
            document.getElementById('activeUsers').textContent = stats.activeUsers;
            document.getElementById('disabledUsers').textContent = stats.disabledUsers;
            document.getElementById('totalAdmins').textContent = stats.totalAdmins;
            document.getElementById('totalClinicians').textContent = stats.totalClinicians;
            document.getElementById('totalPatients').textContent = stats.totalPatients;
            document.getElementById('totalCarers').textContent = stats.totalCarers;
        } else {
            console.error('Failed to load statistics');
            // Keep showing "-" if loading fails
        }
        
    } catch (error) {
        console.error('Error loading statistics:', error);
        // Silently fail - dashboard still usable without stats
    }
}

// Auto-load users when manage users page loads
if (document.getElementById('usersTableBody')) {
    loadUsers();
}

// Auto-load statistics when dashboard page loads
if (document.getElementById('totalUsers')) {
    loadDashboardStats();
}