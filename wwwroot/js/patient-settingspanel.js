 function switchTab(tabId, event) {
        document.querySelectorAll('.tab-content').forEach(t => t.classList.remove('active'));
        document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
       
        document.getElementById(tabId).classList.add('active');
        event.target.classList.add('active');
    }

    function changePassword(e) {
        e.preventDefault();
        const form = e.target;
        const newPass = form.newPassword.value;
        const confirmPass = form.confirmPassword.value;

        if (newPass !== confirmPass) {
            alert('Passwords do not match!');
            return;
        }

        // TODO: Backend integration
        alert('Password changed successfully!');
        form.reset();
    }

    function addCarer() {
        const email = document.getElementById('carerEmail').value;
        if (!email) return;

        document.getElementById('carersList').innerHTML = `
            <div class="added-item">
                <div class="added-item-info">
                    <div class="added-item-name">Carer</div>
                    <div class="added-item-email">${email}</div>
                </div>
                <button class="btn-remove" onclick="this.parentElement.remove()">Remove</button>
            </div>
        `;
       
        document.getElementById('carerEmail').value = '';
        alert('Carer added!');
    }

    function addClinician() {
        const email = document.getElementById('clinicianEmail').value;
        if (!email) return;

        document.getElementById('cliniciansList').innerHTML = `
            <div class="added-item">
                <div class="added-item-info">
                    <div class="added-item-name">Clinician</div>
                    <div class="added-item-email">${email}</div>
                </div>
                <button class="btn-remove" onclick="this.parentElement.remove()">Remove</button>
            </div>
        `;
       
        document.getElementById('clinicianEmail').value = '';
        alert('Clinician added!');
    }