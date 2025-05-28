$.post('/Cart/RemoveFromCart', { cartItemId: cartItemId }, function () {
    updateCartPreview();
    updateCartCount();
}); 