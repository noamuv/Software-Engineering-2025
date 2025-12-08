  // Show notification after 5 seconds
    setTimeout(function() {
        var notification = document.getElementById('movementPlanNotification');
        notification.classList.remove('hidden');
        console.log('📋 Movement plan notification shown');
    }, 5000);

    // Expand to full modal
    function expandMovementPlan() {
        var notification = document.getElementById('movementPlanNotification');
        var modal = document.getElementById('movementPlanModal');
       
        notification.classList.add('hidden');
        modal.classList.remove('hidden');
       
        console.log('📋 Movement plan expanded');
        startCountdown();
    }

    // Close modal
    function closeMovementPlan() {
        var modal = document.getElementById('movementPlanModal');
        modal.classList.add('hidden');
        console.log('📋 Movement plan closed');
    }

    // Countdown timer
    function startCountdown() {
        var minutes = 24;
        var timerElement = document.getElementById('countdownTimer');
       
        var countdown = setInterval(function() {
            minutes--;
            timerElement.textContent = minutes + ' mins';
           
            if (minutes <= 0) {
                clearInterval(countdown);
                timerElement.textContent = 'Time to move!';
                // Could trigger notification sound here
            }
        }, 60000); // Update every minute
    }

    // Mark movement as done
    function markMovementDone(checkbox) {
        if (checkbox.checked) {
            var item = checkbox.closest('.action-item');
            item.style.background = '#e8ffe8';
            item.style.borderLeftColor = '#4caf50';
            console.log('✅ Movement marked as done');
        } else {
            var item = checkbox.closest('.action-item');
            item.style.background = '#f9f9f9';
            item.style.borderLeftColor = '#5D3FD3';
        }
    }

    // Mark all as done
    function markAllDone() {
        var checkboxes = document.querySelectorAll('.action-checkbox input[type="checkbox"]');
        checkboxes.forEach(function(checkbox) {
            checkbox.checked = true;
            markMovementDone(checkbox);
        });
       
        alert('✅ Great job! All movements marked as complete.');
       
        // Close modal after 1 second
        setTimeout(function() {
            closeMovementPlan();
        }, 1500);
    }

    // Remind later
    function remindLater() {
        alert('⏰ Reminder set for 15 minutes from now.');
        closeMovementPlan();
       
        // Show notification again in 15 minutes
        setTimeout(function() {
            var notification = document.getElementById('movementPlanNotification');
            notification.classList.remove('hidden');
        }, 900000); // 15 minutes = 900000 milliseconds
    }

       let currentPanel = null;

        // Load panel content via AJAX
        function loadPanel(panelType) {
            const panel = document.getElementById('slidePanel');
            const icons = document.querySelectorAll('.sidebar-icon');
            
            // If clicking same panel, close it
            if (currentPanel === panelType && panel.classList.contains('open')) {
                panel.classList.remove('open');
                icons.forEach(icon => icon.classList.remove('active'));
                currentPanel = null;
                return;
            }

            // Mark icon as active
            icons.forEach(icon => icon.classList.remove('active'));
            document.getElementById(panelType + 'Icon').classList.add('active');

            // Load content
            fetch(`/Patient/${panelType.charAt(0).toUpperCase() + panelType.slice(1)}Panel?userId=@Model.Id`)
                .then(response => response.text())
                .then(html => {
                    panel.innerHTML = html;
                    panel.classList.add('open');
                    currentPanel = panelType;
                })
                .catch(error => {
                    console.error('Error loading panel:', error);
                    alert('Failed to load panel');
                });
        }

        // Close panel (called from within partial views)
        function closePanel() {
            const panel = document.getElementById('slidePanel');
            panel.classList.remove('open');
            document.querySelectorAll('.sidebar-icon').forEach(icon => icon.classList.remove('active'));
            currentPanel = null;
        }