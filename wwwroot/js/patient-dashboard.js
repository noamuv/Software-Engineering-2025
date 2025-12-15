
// Patient Dashboard JavaScript
console.log('🚀 patient-dashboard.js loading...');

// ===== HEATMAP =====
function drawHeatmap(matrixData) {
    console.log('drawHeatmap called');
    console.log('Matrix data:', matrixData);
   
    const canvas = document.getElementById('pressureHeatmap');
    if (!canvas) {
        console.error('Canvas element not found!');
        return;
    }
   
    console.log('Canvas found:', canvas);
   
    const ctx = canvas.getContext('2d');
    const cellSize = 12.5; // 400px / 32 = 12.5px per cell
   
    ctx.clearRect(0, 0, canvas.width, canvas.height);
   
    for (let row = 0; row < 32; row++) {
        for (let col = 0; col < 32; col++) {
            const value = matrixData[row][col];
            const color = getPressureColor(value);
           
            ctx.fillStyle = color;
            ctx.fillRect(col * cellSize, row * cellSize, cellSize, cellSize);
           
            ctx.strokeStyle = 'rgba(255, 255, 255, 0.1)';
            ctx.lineWidth = 0.5;
            ctx.strokeRect(col * cellSize, row * cellSize, cellSize, cellSize);
        }
    }
   
    console.log('✅ Heatmap drawn successfully');
}

function getPressureColor(value) {
    if (value === 0) return '#ffffff';
    if (value <= 50) return '#0066cc';
    if (value <= 100) return '#00cc66';
    if (value <= 150) return '#ffcc00';
    if (value <= 200) return '#ff6600';
    return '#cc0000';
}

// ===== TREND GRAPH =====
var trendData = { dates: [], peakPressure: [], contactArea: [], riskScore: [] };
var currentMetric = 'peakPressure';

// Load trend data from sessions
function loadTrendData(sessions) {
    console.log('📊 loadTrendData called');
    console.log('Sessions:', sessions);
   
    // Sort sessions by date
    sessions.sort((a, b) => new Date(a.RecordedDate) - new Date(b.RecordedDate));
   
    /*  The trendData.dates will hold formatted dates for the x-axis
        The other arrays will hold corresponding metric values */

    trendData.dates = sessions.map(s => {
        const d = new Date(s.RecordedDate);
        return (d.getMonth() + 1) + '/' + d.getDate(); // MM/DD format
    });

    // Extract metrics, assuming session has PeakPressure, ContactAreaPercent, RiskScore

    trendData.peakPressure = sessions.map(s => s.PeakPressure);
    trendData.contactArea = sessions.map(s => s.ContactAreaPercent);
    trendData.riskScore = sessions.map(s => s.RiskScore);
   
    console.log('Trend data loaded:', trendData);
   
    drawTrendGraph();
}

// Draw trend graph
function drawTrendGraph() {
    console.log('📈 drawTrendGraph called');
   
    // Get canvas element
    const canvas = document.getElementById('trendGraph');
    if (!canvas) {
        console.error('❌ Graph canvas not found!');
        return;
    }
   
    console.log('✅ Graph canvas found');
   
    const ctx = canvas.getContext('2d'); // 2D context is used for drawing
    const padding = 50;  // Padding around the graph area
    const width = canvas.width - padding * 2; // Graph width
    const height = canvas.height - padding * 2; // Graph height
   
    ctx.clearRect(0, 0, canvas.width, canvas.height); // Clear previous drawings
   
    const data = trendData[currentMetric]; // Get data for the selected metric
    const labels = trendData.dates; // X-axis labels (dates)
   
    if (!data || data.length === 0) {
        ctx.fillStyle = '#666'; // Gray color for text
        ctx.font = '14px Arial'; // Font settings
        ctx.textAlign = 'center';
        ctx.fillText('No data available', canvas.width / 2, canvas.height / 2); // Centered message
        return;
    }
   
    const maxValue = Math.max(...data) * 1.1; // Max value for scaling with 10% padding
   
    // Axes
    ctx.strokeStyle = '#333';  // Dark gray for axes
    ctx.lineWidth = 2;   // Thicker lines for axes
    ctx.beginPath(); // Y-axis
    ctx.moveTo(padding, padding); // X-axis
    ctx.lineTo(padding, canvas.height - padding); // Y-axis
    ctx.lineTo(canvas.width - padding, canvas.height - padding); // X-axis
    ctx.stroke(); // Draw axes
   
    // Grid lines
    ctx.strokeStyle = '#e0e0e0'; // Light gray for grid
    ctx.lineWidth = 1; // Thinner lines for grid
    for (let i = 0; i <= 5; i++) { // For loop to draw horizontal grid lines
        const y = padding + (height / 5) * i; // Calculate y position
        ctx.beginPath();  // Start new path for each line
        ctx.moveTo(padding, y); // Move to left edge
        ctx.lineTo(canvas.width - padding, y); // Draw to right edge
        ctx.stroke(); // Draw the line
       
        // Y-axis labels
        const value = maxValue - (maxValue / 5) * i; // Calculate label value
        ctx.fillStyle = '#666'; // Gray color for text
        ctx.font = '11px Arial'; // Font settings
        ctx.textAlign = 'right'; // Right align for Y-axis labels
        ctx.fillText(Math.round(value), padding - 5, y + 4); // Draw label
    }
   
    // Data line
    ctx.strokeStyle = '#5D3FD3'; // Purple line for data
    ctx.lineWidth = 3; // Thicker line for data
    ctx.beginPath(); // Start path for data line
    
    for (let i = 0; i < data.length; i++) {  // Loop through data points
        const x = padding + (width / (data.length - 1)) * i; // X position on the graph
        const y = canvas.height - padding - (data[i] / maxValue) * height; // Y position on the graph
       
        if (i === 0) { 
            ctx.moveTo(x, y); // Move to first data point
        } else {
            ctx.lineTo(x, y); // Draw line to next data point
        }
    }
    ctx.stroke();
   
    // This is for data points and labels
    for (let i = 0; i < data.length; i++) { // Loop through data points
        const x = padding + (width / (data.length - 1)) * i; // X position
        const y = canvas.height - padding - (data[i] / maxValue) * height; // Y position
       
        ctx.fillStyle = '#5D3FD3'; // Purple fill for data points
        ctx.beginPath();  // Data point circle
        ctx.arc(x, y, 4, 0, Math.PI * 2); // Circle at data point
        ctx.fill(); // Draw data point
       
        // Labels
        ctx.fillStyle = '#666'; // Gray color for text
        ctx.font = '10px Arial'; // Font settings
        ctx.textAlign = 'center'; // Center align for X-axis labels
        ctx.fillText(labels[i], x, canvas.height - padding + 15); // Draw label
    }
   
    console.log('✅ Graph drawn successfully');
}
// Switch metric
    function showMetric(metric) {
    console.log('Switching to metric:', metric); // Log the metric being switched to
    currentMetric = metric; // Update current metric
   
    // Update button styles
    document.querySelectorAll('.graph-btn').forEach(btn => {
        btn.classList.remove('active'); // Remove active class from all buttons
    });
    document.querySelector('[data-metric="' + metric + '"]').classList.add('active'); // Add active class to selected button
   
    // Redraw graph with new metric
    drawTrendGraph();
}

// ===== SIDEBAR PANELS =====
var currentPanel = null;

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

        // Show movement plan notification after 5 seconds
    setTimeout(function() {
        var notification = document.getElementById('movementPlanNotification');
        notification.classList.remove('hidden');
        console.log(' Movement plan notification shown');
    }, 5000);

// ===== INITIALIZE =====
window.addEventListener('load', function() {
    console.log('Window loaded, initializing...');
   
    const matrixEl = document.getElementById('matrixData');
    const sessionsEl = document.getElementById('sessionsData');
   
    console.log('Matrix element:', matrixEl);
    console.log('Sessions element:', sessionsEl);
   
    if (matrixEl) {
        try {
            const matrix = JSON.parse(matrixEl.textContent);
            console.log('Matrix parsed successfully, size:', matrix.length);
            drawHeatmap(matrix);
        } catch (e) {
            console.error('Error parsing matrix:', e);
        }
    } else {
        console.error('matrixData element not found!');
    }
   
    if (sessionsEl) {
        try {
            const sessions = JSON.parse(sessionsEl.textContent);
            console.log('Sessions parsed successfully, count:', sessions.length);
            loadTrendData(sessions);
        } catch (e) {
            console.error(' Error parsing sessions:', e);
        }
    } else {
        console.error('sessionsData element not found!');
    }
});

console.log('patient-dashboard.js loaded');



      

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
