   console.log('⚙️ Settings panel loaded');

    let uploadedFiles = [];

    // Tab switching - CRITICAL: No form submission!
   /* function switchSettingsTab(tabName) {
        console.log('🔄 Switching to tab:', tabName);
        
        // Hide all tab contents
        const allTabs = document.querySelectorAll('.tab-content');
        allTabs.forEach(tab => tab.classList.remove('active'));
        
        // Remove active from all tab buttons
        const allButtons = document.querySelectorAll('.tab');
        allButtons.forEach(btn => btn.classList.remove('active'));
        
        // Show selected tab content
        const selectedTab = document.getElementById(tabName + 'Tab');
        if (selectedTab) {
            selectedTab.classList.add('active');
        }
        
        // Mark button as active
        event.target.classList.add('active');
        
        console.log('✅ Switched to', tabName);
    }
*/

function switchSettingsTab(buttonElement, tabName) {
    // 1. Hide all tab contents
    var allTabContents = document.querySelectorAll('.tab-content');
    for (var i = 0; i < allTabContents.length; i++) {
        allTabContents[i].classList.remove('active');
    }
    
    // 2. Remove 'active' status from all buttons
    var allTabButtons = document.querySelectorAll('.tab');
    for (var j = 0; j < allTabButtons.length; j++) {
        allTabButtons[j].classList.remove('active');
    }
    
    // 3. Show selected tab content (makes it appear)
    var selectedTab = document.getElementById(tabName + 'Tab');
    if (selectedTab) {
        selectedTab.classList.add('active'); 
    }
    
    // 4. Mark the clicked button as active (underlines it)
    buttonElement.classList.add('active'); 
}
    // Password change - ONLY runs when form submitted

    /*
    function handlePasswordChange(event) {
        event.preventDefault();
        console.log('🔐 Password change form submitted');
        
        const form = event.target;
        const currentPass = form.currentPassword.value;
        const newPass = form.newPassword.value;
        const confirmPass = form.confirmPassword.value;

        // Validation
        if (newPass !== confirmPass) {
            alert('❌ New passwords do not match!');
            return false;
        }

        if (newPass.length < 6) {
            alert('❌ Password must be at least 6 characters!');
            return false;
        }

        // TODO: Backend integration
        console.log('✅ Password validation passed');
        alert('✅ Password changed successfully!\n(Backend integration pending)');
        form.reset();
        
        return false; // Prevent actual form submission
    }
    */
function handlePasswordChange(event) {
    event.preventDefault();
    event.stopPropagation(); // CRITICAL: Stop event bubbling
   
    console.log('🔐 Password change form submitted');
   
    var form = event.target;
    var currentPass = form.currentPassword.value;
    var newPass = form.newPassword.value;
    var confirmPass = form.confirmPassword.value;

    // Frontend validation
    if (!currentPass || !newPass || !confirmPass) {
        alert('❌ All fields are required');
        return false;
    }

    if (newPass !== confirmPass) {
        alert('❌ New passwords do not match!');
        return false;
    }

    if (newPass.length < 6) {
        alert('❌ Password must be at least 6 characters!');
        return false;
    }

    // Get userId from URL
    var urlParams = new URLSearchParams(window.location.search);
    var userId = urlParams.get('userId');
   
    if (!userId) {
        alert('❌ User ID not found. Please refresh and try again.');
        return false;
    }

    console.log('Sending password change request...');
   
    // Disable submit button to prevent double-click
    var submitBtn = form.querySelector('button[type="submit"]');
    var originalBtnText = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = 'Updating...';

    // Send to backend
    fetch('/Patient/ChangePassword', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            userId: userId,
            currentPassword: currentPass,
            newPassword: newPass
        })
    })
    .then(function(response) {
        console.log('Response status:', response.status);
        return response.json();
    })
    .then(function(data) {
        console.log('Response data:', data);
       
        if (data.success) {
            alert('✅ Password changed successfully! You will use your new password next time you login.');
            form.reset();
        } else {
            alert('❌ ' + (data.message || 'Password update failed'));
        }
    })
    .catch(function(error) {
        console.error('Error:', error);
        alert('❌ An error occurred. Please try again.');
    })
    .finally(function() {
        // Re-enable button
        submitBtn.disabled = false;
        submitBtn.textContent = originalBtnText;
    });

    return false; // CRITICAL: Prevent form submission
}
    // Add carer
    function addCarer() {
        console.log('👥 Add carer clicked');
        const email = document.getElementById('carerEmail').value.trim();
        
        if (!email) {
            alert('❌ Please enter a carer email address');
            return;
        }

        if (!email.includes('@@') === -1 ) {
            alert('❌ Please enter a valid email address');
            return;
        }

        // Add to UI
        const carersList = document.getElementById('carersList');
        carersList.innerHTML = `
            <div class="added-item">
                <div class="added-item-info">
                    <div class="added-item-name">Carer</div>
                    <div class="added-item-email">${email}</div>
                </div>
                <button type="button" class="btn-remove" onclick="this.parentElement.remove()">Remove</button>
            </div>
        `;
        
        document.getElementById('carerEmail').value = '';
        console.log('✅ Carer added:', email);
        alert('✅ Carer added successfully!');
    }

    // Add clinician
    function addClinician() {
        console.log('🩺 Add clinician clicked');
        const email = document.getElementById('clinicianEmail').value.trim();
        
        if (!email) {
            alert('❌ Please enter a clinician email address');
            return;
        }

        if (!email.includes('@@') === -1) {
            alert('❌ Please enter a valid email address');
            return;
        }

        // Add to UI
        const cliniciansList = document.getElementById('cliniciansList');
        cliniciansList.innerHTML = `
            <div class="added-item">
                <div class="added-item-info">
                    <div class="added-item-name">Clinician</div>
                    <div class="added-item-email">${email}</div>
                </div>
                <button type="button" class="btn-remove" onclick="this.parentElement.remove()">Remove</button>
            </div>
        `;
        
        document.getElementById('clinicianEmail').value = '';
        console.log('✅ Clinician added:', email);
        alert('✅ Clinician added successfully!');
    }

    // File upload
    function handleFileUpload(event) {
        console.log('📄 File upload triggered');
        const files = Array.from(event.target.files);
        
        files.forEach(file => {
            // Validate size (10MB)
            if (file.size > 10 * 1024 * 1024) {
                alert(`❌ ${file.name} is too large (max 10MB)`);
                return;
            }

            // Validate type
            const validTypes = ['application/pdf', 'image/jpeg', 'image/png', 'image/jpg', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
            if (!validTypes.includes(file.type)) {
                alert(`❌ ${file.name} is not a supported file type`);
                return;
            }

            uploadedFiles.push(file);
            console.log('✅ File added:', file.name);
        });

        displayUploadedFiles();
        event.target.value = ''; // Reset input
    }

    function displayUploadedFiles() {
        const fileList = document.getElementById('uploadedFilesList');
        
        if (uploadedFiles.length === 0) {
            fileList.innerHTML = '<div class="empty-state">📭 No documents uploaded</div>';
            return;
        }

        fileList.innerHTML = uploadedFiles.map((file, index) => {
            const icon = getFileIcon(file.type);
            const size = formatFileSize(file.size);
            
            return `
                <div class="file-item">
                    <div class="file-info">
                        <div class="file-icon">${icon}</div>
                        <div class="file-details">
                            <div class="file-name">${file.name}</div>
                            <div class="file-size">${size}</div>
                        </div>
                    </div>
                    <button type="button" class="btn-remove" onclick="removeUploadedFile(${index})">Remove</button>
                </div>
            `;
        }).join('');
    }

    function removeUploadedFile(index) {
        if (confirm('Remove this file?')) {
            uploadedFiles.splice(index, 1);
            displayUploadedFiles();
            console.log('✅ File removed');
        }
    }

    function getFileIcon(type) {
        if (type === 'application/pdf') return '📕';
        if (type.startsWith('image/')) return '🖼️';
        if (type.includes('word')) return '📝';
        return '📄';
    }

    function formatFileSize(bytes) {
        if (bytes < 1024) return bytes + ' B';
        if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
        return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
    }