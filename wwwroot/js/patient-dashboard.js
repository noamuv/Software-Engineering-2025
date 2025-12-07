    console.log('🚀 Dashboard JavaScript initializing...');

    // Wait for DOM to fully load
    document.addEventListener('DOMContentLoaded', function() {
        console.log('✅ DOM loaded');
        
        const dateSelector = document.getElementById('dateSelector');
        const metricsGrid = document.getElementById('metricsGrid');

        if (!dateSelector) {
            console.error('❌ Date selector not found!');
            return;
        }

        if (!metricsGrid) {
            console.error('❌ Metrics grid not found!');
            return;
        }

        console.log('✅ Elements found, attaching event listener');

        // Add change event listener
        dateSelector.addEventListener('change', function(event) {
            const selectedDate = event.target.value;
            console.log('📅 Date changed to:', selectedDate);
            loadPressureData(selectedDate);
        });

        console.log('✅ Event listener attached successfully');

        function loadPressureData(date) {
            console.log('🔄 Loading data for date:', date);
            
            // Show loading state
            metricsGrid.classList.add('loading');
            dateSelector.disabled = true; // Prevent multiple clicks

            // Build URL
            const url = `/Patient/GetPressureData?userId=@Model.Id&date=${date}`;
            console.log('🌐 Fetching from:', url);

            fetch(url)
                .then(response => {
                    console.log('📡 Response status:', response.status);
                    if (!response.ok) {
                        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                    }
                    return response.json();
                })
                .then(data => {
                    console.log('📦 Received data:', data);

                    // Validate data
                    if (!data.metrics) {
                        throw new Error('Invalid data: missing metrics');
                    }

                    // Update each metric with animation
                    updateMetricWithAnimation('peakPressure', data.metrics.peakPressure, 'mmHg');
                    updateMetricWithAnimation('contactArea', data.metrics.contactArea.toFixed(1), '%');
                    updateMetricWithAnimation('coefficientVariation', data.metrics.cv.toFixed(1), '%');
                    updateMetricWithAnimation('riskScore', data.metrics.riskScore, '/10');

                    console.log('✅ All metrics updated successfully');
                })
                .catch(error => {
                    console.error('❌ Error loading data:', error);
                    alert('Failed to load pressure data:\n' + error.message + '\n\nPlease try again or contact support.');
                })
                .finally(() => {
                    // Remove loading state
                    metricsGrid.classList.remove('loading');
                    dateSelector.disabled = false;
                });
        }

        function updateMetricWithAnimation(elementId, value, unit) {
            const element = document.getElementById(elementId);
            if (!element) {
                console.error(`❌ Element not found: ${elementId}`);
                return;
            }

            // Fade out
            element.style.opacity = '0.3';
            
            // Update after brief delay
            setTimeout(() => {
                element.innerHTML = `${value} <span>${unit}</span>`;
                // Fade in
                element.style.opacity = '1';
                console.log(`✅ Updated ${elementId}: ${value}${unit}`);
            }, 200);
        }
    });