// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Theme Toggle
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('themeToggle');
    const html = document.documentElement;
    const themeIcon = themeToggle.querySelector('i');

    // Check for saved theme preference
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        html.setAttribute('data-bs-theme', savedTheme);
        updateThemeIcon(savedTheme);
    }

    themeToggle.addEventListener('click', function() {
        const currentTheme = html.getAttribute('data-bs-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        html.setAttribute('data-bs-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        updateThemeIcon(newTheme);
    });

    function updateThemeIcon(theme) {
        themeIcon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    }
});

// Cart Animation
function animateAddToCart(productId) {
    const productCard = document.querySelector(`[data-product-id="${productId}"]`);
    const cartIcon = document.querySelector('.cart-preview i');
    
    if (productCard && cartIcon) {
        const productImage = productCard.querySelector('.card-img-top');
        const clone = productImage.cloneNode();
        
        clone.style.position = 'fixed';
        clone.style.width = '100px';
        clone.style.height = '100px';
        clone.style.objectFit = 'cover';
        clone.style.borderRadius = '50%';
        clone.style.zIndex = '1000';
        
        const rect = productImage.getBoundingClientRect();
        const cartRect = cartIcon.getBoundingClientRect();
        
        clone.style.top = rect.top + 'px';
        clone.style.left = rect.left + 'px';
        
        document.body.appendChild(clone);
        
        clone.classList.add('fly-to-cart');
        
        setTimeout(() => {
            document.body.removeChild(clone);
            updateCartCount();
        }, 800);
    }
}

// Update Cart Count
function updateCartCount() {
    $.get('/Cart/GetCartCount', function (response) {
        const count = response.count || 0;
        $('.cart-count').text(count);
        if (count > 0) {
            $('.cart-count').show();
        } else {
            $('.cart-count').hide();
        }
    });
}

// Quick View Modal
function showQuickView(productId) {
    fetch(`/Product/QuickView/${productId}`)
        .then(response => response.text())
        .then(html => {
            const modal = new bootstrap.Modal(document.getElementById('quickViewModal'));
            document.querySelector('#quickViewModal .modal-body').innerHTML = html;
            modal.show();
        });
}

// Search Functionality
const searchInput = document.querySelector('.search-bar input');
if (searchInput) {
    let searchTimeout;
    
    searchInput.addEventListener('input', function() {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
            const query = this.value.trim();
            if (query.length >= 2) {
                fetch(`/Product/Search?query=${encodeURIComponent(query)}`)
                    .then(response => response.text())
                    .then(html => {
                        document.querySelector('.product-grid').innerHTML = html;
                    });
            }
        }, 300);
    });
}

// Filter Functionality
const filterForm = document.querySelector('.filter-form');
if (filterForm) {
    filterForm.addEventListener('change', function() {
        const formData = new FormData(this);
        const params = new URLSearchParams(formData);
        
        fetch(`/Product/Filter?${params.toString()}`)
            .then(response => response.text())
            .then(html => {
                document.querySelector('.product-grid').innerHTML = html;
            });
    });
}

// Price Range Slider
const priceRange = document.querySelector('.price-range input');
if (priceRange) {
    const priceValues = document.querySelector('.price-values');
    
    priceRange.addEventListener('input', function() {
        const min = this.min;
        const max = this.max;
        const value = this.value;
        
        const percentage = ((value - min) / (max - min)) * 100;
        this.style.background = `linear-gradient(to right, var(--f1-red) 0%, var(--f1-red) ${percentage}%, var(--f1-border) ${percentage}%, var(--f1-border) 100%)`;
        
        if (priceValues) {
            priceValues.textContent = `$${value}`;
        }
    });
}

// Format price with currency symbol
function formatPrice(price) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(price);
}

// Update cart total
function updateCartTotal() {
    const cartItems = document.querySelectorAll('.cart-item');
    let total = 0;
    
    cartItems.forEach(item => {
        const price = parseFloat(item.dataset.price);
        const quantity = parseInt(item.querySelector('.quantity-input').value);
        const isOnSale = item.dataset.isOnSale === 'true';
        const salePrice = parseFloat(item.dataset.salePrice);
        
        total += isOnSale ? (salePrice || price) * quantity : price * quantity;
    });
    
    document.querySelector('.cart-total').textContent = formatPrice(total);
}

// Initialize cart functionality
document.addEventListener('DOMContentLoaded', function() {
    const quantityInputs = document.querySelectorAll('.quantity-input');
    quantityInputs.forEach(input => {
        input.addEventListener('change', updateCartTotal);
    });
});

// Product Rating
function submitRating(productId, rating) {
    fetch('/Product/Rate', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ productId, rating })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            updateProductRating(productId, data.averageRating);
        }
    });
}

function updateProductRating(productId, averageRating) {
    const ratingElement = document.querySelector(`[data-product-id="${productId}"] .product-rating`);
    if (ratingElement) {
        ratingElement.innerHTML = generateStarRating(averageRating);
    }
}

function generateStarRating(rating) {
    const fullStars = Math.floor(rating);
    const halfStar = rating % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (halfStar ? 1 : 0);
    
    return `
        ${'<i class="fas fa-star"></i>'.repeat(fullStars)}
        ${halfStar ? '<i class="fas fa-star-half-alt"></i>' : ''}
        ${'<i class="far fa-star"></i>'.repeat(emptyStars)}
        <span class="rating-value">${rating.toFixed(1)}</span>
    `;
}

// Mobile Menu
const mobileMenuToggle = document.querySelector('.navbar-toggler');
if (mobileMenuToggle) {
    mobileMenuToggle.addEventListener('click', function() {
        document.body.classList.toggle('menu-open');
    });
}

// Smooth Scrolling
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Lazy Loading Images
document.addEventListener('DOMContentLoaded', function() {
    const lazyImages = document.querySelectorAll('img[data-src]');
    
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.removeAttribute('data-src');
                observer.unobserve(img);
            }
        });
    });
    
    lazyImages.forEach(img => imageObserver.observe(img));
});

// Cart functionality
function updateCartPreview() {
    $.get('/Cart/GetCartPreview', function (data) {
        $('.cart-items').html(data);
        updateCartTotal();
    });
}

// Initialize cart on page load
$(document).ready(function () {
    updateCartCount();
    updateCartPreview();

    // Handle cart dropdown
    $('#cartDropdown').on('click', function (e) {
        e.preventDefault();
        updateCartPreview();
    });

    // Handle quantity changes
    $(document).on('click', '.quantity-btn', function () {
        const cartItemId = $(this).data('cart-item-id');
        const change = $(this).data('change');
        const quantityInput = $(`#quantity-${cartItemId}`);
        let newQuantity = parseInt(quantityInput.val()) + change;
        
        if (newQuantity < 1) newQuantity = 1;
        if (newQuantity > 99) newQuantity = 99;
        
        quantityInput.val(newQuantity);
        updateCartItem(cartItemId, newQuantity);
    });

    // Handle quantity input changes
    $(document).on('change', '.quantity-input', function () {
        const cartItemId = $(this).data('cart-item-id');
        let newQuantity = parseInt($(this).val());
        
        if (isNaN(newQuantity) || newQuantity < 1) newQuantity = 1;
        if (newQuantity > 99) newQuantity = 99;
        
        $(this).val(newQuantity);
        updateCartItem(cartItemId, newQuantity);
    });

    // Handle remove item
    $(document).on('click', '.remove-item', function () {
        const cartItemId = $(this).data('cart-item-id');
        removeCartItem(cartItemId);
    });
});

function updateCartItem(cartItemId, quantity) {
    $.post('/Cart/UpdateQuantity', { cartItemId: cartItemId, quantity: quantity }, function () {
        updateCartPreview();
        updateCartCount();
    });
}

function removeCartItem(cartItemId) {
    $.post('/Cart/RemoveItem', { cartItemId: cartItemId }, function () {
        updateCartPreview();
        updateCartCount();
    });
}

// Add to cart animation
function addToCart(productId, quantity = 1) {
    $.post('/Cart/AddToCart', { productId: productId, quantity: quantity }, function (response) {
        if (response.success) {
            updateCartCount();
            updateCartPreview();
            showToast('Ürün sepete eklendi', 'success');
        } else {
            showToast(response.message || 'Bir hata oluştu', 'error');
        }
    });
}

// Toast notifications
function showToast(message, type = 'info') {
    const toast = $(`
        <div class="toast" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header">
                <strong class="me-auto">Bildirim</strong>
                <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `);

    $('.toast-container').append(toast);
    const bsToast = new bootstrap.Toast(toast[0]);
    bsToast.show();

    toast.on('hidden.bs.toast', function () {
        toast.remove();
    });
}

// Product image hover effect
$(document).ready(function () {
    $('.product-image').hover(
        function () {
            $(this).find('.product-overlay').fadeIn();
        },
        function () {
            $(this).find('.product-overlay').fadeOut();
        }
    );
});

// Quantity input validation
function validateQuantity(input) {
    let value = parseInt(input.value);
    if (isNaN(value) || value < 1) value = 1;
    if (value > 99) value = 99;
    input.value = value;
}

// Theme switcher
function toggleTheme() {
    const body = document.body;
    const isDark = body.classList.toggle('dark-theme');
    localStorage.setItem('darkTheme', isDark);
}

// Initialize theme
function initTheme() {
    const isDark = localStorage.getItem('darkTheme') === 'true';
    if (isDark) {
        document.body.classList.add('dark-theme');
    }
}

// Initialize on page load
$(document).ready(function() {
    initTheme();
    updateCartCount();
});

// Lazy loading images
document.addEventListener('DOMContentLoaded', function() {
    const lazyImages = [].slice.call(document.querySelectorAll('img.lazy'));

    if ('IntersectionObserver' in window) {
        let lazyImageObserver = new IntersectionObserver(function(entries, observer) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    let lazyImage = entry.target;
                    lazyImage.src = lazyImage.dataset.src;
                    lazyImage.classList.remove('lazy');
                    lazyImageObserver.unobserve(lazyImage);
                }
            });
        });

        lazyImages.forEach(function(lazyImage) {
            lazyImageObserver.observe(lazyImage);
        });
    }
});

// Mobile menu toggle
$('.navbar-toggler').click(function() {
    $('.navbar-collapse').toggleClass('show');
});

// Smooth scroll
$('a[href*="#"]').not('[href="#"]').not('[href="#0"]').click(function(event) {
    if (
        location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') 
        && 
        location.hostname == this.hostname
    ) {
        var target = $(this.hash);
        target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
        if (target.length) {
            event.preventDefault();
            $('html, body').animate({
                scrollTop: target.offset().top
            }, 1000);
        }
    }
});
