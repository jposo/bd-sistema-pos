document.addEventListener("DOMContentLoaded", async () => {
    // Estado de la aplicación
    const appState = {
        currentOrderId: null,
        selectedTableId: null,
        selectedServerId: 5,
        basketItems: [] // Canasta de platillos
    };

    const basketList = document.querySelector('.basket-item-list');
    const productsGrid = document.getElementById('products-grid');
    const ordersList = document.getElementById('order-item-list');
    const totalCaloriesEl = document.getElementById('total-calories');
    const orderTableEl = document.getElementById('order-table');
    const orderServerEl = document.getElementById('order-server');
    const orderSubtotalEl = document.getElementById('order-subtotal');

    // Fetch Dishes
    const data = await fetch('/Api/GetDishes');
    const products = await data.json();
    products.forEach(product => {
        let productBtn = document.createElement('button');
        productBtn.classList.add('product-item');
        productBtn.innerText = `${product.nombre}\n\$${product.precio}`;
        productBtn.addEventListener('click', () => addToBasket(product));
        productsGrid.appendChild(productBtn);
    });

    // Fetch Orders
    const data2 = await fetch('/Api/GetActiveOrders');
    const orders = await data2.json();
    orders.forEach(order => {
        let orderItem = document.createElement('li');
        orderItem.className = 'order-item';
        orderItem.innerText = `Order #${order.idPedido}`;
        orderItem.addEventListener('click', () => loadOrderDetails(order.idPedido));
        ordersList.appendChild(orderItem);
    });

    // Add to Basket
    function addToBasket(product) {
        const existingItem = appState.basketItems.find(item => item.idPlatillo === product.idPlatillo);

        if (existingItem) {
            existingItem.cantidad += 1;
        } else {
            appState.basketItems.push({
                idPlatillo: product.idPlatillo,
                nombre: product.nombre,
                cantidad: 1,
                especificaciones: '',
                calorias: product.calorias
            });
        }
        renderBasket();
        calculateTotalCalories();
    }

    // Render Basket
    function renderBasket() {
        basketList.innerHTML = '';
        if (appState.basketItems.length === 0) {
            basketList.innerHTML = 'Empty';
            return;
        }
        appState.basketItems.forEach(item => {
            const listItem = document.createElement('li');
            listItem.innerHTML = `
                <span class="basket-item">${item.nombre}</span>
                <span class="basket-quantity">${item.cantidad}</span>
                <button class="remove-item">Remove</button>
            `;
            listItem.querySelector('.remove-item').addEventListener('click', () => {
                appState.basketItems = appState.basketItems.filter(b => b.idPlatillo !== item.idPlatillo);
                renderBasket();
                calculateTotalCalories();
            });
            basketList.appendChild(listItem);
        });
    }

    // Calculate Total Calories
    function calculateTotalCalories() {
        const totalCalories = appState.basketItems.reduce(
            (sum, item) => sum + item.calorias * item.cantidad,
            0
        );
        totalCaloriesEl.textContent = totalCalories;
    }

    // Load Order Details
    async function loadOrderDetails(orderId) {
        const response = await fetch(`/Api/GetOrderDetails?orderId=${orderId}`);
        const order = await response.json();
        orderTableEl.textContent = order.idMesa;
        orderServerEl.textContent = order.mesero;
        orderSubtotalEl.textContent = `$${order.subtotal}`;
        appState.currentOrderId = orderId; // Actualizar estado
    }

    // Create New Order
    document.getElementById('new-order').addEventListener('click', async () => {
        appState.selectedTableId = parseInt(document.getElementById('new-order-table-num').value)
        const orderData = {
            idMesa: appState.selectedTableId, // ID de la mesa seleccionada
            idMesero: appState.selectedServerId // ID del mesero asignado
        };

        console.log(orderData);

        const response = await fetch('/Api/CreateNewOrder', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(orderData)
        });

        const result = await response.json();
        if (response.ok) {
            alert(`Orden creada exitosamente con ID: ${result.idPedido}`);
            appState.currentOrderId = result.idPedido; // Guardar el ID de la orden creada
            location.reload();
        } else {
            alert(`Error: ${result.message}`);
        }
    });

    // Send to Kitchen
    document.getElementById('send-kitchen').addEventListener('click', async () => {
        if (!appState.currentOrderId) {
            alert("Primero crea o seleccione una orden.");
            return;
        }

        const requestData = {
            idPedido: appState.currentOrderId,
            platillos: appState.basketItems.map(item => ({
                idPlatillo: item.idPlatillo,
                cantidad: item.cantidad,
                especificaciones: item.especificaciones || ''
            }))
        };

        console.log(requestData);

        const response = await fetch('/Api/SendToKitchen', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestData)
        });

        const result = await response.json();
        if (response.ok) {
            alert("Platillos enviados a la cocina exitosamente.");
            appState.basketItems = []; // Vaciar la canasta después de enviar
            renderBasket();
        } else {
            alert(`Error: ${result.message}`);
        }
    });
});